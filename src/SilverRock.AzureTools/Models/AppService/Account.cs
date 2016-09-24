namespace SilverRock.AzureTools.Models.AppService
{
	public class Account
	{
		/// <summary>
		/// Gets or sets the name of the Azure App Service.  This is the name that is incorporated in the
		/// app service URI: https://{service-name}.azurewebsites.net/
		/// </summary>
		public string ServiceName { get; set; }

		/// <summary>
		/// Gets or sets the username of the App Services Deployment Credentials.
		/// </summary>
		public string Username { get; set; }

		/// <summary>
		/// Gets or sets the password of the App Services Deployment Credentials.
		/// </summary>
		public string Password { get; set; }
	}
}
