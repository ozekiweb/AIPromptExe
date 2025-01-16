using System.Text.Json.Serialization;

namespace AIPrompt
{
    [JsonSerializable(typeof(AIRequest))]
    partial class AIRequestJsonContext : JsonSerializerContext
    {
      
    }
}
