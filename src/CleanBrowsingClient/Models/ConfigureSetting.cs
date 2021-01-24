using System;
using System.Collections.Generic;
using System.Text;


namespace CleanBrowsingClient.Models
{
    [Serializable]
    class ConfigureSetting
    {
        public string UserCode { get; set; }
        public string PinCode { get; set; }

        public ConfigureSetting(string ucode, string pcode)
        {
            this.UserCode = ucode;
            this.PinCode = pcode;
        }

        public ConfigureSetting()
        {
            this.UserCode = "";
            this.PinCode = "";
        }
    }
}
