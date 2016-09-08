nuget install OpenCover -Version 4.6.519 -OutputDirectory tools
nuget install coveralls.net -Version 0.412.0 -OutputDirectory tools

.\tools\OpenCover.4.6.519\tools\OpenCover.Console.exe -target:"vstest.console.exe" -filter:"+[*]* -[*.*Tests]*" -excludebyfile:"*.g.cs" -targetargs:".\tests\SilverRock.AzureTools.UnitTests\bin\Release\SilverRock.AzureTools.UnitTests.dll" -register:user
.\tools\coveralls.net.0.412\tools\csmacnz.Coveralls.exe --opencover -i .\results.xml