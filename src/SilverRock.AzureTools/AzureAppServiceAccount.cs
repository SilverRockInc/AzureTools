using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SilverRock.AzureTools
{
	public sealed class AzureAppServiceAccount : AppServiceAccount
	{
		public AzureAppServiceAccount(string serviceName, string username, string password) : base(serviceName, username, password)
		{
			_authHeader = GetAuthHeader(username, password);
			_endpoint = GetEndpoint(serviceName);
		}

		internal override string GetResource(string resource)
		{
			HttpWebRequest req = WebRequest.CreateHttp($"{_endpoint}api/{resource}");
			req.Headers["Authorization"] = _authHeader;

			using (HttpWebResponse res = (HttpWebResponse)req.GetResponse())
			using (Stream stream = res.GetResponseStream())
			using (StreamReader reader = new StreamReader(stream))
				return reader.ReadToEnd();
		}

		internal override async Task<string> GetResourceAsync(string resource)
		{
			HttpWebRequest req = WebRequest.CreateHttp($"{_endpoint}api/{resource}");
			req.Headers["Authorization"] = _authHeader;

			using (HttpWebResponse res = (HttpWebResponse)(await req.GetResponseAsync()))
			using (Stream stream = res.GetResponseStream())
			using (StreamReader reader = new StreamReader(stream))
				return await reader.ReadToEndAsync();
		}

		internal override void PostResource(string resource, string jsonObj)
		{
			HttpWebRequest req = WebRequest.CreateHttp($"{_endpoint}api/{resource}");
			req.Headers["Authorization"] = _authHeader;
			req.Method = "POST";
			req.ContentType = "application/json";

			using (var streamWriter = new StreamWriter(req.GetRequestStream()))
			{
				streamWriter.Write(jsonObj);
				streamWriter.Flush();
				streamWriter.Close();
			}

			using (HttpWebResponse res = (HttpWebResponse)req.GetResponse())
			using (Stream stream = res.GetResponseStream())
			using (StreamReader reader = new StreamReader(stream))
				reader.ReadToEnd();
		}

		internal override async Task PostResourceAsync(string resource, string jsonObj)
		{
			HttpWebRequest req = WebRequest.CreateHttp($"{_endpoint}api/{resource}");
			req.Headers["Authorization"] = _authHeader;
			req.Method = "POST";
			req.ContentType = "application/json";

			using (var streamWriter = new StreamWriter(req.GetRequestStream()))
			{
				streamWriter.Write(jsonObj);
				streamWriter.Flush();
				streamWriter.Close();
			}

			using (HttpWebResponse res = (HttpWebResponse)(await req.GetResponseAsync()))
			using (Stream stream = res.GetResponseStream())
			using (StreamReader reader = new StreamReader(stream))
				await reader.ReadToEndAsync();
		}

		internal static string GetEndpoint(string serviceName)
		{
			return $"https://{serviceName}.scm.azurewebsites.net/";
		}

		internal static string GetAuthHeader(string username, string password)
		{
			return $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"), Base64FormattingOptions.None)}";
		}

		readonly string _authHeader;
		readonly string _endpoint;
	}
}
