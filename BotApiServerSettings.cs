using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;

#pragma warning disable CS8625

namespace Telegram.Bot.ApiServer
{
    /// <summary>
    /// Represents the settings for running the Telegram Bot API server.
    /// </summary>
    [DebuggerDisplay("{BuildArguments()}")]
    public sealed class BotApiServerSettings
    {
        /// <summary>
        /// Gets or sets the API ID.
        /// </summary>
        [JsonPropertyName("api_id")]
        public int ApiId { get; set; }

        /// <summary>
        /// Gets or sets the API hash.
        /// </summary>
        [JsonPropertyName("api_hash")]
        public string ApiHash { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the server should run locally.
        /// </summary>
        [JsonPropertyName("local")]
        public bool Local { get; set; } = false;

        /// <summary>
        /// Gets or sets the HTTP port.
        /// </summary>
        [JsonPropertyName("http_port")]
        public int HttpPort { get; set; } = 8081;

        /// <summary>
        /// Gets or sets the HTTP stat port.
        /// </summary>
        [JsonPropertyName("http_stat_port")]
        public int? HttpStatPort { get; set; }

        /// <summary>
        /// Gets or sets the working directory.
        /// </summary>
        [JsonPropertyName("working_directory")]
        public string WorkingDirectory { get; set; } = Environment.CurrentDirectory;

        /// <summary>
        /// Gets or sets the temporary directory.
        /// </summary>
        [JsonPropertyName("temp_directory")]
        public string TempDirectory { get; set; } = Path.GetTempPath();

        /// <summary>
        /// Gets or sets the log file path.
        /// </summary>
        [JsonPropertyName("log_file_path")]
        public string? LogFilePath { get; set; }

        /// <summary>
        /// Gets or sets the verbosity level.
        /// </summary>
        [JsonPropertyName("verbosity")]
        public int? Verbosity { get; set; }

        /// <summary>
        /// Gets or sets the executable path.
        /// </summary>
        [JsonPropertyName("executable_path")]
        public string ExecutablePath { get; set; } = "telegram-bot-api.exe";

        /// <summary>
        /// Gets or sets the maximum number of connections.
        /// </summary>
        [JsonPropertyName("max_connections")]
        public int MaxConnections { get; set; } = 100;

        /// <summary>
        /// Gets or sets the CPU affinity.
        /// </summary>
        [JsonPropertyName("cpu_affinity")]
        public int CpuAffinity { get; set; } = -1; // Default to all available CPUs

        /// <summary>
        /// Gets or sets the proxy server.
        /// </summary>
        [JsonPropertyName("proxy_server")]
        public string? ProxyServer { get; set; }

        /// <summary>
        /// Gets or sets the maximum log file size.
        /// </summary>
        [JsonPropertyName("log_max_file_size")]
        public int LogMaxFileSize { get; set; } = 2000000000;

        /// <summary>
        /// Parses the command-line arguments and returns a <see cref="BotApiServerSettings"/> object.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        /// <returns>A <see cref="BotApiServerSettings"/> object.</returns>
        public static BotApiServerSettings Parse(string[] args)
        {
            var settings = new BotApiServerSettings(0, null); // Default values will be overridden
            for (int i = 0; i < args.Length; i++)
            {
                var notLastArg = i + 1 < args.Length;
                switch (args[i].ToLower())
                {
                    case "--api-id" when notLastArg && int.TryParse(args[i + 1], out var apiId):
                        settings.ApiId = apiId;
                        i++;
                        break;
                    case "--api-hash" when notLastArg:
                        settings.ApiHash = args[i + 1];
                        i++;
                        break;
                    case "--local":
                        settings.Local = true;
                        break;
                    case "--http-port" when notLastArg && int.TryParse(args[i + 1], out var httpPort):
                        settings.HttpPort = httpPort;
                        i++;
                        break;
                    case "--http-stat-port" when notLastArg && int.TryParse(args[i + 1], out var httpStatPort):
                        settings.HttpStatPort = httpStatPort;
                        i++;
                        break;
                    case "--temp-dir" when notLastArg:
                        settings.TempDirectory = args[i + 1];
                        i++;
                        break;
                    case "--log" when notLastArg:
                        settings.LogFilePath = args[i + 1];
                        i++;
                        break;
                    case "--verbosity" when notLastArg && int.TryParse(args[i + 1], out var verbosity):
                        settings.Verbosity = verbosity;
                        i++;
                        break;
                    case "--executable-path" when notLastArg:
                        settings.ExecutablePath = args[i + 1];
                        i++;
                        break;
                    case "--max-connections" when notLastArg && int.TryParse(args[i + 1], out var maxConnections):
                        settings.MaxConnections = maxConnections;
                        i++;
                        break;
                    case "--cpu-affinity" when notLastArg && int.TryParse(args[i + 1], out var cpuAffinity):
                        settings.CpuAffinity = cpuAffinity;
                        i++;
                        break;
                    case "--proxy" when notLastArg:
                        settings.ProxyServer = args[i + 1];
                        i++;
                        break;
                    case "--log-max-file-size" when notLastArg && int.TryParse(args[i + 1], out var logMaxFileSize):
                        settings.LogMaxFileSize = logMaxFileSize;
                        i++;
                        break;
                }
            }

            return settings;
        }

        /// <summary>
        /// Parses the command-line string and returns a <see cref="BotApiServerSettings"/> object.
        /// </summary>
        /// <param name="cmdLine">The command-line string.</param>
        /// <returns>A <see cref="BotApiServerSettings"/> object.</returns>
        public static BotApiServerSettings Parse(string cmdLine)
        {
            var args = cmdLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return Parse(args);
        }

        /// <summary>
        /// Builds the command-line arguments string based on the current settings.
        /// </summary>
        /// <returns>The command-line arguments string.</returns>
        internal string BuildArguments()
        {
            var args = new StringBuilder();

            args.Append($"--api-id={ApiId} ");
            args.Append($"--api-hash={ApiHash} ");

            if (Local)
                args.Append("--local ");

            args.Append($"--http-port={HttpPort} ");

            if (HttpStatPort.HasValue)
                args.Append($"--http-stat-port={HttpStatPort} ");

            if (!string.IsNullOrEmpty(TempDirectory))
                args.Append($"--temp-dir={TempDirectory} ");

            if (!string.IsNullOrEmpty(LogFilePath))
                args.Append($"--log={LogFilePath} ");

            if (Verbosity.HasValue)
                args.Append($"--verbosity={Verbosity} ");

            args.Append($"--max-connections={MaxConnections} ");

            if (CpuAffinity >= 0)
                args.Append($"--cpu-affinity={CpuAffinity} ");

            if (!string.IsNullOrEmpty(ProxyServer))
                args.Append($"--proxy={ProxyServer} ");

            args.Append($"--log-max-file-size={LogMaxFileSize} ");

            return args.ToString().Trim();
        }

        internal BotApiServerSettings() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BotApiServerSettings"/> class.
        /// </summary>
        /// <param name="apiId">The API ID.</param>
        /// <param name="apiHash">The API hash.</param>
        public BotApiServerSettings(int apiId, string apiHash)
        {
            ApiId = apiId;
            ApiHash = apiHash ?? throw new ArgumentNullException(nameof(apiHash));
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return BuildArguments();
        }
    }
}