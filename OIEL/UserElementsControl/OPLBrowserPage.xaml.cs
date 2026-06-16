using IEL.CORE.Classes;
using IEL.UserElementsControl;
using IEL.UserElementsControl.Base;
using OPLAPI.CORE.Animation;
using OPLAPI.CORE.Interfaces;
using OPLAPI.OIEL.CORE.Browser;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Brush = System.Windows.Media.Brush;
using FontFamily = System.Windows.Media.FontFamily;

namespace OPLAPI.OIEL.UserElementsControl
{
    /// <summary>
    /// Объект реализующий отображение PageBrowser
    /// </summary>
    public partial class OPLBrowserPage : System.Windows.Controls.UserControl, IOPLAnimate
    {
        /// <summary>
        /// Объект менеджера анимационных настроек OPL
        /// </summary>
        public OPLAnimationManager? ManagerAnimation { get; set; }

        /// <summary>
        /// Массив объектов страниц
        /// </summary>
        private readonly List<OPLInlay> SourceInlays;

        /// <summary>
        /// Массив активных вкладок браузера
        /// </summary>
        public ReadOnlyCollection<OPLInlay> Inlays => SourceInlays.AsReadOnly();

        /// <summary>
        /// Количество вкладок в браузере страниц
        /// </summary>
        public int InlaysCount => SourceInlays.Count;

        private int _ActivateIndex = -1;
        /// <summary>
        /// Индекс активированной вкладки
        /// </summary>
        public int ActivateIndex => _ActivateIndex;

        /// <summary>
        /// Активная вкладка в браузере
        /// </summary>
        public OPLInlay? ActualInlay => ActivateIndex > -1 ? SourceInlays[ActivateIndex] : null;

        #region Events
        /// <summary>
        /// Событие закрытия вкладки
        /// </summary>
        public event EventHandler<OPLInlay> EventCloseInlay = null!;

        /// <summary>
        /// Событие создания кового страничного приложения
        /// </summary>
        public event EventHandler<OPLInlay> NewInicializedAppPage = null!;
        #endregion

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
            SourceInlays = [];
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
            if (ActivateIndex > -1)
            {
                SourceInlays[ActivateIndex].SourceBackground.SetUsedState(false);
                _ActivateIndex = -1;
            }
            if (ActivateCustomPage)
            {
                BorderInlays.Height = 55d;
                ActivateCustomPage = false;
            }
            MainPageController.NextPage(SourceManagerAppPage, false);
            Keyboard.Focus(SourceManagerAppPage);
            SourceManagerAppPage.Focus();
        }

        /// <summary>
        /// Добавить отображение иконки в менеджере приложений страниц
        /// </summary>
        /// <param name="TypeAppPage">Тип создаваемого приложения страницы</param>
        public void AddNewAppPage(Type TypeAppPage)
        {
            if (SourceManagerAppPage == null) throw ExceptionManagerAppPage;
            AppPage Source = SourceManagerAppPage.AddNewAppPage(TypeAppPage);
            Source.ApplicationPageActivate += Source_ApplicationPageActivate;
        }

        /// <summary>
        /// Добавить отображение иконки в менеджере приложений страниц
        /// </summary>
        /// <param name="PathFile">Директория установочного файла страничного приложения</param>
        public void AddNewAppPage(string PathFile)
        {
            if (SourceManagerAppPage == null) throw ExceptionManagerAppPage;
            InstallableAppPage Source = SourceManagerAppPage.AddNewAppPage(PathFile);
            Source.ApplicationPageActivate += Source_ApplicationPageActivate;
        }

        private void Source_ApplicationPageActivate(object? sender, AppPage e)
        {
            PageBrowser? InicializeInlay = SearchAnyPageType(e.TypePage);
            if (InicializeInlay != null)
                ActivateInlayInBrowserPage(InicializeInlay);
            else
            {
                NewInicializedAppPage.Invoke(sender, InitAppPageFromType(in e));
            }
        }

        private void Source_ApplicationPageActivate(object? sender, InstallableAppPage e)
        {
            PageBrowser? InicializeInlay = SearchAnyPageType(e.TypePage);
            if (InicializeInlay != null)
                ActivateInlayInBrowserPage(InicializeInlay);
            else
            {
                NewInicializedAppPage.Invoke(sender, InitAppPageFromType(in e));
            }
        }

        /// <summary>
        /// Инициализировать страницу по хранимому типу в иконке
        /// </summary>
        /// <param name="AppPage">Объект данных страничного приложения</param>
        private OPLInlay InitAppPageFromType<T>(in T AppPage) where T : AppPage
        {
            if (SourceManagerAppPage == null) throw ExceptionManagerAppPage;
            PageBrowser ElementAppPage = MainPageBrowser.InitPageBrowserFromType(AppPage);
            ElementAppPage.ManagerAnimation = ManagerAnimation;
            return AddInlayPage(in ElementAppPage, AppPage.VisualELement.PaletteElement, true);
        }
        #endregion

