using CleanBrowsingClient.Helper;
using CleanBrowsingClient.Models;
using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace CleanBrowsingClient.Views
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class PasswordDlg : Window
    {
        private DispatcherTimer dispatcherTimer;
        private ConfigureSettingManager confSetting = new ConfigureSettingManager();
        private ConfigureSetting prof_setting = new ConfigureSetting();

        public PasswordDlg()
        {
            InitializeComponent();
            if (File.Exists(confSetting.FileName))
            {
                prof_setting = confSetting.Deserialize();
            }

            //Create a timer with interval of 2 secs
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
            dispatcherTimer.Start();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (this.passwordTextBox.Password.Equals(prof_setting.PinCode))
            {
                this.DialogResult = true;
                this.Close();
            } else
            {
                this.msgLabel.Content = "Wrong Password";
                this.msgLabel.Visibility = Visibility.Visible;
                dispatcherTimer.Start();
                this.passwordTextBox.Password = "";
                this.passwordTextBox.Focus();
            }
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            msgLabel.Visibility = Visibility.Hidden;
            //Disable the timer
            dispatcherTimer.IsEnabled = false;
        }
    }
}
