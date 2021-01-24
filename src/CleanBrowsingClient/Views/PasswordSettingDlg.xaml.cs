using CleanBrowsingClient.Helper;
using CleanBrowsingClient.Models;
using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace CleanBrowsingClient.Views
{
    /// <summary>
    /// Interaction logic for PasswordSettingDlg.xaml
    /// </summary>
    public partial class PasswordSettingDlg : Window
    {
        private DispatcherTimer dispatcherTimer;
        private ConfigureSettingManager confSetting = new ConfigureSettingManager();
        private ConfigureSetting prof_setting = new ConfigureSetting();
        
        public PasswordSettingDlg()
        {
            InitializeComponent();
            this.msgLabel.Visibility = Visibility.Hidden;

            if (File.Exists(confSetting.FileName))
            {
                prof_setting = confSetting.Deserialize();
            }
            //Create a timer with interval of 2 secs
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
        }

        private void Set_Click(object sender, RoutedEventArgs e)
        {
            if (this.newpassTextBox.Password.Length > 3)
            {
                if (this.newpassTextBox.Password.Equals(this.confirmTextBox.Password))
                {
                    prof_setting.PinCode = this.newpassTextBox.Password;
                    confSetting.Serialize(prof_setting);
                    this.Close();
                }
                else
                {
                    this.msgLabel.Content = "Don't match the password.";
                    this.msgLabel.Visibility = Visibility.Visible;
                    //Create a timer with interval of 2 secs
                    dispatcherTimer = new DispatcherTimer();
                    dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                    dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
                    dispatcherTimer.Start();

                    this.confirmTextBox.Password = "";
                    this.confirmTextBox.Focus();
                }
            } else
            {
                this.msgLabel.Content = "Please input long password.";
                this.msgLabel.Visibility = Visibility.Visible;
                
                dispatcherTimer.Start();

                this.confirmTextBox.Password = "";
                this.newpassTextBox.Password = "";
                this.newpassTextBox.Focus();
            }
            
        }

        private void Clear_click(object sender, RoutedEventArgs e)
        {
            prof_setting.PinCode = "";
            confSetting.Serialize(prof_setting);
            this.Close();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            msgLabel.Visibility = Visibility.Hidden;
            //Disable the timer
            dispatcherTimer.IsEnabled = false;
        }
    }
}
