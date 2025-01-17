using Ozeki;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIPrompt
{
    internal class AIPromptArguments
    {
        public required string Url;
        public required string? Username;
        public required string? Password;
        public required string? Apikey;
        public required bool Json;
        public required string Model;
        public required bool Logging;
        public required bool Interactive;
        public required string Prompt;
    }
}
