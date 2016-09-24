using System.Collections.Generic;

namespace SilverRock.AzureTools.Models.ServiceBus
{
	public class Topics
	{
		public List<Topic> Create { get; set; }
		public List<Topic> Update { get; set; }
		public List<Topic> Remove { get; set; }
	}
}
