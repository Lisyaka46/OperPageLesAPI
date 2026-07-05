using IEL.CORE.Classes;
using IEL.UserElementsControl.Base;
using Newtonsoft.Json.Linq;
using OPLAPI.CORE.Animation;
using OPLAPI.CORE.Interfaces;
using OPLAPI.OIEL.UserElementsControl.Base;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace OPLAPI.OIEL.UserElementsControl.Network
{
    /// <summary>
    /// Объект отображающий вложенный объект
    /// </summary>
    public partial class OPLVisualNetworkClipFile : NetworkClipElementBase, IOPLAnimate
    {
        #region Properties

        #region TextFileName
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty TextFileNameProperty =
            DependencyProperty.Register("TextFileName", typeof(string), typeof(OPLVisualNetworkClipFile),
                new("Name",
                    (sender, e) =>
                    {
                        ((OPLVisualNetworkClipFile)sender).TextBlockNameFile.Text = (string)e.NewValue;
                    }));

        /// <summary>
        /// Текст имени файла
        /// </summary>
        public string TextFileName
        {
            get => (string)GetValue(TextFileNameProperty);
            set => SetValue(TextFileNameProperty, value);
        }
        #endregion

        #region FontFamily
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly new DependencyProperty FontFamilyProperty =
            DependencyProperty.Register("FontFamily", typeof(System.Windows.Media.FontFamily), typeof(OPLVisualNetworkClipFile),
                new(new System.Windows.Media.FontFamily("Calibri"),
                    (sender, e) =>
                    {
                        ((OPLVisualNetworkClipFile)sender).TextBlockNameFile.FontFamily = (System.Windows.Media.FontFamily)e.NewValue;
                        ((OPLVisualNetworkClipFile)sender).TextBlockSizeFile.FontFamily = (System.Windows.Media.FontFamily)e.NewValue;
                    }));

        /// <summary>
        /// Шрифт отображаемый в элементе
        /// </summary>
        public new System.Windows.Media.FontFamily FontFamily
        {
            get => (System.Windows.Media.FontFamily)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }
        #endregion

        #region StrokeDashLength
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty StrokeDashLengthProperty =
            DependencyProperty.Register("StrokeDashLength", typeof(double), typeof(OPLVisualNetworkClipFile),
                new(28d,
                    (sender, e) =>
                    {
                        ((OPLVisualNetworkClipFile)sender).RectangleLoading.StrokeDashArray[0] = (double)e.NewValue;
                    }));

        /// <summary>
        /// Длинна прирывистой линии
        /// </summary>
        public double StrokeDashLength
        {
            get => (double)GetValue(StrokeDashLengthProperty);
            set => SetValue(StrokeDashLengthProperty, value);
        }
        #endregion

        #endregion

        /// <summary>
        /// Состояние текстовой визуализации загрузки
        /// </summary>
        private bool IsProgressTextVizualizate = false;

        /// <summary>
        /// Инициализировать объект отображающий вложение файла
        /// </summary>
        public OPLVisualNetworkClipFile()
        {
            InitializeComponent();
            TextBlockProgress.Opacity = 0d;
            IconLoadingFile.Opacity = 0d;
            RotateGradientLoading.Angle = 0d;
            RadialGradientLoading.Center = new(0.5d, 0.5d);
            RectangleLoading.StrokeDashOffset = 28d;
            RectangleLoading.StrokeDashArray[0] = 28d;
            TextBlockSizeFile.Text = string.Empty;
            TextBlockNameFile.Text = string.Empty;

            TextBlockSizeFile.Foreground = SourceForeground.SourceBrush;
            TextBlockNameFile.Foreground = SourceForeground.SourceBrush;
            TextBlockProgress.Foreground = SourceForeground.SourceBrush;
            RectangleLoading.Fill = SourceForeground.SourceBrush;
        }

        /// <summary>
        /// Начать визуализировать взаимодействие
        /// </summary>
        public void StartManipulate(bool ProgressTextVisualizate)
        {
            if (IsManipulate)
                throw new Exception("Невозможно визуализировать взаимодействие при уже активном взаимодействии");
            IsManipulate = true;
            IsProgressTextVizualizate = ProgressTextVisualizate;

            RectangleLoading.StrokeDashOffset = 28d;
            if (ManagerAnimation != null)
            {
                RadialGradientLoading.BeginAnimation(RadialGradientBrush.OpacityProperty, null);
                RadialGradientLoading.Opacity = 0d;
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, RectangleLoading, System.Windows.Shapes.Rectangle.StrokeThicknessProperty,
                    4d, TimeSpan.FromSeconds(1d));
                StrokeDashLength = 28d;
                DoubleAnimation animation = ManagerAnimation.GetCloneAnimationElementFromType<DoubleAnimation>();
                animation.From = 0d;
                animation.To = 1d;
                animation.Duration = TimeSpan.FromSeconds(2d);
                animation.BeginTime = TimeSpan.FromSeconds(0.8d);
                RadialGradientLoading.BeginAnimation(RadialGradientBrush.OpacityProperty, animation);
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, RadialGradientLoading, RadialGradientBrush.CenterProperty,
                    new System.Windows.Point(0.35d, 0.5d), TimeSpan.FromMilliseconds(1500d));
                DoubleAnimation animationAngle = ManagerAnimation.GetCloneAnimationElementFromType<DoubleAnimation>();
                animationAngle.EasingFunction = new SineEase()
                {
                    EasingMode = EasingMode.EaseInOut,
                };
                animationAngle.From = 0d;
                animationAngle.To = 360d;
                animationAngle.RepeatBehavior = RepeatBehavior.Forever;
                animationAngle.Duration = TimeSpan.FromSeconds(4d);
                RotateGradientLoading.BeginAnimation(RotateTransform.AngleProperty, animationAngle);
            }
            else
            {
                StrokeDashLength = 28d;
                RectangleLoading.StrokeThickness = 4d;
                RotateGradientLoading.Angle = 0d;
                RadialGradientLoading.Center = new(0.5d, 0.5d);
            }

            //while (OpenStreamFile.Length < DataCount)
            //{
            //    await Task.Delay(500);
            //    if (ManagerAnimation != null)
            //        ManagerAnimation.DoubleAnimationType.AnimateEffect(RectangleLoading, System.Windows.Shapes.Rectangle.StrokeDashOffsetProperty,
            //            26 - (26 * (DataCount / OpenStreamFile.Length)), TimeSpan.FromMilliseconds(400d));
            //    else
            //        RectangleLoading.StrokeDashOffset =
            //            26 - (26 * (DataCount / OpenStreamFile.Length));
            //}

        }

        /// <summary>
        /// Установить текущее значение для манипуляции
        /// </summary>
        /// <param name="Value">Значение отображающее степень манипуляции где 0 это минимум, 1 максимум</param>
        [LoaderOptimization(LoaderOptimization.NotSpecified)]
        public void SetValueManipulate(double Value)
        {
            if (!IsManipulate)
                throw new Exception("Невозможно взаимодействовать с объектом, предварительно не включив режим взаимодействия!");
            Value = Math.Clamp(Value, 0d, 1d);
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, RectangleLoading, System.Windows.Shapes.Rectangle.StrokeDashOffsetProperty,
                        StrokeDashLength - (Value * StrokeDashLength), TimeSpan.FromMilliseconds(200d));
            if (IsProgressTextVizualizate && Value > 0.7d)
            {
                if (TextBlockProgress.Opacity == 0d)
                    OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, TextBlockProgress, OpacityProperty,
                        1d, TimeSpan.FromSeconds(2d));
                CurrentProgress.Text = $"{Math.Round(Value * 100, 2)}";
            }
        }

        /// <summary>
        /// Закончить визуализировать взаимодействие
        /// </summary>
        public void EndManipulate()
        {
            if (!IsManipulate)
                throw new Exception("Невозможно закончить визуализировать взаимодействие при не активном взаимодействии");
            IsManipulate = false;
            CurrentProgress.Text = "100.00";
            if (ManagerAnimation != null)
            {
                RectangleLoading.BeginAnimation(System.Windows.Shapes.Rectangle.StrokeDashOffsetProperty, null);
                BeginAnimation(StrokeDashLengthProperty, null);
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, RadialGradientLoading, RadialGradientBrush.CenterProperty,
                    new System.Windows.Point(0.5d, 0.5d), TimeSpan.FromSeconds(2d));
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, RotateGradientLoading, RotateTransform.AngleProperty,
                    0d, TimeSpan.FromSeconds(2d));
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, RectangleLoading, System.Windows.Shapes.Rectangle.StrokeThicknessProperty,
                    0d, TimeSpan.FromSeconds(2d));
                if (IsProgressTextVizualizate)
                {
                    OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, TextBlockProgress, OpacityProperty,
                        0d, TimeSpan.FromSeconds(2d));
                }
                StrokeDashLength = 448d;
            }
            else
            {
                RectangleLoading.StrokeThickness = 0d;
                if (IsProgressTextVizualizate)
                {
                    TextBlockProgress.Opacity = 0d;
                }
            }
            RectangleLoading.StrokeDashOffset = 28d;
        }

        /// <summary>
        /// Установить иконку по расширению файла
        /// </summary>
        /// <param name="FilePath">Путь к файлу</param>
        /// <param name="DefaultIconFile">Значение по умолчанию при неудачной установке иконки</param>
        public void SetExtractAssociatedIcon(string FilePath, ImageSource? DefaultIconFile = null)
        {
            Icon? FileIcon = File.Exists(FilePath) ? System.Drawing.Icon.ExtractAssociatedIcon(FilePath) : null;
            Dispatcher.Invoke(() =>
            {
                IconLoadingFile.Source =
                    FileIcon != null ? Imaging.CreateBitmapSourceFromHBitmap(FileIcon.ToBitmap().GetHbitmap(),
                    IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()) : DefaultIconFile;
                OPLAnimationManager.AnimateTakingZeroFromTo(ManagerAnimation, IconLoadingFile, OpacityProperty,
                    0d, 1d, TimeSpan.FromMilliseconds(400d));
            });
        }

        /// <summary>
        /// Расчитать и отобразить размер файла
        /// </summary>
        /// <param name="Path">Директория файла</param>
        /// <exception cref="Exception"></exception>
        public void MathSizeFile(string Path) => MathSizeFile(new FileInfo(Path).Length);

        /// <summary>
        /// Расчитать и отобразить размер файла
        /// </summary>
        /// <param name="CountBytes">Количетсво байт хранящееся в файле</param>
        /// <exception cref="Exception"></exception>
        public void MathSizeFile(long CountBytes)
        {
            long LengthFile;
            byte CountR;
            double MainLengthFile;
            string R;
            MainLengthFile = (short)(CountBytes % 1024); // Установка смещения от 1024
            LengthFile = CountBytes - (int)MainLengthFile; // Число кратное 1024
            CountR = 0;
            while (LengthFile >= 1024)
            {
                LengthFile /= 1024;
                if (MainLengthFile > 0.01f)
                    MainLengthFile /= 1024; // Расчёт смещения относительно единицы измерения кол-ва информ.
                CountR++;
            }
            R = CountR switch
            {
                0 => "B",
                1 => "KB",
                2 => "MB",
                3 => "TB",
                4 => "PB",
                _ => throw new Exception("Слишком большой размер файла."),
            };
            MainLengthFile += LengthFile;
            Dispatcher.Invoke(() => TextBlockSizeFile.Text = $"{Math.Round(MainLengthFile, 2)} {R}");
        }
    }
}
