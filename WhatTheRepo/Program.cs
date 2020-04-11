using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Tweetinvi;
using Tweetinvi.Exceptions;
using Octokit;
using System.Threading.Tasks;

namespace WhatTheRepo
{
    static class Global
    {
        public static int counter;
        public static string tweetBody;
    }

    class Program
    {
        static void Main(string[] args)
        {
                   
            Console.WriteLine("================STARTING WHAT-THE-REPO================");

            Global.counter = 0;

            getTweet();

            TwitterConfig twitterConfig = new TwitterConfig();

            // Allow Tweetinvi to throw exceptions
            ExceptionHandler.SwallowWebExceptions = false;

            // Twitter credentials
             Auth.SetUserCredentials( twitterConfig.CONSUMER_KEY, twitterConfig.CONSUMER_SECRET, twitterConfig.ACCESS_TOKEN, twitterConfig.ACCESS_TOKEN_SECRET);

            try
            {
                DateTime now = DateTime.Now;

                // Publish the Tweet
                var tweet = Tweet.PublishTweet(Global.tweetBody);

                Console.WriteLine("{0} ...... @whattherepo tweeted : {1}", now.ToString("F"), tweet);

            }
            catch (TwitterException ex)
            {
                // Twitter API Request has been failed; Bad request, network failure or unauthorized request
                Console.WriteLine("Something went wrong when we tried to execute the http request : '{0}'", ex.TwitterDescription);
            }
        }

        static void getTweet()
        {
            JSON tweets = new JSON();

            var client = new GitHubClient(new ProductHeaderValue("what-the-repo"));

            GithubConfig githubConfig = new GithubConfig();

            var tokenAuth = new Credentials(githubConfig.ACCESS_TOKEN); // Can also authenticate using username and password
            client.Credentials = tokenAuth;

            string key = "shit";

            // Search for repos
            var request = new SearchRepositoriesRequest(key)
            {
                // sort by the number of stars
                SortField = RepoSearchSort.Stars

            };

            Task.Run(async () =>
            {
                // Do any async anything you need here without worry
                var result = await client.Search.SearchRepo(request);

                Console.WriteLine("Fetching repos...");

                long repo_id;
                string date;
                string time;
                string body;

                int i = 0;

                while (true)
                {
                    var obj = result.Items[i];

                    DateTime today = DateTime.Today;
                    DateTime now = DateTime.Now;
                    
                    repo_id = obj.Id;
                    date = today.ToString("dd/MM/yyyy");
                    time = now.ToString("h:mm:ss tt");

                    body = "Oh look! I've found a repo : " + obj.Name + " on github that says - " + obj.Description;

                    if (tweets.SearchTweets(repo_id) == false)
                    {
                        try
                        {
                            tweets.AddTweet(repo_id, date, time, body);
                            Global.tweetBody = body;
                            Console.WriteLine("Added tweet to json");
                            Global.counter += 1;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("An Error occurred : " + ex.Message.ToString());
                        }
                        break;

                    }
                    else
                    {
                        i += 1;                 // keep searching for repos that've not been tweeted so far
                    }
                }

            }).GetAwaiter().GetResult();

            //tweets.AddTweet(32423, "11/04/20", "11:00:00 a.m", "wtf");

            

            Console.WriteLine("------------");

            return;

        }
    }
}
