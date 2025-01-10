# AIPromptExe

This tool makes it possible to send prompts from a command prompt using the Ozeki 10 HTTP API or OpenAI API.

## Table of Contents

- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
- [Arguments](#arguments)
- [Environment Variables](#environment-variables)
- [Contact](#contact)
- [License](#license)

## Features

- Send prompts via command prompt.
- Supports Ozeki 10 HTTP API and OpenAI API.
- Configurable via command-line arguments and environment variables.
- JSON format support and verbose mode available.
- Accepts input from standard I/O.

## Authentication
AIPromptExe supports two types of Authentication
| Method | Compatible |
|-|-|
|Using API key |Ozeki and OpenAI|
|Using HTTP user credentials| Ozeki|


## Installation

Provide step-by-step instructions on how to set up your project locally.

### Option 1: Use the Precompiled Executable (Recommended)

1. Download the precompiled executable from the [Ozeki Website](https://ozeki.chat/p_8675-ai-command-line-tool-to-run-ai-prompts-from-cmd-or-powershell.html) or the [Releases](https://github.com/ozekiweb/AIPromptExe/releases) page on GitHub.
2. Extract the downloaded file (if zipped).
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

4. Run the project:

```bash
dotnet run
```

## Usage

To use the app, provide a prompt and optional configuration parameters.

You can also pipe input from standard I/O:

```bash
echo "your prompt" | AIPrompt [options]
```

Or interactively:

```bash
AIPrompt [options]
# Type your prompt directly into the terminal
```

## Arguments

The app supports the following command-line arguments:

| Argument                     | Description                                     | Default Value                                   |
|------------------------------|-------------------------------------------------|------------------------------------------------|
| `<prompt>`                   | The prompt to be sent to the HTTP AI API.      | (none)                                         |
| `-u, --url <url>`            | Specifies the URL of the server.               | `http://www1.ozeki.hu:9511/api?command=chatgpt`|
| `-us, --username <username>` | Specifies the username.                        | (none)                                        |
| `-p, --password <password>`  | Specifies the password.                        | (none)                                       |
| `-a, --apikey <apikey>`      | Specifies the API key.                         | (none)                                         |
| `-j, -m, --json`             | Specifies if the prompt is in JSON format      | `False`                                         |
| `--model <model>`            | Specifies the model name.                      | `AI`                                           |
| `-v, --verbose`              | Enables verbose mode.                          | `False`                                        |
| `--version`                  | Displays version information.                  | (none)                                         |
| `-?, -h, --help`             | Displays help and usage information.           | (none)                                         |

## Environment Variables

Specify the environment variables that can be used to configure your app. Environment variables are particularly useful when you need to set configurations that are consistent across multiple runs or systems. For instance, they allow sensitive information, such as API keys or passwords, to be stored securely without hardcoding them into the application or passing them as command-line arguments.

| Name                         | Usage                                          | Accepted Values                                  |
|------------------------------|-----------------------------------------------|-------------------------------------------------|
| `OZEKI_AIPROMPT_URL`         | Specifies the URL of the server               | Any valid URL                                    |
| `OZEKI_AIPROMPT_USERNAME`    | Specifies the username for authentication     | Any valid username                               |
| `OZEKI_AIPROMPT_PASSWORD`    | Specifies the password for authentication     | Any valid password                               |
| `OZEKI_AIPROMPT_APIKEY`      | Specifies the API key                         | Any valid API key                                |
| `OZEKI_AIPROMPT_USE_JSON`    | Specifies if the prompt is in JSON format              | `True`, `False`                                  |
| `OZEKI_AIPROMPT_MODEL`       | Specifies the model name                      | Any valid model name                             |

The app supports the following scopes for environment variables:

- **Process:** Variables set for the current process.
- **User:** Variables set at the user level.
- **Machine:** Variables set at the system level and accessible by all users.

## Contact

For support or questions about the project, please contact:

- **Name:** Your Name
- **Email:** your.email@example.com
- **GitHub Issues:** [Open an issue](https://github.com/username/repository/issues)

## License

This project is licensed under the [MIT License](LICENSE).

