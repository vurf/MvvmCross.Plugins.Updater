namespace MvvmCross.Plugins.Updater.Droid
{
    using System;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Android.App;
    using Android.Content;
    using ModernHttpClient;

    [Preserve(AllMembers = true)]
    public class MvxUpdaterHelper : IMvxUpdaterHelper
    {
        private const string VersionPattern = @"(?<=<div class=""content"" itemprop=""softwareVersion"">)(.+?)(?=</div>)";
        private const string WhatsNewPattern = @"(?<=<div class=""recent-change"">)(.+?)(?=</div>)";
        private const string ApplicationNamePattern = @"(?<=<div class=""id-app-title"" tabindex=""0"">)(.+?)(?=</div>)";
        private Context _context;

        private Context Context
        {
            get { return this._context ?? (this._context = Application.Context); }
        }

        private string Bundle
        {
            get { return this.Context.PackageName; }
        }

        public async Task<bool> UpdateAvailable()
        {
            var result = await this.GetResponse();
            var storeVersion = Version.Parse(this.GetStoreMatch(VersionPattern, result.Content));
            var currentVersion = Version.Parse(this.Context.PackageManager.GetPackageInfo(this.Bundle, 0).VersionName);

            return currentVersion < storeVersion;
        }

        public async Task<MvxApplicationModel> GetInfoApplication()
        {
            var result = await this.GetResponse();
            var storeVersion = Version.Parse(this.GetStoreMatch(VersionPattern, result.Content));
            var storeWhatsNew = this.GetStoreMatch(WhatsNewPattern, result.Content)?.Trim();
            var storeName = this.GetStoreMatch(ApplicationNamePattern, result.Content)?.Trim(); 
            var currentVersion = Version.Parse(this.Context.PackageManager.GetPackageInfo(this.Bundle, 0).VersionName);
            
            return new MvxApplicationModel()
            {
                Name = storeName,
                Version = storeVersion,
                WhatsNew = storeWhatsNew,
                Url = result.Url,
                UpdateAvailable = storeVersion > currentVersion
            };
        }
        
        private async Task<MvxResponseModel> GetResponse()
        {
            try
            {
                var devRegion = "ru";
                var url = $"https://play.google.com/store/apps/details?id={this.Bundle}&hl={devRegion}";
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
        
        private string GetStoreMatch(string pattern, string content)
        {
            var regext = new Regex(pattern, RegexOptions.ExplicitCapture);
            var storeMatch = regext.Match(content);
            if (storeMatch.Success)
            {
                return storeMatch.Value;
            }

            return string.Empty;
        }
    }
}