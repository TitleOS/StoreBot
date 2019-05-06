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
}
