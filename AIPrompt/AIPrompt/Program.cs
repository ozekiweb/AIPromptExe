using AIPrompt;
using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Help;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Security.AccessControl;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;
namespace Ozeki
{
    /*
     * Defaults if nothing is specified:
     * URL: http://www1.ozeki.hu:9511/api?command=chatgpt
     * Username: none
     * Password: none
     * APIKey: none
     * Use Json: false
     * Model: AI
     */



    internal class Program
    {

        static async Task<int> Main(string[] args)
        {
            string? standardInput = Console.IsInputRedirected ? Console.In.ReadToEnd() : null;

            var optURL = new Option<string?>("-h");
            var optUsername = new Option<string?>("-u");
            var optPassword = new Option<string?>("-p");
            var optAPIKey = new Option<string?>("-a");
            var optJson = new Option<bool?>("-j");
            var optModel = new Option<string?>("-m");
            var optLogging = new Option<bool>("-l", () => false);

            var argPrompt = new Argument<string>("prompt", "the prompt to be sent to the HTTP AI API");
            if (standardInput != null)
            {
                argPrompt.SetDefaultValue(standardInput);
            }

            var rootCommand = new RootCommand("This tool makes it possible to run AI prompts on the command line, that are evaluated by HTTP AI APIs, such as Ozeki AI Server or Chat GPT.")
            {
                optURL,
                optUsername,
                optPassword,
                optAPIKey,
                optJson,
                optModel,
                optLogging,
                argPrompt,
            };
            rootCommand.SetHandler(async context =>
            {
                string url = context.ParseResult.GetValueForOption(optURL) ?? EnvironmentVariable.URL ?? "http://localhost:9511/api?command=chatgpt";
                string? username = context.ParseResult.GetValueForOption(optUsername) ?? EnvironmentVariable.USERNAME;
                string? password = context.ParseResult.GetValueForOption(optPassword) ?? EnvironmentVariable.PASSWORD;
                string? apikey = context.ParseResult.GetValueForOption(optAPIKey) ?? EnvironmentVariable.APIKEY;
                bool json = (context.ParseResult.GetValueForOption(optJson) ?? (Boolean.TryParse(EnvironmentVariable.USE_JSON, out bool b)) && b);
                string model = context.ParseResult.GetValueForOption(optModel) ?? EnvironmentVariable.MODEL ?? "AI";
                bool logging = context.ParseResult.GetValueForOption(optLogging);
                string prompt = context.ParseResult.GetValueForArgument(argPrompt);

                Logger.setVerbosity(logging);

                if (!(apikey != null || (username != null && password != null) && apikey == null))
                {
                    Logger.Error("No credentials specified");
                    return;
                }

                await CommandHandler(url, username, password, apikey, json, model, logging, prompt);
            });

            var parser = new CommandLineBuilder(rootCommand)
                .UseHelp("-?")
                .UseHelp(ctx =>
                {
                    ctx.HelpBuilder.CustomizeLayout(_ => HelpBuilder.Default.GetLayout()
                        .Prepend(_ => _.Output.WriteLine("OZEKI AI Prompt v1.0.0"))
                        .Append(_ => _.Output.WriteLine("Examples:"))
                        .Append(_ => _.Output.WriteLine("  Send basic prompt with HTTP User Authentication:"))
                        .Append(_ => _.Output.WriteLine(@"  aiprompt.exe ""basic_prompt"" -h http://localhost:9511/api?command=chatgpt -u username -p password -model AI -l"))
                        .Append(_ => _.Output.WriteLine("  Send JSON prompt with API Key Authentication:"))
                        .Append(_ => _.Output.WriteLine(@"  aiprompt.exe ""json_prompt"" -h http://localhost:9511/api?command=chatgpt -a api_key -model AI -jl"))
                        .Append(_ => _.Output.WriteLine("\nFor more information visit:" + Environment.NewLine + "  https://ozeki.chat/p_8675-ai-prompt.html")));

                    ctx.HelpBuilder.CustomizeSymbol(optURL, "-h <host url>", "specifies the URL of the server [default: http://localhost:9511/api?command=chatgpt]");
                    ctx.HelpBuilder.CustomizeSymbol(optUsername, "-u <user name>", "specifies the username");
                    ctx.HelpBuilder.CustomizeSymbol(optPassword, "-p <password>", "specifies the password");
                    ctx.HelpBuilder.CustomizeSymbol(optAPIKey, "-a <API key>", "specifies the API key");
                    ctx.HelpBuilder.CustomizeSymbol(optJson, "-j", "specifies if JSON format is used [default: false]");
                    ctx.HelpBuilder.CustomizeSymbol(optModel, "-m <model name>", "specifies model name [default: AI]");
                    ctx.HelpBuilder.CustomizeSymbol(optLogging, "-l", "logging mode");
                })
                .UseEnvironmentVariableDirective()
                .UseVersionOption("-v")
                .UseParseDirective()
                .UseSuggestDirective()
                .RegisterWithDotnetSuggest()
                .UseTypoCorrections()
                .UseParseErrorReporting()
                .UseExceptionHandler()
                .CancelOnProcessTermination()
                .Build();
            return await parser.InvokeAsync(args);
        }

