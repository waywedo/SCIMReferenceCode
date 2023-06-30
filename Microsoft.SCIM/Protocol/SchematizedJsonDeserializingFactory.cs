//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.SCIM
{

    public sealed class SchematizedJsonDeserializingFactory : SchematizedJsonDeserializingFactoryBase
    {
        private ISchematizedJsonDeserializingFactory<PatchRequest2> patchSerializer;

        public override IReadOnlyCollection<IExtension> Extensions { get; set; }

        public override IResourceJsonDeserializingFactory<GroupBase> GroupDeserializationBehavior { get; set; }

        public override ISchematizedJsonDeserializingFactory<PatchRequest2> PatchRequest2DeserializationBehavior
        {
            get { return LazyInitializer.EnsureInitialized(ref patchSerializer, InitializePatchSerializer); }
            set { patchSerializer = value; }
        }

        public override IResourceJsonDeserializingFactory<Core2UserBase> UserDeserializationBehavior { get; set; }

        private Resource CreateGroup(IReadOnlyCollection<string> schemaIdentifiers, IReadOnlyDictionary<string, object> json)
        {
            if (schemaIdentifiers == null)
            {
                throw new ArgumentNullException(nameof(schemaIdentifiers));
            }

            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            if (GroupDeserializationBehavior != null)
            {
                return GroupDeserializationBehavior.Create(json);
            }

            if (schemaIdentifiers.Count != 1)
            {
                throw new ArgumentException(ProtocolResources.ExceptionInvalidResource);
            }

            return new Core2GroupJsonDeserializingFactory().Create(json);
        }

        private Schematized CreatePatchRequest(IReadOnlyDictionary<string, object> json)
        {
            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            if (TryCreatePatchRequest2Legacy(json, out Schematized result))
            {
                return result;
            }

            if (TryCreatePatchRequest2Compliant(json, out result))
            {
                return result;
            }

            throw new InvalidOperationException(ProtocolResources.ExceptionInvalidRequest);
        }

        private Resource CreateUser(IReadOnlyCollection<string> schemaIdentifiers, IReadOnlyDictionary<string, object> json)
        {
            if (schemaIdentifiers == null)
            {
                throw new ArgumentNullException(nameof(schemaIdentifiers));
            }

            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            if (UserDeserializationBehavior != null)
            {
                return UserDeserializationBehavior.Create(json);
            }

            if (schemaIdentifiers.SingleOrDefault(
                item => item.Equals(SchemaIdentifiers.CORE_2_ENTERPRISE_USER, StringComparison.OrdinalIgnoreCase)) != null)
            {
                return new Core2EnterpriseUserJsonDeserializingFactory().Create(json);
            }
            else
            {
                if (schemaIdentifiers.Count != 1)
                {
                    throw new ArgumentException(ProtocolResources.ExceptionInvalidResource);
                }

                return new Core2UserJsonDeserializingFactory().Create(json);
            }
        }

        public override Schematized Create(IReadOnlyDictionary<string, object> json)
        {
            if (json == null)
            {
                return null;
            }

            var normalizedJson = Normalize(json);

            if (!normalizedJson.TryGetValue(AttributeNames.SCHEMAS, out object value))
            {
                throw new ArgumentException(ProtocolResources.ExceptionUnidentifiableSchema);
            }

            IReadOnlyCollection<string> schemaIdentifiers;

            switch (value)
            {
                case IEnumerable schemas:
                    schemaIdentifiers = schemas.ToCollection<string>();
                    break;
                default:
                    throw new ArgumentException(
                        ProtocolResources.ExceptionUnidentifiableSchema);
            }


            if (TryCreateResourceFrom(normalizedJson, schemaIdentifiers, out Schematized result))
            {
                return result;
            }

            if (TryCreateProtocolObjectFrom(normalizedJson, schemaIdentifiers, out result))
            {
                return result;
            }

            if (TryCreateExtensionObjectFrom(normalizedJson, schemaIdentifiers, out result))
            {
                return result;
            }

            var allSchemaIdentifiers = string.Join(Environment.NewLine, schemaIdentifiers);

            throw new NotSupportedException(allSchemaIdentifiers);
        }

        private static ISchematizedJsonDeserializingFactory<PatchRequest2> InitializePatchSerializer()
        {
            return new PatchRequest2JsonDeserializingFactory();
        }

        private bool TryCreateExtensionObjectFrom(IReadOnlyDictionary<string, object> json,
            IReadOnlyCollection<string> schemaIdentifiers, out Schematized schematized)
        {
            schematized = null;

            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            if (schemaIdentifiers == null)
            {
                throw new ArgumentNullException(nameof(schemaIdentifiers));
            }

            if (Extensions == null)
            {
                return false;
            }

            if (Extensions.TryMatch(schemaIdentifiers, out IExtension matchingExtension))
            {
                schematized = matchingExtension.JsonDeserializingFactory(json);
                return true;
            }

            return false;
        }

        private static bool TryCreatePatchRequest2Compliant(IReadOnlyDictionary<string, object> json,
            out Schematized schematized)
        {
            schematized = null;

            try
            {
                var deserializer = new PatchRequest2JsonDeserializingFactory();
                schematized = deserializer.Create(json);
            }
            catch (OutOfMemoryException)
            {
                throw;
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (ThreadInterruptedException)
            {
                throw;
            }
            catch (StackOverflowException)
            {
                throw;
            }
            catch
            {
                return false;
            }

            return true;
        }

        private bool TryCreatePatchRequest2Legacy(IReadOnlyDictionary<string, object> json,
            out Schematized schematized)
        {
            schematized = null;

            try
            {
                var deserializer = PatchRequest2DeserializationBehavior
                    ?? new PatchRequest2JsonDeserializingFactory();

                schematized = deserializer.Create(json);
            }
            catch (OutOfMemoryException)
            {
                throw;
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (ThreadInterruptedException)
            {
                throw;
            }
            catch (StackOverflowException)
            {
                throw;
            }
            catch
            {
                return false;
            }

            return true;
        }

        private bool TryCreateProtocolObjectFrom(IReadOnlyDictionary<string, object> json,
            IReadOnlyCollection<string> schemaIdentifiers, out Schematized schematized)
        {
            schematized = null;

            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            if (schemaIdentifiers == null)
            {
                throw new ArgumentNullException(nameof(schemaIdentifiers));
            }

            if (schemaIdentifiers.Count != 1)
            {
                return false;
            }

            if (schemaIdentifiers.SingleOrDefault(
                    item => item.Equals(ProtocolSchemaIdentifiers.VERSION_2_PATCH_OPERATION, StringComparison.OrdinalIgnoreCase)
                ) != null)
            {
                schematized = CreatePatchRequest(json);
                return true;
            }

            if (schemaIdentifiers.SingleOrDefault(
                item => item.Equals(ProtocolSchemaIdentifiers.VERSION_2_ERROR, StringComparison.OrdinalIgnoreCase)
                ) != null)
            {
                schematized = new ErrorResponseJsonDeserializingFactory().Create(json);
                return true;
            }

            return false;
        }

        private bool TryCreateResourceFrom(IReadOnlyDictionary<string, object> json,
            IReadOnlyCollection<string> schemaIdentifiers, out Schematized schematized)
        {
            schematized = null;

            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            if (schemaIdentifiers == null)
            {
                throw new ArgumentNullException(nameof(schemaIdentifiers));
            }

            if (schemaIdentifiers.SingleOrDefault(
                    item => item.Equals(SchemaIdentifiers.CORE_2_USER, StringComparison.OrdinalIgnoreCase)
                ) != null)
            {
                schematized = CreateUser(schemaIdentifiers, json);
                return true;
            }

            if (schemaIdentifiers.SingleOrDefault(
                item => item.Equals(SchemaIdentifiers.CORE_2_GROUP, StringComparison.OrdinalIgnoreCase)
                ) != null)
            {
                schematized = CreateGroup(schemaIdentifiers, json);
                return true;
            }

            return false;
        }
    }
}
