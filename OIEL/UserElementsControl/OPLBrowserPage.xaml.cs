using OPLAPI.CORE.Animation;
using OPLAPI.CORE.Browser;
using OPLAPI.CORE.Interfaces;
using OPLAPI.OIEL.CORE.Browser;
using OPLAPI.OIEL.CORE.Browser.Base;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Animation;
using Brush = System.Windows.Media.Brush;
using FontFamily = System.Windows.Media.FontFamily;

namespace OPLAPI.OIEL.UserElementsControl
{
    /// <summary>
    /// Объект реализующий отображение PageBrowser
    /// </summary>
    public partial class OPLBrowserPage : UserControl, IOPLAnimate
    {
        private OPLAnimationManager? _ManagerAnimation;
        /// <summary>
        /// Объект менеджера анимационных настроек OPL
        /// </summary>
        public OPLAnimationManager? ManagerAnimation
        {
            get => _ManagerAnimation;
            set
            {
                MainPage?.ManagerAnimation = value;
                _ManagerAnimation = value;
            }
        }

        /// <summary>
        /// Массив объектов страниц
        /// </summary>
        private readonly List<Inlay> Inlays;

        /// <summary>
        /// Активный индекс вкладки в браузере
        /// </summary>
        public int ActualIndex { get; private set; }

        #region MainPage
        /// <summary>
        /// Главная страница в браузере
        /// </summary>
        /// <remarks>
        /// Данный тип страницы открывается только когда нет активной вкладки, либо нет ниодной вкладки
        /// </remarks>
        private MainPageBrowser? MainPage;

        /// <summary>
        /// Состояние инициализации главной страницы для браузера
        /// </summary>
        public bool InicializeMainPage { get; private set; } = false;

        /// <summary>
        /// Состояние активации главной страницы в браузере
        /// </summary>
        public bool ActivateMainPage { get; private set; } = false;

        /// <summary>
        /// Объект исключения отсутствия инициализации главной страницы
        /// </summary>
        private static readonly Exception ExceptionManagerAppPage =
            new($"Главная страница браузера не присвоена. {nameof(GenerateNewMainManagerAppPage)}()");
        #endregion

        /// <summary>
        /// Индекс вкладки, которая была открыта до момента регистрации кастомной страницы
        /// </summary>
        private int? HistoryBackInlayIndex = null;

        /// <summary>
        /// Состояние открытия прочей страницы
        /// </summary>
        public bool ActivateCustomPage { get; private set; } = false;

        #region Properties

        #region Background
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly new DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(OPLBrowserPage),
                new(
                    (sender, e) =>
                    {
                        ((OPLBrowserPage)sender).BorderMain.Background = (Brush)e.NewValue;
                    }));

