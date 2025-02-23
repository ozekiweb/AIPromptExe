# OZEKI AI Prompt 

OZEKI AI Prompt is a simple, easy to use command line tool that makes it possible to run AI prompts in Windows command shell (cmd.exe), in Windows powershell, in Linux bash or in the Mac shell. It is a single executable file, it does not require any installation procedure. It has no dependencies.

The aiprompt.exe executable can be run from the command line, and the prompt can be passed to it as a command line argument (or through the standard input). The prompt is executed using a http api call to [Ozeki AI Server](https://ozeki-ai-server.com) or to ChatGPT, DeepSeek or other OpenAI API compatible providers(depending on the configuration)

## Project website

This project is sponsored by Ozeki. The project's website can be found at the following link:
https://ozeki-ai-server.com/p_8675-ai-command-line-tool-to-run-ai-prompts-from-cmd-or-powershell.html

## Table of Contents

- [Introduction](#introduction)
- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
- [Guides](#guides)
- [Examples](#examples)
- [Arguments](#arguments)
- [Environment Variables](#environment-variables)
- [Contact](#contact)
- [Authentication](#authentication)
- [Using OpenAI API / ChatGPT API](#using-openai-api--chatgpt-api)
- [Using Deepseek](#using-deepseek)
- [License](#license)

## Introduction
**Ozeki AI Server** is a powerful software solution for running AI models locally or accessing remote AI services on Windows or Linux. It allows multiple AI models to operate simultaneously on shared hardware, maximizing efficiency and performance. A key feature is its ability to connect AI models to communication channels like **email**, **phone**, and **SMS**, enabling automated responses and streamlined interactions. Unlike subscribing to online AI services, owning your AI system **reduces long-term costs**. Combined with its **privacy**, **scalability**, **reliability**, and **user-friendly interface** Ozeki AI Server is the ultimate tool for leveraging AI in real-world applications while staying budget-friendly.

![Ozeki AI Server Logo](/Resources/ai-prompts-command-line.png)

**More information:** https://ozeki-ai-server.com

**HTTP API:** https://ozeki-ai-server.com/p_7097-ai-developer.html


## Features

- Prompt Sending: Allows users to send prompts directly to HTTP AI APIs from the command line.
- Multiple API Support: Supports Ozeki 10 HTTP API, OpenAI API, Deepseek and all other OpenAI API compatible providers.
- Configurable: Easy to set your preferences via command-line arguments and environment variables.
- Flexible: Read prompts from Standard I/O, send a simple prompt or customize your request using JSON, use HTTP and HTTPS as well
- Logging Mode: Provides detailed logging to facilitate debugging and monitoring.
- Interactive Mode: Chat with an LLM by sending multiple prompts through HTTP
- Fast deployment: Ready to use, self-contained executable file is available to download. 
- Open Source: This tool is fully open sourced, enabling users to build, inspect, and modify the source code according to their needs

## Installation

### Option 1: Use the Precompiled Executable (Recommended)

1. Download the precompiled executable from the [Ozeki Website](https://ozeki-ai-server.com/p_8675-ai-command-line-tool-to-run-ai-prompts-from-cmd-or-powershell.html) or the [Releases](https://github.com/ozekiweb/AIPromptExe/releases) page on GitHub.  
2. Extract the downloaded file, if you downloaded a compressed executable.
3. Run the executable directly:

```bash
./AIPrompt.exe
```

### Option 2: Build the App Using the .NET Tool

1. Clone the repository:

```bash
git clone https://github.com/ozekiweb/AIPromptExe
```

2. Navigate to the project directory:

```bash
cd repository
```

3. Build the project using the .NET CLI:

```bash
dotnet build
```

To install Ozeki AI Prompt on Linux or MacOS, visit the following pages:

**Linux:** https://ozeki-ai-server.com/p_8699-how-to-run-ai-prompts-in-linux-bash-shell.html
**MacOS:** https://ozeki-ai-server.com/p_8701-how-to-run-ai-prompts-in-mac-shell.html

## Usage 

To use the app, provide a prompt and optional configuration parameters:.

```bash
AIPrompt [prompt] <options>
```

You can also pipe the prompt from standard I/O:

```bash
echo "your prompt" | AIPrompt [options]
```

## Guides
For more detailed instructions, check out our dedicated guides on how to use the Ozeki AI Prompt on different operating systems:
##### How to use Ozeki AI Prompt on Windows:
https://ozeki-ai-server.com/p_8675-ai-prompt.html
##### How to use Ozeki AI Prompt on Linux:
https://ozeki-ai-server.com/p_8699-how-to-run-ai-prompts-in-linux-bash-shell.html
##### How to use Ozeki AI Prompt on Mac:
https://ozeki-ai-server.com/p_8701-how-to-run-ai-prompts-in-mac-shell.html
##### How to generate an Ozeki API Key:
https://ozeki-ai-server.com/p_8698-how-to-create-an-api-key.html

## Examples

### Send basic prompt with HTTP User Authentication:
```bash
aiprompt.exe "basic_prompt" -h http://localhost:9509/api?command=chatgpt -u username -p password -model GGUF_Model_1 -l
```
### Send JSON prompt with API Key Authentication using standard I/O and logging mode:
```bash
echo "json_prompt" | aiprompt.exe -h http://localhost:9509/api?command=chatgpt -a api_key -jl
```
### Read JSON prompt from file with API Key Authentication using standard I/O and logging mode:
```bash
type prompt.json | aiprompt.exe -h http://localhost:9509/api?command=chatgpt -a api_key -jl
```
### Chat with an AI Model by using Interactive Mode and HTTP User Authentication:
```bash
aiprompt.exe "basic_prompt" -h http://localhost:9509/api?command=chatgpt -u username -p password -model GGUF_Model_1 -i
```
                                          

## Arguments

The app supports the following command-line arguments:

| Argument                      | Description                                   | Default Value                                 |
|-------------------------------|-----------------------------------------------|-----------------------------------------------|
| `<prompt>`                    | The prompt to be sent to the HTTP AI API.     | (none)                                        |
| `-h <url>`                    | Specifies the URL of the server.              | `http://localhost:9509/api?command=chatgpt`   |
| `-u <username>`               | Specifies the username.                       | (none)                                        |
| `-p <password>`               | Specifies the password.                       | (none)                                        |
| `-a <apikey>`                 | Specifies the API key.                        | (none)                                        |
| `-j`                          | Specifies if the prompt is in JSON format     | `False`                                       |
| `-m <model>`                  | Specifies the model name.                     | `GGUF_Model_1`                                |
| `-l`                          | Enables logging mode.                         | `False`                                       |
| `-i`                          | Enables interactive mode.                     | `False`                                       |
| `-v`                          | Displays version information.                 | (none)                                        |
| `-?`                          | Displays help and usage information.          | (none)                                        |

## Environment Variables

Use environment variables to configure your preferences using the key-values specified below. Environment variables are particularly useful when you need to set configurations that are consistent across multiple runs or systems. For instance, they allow sensitive information, such as API keys or passwords, to be stored securely without hardcoding them into the application or passing them as command-line arguments. The app supports all Environment Variable Scope for Windows. If the same key is specified in multiple scopes, then order of evaluation is *Process, User, and Machine*.

| Key                          | Usage                                          | Accepted Values                    |
|------------------------------|-----------------------------------------------|------------------------------------|
| `OZEKI_AIPROMPT_URL`         | Specifies the URL of the server               | URLs with http or https scheme     |
| `OZEKI_AIPROMPT_USERNAME`    | Specifies the username for authentication     | string                             |
| `OZEKI_AIPROMPT_PASSWORD`    | Specifies the password for authentication     | string                             |
| `OZEKI_AIPROMPT_APIKEY`      | Specifies the API key                         | string                             |
| `OZEKI_AIPROMPT_USE_JSON`    | Specifies if the prompt is in JSON format     | `True`, `False`                    |
| `OZEKI_AIPROMPT_MODEL`       | Specifies the model name                      | string                             |


## Authentication
Ozeki AI Prompt supports two types of Authentication
| Method | Compatibility |
|-|-|
|Using HTTP user credentials| Ozeki AI Server|
|Using API key |Ozeki AI Server, OpenAI, Deepseek, other OpenAI Api compatible providers|

### Method 1: HTTP user credentials
To create a HTTP user, open the Chat Gateway and click on the Add application or chatbot link, then install a new HTTP user.
For more details, check out this [guide](https://ozeki-ai-server.com/p_8675-ai-command-line-tool-to-run-ai-prompts-from-cmd-or-powershell.html).

### Method 2: API Key
To generate you own Ozeki API key, create a HTTP user, then generate an API Key on the Advanced tab.
For more details, visit this [tutorial](https://ozeki-ai-server.com/p_8698-how-to-create-an-api-key.html).
If you want to connect to the OpenAI API, you can find your API keys [here](https://platform.openai.com/api-keys).


## Using OpenAI API / ChatGPT API

To connect to the OpenAI API, you'll need an OpenAI API key. After logging into the [OpenAI developer platform](https://platform.openai.com), generate an API Key at https://platform.openai.com/api-keys. If you don't have an OpenAI account register one [here](https://platform.openai.com/signup).

**Ozeki AI Prompt** is fully compatible with the *OpenAI Chat Completion API*, allowing you to use basic prompts, JSON prompts, and interactive mode as well to connect to your favorite OpenAI LLM model.

The OpenAI Chat Completion API can be accessed through the following endpoint:
```
https://api.openai.com/v1/chat/completions
```

### Basic prompt example:
```bash
::Change YOUR_API_KEY to your OpenAI API key
aiprompt.exe "Where is Budapest?" -m gpt-4o-mini -h https://api.openai.com/v1/chat/completions -a YOUR_API_KEY
```

### JSON prompt example:
```bash
::Contents of prompt.json
{
    "model":"gpt-4o-mini",
    "messages":[ 
       {
          "role":"user",
          "content":"Where is Budapest?"
       },
       {
         "role":"assistant",
         "content":"Budapest is the capital city of Hungary, located in Central Europe. It sits on both sides of the Danube River, with Buda on the west bank and Pest on the east. Budapest is known for its stunning architecture, historic sites, and vibrant culture, making it a popular destination for tourists."
       },
       {
         "role":"user",
         "content":"Tell me the three most visited tourist attraction in Budapest"
      }
    ]
}

::Change the example API key to your OpenAI API key
type prompt.json | aiprompt.exe -m gpt-4o-mini -h https://api.openai.com/v1/chat/completions -a YOUR_API_KEY
```

## Using DeepSeek API

To connect to the Deepseek API, you'll need an DeepSeek API key, which you can generate after registering on the [DeepSeek developer platform](https://platform.deepseek.com).

**Ozeki AI Prompt** is fully compatible with the DeepSeek Chat Completion API, allowing you to use basic prompts, JSON prompts, and interactive mode with the DeepSeek-V3 or DeepSeek-R1 model.

The DeepSeek Chat Completion API can be accessed through the following endpoint:
```
https://api.deepseek.com/chat/completions
```

### Basic prompt example:
```bash
::Change YOUR_API_KEY to your DeepSeek API key
aiprompt.exe "Where is Beijing?" -m deepseek-chat -h https://api.deepseek.com/chat/completions -a YOUR_API_KEY
```

### JSON prompt example:
```bash
::Contents of prompt.json
{
    "model":"deepseek-chat",
    "messages":[ 
       {
          "role":"user",
          "content":"Where is Beijing?"
       },
       {
         "role":"assistant",
         "content":"Beijing is the capital city of the People's Republic of China. It is located in the northern part of the country, near the northeastern edge of the North China Plain. Beijing is one of the most populous cities in the world and serves as the political, cultural, and educational center of China. The city is surrounded by mountains to the north, northwest, and west, which provide a natural defense and have historically contributed to its strategic importance. Beijing is also known for its rich history, with numerous historical sites such as the Forbidden City, the Great Wall of China, and the Temple of Heaven."
       },
       {
         "role":"user",
         "content":"Tell me more about the Forbidden City"
      }
    ]
}

::Change the example API key to your Deepseek API key
type prompt.json | aiprompt.exe -m deepseek-chat -h https://api.deepseek.com/chat/completions -a YOUR_API_KEY
```

## Contact

For support or questions about the project, please contact:

- **GitHub Issues:** [Open an issue](https://github.com/ozekiweb/AIPromptExe/issues)
- **Ozeki:** [Contact us](https://ozeki-ai-server.com/p_4597-contact-us-to-get-ozeki-chat-server.html)

## License

This project is licensed under the [MIT License](LICENSE).

