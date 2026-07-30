using OPLAPI.CORE.Animation;
using OPLAPI.CORE.Browser;
using OPLAPI.CORE.Interfaces;
using OPLAPI.OIEL.CORE.Browser;
using OPLAPI.OIEL.CORE.Browser.Base;
using System.Collections.ObjectModel;
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
        /// <summary>
        /// Объект менеджера анимационных настроек OPL
        /// </summary>
        public OPLAnimationManager? ManagerAnimation { get; set; }

        /// <summary>
        /// Массив объектов страниц
        /// </summary>
        private readonly List<Inlay> Inlays;

        /// <summary>
        /// Активный индекс вкладки в браузере
        /// </summary>
        public int ActualIndex { get; private set; }

        /// <summary>
        /// Активная вкладка в браузере
        /// </summary>
        public Inlay? ActualInlay => ActualIndex > -1 ? Inlays[ActualIndex] : null;

        /// <summary>
        /// Событие закрытия вкладки
        /// </summary>
        public event EventHandler<Inlay>? AddNewInlay;

        /// <summary>
        /// Страница выбора приложения страницы для добавления её в браузер
        /// </summary>
        public MainPageBrowser? SourceManagerAppPage { get; private set; }

        /// <summary>
        /// Объект исключения отсутствия инициализации главной страницы
        /// </summary>
        private static readonly Exception ExceptionManagerAppPage =
            new($"Главная страница браузера не присвоена. {nameof(GenerateNewMainManagerAppPage)}()");

        /// <summary>
        /// Состояние открытия главной страницы
        /// </summary>
        public bool ActivateManagerPage { get; private set; } = false;

        /// <summary>
        /// Состояние открытия прочей страницы
        /// </summary>
        public bool ActivateCustomPage { get; private set; } = false;

        /// <summary>
        /// Актуальная страница которая открыта в браузере
        /// </summary>
        public Page? ActualPage => MainPageController.ActualPage;

        /// <summary>
        /// Страница которая была открыта до момента регистрации кастомной страницы
        /// </summary>
        private Page? HistoryBackPage = null;

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

        /// <summary>
        /// Инициализировать объект интерфейса отображения страничных объектов
        /// </summary>
        public OPLBrowserPage()
        {
            InitializeComponent();
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
                    case System.Windows.Input.Key.Home:
                        OpenManagerAppPage();
                        break;
                }
            };
        }

        /// <summary>
        /// Создать новый экземпляр главной страницы для браузера
        /// </summary>
        /// <param name="TypeManagerAppPage"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public void GenerateNewMainManagerAppPage(Type TypeManagerAppPage)
        {
            if (SourceManagerAppPage != null) return;
            else if (TypeManagerAppPage.BaseType != typeof(MainPageBrowser))
                throw new ArgumentException($"Главная страница браузера должна быть наследованным классом от {nameof(MainPageBrowser)}");
            else
            {
                SourceManagerAppPage = (MainPageBrowser)(Activator.CreateInstance(TypeManagerAppPage) ??
                    throw new Exception("Не удалось создать объект главной страницы"));
                SourceManagerAppPage.ManagerAnimation = ManagerAnimation;
            }
        }

        #region MainPageBrowser
        /// <summary>
        /// Отобразить страницу выбора приложения страницы
        /// </summary>
        public void OpenManagerAppPage()
        {
            if (SourceManagerAppPage == null) throw ExceptionManagerAppPage;
            else if (ActivateManagerPage) return;
            ActivateManagerPage = true;
            if (ActualIndex > -1)
            {
                Inlays[ActualIndex].Visual?.SourceBackground.UsedState = false;
                ActualIndex = -1;
            }
            if (ActivateCustomPage)
            {
                BorderInlays.Height = 55d;
                ActivateCustomPage = false;
            }
            MainPageController.NextElement(SourceManagerAppPage, false);
            SourceManagerAppPage.Focus();
        }

        /// <summary>
        /// Добавить страничное приложение в менеджер приложений страниц
        /// </summary>
        /// <param name="TypeAppPage">Тип создаваемого приложения страницы</param>
        public void AddNewAppPage(Type TypeAppPage)
        {
            if (SourceManagerAppPage == null) throw ExceptionManagerAppPage;
            AppPage Source = SourceManagerAppPage.AddNewAppPage(TypeAppPage);
            Source.ApplicationPageActivate += Source_ApplicationPageActivate;
        }

        /// <summary>
        /// Установить и добавить страничное приложение в менеджер приложений страниц
        /// </summary>
        /// <param name="PathFile">Директория установочного файла страничного приложения</param>
        public void AddNewAppPage(string PathFile)
        {
            if (SourceManagerAppPage == null) throw ExceptionManagerAppPage;
            InstallableAppPage Source = SourceManagerAppPage.AddNewAppPage(PathFile);
            Source.ApplicationPageActivate += Source_ApplicationPageActivate;
        }

        private void Source_ApplicationPageActivate(object? sender, Type e)
        {
            Index? SearchIndex = SearchInlayFromType(e);
            if (SearchIndex.HasValue)
                ActivateInlay(SearchIndex.Value);
            else
            {
                Inlay SourceInlay = AddInlay(AppPageBase.InicializeAppPage(e));
                AddNewInlay?.Invoke(sender, SourceInlay);
                ActivateInlay(^1);
            }
        }
        #endregion

        #region ManipulateInlay
        /// <summary>
        /// Создать новый объект вкладки
        /// </summary>
        /// <param name="Content">Страница содержимого вкладки</param>
        private Inlay AddInlay(in PageBrowser Content)
        {
            Inlay InlayData = Inlay.InicializeInlay(Content);
            InlayData.Closed += (sender, e) =>
            {
                e.Visual?.IsEnabled = false;
                DeleteInlayPage(e, ActualIndex == Inlays.IndexOf(e));
            };
            InlayData.Activated += (sender, e) =>
            {
                ActivateInlay(Inlays.IndexOf(e));
            };
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
        #endregion

        /// <summary>
        /// Открыть страницу по индексу
        /// </summary>
        /// <param name="SourceIndex">Индекс открываемой страницы</param>
        public void ActivateInlay(Index SourceIndex)
        {
            if (SourceIndex.Value >= Inlays.Count) throw new ArgumentOutOfRangeException(nameof(SourceIndex));
            else if (SourceIndex.Value == ActualIndex && Inlays[SourceIndex].Visual.SourceBackground.UsedState) return;
            else if (ActivateManagerPage) ActivateManagerPage = false;
            PageBrowser Page = Inlays[SourceIndex].ContentPage;
            if (ActualIndex > -1 && Inlays.Count > ActualIndex)
            {
                Inlay BackInlay = Inlays[ActualIndex];
                BackInlay.Visual.SourceBackground.UsedState = false;
            }
            Inlay NextInlay = Inlays[SourceIndex];
            NextInlay.Visual.SourceBackground.UsedState = true;
            MainPageController.NextElement(Page, SourceIndex.Value >= ActualIndex);
            ActualIndex = SourceIndex.Value;
        }

        /// <summary>
        /// Вернуться назад к странице до открытия кастомных страниц
        /// </summary>
        public void GoBack()
        {
            if (HistoryBackPage != null)
            {
                BorderInlays.Height = 55d;
                ActivateCustomPage = false;
                if (ActualIndex > -1)
                {
                    ActivateInlay(ActualIndex);
                }
                else
                    MainPageController.NextElement(HistoryBackPage, false);
                HistoryBackPage = null;
            }
            else
            {
                OpenManagerAppPage();
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
                if (ActivateManagerPage) ActivateManagerPage = false;
                else HistoryBackPage = MainPageController.ActualPage;
                if (ActualIndex > -1)
                    Inlays[ActualIndex].Visual?.SourceBackground.UsedState = false;
                BorderInlays.Height = 0d;
                ActivateCustomPage = true;
            }
            MainPageController.NextElement(SourcePage, RightAlign);
        }

        /// <summary>
        /// Сделать поиск страницы по типу
        /// </summary>
        /// <returns>Найденная страница</returns>
        public Index? SearchInlayFromType(Type SourceType)
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
        /// <param name="inlay">Объект вкладки</param>
        /// <param name="ActivateNextInlay">Активировать ли следующую после удалённой вкладки вкладку</param>
        public void DeleteInlayPage(Inlay inlay, bool ActivateNextInlay = true)
        {
            if (Inlays.IndexOf(inlay) is int Index && Index == -1) return;
            int IndexNext = NextIndex(Index, Inlays.Count - 1);
            Inlay SourceInlay = Inlays[Index];
            SourceInlay.ContentPage?.Dispose();
            if (SourceInlay.Visual == null) return;
            else if (ManagerAnimation != null)
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
                };
                SourceInlay.Visual.BeginAnimation(WidthProperty, animationDouble);
            }
            else
            {
                //ActualInlay.Width = 0d;
                StackPanelInlays.Children.Remove(SourceInlay.Visual);
            }
            Inlays.RemoveAt(Index);

            if (ActivateNextInlay)
            {
                if (IndexNext == -1)
                {
                    if (SourceManagerAppPage == null) throw ExceptionManagerAppPage;
                    ActualIndex = -1;
                    MainPageController.CloseElement();
                    MainPageController.NextElement(SourceManagerAppPage, true);
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