        private static async Task CommandHandler(string rawUrl, string? username, string? password, string? apikey, bool json, string model, bool verbose, string prompt)
        {
            Logger.Debug("Parsing URL");
            if (!TryCreateHTTPUrl(rawUrl, out var url)) return;
            Logger.Debug("Checking done");

            Logger.Debug("Creating Authorisation Header");
            if (!TryCreateAuthenticationHeader(apikey, username, password, out var authorizationHeader)) return;
            Logger.Debug("Creating Authorisation Header done!");

            Logger.Debug("Creating Request Body");
            if (!TryCreateRequestContent(prompt, json, model, out var content)) return;
            Logger.Debug("Request body done");

            Logger.Debug("Setting up request");
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = content;
            request.Headers.Authorization = authorizationHeader;

            //Normal execution
            Logger.Debug("Standard execution");

            try
            {
                string response = await SendAPIRequest(request);
                if (verbose)
                {
                    Logger.Debug(response);
                }
                else if (json)
                {
                    Console.WriteLine(response);
                }
                else
                {
                    var aiResponse = JsonSerializer.Deserialize<AIResponse>(response, AIResponseJsonContext.Default.AIResponse);
                    if (aiResponse == null)
                    {
                        throw new NullReferenceException();
                    }
                    Console.WriteLine(aiResponse.Choices[0].Message.ToString());
                }
            }
            catch (HttpRequestException e)
            {
                Logger.Error("An error happened while sending request.");
                Logger.Error(e.Message);
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }
        }

        private static bool TryCreateRequestContent(string rawContent, bool isJsonFormat, string model, out StringContent? content)
        {

            try
            {
                var jsonString = "";
                if (isJsonFormat)
                {
                    //Parse prompt as JSON                   
                    Logger.Debug("Checking if JSON is valid");

                    rawContent = rawContent.Trim().Trim('"', '\'');           
                    Logger.Debug(rawContent);
                    var parsed = JsonNode.Parse(rawContent);
                    
                    Logger.Debug("JSON is valid.");
                    jsonString = parsed.ToJsonString(new JsonSerializerOptions()
                    {
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                    });
                    jsonString = jsonString.Replace("\\\"", "\"");

                    Logger.Debug(jsonString);
                }
                else
                {
                    Logger.Debug("Generating Request Body from prompt");
                    Message message = new Message() { Content = rawContent, Role = "user" };
                    AIRequest aiRequest = new AIRequest() { Messages = new List<Message> { message }, Model = model };
                    jsonString = JsonSerializer.Serialize<AIRequest>(aiRequest, AIRequestJsonContext.Default.AIRequest);
                    Logger.Debug("Generating done");
                }
                content = new StringContent(jsonString, Encoding.ASCII, "application/json");
                return true;
            }
            catch (JsonException)
            {
                Logger.Error("Invalid JSON specified.");
                content = null;
                return false;
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                content = null;
                return false;
            }
        }

        private static bool TryCreateAuthenticationHeader(string? apikey, string? username, string? password, out AuthenticationHeaderValue? authorization)
        {
            if (apikey != null)
            {
                //APIKey Auth Request
                Logger.Debug("Using API Authorisation Header");
                authorization = new AuthenticationHeaderValue("Bearer", apikey);

            }
            else if (username != null && password != null)
            {
                //Username Password auth
                Logger.Debug("Using Basic Authorisation Header");
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(username + ":" + password);
                var base64Encoded = Convert.ToBase64String(plainTextBytes);
                authorization = new AuthenticationHeaderValue("Basic", base64Encoded);
            }
            else
            {
                //Error
                Logger.Error("Credentials not specified");
                authorization = null;
                return false;
            }
            return true;
        }


        private static bool TryCreateHTTPUrl(string rawUrl, out Uri url)
        {
            if (!Uri.TryCreate(rawUrl, UriKind.Absolute, out url))
            {
                Logger.Error("Invalid URL specified.");
                return false;
            }
            Logger.Debug("Parsing URL done");

            Logger.Debug("Checking if URL fits HTTP or HTTPS scheme");
            if (!(url.Scheme == Uri.UriSchemeHttp || url.Scheme == Uri.UriSchemeHttps))
            {
                Logger.Error("The scheme of specified url is not http or https.");
                return false;
            }
            return true;
        }

        private static async Task<string> SendAPIRequest(HttpRequestMessage request)
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(240);
            Logger.Debug("Sending request");
            if (request.Content == null)
            {
                throw new NullReferenceException();
            }
            Logger.Debug(await request.Content.ReadAsStringAsync());
            var response = client.Send(request);
            Logger.Debug("HTTP Request sent: " + response.RequestMessage);
            response.EnsureSuccessStatusCode();
            Logger.Debug(response.StatusCode.ToString());
            var responseStream = await response.Content.ReadAsStreamAsync();
            Logger.Debug("Response arrived");
            using var sr = new StreamReader(responseStream, Encoding.UTF8);
            var responseString = sr.ReadToEnd();
            return responseString;
        }
    }
}
