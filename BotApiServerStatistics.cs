using System.Collections;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

#pragma warning disable CS8625

namespace Telegram.Bot.ApiServer
{
    /// <summary>
    /// Represents the combined statistics for the server and multiple bots.
    /// </summary>
    public sealed class BotApiServerStatistics : IEnumerable<BotStatistics>
    {
        private ServerStatistics _server = new ServerStatistics();
        private List<BotStatistics> _bots = new List<BotStatistics>(1) { new BotStatistics() };

        /// <summary>
        /// Initializes a new instance of the <see cref="BotApiServerStatistics"/> class.
        /// </summary>
        internal BotApiServerStatistics() { }

        /// <summary>
        /// Gets or sets the server statistics.
        /// </summary>
        [JsonPropertyName("server")]
        public ServerStatistics Server
        {
            get => _server;
            set => _server = value;
        }

        /// <summary>
        /// Gets or sets the list of bot statistics.
        /// </summary>
        [JsonPropertyName("bots")]
        public List<BotStatistics> Bots
        {
            get => _bots;
            set => _bots = value;
        }

        /// <summary>
        /// Attempts to parse the given response string and populate the <see cref="BotApiServerStatistics"/> object.
        /// </summary>
        /// <param name="response">The response string to parse.</param>
        /// <param name="statistics">The <see cref="BotApiServerStatistics"/> object to populate.</param>
        /// <returns>True if parsing was successful; otherwise, false.</returns>
        public static bool TryParse(string response, out BotApiServerStatistics statistics)
        {
            try
            {
                statistics = Parse(response);
                return true;
            }
            catch
            {
                statistics = default;
                return false;
            }
        }

        /// <summary>
        /// Parses the given response string and returns a new <see cref="BotApiServerStatistics"/> object.
        /// </summary>
        /// <param name="response">The response string to parse.</param>
        /// <returns>A new <see cref="BotApiServerStatistics"/> object.</returns>
        public static BotApiServerStatistics Parse(string response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            if (string.IsNullOrWhiteSpace(response))
                throw new ArgumentException("Response cannot be empty or whitespace.", nameof(response));

            var serverStats = new ServerStatistics();
            var botStatsList = new List<BotStatistics>();
            BotStatistics currentBotStats = null;

            var lines = response.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var parts = line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2) continue;

                var key = parts[0].Trim().ToLower();
                var value = parts[1].Trim();
                var notEmptyBotList = currentBotStats != null;

                switch (key)
                {
                    case "uptime" when double.TryParse(value, out var uptime):
                        serverStats.Uptime = uptime;
                        break;
                    case "bot_count" when int.TryParse(value, out var botCount):
                        serverStats.BotCount = botCount;
                        break;
                    case "active_bot_count" when int.TryParse(value, out var activeBotCount):
                        serverStats.ActiveBotCount = activeBotCount;
                        break;
                    case "rss":
                        serverStats.Rss = value;
                        break;
                    case "vm":
                        serverStats.Vm = value;
                        break;
                    case "rss_peak":
                        serverStats.RssPeak = value;
                        break;
                    case "vm_peak":
                        serverStats.VmPeak = value;
                        break;
                    case "total_cpu" when TryParseCpuUsage(value, out var totalCpu):
                        serverStats.TotalCpu = totalCpu;
                        break;
                    case "user_cpu" when TryParseCpuUsage(value, out var userCpu):
                        serverStats.UserCpu = userCpu;
                        break;
                    case "system_cpu" when TryParseCpuUsage(value, out var systemCpu):
                        serverStats.SystemCpu = systemCpu;
                        break;
                    case "buffer_memory":
                        serverStats.BufferMemory = value;
                        break;
                    case "active_webhook_connections" when int.TryParse(value, out var activeWebhookConnections):
                        serverStats.ActiveWebhookConnections = activeWebhookConnections;
                        break;
                    case "active_requests" when int.TryParse(value, out var activeRequests):
                        serverStats.ActiveRequests = activeRequests;
                        break;
                    case "active_network_queries" when int.TryParse(value, out var activeNetworkQueries):
                        serverStats.ActiveNetworkQueries = activeNetworkQueries;
                        break;
                    case "request_count" when double.TryParse(value, out var requestCount):
                        serverStats.RequestCount = requestCount;
                        break;
                    case "request_bytes" when double.TryParse(value, out var requestBytes):
                        serverStats.RequestBytes = requestBytes;
                        break;
                    case "request_file_count" when double.TryParse(value, out var requestFileCount):
                        serverStats.RequestFileCount = requestFileCount;
                        break;
                    case "request_files_bytes" when double.TryParse(value, out var requestFilesBytes):
                        serverStats.RequestFilesBytes = requestFilesBytes;
                        break;
                    case "request_max_bytes" when int.TryParse(value, out var requestMaxBytes):
                        serverStats.RequestMaxBytes = requestMaxBytes;
                        break;
                    case "response_count" when double.TryParse(value, out var responseCount):
                        serverStats.ResponseCount = responseCount;
                        break;
                    case "response_count_ok" when double.TryParse(value, out var responseCountOk):
                        serverStats.ResponseCountOk = responseCountOk;
                        break;
                    case "response_count_error" when double.TryParse(value, out var responseCountError):
                        serverStats.ResponseCountError = responseCountError;
                        break;
                    case "response_bytes" when double.TryParse(value, out var responseBytes):
                        serverStats.ResponseBytes = responseBytes;
                        break;
                    case "update_count" when double.TryParse(value, out var updateCount):
                        serverStats.UpdateCount = updateCount;
                        break;
                    case "id" when long.TryParse(value, out var id):
                        currentBotStats = new BotStatistics { Id = id };
                        botStatsList.Add(currentBotStats);
                        break;
                    case "token" when notEmptyBotList:
                        currentBotStats.Token = value;
                        break;
                    case "username" when notEmptyBotList:
                        currentBotStats.Username = value;
                        break;
                    case "active_request_count" when notEmptyBotList && int.TryParse(value, out var activeRequestCount):
                        currentBotStats.ActiveRequestCount = activeRequestCount;
                        break;
                    case "head_update_id" when notEmptyBotList && long.TryParse(value, out var headUpdateId):
                        currentBotStats.HeadUpdateId = headUpdateId;
                        break;
                    case "request_count/sec" when notEmptyBotList && double.TryParse(value, out var requestCountPerSec):
                        currentBotStats.RequestCountPerSec = requestCountPerSec;
                        break;
                    case "update_count/sec" when notEmptyBotList && double.TryParse(value, out var updateCountPerSec):
                        currentBotStats.UpdateCountPerSec = updateCountPerSec;
                        break;
                }
            }