        #region ManipulateInlay
        /// <summary>
        /// Добавить новую страницу
        /// </summary>
        /// <param name="Content">Добавляемая страница в баузер страниц</param>
        /// <param name="Activate">Активировать сразу или нет страницу</param>
        public OPLInlay AddInlayPage(in PageBrowser Content, PaletteSpectrum SpectrumInlay, bool Activate = true)
        {
            OPLInlay InlaySource = CreateInlay(Content);
            InlaySource.PaletteElement = SpectrumInlay;

            SourceInlays.Add(InlaySource);
            StackPanelInlays.Children.Add(InlaySource);
            InlaySource.UpdateLayout();

            OPLAnimationManager.AnimateTakingZeroFromTo(ManagerAnimation, InlaySource, WidthProperty,
                0d, InlaySource.ActualWidth, TimeSpan.FromMilliseconds(350d));
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, InlaySource, OpacityProperty,
                1d, TimeSpan.FromMilliseconds(400d));

            //SourceDoubleAnimation.To = InlaySource.ActualWidth;
            //InlaySource.BeginAnimation(WidthProperty, SourceDoubleAnimation, HandoffBehavior.SnapshotAndReplace);

            if (Activate) ActivateInlayIndex(SourceInlays.Count - 1);
            return InlaySource;
        }

        /// <summary>
        /// Создать вкладку в браузере
        /// </summary>
        /// <param name="Content">Страница ссылки</param>
        /// <returns>Созданная вкладка</returns>
        private OPLInlay CreateInlay(PageBrowser Content)
        {
            OPLInlay Inlay = new(Content)
            {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Bottom,
                BorderThickness = new(1.5d),
                CornerRadius = new(12),
                Margin = new(2),
                Opacity = 0d,
                IsAnimatedSettingQ = false,
                IsEnabledSettingQ = false,
            };
            Inlay.OnActivateCloseInlay += (sender, e) =>
            {
                //if (ScrollMouseUse) return;
                e.IsEnabled = false;
                DeleteInlayPage(e, ActivateIndex == SourceInlays.IndexOf(e));
                EventCloseInlay.Invoke(this, e);
            };
            Inlay.MouseLeftButtonUp += (sender, e) =>
            {
                //if (ScrollMouseUse)
                //{
                //    ScrollMouseUse = false;
                //    return;
                //}
                ActivateInlayInBrowserPage(Inlay.Content);
            };
            Binding binding = new()
            {
                Mode = BindingMode.OneWay,
                Source = (FontFamily)Application.Current.Resources["Bree CYR var"]
            };
            BindingOperations.SetBinding(Inlay, OPLInlay.FontFamilyProperty, binding);
            //Inlay.MouseHover += (sender, e) =>
            //{
            //    if (Inlay.Content.Description.Length == 0) return;
            //    EventOnDescriptionInlay?.Invoke(Inlay, Inlay.Content.Description);
            //};
            //Inlay.MouseLeave += (sender, e) =>
            //{
            //    if (Inlay.Content.Description.Length == 0) return;
            //    EventOffDescriptionInlay?.Invoke();
            //};
            //Inlay.MouseDown += (sender, e) =>
            //{
            //    if (Inlay.Content.Description.Length == 0) return;
            //    EventOffDescriptionInlay?.Invoke();
            //};
            return Inlay;
        }
        #endregion

        #region ManipulateInlay
        /// <summary>
        /// Открыть страницу по индексу
        /// </summary>
        /// <param name="index">Индекс открываемой страницы</param>
        /// <exception cref="Exception">Исключение при пустой странице в найденой вкладке</exception>
        public void ActivateInlayIndex(Index index)
        {
            if (index.Value == ActivateIndex && SourceInlays[index].SourceBackground.GetUsedState()) return;
            else if (ActivateManagerPage) ActivateManagerPage = false;
            PageBrowser Page = SourceInlays[index].Content ?? throw new Exception("Объект заголовка не может быть без страницы!");
            //SourceDoubleAnimation.Duration = TimeSpan.FromMilliseconds(300d);
            if (ActivateIndex > -1 && SourceInlays.Count > ActivateIndex)
            {
                OPLInlay BackInlay = SourceInlays[ActivateIndex];
                BackInlay.SourceBackground.SetUsedState(false);
                //SourceDoubleAnimation.To = 45d;
                //BackInlay.BeginAnimation(HeightProperty, SourceDoubleAnimation, HandoffBehavior.SnapshotAndReplace);
            }
            OPLInlay NextInlay = SourceInlays[index];
            //SourceDoubleAnimation.To = 50d;
            //NextInlay.BeginAnimation(HeightProperty, SourceDoubleAnimation, HandoffBehavior.SnapshotAndReplace);
            NextInlay.SourceBackground.SetUsedState(true);
            MainPageController.NextPage(Page, index.Value >= ActivateIndex);
            _ActivateIndex = index.Value;
            //Page.EventFocusPage?.Invoke(Page);
        }

        /// <summary>
        /// Открыть страницу по элементу
        /// </summary>
        /// <param name="Page">Открываемая вкладка страницы</param>
        /// <exception cref="Exception">Исключение при пустой странице в найденой вкладке</exception>
        public void ActivateInlayInBrowserPage(PageBrowser Page)
        {
            try
            {
                PageBrowser?[] Pages = [.. SourceInlays.Select((i) => i.Content)];
                ActivateInlayIndex(Array.IndexOf(Pages, Page));
            }
            catch { }
        }
        #endregion

        /// <summary>
        /// Вернуться назад к странице до открытия кастомных страниц
        /// </summary>
        public void GoBack()
        {
            if (HistoryBackPage != null)
            {
                BorderInlays.Height = 55d;
                ActivateCustomPage = false;
                if (ActivateIndex > -1)
                {
                    ActivateInlayIndex(_ActivateIndex);
                }
                else
                    MainPageController.NextPage(HistoryBackPage, false);
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
                if (ActivateIndex > -1)
                    SourceInlays[ActivateIndex].SourceBackground.SetUsedState(false);
                BorderInlays.Height = 0d;
                ActivateCustomPage = true;
            }
            MainPageController.NextPage(SourcePage, RightAlign);
        }

        /// <summary>
        /// Сделать поиск страниц по типу
        /// </summary>
        /// <typeparam name="T">Тип страницы поиска</typeparam>
        /// <returns>Найденные страницы</returns>
        public T[]? SearchAllPageType<T>() where T : PageBrowser
        {
            if (InlaysCount == 0) return null;
            List<T> values = [];
            foreach (OPLInlay Inlay in SourceInlays)
            {
                if (Inlay.Content?.GetType() == typeof(T)) values.Add((T)Inlay.Content);
            }
            return values.Count == 0 ? null : [.. values];
        }

        /// <summary>
        /// Сделать поиск страницы по типу
        /// </summary>
        /// <typeparam name="T">Тип страницы поиска</typeparam>
        /// <returns>Найденная страница</returns>
        public PageBrowser? SearchAnyPageType(Type SourceType)
        {
            if (InlaysCount == 0 || SourceType.BaseType != typeof(PageBrowser)) return null;
            foreach (OPLInlay Inlay in SourceInlays)
            {
                if (Inlay.Content?.GetType() == SourceType)
                    return Inlay.Content;
            }
            return null;
        }

        /// <summary>
        /// Удалить вкладку в браузере
        /// </summary>
        /// <param name="inlay">Объект вкладки</param>
        /// <param name="ActivateNextInlay">Активировать ли следующую после удалённой вкладки вкладку</param>
        public void DeleteInlayPage(OPLInlay inlay, bool ActivateNextInlay = true)
        {
            if (SourceInlays.IndexOf(inlay) is int Index && Index == -1) return;
            int IndexNext = NextIndex(Index, InlaysCount - 1);
            OPLInlay ActualInlay = SourceInlays[Index];
            ActualInlay.Content?.Dispose();

            if (ManagerAnimation != null)
            {
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, ActualInlay, MarginProperty,
                    new Thickness(0), TimeSpan.FromMilliseconds(350d));
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, ActualInlay, OpacityProperty,
                    0d, TimeSpan.FromMilliseconds(350d));
                DoubleAnimation animationDouble = ManagerAnimation.GetCloneAnimationElementFromType<DoubleAnimation>();
                animationDouble.Duration = TimeSpan.FromMilliseconds(400d);
                animationDouble.From = ActualInlay.Width;
                animationDouble.To = 0d;
                animationDouble.FillBehavior = FillBehavior.Stop;
                animationDouble.Completed += (sender, e) =>
                {
                    ActualInlay.Width = 0d;
                    StackPanelInlays.Children.Remove(ActualInlay);
                };
                ActualInlay.BeginAnimation(WidthProperty, animationDouble);
            }
            else
            {
                //ActualInlay.Width = 0d;
                StackPanelInlays.Children.Remove(ActualInlay);
            }
            SourceInlays.RemoveAt(Index);

            if (ActivateNextInlay)
            {
                if (IndexNext == -1)
                {
                    if (SourceManagerAppPage == null) throw ExceptionManagerAppPage;
                    _ActivateIndex = -1;
                    MainPageController.ClosePage();
                    MainPageController.NextPage(SourceManagerAppPage, true);
                }
                else
                {
                    ActivateInlayIndex(IndexNext);
                }
            }
            else if (ActivateIndex >= Index) _ActivateIndex--;
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
