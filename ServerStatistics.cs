using System.Text.Json.Serialization;

#pragma warning disable CS8625

namespace Telegram.Bot.ApiServer
{
    /// <summary>
    /// Represents the statistics for the server.
    /// </summary>
    public sealed class ServerStatistics
    {
        /// <summary>
        /// Gets or sets the uptime of the server.
        /// </summary>
        [JsonPropertyName("uptime")]
        public double Uptime { get; set; }

        /// <summary>
        /// Gets or sets the count of bots on the server.
        /// </summary>
        [JsonPropertyName("bot_count")]
        public int BotCount { get; set; }

        /// <summary>
        /// Gets or sets the count of active bots on the server.
        /// </summary>
        [JsonPropertyName("active_bot_count")]
        public int ActiveBotCount { get; set; }

        /// <summary>
        /// Gets or sets the RSS memory usage of the server.
        /// </summary>
        [JsonPropertyName("rss")]
        public string Rss { get; set; }

        /// <summary>
        /// Gets or sets the VM memory usage of the server.
        /// </summary>
        [JsonPropertyName("vm")]
        public string Vm { get; set; }

        /// <summary>
        /// Gets or sets the peak RSS memory usage of the server.
        /// </summary>
        [JsonPropertyName("rss_peak")]
        public string RssPeak { get; set; }

        /// <summary>
        /// Gets or sets the peak VM memory usage of the server.
        /// </summary>
        [JsonPropertyName("vm_peak")]
        public string VmPeak { get; set; }

        /// <summary>
        /// Gets or sets the total CPU usage of the server.
        /// </summary>
        [JsonPropertyName("total_cpu")]
        public double TotalCpu { get; set; }

        /// <summary>
        /// Gets or sets the user CPU usage of the server.
        /// </summary>
        [JsonPropertyName("user_cpu")]
        public double UserCpu { get; set; }

        /// <summary>
        /// Gets or sets the system CPU usage of the server.
        /// </summary>
        [JsonPropertyName("system_cpu")]
        public double SystemCpu { get; set; }

        /// <summary>
        /// Gets or sets the buffer memory usage of the server.
        /// </summary>
        [JsonPropertyName("buffer_memory")]
        public string BufferMemory { get; set; }

        /// <summary>
        /// Gets or sets the count of active webhook connections on the server.
        /// </summary>
        [JsonPropertyName("active_webhook_connections")]
        public int ActiveWebhookConnections { get; set; }

        /// <summary>
        /// Gets or sets the count of active requests on the server.
        /// </summary>
        [JsonPropertyName("active_requests")]
        public int ActiveRequests { get; set; }

        /// <summary>
        /// Gets or sets the count of active network queries on the server.
        /// </summary>
        [JsonPropertyName("active_network_queries")]
        public int ActiveNetworkQueries { get; set; }

        /// <summary>
        /// Gets or sets the count of requests on the server.
        /// </summary>
        [JsonPropertyName("request_count")]
        public double RequestCount { get; set; }

        /// <summary>
        /// Gets or sets the count of bytes in requests on the server.
        /// </summary>
        [JsonPropertyName("request_bytes")]
        public double RequestBytes { get; set; }

        /// <summary>
        /// Gets or sets the count of files in requests on the server.
        /// </summary>
        [JsonPropertyName("request_file_count")]
        public double RequestFileCount { get; set; }

        /// <summary>
        /// Gets or sets the count of bytes in files in requests on the server.
        /// </summary>
        [JsonPropertyName("request_files_bytes")]
        public double RequestFilesBytes { get; set; }

        /// <summary>
        /// Gets or sets the maximum bytes in a single request on the server.
        /// </summary>
        [JsonPropertyName("request_max_bytes")]
        public int RequestMaxBytes { get; set; }

        /// <summary>
        /// Gets or sets the count of responses on the server.
        /// </summary>
        [JsonPropertyName("response_count")]
        public double ResponseCount { get; set; }

        /// <summary>
        /// Gets or sets the count of successful responses on the server.
        /// </summary>
        [JsonPropertyName("response_count_ok")]
        public double ResponseCountOk { get; set; }

        /// <summary>
        /// Gets or sets the count of error responses on the server.
        /// </summary>
        [JsonPropertyName("response_count_error")]
        public double ResponseCountError { get; set; }

        /// <summary>
        /// Gets or sets the count of bytes in responses on the server.
        /// </summary>
        [JsonPropertyName("response_bytes")]
        public double ResponseBytes { get; set; }

        /// <summary>
        /// Gets or sets the count of updates on the server.
        /// </summary>
        [JsonPropertyName("update_count")]
        public double UpdateCount { get; set; }
    }
}