            return new BotApiServerStatistics
            {
                Server = serverStats,
                Bots = botStatsList
            };
        }

        /// <summary>
        /// Attempts to parse the CPU usage value from the given string.
        /// </summary>
        /// <param name="value">The string to parse.</param>
        /// <param name="cpuUsage">The output parameter to receive the parsed CPU usage value.</param>
        /// <returns>True if parsing was successful; otherwise, false.</returns>
        private static bool TryParseCpuUsage(string value, out double cpuUsage)
        {
            cpuUsage = 0d;
            var match = Regex.Match(value, @"(\d+\.\d+)%");
            return match.Success && double.TryParse(match.Groups[1].Value, out cpuUsage);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="BotStatistics"/> collection.
        /// </summary>
        /// <returns>An enumerator that iterates through the <see cref="BotStatistics"/> collection.</returns>
        public IEnumerator<BotStatistics> GetEnumerator()
        {
            return new InternalEnumerator(Bots);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="BotStatistics"/> collection.
        /// </summary>
        /// <returns>An enumerator that iterates through the <see cref="BotStatistics"/> collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #region Enumerator

        /// <summary>
        /// Represents the internal enumerator for the <see cref="BotApiServerStatistics"/> class.
        /// </summary>
        private class InternalEnumerator : IEnumerator<BotStatistics>
        {
            private readonly List<BotStatistics> _bots;
            private int _currentIndex = -1;

            /// <summary>
            /// Initializes a new instance of the <see cref="InternalEnumerator"/> class.
            /// </summary>
            /// <param name="bots">The list of <see cref="BotStatistics"/> to enumerate.</param>
            public InternalEnumerator(List<BotStatistics> bots)
            {
                _bots = bots ?? throw new ArgumentNullException(nameof(bots));
            }

            /// <summary>
            /// Gets the current <see cref="BotStatistics"/> object.
            /// </summary>
            public BotStatistics Current => _bots[_currentIndex];

            /// <summary>
            /// Gets the current object.
            /// </summary>
            object IEnumerator.Current => Current;

            /// <summary>
            /// Disposes the enumerator.
            /// </summary>
            public void Dispose()
            {
                // No resources to dispose
            }

            /// <summary>
            /// Advances the enumerator to the next element.
            /// </summary>
            /// <returns>True if the enumerator was successfully advanced to the next element; otherwise, false.</returns>
            public bool MoveNext()
            {
                _currentIndex++;
                return _currentIndex < _bots.Count;
            }

            /// <summary>
            /// Sets the enumerator to its initial position.
            /// </summary>
            public void Reset()
            {
                _currentIndex = -1;
            }
        }

        #endregion
    }
}