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

            string key = tweets.GetKeyword();

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

                    body = getTweetBody( obj.HtmlUrl, obj.Name, obj.Description, obj.StargazersCount, obj.Language);

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

        static string getTweetBody( string repo_url, string repo_name, string repo_desc, int repo_star_count, string repo_language)
        {
            int min, max;
            string body = "";

            Random random = new Random();
            min = repo_desc.Length < 50 ? 4 : 1;
            max = repo_desc.Length > 100 ? 5 : 7;
            int choice = random.Next( min, max);

            // handle specific cases
            if (repo_desc.Length == 0)
            {
                choice = 8;
            } if (repo_desc.Length > 50 && repo_language.Length != 0)
            {
                choice = 4;
            }
            
            // Don't act smart and change indentation
            switch (choice)
            {
                case 1:
                    body = $@"Someone on Github thought it would be a funny to name a repo : {repo_name}.

[repo]{repo_url}";
                    break;
                case 2:
                    body = $@"Who would've thought a {repo_language} repo named {repo_name} would have {repo_star_count.ToString()} stargazers.
                        
[repo] {repo_url}"; 
                    break;
                case 3:
                    body = $@"Github needs more repositories with such creative names like {repo_name}.
                        
[repo] {repo_url}";
                    break;
                case 4:
                    body = $@"A {repo_name} project called {repo_language} (yup!!!)
                        
[repo] {repo_url}";
                    break;
                case 5:
                    body = $@"'{repo_desc}' is apparently what this repo does. Sounds uncool, {repo_name}!
                        
[repo] {repo_url}";
                    break;
                case 6:
                    body = $@"The next big thing in software - {repo_name} : '{repo_desc}'.
                        
[repo] {repo_url}";
                    break;
                case 7:
                    body = $@"A developer used {repo_language} to make a {repo_name}.
Description: {repo_desc} (mostly...)
                        
[repo] {repo_url}";
                    break;
                case 8:
                    body = $@"{repo_name} - A repo so good the owner didn't bother to write a description for it.
                        
[repo] {repo_url}";
                    break;
            }

            return body;
        }
    }
}
