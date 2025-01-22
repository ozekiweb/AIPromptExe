using Ozeki;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Help;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIPrompt
{
    internal class AIPromptRootCommand : RootCommand
    {
        private static readonly Option<string?> optURL = new Option<string?>("-h");
        private static readonly Option<string?> optUsername = new Option<string?>("-u");
        private static readonly Option<string?> optPassword = new Option<string?>("-p");
        private static readonly Option<string?> optApikey = new Option<string?>("-a");
        private static readonly Option<bool?> optJson = new Option<bool?>("-j");
        private static readonly Option<string?> optModel = new Option<string?>("-m");
        private static readonly Option<bool> optLogging = new Option<bool>("-l", () => false);
        private static readonly Option<bool> optInteractive = new Option<bool>("-i", () => false);
        private static readonly Argument<string> argPrompt = new Argument<string>("prompt", () => "");

        private Action<AIPromptArguments> command;
        private string? standardInput;
        private AIPromptRootCommand(Action<AIPromptArguments> command, string? standardInput)
        {
            this.command = command;
            this.Description = "This tool makes it possible to run AI prompts on the command line, that are evaluated by HTTP AI APIs, such as Ozeki AI Server or Chat GPT.";
            this.standardInput = standardInput;
            this.SetHandler(ParseArguments);            
        }

        public static Parser Create(Action<AIPromptArguments> command, string? standardInput)
        {
            var rootCommand = new AIPromptRootCommand(command, standardInput) {
                optURL,
                optUsername,
                optPassword,
                optApikey,
                optJson,
                optModel,
                optLogging,
                optInteractive,
                argPrompt          
            };

            var parser = new CommandLineBuilder(rootCommand)
                .UseHelp("-?")
                .UseVersionOption("-v")
                .UseHelp(ctx =>
                {
                    SetHelpApperence(ctx);
                    ctx.HelpBuilder.CustomizeSymbol(argPrompt, "<prompt>", "the prompt to be sent to the HTTP AI API");
                    ctx.HelpBuilder.CustomizeSymbol(optURL, "-h <host url>", "specifies the URL of the server [default: http://localhost:9509/api?command=chatgpt]");
                    ctx.HelpBuilder.CustomizeSymbol(optUsername, "-u <user name>", "specifies the arguments.Username");
                    ctx.HelpBuilder.CustomizeSymbol(optPassword, "-p <password>", "specifies the password");
                    ctx.HelpBuilder.CustomizeSymbol(optApikey, "-a <API key>", "specifies the API key");
                    ctx.HelpBuilder.CustomizeSymbol(optJson, "-j", "specifies if JSON format is used [default: false]");
                    ctx.HelpBuilder.CustomizeSymbol(optModel, "-m <model name>", "specifies model name [default: GGUF_Model_1]");
                    ctx.HelpBuilder.CustomizeSymbol(optLogging, "-l", "enable logging mode");
                    ctx.HelpBuilder.CustomizeSymbol(optInteractive, "-i", "enable interactive mode");
                })
                .UseEnvironmentVariableDirective()
                .UseParseDirective()
                .UseSuggestDirective()
                .RegisterWithDotnetSuggest()
                .UseTypoCorrections()
                .UseParseErrorReporting()
                .UseExceptionHandler()
                .CancelOnProcessTermination()
                .Build();

            return parser;
        }

        private static void SetHelpApperence(HelpContext ctx)
        {
            ctx.HelpBuilder.CustomizeLayout(_ => HelpBuilder.Default.GetLayout()
                .Prepend(_ => _.Output.WriteLine("OZEKI AI Prompt v1.0.0"))
                .Append(_ => _.Output.WriteLine("Examples:"))
                .Append(_ => _.Output.WriteLine("  Send basic prompt with HTTP User Authentication:"))
                .Append(_ => _.Output.WriteLine(@"  aiprompt.exe ""basic_prompt"" -h http://localhost:9509/api?command=chatgpt -u username -p password -m GGUF_Model_1 -l"))
                .Append(_ => _.Output.WriteLine("  Send JSON prompt with API Key Authentication using standard I/O and logging mode:"))
                .Append(_ => _.Output.WriteLine(@"  echo ""json_prompt"" | aiprompt.exe -h http://localhost:9509/api?command=chatgpt -a api_key -jl"))
                .Append(_ => _.Output.WriteLine("  Read JSON prompt from file with API Key Authentication using standard I/O:"))
                .Append(_ => _.Output.WriteLine(@"  type prompt.json | aiprompt.exe -h http://localhost:9509/api?command=chatgpt -a api_key -jl"))
                .Append(_ => _.Output.WriteLine("  Chat with an AI Model by using Interactive Mode and HTTP User Authentication:"))
                .Append(_ => _.Output.WriteLine(@"  aiprompt.exe ""basic_prompt"" -h http://localhost:9509/api?command=chatgpt -u username -p password -m GGUF_Model_1 -i"))
                .Append(_ => _.Output.WriteLine("\nFor more information visit:" + Environment.NewLine + "  https://ozeki.chat/p_8675-ai-prompt.html")));
        }

        private void ParseArguments(InvocationContext context){
            var arguments = new AIPromptArguments()
            {
                Url = context.ParseResult.GetValueForOption(optURL) ?? EnvironmentVariable.URL ?? "http://localhost:9509/api?command=chatgpt",
                Username = context.ParseResult.GetValueForOption(optUsername) ?? EnvironmentVariable.USERNAME,
                Password = context.ParseResult.GetValueForOption(optPassword) ?? EnvironmentVariable.PASSWORD,
                Apikey = context.ParseResult.GetValueForOption(optApikey) ?? EnvironmentVariable.APIKEY,
                Json = (context.ParseResult.GetValueForOption(optJson) ?? (Boolean.TryParse(EnvironmentVariable.USE_JSON, out bool b)) && b),
                Model = context.ParseResult.GetValueForOption(optModel) ?? EnvironmentVariable.MODEL ?? "GGUF_Model_1",
                Logging = context.ParseResult.GetValueForOption(optLogging),
                Interactive = context.ParseResult.GetValueForOption(optInteractive),
                Prompt = context.ParseResult.GetValueForArgument(argPrompt)
            };

            Logger.setVerbosity(arguments.Logging);

            if (!(arguments.Apikey != null || (arguments.Username != null && arguments.Password != null) && arguments.Apikey == null))
            {
                Logger.Error("No credentials specified.");
                return;
            }
            if (arguments.Json)
            {
                if (arguments.Prompt != "")
                {
                    Logger.Error("JSON must be specified through Standard Input.");
                    return;
                }
            }

            if (arguments.Prompt == "" && standardInput == null)
            {
                Logger.Error("Prompt is not specified.");
                return;
            }

            if (arguments.Prompt == "" && standardInput != null)
            {
                Logger.Debug("Using StandardInput as prompt.");
                arguments.Prompt = standardInput;
            }
            //Start application with arguments
            command(arguments);
        }
    }
}
