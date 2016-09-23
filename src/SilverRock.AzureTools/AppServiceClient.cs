using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SilverRock.AzureTools
{
	public sealed class AppServiceClient : IAppServiceClient
	{
		public AppServiceClient(AppServiceAccount account)
		{
			_account = account;
		}

		public string GetSetting(string settingKey)
		{
			string jsonObj = _account.GetResource($"{SETTINGS}/{settingKey}");
			JToken token = JToken.Parse(jsonObj);
			return token.Type == JTokenType.String ? token.Value<string>() : null;
		}

		public async Task<string> GetSettingAsync(string settingKey)
		{
			string jsonObj = await _account.GetResourceAsync($"{SETTINGS}/{settingKey}");
			JToken token = JToken.Parse(jsonObj);
			return token.Type == JTokenType.String ? token.Value<string>() : null;
		}

		public Dictionary<string, string> GetSettings()
		{
			string jsonObj = _account.GetResource(SETTINGS);
			return ToDictionary(jsonObj);
		}

		public async Task<Dictionary<string, string>> GetSettingsAsync()
		{
			string jsonObj = await _account.GetResourceAsync(SETTINGS);
			return ToDictionary(jsonObj);
		}

		public void SetSettings(Dictionary<string, string> settings)
		{
			string jsonObj = ToJson(settings);
			_account.PostResource(SETTINGS, jsonObj);
		}

		public async Task SetSettingsAsync(Dictionary<string, string> settings)
		{
			string jsonObj = ToJson(settings);
			await _account.PostResourceAsync(SETTINGS, jsonObj);
		}

		internal static string GetEndpoint(string serviceName)
		{
			return $"https://{serviceName}.scm.azurewebsites.net/";
		}

		internal static string GetAuthHeader(string username, string password)
		{
			return $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"), Base64FormattingOptions.None)}";
		}

		internal static string ToJson(Dictionary<string, string> data)
		{
			JObject obj = new JObject();

			foreach (string key in data.Keys)
				obj.Add(key, JToken.FromObject(data[key]));

			return obj.ToString();
		}

		internal static Dictionary<string, string> ToDictionary(string jsonObj)
		{
			Dictionary<string, string> data = new Dictionary<string, string>();

			JObject obj = JObject.Parse(jsonObj);

			foreach (JToken token in obj.Children())
			{
				JProperty prop = token as JProperty;

				if (prop != null && prop.Value.Type == JTokenType.String)
				{
					data[prop.Name] = prop.Value.Value<string>();
				}
			}

			return data;
		}

		internal const string SETTINGS = "settings/";

		readonly AppServiceAccount _account;
	}
}
