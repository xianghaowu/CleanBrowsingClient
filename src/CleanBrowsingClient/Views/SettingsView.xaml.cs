using CleanBrowsingClient.Helper;
using CleanBrowsingClient.Models;
using System.Windows.Controls;
using System.IO;

namespace CleanBrowsingClient.Views
{
    public partial class SettingsView : UserControl
    {
        private ConfigureSettingManager confSetting = new ConfigureSettingManager();
        private ConfigureSetting prof_setting = new ConfigureSetting();

        public SettingsView()
        {
            InitializeComponent();
            if (File.Exists(confSetting.FileName))
            {
                prof_setting = confSetting.Deserialize();
            }

            if (prof_setting.PinCode.Length == 0)
            {
                this.PasswordBtn.Content = "Set Password";
            }
            else
            {
                this.PasswordBtn.Content = "Change Password";
            }
        }
    }
}
