using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AIPrompt
{
    public class Message
    {
        [JsonPropertyName("role")]
        public required string Role { get; set; }
        [JsonPropertyName("content")]
        public required string Content { get; set; }      
    }
}
