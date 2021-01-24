using CleanBrowsingClient.Config;
using CleanBrowsingClient.Events;
using CleanBrowsingClient.Models;
using CleanBrowsingClient.Helper;
using DnsCrypt.Stamps;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace CleanBrowsingClient.ViewModels
{
    public class StampViewModel : BindableBase
    {
        private string _stamp;
        private readonly ILoggerFacade _logger;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private ISnackbarMessageQueue _messageQueue;

        private ConfigureSettingManager confSetting = new ConfigureSettingManager();

        public ISnackbarMessageQueue MessageQueue
        {
            get { return _messageQueue; }
            set { SetProperty(ref _messageQueue, value); }
        }

        public string Stamp
        {
            get { return _stamp; }
            set { SetProperty(ref _stamp, value); }
        }

        public DelegateCommand NavigateToMainView { get; private set; }
        public DelegateCommand SaveStampCommand { get; private set; }
        public StampViewModel(
            ILoggerFacade logger,
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            ISnackbarMessageQueue snackbarMessageQueue)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _regionManager = regionManager ?? throw new ArgumentNullException(nameof(regionManager));
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            _messageQueue = snackbarMessageQueue ?? throw new ArgumentNullException(nameof(snackbarMessageQueue));
            NavigateToMainView = new DelegateCommand(NavigateToMain);
            SaveStampCommand = new DelegateCommand(async() =>
                {
                    await SaveStamp();
            });
        }

        private void NavigateToMain()
        {
            _regionManager.RequestNavigate("ContentRegion", "MainView");
        }
        
        private async Task SaveStamp()
        {
            if (!string.IsNullOrEmpty(Stamp))
            {
                //2021/01/22
                using var client = new HttpClient()
                {
                    DefaultRequestVersion = new Version(2, 0)
                };
                string hostName = Environment.GetEnvironmentVariable("COMPUTERNAME");
                string get_stamp_url = string.Format("https://my.cleanbrowsing.org/apis/devices/get-stamp?apikey={0}&device-name={1}&device-type=Windows", Stamp.Trim(), hostName);
                var response = await client.GetStringAsync(get_stamp_url);

                if (response != null)
                {
                    var res_stamp = response.Replace("\n", "");

                    var decodedStamp = StampTools.Decode(res_stamp);
                    if (decodedStamp != null)
                    {
                        var addStamp = false;
                        if (decodedStamp.Protocol == DnsCrypt.Models.StampProtocol.DnsCrypt)
                        {
                            //simple check if the stamp is a valid cleanbrowsing stamp
                            if (decodedStamp.ProviderName.Equals(Global.ValidCleanBrowsingDnsCryptStamp))
                            {
                                _logger.Log("valid DnsCrypt stamp", Category.Info, Priority.Low);
                                addStamp = true;
                            }
                        }
                        else if (decodedStamp.Protocol == DnsCrypt.Models.StampProtocol.DoH)
                        {
                            //simple check if the stamp is a valid cleanbrowsing stamp
                            if (decodedStamp.Hostname.Equals(Global.ValidCleanBrowsingDohStamp))
                            {
                                _logger.Log("valid DoH stamp", Category.Info, Priority.Low);
                                addStamp = true;
                            }
                        }
                        else
                        {
                            //unsupported stamp
                            _logger.Log("unsupported stamp type", Category.Warn, Priority.Medium);
                            addStamp = false;
                        }

                        if (addStamp)
                        {
                            var prof_setting = new ConfigureSetting();
                            if (File.Exists(confSetting.FileName))
                            {
                                prof_setting = confSetting.Deserialize();
                            }

                            prof_setting.UserCode = Stamp.Trim();
                            confSetting.Serialize(prof_setting);

                            _eventAggregator.GetEvent<StampAddedEvent>().Publish(new Proxy
                            {
                                Name = Global.DefaultCustomFilterKey,
                                Stamp = res_stamp
                            });
                            NavigateToMain();
                        }
                        else
                        {
                            MessageQueue.Enqueue("not a valid [CODE]");
                        }
                    }
                    else
                    {
                        MessageQueue.Enqueue("not a valid [CODE]");
                    }
                }
                
            }
        }
    }
}
