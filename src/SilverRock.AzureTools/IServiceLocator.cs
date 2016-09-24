using Microsoft.ServiceBus;
using SilverRock.AzureTools.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverRock.AzureTools
{
	public interface IServiceLocator
	{
		INamespaceService GetNamespaceService(string endpoint, string accessKeyName, string accessKey);
		AppServiceAccount GetAppServiceAccount(string serviceName, string username, string password);
	}

	public sealed class DefaultServiceLocator : IServiceLocator
	{
		public AppServiceAccount GetAppServiceAccount(string serviceName, string username, string password)
		{
			return new AzureAppServiceAccount(serviceName, username, password);
		}

		public INamespaceService GetNamespaceService(string endpoint, string accessKeyName, string accessKey)
		{
			return new AzureNamespaceService(NamespaceManager.CreateFromConnectionString(CreateConnectionString(endpoint, accessKeyName, accessKey)));
		}

		internal static string CreateConnectionString(string endpoint, string accessKeyName, string accessKey)
		{
			return $"Endpoint={endpoint};SharedAccessKeyName={accessKeyName};SharedAccessKey={accessKey}";
		}
	}
}
