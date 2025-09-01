using System.Text.Json;
using System.Text.Json.Serialization;

namespace TouchpointMedical.Integration.PointClickCare.Infrastructure
{
    public class PointClickCareWebhookConverter : JsonConverter<PointClickCareWebhookMessage>
    {
        public override PointClickCareWebhookMessage? Read(
            ref Utf8JsonReader reader, 
            Type typeToConvert, 
            JsonSerializerOptions options)
        {
            // Let the default serializer handle this (custom deserialization optional)
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            return new PointClickCareWebhookMessage
            {
                MessageId = root.GetProperty("messageId").GetString()!,
                Type = root.GetProperty("eventType").GetString()!,
                ResidentId = root.GetProperty("patientId").GetInt64(),
                FacilityId = root.GetProperty("facId").GetInt64(),
                OrgUuid  = root.GetProperty("orgUuid").GetString()!
            };
        }

        public override void Write(
            Utf8JsonWriter writer, 
            PointClickCareWebhookMessage value, 
            JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            // Rename the field during serialization
            writer.WriteString("MessageId", value.MessageId);
            writer.WriteString("Type", value.Type);
            //writer.WriteString("Payload", value.Payload);
            writer.WriteString("ReceivedAt", value.MessageReceivedAt);

            writer.WriteEndObject();
        }
    }
}
