using IEL.UserElementsControl.Base;
using OPLAPI.CORE.Animation;
using OPLAPI.CORE.Interfaces;
using System.Windows;
using System.Windows.Media;

namespace OPLAPI.OIEL.UserElementsControl
{
    /// <summary>
    /// Логика взаимодействия для OPLCheckBox.xaml
    /// </summary>
    public partial class OPLCheckBox : IELContainerBase, IOPLAnimate
    {
        #region Properties

        #region CheckBoxBorderThickness
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty CheckBoxBorderThicknessProperty =
            DependencyProperty.Register("CheckBoxBorderThickness", typeof(Thickness), typeof(OPLCheckBox),
                new(new Thickness(2),
                    (sender, e) =>
                    {
                        ((OPLCheckBox)sender).BorderCheck.BorderThickness = (Thickness)e.NewValue;
                    }));

        /// <summary>
        /// Толщина границ контейнера индикатора объекта
        /// </summary>
        public Thickness CheckBoxBorderThickness
        {
            get => (Thickness)GetValue(CheckBoxBorderThicknessProperty);
            set => SetValue(CheckBoxBorderThicknessProperty, value);
        }
        #endregion

        #region CheckBoxCornerRadius
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty CheckBoxCornerRadiusProperty =
            DependencyProperty.Register("CheckBoxCornerRadius", typeof(double), typeof(OPLCheckBox),
                new(0d, SetProperty_CheckBoxCornerRadius));

        /// <summary>
        /// Скругление границ контейнера индикатора объекта
        /// </summary>
        public double CheckBoxCornerRadius
        {
            get => (double)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        /// <summary>
        /// Установить элементу свойство
        /// </summary>
        /// <param name="Element">Объект которому устанавливается свойство</param>
        /// <param name="e">Данные о устанавливаемом свойстве</param>
        private static void SetProperty_CheckBoxCornerRadius(DependencyObject Element, DependencyPropertyChangedEventArgs e)
        {
            OPLCheckBox Source = (OPLCheckBox)Element;
            double SourceNewValue = (double)e.NewValue;
            if (SourceNewValue - 2 >= 0)
            {
                Source.RectangleCheck.RadiusX = SourceNewValue - 2;
                Source.RectangleCheck.RadiusY = SourceNewValue - 2;
            }
            CornerRadius NewValue = new(SourceNewValue);
            Source.BorderCheck.CornerRadius = NewValue;
        }
        #endregion

        #region Text
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(OPLCheckBox),
                new(string.Empty, SetProperty_Text));

        /// <summary>
        /// Отображаемый текст в поле
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        /// <summary>
        /// Установить элементу свойство
        /// </summary>
        /// <param name="Element">Объект которому устанавливается свойство</param>
        /// <param name="e">Данные о устанавливаемом свойстве</param>
        private static void SetProperty_Text(DependencyObject Element, DependencyPropertyChangedEventArgs e)
        {
            OPLCheckBox Source = (OPLCheckBox)Element;
            string SourceNewValue = (string)e.NewValue;
            Source.TextBlockElement.Text = SourceNewValue;
        }
        #endregion

        #region ImageOpacityTexture
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty ImageOpacityTextureProperty =
            DependencyProperty.Register("ImageOpacityTexture", typeof(ImageSource), typeof(OPLCheckBox),
                new(null, SetProperty_ImageOpacityTexture));

        /// <summary>
        /// Текстура изображения используемая для индикатора выделения
        /// </summary>
        public ImageSource ImageOpacityTexture
        {
            get => (ImageSource)GetValue(ImageOpacityTextureProperty);
            set => SetValue(ImageOpacityTextureProperty, value);
        }

        /// <summary>
        /// Установить элементу свойство
        /// </summary>
        /// <param name="Element">Объект которому устанавливается свойство</param>
        /// <param name="e">Данные о устанавливаемом свойстве</param>
        private static void SetProperty_ImageOpacityTexture(DependencyObject Element, DependencyPropertyChangedEventArgs e)
        {
            OPLCheckBox Source = (OPLCheckBox)Element;
            ImageSource? SourceNewValue = (ImageSource?)e.NewValue;
            if (SourceNewValue == null) Source.RectangleCheck.OpacityMask = null;
            else if (Source.RectangleCheck.OpacityMask == null)
                Source.RectangleCheck.OpacityMask = new ImageBrush(SourceNewValue);
            else
                ((ImageBrush)Source.RectangleCheck.OpacityMask).ImageSource = SourceNewValue;
        }
        #endregion

        #region IsChecked
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(OPLCheckBox),
                new(false, SetProperty_IsChecked));

        /// <summary>
        /// Состояние выделения
        /// </summary>
        public bool IsChecked
        {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        /// <summary>
        /// Установить элементу свойство
        /// </summary>
        /// <param name="Element">Объект которому устанавливается свойство</param>
        /// <param name="e">Данные о устанавливаемом свойстве</param>
        private static void SetProperty_IsChecked(DependencyObject Element, DependencyPropertyChangedEventArgs e)
        {
            OPLCheckBox Source = (OPLCheckBox)Element;
            bool SourceNewValue = (bool)e.NewValue;
            Source.IsCheckedChanged.Invoke(Source, SourceNewValue);
        }

        /// <summary>
        /// Событие изменения состояния выделения
        /// </summary>
        public event EventHandler<bool> IsCheckedChanged;
        #endregion

        #endregion

        /// <summary>
        /// Объект менеджера анимаций настроек OPL
        /// </summary>
        public OPLAnimationManager? ManagerAnimation { get; set; }

        public OPLCheckBox()
        {
            InitializeComponent();
            RectangleCheck.Opacity = 0d;
            RectangleCheck.Margin = new(2d);
            RectangleCheck.RadiusX = 0d;
            RectangleCheck.RadiusY = 0d;
            BorderCheck.BorderBrush = SourceBorderBrush.SourceBrush;
            RectangleCheck.Fill = SourceForeground.SourceBrush;
            TextBlockElement.Text = string.Empty;
            TextBlockElement.Foreground = SourceForeground.SourceBrush;

            MouseDown += (sender, e) =>
            {
                SetActiveSpecrum(IEL.CORE.Enums.StateSpectrum.Used);
            };

            MouseUp += (sender, e) =>
            {
                SetActiveSpecrum(IEL.CORE.Enums.StateSpectrum.Select);
            };

            MouseLeftButtonUp += (sender, e) => IsChecked = !IsChecked;

            IsCheckedChanged += OPLCheckBox_IsCheckedChanged;
        }

        /// <summary>
        /// Активировать состояние активации
        /// </summary>
        /// <param name="sender">Объект вызвавший событие</param>
        /// <param name="e">Устанавливаемое состояние активации</param>
        private void OPLCheckBox_IsCheckedChanged(object? sender, bool e)
        {
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, RectangleCheck, OpacityProperty,
                e ? 1d : 0d, TimeSpan.FromMilliseconds(400d));
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, RectangleCheck, MarginProperty,
                new Thickness(e ? 0d : 2d), TimeSpan.FromMilliseconds(400d));
        }
    }
}
