# UsersDeltaQuery
.NET Core app

## Minimal instructions

1. Clone the repo
2. Open VS Code - make sure you have the the c# extension
3. Open folder (with the .sln) in VS Code
4. When it finishes opening, click yes to resolve any required C# elements and also click to restore packages.  See later if this doesn't happen and you need to still restore.  
4. Go to app registration.  Register a new single tenant app, and create a new secret.  Also get the client id. Confige the app with User.ReadWrite.All and then **grant** the app this permission.
5. Update the Config class in the Program.cs file with your app's co-ordinates.
7. In VS Code, Debug -> Open Configuration, and then click "Add Configuration" button. Then select an appropriate option (.NET: Launch .Net Core Console App)
  * Change "program" to the same execuatable name as in the existing .Net Core console app config
  * Change "console" to "externalTerminal" (not sure if this is platform independent - may need to look online)
  * Change "name" to "External Console"
8. Start Debugging (or F5/equivalent), but switch debug to use "External Console"

### If you need to manually restore packages

In VS Code, Terminal -> New Terminal, and then in the new terminal, type: 
  * `dotnet add package Microsoft.Graph`, and hit Restore button when prompted (if prompted)
  * `dotnet add package Microsoft.Identity.Client`, and hit Restore button when prompted (if prompted)



