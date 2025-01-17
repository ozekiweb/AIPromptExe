using AIPrompt;
using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Help;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Reflection.Metadata;
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
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            string? standardInput = Console.IsInputRedirected ? Console.In.ReadToEnd() : null;
            var rootCommand = AIPromptRootCommand.Create(CommandHandler, standardInput);
            return await rootCommand.InvokeAsync(args);
        }

        private static async void CommandHandler(AIPromptArguments arguments)
        {
            Logger.Debug("Parsing URL");
            if (!TryCreateHTTPUrl(arguments.Url, out Uri url)) return;
            Logger.Debug("Checking done");

            Logger.Debug("Creating Authorisation Header");
            if (!TryCreateAuthenticationHeader(arguments.Apikey, arguments.Username, arguments.Password, out var authorizationHeader)) return;
            if (authorizationHeader == null) { return; }

            Logger.Debug("Creating Authorisation Header done!");

            Logger.Debug("Creating Request Body");
            if (!TryCreateRequestContent(arguments.Prompt, arguments.Json, arguments.Model, out var content)) return;
            Logger.Debug("Request body done");

            Logger.Debug("Setting up request");
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = content;
            request.Headers.Authorization = authorizationHeader;

            if (arguments.Interactive)
            {
                //Interactive mode
                Logger.Debug("Switching to interactive mode");
                InteractiveMode(request, url, authorizationHeader);
                return;
            }

            //Normal execution
            Logger.Debug("Standard execution");
            await NormalMode(arguments, request);
        }

        private static async Task NormalMode(AIPromptArguments arguments, HttpRequestMessage request)
        {
            string response = "";
            try
            {
                response = await SendAPIRequest(request);
            }
            catch (HttpRequestException e)
            {
                Logger.Error("An error happened while sending request:");
                Logger.Error(e.Message);
                if (e.InnerException != null)
                {
                    Logger.Error(e.InnerException.Message);
                }
                return;
            }
            catch (Exception e)
            {
                Logger.Error("Unexpected error happened:");
                Logger.Error(e.Message);
                return;
            }

            if (arguments.Logging)
            {
                Logger.Debug(response);
                return;
            }
            else if (arguments.Json)
            {
                Console.WriteLine(response);
                return;
            }

            //Basic prompt
            try
            {
                var aiResponse = JsonSerializer.Deserialize<AIResponse>(response, AIResponseJsonContext.Default.AIResponse);
                AIPromptConsole.PrintMessage(aiResponse.Choices[0].Message);
            }
            catch (JsonException)
            {
                Logger.Error("Unexpected response from server:");
                Logger.Error(response);
            }
        }

        private async static void InteractiveMode(HttpRequestMessage initialRequest, Uri url, AuthenticationHeaderValue authenticationHeader)
        {
            AIRequest? aiRequest = await GetInitialResponse(initialRequest);
            if (aiRequest == null) return;

            AIPromptConsole.WriteLine("Messages from the initial prompt: ");
            AIPromptConsole.PrintMessage(aiRequest.Messages);

            while (true)
            {
                var prompt = AIPromptConsole.Read("Enter your prompt:");
                Console.WriteLine();
                if (prompt == "exit")
                {
                    Logger.Debug("Interactive mode exited");
                    break;
                }
                var message = new Message() { Content = prompt, Role = "user" };
                aiRequest.Messages.Add(message);
                Logger.Debug(message.Content);

                var httprequest = new HttpRequestMessage(HttpMethod.Post, url);
                httprequest.Headers.Authorization = authenticationHeader;

                Message? answer = await GetAnswer(aiRequest, httprequest);
                if (answer == null) return;
                aiRequest.Messages.Add(answer);
                AIPromptConsole.PrintMessage(aiRequest.Messages.Last(), "Response:");
            }
        }

        private static async Task<Message?> GetAnswer(AIRequest aiRequest, HttpRequestMessage request)
        {
            try
            {
                var json = JsonSerializer.Serialize(aiRequest, AIRequestJsonContext.Default.AIRequest);
                request.Content = new StringContent(json, Encoding.ASCII, "application/json");
                Spinner spinner = new Spinner("Waiting for response:");
                string response = await SendAPIRequest(request);
                spinner.Stop();
                var aiResponse = JsonSerializer.Deserialize<AIResponse>(response, AIResponseJsonContext.Default.AIResponse);
                var answer = aiResponse.Choices[0].Message;
                return answer;
            }
            catch (HttpRequestException e)
            {
                Logger.Error("An error happened while sending request:");
                Logger.Error(e.Message);
                if (e.InnerException != null)
                {
                    Logger.Error(e.InnerException.Message);
                }
                return null;
            }
            catch (Exception e)
            {
                Logger.Error("Unexpected error happened:");
                Logger.Error(e.Message);
                return null;
            }
        }

        private static async Task<AIRequest?> GetInitialResponse(HttpRequestMessage httpRequest)
        {
            try
            {
                Logger.Debug("Setting up initial chat");
                Spinner spinner = new Spinner("Waiting for response of initial request:");
                string initiationResponse = await SendAPIRequest(httpRequest);
                Logger.Debug(initiationResponse);
                spinner.Stop();
                var initialAiResponse = JsonSerializer.Deserialize<AIResponse>(initiationResponse, AIResponseJsonContext.Default.AIResponse);
                var responseJson = await httpRequest.Content.ReadAsStringAsync();
                Logger.Debug(responseJson);
                var aiRequest = JsonSerializer.Deserialize<AIRequest>(responseJson, AIRequestJsonContext.Default.AIRequest);
                aiRequest.Messages.Add(initialAiResponse.Choices[0].Message);
                return aiRequest;
            }
            catch (HttpRequestException e)
            {
                Logger.Error("An error happened while sending request:");
                Logger.Error(e.Message);
                if (e.InnerException != null)
                {
                    Logger.Error(e.InnerException.Message);
                }
                return null;
            }
            catch (JsonException)
            {
                Logger.Error("Unexpected answer from server.");
                return null;
            }
            catch (Exception e)
            {
                Logger.Error("Unexpected error happened:");
                Logger.Error(e.Message);
                return null;
            }
        }

        private static bool TryCreateRequestContent(string rawContent, bool isJsonFormat, string model, out StringContent? content)
        {
            if (isJsonFormat && TryParseJson(rawContent, out string jsonString) || !isJsonFormat && TryBuildBasicRequest(rawContent, model, out jsonString))
            {
                content = new StringContent(jsonString, Encoding.ASCII, "application/json");
                return true;
            }
            content = null;
            return false;
        }

        private static bool TryBuildBasicRequest(string rawContent, string model, out string jsonString)
        {
            Logger.Debug("Generating Request Body from prompt");
            Message message1 = new() { Role = "assistant", Content = "You are an assistant." };
            Message message2 = new() { Content = rawContent, Role = "user" };
            AIRequest aiRequest = new AIRequest() { Messages = new List<Message> { message1, message2 }, Model = model };
            jsonString = JsonSerializer.Serialize<AIRequest>(aiRequest, AIRequestJsonContext.Default.AIRequest);
            Logger.Debug("Generating done");
            Logger.Debug(jsonString);
            return true;
        }

        private static bool TryParseJson(string rawContent, out string jsonString)
        {
            try
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

                return true;
            }
            catch (JsonException)
            {
                Logger.Error("Invalid JSON specified.");
                jsonString = "";
                return false;
            }
            catch (Exception e)
            {
                Logger.Error("Unexpected error happened:");
                Logger.Error(e.ToString());
                jsonString = "";
                return false;
            }
        }

        private static bool TryCreateAuthenticationHeader(string? apikey, string? username, string? password, out AuthenticationHeaderValue? authorization)
        {
            if (apikey != null)
            {
                //arguments.Apikey Auth Request
                Logger.Debug("Using API Authorisation Header");
                authorization = new AuthenticationHeaderValue("Bearer", apikey);

            }
            else if (username != null && password != null)
            {
                //arguments.Username Password auth
                Logger.Debug("Using Basic Authorisation Header");
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(username + ":" + password);
                var base64Encoded = Convert.ToBase64String(plainTextBytes);
                authorization = new AuthenticationHeaderValue("Basic", base64Encoded);
            }
            else
            {
                //Error
                Logger.Error("Credentials not specified.");
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
                Logger.Error("The scheme of specified URL is not http or https.");
                return false;
            }
            return true;
        }

        private static async Task<string> SendAPIRequest(HttpRequestMessage request)
        {
            var httpClientHandler = new HttpClientHandler();

            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(240);
            
            if (request.Content == null)
            {
                throw new NullReferenceException();
            }
            Logger.Debug("HTTP Request: " + request);
            Logger.Debug("Content: " + await request.Content.ReadAsStringAsync());
            Logger.Debug("Sending HTTP Request...");
            var response = client.Send(request);
            Logger.Debug("HTTP Request sent: " + response.RequestMessage);
            Logger.Debug("HTTP Response: " + (int)response.StatusCode + " " + response.StatusCode.ToString());
            var responseStream = await response.Content.ReadAsStreamAsync();
            using var sr = new StreamReader(responseStream, Encoding.UTF8);
            var responseString = sr.ReadToEnd();
            return responseString;
        }
    }
}
