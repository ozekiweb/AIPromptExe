using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ozeki
{
    internal static class EnvironmentVariable
    {
        public static readonly string? URL = Environment.GetEnvironmentVariable("OZEKI_AIPROMPT_URL");

        public static readonly string? USERNAME = Environment.GetEnvironmentVariable("OZEKI_AIPROMPT_USERNAME");
        
        public static readonly string? PASSWORD = Environment.GetEnvironmentVariable("OZEKI_AIPROMPT_PASSWORD");

        public static readonly string? APIKEY = Environment.GetEnvironmentVariable("OZEKI_AIPROMPT_APIKEY");

        public static readonly string? USE_JSON = Environment.GetEnvironmentVariable("OZEKI_AIPROMPT_USE_JSON");
    }
}
