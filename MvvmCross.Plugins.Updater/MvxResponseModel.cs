namespace MvvmCross.Plugins.Updater
{
    public class MvxResponseModel
    {
        public string Url { get; set; }
        
        public string Content { get; set; }
        
        public bool IsSuccess { get; set; }
        
        public string ErrorMessage { get; set; }
    }
}