using System;
using Tweetinvi;
using Tweetinvi.Exceptions;

namespace WhatTheRepo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("================STARTING WHAT-THE-REPO================");

            Config config = new Config();

            // Disable the exception swallowing to allow exception to be thrown by Tweetinvi
            ExceptionHandler.SwallowWebExceptions = false;

            // Set up your credentials (https://apps.twitter.com)
            Auth.SetUserCredentials(config.CONSUMER_KEY, config.CONSUMER_SECRET, config.ACCESS_TOKEN, config.ACCESS_TOKEN_SECRET);

            try
            {
                DateTime now = DateTime.Now;

                // Publish the Tweet
                var tweet = Tweet.PublishTweet("Hello World! -WhatTheRepo 2020");

                Console.WriteLine( "{0} ...... @whattherepo tweeted : {1}", now.ToString("F"), tweet);

            }
            catch (TwitterException ex)
            {
                // Twitter API Request has been failed; Bad request, network failure or unauthorized request
                Console.WriteLine("Something went wrong when we tried to execute the http request : '{0}'", ex.TwitterDescription);
            }
        }
    }
}
