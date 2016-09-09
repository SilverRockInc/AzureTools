using Microsoft.ServiceBus.Messaging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SilverRock.AzureTools
{
	/// <summary>
	/// Provides functionality for long-polling of Service Bus Subscriptions.
	/// </summary>
	public sealed class SubscriptionPoller
	{
		/// <summary>
		/// Creates a new subscription poller.
		/// </summary>
		/// <param name="client">Subscription client</param>
		/// <param name="rate">Long-polling duration</param>
		public SubscriptionPoller(SubscriptionClient client, TimeSpan rate)
		{
			_client = client;
			_rate = rate;
		}

		public event EventHandler<BrokeredMessageEventArgs> MessageReceived;

		private void OnMessageReceived(BrokeredMessage message)
		{
			MessageReceived?.Invoke(this, new BrokeredMessageEventArgs(message));
		}

		/// <summary>
		/// Begins listening for BrokeredMessages.  This task will complete if cancellation
		/// is requested via the CancellationToken or via WEBJOBS_SHUTDOWN_FILE environment
		/// variable.
		/// </summary>
		/// <param name="token">Cancellation token.</param>
		/// <returns></returns>
		public async Task Listen(CancellationToken token = default(CancellationToken))
		{
			// See: http://blog.amitapple.com/post/2014/05/webjobs-graceful-shutdown/#.V9L2UJMrJTY

			// Get the shutdown file path from the environment
			_shutdownFile = Environment.GetEnvironmentVariable(WEBJOBS_SHUTDOWN_FILE);

			if (!string.IsNullOrWhiteSpace(_shutdownFile))
			{
				// Setup a file system watcher on that file's directory to know when the file is created
				var fileSystemWatcher = new FileSystemWatcher(Path.GetDirectoryName(_shutdownFile));
				fileSystemWatcher.Created += OnShutdownFileChanged;
				fileSystemWatcher.Changed += OnShutdownFileChanged;
				fileSystemWatcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.LastWrite;
				fileSystemWatcher.IncludeSubdirectories = false;
				fileSystemWatcher.EnableRaisingEvents = true;
			}

			while (_running && !token.IsCancellationRequested)
			{
				BrokeredMessage message = await _client.ReceiveAsync(_rate);

				if (!_running || token.IsCancellationRequested)
					break;
				else if (!ReferenceEquals(message, null))
					OnMessageReceived(message);
			}
		}

		private void OnShutdownFileChanged(object sender, FileSystemEventArgs e)
		{
			if (e.FullPath.IndexOf(Path.GetFileName(_shutdownFile), StringComparison.OrdinalIgnoreCase) >= 0)
			{
				_running = false;
			}
		}

		const string WEBJOBS_SHUTDOWN_FILE = "WEBJOBS_SHUTDOWN_FILE";
		string _shutdownFile;
		bool _running = true;
		readonly SubscriptionClient _client;
		readonly TimeSpan _rate;
	}
}
