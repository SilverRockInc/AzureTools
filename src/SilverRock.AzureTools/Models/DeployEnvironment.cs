using SilverRock.AzureTools.Models.AppService;
using SilverRock.AzureTools.Models.ServiceBus;

namespace SilverRock.AzureTools.Models
{
	public class DeployEnvironment
	{
		/// <summary>
		/// Gets or sets the name of the environment (eg. Production, Test, UAT, etc).
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the Service Bus Topics to create, update, or remove.
		/// </summary>
		public Topics Topics { get; set; }

		/// <summary>
		/// Gets or sets the App Services to create, update, or remove.
		/// </summary>
		public AppServices AppServices { get; set; }
	}
}
