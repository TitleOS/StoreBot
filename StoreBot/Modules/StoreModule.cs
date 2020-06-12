using Discord.Commands;
using StoreBot.Services;
using StoreLib.Models;
using StoreLib.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StoreBot.Modules
{
    public class StoreModule : ModuleBase<SocketCommandContext>
    {
        public StoreService StoreService { get; set; }


        [Command("info")]
        public Task Info()
            => ReplyAsync(
                $"Hello, I am a bot called {Context.Client.CurrentUser.Username} written in Discord.Net 1.0\n");

        [Command("help")]
        public Task Help() => ReplyAsync($"[Commands]\nquery: @{Context.Client.CurrentUser.Username} query ProductID\nquery: @{Context.Client.CurrentUser.Username} query ProductID Endpoint\nEndpoints: Production, Int, Xbox, XboxInt, Dev, OneP, OnePInt");


        [Command("query")]
        public async Task Query(string ProductID)
        {
            string result = await StoreService.QueryProduct(ProductID);
            if (String.IsNullOrEmpty(result))
            {
                await Context.Channel.SendMessageAsync("No listing found.");
            }
            string[] messages = result.Split('#', StringSplitOptions.RemoveEmptyEntries);
            foreach(string m in messages)
            {
                List<string> packagelist = new List<string>(Regex.Split(m.ToString(), @"(?<=\G.{1999})", RegexOptions.Singleline));
                foreach(string s in packagelist)
                {
                    await Context.Channel.SendMessageAsync(s);

                }
            }
        }

        [Command("query")]
        public async Task Query(string ProductID, DCatEndpoint endpoint)
        {
            string result = await StoreService.QueryProduct(ProductID, endpoint);
            if (String.IsNullOrEmpty(result))
            {
                await Context.Channel.SendMessageAsync("No listing found.");
            }
            string[] messages = result.Split('#', StringSplitOptions.RemoveEmptyEntries);
            foreach (string m in messages)
            {
                List<string> packagelist = new List<string>(Regex.Split(m.ToString(), @"(?<=\G.{1999})", RegexOptions.Singleline));
                foreach (string s in packagelist)
                {
                    await Context.Channel.SendMessageAsync(s);

                }
            }
        }



    }
}
