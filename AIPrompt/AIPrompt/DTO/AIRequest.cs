using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AIPrompt
{
    public class AIRequest
    {
        [JsonPropertyName("model")]
        public required string Model { get; set; }
        [JsonPropertyName("messages")]
        public required List<Message> Messages { get; set; }
    }
}
