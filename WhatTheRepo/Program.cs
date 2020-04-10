using System;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Exceptions;

namespace WhatTheRepo
{
    class Program
    {
        static void Main(string[] args)
        {
            Config config = new Config();

            DateTime now = DateTime.Now;

            // Set up your credentials 
            Auth.SetUserCredentials(config.CONSUMER_KEY, config.CONSUMER_SECRET, config.ACCESS_TOKEN, config.ACCESS_TOKEN_SECRET);

            ExceptionHandler.SwallowWebExceptions = false;

            // Publish the Tweet "Hello World" on your Timeline
            try
            {
                var status = Tweet.PublishTweet("Hello World! I'm a bot tweeting to twitter adsnnj.");
                Console.WriteLine(" ");
                Console.WriteLine("{0} ......... @whattherepo tweeted : {1} ", now.ToString("F"), status);
                Console.WriteLine(" ");
            }
            catch (TwitterException ex)
            {
                // Twitter API Request has been failed; Bad request, network failure or unauthorized request
                Console.WriteLine("Something went wrong when we tried to execute the http request : '{0}'", ex.TwitterDescription);
            }
        }
    }
}
