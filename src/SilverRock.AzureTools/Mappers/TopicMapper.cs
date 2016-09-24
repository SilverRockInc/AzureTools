using SilverRock.AzureTools.Models;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Linq;
using SilverRock.AzureTools.Models.ServiceBus;

namespace SilverRock.AzureTools.Mappers
{
	internal static class TopicMapper
	{
		public static Topic Map(TopicDescription source)
		{
			return new Topic
			{
				Authorization = source.Authorization.OfType<SharedAccessAuthorizationRule>().Select(Map).ToList(),
				AutoDeleteOnIdle = source.AutoDeleteOnIdle,
				DefaultMessageTimeToLive = source.DefaultMessageTimeToLive,
				DuplicateDetectionHistoryTimeWindow = source.DuplicateDetectionHistoryTimeWindow,
				EnableBatchedOperations = source.EnableBatchedOperations,
				EnableExpress = source.EnableExpress,
				EnableFilteringMessagesBeforePublishing = source.EnableFilteringMessagesBeforePublishing,
				EnablePartitioning = source.EnablePartitioning,
				MaxSizeInMegabytes = source.MaxSizeInMegabytes,
				RequiresDuplicateDetection = source.RequiresDuplicateDetection,
				SupportOrdering = source.SupportOrdering,
				UserMetadata = source.UserMetadata
			};
		}

		public static TopicDescription Map(Topic source)
		{
			var result = new TopicDescription(source.Path);

			if (source.AutoDeleteOnIdle.HasValue)
				result.AutoDeleteOnIdle = source.AutoDeleteOnIdle.Value;

			if (source.DefaultMessageTimeToLive.HasValue)
				result.DefaultMessageTimeToLive = source.DefaultMessageTimeToLive.Value;

			if (source.DuplicateDetectionHistoryTimeWindow.HasValue)
				result.DuplicateDetectionHistoryTimeWindow = source.DuplicateDetectionHistoryTimeWindow.Value;

			if (source.EnableBatchedOperations.HasValue)
				result.EnableBatchedOperations = source.EnableBatchedOperations.Value;

			if (source.EnableExpress.HasValue)
				result.EnableExpress = source.EnableExpress.Value;

			if (source.EnableFilteringMessagesBeforePublishing.HasValue)
				result.EnableFilteringMessagesBeforePublishing = source.EnableFilteringMessagesBeforePublishing.Value;

			if (source.EnablePartitioning.HasValue)
				result.EnablePartitioning = source.EnablePartitioning.Value;

			if (source.MaxSizeInMegabytes.HasValue)
				result.MaxSizeInMegabytes = source.MaxSizeInMegabytes.Value;

			if (source.RequiresDuplicateDetection.HasValue)
				result.RequiresDuplicateDetection = source.RequiresDuplicateDetection.Value;

			if (source.SupportOrdering.HasValue)
				result.SupportOrdering = source.SupportOrdering.Value;
			
			result.UserMetadata = source.UserMetadata;

			foreach (SharedAccessPolicy policy in source.Authorization ?? Enumerable.Empty<SharedAccessPolicy>())
				result.Authorization.Add(Map(policy));

			return result;
		}

		public static SharedAccessPolicy Map(SharedAccessAuthorizationRule source)
		{
			return new SharedAccessPolicy
			{
				Name = source.KeyName,
				AccessRights = source.Rights.Select(Map).ToList(),
				PrimaryKey = source.PrimaryKey,
				SecondaryKey = source.SecondaryKey
			};
		}

		public static SharedAccessAuthorizationRule Map(SharedAccessPolicy source)
		{
			if (source.SecondaryKey != null)
			{
				return new SharedAccessAuthorizationRule(
					source.Name,
					source.PrimaryKey,
					source.SecondaryKey,
					source.AccessRights?.Select(Map) ?? Enumerable.Empty<AccessRights>());
			}
			else if (source.PrimaryKey != null)
			{
				return new SharedAccessAuthorizationRule(
					source.Name,
					source.PrimaryKey,
					source.AccessRights?.Select(Map) ?? Enumerable.Empty<AccessRights>());
			}
			else
			{
				return new SharedAccessAuthorizationRule(
					source.Name,
					source.AccessRights?.Select(Map) ?? Enumerable.Empty<AccessRights>());
			}
		}

		public static AccessRight Map(AccessRights source)
		{
			switch (source)
			{
				case AccessRights.Listen: return AccessRight.Listen;
				case AccessRights.Manage: return AccessRight.Manage;
				case AccessRights.ManageNotificationHub: return AccessRight.ManageNotificationHub;
				case AccessRights.Send: return AccessRight.Send;
				default: throw new ArgumentOutOfRangeException(nameof(source));
			}
		}

		public static AccessRights Map(AccessRight source)
		{
			switch (source)
			{
				case AccessRight.Listen: return AccessRights.Listen;
				case AccessRight.Manage: return AccessRights.Manage;
				case AccessRight.ManageNotificationHub: return AccessRights.ManageNotificationHub;
				case AccessRight.Send: return AccessRights.Send;
				default: throw new ArgumentOutOfRangeException(nameof(source));
			}
		}
	}
}
