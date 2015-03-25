using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        const string userName = "someuser@someemail.com";
        const string password = "Password1!";
        const string apiBaseUri = "http://localhost:18342";
        const string apiGetPeoplePath = "/api/people";

        static void Main(string[] args)
        {
            //Get the token
            var token = GetAPIToken(userName, password, apiBaseUri).Result;
            Console.WriteLine("Token: {0}", token);

            //Make the call
            var response = GetRequest(token, apiBaseUri, apiGetPeoplePath).Result;
            Console.WriteLine("response: {0}", response);

            //wait for key press to exit
            Console.ReadKey();
        }

        private static async Task<string> GetAPIToken(string userName, string password, string apiBaseUri)
        {
            using (var client = new HttpClient())
            {
                //setup client
                client.BaseAddress = new Uri(apiBaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //setup login data
                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "password"), 
                    new KeyValuePair<string, string>("username", userName), 
                    new KeyValuePair<string, string>("password", password), 
                });

                //send request
                HttpResponseMessage responseMessage = await client.PostAsync("/Token", formContent);

                //get access token from response body
                var responseJson = await responseMessage.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);
                return jObject.GetValue("access_token").ToString();
            }
        }

        static async Task<string> GetRequest(string token, string apiBaseUri, string requestPath)
        {
            using (var client = new HttpClient())
            {
                //setup client
                client.BaseAddress = new Uri(apiBaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                //make request
                HttpResponseMessage response = await client.GetAsync(requestPath);
                var responseString = await response.Content.ReadAsStringAsync();
                return responseString;
            }
        }
    }
}
