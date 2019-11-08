namespace TranslationAssistant.TranslationServices.Core
{
    public static class LoadSaveCredentials
    {

        /// <summary>
        /// Loads credentials from settings file.
        /// Doesn't need to be public, because it is called during Initialize();
        /// </summary>
        public static void LoadCredentials()
        {
            TranslationServiceFacade.AzureKey = Properties.Settings.Default.AzureKey;
            TranslationServiceFacade.CategoryID = Properties.Settings.Default.CategoryID;
            TranslationServiceFacade.AppId = Properties.Settings.Default.AppId;
            TranslationServiceFacade.UseAdvancedSettings = Properties.Settings.Default.UseAdvancedSettings;
            TranslationServiceFacade.Adv_CategoryId = Properties.Settings.Default.Adv_CategoryID;
            TranslationServiceFacade.UseAzureGovernment = Properties.Settings.Default.UseAzureGovernment;
            TranslationServiceFacade.UseCustomEndpoint = Properties.Settings.Default.UseCustomEndpoint;
            TranslationServiceFacade.CustomEndpointUrl = Properties.Settings.Default.CustomEndpointUrl;
            SpeechServiceFacade.SpeechAccountKey = Properties.Settings.Default.SpeechAccountKey;
        }

        /// <summary>
        /// Saves credentials Azure Key and categoryID to the personalized settings file.
        /// </summary>
        public static void SaveCredentials()
        {
            Properties.Settings.Default.AzureKey = TranslationServiceFacade.AzureKey;
            Properties.Settings.Default.CategoryID = TranslationServiceFacade.CategoryID;
            Properties.Settings.Default.AppId = TranslationServiceFacade.AppId;
            Properties.Settings.Default.UseAdvancedSettings = TranslationServiceFacade.UseAdvancedSettings;
            Properties.Settings.Default.Adv_CategoryID = TranslationServiceFacade.Adv_CategoryId;
            Properties.Settings.Default.UseAzureGovernment = TranslationServiceFacade.UseAzureGovernment;
            Properties.Settings.Default.UseCustomEndpoint = TranslationServiceFacade.UseCustomEndpoint;
            Properties.Settings.Default.CustomEndpointUrl = TranslationServiceFacade.CustomEndpointUrl;
            Properties.Settings.Default.SpeechAccountKey = SpeechServiceFacade.SpeechAccountKey;
            Properties.Settings.Default.Save();
        }

    }
}
