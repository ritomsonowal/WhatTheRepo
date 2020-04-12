using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace WhatTheRepo
{
    public class Data
    {
        public long RepoID { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string TweetBody { get; set; }
    }
    class JSON
    {
        private string tweetsJSON = @"C:\Users\ritom\Desktop\twitterbot\WhatTheRepo\WhatTheRepo\tweets.json";
        private string settingsJSON = @"C:\Users\ritom\Desktop\twitterbot\WhatTheRepo\WhatTheRepo\settings.json";

        public string GetKeyword()
        {
            Random random = new Random();
            int choice;
            string key = "";

            try
            {
                var initialJson = File.ReadAllText(settingsJSON);
                var jsonObj = JObject.Parse(initialJson);
                var keywords = jsonObj.GetValue("keywords") as JArray;

                int max = keywords.Count;
                choice = random.Next(0, max);

                key = keywords[choice].ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Add Error : " + ex.Message.ToString());
            } return key;
        }
             
        public void AddTweet(long repo_id, string date, string time, string tweet_body)
        {
            try
            {
                var initialJson = File.ReadAllText(tweetsJSON);
                var list = JsonConvert.DeserializeObject<List<Data>>(initialJson);
                list.Add(new Data(){
                    RepoID = repo_id,
                    Date = date,
                    Time = time,
                    TweetBody = tweet_body
                });
                var convertedJson = JsonConvert.SerializeObject(list, Formatting.Indented);

                System.IO.File.WriteAllText(tweetsJSON, convertedJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An Error occurred in JSON.cs: " + ex.Message.ToString());
            }
        }

        public bool SearchTweets(long repo_id)
        {
            var tweetsJson = File.ReadAllText(tweetsJSON);
            var list = JsonConvert.DeserializeObject<List<Data>>(tweetsJson);

            int i;
            int n = list.Count;

            for ( i=0; i<n; i++)
            {
                if (list[i].RepoID == repo_id)
                {
                    return true;
                }
            } return false;
        }

    }
}
