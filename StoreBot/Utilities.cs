using Discord;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace StoreBot
{
    public static class Utilities
    {
        public static async Task Log(LogMessage message)
        {
            switch (message.Severity)
            {
                case LogSeverity.Error:
                    Console.WriteLine($"Exception: {message.Message}:{message.Exception.StackTrace}. Caller: {message.Source}");
                     await File.AppendAllTextAsync("log.txt", $"[{DateTime.Now.ToString()}]Exception: {message.Message}:{message.Exception.StackTrace}. Caller: {message.Source}");
                    return;
                case LogSeverity.Warning:
                    Console.WriteLine($"Warning: {message.Message}. Caller: {message.Source}");
                    await File.AppendAllTextAsync("log.txt", $"[{DateTime.Now.ToString()}]Warning: {message.Message}. Caller: {message.Source}");
                    return;
                case LogSeverity.Critical:
                    Console.WriteLine($"CRITICAL FAILURE: {message.Message}:{message.Exception.StackTrace}. StoreBot will now exit.");
                    await File.AppendAllTextAsync("log.txt", $"[{DateTime.Now.ToString()}]CRITICAL FAILURE: {message.Message}:{message.Exception.StackTrace}. StoreBot will now exit.");
                    throw message.Exception;
#if DEBUG
                case LogSeverity.Verbose:
                    Console.WriteLine($"Verbose Output: {message.Message}. Caller: {message.Source}");
                    await File.AppendAllTextAsync("log.txt", $"Verbose Output: {message.Message}. Caller: {message.Source}");
                    return;
                case LogSeverity.Debug:
                    Console.WriteLine($"Debug Output: {message.Message}. Caller: {message.Source}");
                    await File.AppendAllTextAsync("log.txt", $"Debug Output: {message.Message}. Caller: {message.Source}");
                    return;
#endif
                default:
                    return;
            }
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
            if(AuthTokenRaw[1] == "REPLACEME")
            {
                Debug.WriteLine("Using Environment Variable Token");
                this.AuthToken = System.Environment.GetEnvironmentVariable("STOREBOTTOKEN", EnvironmentVariableTarget.User);
                Debug.WriteLine($"Got Environment Variable Token: {this.AuthToken}");

            }
            else
            {
                this.AuthToken = AuthTokenRaw[1];

            }
        }


    }
}
