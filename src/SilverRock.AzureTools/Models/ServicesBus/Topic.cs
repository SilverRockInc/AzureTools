using SilverRock.AzureTools.Models.ServicesBus;
using System;
using System.Collections.Generic;

namespace SilverRock.AzureTools.Models.ServiceBus
{
	public class Topic
	{
		public Namespace Namespace { get; set; }

		public string Path { get; set; }
		public List<SharedAccessPolicy> Authorization { get; set; }
		public TimeSpan? AutoDeleteOnIdle { get; set; }
		public TimeSpan? DefaultMessageTimeToLive { get; set; }
		public TimeSpan? DuplicateDetectionHistoryTimeWindow { get; set; }
		public bool? EnableBatchedOperations { get; set; }
		public bool? EnableExpress { get; set; }
		public bool? EnableFilteringMessagesBeforePublishing { get; set; }
		public bool? EnablePartitioning { get; set; }
		public long? MaxSizeInMegabytes { get; set; }
		public bool? RequiresDuplicateDetection { get; set; }
		public bool? SupportOrdering { get; set; }
		public string UserMetadata { get; set; }

		public List<Subscription> Subscriptions { get; set; }
	}

	public class SharedAccessPolicy
	{
		public string Name { get; set; }
		public string PrimaryKey { get; set; }
		public string SecondaryKey { get; set; }
		public List<AccessRight> AccessRights { get; set; }
	}

	public enum AccessRight
	{
		Manage,
		Send,
		Listen,
		ManageNotificationHub
	}
}
