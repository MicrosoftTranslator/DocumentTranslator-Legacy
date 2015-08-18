// // ----------------------------------------------------------------------
// // <copyright file="SettingsAppearanceViewModel.cs" company="Microsoft Corporation">
// // Copyright (c) Microsoft Corporation.
// // All rights reserved.
// // THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// // KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// // IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// // PARTICULAR PURPOSE.
// // </copyright>
// // ----------------------------------------------------------------------
// // <summary>SettingsAppearanceViewModel.cs</summary>
// // ----------------------------------------------------------------------

namespace TranslationAssistant.DocumentTranslationInterface.Content
{
    #region

    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Media;

    using FirstFloor.ModernUI.Presentation;

    #endregion

    /// <summary>
    ///     A simple view model for configuring theme, font and accent colors.
    /// </summary>
    /// <summary>
    ///     A simple view model for configuring theme, font and accent colors.
    /// </summary>
    public class SettingsAppearanceViewModel : NotifyPropertyChanged
    {
        #region Constants

        private const string FontLarge = "large";

        private const string FontSmall = "small";

        #endregion

        // 9 accent colors from modern design principles
        /*private Color[] accentColors = new Color[]{
            Color.FromRgb(0x33, 0x99, 0xff),   // blue
            Color.FromRgb(0x00, 0xab, 0xa9),   // teal
            Color.FromRgb(0x33, 0x99, 0x33),   // green
            Color.FromRgb(0x8c, 0xbf, 0x26),   // lime
            Color.FromRgb(0xf0, 0x96, 0x09),   // orange
            Color.FromRgb(0xff, 0x45, 0x00),   // orange red
            Color.FromRgb(0xe5, 0x14, 0x00),   // red
            Color.FromRgb(0xff, 0x00, 0x97),   // magenta
            Color.FromRgb(0xa2, 0x00, 0xff),   // purple            
        };*/

        // 20 accent colors from Windows Phone 8

        #region Fields

        private readonly Color[] accentColors = { Color.FromRgb(0xa4, 0xc4, 0x00), // lime
                                                  Color.FromRgb(0x60, 0xa9, 0x17), // green
                                                  Color.FromRgb(0x00, 0x8a, 0x00), // emerald
                                                  Color.FromRgb(0x00, 0xab, 0xa9), // teal
                                                  Color.FromRgb(0x1b, 0xa1, 0xe2), // cyan
                                                  Color.FromRgb(0x00, 0x50, 0xef), // cobalt
                                                  Color.FromRgb(0x6a, 0x00, 0xff), // indigo
                                                  Color.FromRgb(0xaa, 0x00, 0xff), // violet
                                                  Color.FromRgb(0xf4, 0x72, 0xd0), // pink
                                                  Color.FromRgb(0xd8, 0x00, 0x73), // magenta
                                                  Color.FromRgb(0xa2, 0x00, 0x25), // crimson
                                                  Color.FromRgb(0xe5, 0x14, 0x00), // red
                                                  Color.FromRgb(0xfa, 0x68, 0x00), // orange
                                                  Color.FromRgb(0xf0, 0xa3, 0x0a), // amber
                                                  Color.FromRgb(0xe3, 0xc8, 0x00), // yellow
                                                  Color.FromRgb(0x82, 0x5a, 0x2c), // brown
                                                  Color.FromRgb(0x6d, 0x87, 0x64), // olive
                                                  Color.FromRgb(0x64, 0x76, 0x87), // steel
                                                  Color.FromRgb(0x76, 0x60, 0x8a), // mauve
                                                  Color.FromRgb(0x87, 0x79, 0x4e) // taupe
                                                };

        private readonly LinkCollection themes = new LinkCollection();

        private Color selectedAccentColor;

        private string selectedFontSize;

        private Link selectedTheme;

        #endregion

        #region Constructors and Destructors

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SettingsAppearanceViewModel()
        {
            // add the default themes
            this.themes.Add(new Link { DisplayName = "dark", Source = AppearanceManager.DarkThemeSource });
            this.themes.Add(new Link { DisplayName = "light", Source = AppearanceManager.LightThemeSource });

            this.SelectedFontSize = AppearanceManager.Current.FontSize == FontSize.Large ? FontLarge : FontSmall;
            this.SyncThemeAndColor();

            AppearanceManager.Current.PropertyChanged += this.OnAppearanceManagerPropertyChanged;
        }

        #endregion

        #region Public Properties

        public Color[] AccentColors
        {
            get
            {
                return this.accentColors;
            }
        }

        public string[] FontSizes
        {
            get
            {
                return new[] { FontSmall, FontLarge };
            }
        }

        public Color SelectedAccentColor
        {
            get
            {
                return this.selectedAccentColor;
            }
            set
            {
                if (this.selectedAccentColor != value)
                {
                    this.selectedAccentColor = value;
                    this.OnPropertyChanged("SelectedAccentColor");

                    AppearanceManager.Current.AccentColor = value;
                }
            }
        }

        public string SelectedFontSize
        {
            get
            {
                return this.selectedFontSize;
            }
            set
            {
                if (this.selectedFontSize != value)
                {
                    this.selectedFontSize = value;
                    this.OnPropertyChanged("SelectedFontSize");

                    AppearanceManager.Current.FontSize = value == FontLarge ? FontSize.Large : FontSize.Small;
                }
            }
        }

        public Link SelectedTheme
        {
            get
            {
                return this.selectedTheme;
            }
            set
            {
                if (this.selectedTheme != value)
                {
                    this.selectedTheme = value;
                    this.OnPropertyChanged("SelectedTheme");

                    // and update the actual theme
                    AppearanceManager.Current.ThemeSource = value.Source;
                }
            }
        }

        public LinkCollection Themes
        {
            get
            {
                return this.themes;
            }
        }

        #endregion

        #region Methods

        private void OnAppearanceManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ThemeSource" || e.PropertyName == "AccentColor")
            {
                this.SyncThemeAndColor();
            }
        }

        private void SyncThemeAndColor()
        {
            // synchronizes the selected viewmodel theme with the actual theme used by the appearance manager.
            this.SelectedTheme = this.themes.FirstOrDefault(l => l.Source.Equals(AppearanceManager.Current.ThemeSource));

            // and make sure accent color is up-to-date
            this.SelectedAccentColor = AppearanceManager.Current.AccentColor;
        }

        #endregion
    }
}