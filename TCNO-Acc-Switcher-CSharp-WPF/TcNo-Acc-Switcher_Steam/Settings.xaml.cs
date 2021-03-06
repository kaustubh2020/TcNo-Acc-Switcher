﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TcNo_Acc_Switcher_Globals;


namespace TcNo_Acc_Switcher_Steam
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>+

    public partial class Settings
    {
        private MainWindow _mw;
        private bool _enableButtons;
        public Settings()
        {
            InitializeComponent();
        }

        private bool _shown;
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (_shown)
                return;

            _shown = true;
            // Enables buttons once the settings page has loaded. This stops them being fired by .NET setting values.
            _enableButtons = true;
        }


        public void ShareMainWindow(MainWindow imw) => _mw = imw;
        private void BtnExit(object sender, RoutedEventArgs e) => Globals.WindowHandling.BtnExit(sender, e, this);
        private void BtnMinimize(object sender, RoutedEventArgs e) => Globals.WindowHandling.BtnMinimize(sender, e, this);
        private void DragWindow(object sender, MouseButtonEventArgs e) => Globals.WindowHandling.DragWindow(sender, e, this);
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }
        private void btnResetSettings_Click(object sender, RoutedEventArgs e)
        {
            if (_mw.VacCheckRunning) return;
            var messageBoxResult = MessageBox.Show("Are you sure?", "Reset settings", MessageBoxButton.YesNo);
            if (messageBoxResult != MessageBoxResult.Yes) return;
            _mw.ResetSettings();
            this.Close();
        }
        private void btnPickSteamFolder_Click(object sender, RoutedEventArgs e) => _mw.PickSteamFolder();
        private void btnResetImages_Click(object sender, RoutedEventArgs e)
        {
            if (_mw.VacCheckRunning) return;
            var messageBoxResult = MessageBox.Show("Are you sure?", "Reset settings", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
                _mw.ResetImages();
        }
        private void btnCheckVac_Click(object sender, RoutedEventArgs e)
        {
            if (_enableButtons)
                _mw.CheckVac();
        }

        private void ShowSteamID_CheckChanged(object sender, RoutedEventArgs e)
        {
            if (_enableButtons)
                _mw.ShowSteamIDHidden.IsChecked = ShowSteamID.IsChecked != null && (bool)ShowSteamID.IsChecked;
        }

        private void ShowVACStatus_CheckChanged(object sender, RoutedEventArgs e)
        {
            if (_enableButtons)
                _mw.ToggleVacStatus(ShowVACStatus.IsChecked != null && (bool)ShowVACStatus.IsChecked);
        }

        private void LimitedAsVAC_CheckChanged(object sender, RoutedEventArgs e)
        {
            if (_enableButtons)
                _mw.ToggleLimitedAsVac(LimitedAsVAC.IsChecked != null && (bool)LimitedAsVAC.IsChecked);
        }

        private void btnRestoreForgotten_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(_mw.GetForgottenBackupPath()))
            {
                var restoreForgottenDialog = new RestoreForgotten();
                restoreForgottenDialog.ShareMainWindow(_mw);
                restoreForgottenDialog.Owner = this;
                restoreForgottenDialog.ShowDialog();
            }
            else
            {
                MessageBox.Show($"No backups available. ({_mw.GetForgottenBackupPath()})");
            }
        }

        private void btnClearForgottenBackups_Click(object sender, RoutedEventArgs e)
        {
            _mw.ClearForgottenBackups();
        }

        private void btnOpenSteamFolder_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", _mw.GetSteamDirectory());
        }

        private void btnAdvancedCleaning_Click(object sender, RoutedEventArgs e)
        {
            var clearLoginsDialog = new ClearLogins() {Owner = this};
            clearLoginsDialog.ShareMainWindow(_mw);
            clearLoginsDialog.ShowDialog();
        }

        private void ToggleStartShortcut_CheckChanged(object sender, RoutedEventArgs e)
        {
            if (_enableButtons)
                _mw.StartMenuShortcut(ToggleStartShortcut.IsChecked != null && (bool)ToggleStartShortcut.IsChecked);
        }
        private void ToggleStartWithWindows_CheckChanged(object sender, RoutedEventArgs e)
        {
            if (_enableButtons)
                _mw.StartWithWindows(ToggleStartWithWindows.IsChecked != null && (bool)ToggleStartWithWindows.IsChecked);
        }
        private void ToggleDesktopShortcut_CheckChanged(object sender, RoutedEventArgs e)
        {
            if (_enableButtons)
                _mw.DesktopShortcut(ToggleDesktopShortcut.IsChecked != null && (bool)ToggleDesktopShortcut.IsChecked);
        }

        private void ToggleAccNames_CheckChanged(object sender, RoutedEventArgs e)
        {
            if (_enableButtons)
                _mw.ToggleAccNames(ToggleAccNames.IsChecked != null && (bool)ToggleAccNames.IsChecked);
        }

        private void NumberRecentAccounts_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_enableButtons) return;
            if (NumberRecentAccounts.Text == "")
            {
                NumberRecentAccounts.Text = "0";
                return;
            }

            if ((NumberRecentAccounts.Text).Length > 1)
                while ((NumberRecentAccounts.Text)[0] == '0')
                    NumberRecentAccounts.Text = NumberRecentAccounts.Text.Substring(1);

            if (int.TryParse(NumberRecentAccounts.Text, out _))
                _mw.SetTotalRecentAccount(NumberRecentAccounts.Text);
            else
                NumberRecentAccounts.Text = new string((NumberRecentAccounts.Text).Where(c => "0123456789".Contains(c)).ToArray());
        }

        private void Settings_OnClosing(object sender, CancelEventArgs e) => _mw.CapTotalTrayUsers();

        private void ImageExpiry_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_enableButtons) return;
            if (ImageExpiry.Text == "")
            {
                ImageExpiry.Text = "0";
                return;
            }

            if (ImageExpiry.Text.Length > 1)
                while (ImageExpiry.Text[0] == '0')
                    ImageExpiry.Text = ImageExpiry.Text.Substring(1);

            if (int.TryParse(ImageExpiry.Text, out _))
                _mw.SetImageExpiry(ImageExpiry.Text);
            else
                ImageExpiry.Text = new string(ImageExpiry.Text.Where(c => "0123456789".Contains(c)).ToArray());
        }
    }
}
