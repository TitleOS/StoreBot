using Discord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace StoreBot
{
    public static class Utilities
    {
        public static async Task Log(LogMessage message)
        {
            Console.WriteLine($"Exception: {message.Message} at {message.Source}");
            await File.AppendAllTextAsync("log.txt", $"Exception: {message.Message} at {message.Source}");
        }

        public static async Task Log(string message)
        {
            Console.WriteLine(message);
            await File.AppendAllTextAsync("log.txt", message);

        }


    }

    public class Settings
    {
        public int NumberOfSearchResults;
        public string AuthToken;


        public Settings()
        {
            string[] rawsettings = File.ReadAllLines("settings.config");
            string[] NumberOfSearchResultsRaw = rawsettings[0].Split("=");
            string[] AuthTokenRaw = rawsettings[1].Split("=");
            this.NumberOfSearchResults = int.Parse(NumberOfSearchResultsRaw[1]);
            this.AuthToken = AuthTokenRaw[1];
        }


    }
}
