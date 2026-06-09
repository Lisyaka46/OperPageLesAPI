using OPLAnimation.CORE.Animation;
using OPLAnimation.CORE.Interfaces;
using OPLAPI.OIEL.CORE.Interfaces.Browser;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace OPLAPI.OIEL.CORE.Browser
{
    public class MainPageBrowser : PageBrowser, IMainPageBrowser, IOPLAnimate
    {
        #region Data
        /// <summary>
        /// Объект менеджера анимаций настроек OPL
        /// </summary>
        public OPLAnimationManager? ManagerAnimation { get; set; }

        /// <summary>
        /// Массив всех страничных приложений подключённых к начальной странице
        /// </summary>
        private List<ApplicationPage> SourceAppPages;

        /// <summary>
        /// Массив всех страничных приложений доступный только для чтения
        /// </summary>
        internal ReadOnlyCollection<ApplicationPage> AppPages => SourceAppPages.AsReadOnly();
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

        public MainPageBrowser(System.Windows.Size SizeIcons) : base()
        {
            SourceAppPages = [];
            ConstSizeIconsAppPages = SizeIcons;
            MainPanelAllApplicationPages = new()
            {
                //ItemWidth = ConstSizeIconsAppPages.Width,
                //ItemHeight = ConstSizeIconsAppPages.Height,
            };
        }

        /// <summary>
        /// Добавить отображение иконки в менеджере приложений страниц
        /// </summary>
        /// <param name="TypeAppPage">Тип создаваемого приложения страницы</param>
        /// <param name="NameAppPage">Отображаемое имя</param>
        public virtual ApplicationPage AddNewAppPage(Type TypeAppPage, string NameAppPage)
        {
            ApplicationPage Source = new(TypeAppPage, NameAppPage);
            SourceAppPages.Add(Source);
            Source.VisualELement.ManagerAnimation = ManagerAnimation;
            Source.VisualELement.Width = ConstSizeIconsAppPages.Width;
            Source.VisualELement.Height = ConstSizeIconsAppPages.Height;
            MainPanelAllApplicationPages.Children.Add(Source.VisualELement);
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, Source.VisualELement, OpacityProperty,
                1d, TimeSpan.FromMilliseconds(500d));
            return Source;
        }

        /// <summary>
        /// Инициализировать страницу по хранимому типу в иконке
        /// </summary>
        /// <param name="AppPage">Объект страничного приложения</param>
        internal PageBrowser InitPageBrowserFromType(in ApplicationPage AppPage)
        {
            PageBrowser ElementAppPage = (PageBrowser)(Activator.CreateInstance(AppPage.TypeBrowserAppPage) ??
                throw new Exception("Не удалось создать объект приложения страницы"));
            ElementAppPage.Title = AppPage.Name;
            return ElementAppPage;
            //IELButtonImage CloseButtonInlay = App.CurrentApp.MainBrowser.AddInlayPage(in ElementAppPage, AppPage.VisualELement.PaletteElement, true).GetButtonCloseInlay();
            //CloseButtonInlay.MarginViewBox = new(0);
            //CloseButtonInlay.PaletteElement = App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.Red];
            //CloseButtonInlay.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Cross));
        }
    }
}
