using IEL.UserElementsControl.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OPLAPI.OIEL.UserElementsControl.Base
{
    public class OPLNetworkElementViewerBase : IELContainerBase
    {
        /// <summary>
        /// Главный контейнер элемента
        /// </summary>
        private Grid MainContentElement;

        /// <summary>
        /// Главный барьер контента
        /// </summary>
        private Border ContentBorder;

        /// <summary>
        /// Объект текста отправителя
        /// </summary>
        private TextBlock TextBlockTitleSender;

        #region Properties

        #region Content
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly new DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(UIElement), typeof(OPLNetworkElementViewerBase),
                new(
                    (sender, e) =>
                    {
                        ((OPLNetworkElementViewerBase)sender).ContentBorder.Child = (UIElement)e.NewValue;
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

        #region BorderThicknessTop
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty BorderThicknessTopProperty =
            DependencyProperty.Register("BorderThicknessTop", typeof(int), typeof(OPLNetworkElementViewerBase),
                new(2,
                    (sender, e) =>
                    {
                        ((OPLNetworkElementViewerBase)sender).ContentBorder.BorderThickness = new(0, (int)e.NewValue, 0, 0);
                    }));

        /// <summary>
        /// Толщина границы объекта относительно вернего разрганичения
        /// </summary>
        public int BorderThicknessTop
        {
            get => (int)GetValue(BorderThicknessTopProperty);
            set
            {
                if (value < 0) return;
                SetValue(BorderThicknessTopProperty, value);
            }
        }
        #endregion

        #region SenderTextPoint
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty SenderTextPointProperty =
            DependencyProperty.Register("SenderTextPoint", typeof(string), typeof(OPLNetworkElementViewerBase),
                new(string.Empty,
                    (sender, e) =>
                    {
                        ((OPLNetworkElementViewerBase)sender).TextBlockTitleSender.Text = (string)e.NewValue;
                    }));

        /// <summary>
        /// Текст заголовка отправителя
        /// </summary>
        public string SenderTextPoint
        {
            get => (string)GetValue(SenderTextPointProperty);
            set => SetValue(SenderTextPointProperty, value);
        }
        #endregion

        #region SenderPadding
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty SenderPaddingProperty =
            DependencyProperty.Register("SenderPadding", typeof(Thickness), typeof(OPLNetworkElementViewerBase),
                new(new Thickness(0),
                    (sender, e) =>
                    {
                        ((OPLNetworkElementViewerBase)sender).TextBlockTitleSender.Padding = (Thickness)e.NewValue;
                    }));

        /// <summary>
        /// Смещение содержимого заголовка отправителя
        /// </summary>
        public Thickness SenderPadding
        {
            get => (Thickness)GetValue(SenderPaddingProperty);
            set => SetValue(SenderPaddingProperty, value);
        }
        #endregion

        //#region SendIndicatorMargin
        ///// <summary>
        ///// Данные конкретного свойства
        ///// </summary>
        //public static readonly DependencyProperty SendIndicatorMarginProperty =
        //    DependencyProperty.Register("SendIndicatorMargin", typeof(Thickness), typeof(OPLNetworkElementViewerBase),
        //        new(new Thickness(0),
        //            (sender, e) =>
        //            {
        //                ((OPLNetworkElementViewerBase)sender).ImageIndicatorSendComplete.Margin = (Thickness)e.NewValue;
        //            }));

        ///// <summary>
        ///// Смещение индикатора подтверждения отправки
        ///// </summary>
        //public Thickness SendIndicatorMargin
        //{
        //    get => (Thickness)GetValue(SendIndicatorMarginProperty);
        //    set => SetValue(SendIndicatorMarginProperty, value);
        //}
        //#endregion

        //#region SendIndicatorSource
        ///// <summary>
        ///// Данные конкретного свойства
        ///// </summary>
        //public static readonly DependencyProperty SendIndicatorSourceProperty =
        //    DependencyProperty.Register("SendIndicatorSource", typeof(ImageSource), typeof(OPLNetworkElementViewerBase),
        //        new(
        //            (sender, e) =>
        //            {
        //                ((OPLNetworkElementViewerBase)sender).ImageIndicatorSendComplete.Source = (ImageSource)e.NewValue;
        //            }));

        ///// <summary>
        ///// Отображаемое изображение индикатора
        ///// </summary>
        //public ImageSource SendIndicatorSource
        //{
        //    get => (ImageSource)GetValue(SendIndicatorSourceProperty);
        //    set => SetValue(SendIndicatorSourceProperty, value);
        //}
        //#endregion

        #endregion

        public OPLNetworkElementViewerBase()
        {
            MainContentElement = new();
            MainContentElement.RowDefinitions.Add(new() { Height = new(0d, GridUnitType.Auto) });
            MainContentElement.RowDefinitions.Add(new() { Height = new(1d, GridUnitType.Star) });
            MainContentElement.RowDefinitions.Add(new() { Height = new(0d, GridUnitType.Auto) });

            TextBlockTitleSender = new()
            {
                Padding = new(0),
            };
            Grid.SetRow(TextBlockTitleSender, 0);
            MainContentElement.Children.Add(TextBlockTitleSender);

            ContentBorder = new()
            {
                BorderThickness = new(0, 2, 0, 0),
                BorderBrush = SourceBorderBrush.SourceBrush,
            };
            Grid.SetRow(ContentBorder, 1);
            MainContentElement.Children.Add(ContentBorder);

            //ContainerIndicators = new()
            //{

            //};
            //Grid.SetRow(ContainerIndicators, 2);
            //MainContentElement.Children.Add(ContainerIndicators);
            //ContainerIndicators.ColumnDefinitions.Add(new() { Width = new(1d, GridUnitType.Star) });
            //ContainerIndicators.ColumnDefinitions.Add(new() { Width = new(0d, GridUnitType.Auto) });
            //ContainerIndicators.ColumnDefinitions.Add(new() { Width = new(0d, GridUnitType.Auto) });
            //ContainerIndicators.ColumnDefinitions.Add(new() { Width = new(0d, GridUnitType.Auto) });

            //ImageIndicatorSendComplete = new()
            //{
            //    Width = 20,
            //    Height = 20,
            //    Margin = new(0),
            //    Stretch = Stretch.Fill,
            //    StretchDirection = StretchDirection.Both,
            //    HorizontalAlignment = HorizontalAlignment.Right,
            //    VerticalAlignment = VerticalAlignment.Bottom,
            //};
            //Grid.SetColumn(ImageIndicatorSendComplete, 3);
            //ContainerIndicators.Children.Add(ImageIndicatorSendComplete);

            base.SetValue(IELContainerBase.ContentProperty, MainContentElement);
        }
    }
}
