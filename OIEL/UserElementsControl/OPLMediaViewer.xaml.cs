using IEL.UserElementsControl.Base;
using OPLAPI.OIEL.UserElementsControl.Interfaces;
using System.Windows;

namespace OPLAPI.OIEL.UserElementsControl
{
    /// <summary>
    /// Логика взаимодействия для OPLViewerLoadingProcess.xaml
    /// </summary>
    public partial class OPLMediaViewer : IELContainerBase, IOPLObjectViewer<Uri>
    {
        #region Parameters

        #region Source
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty SourceElementProperty =
            DependencyProperty.Register("SourceElement", typeof(Uri), typeof(OPLMediaViewer),
                new(
                    (sender, e) =>
                    {
                        ((OPLMediaViewer)sender).IndicatorMedia.Source = (Uri)e.NewValue;
                    }));

        /// <summary>
        /// Данные пути к медиа загрузки объекта
        /// </summary>
        public Uri SourceElement
        {
            get => (Uri)GetValue(SourceElementProperty);
            set => SetValue(SourceElementProperty, value);
        }
        #endregion

        #endregion

        public OPLMediaViewer()
        {
            InitializeComponent();

            IndicatorMedia.MediaEnded += (sender, e) =>
            {
                IndicatorMedia.Position = TimeSpan.FromMilliseconds(1);
            };
            IndicatorMedia.Opacity = 0d;
        }
    }
}
