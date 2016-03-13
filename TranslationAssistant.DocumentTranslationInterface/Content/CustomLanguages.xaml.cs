using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        }

        private void CustomLanguageGrid_TargetUpdated(object sender, DataTransferEventArgs e)
        {

        }

        private void CustomLanguageGrid_SourceUpdated(object sender, DataTransferEventArgs e)
        {

        }
    }
}
