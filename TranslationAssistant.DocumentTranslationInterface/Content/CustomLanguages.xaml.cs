using System.Windows.Controls;
using System.Windows.Data;

namespace TranslationAssistant.DocumentTranslationInterface.Content
{
    /// <summary>
    /// Interaction logic for CustomLanguages.xaml
    /// </summary>
    public partial class CustomLanguages : UserControl
    {
        public CustomLanguages()
        {
            InitializeComponent();
            TranslationServices.Core.CustomLanguages CL = new TranslationServices.Core.CustomLanguages();
        }

        private void CustomLanguageGrid_TargetUpdated(object sender, DataTransferEventArgs e)
        {

        }

        private void CustomLanguageGrid_SourceUpdated(object sender, DataTransferEventArgs e)
        {

        }
    }
}
