using IEL.UserElementsControl;
using IEL.UserElementsControl.Base;
using OPLAPI.OIEL.CORE.Browser;
using System.Windows;
using System.Windows.Media;

namespace OPLAPI.OIEL.UserElementsControl
{
    /// <summary>
    /// Логика взаимодействия для OPLInley.xaml
    /// </summary>
    public partial class OPLInlay : IELContainerBase
    {
        /// <summary>
        /// Объект события активации закрытия вкладки
        /// </summary>
        public event EventHandler<OPLInlay> OnActivateCloseInlay = null!;

        #region Properties

        #region IsEnabled
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly new DependencyProperty IsEnabledProperty =
            DependencyProperty.Register("IsEnabled", typeof(bool), typeof(OPLInlay),
                new(false,
                    (sender, e) =>
                    {
                        ((IELButtonBase)sender).IsEnabled = (bool)e.NewValue;
                    }));

        /// <summary>
        /// Состояние включения элемента вкладки
        /// </summary>
        public new bool IsEnabled
        {
            get => (bool)base.GetValue(IsEnabledProperty);
            set
            {
                if (Content == null && value) throw new Exception("Невозможно включить элемент не имея отображаемой страницы.");
                base.SetValue(IELContainerBase.IsEnabledProperty, value);
            }
        }
        #endregion

        #region Padding
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly new DependencyProperty PaddingProperty =
            DependencyProperty.Register("Padding", typeof(Thickness), typeof(OPLInlay),
                new(new Thickness(0),
                    (sender, e) =>
                    {
                        ((OPLInlay)sender).MainGridElement.Margin = (Thickness)e.NewValue;
                    }));

        /// <summary>
        /// Внутреннее смещение в объекте
        /// </summary>
        public new Thickness Padding
        {
            get => (Thickness)GetValue(PaddingProperty);
            set => SetValue(PaddingProperty, value);
        }
        #endregion

        #region FontFamily
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly new DependencyProperty FontFamilyProperty =
            DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(OPLInlay),
                new(
                    (sender, e) =>
                    {
                        ((OPLInlay)sender).TextBlockHead.FontFamily = (FontFamily)e.NewValue;
                    }));

        /// <summary>
        /// Шрифт текста элемента
        /// </summary>
        public new FontFamily FontFamily
        {
            get => (FontFamily)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }
        #endregion

        #region Text
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(OPLInlay),
                new(string.Empty,
                    (sender, e) =>
                    {
                        ((OPLInlay)sender).TextBlockHead.Text = (string)e.NewValue;
                    }));

        /// <summary>
        /// Текст отображаемый в элементе вкладки
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        #endregion

        #endregion

        /// <summary>
        /// Страница которую содержит вкладка
        /// </summary>
        public readonly new PageBrowser Content;

        /// <summary>
        /// Инициализировать объект интерфейса, вкладка браузера
        /// </summary>
        /// <param name="AppPageContent">Все компоненты которые находятся в данном приложении странице</param>
        public OPLInlay(in PageBrowser AppPageContent)
        {
            InitializeComponent();
            Content = AppPageContent;
            Text = AppPageContent.Title;

            TextBlockHead.Foreground = SourceForeground.SourceBrush;

            IELButtonCloseInlay.OnActivateMouseLeft += (sender, e) =>
            {
                OnActivateCloseInlay.Invoke(sender, this);
            };
        }

        /// <summary>
        /// Получить кнопку закрытия вкладки
        /// </summary>
        public IELButtonImage GetButtonCloseInlay() => IELButtonCloseInlay;
    }
}
