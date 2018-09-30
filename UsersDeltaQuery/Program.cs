using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace UsersDeltaQuery
{
    class Program
    {
        public static class Config
        {
            // Create a static dictionary to store config parameters
            private static Dictionary<string, string> _config = new Dictionary<string, string>() {
            //Adding Items to the Collection
                {"clientId", "410c97f1-b895-49f4-a48a-3b0b7d911e67" },
                {"clientSecret", "+@![{p:e=l#0#A[@o{B@]>|(:$*-:(R:;%j@:=/:q{d]#^;.&DA&8(z}^#q^|#:" },
                {"tenantId", "adatumisv.onmicrosoft.com" },
                {"authorityFormat", "https://login.microsoftonline.com/{0}/v2.0" },
                {"replyUri", "https://localhost" }
            };

            /// <summary>
            /// Access the Dictionary from external sources
            /// </summary>
            public static String GetConfig(String key)
            {
                // Try to get the result in the static Dictionary
                String result;
                if (_config.TryGetValue(key, out result))
                {
                    return result;
                }
                else
                {
                    return null;
                }
            }


        }
        static void Main(String[] args)
        {
            RunAsync(args).GetAwaiter().GetResult();
            Console.WriteLine("Press any key to finish.");
            Console.ReadKey();
        }

        static async Task RunAsync(String[] args)
        {
            var tenantId = Config.GetConfig("tenantId");

            ConfidentialClientApplication daemonClient = new ConfidentialClientApplication(
                Config.GetConfig("clientId"),
                String.Format(Config.GetConfig("authorityFormat"), tenantId),
                Config.GetConfig("replyUri"),
                new ClientCredential(Config.GetConfig("clientSecret")),
                null,
                new TokenCache());


            GraphServiceClient graphClient = new GraphServiceClient(
                "https://graph.microsoft.com/v1.0",
                new DelegateAuthenticationProvider(
                    async (requestMessage) =>
                    {
                        var authenticationResult = await daemonClient.AcquireTokenForClientAsync(new string[] { "https://graph.microsoft.com/.default" });
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", authenticationResult.AccessToken);
                    }));

            Console.WriteLine("=== Getting users");

            //Get the list of changed users
            var userPage = await graphClient.Users
                .Delta()
                .Request()
                .Select("displayName,userPrincipalName")
                .GetAsync();

            //Display users and get the delta link
            var deltaLink = await DisplayChangedUsersAndGetDeltaLink(userPage);


            Console.WriteLine("=== Adding user");

            //Create a new user
            var u = new User()
            {
                DisplayName = "UsersDeltaQuery Demo User",
                GivenName = "UsersDeltaQueryDemo",
                Surname = "User",
                MailNickname = "UsersDeltaQueryDemoUser",
                UserPrincipalName = Guid.NewGuid().ToString() + "@" + tenantId,
                PasswordProfile = new PasswordProfile() { ForceChangePasswordNextSignIn = true, Password = "D3m0p@55w0rd!" },
                AccountEnabled = true
            };
            var newUser = await graphClient.Users.Request().AddAsync(u);

            Console.WriteLine("Press any key to execute delta query.");
            Console.ReadKey();
            Console.WriteLine("=== Getting delta users");

            //Query using the delta link to see the new user
            userPage.InitializeNextPageRequest(graphClient, deltaLink);
            userPage = await userPage.NextPageRequest.GetAsync();

            //Display again... notice that only the added user is returned
            var newDeltaLink = await DisplayChangedUsersAndGetDeltaLink(userPage);
            while (deltaLink.Equals(newDeltaLink))
            {
                //If the two are equal, then we didn't receive changes yet
                //Query using the delta link to see the new user
                userPage.InitializeNextPageRequest(graphClient, deltaLink);
                userPage = await userPage.NextPageRequest.GetAsync();
                newDeltaLink = await DisplayChangedUsersAndGetDeltaLink(userPage);
            }

            Console.WriteLine("=== Deleting user");
            //Finally, delete the user
            await graphClient.Users[newUser.Id].Request().DeleteAsync();

        }

        static async Task<string> DisplayChangedUsersAndGetDeltaLink(IUserDeltaCollectionPage userPage)
        {

            //Iterate through the users
            foreach (var user in userPage)
            {
                if (user.UserPrincipalName != null)
                    Console.WriteLine(user.UserPrincipalName.ToLower().Replace("m365x287476", "msgraphdemo") + "\t\t" + user.DisplayName);
            }
            while (userPage.NextPageRequest != null)
            {
                //Console.WriteLine("=== NEXT LINK: " + userPage.NextPageRequest.RequestUrl);
                //Console.WriteLine("=== SKIP TOKEN: " + userPage.NextPageRequest.QueryOptions[0].Value);

                userPage = await userPage.NextPageRequest.GetAsync();
                foreach (var user in userPage)
                {
                    if (user.UserPrincipalName != null)
                        Console.WriteLine(user.UserPrincipalName.ToLower().Replace("m365x287476", "msgraphdemo") + "\t\t" + user.DisplayName);
                }
            }

            //Finally, get the delta link
            string deltaLink = (string)userPage.AdditionalData["@odata.deltaLink"];
            //Console.WriteLine("=== DELTA LINK: " + deltaLink);

            return deltaLink;
        }
    }
}
