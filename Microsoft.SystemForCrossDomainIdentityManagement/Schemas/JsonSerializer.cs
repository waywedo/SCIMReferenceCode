//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Microsoft.SCIM
{
    internal class JsonSerializer : IJsonSerializable
    {
        private static readonly Lazy<DataContractJsonSerializerSettings> _serializerSettings =
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
            return JsonFactory.Instance.Create(json, true);
        }

        public Dictionary<string, object> ToJson()
        {
            var type = _dataContractValue.GetType();
            var serializer = new DataContractJsonSerializer(type, _serializerSettings.Value);

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
                    stream = null;
                }
            }

            return JsonFactory.Instance.Create(json, true);
        }
    }
}