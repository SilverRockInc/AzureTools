using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
	class Program
	{
		static void Main(string[] args)
		{
			SilverRock.AzureTools.ScriptRunner runner = new SilverRock.AzureTools.ScriptRunner(
				"sb://argo-services-servicebus-west-test.servicebus.windows.net/",
				"RootManageSharedAccessKey",
				"wWQWBb8dzc0ra1LNZ1cI1ghQs78wSUqwCORNxn96FNo=");

			runner.Message += (sender, e) => Console.Write(e.Message);

			string script = $@"
{{
	topics: [
		{{
			path: 'contracts-test',
			authorization: [
				{{ name: 'default', primaryKey: 'j5+b+xAVHsVqFLqqTllGXVMAtJ/AMdd/CL29NQ2VWds=', accessRights: ['Manage', 'Send', 'Listen'] }}
			],
			maxSizeInMegabytes: 4096,
			subscriptions: [
				{{ name: 'audit' }},
				{{ name: 'cancellations', sqlFilter: ""Type = 'Cancellation'"" }},
				{{ name: 'operations', sqlFilter: 'Type = ""Contract""' }}
			]
		}}
	]
}}
";

			runner.Create(script, force: true);

			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("press the any key to continue ...");

			Console.ReadKey();
		}
	}
}
