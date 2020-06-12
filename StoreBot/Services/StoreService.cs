using StoreLib.Models;
using StoreLib.Services;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StoreBot.Services
{
    public class StoreService
    {
        public static async Task<string> QueryProduct(string ProductID)
        {
            DisplayCatalogHandler dcat = DisplayCatalogHandler.ProductionConfig();
            await dcat.QueryDCATAsync(ProductID);
            if (!dcat.IsFound)
            {
                return "";
            }
            StringBuilder MoreDetailsHelper = new StringBuilder();
            MoreDetailsHelper.AppendLine($"`{dcat.ProductListing.Product.LocalizedProperties[0].ProductTitle} - {dcat.ProductListing.Product.LocalizedProperties[0].PublisherName}");
            MoreDetailsHelper.AppendLine($"Rating: {dcat.ProductListing.Product.MarketProperties[0].UsageData[0].AverageRating} Stars");
            MoreDetailsHelper.AppendLine($"Last Modified: {dcat.ProductListing.Product.LastModifiedDate}");
            MoreDetailsHelper.AppendLine($"Original Release: {dcat.ProductListing.Product.MarketProperties[0].OriginalReleaseDate}");
            MoreDetailsHelper.AppendLine($"Product Type: {dcat.ProductListing.Product.ProductType}");
            MoreDetailsHelper.AppendLine($"Is a Microsoft Listing: {dcat.ProductListing.Product.IsMicrosoftProduct}");
            if (dcat.ProductListing.Product.ValidationData != null)
            {
                MoreDetailsHelper.AppendLine($"Validation Info: {dcat.ProductListing.Product.ValidationData.RevisionId}");
            }
            if (dcat.ProductListing.Product.SandboxID != null)
            {
                MoreDetailsHelper.AppendLine($"SandboxID: {dcat.ProductListing.Product.SandboxID}");
            }

            foreach (AlternateId ID in dcat.ProductListing.Product.AlternateIds) //Dynamicly add any other ID(s) that might be present rather than doing a ton of null checks.
            {
                MoreDetailsHelper.AppendLine($"{ID.IdType}: {ID.Value}");
            }
            if (dcat.ProductListing.Product.DisplaySkuAvailabilities[0].Sku.Properties.FulfillmentData != null)
            {
                MoreDetailsHelper.AppendLine($"WuCategoryID: {dcat.ProductListing.Product.DisplaySkuAvailabilities[0].Sku.Properties.FulfillmentData.WuCategoryId}");
                MoreDetailsHelper.AppendLine($"EAppx Key ID: {dcat.ProductListing.Product.DisplaySkuAvailabilities[0].Sku.Properties.Packages[0].KeyId}");
            }
            MoreDetailsHelper.AppendLine("`");
            IList<PackageInstance> packageInstances = await dcat.GetPackagesForProductAsync();
            StringBuilder packages = new StringBuilder();
            if (dcat.ProductListing.Product.DisplaySkuAvailabilities[0].Sku.Properties.Packages.Count > 0)
            {
                if (dcat.ProductListing.Product.DisplaySkuAvailabilities[0].Sku.Properties.Packages.Count != 0)
                {
                    //For some weird reason, some listings report having packages when really they don't have one hosted. This checks the child to see if the package is really null or not.
                    if (!object.ReferenceEquals(dcat.ProductListing.Product.DisplaySkuAvailabilities[0].Sku.Properties.Packages[0].PackageDownloadUris, null))
                    {
                        foreach (var Package in dcat.ProductListing.Product.DisplaySkuAvailabilities[0].Sku.Properties.Packages[0].PackageDownloadUris)
                        {
                            packages.AppendLine($"Xbox Live Package: {Package.Uri}");
                        }
                    }
                }


            }
            StringBuilder fe3packages = new StringBuilder();
            foreach (PackageInstance package in packageInstances)
            {
                try
                {
                    fe3packages.AppendLine($"{package.PackageMoniker} : {package.PackageType} : {package.PackageUri}");
                }
                catch { }
            }
            return MoreDetailsHelper.ToString() + "#" + packages.ToString() + "#" + fe3packages.ToString();
        }

        public static async Task<string> QueryProduct(string ProductID, DCatEndpoint DCatEndpoint)
        {
            DisplayCatalogHandler dcat = new DisplayCatalogHandler(DCatEndpoint, new Locale(Market.US, Lang.en, true));
            await dcat.QueryDCATAsync(ProductID);
            if (!dcat.IsFound)
            {
                return "";
            }
            StringBuilder MoreDetailsHelper = new StringBuilder();
            MoreDetailsHelper.AppendLine($"`{dcat.ProductListing.Product.LocalizedProperties[0].ProductTitle} - {dcat.ProductListing.Product.LocalizedProperties[0].PublisherName}");
            MoreDetailsHelper.AppendLine($"Rating: {dcat.ProductListing.Product.MarketProperties[0].UsageData[0].AverageRating} Stars");
            MoreDetailsHelper.AppendLine($"Last Modified: {dcat.ProductListing.Product.LastModifiedDate}");
            MoreDetailsHelper.AppendLine($"Original Release: {dcat.ProductListing.Product.MarketProperties[0].OriginalReleaseDate}");
            MoreDetailsHelper.AppendLine($"Product Type: {dcat.ProductListing.Product.ProductType}");
            MoreDetailsHelper.AppendLine($"Is a Microsoft Listing: {dcat.ProductListing.Product.IsMicrosoftProduct}");
            if (dcat.ProductListing.Product.ValidationData != null)
            {
                MoreDetailsHelper.AppendLine($"Validation Info: {dcat.ProductListing.Product.ValidationData.RevisionId}");
            }
            if (dcat.ProductListing.Product.SandboxID != null)
            {
                MoreDetailsHelper.AppendLine($"SandboxID: {dcat.ProductListing.Product.SandboxID}");
            }

            foreach (AlternateId ID in dcat.ProductListing.Product.AlternateIds) //Dynamicly add any other ID(s) that might be present rather than doing a ton of null checks.
            {
                MoreDetailsHelper.AppendLine($"{ID.IdType}: {ID.Value}");
            }
            if (dcat.ProductListing.Product.DisplaySkuAvailabilities[0].Sku.Properties.FulfillmentData != null)
            {
                MoreDetailsHelper.AppendLine($"WuCategoryID: {dcat.ProductListing.Product.DisplaySkuAvailabilities[0].Sku.Properties.FulfillmentData.WuCategoryId}");
                MoreDetailsHelper.AppendLine($"EAppx Key ID: {dcat.ProductListing.Product.DisplaySkuAvailabilities[0].Sku.Properties.Packages[0].KeyId}");
            }
            MoreDetailsHelper.AppendLine("`");
            IList<PackageInstance> packageInstances = await dcat.GetPackagesForProductAsync();
            StringBuilder packages = new StringBuilder();
            if (dcat.ProductListing.Product.DisplaySkuAvailabilities[0].Sku.Properties.Packages.Count > 0)
            {
                if (dcat.ProductListing.Product.DisplaySkuAvailabilities[0].Sku.Properties.Packages.Count != 0)
                {
                    //For some weird reason, some listings report having packages when really they don't have one hosted. This checks the child to see if the package is really null or not.
                    if (!object.ReferenceEquals(dcat.ProductListing.Product.DisplaySkuAvailabilities[0].Sku.Properties.Packages[0].PackageDownloadUris, null))
                    {
                        foreach (var Package in dcat.ProductListing.Product.DisplaySkuAvailabilities[0].Sku.Properties.Packages[0].PackageDownloadUris)
                        {
                            packages.AppendLine($"Xbox Live Package: {Package.Uri}");
                        }
                    }
                }


            }
            StringBuilder fe3packages = new StringBuilder();
            foreach (PackageInstance package in packageInstances)
            {
                try
                {
                    fe3packages.AppendLine($"{package.PackageMoniker} : {package.PackageType} : {package.PackageUri}");
                }
                catch { }
            }
            return MoreDetailsHelper.ToString() + "#" + packages.ToString() + "#" + fe3packages.ToString();
        }
    }
}
