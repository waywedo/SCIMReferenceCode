//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.SCIM.Schemas
{
    using Microsoft.SCIM;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    public abstract class JsonFactory
    {
        private static readonly Lazy<JsonFactory> LargeObjectFactory =
            new(() => new TrustedJsonFactory());

        private static readonly Lazy<JsonFactory> Singleton =
            new(() => InitializeFactory());

        public static JsonFactory Instance
        {
            get { return Singleton.Value; }
        }

        public abstract Dictionary<string, object> Create(string json);

        public virtual Dictionary<string, object> Create(string json, bool acceptLargeObjects)
        {
            return acceptLargeObjects ? LargeObjectFactory.Value.Create(json) : Create(json);
        }

        public abstract string Create(string[] json);

        public virtual string Create(string[] json, bool acceptLargeObjects)
        {
            return acceptLargeObjects ? LargeObjectFactory.Value.Create(json) : Create(json);
        }

        public abstract string Create(Dictionary<string, object> json);

        public virtual string Create(Dictionary<string, object> json, bool acceptLargeObjects)
        {
            return acceptLargeObjects ? LargeObjectFactory.Value.Create(json) : Create(json);
        }

        public abstract string Create(IDictionary<string, object> json);

        public virtual string Create(IDictionary<string, object> json, bool acceptLargeObjects)
        {
            return acceptLargeObjects ? LargeObjectFactory.Value.Create(json) : Create(json);
        }

        public abstract string Create(IReadOnlyDictionary<string, object> json);

        public virtual string Create(IReadOnlyDictionary<string, object> json, bool acceptLargeObjects)
        {
            return acceptLargeObjects ? LargeObjectFactory.Value.Create(json) : Create(json);
        }

        private static JsonFactory InitializeFactory()
        {
            //FIXME:Cannot see the difference between the two
            //return ConfigurationSection.Instance.AcceptLargeObjects
            //    ? new TrustedJsonFactory()
            //    : new Implementation();
            return new TrustedJsonFactory();
        }

        private class Implementation : JsonFactory
        {
            public override Dictionary<string, object> Create(string json)
            {
                return JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            }

            public override string Create(string[] json)
            {
                return JsonConvert.SerializeObject(json);
            }

            public override string Create(Dictionary<string, object> json)
            {
                return JsonConvert.SerializeObject(json);
            }

            public override string Create(IDictionary<string, object> json)
            {
                return JsonConvert.SerializeObject(json);
            }

            public override string Create(IReadOnlyDictionary<string, object> json)
            {
                return JsonConvert.SerializeObject(json);
            }
        }
    }
}
