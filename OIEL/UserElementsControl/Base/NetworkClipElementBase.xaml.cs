using IEL.CORE.Classes;
using IEL.UserElementsControl;
using IEL.UserElementsControl.Base;
using Newtonsoft.Json.Linq;
using OPLAPI.CORE.Animation;
using OPLAPI.CORE.Interfaces;
using System.Windows;
using System.Windows.Controls;

namespace OPLAPI.OIEL.UserElementsControl.Base
{
    /// <summary>
    /// Объект отображающий вложенный объект
    /// </summary>
    public class NetworkClipElementBase : IELContainerBase, IOPLAnimate
    {
        #region UIElements
        /// <summary>
        /// Главный контейнер
        /// </summary>
        private Grid MainGrid;

        /// <summary>
        /// Кнопка открепления элемента
        /// </summary>
        private IELButtonText ButtonUnClip;

        /// <summary>
        /// Барьерный конткйнер содержимого
        /// </summary>
        protected Border BorderContent;
        #endregion

        #region Properties

        #region Content
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly new DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(UIElement), typeof(NetworkClipElementBase),
                new(
                    (sender, e) =>
                    {
                        ((NetworkClipElementBase)sender).BorderContent.Child = (UIElement)e.NewValue;
                    }));

        /// <summary>
        /// Внутренний элемент объекта
        /// </summary>
        public new UIElement Content
        {
            get => (UIElement)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }
        #endregion

        #region PaletteElement
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly new DependencyProperty PaletteElementProperty =
            DependencyProperty.Register("PaletteElement", typeof(PaletteSpectrum), typeof(NetworkClipElementBase),
                new(new PaletteSpectrum(),
                    (sender, e) =>
                    {
                        PaletteSpectrum Value = (PaletteSpectrum)e.NewValue;
                        ((NetworkClipElementBase)sender).ButtonUnClip.PaletteElement = Value;
                        ((IELContainerBase)sender).PaletteElement = Value;
                    }));

        /// <summary>
        /// Объект палитры
        /// </summary>
        public new PaletteSpectrum PaletteElement
        {
            get => (PaletteSpectrum)GetValue(PaletteElementProperty);
            set
            {
                SetValue(PaletteElementProperty, value);
            }
        }
        #endregion

        #region NumberIndex
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty NumberIndexProperty =
            DependencyProperty.Register("NumberIndex", typeof(uint), typeof(NetworkClipElementBase),
                new(0u,
                    (sender, e) =>
                    {
                        ((NetworkClipElementBase)sender).ButtonUnClip.Text = ((uint)e.NewValue).ToString();
                    }));

        /// <summary>
        /// Отображаемый индекс прикреплённого элемента
        /// </summary>
        public uint NumberIndex
        {
            get => (uint)GetValue(NumberIndexProperty);
            set
            {
                SetValue(NumberIndexProperty, value);
            }
        }
        #endregion

        #region IsVisibleIndex
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty IsVisibleIndexProperty =
            DependencyProperty.Register("IsVisibleIndex", typeof(bool), typeof(NetworkClipElementBase),
                new(true,
                    (sender, e) =>
                    {
                        OPLAnimationManager.AnimateTakingZeroTo(((IOPLAnimate)sender).ManagerAnimation, ((NetworkClipElementBase)sender).ButtonUnClip, WidthProperty,
                            (bool)e.NewValue ? 35d : 0d, TimeSpan.FromMilliseconds(500d));
                    }));

        /// <summary>
        /// Отображается ли индекс
        /// </summary>
        public bool IsVisibleIndex
        {
            get => (bool)GetValue(IsVisibleIndexProperty);
            set
            {
                SetValue(IsVisibleIndexProperty, value);
            }
        }
        #endregion

        #endregion

        /// <summary>
        /// Состояние активности взаимодействия с файлом
        /// </summary>
        public bool IsManipulate { get; protected set; } = false;

        /// <summary>
        /// Объект менеджера анимационных настроек OPL
        /// </summary>
        public OPLAnimationManager? ManagerAnimation { get; set; }

        /// <summary>
        /// Событие открепления элемента
        /// </summary>
        public event EventHandler? UnClipElement;

        /// <summary>
        /// Инициализировать объект отображающий вложение файла
        /// </summary>
        public NetworkClipElementBase()
        {
            IsAnimatedSettingQ = false;
            IsEnabledSettingQ = false;

            #region MainGrid
            MainGrid = new();
            MainGrid.ColumnDefinitions.Add(new() { Width = new(0d, GridUnitType.Auto) });
            MainGrid.ColumnDefinitions.Add(new() { Width = new(1d, GridUnitType.Star) });
            base.Content = MainGrid;
            #endregion

            #region ButtonUnClip
            ButtonUnClip = new()
            {
                Width = 35d,
                Height = 30d,
                HeightViewBox = 40d,
                Text = "0",
                PaletteElement = PaletteElement,
                Padding = new(2d),
                BorderThickness = new(0d, 2d, 2d, 2d),
                CornerRadius = new(0d, 5d, 5d, 0d),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };
            ButtonUnClip.OnActivateMouseLeft += (sender, e) => UnClipElement?.Invoke(sender, e);
            Grid.SetColumn(ButtonUnClip, 0);
            MainGrid.Children.Add(ButtonUnClip);
            #endregion

            #region BorderContent
            BorderContent = new()
            {
                BorderBrush = SourceBorderBrush.SourceBrush,

            };

            Grid.SetColumn(BorderContent, 1);
            MainGrid.Children.Add(BorderContent);
            #endregion
        }
    }
}
