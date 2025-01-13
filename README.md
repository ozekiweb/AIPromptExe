# OZEKI AI Prompt

[OZEKI AI Prompt](https://ozeki.chat/p_8675-ai-command-line-tool-to-run-ai-prompts-from-cmd-or-powershell.html) is a tool which makes it possible to run AI prompts on the command line, that are evaluated by HTTP AI APIs, such as Ozeki AI Server or Chat GPT.

## Table of Contents

- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
- [Arguments](#arguments)
- [Environment Variables](#environment-variables)
- [Contact](#contact)
- [Authentication](#authentication)
- [License](#license)

## Features

- Send prompts via command prompt.
- Supports Ozeki 10 HTTP API and OpenAI API.
- Configurable via command-line arguments and environment variables.
- JSON format support and logging mode available.
- Accepts input from standard I/O.

## Installation

### Option 1: Use the Precompiled Executable (Recommended)

1. Download the precompiled executable from the [Ozeki Website](https://ozeki.chat/p_8675-ai-command-line-tool-to-run-ai-prompts-from-cmd-or-powershell.html) or the [Releases](https://github.com/ozekiweb/AIPromptExe/releases) page on GitHub.
2. Extract the downloaded file.
3. Run the executable directly:

```bash
./AIPrompt.exe
```

### Option 2: Build the App Using the .NET Tool

1. Clone the repository:

```bash
git clone https://github.com/username/repository.git
```

2. Navigate to the project directory:

```bash
cd repository
```

3. Build the project using the .NET CLI:

```bash
dotnet build
```

## Usage

To use the app, provide a prompt and optional configuration parameters:.

```bash
AIPrompt [prompt] <options>
```

You can also pipe the prompt from standard I/O:

```bash
echo "your prompt" | AIPrompt [options]
```
For more examples check out this [guide](https://ozeki.chat/p_8675-ai-command-line-tool-to-run-ai-prompts-from-cmd-or-powershell.html).

## Arguments

The app supports the following command-line arguments:

| Argument                     | Description                                     | Default Value                                   |
|------------------------------|-------------------------------------------------|------------------------------------------------|
| `<prompt>`                   | The prompt to be sent to the HTTP AI API.      | (none)                                         |
| `-h <url>`            | Specifies the URL of the server.               | `http://localhost:9511/api?command=chatgpt`|
| `-u <username>` | Specifies the username.                        | (none)                                        |
| `-p <password>`  | Specifies the password.                        | (none)                                       |
| `-a <apikey>`      | Specifies the API key.                         | (none)                                         |
| `-j`             | Specifies if the prompt is in JSON format      | `False`                                         |
| `-m <model>`            | Specifies the model name.                      | `AI`                                           |
| `-l`              | Enables verbose mode.                          | `False`                                        |
| `-v`                  | Displays version information.                  | (none)                                         |
| `-?`             | Displays help and usage information.           | (none)                                         |

## Environment Variables

Use environment variables to configure your the using the key-values specified below. Environment variables are particularly useful when you need to set configurations that are consistent across multiple runs or systems. For instance, they allow sensitive information, such as API keys or passwords, to be stored securely without hardcoding them into the application or passing them as command-line arguments. The app supports all Environment Variable Scope for Windows. If the same key is specified in multiple scopes, then order of evaluation is *Process, User, and Machine*

| Key                         | Usage                                          | Accepted Values                                  |
|------------------------------|-----------------------------------------------|-------------------------------------------------|
| `OZEKI_AIPROMPT_URL`         | Specifies the URL of the server               | Any valid URL                                    |
| `OZEKI_AIPROMPT_USERNAME`    | Specifies the username for authentication     | Any valid username                               |
| `OZEKI_AIPROMPT_PASSWORD`    | Specifies the password for authentication     | Any valid password                               |
| `OZEKI_AIPROMPT_APIKEY`      | Specifies the API key                         | Any valid API key                                |
| `OZEKI_AIPROMPT_USE_JSON`    | Specifies if the prompt is in JSON format              | `True`, `False`                                  |
| `OZEKI_AIPROMPT_MODEL`       | Specifies the model name                      | Any valid model name                             |


## Authentication
AIPromptExe supports two types of Authentication
| Method | Compatibility |
|-|-|
|Using API key |Ozeki and OpenAI|
|Using HTTP user credentials| Ozeki|

### Method 1: API Key

If you want to connect to the OpenAI API, you can find your API keys from [here](https://platform.openai.com/api-keys)

### Method 2: HTTP user credentials
To create a HTTP user, open the Chat Gateway and click on the Add application or chatbot link, then install a new HTTP user.
For more details, check out this [guide](https://ozeki.chat/p_8675-ai-command-line-tool-to-run-ai-prompts-from-cmd-or-powershell.html)

## Contact

For support or questions about the project, please contact:

- **GitHub Issues:** [Open an issue](https://github.com/ozekiweb/AIPromptExe/issues)
- **Ozeki** [Contact us](http://in.ozeki.hu:86/p_4597-contact-us-to-get-ozeki-chat-server.html)

## License

This project is licensed under the [MIT License](LICENSE).

