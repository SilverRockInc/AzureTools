using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.ServiceBus.Messaging;
using SilverRock.AzureTools.Models;

namespace SilverRock.AzureTools.UnitTests
{
	[TestClass]
	public class ScriptRunnerTestFixture
	{
		[TestMethod]
		public void ParseScriptTest()
		{
			// Arrange
			string path = "my-topic";

			string script = $@"
{{
	topics: [
		{{
			path: '{path}',
			authorization: [
				{{ name: 'default', accessRights: ['Manage', 'Send', 'Listen'] }}
			],
			subscriptions: [
				{{ name: 'some-subscription' }},
				{{ name: 'another-subscription' }}
			]
		}}
	]
}}
";

			ScriptRunner sut = new ScriptRunner(null, null, null);

			// Act
			Script result = sut.ParseScript(script);

			// Assert
			Assert.IsNotNull(result);
		}
	}
}
