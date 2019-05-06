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

namespace StoreBot
{
    class Program
    {
        public static DisplayCatalogHandler dcat;
        static async Task Main(string[] args)
        {
            Console.WriteLine($"StoreBot - {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}");
            Console.WriteLine("Connecting to Discord services....");
            DiscordSocketClient client = new DiscordSocketClient();
            await client.LoginAsync(TokenType.Bot, "NDU2MjkyMzQzMjg5NzQxMzg0.DgIbEA.ju69RwrT7auOtNxu8g73Cg9-_o4");
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
            MoreDetailsHelper.AppendLine($"Packages: (1/2)\n");
            await message.Channel.SendMessageAsync(MoreDetailsHelper.ToString());
            StringBuilder packages = new StringBuilder();
            packages.AppendLine("(2/2)");
            foreach (Uri fileuri in FileUris)
            {
                packages.AppendLine(fileuri.ToString());
            }
            List<string> packagelist = new List<string>(Regex.Split(packages.ToString(), @"(?<=\G.{1999})", RegexOptions.Singleline));
            foreach(string package in packagelist)
            {
                await message.Channel.SendMessageAsync(package);
            }
            //await message.Channel.SendMessageAsync(packages.ToString().Substring(0, 1999));

            /*
            string xml = await FE3Handler.SyncUpdatesAsync(displayCatalogModel.Product.DisplaySkuAvailabilities[0].Sku.Properties.FulfillmentData.WuCategoryId);
            IList<string> RevisionIds = new List<string>();
            IList<string> PackageNames = new List<string>();
            IList<string> UpdateIDs = new List<string>();
            FE3Handler.ProcessUpdateIDs(xml, out RevisionIds, out PackageNames, out UpdateIDs);
            */
        }


    }
}
