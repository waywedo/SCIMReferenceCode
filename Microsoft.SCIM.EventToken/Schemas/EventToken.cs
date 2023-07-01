//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using Microsoft.SCIM.Resources;
using Microsoft.SCIM.Schemas;

namespace Microsoft.SCIM.EventToken.Schemas
{
    // Implements https://tools.ietf.org/html/draft-ietf-secevent-token
    public class EventToken : IEventToken
    {
        public const string HEADER_KEY_ALGORITHM = "alg";
        public const string JWT_ALGORITHM_NONE = "none";

        private static readonly Lazy<JwtHeader> HeaderDefault =
            new(() => ComposeDefaultHeader());

        private static readonly Lazy<SecurityTokenHandler> TokenSerializer =
            new(() => new JwtSecurityTokenHandler());

        private EventToken(string issuer, JwtHeader header)
        {
            if (string.IsNullOrWhiteSpace(issuer))
            {
                throw new ArgumentNullException(nameof(issuer));
            }

            Issuer = issuer;
            Header = header ?? throw new ArgumentNullException(nameof(header));

            Identifier = Guid.NewGuid().ToString();
            IssuedAt = DateTime.UtcNow;
        }

        public EventToken(string issuer, JwtHeader header, IDictionary<string, object> events) : this(issuer, header)
        {
            Events = events ?? throw new ArgumentNullException(nameof(events));
        }

        public EventToken(string issuer, Dictionary<string, object> events) : this(issuer, HeaderDefault.Value, events)
        {
        }

        public EventToken(string serialized)
        {
            if (string.IsNullOrWhiteSpace(serialized))
            {
                throw new ArgumentNullException(nameof(serialized));
            }

            var token = new JwtSecurityToken(serialized);

            Header = token.Header;

            ParseIdentifier(token.Payload);
            ParseIssuer(token.Payload);
            ParseAudience(token.Payload);
            ParseIssuedAt(token.Payload);
            ParseNotBefore(token.Payload);
            ParseSubject(token.Payload);
            ParseExpiration(token.Payload);
            ParseEvents(token.Payload);
            ParseTransaction(token.Payload);
        }

        public IReadOnlyCollection<string> Audience { get; set; }

        public IDictionary<string, object> Events { get; private set; }

        public DateTime? Expiration { get; set; }

        public JwtHeader Header { get; }

        public string Identifier { get; private set; }

        public DateTime IssuedAt { get; private set; }

        public string Issuer { get; private set; }

        public DateTime? NotBefore { get; private set; }

        public string Subject { get; set; }

        public string Transaction { get; set; }

        private static JwtHeader ComposeDefaultHeader()
        {
            return new JwtHeader
            {
                { HEADER_KEY_ALGORITHM, JWT_ALGORITHM_NONE }
            };
        }

        private void ParseAudience(JwtPayload payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            if (!payload.TryGetValue(EventTokenClaimTypes.AUDIENCE, out object value) || value == null)
            {
                return;
            }

            if (value is not object[] values)
            {
                var exceptionMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    SchemasResources.ExceptionEventTokenInvalidClaimValueTemplate,
                    EventTokenClaimTypes.AUDIENCE,
                    value
                );
                throw new ArgumentException(exceptionMessage);
            }

            IReadOnlyCollection<string> audience = values.OfType<string>().ToArray();

            if (audience.Count != values.Length)
            {
                var exceptionMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    SchemasResources.ExceptionEventTokenInvalidClaimValueTemplate,
                    EventTokenClaimTypes.AUDIENCE,
                    value
                );
                throw new ArgumentException(exceptionMessage);
            }

