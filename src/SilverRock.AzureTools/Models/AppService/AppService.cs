using System.Collections.Generic;

namespace SilverRock.AzureTools.Models.AppService
{
	public class AppService
	{
		/// <summary>
		/// Gets or sets a collection of accounts which constitute this environment.
		/// </summary>
		public List<Account> Accounts { get; set; }
		/// <summary>
		/// Gets or sets the Application Settings associated with each of the accounts in this environment.
		/// </summary>
		public Dictionary<string, string> Settings { get; set; }
	}
}
