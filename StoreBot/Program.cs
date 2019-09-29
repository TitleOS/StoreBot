using System;
using StoreLib;
using StoreLib.Models;
using Discord;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord.Commands;
using StoreLib.Services;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace StoreBot
{
    class Program
    {
        public static DisplayCatalogHandler dcat;
        public static Settings settingsinstance;
        static async Task Main(string[] args)
        {
            Console.WriteLine($"StoreBot - {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}");
            Console.WriteLine("Connecting to Discord services....");
            DiscordSocketClient client = new DiscordSocketClient();
            settingsinstance = new Settings();
            await client.LoginAsync(TokenType.Bot, settingsinstance.AuthToken);
            await client.StartAsync();
            client.MessageReceived += MessageReceived;
            Console.WriteLine($"Connected to Discord services");
            await Utilities.Log($"Logged into Discord at {System.DateTime.Now}");
            await client.SetGameAsync("DisplayCatalog", null, ActivityType.Watching);
            dcat = new DisplayCatalogHandler(StoreLib.DataContracts.DCatEndpoint.Production, new StoreLib.DataContracts.Locale(StoreLib.DataContracts.Market.US, StoreLib.DataContracts.Lang.en, true));
            await Task.Delay(-1);
            
        }


        static async Task MessageReceived(SocketMessage message)
        {
            if (message.Content.StartsWith("$"))
            {
#if DEBUG
                await message.Channel.SendMessageAsync($"StoreBot is running in DEBUG mode. Output will be verbose.");
#endif
                switch (message.Content)
                {
                    case "$DEV":
                        dcat = new DisplayCatalogHandler(StoreLib.DataContracts.DCatEndpoint.Dev, new StoreLib.DataContracts.Locale(StoreLib.DataContracts.Market.US, StoreLib.DataContracts.Lang.en, true));
                        await message.Channel.SendMessageAsync($"DCAT Endpoint was changed to {message.Content}");
                        break;
                    case "$INT":
                        dcat = new DisplayCatalogHandler(StoreLib.DataContracts.DCatEndpoint.Int, new StoreLib.DataContracts.Locale(StoreLib.DataContracts.Market.US, StoreLib.DataContracts.Lang.en, true));
                        await message.Channel.SendMessageAsync($"DCAT Endpoint was changed to {message.Content}");
                        break;
                    case "$PROD":
                        dcat = new DisplayCatalogHandler(StoreLib.DataContracts.DCatEndpoint.Production, new StoreLib.DataContracts.Locale(StoreLib.DataContracts.Market.US, StoreLib.DataContracts.Lang.en, true));
                        await message.Channel.SendMessageAsync($"DCAT Endpoint was changed to {message.Content}");
                        break;
                    case "$XBOX":
                        dcat = new DisplayCatalogHandler(StoreLib.DataContracts.DCatEndpoint.Xbox, new StoreLib.DataContracts.Locale(StoreLib.DataContracts.Market.US, StoreLib.DataContracts.Lang.en, true));
                        await message.Channel.SendMessageAsync($"DCAT Endpoint was changed to {message.Content}");
                        break;

                }
                if (message.Content.Length != 13)
                {
                    return;
                }
                string ProductID = message.Content.Substring(1, 12);
                await dcat.QueryDCATAsync(ProductID);
                if (dcat.IsFound)
                {
                    await message.Channel.SendMessageAsync("Working....");
                    await BuildReply(dcat.ProductListing, message);
                }
                else
                {
                    await message.Channel.SendMessageAsync("No Product was Found.");
                }
            }
            else if (message.Content.StartsWith("*X")) //Search with Xbox Device Family
            {
                DisplayCatalogHandler dcathandler = DisplayCatalogHandler.ProductionConfig();
                DCatSearch SearchResults = await dcathandler.SearchDCATAsync(message.Content.Substring(2), StoreLib.DataContracts.DeviceFamily.Universal);
                int count = 0;
                foreach(Result R in SearchResults.Results)
                {
                    while (settingsinstance.NumberOfSearchResults >= count)
                    {
                        await message.Channel.SendMessageAsync(R.Products[0].Title);
                        count++;
                    }
                }
            }
            else if (message.Content.StartsWith("*D")) //Search with Desktop Device Family
            {
                DisplayCatalogHandler dcathandler = DisplayCatalogHandler.ProductionConfig();
                DCatSearch SearchResults = await dcathandler.SearchDCATAsync(message.Content.Substring(2), StoreLib.DataContracts.DeviceFamily.Desktop);
                int count = 0;
                foreach (Result R in SearchResults.Results)
                {
                    while (settingsinstance.NumberOfSearchResults >= count)
                    {
                        await message.Channel.SendMessageAsync(R.Products[0].Title);
                        count++;
                    }
                    
                }
            }

        }

        private static async Task BuildReply(DisplayCatalogModel displayCatalogModel, SocketMessage message)
        {

            StringBuilder MoreDetailsHelper = new StringBuilder();
            await Utilities.Log($"{message.Content.Substring(1, 12)} - {displayCatalogModel.Product.LocalizedProperties[0].ProductTitle} was queried by {message.Author.Username}#{message.Author.Id} at {message.Channel.Name}");
            MoreDetailsHelper.AppendLine($"`{displayCatalogModel.Product.LocalizedProperties[0].ProductTitle} - {displayCatalogModel.Product.LocalizedProperties[0].PublisherName}");
            MoreDetailsHelper.AppendLine($"Rating: {displayCatalogModel.Product.MarketProperties[0].UsageData[0].AverageRating} Stars");
            MoreDetailsHelper.AppendLine($"Last Modified: {displayCatalogModel.Product.LastModifiedDate}");
            MoreDetailsHelper.AppendLine($"Original Release: {displayCatalogModel.Product.MarketProperties[0].OriginalReleaseDate}");
            MoreDetailsHelper.AppendLine($"Product Type: {displayCatalogModel.Product.ProductType}");
            MoreDetailsHelper.AppendLine($"Is a Microsoft Listing: {displayCatalogModel.Product.IsMicrosoftProduct}");
            if (displayCatalogModel.Product.ValidationData != null)
            {
                MoreDetailsHelper.AppendLine($"Validation Info: {displayCatalogModel.Product.ValidationData.RevisionId}");
            }
            if (displayCatalogModel.Product.SandboxID != null)
            {
                MoreDetailsHelper.AppendLine($"SandBoxID: {displayCatalogModel.Product.SandboxID}");
            }

            foreach (AlternateId ID in displayCatalogModel.Product.AlternateIds) //Dynamicly add any other ID(s) that might be present rather than doing a ton of null checks.
            {
                MoreDetailsHelper.AppendLine($"{ID.IdType}: {ID.Value}");
            }
            if (displayCatalogModel.Product.DisplaySkuAvailabilities[0].Sku.Properties.FulfillmentData != null)
            {
                MoreDetailsHelper.AppendLine($"WuCategoryID: {displayCatalogModel.Product.DisplaySkuAvailabilities[0].Sku.Properties.FulfillmentData.WuCategoryId}");
                MoreDetailsHelper.AppendLine($"EAppx Key ID: {displayCatalogModel.Product.DisplaySkuAvailabilities[0].Sku.Properties.Packages[0].KeyId}");
            }
            MoreDetailsHelper.AppendLine("`");
            IList<Uri> FileUris = await dcat.GetPackagesForProduct();
#if DEBUG
            await message.Channel.SendMessageAsync($"Found {FileUris.Count} FE3 Links.");
#endif
            MoreDetailsHelper.AppendLine($"Packages: (1/3)\n");
            await message.Channel.SendMessageAsync(MoreDetailsHelper.ToString());
            StringBuilder packages = new StringBuilder();
            List<string> packagelist = new List<string>(Regex.Split(packages.ToString(), @"(?<=\G.{1999})", RegexOptions.Singleline));
            if (displayCatalogModel.Product.DisplaySkuAvailabilities[0].Sku.Properties.Packages.Count > 0)
            {
                try
                {
                    foreach (var Package in displayCatalogModel.Product.DisplaySkuAvailabilities[0].Sku.Properties.Packages[0].PackageDownloadUris)
                    {
                        packages.AppendLine($"Xbox Live Package: {Package.Uri}");
                    }
                }
                catch { }
            }
            /*
            
            */
            packages.AppendLine($"(2/3)");
            await message.Channel.SendMessageAsync(packages.ToString());
            if (FileUris.Count > 0)
            {
                foreach (Uri uri in FileUris)
                {
                    packagelist.Add(uri.ToString());
                }
            }
            foreach (string downloaduri in packagelist)
            {
                try
                {
                    await message.Channel.SendMessageAsync(downloaduri);
                }
                catch { }
            }
            
            await message.Channel.SendMessageAsync("(3/3)");
        }


    }
}
