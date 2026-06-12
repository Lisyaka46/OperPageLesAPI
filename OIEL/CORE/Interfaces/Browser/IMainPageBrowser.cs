using OPLAPI.OIEL.CORE.Browser;
using System.Collections.ObjectModel;

namespace OPLAPI.OIEL.CORE.Interfaces.Browser
{
    /// <summary>
    /// Интерфейс объекта контента главной страницы браузера OPL
    /// </summary>
    /// <remarks>
    /// Интерфейс реализует базовая страница браузера, предоставляя контейнер для выбора открываемого страничного приложения
    /// </remarks>
    internal interface IMainPageBrowser : IPageBrowser, IOPLElementBaseContent
    {
        /// <summary>
        /// Массив всех страничных приложений доступный только для чтения
        /// </summary>
        internal abstract ReadOnlyCollection<AppPage> AppPages { get; }
    }
}
