namespace MvvmCross.Plugins.Updater
{
    using System.Threading.Tasks;

    public interface IMvxUpdaterHelper
    {
        /// <summary>
        /// Проверить наличие новой версии в магазине
        /// </summary>
        /// <returns>True - если есть</returns>
        Task<bool> UpdateAvailable();

        /// <summary>
        /// Получить информацию о приложении в сторе
        /// </summary>
        /// <returns>Информацию о приложении</returns>
        Task<MvxApplicationModel> GetInfoApplication();
    }
}