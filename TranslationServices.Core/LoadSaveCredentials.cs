using System.Collections.Generic;

namespace TranslationAssistant.TranslationServices.Core
{
    public static partial class TranslationServiceFacade
    {

        /// <summary>
        /// Loads credentials from settings file.
        /// Doesn't need to be public, because it is called during Initialize();
        /// </summary>
        public static void LoadCredentials()
        {
            AzureKey = Properties.Settings.Default.AzureKey;
            CategoryID = Properties.Settings.Default.CategoryID;
            AppId = Properties.Settings.Default.AppId;
            ShowExperimental = Properties.Settings.Default.ShowExperimental;
            UseAdvancedSettings = Properties.Settings.Default.UseAdvancedSettings;
            AzureCloud = Properties.Settings.Default.AzureCloud;
            AzureRegion = Properties.Settings.Default.SubscriptionRegion;
            UseCustomEndpoint = Properties.Settings.Default.UseCustomEndpoint;
            CustomEndpointUrl = Properties.Settings.Default.CustomEndpointUrl;
            EndPointAddress = SetEndPointAddress(AzureCloud);
        }

        /// <summary>
        /// Saves credentials Azure Key and categoryID to the personalized settings file.
        /// </summary>
        public static void SaveCredentials()
        {
            Properties.Settings.Default.AzureKey = AzureKey;
            Properties.Settings.Default.CategoryID = CategoryID;
            Properties.Settings.Default.AppId = AppId;
            Properties.Settings.Default.ShowExperimental = ShowExperimental;
            Properties.Settings.Default.UseAdvancedSettings = UseAdvancedSettings;
            Properties.Settings.Default.AzureCloud = AzureCloud;
            EndPointAddress = SetEndPointAddress(AzureCloud);
            Properties.Settings.Default.SubscriptionRegion = AzureRegion;
            Properties.Settings.Default.UseCustomEndpoint = UseCustomEndpoint;
            Properties.Settings.Default.CustomEndpointUrl = CustomEndpointUrl;
            Properties.Settings.Default.Save();
        }

        public static void ResetCredentials()
        {
            Properties.Settings.Default.Reset();
        }

        /// <summary>
        /// Calculates the EndpointAddress to use for this cloud.
        /// </summary>
        /// <param name="Cloud">The cloud you want the Translator endpoint address for.</param>
        /// <returns></returns>
        private static string SetEndPointAddress(string Cloud)
        {
            Endpoints.cloud_endpoint cloud_Endpoint = new Endpoints.cloud_endpoint();

            if (string.IsNullOrEmpty(Cloud)) Cloud = "Global";

            try
            {
                return "https://" + Endpoints.CloudEndpoints[Cloud];
            }
            catch(KeyNotFoundException)
            {
                return "https://api.microsofttranslator.com";
            }

        }

    }
}
