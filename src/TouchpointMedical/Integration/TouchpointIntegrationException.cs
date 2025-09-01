
namespace TouchpointMedical.Integration
{
    public class TouchpointIntegrationException : Exception
    {
        public TouchpointIntegrationException() { }

        public TouchpointIntegrationException(string message)
            : base(message) { }

        public TouchpointIntegrationException(string message, Exception innerException)
            : base(message, innerException) { }

    }
}
