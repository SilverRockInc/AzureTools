using SilverRock.AzureTools.Models;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverRock.AzureTools
{
	public class ScriptRunner
	{
		public ScriptRunner(string endpoint, string accessKeyName, string accessKey)
		{
			_endpoint = endpoint;
			_accessKeyName = accessKeyName;
			_accessKey = accessKey;
		}

		public void Create(string script, bool force = false)
		{
			Script s = ParseScript(script);

			NamespaceManager ns = NamespaceManager.CreateFromConnectionString(CreateConnectionString(_endpoint, _accessKeyName, _accessKey));

			OnMessage($"Creating {s.Topics.Count} topics ... " + Environment.NewLine + Environment.NewLine);

			foreach (Topic topic in s.Topics ?? Enumerable.Empty<Topic>())
			{
				if (ns.TopicExists(topic.Path))
				{
					if (force)
					{
						OnMessage($"Topic '{topic.Path}' already exists.  Deleting ... ");
						ns.DeleteTopic(topic.Path);
						OnMessage("done" + Environment.NewLine);
					}
					else
					{
						OnMessage($"Topic '{topic.Path}' already exists.  Skipping ... " + Environment.NewLine);
						continue;
					}
				}

				

				TopicDescription topicDescription = Mappers.TopicMapper.Map(topic);

				OnMessage($"Creating topic '{topic.Path}' ... ");
				ns.CreateTopic(topicDescription);
				OnMessage("done" + Environment.NewLine);

				foreach (Subscription subscription in topic.Subscriptions ?? Enumerable.Empty<Subscription>())
				{
					SubscriptionDescription subscriptionDescription = Mappers.SubscriberMapper.Map(subscription, topic.Path);

					OnMessage($"Creating subscription '{subscription.Name}' for topic '{topic.Path}' ... ");
					if (subscription.SqlFilter == null)
						ns.CreateSubscription(subscriptionDescription);
					else
						ns.CreateSubscription(subscriptionDescription, new SqlFilter(subscription.SqlFilter));
					OnMessage("done" + Environment.NewLine);
				}
			}
		}

		public Script ParseScript(string script)
		{
			return JsonConvert.DeserializeObject<Script>(script);
		}

		internal static string CreateConnectionString(string endpoint, string accessKeyName, string accessKey)
		{
			return $"Endpoint={endpoint};SharedAccessKeyName={accessKeyName};SharedAccessKey={accessKey}";
		}

		public event EventHandler<MessageEventArgs> Message;

		protected virtual void OnMessage(string message)
		{
			Message?.Invoke(this, new MessageEventArgs(message));
		}

		readonly string _endpoint;
		readonly string _accessKeyName;
		readonly string _accessKey;
	}

	public class MessageEventArgs : EventArgs
	{
		public MessageEventArgs(string message)
		{
			Message = message;
		}

		public string Message { get; }
	}
}
