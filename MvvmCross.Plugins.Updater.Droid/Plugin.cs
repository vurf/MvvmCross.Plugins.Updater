namespace MvvmCross.Plugins.Updater.Droid
{
    using MvvmCross.Platform;
    using MvvmCross.Platform.Plugins;

    [Preserve(AllMembers = true)]
    public class Plugin : IMvxPlugin
    {
        public void Load()
        {
            Mvx.RegisterType<IMvxUpdaterHelper, MvxUpdaterHelper>();
        }
    }
}