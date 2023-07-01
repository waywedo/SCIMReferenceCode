//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.SCIM.Schemas
{
    public class TrustedJsonFactory : JsonFactory
    {
        public override Dictionary<string, object> Create(string json)
        {
            return (Dictionary<string, object>)JsonConvert.DeserializeObject(json);
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
