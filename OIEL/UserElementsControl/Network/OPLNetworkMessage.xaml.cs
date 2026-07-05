using OPLAPI.CORE.Animation;
using OPLAPI.OIEL.CORE.Network;
using OPLAPI.OIEL.UserElementsControl.Base;
using System.Windows;
using System.Windows.Controls;

namespace OPLAPI.OIEL.UserElementsControl.Network
{
    /// <summary>
    /// Класс объекта отображающего сообщение в сети
    /// </summary>
    public partial class OPLNetworkMessage : OPLNetworkElementViewerBase
    {
        #region Properties

        #region Message
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(OPLNetworkMessage),
                new(string.Empty,
                    (sender, e) =>
                    {
                        ((OPLNetworkMessage)sender).TextBlockMessage.Text = (string)e.NewValue;
                    }));

        /// <summary>
        /// Сообщение хранящееся в объекте
        /// </summary>
        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }
        #endregion

        #region BorderThicknessClipContent
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty BorderThicknessClipContentProperty =
            DependencyProperty.Register("BorderThicknessClipContentProperty", typeof(Thickness), typeof(OPLNetworkMessage),
                new(new Thickness(0),
                    (sender, e) =>
                    {
                        ((OPLNetworkMessage)sender).BorderClipElements.BorderThickness = (Thickness)e.NewValue;
                    }));

        /// <summary>
        /// Толщина границы прикреплённого контента к сообщению
        /// </summary>
        public int BorderThicknessClipContent
        {
            get => (int)GetValue(BorderThicknessClipContentProperty);
            set
            {
                SetValue(BorderThicknessClipContentProperty, value);
            }
        }
        #endregion

        #endregion

        private StackPanel? _StackPanelClip = null;
        /// <summary>
        /// Панель отображающая прикреплённые элементы к сообщению
        /// </summary>
        public StackPanel StackPanelClip => _StackPanelClip ??= new();

        /// <summary>
        /// Инициализировать объект отображения сообщения в сети
        /// </summary>
        public OPLNetworkMessage()
        {
            InitializeComponent();
            BorderClipElements.BorderThickness = new(0);
            TextBlockMessage.Text = string.Empty;
            BorderClipElements.Child = StackPanelClip;
        }

        /// <summary>
        /// Установить визуализацию объекта сообветственно данным
        /// </summary>
        /// <param name="NetworkInfo">Данные о передаваемых данных</param>
        /// <param name="SourceClipCollection">Массив прикреплённых элементов</param>
        public void SetVisualFromNetworkInfo(DataNetworkInfo NetworkInfo, UIElementCollection? SourceClipCollection = null)
        {
            //PaletteElement = App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.LightBlue];
            if (NetworkInfo.LengthMessage > 0)
            {
                TextBlockMessage.Height = double.NaN;
                TextBlockMessage.Margin = new(5);
            }
            else
            {
                TextBlockMessage.Height = 0d;
                TextBlockMessage.Margin = new(0);
                TextBlockMessage.Text = string.Empty;
            }
            if (NetworkInfo.FilesInfo.Count > 0)
            {
                OPLVisualNetworkClipFile Element;
                StackPanelClip.Children.Clear();
                for (int i = 0; i < NetworkInfo.FilesInfo.Count; i++)
                {
                    if (SourceClipCollection != null)
                    {
                        Element = (OPLVisualNetworkClipFile)SourceClipCollection[0];
                        SourceClipCollection.RemoveAt(0);
                        Element.Margin = new(0);
                        Element.Opacity = 0d;
                    }
                    else
                    {
                        Element = new()
                        {
                            Opacity = 0d,
                            CornerRadius = new(5),
                            Margin = new(0),
                            TextFileName = $"{NetworkInfo.FilesInfo[i].FileName}.{NetworkInfo.FilesInfo[i].FileExtension}",
                            ManagerAnimation = ManagerAnimation,
                        };
                        Element.MathSizeFile(NetworkInfo.FilesInfo[i].LengthFileData);
                    }
                    StackPanelClip.Children.Add(Element);
                    OPLAnimationManager.AnimateTakingZeroFromTo(ManagerAnimation, Element, OpacityProperty,
                        0d, 1d, TimeSpan.FromMilliseconds(500d));
                    OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, Element, MarginProperty,
                        new Thickness(3), TimeSpan.FromMilliseconds(500d));
                }
            }
        }

        /// <summary>
        /// Прикрепить объект к сообщению
        /// </summary>
        /// <param name="Source">Прикрепляемый элемент к сообщению</param>
        /// <returns></returns>
        public void ClipObjectFromMessage(ref OPLVisualNetworkClipFile Source)
        {
            StackPanelClip.Children.Add(Source);
        }
    }
}
