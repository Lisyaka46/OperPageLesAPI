using IEL.CORE.Classes;
using IEL.UserElementsControl.Base;
using OPLAPI.CORE.Animation;
using OPLAPI.CORE.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace OPLAPI.OIEL.UserElementsControl
{
    /// <summary>
    /// Пересичление ориентации отображения наименования
    /// </summary>
    public enum OrientationName
    {
        /// <summary>
        /// Снизу
        /// </summary>
        Down = 0,

        /// <summary>
        /// Сверху
        /// </summary>
        Up = 1,
    }

    /// <summary>
    /// Логика взаимодействия для OPLVisualElementIM.xaml
    /// </summary>
    public partial class OPLVisualElementIM : UserControl, IOPLAnimate
    {
        #region Properties

        #region CornerRadius
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(OPLVisualElementIM),
                new(new CornerRadius(0),
                    (sender, e) =>
                    {
                        ((OPLVisualElementIM)sender).ButtonApplication.CornerRadius = (CornerRadius)e.NewValue;
                    }));

        /// <summary>
        /// Скругление границ объекта
        /// </summary>
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
        #endregion

        #region BorderThickness
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly new DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(OPLVisualElementIM),
                new(new Thickness(2),
                    (sender, e) =>
                    {
                        ((OPLVisualElementIM)sender).ButtonApplication.BorderThickness = (Thickness)e.NewValue;
                    }));

        /// <summary>
        /// Толщина границ объекта
        /// </summary>
        public new Thickness BorderThickness
        {
            get => (Thickness)ButtonApplication.GetValue(BorderThicknessProperty);
            set => ButtonApplication.SetValue(BorderThicknessProperty, value);
        }
        #endregion

        #region Padding
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly new DependencyProperty PaddingProperty =
            DependencyProperty.Register("Padding", typeof(Thickness), typeof(OPLVisualElementIM),
                new(new Thickness(0),
                    (sender, e) =>
                    {
                        ((OPLVisualElementIM)sender).ButtonApplication.Padding = (Thickness)e.NewValue;
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

        #region Source
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(ImageSource), typeof(OPLVisualElementIM),
                new(null,
                    (sender, e) =>
                    {
                        ((OPLVisualElementIM)sender).ButtonApplication.Source = (ImageSource)e.NewValue;
                    }));

        /// <summary>
        /// Ссылка на элемент изображения
        /// </summary>
        public ImageSource Source
        {
            get => (ImageSource)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }
        #endregion

        #region PaletteElement
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty PaletteElementProperty =
            DependencyProperty.Register("PaletteElement", typeof(PaletteSpectrum), typeof(OPLVisualElementIM),
                new(
                    (sender, e) =>
                    {
                        ((OPLVisualElementIM)sender).ButtonApplication.PaletteElement = (PaletteSpectrum)e.NewValue;
                    }));

        /// <summary>
        /// Объект палитры
        /// </summary>
        public PaletteSpectrum PaletteElement
        {
            get => ButtonApplication.PaletteElement;
            set
            {
                SetValue(PaletteElementProperty, value);
            }
        }
        #endregion

        #region Text
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(OPLVisualElementIM),
                new(null,
                    (sender, e) =>
                    {
                        ((OPLVisualElementIM)sender).TextBlockNameApplication.Text = (string)e.NewValue;
                    }));

        /// <summary>
        /// Строка имени
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set
            {
                SetValue(TextProperty, value);
                TextBlockNameApplication.UpdateLayout();
                ViewBoxName.Margin = VisualOrientationName == OrientationName.Down ?
                        new(0d, 0d, 0d, -BorderName.ActualHeight + 3d) : new(0d, -BorderName.ActualHeight + 3d, 0, 0);
            }
        }
        #endregion

        #region VisualOrientationName
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty VisualOrientationNameProperty =
            DependencyProperty.Register("VisualOrientationName", typeof(OrientationName), typeof(OPLVisualElementIM),
                new(OrientationName.Down));

        /// <summary>
        /// Ориентация отображения имени
        /// </summary>
        public OrientationName VisualOrientationName
        {
            get => (OrientationName)GetValue(VisualOrientationNameProperty);
            set
            {
                ViewBoxName.VerticalAlignment = value == OrientationName.Down ? VerticalAlignment.Bottom : VerticalAlignment.Top;
                TextBlockNameApplication.UpdateLayout();
                ViewBoxName.Margin = VisualOrientationName == OrientationName.Down ?
                        new(0d, 0d, 0d, -BorderName.ActualHeight + 3d) : new(0d, -BorderName.ActualHeight + 3d, 0, 0);
                SetValue(VisualOrientationNameProperty, value);
            }
        }
        #endregion

        #endregion

        private OPLAnimationManager? SourceManagerAnimation;
        /// <summary>
        /// Объект менеджера анимационных настроек OPL
        /// </summary>
        public OPLAnimationManager? ManagerAnimation
        {
            get => SourceManagerAnimation;
            set
            {
                SourceManagerAnimation = value;
                VisualLoading.ManagerAnimation = SourceManagerAnimation;
            }
        }

        /// <summary>
        /// Объект события активации левым щелчком мыши
        /// </summary>
        public IELButtonBase.ActivateHandler? OnActivateMouseLeft { get; set; }

        /// <summary>
        /// Объект события активации правым щелчком мыши
        /// </summary>
        public IELButtonBase.ActivateHandler? OnActivateMouseRight { get; set; }

        //
        public BrushSettingQ SourceBackground => ButtonApplication.SourceBackground;

        //
        public BrushSettingQ SourceBorderBrush => ButtonApplication.SourceBorderBrush;

        //
        public BrushSettingQ SourceForeground => ButtonApplication.SourceForeground;

        public OPLVisualElementIM() 
        {
            InitializeComponent();
            BorderName.Background = ButtonApplication.SourceBackground.SourceBrush;
            BorderName.BorderBrush = ButtonApplication.SourceBorderBrush.SourceBrush;
            TextBlockNameApplication.Foreground = ButtonApplication.SourceForeground.SourceBrush;
            TextBlockNameApplication.Text = string.Empty;
            ViewBoxName.Opacity = 0d;
            ViewBoxName.Height = 0d;
            VisualLoading.Opacity = 0d;
            VisualLoading.BorderBrush = ButtonApplication.SourceBorderBrush.SourceBrush;

            Margin = new(0d, 5d, 0d, 5d);
            ButtonApplication.OnActivateMouseLeft += (sender, e) => OnActivateMouseLeft?.Invoke(this, e);
            ButtonApplication.OnActivateMouseRight += (sender, e) => OnActivateMouseRight?.Invoke(this, e);
            ButtonApplication.MouseEnter += (sender, e) =>
            {
                ViewBoxName.Height = double.NaN;
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation,
                    ViewBoxName, OpacityProperty, 1d, TimeSpan.FromMilliseconds(450d));
                OPLAnimationManager.AnimateTakingZeroFromTo(ManagerAnimation, ViewBoxName, MarginProperty,
                    VisualOrientationName == OrientationName.Down ?
                    new Thickness(0d, 0d, 0d, -BorderName.ActualHeight + 3d) : new(0d, -BorderName.ActualHeight + 3d, 0, 0),
                    VisualOrientationName == OrientationName.Down ?
                    new Thickness(0d, 0d, 0d, -BorderName.ActualHeight + 1d) : new(0d, -BorderName.ActualHeight + 1d, 0, 0),
                    TimeSpan.FromMilliseconds(450d));
            };
            ButtonApplication.MouseLeave += (seder, e) =>
            {
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, ViewBoxName, OpacityProperty,
                    0d, TimeSpan.FromMilliseconds(450d));
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, ViewBoxName, MarginProperty,
                    VisualOrientationName == OrientationName.Down ?
                    new Thickness(0d, 0d, 0d, -BorderName.ActualHeight + 3d) : new(0d, -BorderName.ActualHeight + 3d, 0, 0),
                    TimeSpan.FromMilliseconds(450d));
            };
            Initialized += (sender, e) =>
            {
                TextBlockNameApplication.UpdateLayout();
                ViewBoxName.Margin = VisualOrientationName == OrientationName.Down ?
                        new(0d, 0d, 0d, -BorderName.ActualHeight + 3d) : new(0d, -BorderName.ActualHeight + 3d, 0, 0);
            };
        }

        /// <summary>
        /// Активировать анимирование и отображение медиа
        /// </summary>
        public void ActivateVisualMedia()
        {
            VisualLoading.OpenLoading();
            //VisualLoad.Source = Media;
            //VisualLoad.IsEnabled = true;
            //VisualLoad.Position = TimeSpan.MinValue;
            //VisualLoad.Play();
            //if (ManagerAnimation != null)
            //    ManagerAnimation.DoubleAnimationType.AnimateEffect(VisualLoad, OpacityProperty, 1d, TimeSpan.FromMilliseconds(2000d));
            //else
            //    VisualLoad.Opacity = 1d;
        }

        /// <summary>
        /// Диактивировать анимирование и отображение медиа
        /// </summary>
        public void DiactivateVisualMedia()
        {
            //if (ManagerAnimation != null)
            //    ManagerAnimation.DoubleAnimationType.AnimateEffect(VisualLoad, OpacityProperty, 0d, TimeSpan.FromMilliseconds(500d));
            //else
            //    VisualLoad.Opacity = 0d;
            VisualLoading.CloseLoading();
            //Canvas.SetZIndex(VisualLoad, -1);
            //VisualLoad.IsEnabled = false;
        }

        /// <summary>
        /// Изменить изображение отображаемое в элементе
        /// </summary>
        /// <param name="Source">Устанавливаемое изображение</param>
        public void ChangeSourceImage(in ImageSource Source)
        {
            ButtonApplication.ChangeImageIcon(in Source, ManagerAnimation?.GetCloneAnimationElementFromType<DoubleAnimation>());
        }

        /// <summary>
        /// Задать размер визуальному элементу
        /// </summary>
        /// <param name="Source">Устанавливаемый размер</param>
        public void SetSizeIconApp(in Size Source)
        {
            ButtonApplication.Width = Source.Width;
            ButtonApplication.Height = Source.Height;
        }
    }
}
