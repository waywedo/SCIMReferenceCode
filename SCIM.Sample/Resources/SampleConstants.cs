// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

namespace SCIM.Sample.Resources
{
    public static class SampleConstants
    {
        public const string CORE_2_SCHEMA_PREFIX = "urn:ietf:params:scim:schemas:core:2.0:";
        public const string USER_ACCOUNT = "User Account";
        public const string SERVICE_PROVIDER_CONFIG = "Service Provider Configuration";
        public const string USER_ENTERPRISE = "Enterprise User";
        public const string USER_ENTERPRISE_NAME = "EnterpriseUser";
        public const string USER_ENTERPRISE_SCHEMA = "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User";
        public const string DESCRIPTION_RESOURCE_TYPE_SCHEMA = "Specifies the schema that describes a SCIM resource type";
        public const string DESCRIPTION_ACTIVE = "A Boolean value indicating the User's administrative status.";
        public const string DESCRIPTION_ADDRESSES = "A physical mailing address for this User. Canonical type" +
            " values of \"work\", \"home\", and \"other\".  This attribute is a complex type with the following sub-attributes.";
        public const string DESCRIPTION_COST_CENTER = "Identifies the name of a cost center.";
        public const string DESCRIPTION_DISPLAY_NAME = "The name of the User, suitable for display to end-users.  " +
            "The name SHOULD be the full name of the User being described, if known.";
        public const string DESCRIPTION_DIVISION = "Identifies the name of a division.";
        public const string DESCRIPTION_DEPARTMENT = "Identifies the name of a department.";
        public const string DESCRIPTION_EMAILS = "Email addresses for the user.  The value SHOULD be canonicalized by " +
            "the service provider, e.g., \"bjensen@example.com\" instead of \"bjensen@EXAMPLE.COM\". Canonical type values " +
            "of \"work\", \"home\", and \"other\".";
        public const string DESCRIPTION_EMPLOYEE_NUMBER = "Numeric or alphanumeric identifier assigned to a person, " +
            "typically based on order of hire or association with an organization.";
        public const string DESCRIPTION_GROUP_DISPLAY_NAME = "A human-readable name for the Group. REQUIRED.";
        public const string DESCRIPTION_LOCALE = "Used to indicate the User's default location for purposes of localizing" +
            " items such as currency, date time format, or numerical representations.";
        public const string DESCRIPTION_MANAGER = "The User's manager.  A complex type that optionally allows " +
            "service providers to represent organizational hierarchy by referencing the \"id\" attribute of another User.";
        public const string DESCRIPTION_MEMBERS = "A list of members of the Group.";
        public const string DESCRIPTION_NAME = "The components of the user's real name. Providers MAY return just " +
            "the full name as a single string in the formatted sub-attribute, or they MAY return just the " +
            "individual component attributes using the other sub-attributes, or they MAY return both.  " +
            "If both variants are returned, they SHOULD be describing the same name, with the formatted name " +
            "indicating how the component attributes should be combined.";
        public const string DESCRIPTION_ORGANIZATION = "Identifies the name of an organization.";
        public const string DESCRIPTION_PHONE_NUMBERS = "Phone numbers for the User.  The value SHOULD be canonicalized" +
            " by the service provider according to the format specified in RFC 3966, e.g., \"tel:+1-201-555-0123\". " +
            "Canonical type values of \"work\", \"home\", \"mobile\", \"fax\", \"pager\", and \"other\".";
        public const string DESCRIPTION_PREFERRED_LANGUAGE = "Indicates the User's preferred written or spoken language.  " +
            "Generally used for selecting a localized user interface; e.g., \"en_US\" specifies the language English and country US.";
        public const string DESCRIPTION_TITLE = "The user's title, such as \"Vice President.\"";
        public const string DESCRIPTION_USER_NAME = "Unique identifier for the User, typically used by the user to " +
            "directly authenticate to the service provider. Each User MUST include a non-empty userName value.  " +
            "This identifier MUST be unique across the service provider's entire set of Users. REQUIRED.";
        public const string DESCRIPTION_USER_TYPE = "Used to identify the relationship between the organization and the user.  " +
            "Typical values used might be \"Contractor\", \"Employee\", \"Intern\", \"Temp\", \"External\", and \"Unknown\", " +
            "but any value may be used.";
        public const string DESCRIPTION_VALUE = "The significant value for the attribute";
        public const string DESCRIPTION_TYPE = "A label indicating the attribute's function";
        public const string DESCRIPTION_FORMATTED_NAME = "The full name, including all middle " +
            "names, titles, and suffixes as appropriate, formatted for display e.g., 'Ms. Barbara J Jensen, III').";
        public const string DESCRIPTION_GIVEN_NAME = "The given name of the User, or" +
            " first name in most Western languages(e.g., 'Barbara' given the full name 'Ms. Barbara J Jensen, III').";
        public const string DESCRIPTION_DISPLAY = "A human-readable name, primarily used for display purposes.READ-ONLY.";
        public const string DESCRIPTION_PRIMARY = "A Boolean value indicating the 'primary' " + " or preferred attribute value for this attribute, " +
            "The primary attribute value 'true' MUST appear no more than once.";
        public const string DESCRIPTION_STREET_ADDRESS = "The full street address component, which may include house number, street name, P.O.box, and multi-line" +
            "extended street address information.This attribute MAY contain newlines.";
        public const string DESCRIPTION_FORMATTED_ADDRESS = "The full mailing address, formatted for display or use with a mailing label." +
            " This attribute MAY contain newlines.";
        public const string DESCRIPTION_FAMILY_NAME = "The family name of the User, or last name in most Western languages(e.g., 'Jensen' given the full " +
            "name 'Ms. Barbara J Jensen, III').";
        public const string DESCRIPTION_HONORIFIC_PREFIX = "The honorific prefix(es) of the User, or title in most Western languages(e.g., 'Ms.' " +
            "given the full name 'Ms. Barbara J Jensen, III').";
        public const string DESCRIPTION_HONORIFIC_SUFFIX = "The honorific suffix(es) of the User, or suffix in most Western languages(e.g., 'III' " +
            "given the full name 'Ms. Barbara J Jensen, III').";
        public const string DESCRIPTION_NICK_NAME = "The casual way to address the user in real life, e.g., 'Bob' or 'Bobby' instead of 'Robert'. " +
            "This attribute SHOULD NOT be used to represent a User's username (e.g., 'bjensen' or 'mpepperidge').";
        public const string DESCRIPTION_TIME_ZONE = "The User's time zone in the 'Olson' time zone database format, e.g., 'America/Los_Angeles'.";
        public const string DESCRIPTION_PASSWORD = "The User's cleartext password.  This attribute is intended to be used as a means to specify an initial " +
            "password when creating a new User or to reset an existing User's password.";
        public const string DESCRIPTION_IMS = "Instant messaging addresses for the User.";
        public const string DESCRIPTION_ROLES = "A list of roles for the User that collectively represent who the User is, e.g., 'Student', 'Faculty'.";
        public const string DESCRIPTION_IDENTIFIER = "The server unique id of a SCIM resource";
        public const string DESCRIPTION_RESOURCE_TYPE_NAME = "The resource type name.  When applicable,service providers MUST specify the name, e.g., 'User'.";
        public const string DESCRIPTION_RESOURCE_TYPE_ENDPOINT = "The resource type's HTTP-addressable endpoint relative to the Base URL, e.g., '/Users'.";
        public const string DESCRIPTION_RESOURCE_TYPE_SCHEMA_ATTRIBUTE = "The resource type's primary/base schema URI.";
        public const string DESCRIPTION_SCIM_SCHEMA = "Specifies the schema that describes a SCIM schema";
        public const string DESCRIPTION_SCHEMA_NAME = "The schema's human-readable name.  When applicable, service providers MUST specify the name, e.g., 'User'.";
        public const string DESCRIPTION_SCHEMA_ATTRIBUTES = "A complex attribute that includes the attributes of a schema.";
        public const string DESCRIPTION_ATTRIBUTE_NAME = "The attribute's name.";
        public const string DESCRIPTION_ATTRIBUTE_TYPE = "The attribute's data type. Valid values include 'string', 'complex', 'boolean', 'decimal', 'integer', 'dateTime', 'reference'.";
        public const string DESCRIPTION_ATTRIBUTE_MULTI_VALUED = "A Boolean value indicating an attribute's plurality.";
        public const string DESCRIPTION_ATTRIBUTE_DESCRIPTION = "A human-readable description of the attribute.";
        public const string DESCRIPTION_ATTRIBUTE_REQUIRED = "A boolean value indicating whether or not the attribute is required.";
        public const string DESCRIPTION_ATTRIBUTE_CANONICAL_VALUES = "A collection of canonical values.  When applicable, service providers MUST specify the canonical types, e.g., 'work', 'home'.";
        public const string DESCRIPTION_ATTRIBUTE_CASE_EXACT = "A Boolean value indicating whether or not a string attribute is case sensitive.";
        public const string DESCRIPTION_ATTRIBUTE_MUTABILITY = "Indicates whether or not an attribute is modifiable.";
        public const string DESCRIPTION_ATTRIBUTE_RETURNED = "Indicates when an attribute is returned in a response(e.g., to a query).";
        public const string DESCRIPTION_ATTRIBUTE_UNIQUENESS = "Indicates how unique a value must be.";
        public const string DESCRIPTION_ATTRIBUTE_REFERENCE_TYPES = "Used only with an attribute of type 'reference'.  Specifies a SCIM resourceType that a reference attribute MAY refer to, e.g., 'User'.";
        public const string DESCRIPTION_ATTRIBUTE_SUB_ATTRIBUTES = "Used to define the sub-attributes of a complex attribute.";
        public const string DESCRIPTION_SERVICE_PROVIDER_CONFIG_SCHEMA = "Schema for representing the service provider's configuration";
        public const string DESCRIPTION_SERVICE_PROVIDER_CONFIG_DOCUMENTATION_URI = "An HTTP-addressable URL pointing to the service provider's human-consumable help documentation.";
        public const string DESCRIPTION_SERVICE_PROVIDER_CONFIG_PATCH = "A complex type that specifies PATCH configuration options.";
        public const string DESCRIPTION_SERVICE_PROVIDER_CONFIG_SUPPORTED = "A Boolean value specifying whether or not the operation is supported.";
        public const string DESCRIPTION_SERVICE_PROVIDER_CONFIG_BULK = "A complex type that specifies bulk configuration options.";
        public const string DESCRIPTION_SERVICE_PROVIDER_CONFIG_ETAG = "A complex type that specifies ETag configuration options.REQUIRED.";
        public const string DESCRIPTION_SERVICE_PROVIDER_CONFIG_BULK_MAX_OPERATIONS = "An integer value specifying the maximum number of operations.";
        public const string DESCRIPTION_SERVICE_PROVIDER_CONFIG_BULK_MAX_PAYLOAD_SIZE = "An integer value specifying the maximum payload size in bytes.";
        public const string DESCRIPTION_SERVICE_PROVIDER_CONFIG_FILTER = "A complex type that specifies FILTER options.";
        public const string DESCRIPTION_SERVICE_PROVIDER_CONFIG_FILTER_MAX_RESULTS = "An integer value specifying the maximum number of resources returned in a response.";
        public const string DESCRIPTION_SERVICE_PROVIDER_CONFIG_CHANGE_PASSWORD = "A complex type that specifies configuration options related to changing a password.";
        public const string DESCRIPTION_SERVICE_PROVIDER_CONFIG_SORT = "A complex type that specifies sort result options.";
        public const string DESCRIPTION_SERVICE_PROVIDER_AUTHENTICATION_SCHEMES = "A complex type that specifies supported authentication scheme properties.";
        public const string DESCRIPTION_SERVICE_PROVIDER_AUTHENTICATION_SCHEMES_NAME = "The common authentication scheme name, e.g., HTTP Basic.";
        public const string DESCRIPTION_SERVICE_PROVIDER_AUTHENTICATION_SCHEMES_DESCRIPTION = "A description of the authentication scheme.";
        public const string DESCRIPTION_SERVICE_PROVIDER_AUTHENTICATION_SCHEMES_SPEC_URI = "An HTTP-addressable URL pointing to the authentication scheme's specification.";
        public const string DESCRIPTION_SERVICE_PROVIDER_AUTHENTICATION_SCHEMES_DOCUMENTATION_URI = "An HTTP-addressable URL pointing to the authentication scheme's usage documentation.";
        public const string DESCRIPTION_ADDRESS_COUNTRY = "The country name component.";
        public const string DESCRIPTION_ADDRESS_LOCALITY = "The city or locality component.";
        public const string DESCRIPTION_ADDRESS_POSTAL_CODE = "The country name component.";
        public const string DESCRIPTION_ADDRESS_REGION = "The state or region component.";
        public const string DESCRIPTION_ADDRESS_TYPE = "A label indicating the attribute's function, e.g., 'work' or 'home'.";
        public const string SAMPLE_SCIM_ENDPOINT = "http://localhost:5000/scim";
    }
}
