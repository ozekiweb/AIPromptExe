using System.CommandLine;
using System.CommandLine.Parsing;
using Ozeki;
namespace Ozeki
{
    /*
     * Defaults if nothing is specified:
     * URL: http://www1.ozeki.hu:9511/api?command=chatgpt
     * Username: none
     * Password: none
     * APIKey: none
     * Use Json: false
     * Interactive: false
     */

    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            string? standardInput = Console.IsInputRedirected ? Console.In.ReadToEnd() : null;

            var optURL = new Option<string>("--hostname", () => EnvironmentVariable.URL ?? "http://www1.ozeki.hu:9511/api?command=chatgpt", "specifies the URL of the server");
            optURL.AddAlias("-hn");

            var optUsername = new Option<string?>("--username", () => EnvironmentVariable.USERNAME, "specifies the username");
            optUsername.AddAlias("-u");

            var optPassword = new Option<string?>("--password", () => EnvironmentVariable.PASSWORD, "specifies the password");
            optPassword.AddAlias("-p");

            var optAPIKey = new Option<string?>("--apikey", () => EnvironmentVariable.APIKEY, "specifies the API key");
            optAPIKey.AddAlias("-a");

            //Default is false if Environment variable doesn't exist, or can't be parsed to bool
            var optJson = new Option<bool>("--json", () => Boolean.TryParse(EnvironmentVariable.USE_JSON, out bool env) ? env : false, "specifies if JSON format is used");
            optJson.AddAlias("-j");

            var optInteractive = new Option<bool>("--interactive", () => false, "specifies if interactive mode is active");
            optJson.AddAlias("-i");

            var argPrompt = new Argument<string>("prompt", "the prompt to be sent to the HTTP AI API");
            if (standardInput != null)
            {
                argPrompt.SetDefaultValue(standardInput);
            }

            var rootCommand = new RootCommand("This tool makes it possible to send prompts from a command prompt using the Ozeki 10 HTTP API or OpenAI API")
            {
                optURL,
                optUsername,
                optPassword,
                optAPIKey,
                optJson,
                argPrompt,
            };
            rootCommand.SetHandler(CommandHandler, optURL, optUsername, optPassword, optAPIKey, optJson, optInteractive, argPrompt);

            return await rootCommand.InvokeAsync(args);
        }

        private static async void CommandHandler(string url, string? username, string? password, string? apikey, bool json, bool interactive, string prompt)
        {
            if (apikey != null)
            {
                //APIKey Auth Request
                Console.WriteLine("API Auth");
            }
            else if (username != null && password != null)
            {
                //Username Password auth
                Console.WriteLine("Basic Auth");
            }
            else
            {
                //Error
                OutputError("Credentials not specified");
                return;
            }

            if (json)
            {
                //Parse prompt as JSON
                Console.WriteLine("JSON");
            }
            else
            {
                //Build JSON for prompt
                Console.WriteLine("Single prompt");

            }

            if (interactive)
            {
                //Interactive mode
                Console.WriteLine("Interactive mode");
            }
            else 
            {
                //Normal execution
                Console.WriteLine("Normal mode");
            }
            Console.WriteLine(prompt);
        }
        private static void OutputError(string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(error);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
