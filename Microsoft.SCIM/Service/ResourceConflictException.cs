using System;

namespace Microsoft.SCIM.Service
{
    public class ResourceConflictException : ApplicationException
    {
        public ResourceConflictException() : base()
        {
        }

        public ResourceConflictException(string message) : base(message)
        {
        }

        public ResourceConflictException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
