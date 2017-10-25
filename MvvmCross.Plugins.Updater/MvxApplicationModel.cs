namespace MvvmCross.Plugins.Updater
{
    using System;

    public class MvxApplicationModel
    {
        public string Name { get; set; }
        
        public string WhatsNew { get; set; }

        public Version Version { get; set; }
        
        public string Url { get; set; }
        
        public bool UpdateAvailable { get; set; }
    }
}