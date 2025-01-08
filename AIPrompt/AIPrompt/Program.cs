using AIPrompt;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
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
            
            var optURL = new Option<string>("--url", () => EnvironmentVariable.URL ?? "http://www1.ozeki.hu:9511/api?command=chatgpt", "specifies the URL of the server");
            optURL.AddAlias("-u");

            var optUsername = new Option<string?>("--username", () => EnvironmentVariable.USERNAME, "specifies the username");
            optUsername.AddAlias("-us");

            var optPassword = new Option<string?>("--password", () => EnvironmentVariable.PASSWORD, "specifies the password");
            optPassword.AddAlias("-p");

            var optAPIKey = new Option<string?>("--apikey", () => EnvironmentVariable.APIKEY, "specifies the API key");
            optAPIKey.AddAlias("-a");

            //Default is false if Environment variable doesn't exist, or can't be parsed to bool
            var optJson = new Option<bool>("--json", () => Boolean.TryParse(EnvironmentVariable.USE_JSON, out bool env) ? env : false, "specifies if JSON format is used");
            optJson.AddAlias("-j");

            var optInteractive = new Option<bool>("--interactive", () => false, "specifies if interactive mode is active");
            optJson.AddAlias("-i");

            var optVerbose = new Option<bool>("--verbose", () => false, "verbose mode");
            optVerbose.AddAlias("-v");

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
                optInteractive,
                optVerbose,
                argPrompt,
            };
            rootCommand.SetHandler(CommandHandler, optURL, optUsername, optPassword, optAPIKey, optJson, optInteractive, optVerbose, argPrompt);

            return await rootCommand.InvokeAsync(args);
        }

        private static async void CommandHandler(string rawUrl, string? username, string? password, string? apikey, bool json, bool interactive, bool verbose, string prompt)
        {
            if (verbose)
            {
                Logger.logLevel = Logger.LogLevel.Debug;
            }
            else
            {
                Logger.logLevel = Logger.LogLevel.Error;
            }

            Logger.Debug("Parsing URL");
            if (!Uri.TryCreate(rawUrl, UriKind.Absolute, out var url))
            {
                Logger.Error("Invalid URL specified.");
                return;
            }
            Logger.Debug("Parsing URL done");

            Logger.Debug("Checking if URL fits HTTP or HTTPS scheme");
            if (!(url.Scheme == Uri.UriSchemeHttp || url.Scheme == Uri.UriSchemeHttps))
            {
                Logger.Error("The scheme of specified url is not http or https.");
                return;
            }
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            Logger.Debug("Checking done");

            Logger.Debug("Creating Authorisation Header");
            if (apikey != null)
            {
                //APIKey Auth Request
                Logger.Debug("Using API Authorisation Header");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apikey);

            }
            else if (username != null && password != null)
            {
                //Username Password auth
                Logger.Debug("Using Basic Authorisation Header");
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(username + ":" + password);
                var base64Encoded = Convert.ToBase64String(plainTextBytes);
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", apikey);
            }
            else
            {
                //Error
                Logger.Error("Credentials not specified");
                return;
            }
            Logger.Debug("Creating Authorisation Header done!");

            
            Logger.Debug("Creating Request Body");
            AIRequest? body = null;
            if (json)
            {
                //Parse prompt as JSON
                Console.WriteLine("JSON");
                try
                {
                    Logger.Debug("Deserializing JSON to Request");
                    Logger.Debug(prompt);
                    body = JsonSerializer.Deserialize<AIRequest>(prompt, AIRequestJsonContext.Default.AIRequest);
                    Logger.Debug("Deserializing JSON to Request done");
                }
                catch (Exception e)
                {
                    Logger.Error(e.ToString());
                    return;
                }

            }
            else
            {
                //Build JSON for prompt
                Logger.Debug("Generating Request Body from prompt");
                Message message = new Message() { Content = prompt, Role = "user" };
                body = new AIRequest() { Messages = new List<Message> { message }, Model = "AI" };
                Logger.Debug("Generating done: ");
            }

            if(body == null)
            {
                Logger.Error("Request Body generation error");
                return;
            }

            var content = JsonSerializer.Serialize<AIRequest>(body, AIRequestJsonContext.Default.AIRequest);
            request.Content = new StringContent(content, Encoding.ASCII, "application/json");
            Logger.Debug("Request body done");

            if (interactive)
            {
                //Interactive mode
                Logger.Debug("Switching to interactive mode");
            }
            else 
            {
                //Normal execution
                Logger.Debug("Standard execution");
                try
                {
                    using (var client = new HttpClient())
                    {
                        Logger.Debug("Sending request");
                        Logger.Debug(request.Content.ToString());
                        Logger.Debug(await request.Content.ReadAsStringAsync());
                        var response = client.Send(request);
                        Logger.Debug("HTTP Request sent: " + response.RequestMessage);
                        response.EnsureSuccessStatusCode();
                        Logger.Debug(response.StatusCode.ToString());
                        var responseStream = await response.Content.ReadAsStreamAsync();
                        Logger.Debug("Response arrived");
                        using (var sr = new StreamReader(responseStream, Encoding.UTF8))
                            Console.WriteLine(sr.ReadToEnd());
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e.Message);
                }
            }   
        }
    }
}
