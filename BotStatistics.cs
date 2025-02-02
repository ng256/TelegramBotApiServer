using System.Text.Json.Serialization;

#pragma warning disable CS8625

namespace Telegram.Bot.ApiServer
{
    /// <summary>
    /// Represents the statistics for connected bot.
    /// </summary>
    public sealed class BotStatistics
    {
        /// <summary>
        /// Gets or sets the ID of the bot.
        /// </summary>
        [JsonPropertyName("id")]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the uptime of the bot.
        /// </summary>
        [JsonPropertyName("uptime")]
        public double Uptime { get; set; }

        /// <summary>
        /// Gets or sets the token of the bot.
        /// </summary>
        [JsonPropertyName("token")]
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the username of the bot.
        /// </summary>
        [JsonPropertyName("username")]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the count of active requests for the bot.
        /// </summary>
        [JsonPropertyName("active_request_count")]
        public int ActiveRequestCount { get; set; }

        /// <summary>
        /// Gets or sets the ID of the head update for the bot.
        /// </summary>
        [JsonPropertyName("head_update_id")]
        public long HeadUpdateId { get; set; }

        /// <summary>
        /// Gets or sets the request count per second for the bot.
        /// </summary>
        [JsonPropertyName("request_count_per_sec")]
        public double RequestCountPerSec { get; set; }

        /// <summary>
        /// Gets or sets the update count per second for the bot.
        /// </summary>
        [JsonPropertyName("update_count_per_sec")]
        public double UpdateCountPerSec { get; set; }
    }
}