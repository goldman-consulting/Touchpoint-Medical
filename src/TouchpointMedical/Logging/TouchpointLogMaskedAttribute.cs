using Destructurama.Attributed;

namespace TouchpointMedical.Logging
{
    public sealed class TouchpointLogMaskedAttribute : LogMaskedAttribute
    {
        public TouchpointLogMaskedAttribute() : base()
        {
            Text = "{value not logged}";
        }
    }
}
