using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Mts.Common.Tmx.Parser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TranslationAssistant.TranslationServices.Core
{
    /// <summary>
    /// A Dictionary of clouds and regions
    /// </summary>
    public static class Endpoints
    {
        public static Dictionary<string, cloud_endpoint> CloudEndpoints { get; } = new Dictionary<string, cloud_endpoint>();
        public static List<string> AvailableRegions { get; } = new List<string>();


        /// <summary>
        /// Initializes the class
        /// </summary>
        static Endpoints()
        {
            cloud_endpoint c_e = new cloud_endpoint();
            //Enter each cloud and the Translator endpoint here. There is typically one endpoint per cloud.
            c_e.cloudprefix = "global"; c_e.endpoint = "api.cognitive.microsofttranslator.com"; CloudEndpoints.Add("Global", c_e);
            c_e.cloudprefix = "eur"; c_e.endpoint = "api-eur.cognitive.microsofttranslator.com"; CloudEndpoints.Add("Europe", c_e);
            c_e.cloudprefix = "apc"; c_e.endpoint = "api-apc.cognitive.microsofttranslator.com"; CloudEndpoints.Add("Asia Pacific", c_e);
            c_e.cloudprefix = "nam"; c_e.endpoint = "api-nam.cognitive.microsofttranslator.com"; CloudEndpoints.Add("Americas", c_e);
            c_e.cloudprefix = "us"; c_e.endpoint = "api.cognitive.microsofttranslator.us"; CloudEndpoints.Add("US Government", c_e);
            c_e.cloudprefix = "china"; c_e.endpoint = "api.translator.azure.cn"; CloudEndpoints.Add("China", c_e);

            PopulateRegions();
            PopulateRegionList();
        }


        /// <summary>
        /// Holds the geographic region prefix and endpoint
        /// </summary>
        public class cloud_endpoint
        {
            public string cloudprefix { get; set; }
            public string endpoint { get; set; }
        }

        public class prefix_region
        {
            public string cloudprefix { get; set; }
            public string region { get; set; }
        }


        /// <summary>
        /// Fill the regions list with values
        /// </summary>
        private static void PopulateRegions()
        {
            AvailableRegions.Add("Global");
            foreach (var value in Microsoft.Azure.Management.ResourceManager.Fluent.Core.Region.Values)
            {
                AvailableRegions.Add(value.Name);
            }
            AvailableRegions.Sort();
            return;
        }

        /// <summary>
        /// Returns an array of available clouds
        /// </summary>
        /// <returns>Array of available clouds</returns>
        public static string[] GetClouds()
        {
            List<string> clouds = new List<string>();
            foreach (var cloud in CloudEndpoints)
            {
                clouds.Add(cloud.Key);
            }
            clouds.Sort();
            return clouds.ToArray();
        }




        /// <summary>
        /// Holds a table of regions with their cloudprefix
        /// </summary>
        private static List<prefix_region> regionlist = new List<prefix_region>();

        private static void PopulateRegionList()
        {
            regionlist.Clear();
            prefix_region row = new prefix_region();
            foreach (var region in AvailableRegions)
            {
                row.region = region;
                if (region.Contains("china"))
                {
                    row.cloudprefix = "china";
                    regionlist.Add(row);
                    continue;
                }
                if (region.Contains("usgov") || region.Contains("usdod"))
                {
                    row.cloudprefix = "usgov";
                    regionlist.Add(row);
                    continue;
                }
                row.cloudprefix = "global";
                regionlist.Add(row);
                continue;
            }
            return;
        }


        /// <summary>
        /// Returns an array of regions for one cloud. 
        /// </summary>
        /// <param name="cloudname">The cloud you want to obtain the regions for. Acts as a filter over the entire regions list. Obtain the available clouds from CloudEndpoints</param>
        /// <returns>Array of regions in the given cloud.</returns>
        public static string[] GetRegions(string cloudname="Global")
        {
            /* //Can't get the LinQ to work properly...
            if (string.IsNullOrEmpty(cloudname)) cloudname = "Global";
            cloud_endpoint cloudendpoint = new cloud_endpoint();
            bool result = CloudEndpoints.TryGetValue(cloudname, out cloudendpoint);
            var regions = from r in regionlist where r.cloudprefix.Equals(cloudendpoint.cloudprefix) select r;
            List<string> returnregions = new List<string>();
            foreach (var region in regions) returnregions.Add(region.region);
            */
            return AvailableRegions.ToArray();
        }

    }
}
