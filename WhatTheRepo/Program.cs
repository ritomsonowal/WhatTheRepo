using System;

namespace WhatTheRepo
{
    class Program
    {
        static void Main(string[] args)
        {
            Config config = new Config();
            Console.WriteLine("Hello " + config.CONSUMER_KEY);
        }
    }
}
