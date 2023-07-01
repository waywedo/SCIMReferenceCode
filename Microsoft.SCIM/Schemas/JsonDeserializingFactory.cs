//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading;
using Microsoft.SCIM.Schemas.Contracts;

namespace Microsoft.SCIM.Schemas
{
    public abstract class JsonDeserializingFactory<TDataContract> : IJsonNormalizationBehavior
    {
        private static readonly Lazy<DataContractJsonSerializerSettings> _jsonSerializerSettings =
            new(() => new DataContractJsonSerializerSettings() { EmitTypeInformation = EmitTypeInformation.Never });

        private static readonly Lazy<DataContractJsonSerializer> _jsonSerializer =
            new(() => new DataContractJsonSerializer(typeof(TDataContract), _jsonSerializerSettings.Value));

        private IJsonNormalizationBehavior _jsonNormalizer;

        public bool AcceptLargeObjects { get; set; }

        public virtual IJsonNormalizationBehavior JsonNormalizer
        {
            get
            {
                return LazyInitializer.EnsureInitialized(ref _jsonNormalizer, () => new JsonNormalizer());
            }
        }

        public virtual TDataContract Create(IReadOnlyDictionary<string, object> json)
        {
            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            var normalizedJson = Normalize(json);
            var serialized = JsonFactory.Instance.Create(normalizedJson, AcceptLargeObjects);

            MemoryStream stream = null;

            try
            {
                stream = new MemoryStream();
                var streamed = stream;
                StreamWriter writer = null;
                try
                {
                    writer = new StreamWriter(stream);
                    stream = null;
                    writer.Write(serialized);
                    writer.Flush();

                    streamed.Position = 0;

                    return (TDataContract)_jsonSerializer.Value.ReadObject(streamed);
                }
                finally
                {
                    if (writer != null)
                    {
                        writer.Close();
                        writer = null;
                    }
                }
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
#pragma warning disable IDE0059 // Value assigned to symbol is never used
                    stream = null;
#pragma warning restore IDE0059 // Value assigned to symbol is never used
                }
            }
        }

        public virtual TDataContract Create(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentNullException(nameof(json));
            }

            var keyedValues = JsonFactory.Instance.Create(json, AcceptLargeObjects);

            return Create(keyedValues);
        }

        public IReadOnlyDictionary<string, object> Normalize(IReadOnlyDictionary<string, object> json)
        {
            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            return JsonNormalizer.Normalize(json);
        }

        public virtual TDataContract Read(string json)
        {
            MemoryStream stream = null;

            try
            {
                stream = new MemoryStream();
                var streamed = stream;
                StreamWriter writer = null;
                try
                {
                    writer = new StreamWriter(stream);
                    stream = null;
                    writer.Write(json);
                    writer.Flush();

                    streamed.Position = 0;

                    return (TDataContract)_jsonSerializer.Value.ReadObject(streamed);
                }
                finally
                {
                    if (writer != null)
                    {
                        writer.Close();
                        writer = null;
                    }
                }
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
#pragma warning disable IDE0059 // Value assigned to symbol is never used
                    stream = null;
#pragma warning restore IDE0059 // Value assigned to symbol is never used
                }
            }
        }
    }
}