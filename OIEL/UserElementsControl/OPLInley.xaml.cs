using IEL.UserElementsControl;
using IEL.UserElementsControl.Base;
using LibraryIEL.CORE.Themes.Palettes;
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
        public event EventHandler<OPLInlay>? CloseInlay;

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

        #region Title
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(OPLInlay),
                new(string.Empty,
                    (sender, e) =>
                    {
                        ((OPLInlay)sender).TextBlockHead.Text = (string)e.NewValue;
                    }));

        /// <summary>
        /// Текст отображаемый в элементе вкладки
        /// </summary>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
        #endregion

        #region CloseButtonPalette
        /// <summary>
        /// Данные спектра использования цветов кнопки закрытия вкладки
        /// </summary>
        public PaletteData CloseButtonPalette
        {
            get => IELButtonCloseInlay.Palette;
            set => IELButtonCloseInlay.Palette = value;
        }
        #endregion

        #region CloseButtonSource
        /// <summary>
        /// Ссылка на элемент изображения кнопки закрытия вкладки
        /// </summary>
        public ImageSource CloseButtonSource
        {
            get => IELButtonCloseInlay.Source;
            set => IELButtonCloseInlay.Source = value;
        }
        #endregion

        #endregion

        /// <summary>
        /// Инициализировать объект интерфейса, вкладка браузера
        /// </summary>
        public OPLInlay()
        {
            InitializeComponent();
            TextBlockHead.Foreground = SourceForeground.SourceBrush;

            IELButtonCloseInlay.OnActivateMouseLeft += (sender, e) =>
            {
                CloseInlay?.Invoke(sender, this);
            };
        }
    }
}
