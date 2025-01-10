using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ozeki
{
    internal static class EnvironmentVariable
    {
        public static readonly string? URL = TestAllLocation("OZEKI_AIPROMPT_URL");

        public static readonly string? USERNAME = TestAllLocation("OZEKI_AIPROMPT_USERNAME");
        
        public static readonly string? PASSWORD = TestAllLocation("OZEKI_AIPROMPT_PASSWORD");

        public static readonly string? APIKEY = TestAllLocation("OZEKI_AIPROMPT_APIKEY");

        public static readonly string? USE_JSON = TestAllLocation("OZEKI_AIPROMPT_USE_JSON");

        public static readonly string? MODEL = TestAllLocation("OZEKI_AIPROMPT_MODEL");

        private static string? TestAllLocation(string variable)
        {
            return Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.Process)
                ?? Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.User)
                ?? Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.Machine);
        }

    }
}
