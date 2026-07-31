using OPLAPI.CORE.Animation;
using OPLAPI.CORE.Interfaces;
using OPLAPI.OIEL.CORE.Browser.Base;
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
        private List<AppPageBase> SourceAppPages = [];

        /// <summary>
        /// Массив всех страничных приложений доступный только для чтения
        /// </summary>
        ReadOnlyCollection<AppPageBase> IMainPageBrowser.AppPages => SourceAppPages.AsReadOnly();

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
        /// Событие активации страничного приложения
        /// </summary>
        internal event EventHandler<AppPageBase>? ApplicationPageActivated;

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
        internal AppPage AddNewAppPage(Type TypeAppPage)
        {
            AppPage Source = new(TypeAppPage);
            if (DefaultIconAppPage != null && Source.VisualELement.Source == null)
                Source.VisualELement.Source = DefaultIconAppPage;
            SourceAppPages.Add(Source);
            SetVisualInit(Source.VisualELement);
            Source.ApplicationPageClick += ApplicationPageClickHandler;
            return Source;
        }

        /// <summary>
        /// Добавить отображение иконки в менеджере приложений страниц
        /// </summary>
        /// <param name="Path">Директория к установочному файлу страничного приложения</param>
        internal async Task<InstallableAppPage> AddNewAppPage(string Path)
        {
            InstallableAppPage Source = await InstallableAppPage.GetInstallableAppPage(Path);
            if (DefaultIconAppPage != null && Source.VisualELement.Source == null)
                Source.VisualELement.Source = DefaultIconAppPage;
            SourceAppPages.Add(Source);
            SetVisualInit(Source.VisualELement);
            Source.ApplicationPageClick += ApplicationPageClickHandler;
            return Source;
        }

        /// <summary>
        /// Установить начальные значения для визуального элемента страничного приложения
        /// </summary>
        /// <param name="VisualAppPage">Визуальный элемент страничного приложения</param>
        internal void SetVisualInit(OPLVisualElementIM VisualAppPage)
        {
            VisualAppPage.ManagerAnimation = ManagerAnimation;
            VisualAppPage.Width = ConstSizeIconsAppPages.Width;
            VisualAppPage.Height = ConstSizeIconsAppPages.Height;
            OPLAnimationManager.AnimateTakingZeroFromTo(ManagerAnimation, VisualAppPage, OpacityProperty,
                0d, 1d, TimeSpan.FromMilliseconds(500d));
            MainPanelAllApplicationPages.Children.Add(VisualAppPage);
        }
        #endregion
        
        /// <summary>
        /// Обработчик события нажатия на страничное приложение
        /// </summary>
        private void ApplicationPageClickHandler(object? sender, AppPageBase e)
        {
            ApplicationPageActivated?.Invoke(this, e);
        }
    }
}
