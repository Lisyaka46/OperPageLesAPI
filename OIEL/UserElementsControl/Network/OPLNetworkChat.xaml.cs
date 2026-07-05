using OPLAPI.CORE.Animation;
using OPLAPI.CORE.Interfaces;
using OPLAPI.OIEL.UserElementsControl.Base;
using System.Windows;
using System.Windows.Media;

namespace OPLAPI.OIEL.UserElementsControl.Network
{
    /// <summary>
    /// Логика взаимодействия для OPLNetworkChat.xaml
    /// </summary>
    public partial class OPLNetworkChat : OPLNetworkElementViewerBase
    {

        #region Properties

        #region EndMessage
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty EndMessageProperty =
            DependencyProperty.Register("EndMessage", typeof(string), typeof(OPLNetworkChat),
                new(string.Empty,
                    (sender, e) =>
                    {
                        ((OPLNetworkChat)sender).TextBlockEndMessage.Text = (string)e.NewValue;
                    }));

        /// <summary>
        /// Отображаемое последнее сообщение
        /// </summary>
        public string EndMessage
        {
            get => (string)GetValue(EndMessageProperty);
            set => SetValue(EndMessageProperty, value);
        }
        #endregion

        #region Icon
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(OPLNetworkChat),
                new(null,
                    (sender, e) =>
                    {
                        
                        ((OPLNetworkChat)sender).ImageIconMessage.Source = (ImageSource)e.NewValue;
                    }));

        /// <summary>
        /// Отображаемое иконка сообщения
        /// </summary>
        public ImageSource? Icon
        {
            get => (ImageSource?)GetValue(IconProperty);
            set
            {
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, ImageIconMessage, OpacityProperty,
                    value == null ? 0d : 1d, TimeSpan.FromMilliseconds(400d));
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, ImageIconMessage, MarginProperty,
                    value == null ? new Thickness(0d) : new(1d, 0d, 4d, 0d), TimeSpan.FromMilliseconds(400d));
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, ImageIconMessage, OpacityProperty,
                    value == null ? 0d : TextBlockEndMessage.ActualHeight, TimeSpan.FromMilliseconds(400d));
                SetValue(IconProperty, value);
            }
        }
        #endregion

        #region TextCount
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty TextCountProperty =
            DependencyProperty.Register("TextCount", typeof(int), typeof(OPLNetworkChat),
                new(0,
                    (sender, e) =>
                    {
                    }));

        /// <summary>
        /// Отображаемое иконка сообщения
        /// </summary>
        public int TextCount
        {
            get => (int)GetValue(TextCountProperty);
            set
            {
                TextBlockCountElements.Text = value > 0 ? value.ToString() : string.Empty;
                SetValue(TextCountProperty, value);
            }
        }
        #endregion

        #region TextHead
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty TextHeadProperty =
            DependencyProperty.Register("TextHead", typeof(string), typeof(OPLNetworkChat),
                new(string.Empty,
                    (sender, e) =>
                    {
                        ((OPLNetworkChat)sender).TextBlockHead.Text = (string)e.NewValue;
                    }));

        /// <summary>
        /// Отображаемый наглавный текст
        /// </summary>
        public string TextHead
        {
            get => (string)GetValue(TextHeadProperty);
            set => SetValue(TextHeadProperty, value);
        }
        #endregion

        #endregion

        public OPLNetworkChat()
        {
            InitializeComponent();
            TextBlockHead.Text = string.Empty;
            TextBlockEndMessage.Text = string.Empty;
            ImageIconMessage.Width = 0d;
            ImageIconMessage.Opacity = 0d;
            TextBlockCountElements.Text = string.Empty;

            TextBlockHead.Foreground = SourceForeground.SourceBrush;
            TextBlockEndMessage.Foreground = SourceForeground.SourceBrush;
            TextBlockCountElements.Foreground = SourceForeground.SourceBrush;
        }
    }
}
