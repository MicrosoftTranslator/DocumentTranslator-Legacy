using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationAssistant.TranslationServices.Core
{
    /// <summary>
    /// A Dictionary of clouds and regions
    /// </summary>
    class Endpoints
    {
        public static Dictionary<string, cloud_endpoint> CloudEndpoints { get; } = new Dictionary<string, cloud_endpoint>();
        public static List<string> AvailableRegions { get; } = new List<string>();


        /// <summary>
        /// Initializes the class
        /// </summary>
        Endpoints()
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
        }


        /// <summary>
        /// Holds the geographic region prefix and endpoint
        /// </summary>
        public class cloud_endpoint
        {
            public string cloudprefix { get; set; }
            public string endpoint { get; set; }
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
            return;
        }

        /// <summary>
        /// Returns an array of available clouds
        /// </summary>
        /// <returns></returns>
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
        /// Returns an array of regions for one cloud. 
        /// </summary>
        /// <param name="cloudname">The cloud you want to obtain the regions for. Acts as a filter over the entire regions list. Obtain the available clouds fromk CloudEndpoints</param>
        /// <returns></returns>
        public static string[] GetRegions(string cloudname)
        {
            List<string> regions = new List<string>();
            List<string> globalregions = new List<string>();
            cloud_endpoint c_e = new cloud_endpoint();
            CloudEndpoints.TryGetValue(cloudname, out c_e);

            foreach (var region in AvailableRegions)
            {
                if (region.StartsWith(c_e.cloudprefix)) regions.Add(region);
                else globalregions.Add(region);
            }

            if (cloudname.ToUpperInvariant() == "GLOBAL") {
                foreach (cloud_endpoint cev in CloudEndpoints.Values)
                {
                    foreach (string region in globalregions)
                        if (region.StartsWith(cev.cloudprefix)) globalregions.Remove(region);
                }
                globalregions.Sort();
                return globalregions.ToArray();
            }
            else { 
                regions.Sort();
                return regions.ToArray();
            }
        }



    }
}
