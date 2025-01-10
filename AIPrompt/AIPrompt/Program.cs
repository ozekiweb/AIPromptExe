﻿using AIPrompt;
using System;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Net.Http.Headers;
using System.Security.AccessControl;
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
     * Model: AI
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

            var optModel = new Option<string>("--model", () => EnvironmentVariable.MODEL ?? "AI", "specifies model name");
            optJson.AddAlias("-m");

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
                optModel,
                optVerbose,
                argPrompt,
            };
            rootCommand.SetHandler(CommandHandler, optURL, optUsername, optPassword, optAPIKey, optJson, optModel, optVerbose, argPrompt);

            return await rootCommand.InvokeAsync(args);
        }

        private static async void CommandHandler(string rawUrl, string? username, string? password, string? apikey, bool json, string model, bool verbose, string prompt)
        {
            Logger.setVerbosity(verbose);

            Logger.Debug("Parsing URL");
            if (!TryCreateHTTPUrl(rawUrl, out var url)) return;
            Logger.Debug("Checking done");

            Logger.Debug("Creating Authorisation Header");
            if (!TryCreateAuthenticationHeader(apikey, username, password, out var authorizationHeader)) return;
            Logger.Debug("Creating Authorisation Header done!");

            Logger.Debug("Creating Request Body");
            if(!TryCreateRequestContent(prompt,  json, model, out var content)) return;
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
                if (verbose) { 
                    Console.WriteLine(response);
                }
                else
                {
                    var aiResponse = JsonSerializer.Deserialize<AIResponse>(response, AIResponseJsonContext.Default.AIResponse);
                    Console.WriteLine(aiResponse.Choices[0].Message.ToString());
                }
            }
            catch (Exception e) { 
                
                Logger.Error(e.Message);
            }  
        }

        private static bool TryCreateRequestContent(string rawContent, bool isJsonFormat, string model, out StringContent? content)
        {
            var jsonString = "";
            try
            {
                if (isJsonFormat)
                {
                    //Parse prompt as JSON                   
                    Logger.Debug("Checking if JSON is valid");
                    Logger.Debug(rawContent);
                    JsonDocument.Parse(rawContent);
                    Logger.Debug("JSON is valid.");
                    jsonString = rawContent;
                }
                else
                {
                    Logger.Debug("Generating Request Body from prompt");
                    Message message = new Message() { Content = rawContent, Role = "user" };
                    AIRequest aiRequest = new AIRequest() { Messages = new List<Message> { message }, Model = model };
                    jsonString = JsonSerializer.Serialize<AIRequest>(aiRequest, AIRequestJsonContext.Default.AIRequest);
                    Logger.Debug("Generating done");
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                content = null;
                return false;
            }
            content = new StringContent(jsonString, Encoding.ASCII, "application/json");
            return true;
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

        private static async Task<String> SendAPIRequest(HttpRequestMessage request)
        {
            using (var client = new HttpClient())
            {
                Logger.Debug("Sending request");
                Logger.Debug(await request.Content.ReadAsStringAsync());
                var response = client.Send(request);
                Logger.Debug("HTTP Request sent: " + response.RequestMessage);
                response.EnsureSuccessStatusCode();
                Logger.Debug(response.StatusCode.ToString());
                var responseStream = await response.Content.ReadAsStreamAsync();
                Logger.Debug("Response arrived");
                var responseString = "";
                using (var sr = new StreamReader(responseStream, Encoding.UTF8))
                    responseString = sr.ReadToEnd();
                return responseString;
            }
        }
    }
}
