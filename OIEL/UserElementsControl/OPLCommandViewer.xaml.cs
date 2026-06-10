using IEL.UserElementsControl.Base;
using OPLAPI.CORE.Animation;
using OPLAPI.CORE.Interfaces;
using OPLAPI.CORE;
using OPLAPI.OIEL.UserElementsControl.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace OPLAPI.OIEL.UserElementsControl
{
    /// <summary>
    /// Логика взаимодействия для OPLCommandViewer.xaml
    /// </summary>
    public partial class OPLCommandViewer : IELContainerBase, IOPERCommandViewer, IOPLAnimate
    {
        /// <summary>
        /// Главный объект отображения текста
        /// </summary>
        private TextBlock TextBlockHead;

        /// <summary>
        /// Объект менеджера анимационных настроек OPL
        /// </summary>
        public OPLAnimationManager? ManagerAnimation { get; set; }

        #region Properties

        #region Text
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(OPLCommandViewer),
                new("Name",
                    (sender, e) =>
                    {
                        ((OPLCommandViewer)sender).TextBlockNameCommand.Text = (string)e.NewValue;
                    }));

        /// <summary>
        /// Текст отображаемый в названии
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        #endregion

        #region Source
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(Uri), typeof(OPLMediaViewer),
                new(
                    (sender, e) =>
                    {
                        ((OPLMediaViewer)sender).IndicatorMedia.Source = (Uri)e.NewValue;
                    }));

        /// <summary>
        /// Данные пути к медиа загрузки объекта
        /// </summary>
        public Uri Source
        {
            get => (Uri)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }
        #endregion

        #region DeleteButtonSource
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty DeleteButtonSourceProperty =
            DependencyProperty.Register("DeleteButtonSource", typeof(ImageSource), typeof(OPLCommandViewer),
                new(
                    (sender, e) =>
                    {
                        ((OPLCommandViewer)sender).IELButtonDeleteElement.Source = (ImageSource)e.NewValue;
                    }));

        /// <summary>
        /// Отображаемое изображение в кнопке удаления визуализационного элемента
        /// </summary>
        public ImageSource DeleteButtonSource
        {
            get => (ImageSource)GetValue(DeleteButtonSourceProperty);
            set => SetValue(DeleteButtonSourceProperty, value);
        }
        #endregion

        #endregion

        /// <summary>
        /// Токен отмены асинхронной загрузочной операции
        /// </summary>
        private CancellationTokenSource? SourceTokenAsyncLoading = null;

        /// <summary>
        /// Имеется ли активный/исполняемый токен асинхонной загрузки
        /// </summary>
        public bool IsTokenAsyncLoadingEnabled => (SourceTokenAsyncLoading?.Token.CanBeCanceled) ?? false;

        /// <summary>
        /// Событие добавления контента в объект визуализатора
        /// </summary>
        public event EventHandler? AddContentInViewer;

        /// <summary>
        /// Состояние асинхронного постоянного исполнения
        /// </summary>
        public bool IsTokenAsyncWhileEnabled => (SourceTokenAsyncWhile?.Token.CanBeCanceled) ?? false;

        /// <summary>
        /// Токен отмены асинхронной циклической операции
        /// </summary>
        private CancellationTokenSource? SourceTokenAsyncWhile = null;

        /// <summary>
        /// Левая активация кнопки в элементе визуализатора
        /// </summary>
        public event IELButtonBase.ActivateHandler? ButtonDelete_OnActivateMouseLeft
        {
            add => IELButtonDeleteElement.OnActivateMouseLeft += value;
            remove => IELButtonDeleteElement.OnActivateMouseLeft -= value;
        }

        /// <summary>
        /// Правая активация кнопки в элементе визуализатора
        /// </summary>
        public event IELButtonBase.ActivateHandler? ButtonDelete_OnActivateMouseRight
        {
            add => IELButtonDeleteElement.OnActivateMouseRight += value;
            remove => IELButtonDeleteElement.OnActivateMouseRight -= value;
        }

        public OPLCommandViewer()
        {
            InitializeComponent();
            TextBlockNameCommand.Text = "Name";
            AsyncIndicator.Opacity = 0d;
            BorderInfo.BorderBrush = SourceBorderBrush.SourceBrush;
            IndicatorLoading.Opacity = 0d;
            IndicatorLoading.Source = null;
            IndicatorLoading.MediaEnded += (sender, e) =>
            {
                IndicatorLoading.Position = TimeSpan.FromMilliseconds(1);
            };
            IndicatorLoading.Stop();
            Container.Children.Clear();

            TextBlockHead = CreateHeadTextBlock();
            Container.Children.Add(TextBlockHead);
        }

        /// <summary>
        /// Добавить новый <b>не форматированный</b> текст
        /// </summary>
        /// <param name="Source">Добавляемый текст</param>
        public void AddString(string Source)
        {
            Dispatcher.Invoke(() =>
            {
                P_AddString(Source);
                UpdateLayout();
            });
            AddContentInViewer?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Добавить новый <b>не форматированный</b> текст
        /// <b>БЕЗ СОБЫТИЯ</b>
        /// </summary>
        /// <param name="Source">Добавляемый текст</param>
        private void P_AddString(string Source)
        {
            if (TextBlockHead.Inlines.Count > 0)
                TextBlockHead.Inlines.Add(new LineBreak());
            TextBlockHead.Inlines.Add(Source);
        }

        /// <summary>
        /// Добавить новый текст исходя из входящего объекта
        /// </summary>
        /// <param name="Array">Массив зависимых объектов</param>
        /// <param name="Function">Преобразование данных объекта в строку</param>
        public void AddString<TSource>(TSource[] Array, Func<TSource, string?> Function)
        {
            string? Source = null;
            foreach (TSource item in Array)
            {
                Source = Function.Invoke(item);
                if (Source != null)
                    P_AddString(Source);
            }
            UpdateLayout();
            AddContentInViewer?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Добавить новый <b>форматированный</b> текст
        /// </summary>
        /// <param name="Source">Добавляемый текст</param>
        public void AddFormattedString(string Source)
        {
            if (TextBlockHead.Inlines.Count > 0)
                TextBlockHead.Inlines.Add(new LineBreak());
            TextBlockHead.Inlines.Add(BIU.FormattedAllTextDetect(Source));
            UpdateLayout();
            AddContentInViewer?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Добавить новый элемент управления в консоль
        /// </summary>
        /// <param name="Source">Добавляемый элемент</param>
        public void AddNewUIElement(UIElement Source)
        {
            Container.Children.Add(Source);

            TextBlockHead = CreateHeadTextBlock();
            Container.Children.Add(TextBlockHead);
            UpdateLayout();
            AddContentInViewer?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Зарегестрировать циклическую асинхронную операцию
        /// </summary>
        /// <param name="Source">Асинхронная операция</param>
        /// <param name="ExceptionRealized">Выводить ли сообщение об ошибке</param>
        /// <returns></returns>
        /// <exception cref="Exception">Исключение невозможной регистрации операции</exception>
        public async Task WaitWhileTaskOperation(Action Source, bool ExceptionRealized = true)
        {
            SourceTokenAsyncWhile = new();
            if (SourceTokenAsyncWhile.IsCancellationRequested)
                throw new Exception("Невозможно зарегестрировать токен операция которого уже была отменена.");

            if (ManagerAnimation != null)
            {
                DoubleAnimation AnimationDoubleGradientStops = ManagerAnimation.GetCloneAnimationElementFromType<DoubleAnimation>();
                AnimationDoubleGradientStops.RepeatBehavior = RepeatBehavior.Forever;
                AnimationDoubleGradientStops.AutoReverse = true;
                AnimationDoubleGradientStops.EasingFunction = null;
                AnimationDoubleGradientStops.Duration = TimeSpan.FromSeconds(1d);
                AnimationDoubleGradientStops.From = 0d;
                AnimationDoubleGradientStops.To = 1d;

                AsyncIndicator.BeginAnimation(OpacityProperty, AnimationDoubleGradientStops);
            }
            else
                AsyncIndicator.Opacity = 1d;
            Task task = new(() =>
            {
                while (!SourceTokenAsyncWhile.Token.IsCancellationRequested)
                    Source.Invoke();
            }, SourceTokenAsyncWhile.Token);
            task.Start();
            await ExecuteTask(task, SourceTokenAsyncWhile, ExceptionRealized);
        }

        /// <summary>
        /// Осуществить выполнение процесса через визуализацию асинхронной загрузки без ожидаемого значения
        /// </summary>
        /// <param name="Method">Исполняемый асинхронный процесс</param>
        /// <param name="ExceptionRealized">Выводить ли сообщение об ошибке</param>
        /// <returns>Исполненный асинхронный процесс</returns>
        public async Task ExecuteVisualizateTask(Task Method, bool ExceptionRealized = true)
        {
            if (IsTokenAsyncLoadingEnabled) throw new Exception(
                "Невозможно визуализировать ожидание так как текущее ожидание не завершилось!\n" +
                "Завершение визуализации команды.");
            DoubleAnimation? animation = null;
            if (ManagerAnimation != null)
            {
                animation = ManagerAnimation.GetCloneAnimationElementFromType<DoubleAnimation>();
                animation.To = 0d;
                animation.Duration = TimeSpan.FromMilliseconds(480d);
                animation.FillBehavior = FillBehavior.Stop;
                animation.Completed += (sender, e) =>
                {
                    IndicatorLoading.Opacity = 0d;
                    IndicatorLoading.Stop();
                };
            }
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, IndicatorLoading, OpacityProperty,
                1d, TimeSpan.FromMilliseconds(480d));
            SourceTokenAsyncLoading = new();
            IndicatorLoading.Play();

            try { await ExecuteTask(Method, SourceTokenAsyncLoading, ExceptionRealized); }
            finally
            {
                if (animation != null) IndicatorLoading.BeginAnimation(OpacityProperty, animation);
                else
                {
                    IndicatorLoading.Opacity = 0d;
                    IndicatorLoading.Stop();
                }
                SourceTokenAsyncLoading.Dispose();
                GC.Collect();
                SourceTokenAsyncLoading = null;
            }
            if (Method.IsCanceled)
                throw new OperationCanceledException(
                    "Операция исполнения команды была прервана через визуализатор!\n" +
                    "Завершение визуализации команды.");
        }

        /// <summary>
        /// Осуществить выполнение процесса через визуализацию асинхронной загрузки без ожидаемого значения
        /// </summary>
        /// <param name="Method">Исполняемый асинхронный процесс</param>
        /// <param name="ExceptionRealized">Выводить ли сообщение об ошибке</param>
        /// <returns>Исполненный асинхронный процесс</returns>
        public async Task<T> ExecuteVisualizateTask<T>(Task<T> Method, bool ExceptionRealized = true)
        {
            if (IsTokenAsyncLoadingEnabled) throw new Exception(
                "Невозможно визуализировать ожидание так как текущее ожидание не завершилось!\n" +
                "Завершение визуализации команды.");
            DoubleAnimation? animation = null;
            if (ManagerAnimation != null)
            {
                animation = ManagerAnimation.GetCloneAnimationElementFromType<DoubleAnimation>();
                animation.To = 0d;
                animation.Duration = TimeSpan.FromMilliseconds(480d);
                animation.FillBehavior = FillBehavior.Stop;
                animation.Completed += (sender, e) =>
                {
                    IndicatorLoading.Opacity = 0d;
                    IndicatorLoading.Stop();
                };
            }
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, IndicatorLoading, OpacityProperty,
                1d, TimeSpan.FromMilliseconds(480d));
            SourceTokenAsyncLoading = new();
            IndicatorLoading.Play();

            T Result;
            try { Result = await ExecuteTask(Method, SourceTokenAsyncLoading, ExceptionRealized); }
            finally
            {
                if (animation != null) IndicatorLoading.BeginAnimation(OpacityProperty, animation);
                else
                {
                    IndicatorLoading.Opacity = 0d;
                    IndicatorLoading.Stop();
                }
                SourceTokenAsyncLoading.Dispose();
                GC.Collect();
                SourceTokenAsyncLoading = null;
            }
            if (Method.IsCanceled)
                throw new OperationCanceledException(
                    "Операция исполнения команды была прервана через визуализатор!\n" +
                    "Завершение визуализации команды.");
            return Result;
        }

        #region AsyncWait
        /// <summary>
        /// Исполнить Task и исполнить ожидание исполнения
        /// </summary>
        /// <param name="Source">Асинхронная операция</param>
        /// <param name="SourceToken">Управляемый токен</param>
        /// <param name="ExceptionRealized">Выводить ли сообщение об ошибке</param>
        /// <returns></returns>
        private async Task ExecuteTask(Task Source, CancellationTokenSource SourceToken, bool ExceptionRealized)
        {
            try { await Source.WaitAsync(SourceToken.Token); }
            catch
            {
                if (ExceptionRealized)
                    AddFormattedString(
                    "%#FFBABA__Произошла ошибка в исполнении операции:__\n");// +
                    //$"\"%//{ex.Message}//\"");
                else throw;
            }
        }

        /// <summary>
        /// Исполнить Task и исполнить ожидание исполнения
        /// </summary>
        /// <param name="Source">Асинхронная операция</param>
        /// <param name="SourceToken">Управляемый токен</param>
        /// <param name="ExceptionRealized">Выводить ли сообщение об ошибке</param>
        /// <returns></returns>
        private async Task<T> ExecuteTask<T>(Task<T> Source, CancellationTokenSource SourceToken, bool ExceptionRealized = true)
        {
            T? Result = default;
            try { Result = await Source.WaitAsync(SourceToken.Token); }
            catch (Exception ex)
            {
                if (ExceptionRealized)
                    AddFormattedString(
                    "%#FFBABA__Произошла ошибка в исполнении операции:__\n" +
                    $"\"%//{ex.Message}//\"");
                else throw;
            }
            return Result != null ? Result : throw new Exception("Непредвиденное возвращение нулевого объекта в ожидании.");
        }
        #endregion

        /// <summary>
        /// Отменить выполнение асинхронной операции
        /// </summary>
        public void CancelExecuteTaskCommand()
        {
            if (SourceTokenAsyncLoading == null) throw new Exception("Невозможно отменить выполнение асинхронной операции не запустив её!");
            SourceTokenAsyncLoading.Cancel();
        }

        /// <summary>
        /// Осуществить выход из циклической асинхронной операции
        /// </summary>
        public void ExitAsyncWhileOperation()
        {
            if (SourceTokenAsyncWhile == null) throw new Exception("Невозможно отменить выполнение асинхронной операции не запустив её!");
            AsyncIndicator.Dispatcher.Invoke(() =>
            {
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, AsyncIndicator, OpacityProperty,
                    0d, TimeSpan.FromMilliseconds(400d));
            });
            SourceTokenAsyncWhile.Cancel();
        }

        /// <summary>
        /// Создать управляемый объект для текста
        /// </summary>
        /// <returns></returns>
        private static TextBlock CreateHeadTextBlock()
        {
            TextBlock Element = new()
            {
                TextWrapping = TextWrapping.Wrap,
                TextTrimming = TextTrimming.WordEllipsis,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                Margin = new(3),
            };
            return Element;
        }
    }
}
