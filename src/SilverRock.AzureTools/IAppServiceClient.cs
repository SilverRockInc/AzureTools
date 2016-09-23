using System.Collections.Generic;
using System.Threading.Tasks;

namespace SilverRock.AzureTools
{
	public interface IAppServiceClient
	{
		string GetSetting(string settingKey);
		Task<string> GetSettingAsync(string settingKey);
		Dictionary<string, string> GetSettings();
		Task<Dictionary<string, string>> GetSettingsAsync();
		void SetSettings(Dictionary<string, string> settings);
		Task SetSettingsAsync(Dictionary<string, string> settings);
	}
}
