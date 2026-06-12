using OPLAPI.CORE.Animation;
using OPLAPI.CORE.Interfaces;
using OPLAPI.OIEL.CORE.Interfaces.Browser;
using OPLAPI.OIEL.UserElementsControl;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media;

namespace OPLAPI.OIEL.CORE.Browser
{
    /// <summary>
    /// Класс отображаемой главной страницы в браузере
    /// </summary>
    public abstract class MainPageBrowser : PageBrowser, IMainPageBrowser
    {
        #region Data
        /// <summary>
        /// Массив всех страничных приложений подключённых к начальной странице
        /// </summary>
        private List<AppPage> SourceAppPages = [];

        /// <summary>
        /// Массив всех страничных приложений доступный только для чтения
        /// </summary>
        ReadOnlyCollection<AppPage> IMainPageBrowser.AppPages => SourceAppPages.AsReadOnly();

        /// <summary>
        /// Иконка по умолчанию для страничных приложений
        /// </summary>
        protected ImageSource? DefaultIconAppPage { get; set; }
        #endregion

        #region ElementsApp
        /// <summary>
        /// Контейнер визуализации элементов страничных приложений
        /// </summary>
        public readonly WrapPanel MainPanelAllApplicationPages;

        /// <summary>
        /// Размер иконок страничных приложений
        /// </summary>
        public readonly System.Windows.Size ConstSizeIconsAppPages;
        #endregion

        /// <summary>
        /// Инициализировать базовый класс главной страницы браузера
        /// </summary>
        /// <param name="SizeIcons">Размер иконок в главной странице браузера</param>
        protected MainPageBrowser(System.Windows.Size SizeIcons) : base()
        {
            SourceAppPages = [];
            ConstSizeIconsAppPages = SizeIcons;
            MainPanelAllApplicationPages = new()
            {
                //ItemWidth = ConstSizeIconsAppPages.Width,
                //ItemHeight = ConstSizeIconsAppPages.Height,
            };
        }

        #region AppPageControl
        /// <summary>
        /// Добавить отображение иконки в менеджере приложений страниц
        /// </summary>
        /// <param name="TypeAppPage">Тип создаваемого приложения страницы</param>
        /// <param name="NameAppPage">Отображаемое имя</param>
        public virtual AppPage AddNewAppPage(Type TypeAppPage)
        {
            AppPage Source = new(TypeAppPage);
            if (DefaultIconAppPage != null && Source.VisualELement.Source == null)
                Source.VisualELement.Source = DefaultIconAppPage;
            SourceAppPages.Add(Source);
            SetVisualInit(Source.VisualELement);
            return Source;
        }

        /// <summary>
        /// Добавить отображение иконки в менеджере приложений страниц
        /// </summary>
        /// <param name="Path">Директория к установочному файлу страничного приложения</param>
        public InstallableAppPage AddNewAppPage(string Path)
        {
            InstallableAppPage Source = new(Path);
            if (DefaultIconAppPage != null && Source.VisualELement.Source == null)
                Source.VisualELement.Source = DefaultIconAppPage;
            SourceAppPages.Add(Source);
            SetVisualInit(Source.VisualELement);
            return Source;
        }

        /// <summary>
        /// Установить начальные значения для визуального элемента страничного приложения
        /// </summary>
        /// <param name="VisualAppPage">Визуальный элемент страничного приложения</param>
        private void SetVisualInit(OPLVisualElementIM VisualAppPage)
        {
            VisualAppPage.ManagerAnimation = ManagerAnimation;
            VisualAppPage.Width = ConstSizeIconsAppPages.Width;
            VisualAppPage.Height = ConstSizeIconsAppPages.Height;
            MainPanelAllApplicationPages.Children.Add(VisualAppPage);
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, VisualAppPage, OpacityProperty,
                1d, TimeSpan.FromMilliseconds(500d));
        }

        /// <summary>
        /// Инициализировать страницу по хранимому типу в иконке
        /// </summary>
        /// <param name="AppPage">Объект страничного приложения</param>
        internal static PageBrowser InitPageBrowserFromType<T>(in T AppPage) where T : AppPage
        {
            PageBrowser ElementAppPage = AppPage.InicializeAppPage();
            ElementAppPage.Title = AppPage.TitlePage;
            return ElementAppPage;
        }
        #endregion
    }
}
