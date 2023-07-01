//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Microsoft.SCIM.Schemas.Contracts;
using Newtonsoft.Json;

namespace Microsoft.SCIM.Schemas
{
    internal class JsonSerializer : IJsonSerializable
    {
        private static readonly Lazy<DataContractJsonSerializerSettings> SerializerSettings =
            new(() => new DataContractJsonSerializerSettings() { EmitTypeInformation = EmitTypeInformation.Never });

        private readonly object _dataContractValue;

        public JsonSerializer(object dataContract)
        {
            _dataContractValue = dataContract ??
                throw new ArgumentNullException(nameof(dataContract));
        }

        public string Serialize()
        {
            var json = ToJson();
            return JsonConvert.SerializeObject(json);
        }

        public Dictionary<string, object> ToJson()
        {
            var type = _dataContractValue.GetType();
            var serializer = new DataContractJsonSerializer(type, SerializerSettings.Value);

            string json;
            MemoryStream stream = null;

            try
            {
                stream = new MemoryStream();
                serializer.WriteObject(stream, _dataContractValue);
                stream.Position = 0;
                StreamReader streamReader = null;

                try
                {
                    streamReader = new StreamReader(stream);
                    stream = null;
                    json = streamReader.ReadToEnd();
                }
                finally
                {
                    if (streamReader != null)
                    {
                        streamReader.Close();
                        streamReader = null;
                    }
                }
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
#pragma warning disable IDE0059 // Unnecessary assignment of a value
                    stream = null;
#pragma warning restore IDE0059 // Unnecessary assignment of a value
                }
            }

            return JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        }
    }
}