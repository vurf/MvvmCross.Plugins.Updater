namespace MvvmCross.Plugins.Updater.iOS
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Foundation;
    using ModernHttpClient;
    using Newtonsoft.Json;

    [Updater.Preserve(AllMembers = true)]
    public class MvxUpdaterHelper : IMvxUpdaterHelper
    {
        private NSDictionary _info;

        private NSDictionary Info
        {
            get { return this._info ?? (this._info = NSBundle.MainBundle.InfoDictionary); }
        }

        private string Bundle
        {
            get { return this.Info["CFBundleIdentifier"].ToString(); }
        }

        public async Task<bool> UpdateAvailable()
        {
            try
            {
                var response = await this.GetResponse();
                var jsonObject = JsonConvert.DeserializeObject<dynamic>(response.Content);
                var appstoreVersion = new Version(jsonObject?.results[0]?.version?.ToString());
                var currentVersion = Version.Parse(this.Info["CFBundleShortVersionString"].ToString());

                return currentVersion < appstoreVersion;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<MvxApplicationModel> GetInfoApplication()
        {
            try
            {
                var response = await this.GetResponse();
                var jsonObject = JsonConvert.DeserializeObject<dynamic>(response.Content);
                var resultInfo = jsonObject?.results[0];
                var storeVersion = new Version(resultInfo.version.ToString());
                var storeWhatsNew = resultInfo.releaseNotes.ToString();
                var storeName = resultInfo.trackCensoredName.ToString();
                var currentVersion = Version.Parse(this.Info["CFBundleShortVersionString"].ToString());

                return new MvxApplicationModel()
                {
                    Name = storeName,
                    Version = storeVersion,
                    WhatsNew = storeWhatsNew,
                    Url = response.Url,
                    UpdateAvailable = storeVersion > currentVersion
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        
        private async Task<MvxResponseModel> GetResponse()
        {
            try
            {
                var devRegion = "ru";// info["CFBundleDevelopmentRegion"].ToString();
                var url = $"http://itunes.apple.com/lookup?bundleId={this.Bundle}&country={devRegion}";
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var httpClient = new HttpClient(new NativeMessageHandler());
                var response = await httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();
                
                return new MvxResponseModel()
                {
                    Url = url,
                    Content = content,
                    IsSuccess = true
                };
            }
            catch (Exception e)
            {
                return new MvxResponseModel()
                {
                    IsSuccess = false,
                    ErrorMessage = e.Message
                };
            }
        }
    }
}