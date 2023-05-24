//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Microsoft.SCIM
{
    [DataContract]
    public abstract class Core2UserBase : UserBase
    {
        private IDictionary<string, IDictionary<string, object>> _customExtension;

        protected Core2UserBase()
        {
            AddSchema(SchemaIdentifiers.CORE_2_USER);
            Metadata = new Core2Metadata()
            {
                ResourceType = Types.USER
            };
            OnInitialization();
        }

        [DataMember(Name = AttributeNames.ACTIVE)]
        public virtual bool Active { get; set; }

        [DataMember(Name = AttributeNames.ADDRESSES, IsRequired = false, EmitDefaultValue = false)]
        public virtual IEnumerable<Address> Addresses { get; set; }

        public virtual IReadOnlyDictionary<string, IDictionary<string, object>> CustomExtension
        {
            get { return new ReadOnlyDictionary<string, IDictionary<string, object>>(_customExtension); }
        }

        [DataMember(Name = AttributeNames.DISPLAY_NAME, IsRequired = false, EmitDefaultValue = false)]
        public virtual string DisplayName { get; set; }

        [DataMember(Name = AttributeNames.ELECTRONIC_MAIL_ADDRESSES, IsRequired = false, EmitDefaultValue = false)]
        public virtual IEnumerable<ElectronicMailAddress> ElectronicMailAddresses { get; set; }

        [DataMember(Name = AttributeNames.IMS, IsRequired = false, EmitDefaultValue = false)]
        public virtual IEnumerable<InstantMessaging> InstantMessagings { get; set; }

        [DataMember(Name = AttributeNames.LOCALE, IsRequired = false, EmitDefaultValue = false)]
        public virtual string Locale { get; set; }

        [DataMember(Name = AttributeNames.METADATA)]
        public virtual Core2Metadata Metadata { get; set; }

        [DataMember(Name = AttributeNames.NAME, IsRequired = false, EmitDefaultValue = false)]
        public virtual Name Name { get; set; }

        [DataMember(Name = AttributeNames.NICKNAME, IsRequired = false, EmitDefaultValue = false)]
        public virtual string Nickname { get; set; }

        [DataMember(Name = AttributeNames.PHONE_NUMBERS, IsRequired = false, EmitDefaultValue = false)]
        public virtual IEnumerable<PhoneNumber> PhoneNumbers { get; set; }

        [DataMember(Name = AttributeNames.PREFERRED_LANGUAGE, IsRequired = false, EmitDefaultValue = false)]
        public virtual string PreferredLanguage { get; set; }

        [DataMember(Name = AttributeNames.ROLES, IsRequired = false, EmitDefaultValue = false)]
        public virtual IEnumerable<Role> Roles { get; set; }

        [DataMember(Name = AttributeNames.TIME_ZONE, IsRequired = false, EmitDefaultValue = false)]
        public virtual string TimeZone { get; set; }

        [DataMember(Name = AttributeNames.TITLE, IsRequired = false, EmitDefaultValue = false)]
        public virtual string Title { get; set; }

        [DataMember(Name = AttributeNames.USER_TYPE, IsRequired = false, EmitDefaultValue = false)]
        public virtual string UserType { get; set; }

        public virtual void AddCustomAttribute(string key, object value)
        {
            if (key?.StartsWith(SchemaIdentifiers.PREFIX_EXTENSION, StringComparison.OrdinalIgnoreCase) == true
                && !key.StartsWith(SchemaIdentifiers.CORE_2_ENTERPRISE_USER, StringComparison.OrdinalIgnoreCase)
                && value is Dictionary<string, object> nestedObject)
            {
                _customExtension.Add(key, nestedObject);
            }
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext _)
        {
            OnInitialization();
        }

        private void OnInitialization()
        {
            _customExtension = new Dictionary<string, IDictionary<string, object>>();
        }

        public override Dictionary<string, object> ToJson()
        {
            var result = base.ToJson();

            foreach (var entry in CustomExtension)
            {
                result.Add(entry.Key, entry.Value);
            }

            return result;
        }
    }
}