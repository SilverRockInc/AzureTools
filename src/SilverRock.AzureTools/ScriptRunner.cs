using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using SilverRock.AzureTools.Models;
using SilverRock.AzureTools.Models.AppService;
using SilverRock.AzureTools.Models.ServiceBus;
using System;
using System.Linq;

namespace SilverRock.AzureTools
{
	public class ScriptRunner
	{
		public ScriptRunner() : this(new DefaultServiceLocator()) { }

		public ScriptRunner(IServiceLocator serviceLocator)
		{
			_serviceLocator = serviceLocator;
		}

		public void Run(Script script, bool force = false)
		{
			if (script.DeployEnvironments != null)
			{
				foreach (DeployEnvironment env in script.DeployEnvironments)
				{
					OnMessage($"Configuring deployment environment '{env.Name}' ... " + Environment.NewLine + Environment.NewLine);

					if (env.AppServices != null)
						Run(env.AppServices, force);

					if (env.Topics != null)
						Run(env.Topics, force);
				}
			}
		}

		internal void Run(AppServices appServices, bool force = false)
		{
			if (appServices.Create != null)
			{
				OnMessage($"Creating {appServices.Create.Count} App Services ... " + Environment.NewLine + Environment.NewLine);

				foreach (AppService appService in appServices.Create ?? Enumerable.Empty<AppService>())
				{
					CreateAppService(appService, force);
				}
			}

			if (appServices.Update != null)
			{
				OnMessage($"Updating {appServices.Update.Count} App Services ... " + Environment.NewLine + Environment.NewLine);

				foreach (AppService appService in appServices.Update ?? Enumerable.Empty<AppService>())
				{
					UpdateAppService(appService, force);
				}
			}

			if (appServices.Remove != null)
			{
				OnMessage($"Removing {appServices.Remove.Count} App Services ... " + Environment.NewLine + Environment.NewLine);

				foreach (AppService appService in appServices.Remove ?? Enumerable.Empty<AppService>())
				{
					RemoveAppService(appService);
				}
			}
		}

		internal void Run(Topics topics, bool force = false)
		{
			if (topics.Create != null)
			{
				OnMessage($"Creating {topics.Create.Count} topics ... " + Environment.NewLine + Environment.NewLine);

				foreach (Topic topic in topics.Create ?? Enumerable.Empty<Topic>())
				{
					CreateTopic(topic, force);
				}
			}

			if (topics.Update != null)
			{
				OnMessage($"Updating {topics.Update.Count} topics ... " + Environment.NewLine + Environment.NewLine);

				foreach (Topic topic in topics.Update ?? Enumerable.Empty<Topic>())
				{
					UpdateTopic(topic, force);
				}
			}

			if (topics.Remove != null)
			{
				OnMessage($"Removing {topics.Remove.Count} topics ... " + Environment.NewLine + Environment.NewLine);

				foreach (Topic topic in topics.Remove ?? Enumerable.Empty<Topic>())
				{
					RemoveTopic(topic);
				}
			}
		}

		public void CreateTopic(Topic topic, bool force = false)
		{
			INamespaceService ns = _serviceLocator.GetNamespaceService(topic.Namespace.Endpoint, topic.Namespace.AccessKeyName, topic.Namespace.AccessKey); // new AzureNamespaceService(NamespaceManager.CreateFromConnectionString(CreateConnectionString(topic.Namespace.Endpoint, topic.Namespace.AccessKeyName, topic.Namespace.AccessKey)));

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
					return;
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

		public void UpdateTopic(Topic topic, bool force = false)
		{
			throw new NotImplementedException();
		}

		public void RemoveTopic(Topic topic)
		{
			throw new NotImplementedException();
		}

		public void CreateAppService(AppService appService, bool force = false)
		{
			throw new NotImplementedException();
		}

		public void UpdateAppService(AppService appService, bool force = false)
		{
			OnMessage($"Confinuring {appService.Settings.Count} settings for {appService.Accounts.Count} App Service Deployment Accounts ... " + Environment.NewLine + Environment.NewLine);

			foreach (Account account in appService.Accounts)
			{
				OnMessage($"Confinuring settings for '{account.ServiceName}' ... " );
				AppServiceAccount azureAccount = _serviceLocator.GetAppServiceAccount(account.ServiceName, account.Username, account.Password);
				AppServiceClient client = new AppServiceClient(azureAccount);
				client.SetSettings(appService.Settings);
				OnMessage("done" + Environment.NewLine);
			}
		}

		public void RemoveAppService(AppService appService, bool force = false)
		{
			throw new NotImplementedException();
		}

		public Script ParseScript(string script)
		{
			return JsonConvert.DeserializeObject<Script>(script);
		}

		public event EventHandler<MessageEventArgs> Message;

		protected virtual void OnMessage(string message)
		{
			Message?.Invoke(this, new MessageEventArgs(message));
		}

		readonly IServiceLocator _serviceLocator;
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
