
namespace TouchpointMedical.Json
{
    public class TouchpointSerializationException : Exception
    {
        public TouchpointSerializationException() { }

        public TouchpointSerializationException(string message)
            : base(message) { }

        public TouchpointSerializationException(string message, Exception innerException)
            : base(message, innerException) { }

    }
}
