using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AIPrompt
{
    internal class AIResponse
    {
        [JsonPropertyName("created")]
        public required int Created { get; set; }
        [JsonPropertyName("choices")]
        public required List<Choice> Choices {  get; set; } 
        
        internal class Choice
        {
            [JsonPropertyName("message")]
            public required Message Message { get; set; }
        }
    }
}
