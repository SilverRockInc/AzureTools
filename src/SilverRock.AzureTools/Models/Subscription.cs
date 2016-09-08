using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverRock.AzureTools.Models
{
	public class Subscription
	{
		public TimeSpan? AutoDeleteOnIdle { get; set; }
		public TimeSpan? DefaultMessageTimeToLive { get; set; }
		public bool? EnableBatchedOperations { get; set; }
		public bool? EnableDeadLetteringOnFilterEvaluationExceptions { get; set; }
		public bool? EnableDeadLetteringOnMessageExpiration { get; set; }
		public string ForwardDeadLetteredMessagesTo { get; set; }
		public string ForwardTo { get; set; }
		public TimeSpan? LockDuration { get; set; }
		public int? MaxDeliveryCount { get; set; }
		public string Name { get; set; }
		public bool? RequiresSession { get; set; }
		public string UserMetadata { get; set; }

		public string SqlFilter { get; set; }
	}
}
