﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
//using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using RelhaxModpack.Windows;
using System.Xml;


namespace RelhaxModpack
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Forms.NotifyIcon relhaxIcon;
        /// <summary>
        /// Creates the instance of the MainWindow class
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TheMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Hide();
            //load please wait thing
            ProgressIndicator progressIndicator = new ProgressIndicator()
            {
                Message = Translations.GetTranslatedString("loadingTranslations"),
                ProgressMinimum = 0,
                ProgressMaximum = 3
            };
            progressIndicator.Show();
            progressIndicator.UpdateProgress(0);
            //load translation hashes and set default language
            Translations.SetLanguage(Languages.English);
            Translations.LoadTranslations();
            //apply translations to this window
            Translations.LocalizeWindow(this,true);
            //create and localize the tray icons and menus
            CreateTray();
            //load and apply modpack settings
            progressIndicator.UpdateProgress(2, "LoadingSettings");
            ModpackSettings.LoadSettings();
            //apply settings to UI elements
            UISettings.LoadSettings(true);
            UISettings.ApplyUIColorSettings(this);
            //check command line settings
            CommandLineSettings.ParseCommandLineConflicts();
            //apply third party settings
            ThirdPartySettings.LoadSettings();
            //check for updates
            progressIndicator.UpdateProgress(3, "CheckForUpdates");
            CheckForApplicationUpdates();
            //dispose of please wait here
            progressIndicator.Close();
            progressIndicator = null;
            Show();
        }

        private void TheMainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Logging.WriteToLog("Saving settings");
            if (ModpackSettings.SaveSettings())
                Logging.WriteToLog("Settings saved");
            Logging.WriteToLog("Disposing tray icon");
            if(relhaxIcon != null)
            {
                relhaxIcon.Dispose();
                relhaxIcon = null;
            }
        }

        private void CreateTray()
        {
            //create base tray icon
            relhaxIcon = new System.Windows.Forms.NotifyIcon()
            {
                Visible = true,
                Icon = Properties.Resources.modpack_icon,
                Text = Title
            };
            //create menu options
            //TODO
        }

        private void CheckForApplicationUpdates()
        {
            //check if skipping updates
            Logging.WriteToLog("Started check for application updates");
            if(CommandLineSettings.SkipUpdate && ModpackSettings.DatabaseDistroVersion != DatabaseVersions.Test)
            {
                MessageBox.Show(Translations.GetTranslatedString("skipUpdateWarning"));
                Logging.WriteToLog("Skipping updates", Logfiles.Application, LogLevel.Warning);
                return;
            }
            //delete the last one and download a new one
            using (WebClient client = new WebClient())
            {
                try
                {
                    if (File.Exists("TODO"))
                        File.Delete("TODO");
                    client.DownloadFile("http://wotmods.relhaxmodpack.com/RelhaxModpack/managerInfo.dat", "TODO");

                }
                catch (Exception e)
                {
                    Logging.WriteToLog(string.Format("Failed to check for updates: \n{0}", e), Logfiles.Application, LogLevel.Exception);
                    .Application.Current.Shutdown();
                }
            }
            //get the version info string
            string xmlString = Utils.GetStringFromZip("TODO", "manager_version.xml");
            if(string.IsNullOrEmpty(xmlString))
            {
                Logging.WriteToLog("Failed to get get xml string from managerInfo.dat", Logfiles.Application, LogLevel.Exception);
                return;
            }
            //load the document info
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlString);
            //get a string of current applicatoin version

            //get a string of online application version

            //if not equal, update required

        }

        private void ApplySettingsToUIOnApplicationLoad()
        {
            //add localization translation options to combobox
            //TODO these need to be in the lanaugae thatthey are in
            Languages[] allLanguages = (Languages[])Enum.GetValues(typeof(Languages));
            foreach (Languages lang in allLanguages)
                LanguagesSelector.Items.Add(lang.ToString());
        }

        #region all the dumb events for all the changing of settings
        private void OnSelectionViewChanged(object sender, RoutedEventArgs e)
        {
            //selection view code for each new view goes here
            if ((bool)SelectionDefault.IsChecked)
                ModpackSettings.ModSelectionView = SelectionView.DefaultV2;
            else if ((bool)SelectionLegacy.IsChecked)
                ModpackSettings.ModSelectionView = SelectionView.Legacy;
        }

        private void OnMulticoreExtractionChanged(object sender, RoutedEventArgs e)
        {
            ModpackSettings.MulticoreExtraction = (bool)MulticoreExtractionCB.IsChecked;
        }

        private void OnCreateShortcutsChanged(object sender, RoutedEventArgs e)
        {
            ModpackSettings.CreateShortcuts = (bool)CreateShortcutsCB.IsChecked;
        }

        private void OnSaveUserDataChanged(object sender, RoutedEventArgs e)
        {
            ModpackSettings.SaveUserData = (bool)SaveUserDataCB.IsChecked;
        }

        private void OnClearWoTCacheChanged(object sender, RoutedEventArgs e)
        {
            ModpackSettings.ClearCache = (bool)ClearCacheCB.IsChecked;
        }

        private void OnClearLogFilesChanged(object sender, RoutedEventArgs e)
        {
            ModpackSettings.DeleteLogs = (bool)ClearLogFilesCB.IsChecked;
        }

        private void OnCleanInstallChanged(object sender, RoutedEventArgs e)
        {
            ModpackSettings.CleanInstallation = (bool)CleanInstallCB.IsChecked;
        }

        private void OnImmidateExtarctionChanged(object sender, RoutedEventArgs e)
        {
            ModpackSettings.DownloadInstantExtraction = (bool)InstantExtractionCB.IsChecked;
        }

        private void OnShowInstallCompleteWindowChanged(object sender, RoutedEventArgs e)
        {
            ModpackSettings.ShowInstallCompleteWindow = (bool)ShowInstallCompleteWindowCB.IsChecked;
        }

        private void OnBackupModsChanged(object sender, RoutedEventArgs e)
        {
            ModpackSettings.BackupModFolder = (bool)BackupModsCB.IsChecked;
        }

        private void OnPreviewLoadingImageChange(object sender, RoutedEventArgs e)
        {
            if ((bool)ThirdGuardsLoadingImageRB.IsChecked)
                ModpackSettings.GIF = LoadingGifs.ThirdGuards;
            else if ((bool)StandardImageRB.IsChecked)
                ModpackSettings.GIF = LoadingGifs.Standard;
        }

        private void OnForceManuelGameDetectionChanged(object sender, RoutedEventArgs e)
        {
            ModpackSettings.ForceManuel = (bool)ForceManuelGameDetectionCB.IsChecked;
        }

        private void OnInformIfNoNewDatabaseChanged(object sender, RoutedEventArgs e)
        {
            ModpackSettings.NotifyIfSameDatabase = (bool)NotifyIfSameDatabaseCB.IsChecked;
        }

        private void OnSaveLastInstallChanged(object sender, RoutedEventArgs e)
        {
            ModpackSettings.SaveLastConfig = (bool)SaveLastInstallCB.IsChecked;
        }

        private void OnUseBetaAppChanged(object sender, RoutedEventArgs e)
        {
            if ((bool)UseBetaApplicationCB.IsChecked)
                ModpackSettings.ApplicationDistroVersion = ApplicationVersions.Beta;
            else if (!(bool)UseBetaApplicationCB.IsChecked)
                ModpackSettings.ApplicationDistroVersion = ApplicationVersions.Stable;
        }

        private void OnUseBetaDatabaseChanged(object sender, RoutedEventArgs e)
        {
            if ((bool)UseBetaDatabaseCB.IsChecked)
                ModpackSettings.DatabaseDistroVersion = DatabaseVersions.Beta;
            else if (!(bool)UseBetaDatabaseCB.IsChecked)
                ModpackSettings.DatabaseDistroVersion = DatabaseVersions.Stable;
        }

        private void OnDefaultBordersV2Changed(object sender, RoutedEventArgs e)
        {
            ModpackSettings.EnableBordersDefaultV2View = (bool)EnableBordersDefaultV2CB.IsChecked;
        }

        private void OnDefaultSelectColorChanged(object sender, RoutedEventArgs e)
        {
            ModpackSettings.EnableColorChangeDefaultV2View = (bool)EnableColorChangeDefaultV2CB.IsChecked;
        }

        private void OnLegacyBordersChanged(object sender, RoutedEventArgs e)
        {
            ModpackSettings.EnableBordersLegacyView = (bool)EnableBordersLegacyCB.IsChecked;
        }

        private void OnLegacySelectColorChenged(object sender, RoutedEventArgs e)
        {
            ModpackSettings.EnableColorChangeLegacyView = (bool)EnableColorChangeLegacyCB.IsChecked;
        }

        private void OnLanguageSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Translations.SetLanguage((Languages)LanguagesSelector.SelectedIndex);
            Translations.LocalizeWindow(this, true);
        }
        #endregion

        private void InstallModpackButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UninstallModpackButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DiagnosticUtilitiesButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ViewNewsButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DumpColorSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog()
            {
                AddExtension = true,
                CheckPathExists = true,
                OverwritePrompt = true,
                RestoreDirectory = true,
                DefaultExt = "xml",
                Title = Translations.GetTranslatedString("ColorDumpSaveFileDialog"),
                Filter = "XML Documents|*.xml"
            };
            bool result = (bool)saveFileDialog.ShowDialog();
            if(result)
            {
                Logging.WriteToLog("Saving color settings dump to " + saveFileDialog.FileName);
                UISettings.DumpAllWindowColorSettingsToFile(saveFileDialog.FileName);
                Logging.WriteToLog("Color settings saved");
            }
        }
    }
}