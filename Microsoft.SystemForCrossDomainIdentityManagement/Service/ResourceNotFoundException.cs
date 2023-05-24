using System;

namespace Microsoft.SCIM.Service
{
    public class ResourceNotFoundException : ApplicationException
    {
        public ResourceNotFoundException() : base()
        {
        }

        public ResourceNotFoundException(string message) : base(message)
        {
        }

        public ResourceNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
