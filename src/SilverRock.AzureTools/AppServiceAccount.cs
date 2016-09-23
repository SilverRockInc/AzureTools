using System.Threading.Tasks;

namespace SilverRock.AzureTools
{
	/// <summary>
	/// Base class for representing a deployment account (location and credentials)
	/// </summary>
	public abstract class AppServiceAccount
	{
		public AppServiceAccount(string serviceName, string username, string password)
		{
			ServiceName = serviceName;
			Username = username;
			Password = password;
		}

		protected string ServiceName { get; }
		protected string Username { get; }
		protected string Password { get; }

		internal abstract string GetResource(string resource);
		internal abstract Task<string> GetResourceAsync(string resource);
		internal abstract void PostResource(string settings, string jsonObj);
		internal abstract Task PostResourceAsync(string settings, string jsonObj);
	}
}
