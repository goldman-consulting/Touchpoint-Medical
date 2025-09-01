
namespace TouchpointMedical.Services
{
    public class TouchpointServiceException : Exception
    {
        public TouchpointServiceException() { }

        public TouchpointServiceException(string message)
            : base(message) { }

        public TouchpointServiceException(string message, Exception innerException)
            : base(message, innerException) { }

    }
}
