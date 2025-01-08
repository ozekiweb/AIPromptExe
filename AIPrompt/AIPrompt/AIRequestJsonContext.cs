using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace AIPrompt
{
    [JsonSerializable(typeof(AIRequest))]
    partial class AIRequestJsonContext : JsonSerializerContext
    {
      
    }
}
