using IEL.UserElementsControl.Base;
using OPLAPI.OIEL.UserElementsControl.Interfaces;
using System.Windows;
using System.Windows.Media;

namespace OPLAPI.OIEL.UserElementsControl
{
    /// <summary>
    /// Логика взаимодействия для OPLViewerLoadingProcess.xaml
    /// </summary>
    public partial class OPLImageViewer : IELContainerBase, IOPLObjectViewer<ImageSource>
    {
        #region Parameters

        #region SourceElement
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty SourceElementProperty =
            DependencyProperty.Register("SourceElement", typeof(ImageSource), typeof(OPLImageViewer),
                new(
                    (sender, e) =>
                    {
                        ((OPLImageViewer)sender).IndicatorImage.Source = (ImageSource)e.NewValue;
                    }));

        /// <summary>
        /// Данные пути к отображаемому объекту
        /// </summary>
        public ImageSource SourceElement
        {
            get => (ImageSource)GetValue(SourceElementProperty);
            set => SetValue(SourceElementProperty, value);
        }
        #endregion

        #endregion

        public OPLImageViewer()
        {
            InitializeComponent();
        }
    }
}
