using SilverRock.AzureTools.Models;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverRock.AzureTools.Mappers
{
	internal static class SubscriberMapper
	{
		public static Subscription Map(SubscriptionDescription source)
		{
			return new Subscription
			{
				AutoDeleteOnIdle = source.AutoDeleteOnIdle,
				DefaultMessageTimeToLive = source.DefaultMessageTimeToLive,
				EnableBatchedOperations = source.EnableBatchedOperations,
				EnableDeadLetteringOnFilterEvaluationExceptions = source.EnableDeadLetteringOnFilterEvaluationExceptions,
				EnableDeadLetteringOnMessageExpiration = source.EnableDeadLetteringOnMessageExpiration,
				ForwardDeadLetteredMessagesTo = source.ForwardDeadLetteredMessagesTo,
				ForwardTo = source.ForwardTo,
				LockDuration = source.LockDuration,
				MaxDeliveryCount = source.MaxDeliveryCount,
				Name = source.Name,
				RequiresSession = source.RequiresSession,
				UserMetadata = source.UserMetadata
			};
		}

		public static SubscriptionDescription Map(Subscription source, string topicPath)
		{
			var result = new SubscriptionDescription(topicPath, source.Name);

			if (source.AutoDeleteOnIdle.HasValue)
				result.AutoDeleteOnIdle = source.AutoDeleteOnIdle.Value;

			if (source.DefaultMessageTimeToLive.HasValue)
				result.DefaultMessageTimeToLive = source.DefaultMessageTimeToLive.Value;

			if (source.EnableBatchedOperations.HasValue)
				result.EnableBatchedOperations = source.EnableBatchedOperations.Value;

			if (source.EnableDeadLetteringOnFilterEvaluationExceptions.HasValue)
				result.EnableDeadLetteringOnFilterEvaluationExceptions = source.EnableDeadLetteringOnFilterEvaluationExceptions.Value;

			if (source.EnableDeadLetteringOnMessageExpiration.HasValue)
				result.EnableDeadLetteringOnMessageExpiration = source.EnableDeadLetteringOnMessageExpiration.Value;

			result.ForwardDeadLetteredMessagesTo = source.ForwardDeadLetteredMessagesTo;

			result.ForwardTo = source.ForwardTo;

			if (source.LockDuration.HasValue)
				result.LockDuration = source.LockDuration.Value;

			if (source.MaxDeliveryCount.HasValue)
				result.MaxDeliveryCount = source.MaxDeliveryCount.Value;

			if (source.RequiresSession.HasValue)
				result.RequiresSession = source.RequiresSession.Value;

			result.UserMetadata = source.UserMetadata;

			return result;
		}
	}
}
