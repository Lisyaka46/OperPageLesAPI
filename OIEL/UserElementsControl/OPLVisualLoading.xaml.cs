using OPLAPI.CORE.Animation;
using OPLAPI.CORE.Interfaces;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace OPLAPI.OIEL.UserElementsControl
{
    /// <summary>
    /// Логика взаимодействия для OPLVisualLoading.xaml
    /// </summary>
    public partial class OPLVisualLoading : System.Windows.Controls.UserControl, IOPLAnimate
    {
        private OPLAnimationManager? SourceManagerAnimation;
        /// <summary>
        /// Объект менеджера анимаций настроек OPL
        /// </summary>
        public OPLAnimationManager? ManagerAnimation
        {
            get => SourceManagerAnimation;
            set
            {
                if (value == null) Animation = null;
                else
                {
                    Animation = new()
                    {
                        From = 0d,
                        To = -7.44d,
                        EasingFunction = null,
                        Duration = TimeSpan.FromMilliseconds(800d),
                        FillBehavior = FillBehavior.HoldEnd,
                        RepeatBehavior = RepeatBehavior.Forever,
                    };
                }
                SourceManagerAnimation = value;
            }
        }

        /// <summary>
        /// Объект анимации управляемый отображением циклом загрузки
        /// </summary>
        private DoubleAnimation? Animation;

        #region Properties

        #region BorderBrush
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly new DependencyProperty BorderBrushProperty =
            DependencyProperty.Register("BorderBrush", typeof(SolidColorBrush), typeof(OPLVisualLoading),
                new(new SolidColorBrush(Colors.Black),
                    (sender, e) =>
                    {
                        ((OPLVisualLoading)sender).ElementLoading.Stroke = (SolidColorBrush)e.NewValue;
                    }));

        /// <summary>
        /// Цвет барьера отображения загрузки
        /// </summary>
        public new SolidColorBrush BorderBrush
        {
            get => (SolidColorBrush)GetValue(BorderBrushProperty);
            set => SetValue(BorderBrushProperty, value);
        }
        #endregion

        #region Opacity
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly new DependencyProperty OpacityProperty =
            DependencyProperty.Register("Opacity", typeof(double), typeof(OPLVisualLoading),
                new(1d,
                    (sender, e) =>
                    {
                        ((OPLVisualLoading)sender).ElementLoading.Opacity = (double)e.NewValue;
                    }));

        /// <summary>
        /// Прозрачность отображения загрузки
        /// </summary>
        public new double Opacity
        {
            get => (double)GetValue(OPLVisualLoading.OpacityProperty);
            set => SetValue(OPLVisualLoading.OpacityProperty, value);
        }
        #endregion

        #endregion

        public OPLVisualLoading()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Начать отображение загрузки
        /// </summary>
        public void OpenLoading()
        {
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, this, OPLVisualLoading.OpacityProperty,
                1d, TimeSpan.FromMilliseconds(600d));
            if (ManagerAnimation != null)
                ElementLoading.BeginAnimation(Ellipse.StrokeDashOffsetProperty, Animation);

        }

        /// <summary>
        /// Закончить отображение загрузки
        /// </summary>
        public void CloseLoading()
        {
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, this, OPLVisualLoading.OpacityProperty,
                0d, TimeSpan.FromMilliseconds(600d));
            ElementLoading.BeginAnimation(Ellipse.StrokeDashOffsetProperty, null);
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, ElementLoading, Ellipse.StrokeDashOffsetProperty,
                0d, TimeSpan.FromMilliseconds(570d));
        }
    }
}