        /// <summary>
        /// Объект фона
        /// </summary>
        public new Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }
        #endregion

        #region BorderBrush
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly new DependencyProperty BorderBrushProperty =
            DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(OPLBrowserPage),
                new(
                    (sender, e) =>
                    {
                        ((OPLBrowserPage)sender).BorderMain.BorderBrush = (Brush)e.NewValue;
                        ((OPLBrowserPage)sender).BorderInlays.BorderBrush = (Brush)e.NewValue;
                    }));

        /// <summary>
        /// Цвет отображения границ элемента
        /// </summary>
        public new Brush BorderBrush
        {
            get => (Brush)GetValue(BorderBrushProperty);
            set => SetValue(BorderBrushProperty, value);
        }
        #endregion

        #region FontSize
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly new DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(double), typeof(OPLBrowserPage),
                new(12d,
                    (sender, e) =>
                    {
                    }));

        /// <summary>
        /// Размер текста в элементе
        /// </summary>
        public new double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }
        #endregion

        #region FontFamily
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly new DependencyProperty FontFamilyProperty =
            DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(OPLBrowserPage),
                new(new FontFamily("Calibri"),
                    (sender, e) =>
                    {
                    }));

        /// <summary>
        /// Шрифт текста используемый в элементе
        /// </summary>
        public new FontFamily FontFamily
        {
            get => (FontFamily)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }
        #endregion

        #endregion

        /// <summary>
        /// Объект представления стековых объектов - вкладок
        /// </summary>
        private StackPanel StackPanelInlays;

        #region Events
        /// <summary>
        /// Событие открытия главной страницы в браузере
        /// </summary>
        public event EventHandler? MainPageActivated;

        /// <summary>
        /// Событие открытия собственной страницы в браузере
        /// </summary>
        public event EventHandler? CustomPageActivated;

        /// <summary>
        /// Событие открытия вкладки
        /// </summary>
        public event EventHandler<Inlay>? InlayActivated;

        /// <summary>
        /// Событие закрытия вкладки
        /// </summary>
        public event EventHandler<Inlay>? InlayClosed;
        #endregion

        /// <summary>
        /// Инициализировать объект интерфейса отображения страничных объектов
        /// </summary>
        public OPLBrowserPage()
        {
            InitializeComponent();
            ActualIndex = -1;
            Inlays = [];
            StackPanelInlays = new()
            {
                Orientation = System.Windows.Controls.Orientation.Horizontal,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
            };
            IELScrollViewerInlays.Content = StackPanelInlays;
            KeyUp += (sender, e) =>
            {
                switch (e.Key)
                {
                    case System.Windows.Input.Key.LWin:
                        OpenMainPage();
                        break;
                }
                e.Handled = true;
            };
        }

        /// <summary>
        /// Отобразить страницу выбора приложения страницы
        /// </summary>
        public void OpenMainPage()
        {
            if (ActivateMainPage) return;
            else if (MainPage == null) throw ExceptionManagerAppPage;
            ActivateMainPage = true;
            if (ActualIndex > -1)
            {
                Inlays[ActualIndex].Visual.SourceBackground.UsedState = false;
                ActualIndex = -1;
            }
            MainPageController.NextElement(MainPage, false);
            MainPage.Focus();
            MainPageActivated?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        /// Создать новый экземпляр главной страницы для браузера
        /// </summary>
        /// <param name="TypeManagerAppPage"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public void GenerateNewMainManagerAppPage(Type TypeManagerAppPage)
        {
            if (MainPage != null) return;
            else if (TypeManagerAppPage.BaseType != typeof(MainPageBrowser))
                throw new ArgumentException($"Главная страница браузера должна быть наследованным классом от {nameof(MainPageBrowser)}");
            else
            {
                MainPage = (MainPageBrowser)(Activator.CreateInstance(TypeManagerAppPage) ??
                    throw new Exception("Не удалось создать объект главной страницы"));
                MainPage.ManagerAnimation = ManagerAnimation;
                MainPage.ApplicationPageActivated += ApplicationPageActivatedHandler;
                InicializeMainPage = true;
            }
        }

        /// <summary>
        /// Обработчик события активации страничного приложения из главной страницы браузера
        /// </summary>
        private void ApplicationPageActivatedHandler(object? sender, AppPageBase e)
        {
            int? SearchIndex = SearchInlayFromType(e.TypePage);
            if (SearchIndex.HasValue)
                ActivateInlay(SearchIndex.Value);
            else
            {
                Inlay SourceInlay = AddInlay(e.InicializeAppPage());
                SourceInlay.Activated += InlayActivatedHandler;
                SourceInlay.Closed += InlayClosedHandler;
                ActivateInlay(Inlays.Count - 1);
            }
        }

        /// <summary>
        /// Обработчик события активации вкладки браузера
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void InlayActivatedHandler(object? sender, Inlay e)
        {
            ActivateInlay(e, Inlays.IndexOf(e));
        }

        /// <summary>
        /// Обработчик события активации вкладки браузера
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void InlayClosedHandler(object? sender, Inlay e)
        {
            e.Visual.IsEnabled = false;
            DeleteInlayPage(e, ActualIndex == Inlays.IndexOf(e));
            InlayClosed?.Invoke(null, e);
        }

        /// <summary>
        /// Создать новый объект вкладки
        /// </summary>
        /// <param name="Content">Страница содержимого вкладки</param>
        private Inlay AddInlay(in PageBrowser Content)
        {
            Inlay InlayData = Inlay.InicializeInlay(Content);
            Binding binding = new()
            {
                Mode = BindingMode.OneWay,
                Source = (FontFamily)Application.Current.Resources["Bree CYR var"]
            };
            BindingOperations.SetBinding(InlayData.Visual, OPLInlay.FontFamilyProperty, binding);

            Inlays.Add(InlayData);
            StackPanelInlays.Children.Add(InlayData.Visual);

            OPLAnimationManager.AnimateTakingZeroFromTo(ManagerAnimation, InlayData.Visual, WidthProperty,
                0d, InlayData.Visual.ActualWidth, TimeSpan.FromMilliseconds(350d));
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, InlayData.Visual, OpacityProperty,
                1d, TimeSpan.FromMilliseconds(400d));
            return InlayData;
        }

        /// <summary>
        /// Открыть вкладку по индексу
        /// </summary>
        /// <param name="SourceIndex">Индекс открываемой вкладки</param>
        public void ActivateInlay(int SourceIndex)
        {
            if (SourceIndex == ActualIndex && Inlays[SourceIndex].Visual.SourceBackground.UsedState) return;
            else if (ActualIndex > -1 && Inlays.Count > ActualIndex)
            {
                Inlay BackInlay = Inlays[ActualIndex];
                BackInlay.Visual.SourceBackground.UsedState = false;
            }
            ActivateInlay(Inlays[SourceIndex], SourceIndex);
        }

        /// <summary>
        /// Открыть вкладку по управляемому объекту
        /// </summary>
        /// <param name="Source">Открываемая вкладка</param>
        /// <param name="Index">индекс вкладки</param>
        private void ActivateInlay(Inlay Source, int Index)
        {
            if (ActivateMainPage) ActivateMainPage = false;
            PageBrowser Page = Source.ContentPage;
            Source.Visual.SourceBackground.UsedState = true;
            MainPageController.NextElement(Page, Index >= ActualIndex);
            ActualIndex = Index;
            InlayActivated?.Invoke(null, Source);
        }

        /// <summary>
        /// Вернуться назад к странице до открытия кастомных страниц
        /// </summary>
        public void GoBack()
        {
            if (!ActivateCustomPage) return;
            BorderInlays.Height = 55d;
            ActivateCustomPage = false;
            if (HistoryBackInlayIndex.HasValue)
            {
                ActivateInlay(HistoryBackInlayIndex.Value);
                HistoryBackInlayIndex = null;
            }
            else
            {
                OpenMainPage();
            }
        }

        /// <summary>
        /// Активировать собственную страницу в браузере без создания вкладки
        /// </summary>
        /// <param name="SourcePage">Открываемая страница</param>
        /// <param name="RightAlign">Ориентация позиционирования открытия</param>
        public void ActivateCustomPageBrowser(PageBrowser SourcePage, bool RightAlign = true)
        {
            if (SourcePage.GetType() == MainPageController.ActualPage?.GetType()) return;
            else if (!ActivateCustomPage)
            {
                if (ActivateMainPage) ActivateMainPage = false;
                else if (ActualIndex > -1)
                {
                    Inlays[ActualIndex].Visual.SourceBackground.UsedState = false;
                    HistoryBackInlayIndex = ActualIndex;
                }
                BorderInlays.Height = 0d;
                ActivateCustomPage = true;
                ActualIndex = -1;
            }
            MainPageController.NextElement(SourcePage, RightAlign);
        }

        /// <summary>
        /// Сделать поиск страницы по типу
        /// </summary>
        /// <returns>Найденная страница</returns>
        public int? SearchInlayFromType(Type SourceType)
        {
            if (Inlays.Count == 0 || SourceType.BaseType != typeof(PageBrowser)) return null;
            for (int i = 0; i < Inlays.Count; i++)
            {
                if (Inlays[i].ContentPage.GetType() == SourceType)
                    return i;
            }
            return null;
        }

        /// <summary>
        /// Удалить вкладку в браузере
        /// </summary>
        /// <param name="Source">Объект вкладки</param>
        /// <param name="ActivateNextInlay">Активировать ли следующую после удалённой вкладки вкладку</param>
        private void DeleteInlayPage(Inlay Source, bool ActivateNextInlay)
        {
            if (Inlays.IndexOf(Source) is int Index && Index == -1) return;
            int IndexNext = NextIndex(Index, Inlays.Count - 1);
            Inlay SourceInlay = Inlays[Index];
            if (ManagerAnimation != null)
            {
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, SourceInlay.Visual, MarginProperty,
                    new Thickness(0), TimeSpan.FromMilliseconds(350d));
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, SourceInlay.Visual, OpacityProperty,
                    0d, TimeSpan.FromMilliseconds(350d));
                DoubleAnimation animationDouble = ManagerAnimation.GetCloneAnimationElementFromType<DoubleAnimation>();
                animationDouble.Duration = TimeSpan.FromMilliseconds(400d);
                animationDouble.From = SourceInlay.Visual.Width;
                animationDouble.To = 0d;
                animationDouble.FillBehavior = FillBehavior.Stop;
                animationDouble.Completed += (sender, e) =>
                {
                    SourceInlay.Visual.Width = 0d;
                    StackPanelInlays.Children.Remove(SourceInlay.Visual);
                    SourceInlay.Dispose();
                };
                SourceInlay.Visual.BeginAnimation(WidthProperty, animationDouble);
            }
            else
            {
                StackPanelInlays.Children.Remove(SourceInlay.Visual);
                SourceInlay.Dispose();
            }
            Inlays.RemoveAt(Index);

            if (ActivateNextInlay)
            {
                if (IndexNext == -1)
                {
                    if (MainPage == null) throw ExceptionManagerAppPage;
                    ActualIndex = -1;
                    MainPageController.CloseElement();
                    MainPageController.NextElement(MainPage, true);
                }
                else
                {
                    ActivateInlay(IndexNext);
                }
            }
            else if (ActualIndex >= Index) ActualIndex--;
        }

        /// <summary>
        /// Узнать следующий индекс элемента
        /// </summary>
        /// <param name="ActualIndex">Текущий индекс</param>
        /// <param name="Count">Количество элементов</param>
        /// <returns>Ожидаемый индекс элемента</returns>
        private static int NextIndex(int ActualIndex, int Count) => ActualIndex < Count ? ActualIndex : --ActualIndex;
    }
}
