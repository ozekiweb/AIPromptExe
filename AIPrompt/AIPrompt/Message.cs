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

        public override string? ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Role: ")
                .AppendLine(Role)
                .Append("Content: ")
                .AppendLine(Content);
            var formatted = sb.ToString();
            return formatted;
        }
    }
}
