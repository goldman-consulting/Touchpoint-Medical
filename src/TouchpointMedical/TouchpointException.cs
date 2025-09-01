
namespace TouchpointMedical
{
    public class TouchpointException : Exception
    {
        public TouchpointException() { }

        public TouchpointException(string message)
            : base(message) { }

        public TouchpointException(string message, Exception innerException)
            : base(message, innerException) { }

    }
}
