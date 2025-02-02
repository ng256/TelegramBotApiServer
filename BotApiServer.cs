using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

namespace Telegram.Bot.ApiServer
{

    /// <summary>
    /// Represents the managed version of the Telegram Bot API server, which controls the lifecycle of the server process.
    /// </summary>
    public sealed class BotApiServer : IDisposable, IAsyncDisposable, IEquatable<BotApiServer>
    {
        private int _pid = -1;
        private readonly BotApiServerSettings _settings;
        private Process _process;
        private bool _disposed;

        /// <summary>
        /// Indicates that server process is running now.
        /// </summary>
        public bool Running => IsRunningInternal();

        /// <summary>
        /// Initializes a new instance of the <see cref="BotApiServer"/> class.
        /// </summary>
        /// <param name="settings">The settings for the server.</param>
        public BotApiServer(BotApiServerSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        // Creates a HTTP client.
        private static HttpClient GetHttpClient()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
                "AppleWebKit/537.36 (KHTML, like Gecko) " +
                "Chrome/89.0.4389.114 Safari/537.36 Edg/89.0.774.76");
            return httpClient;
        }

        /// <summary>
        /// Starts the Telegram Bot API server synchronously.
        /// </summary>
        /// <param name="progress">The progress reporter.</param>
        public void Start(IProgress<string> progress = null)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(BotApiServer));

            if (IsRunningInternal())
                throw new InvalidOperationException("The server is already running.");

            var startInfo = new ProcessStartInfo
            {
                FileName = _settings.ExecutablePath,
                Arguments = _settings.BuildArguments(),
                WorkingDirectory = _settings.WorkingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            _process = new Process { StartInfo = startInfo };

            _process.OutputDataReceived += (s, e) => HandleOutput(e, progress);
            _process.ErrorDataReceived += (s, e) => HandleError(e, progress);

            _process.Start();
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
            _pid = _process.Id;
        }

        /// <summary>
        /// Starts the Telegram Bot API server asynchronously.
        /// </summary>
        /// <param name="progress">The progress reporter.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task StartAsync(IProgress<string> progress = null, CancellationToken cancellationToken = default)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(BotApiServer));

            await Task.Run(() => Start(progress), cancellationToken);
        }

        /// <summary>
        /// Stops the Telegram Bot API server synchronously.
        /// </summary>
        public void Stop()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(BotApiServer));

            if (!IsRunningInternal())
                throw new InvalidOperationException("The server is not running.");

            _process.Kill();
            _process.WaitForExit();
            _process.Dispose();
            _process = null;
            _pid = -1;
            _disposed = true;
        }

        /// <summary>
        /// Stops the Telegram Bot API server asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(BotApiServer));

            await Task.Run(Stop, cancellationToken);
        }

        /// <summary>
        /// Gets the version of the Telegram Bot API server.
        /// </summary>
        /// <returns>The version of the server.</returns>
        public string GetVersion()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(BotApiServer));

            var startInfo = new ProcessStartInfo
            {
                FileName = _settings.ExecutablePath,
                Arguments = "--version",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            return process?.StandardOutput.ReadToEnd()?.Trim() ?? "Unknown version";
        }

        /// <summary>
        /// Gets the statistics of the Telegram Bot API server.
        /// </summary>
        /// <returns>The statistics of the server.</returns>
        public BotApiServerStatistics GetStatistics()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(BotApiServer));

            if (!IsRunningInternal())
                throw new InvalidOperationException("The server is not running.");

            if (!_settings.HttpStatPort.HasValue)
                throw new InvalidOperationException("HttpStatPort is not set.");

            var httpClient = GetHttpClient();
            var url = $"http://localhost:{_settings.HttpStatPort}";

            try
            {
                var response = httpClient.GetStringAsync(url).Result;
                return BotApiServerStatistics.Parse(response);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to retrieve statistics.", ex);
            }
        }

        /// <summary>
        /// Gets the statistics of the Telegram Bot API server asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task<BotApiServerStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(BotApiServer));

            if (!IsRunningInternal())
                throw new InvalidOperationException("The server is not running.");

            if (!_settings.HttpStatPort.HasValue)
                throw new InvalidOperationException("HttpStatPort is not set.");

            var httpClient = new HttpClient();
            var url = $"http://localhost:{_settings.HttpStatPort}";

            try
            {
                var response = await httpClient.GetStringAsync(url, cancellationToken);
                return BotApiServerStatistics.Parse(response);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to retrieve statistics.", ex);
            }
        }

        // Handles the output data received from the process.
        private void HandleOutput(DataReceivedEventArgs e, IProgress<string> progress)
        {
            if (e.Data is { } data)
            {
                progress?.Report($"[Output] {data}");
            }
        }

        // Handles the error data received from the process
        private void HandleError(DataReceivedEventArgs e, IProgress<string> progress)
        {
            if (e.Data is { } data)
            {
                progress?.Report($"[Error] {data}");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsRunningInternal()
        {
            return _pid < 0 || _process == null || _process.HasExited;
        }

        /// <summary>
        /// Indicates whether the current instance is equal to another <see cref="BotApiServer"/> instance.
        /// </summary>
        /// <param name="other">
        /// The other <see cref="BotApiServer"/> instance to compare with.</param>
        /// <returns>
        /// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter;
        /// otherwise, <see langword="false" />.
        /// </returns>
        public bool Equals(BotApiServer? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _pid == other._pid;
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        /// <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object? obj)
        {
            return obj is BotApiServer other && Equals(other);
        }

        /// <summary>
        /// Disposes the resources used by the <see cref="BotApiServer"/> class.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            if (_process is { HasExited: false })
            {
                _process.Kill();
                _process.WaitForExit();
            }

            _process?.Dispose();
            _process = null;
            _disposed = true;
        }

        /// <summary>
        /// Disposes the resources used by the <see cref="BotApiServer"/> class asynchronously.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous dispose operation.
        /// </returns>
        public async ValueTask DisposeAsync()
        {
            if (_process is IAsyncDisposable asyncDisposable)
                await asyncDisposable.DisposeAsync();
            else
                _process.Dispose();
        }
    }
}