            Audience = audience;
        }

        private void ParseEvents(JwtPayload payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            if (!payload.TryGetValue(EventTokenClaimTypes.EVENTS, out object value) || value == null)
            {
                var exceptionMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    SchemasResources.ExceptionEventTokenMissingClaimTemplate,
                    EventTokenClaimTypes.EVENTS
                );
                throw new ArgumentException(exceptionMessage);
            }

            IDictionary<string, object> events = value as Dictionary<string, object>;

            if (events == null)
            {
                var exceptionMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    SchemasResources.ExceptionEventTokenInvalidClaimValueTemplate,
                    EventTokenClaimTypes.EVENTS,
                    value
                );
                throw new ArgumentException(exceptionMessage);
            }
            Events = events;
        }

        private void ParseExpiration(JwtPayload payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            if (!payload.TryGetValue(EventTokenClaimTypes.EXPIRATION, out object value) || value == null)
            {
                return;
            }

            var serializedValue = value.ToString();

            if (!long.TryParse(serializedValue, out long expiration))
            {
                var exceptionMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    SchemasResources.ExceptionEventTokenInvalidClaimValueTemplate,
                    EventTokenClaimTypes.EXPIRATION,
                    value
                );
                throw new ArgumentException(exceptionMessage);
            }

            Expiration = new UnixTime(expiration).ToUniversalTime();

            if (Expiration > DateTime.UtcNow)
            {
                throw new SecurityTokenExpiredException(SchemasResources.ExceptionEventTokenExpired);
            }
        }

        private void ParseIdentifier(JwtPayload payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            if (!payload.TryGetValue(EventTokenClaimTypes.IDENTIFIER, out object value) || value == null)
            {
                var exceptionMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    SchemasResources.ExceptionEventTokenMissingClaimTemplate,
                    EventTokenClaimTypes.IDENTIFIER
                );
                throw new ArgumentException(exceptionMessage);
            }

            var identifier = value as string;

            if (string.IsNullOrWhiteSpace(identifier))
            {
                var exceptionMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    SchemasResources.ExceptionEventTokenInvalidClaimValueTemplate,
                    EventTokenClaimTypes.IDENTIFIER,
                    value
                );
                throw new ArgumentException(exceptionMessage);
            }

            Identifier = identifier;
        }

        private void ParseIssuedAt(JwtPayload payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            if (!payload.TryGetValue(EventTokenClaimTypes.ISSUED_AT, out object value) || value == null)
            {
                var exceptionMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    SchemasResources.ExceptionEventTokenMissingClaimTemplate,
                    EventTokenClaimTypes.ISSUED_AT
                );
                throw new ArgumentException(exceptionMessage);
            }

            var serializedValue = value.ToString();

            if (!long.TryParse(serializedValue, out long issuedAt))
            {
                var exceptionMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    SchemasResources.ExceptionEventTokenMissingClaimTemplate,
                    EventTokenClaimTypes.ISSUED_AT
                );
                throw new ArgumentException(exceptionMessage);
            }

            IssuedAt = new UnixTime(issuedAt).ToUniversalTime();
        }

        private void ParseIssuer(JwtPayload payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            if (!payload.TryGetValue(EventTokenClaimTypes.ISSUER, out object value) || value == null)
            {
                var exceptionMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    SchemasResources.ExceptionEventTokenMissingClaimTemplate,
                    EventTokenClaimTypes.ISSUER
                );
                throw new ArgumentException(exceptionMessage);
            }

            var issuer = value as string;

            if (string.IsNullOrWhiteSpace(issuer))
            {
                var exceptionMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    SchemasResources.ExceptionEventTokenInvalidClaimValueTemplate,
                    EventTokenClaimTypes.ISSUER,
                    value
                );
                throw new ArgumentException(exceptionMessage);
            }
            Issuer = issuer;
        }

        private void ParseNotBefore(JwtPayload payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            if (!payload.TryGetValue(EventTokenClaimTypes.NOT_BEFORE, out object value) || value == null)
            {
                return;
            }

            var serializedValue = value.ToString();

            if (!long.TryParse(serializedValue, out long notBefore))
            {
                var exceptionMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    SchemasResources.ExceptionEventTokenInvalidClaimValueTemplate,
                    EventTokenClaimTypes.NOT_BEFORE,
                    value
                );
                throw new ArgumentException(exceptionMessage);
            }

            NotBefore = new UnixTime(notBefore).ToUniversalTime();
        }

        private void ParseSubject(JwtPayload payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            if (!payload.TryGetValue(EventTokenClaimTypes.SUBJECT, out object value) || value == null)
            {
                return;
            }

            if (value is not string subject)
            {
                var exceptionMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    SchemasResources.ExceptionEventTokenInvalidClaimValueTemplate,
                    EventTokenClaimTypes.SUBJECT,
                    value
                );
                throw new ArgumentException(exceptionMessage);
            }

            Subject = subject;
        }

        private void ParseTransaction(JwtPayload payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            if (!payload.TryGetValue(EventTokenClaimTypes.TRANSACTION, out object value) || value == null)
            {
                return;
            }

            if (value is not string transaction)
            {
                var exceptionMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    SchemasResources.ExceptionEventTokenInvalidClaimValueTemplate,
                    EventTokenClaimTypes.TRANSACTION,
                    value
                );
                throw new ArgumentException(exceptionMessage);
            }

            Transaction = transaction;
        }

        public override string ToString()
        {
            var payload = new JwtPayload
            {
                { EventTokenClaimTypes.IDENTIFIER, Identifier },
                { EventTokenClaimTypes.ISSUER, Issuer }
            };

            if (Audience?.Any() == true)
            {
                var audience = Audience.ToArray();
                payload.Add(EventTokenClaimTypes.AUDIENCE, audience);
            }

            var issuedAt = new UnixTime(IssuedAt).EpochTimestamp;
            payload.Add(EventTokenClaimTypes.ISSUED_AT, issuedAt);

            if (NotBefore.HasValue)
            {
                var notBefore = new UnixTime(NotBefore.Value).EpochTimestamp;
                payload.Add(EventTokenClaimTypes.NOT_BEFORE, notBefore);
            }

            if (!string.IsNullOrWhiteSpace(Subject))
            {
                payload.Add(EventTokenClaimTypes.SUBJECT, Subject);
            }

            if (Expiration.HasValue)
            {
                var expiration = new UnixTime(Expiration.Value).EpochTimestamp;
                payload.Add(EventTokenClaimTypes.EXPIRATION, expiration);
            }

            payload.Add(EventTokenClaimTypes.EVENTS, Events);

            if (!string.IsNullOrWhiteSpace(Transaction))
            {
                payload.Add(EventTokenClaimTypes.TRANSACTION, Transaction);
            }

            var token = new JwtSecurityToken(Header, payload);

            return TokenSerializer.Value.WriteToken(token);
        }
    }
}