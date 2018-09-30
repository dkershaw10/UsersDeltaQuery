# UsersDeltaQuery
.NET Core app

## Minimal instructions

1. Clone the repo
2. Open VS Code - make sure you have the the c# extension
3. Open folder (with the .sln) in VS Code
4. Go to app registration.  Register a new single tenant app, and create a new secret.  Also get the client id.
5. Update the Config class in the Program.cs file with your app's co-ordinates.
6. In VS Code, Terminal -> New Terminal, and then in the new terminal, type: 
  * `dotnet add package Microsoft.Graph`, and hit Restore button when prompted (if prompted)
  * `dotnet add package Microsoft.Identity.Client`, and hit Restore button when prompted (if prompted)
7. In VS Code, Debug -> Add Configuration, and then select an appropriate option (.NET: Launch .Net Core Console App)
  * Change "program" to the same execuatable name as in the existing .Net Core console app config
  * Change "console" to "externalTerminal" (not sure if this is platform independent - may need to look online)
  * Change "name" to "External Console"
8. Debug->Start Debugging (or F5/equivalent) and switch debug to use "External Console"


