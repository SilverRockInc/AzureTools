using Microsoft.ServiceBus.Messaging;
using System;

namespace SilverRock.AzureTools
{
	public class BrokeredMessageEventArgs : EventArgs
	{
		public BrokeredMessageEventArgs(BrokeredMessage message)
		{
			Message = message;
		}

		public BrokeredMessage Message { get; }
	}
}