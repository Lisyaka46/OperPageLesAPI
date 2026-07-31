using OPLAPI.OIEL.CORE.Browser;
using OPLAPI.OIEL.UserElementsControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace OPLAPI.CORE.Browser
{
    /// <summary>
    /// Объект вкладки для страничного браузера
    /// </summary>
    /// <remarks>
    /// Используется для обозначений объектов вкладок для <see cref="OPLAPI.OIEL.UserElementsControl.OPLBrowserPage"/>
    /// </remarks>
    public class Inlay : INotifyPropertyChanged, IDisposable
    {
        private string _Title;
        /// <summary>
        /// Заголовок вкладки
        /// </summary>
        public string Title
        {
            get => _Title;
            set
            {
                _Title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        private string _Description;
        /// <summary>
        /// Заголовок вкладки
        /// </summary>
        public string Description
        {
            get => _Description;
            set
            {
                _Description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        private OPLInlay? _Visual = null;
        /// <summary>
        /// Визуальный объект, отображающий вкладку
        /// </summary>
        internal OPLInlay Visual
        {
            get => _Visual ?? throw new Exception("Недопустимое пустое состояние отображаемой вкладки");
            set
            {
                if (value != null)
                {
                    value.Title = _Title;
                    value.MouseLeftButtonUp += Inlay_Activated;
                    value.CloseInlay += Inlay_Closed;
                    value.MouseHover += Inlay_MouseHover;
                    value.MouseLeave += Inlay_MouseLeave;
                }
                if (_Visual != null)
                {
                    _Visual.MouseLeftButtonUp -= Inlay_Activated;
                    _Visual.CloseInlay -= Inlay_Closed;
                    _Visual.MouseHover -= Inlay_MouseHover;
                    _Visual.MouseLeave -= Inlay_MouseLeave;
                }
                _Visual = value;
            }
        }

        private PageBrowser? _ContentPage = null;
        /// <summary>
        /// Визуальный объект содержимого вкладки
        /// </summary>
        internal PageBrowser ContentPage
        {
            get => _ContentPage ?? throw new Exception("Недопустимое пустое состояние для содержимого контента вкладки");
            set => _ContentPage = value;
        }

        #region PropertyChanged
        /// <summary>
        /// Событие изменения свойства параметра
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Запустить событие изменения свойства объекта
        /// </summary>
        /// <param name="Name">Имя изменяемого свойства</param>
        protected void OnPropertyChanged([CallerMemberName] string? Name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        #endregion

        /// <summary>
        /// Событие активации вкладки
        /// </summary>
        internal event EventHandler<Inlay>? Activated;

        /// <summary>
        /// Событие задержки мыши на элементе вкладки
        /// </summary>
        public event EventHandler<Inlay>? MouseHover;

        /// <summary>
        /// Событие скрытия мыши с элемента вкладки
        /// </summary>
        public event EventHandler<Inlay>? MouseLeave;

        /// <summary>
        /// Событие закрытия вкладки
        /// </summary>
        public event EventHandler<Inlay>? Closed;

        /// <summary>
        /// Инициализировать пустой объект вкладки
        /// </summary>
        private Inlay()
        {
            _Title = string.Empty;
            _Description = string.Empty;
            PropertyChanged += Inlay_PropertyChanged;
        }

        /// <summary>
        /// Инициализировать объект вкладки
        /// </summary>
        /// <param name="Content">Страница содержимого</param>
        internal static Inlay InicializeInlay(PageBrowser Content)
        {
            Inlay Result = new()
            {
                Visual = new()
                {
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    BorderThickness = new(1.5d),
                    CornerRadius = new(12),
                    Margin = new(2),
                    Opacity = 0d,
                    IsAnimatedSettingQ = false,
                    IsEnabledSettingQ = false,
                    IsEnabled = true,
                },
                ContentPage = Content,
                Title = Content.Title,
            };
            return Result;
        }

        /// <summary>
        /// Выгрузить все ресурсы из элемента вкладки
        /// </summary>
        public void Dispose()
        {
            if (_ContentPage != null)
            {
                ContentPage.Dispose();
            }
            if (_Visual != null)
            {
                Visual.MouseLeftButtonUp -= Inlay_Activated;
                Visual.CloseInlay -= Inlay_Closed;
                Visual.MouseHover -= Inlay_MouseHover;
                Visual.MouseLeave -= Inlay_MouseLeave;
            }
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Обработчик события изменения свойства объекта вкладки
        /// </summary>
        private void Inlay_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Title):
                    Visual?.Title = Title;
                    break;
            }
        }

        /// <summary>
        /// Обработчик события активации вкладки
        /// </summary>
        private void Inlay_Activated(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OPLInlay Element = (OPLInlay)sender;
            if (!Element.IsEnabled) return;
            Element.SourceBackground.UsedState = true;
            Activated?.Invoke(sender, this);
        }

        /// <summary>
        /// Обработчик события задержки мыши на элементе
        /// </summary>
        private void Inlay_MouseHover(object? sender, EventArgs e)
        {
            MouseHover?.Invoke(sender, this);
        }

        /// <summary>
        /// Обработчик события скрытия мыши с элемента
        /// </summary>
        private void Inlay_MouseLeave(object? sender, EventArgs e)
        {
            MouseLeave?.Invoke(sender, this);
        }

        /// <summary>
        /// Обработчик события закрытия вкладки
        /// </summary>
        private void Inlay_Closed(object? sender, OPLInlay e)
        {
            e.SourceBackground.UsedState = false;
            e.IsEnabled = false;
            Closed?.Invoke(sender, this);
        }
    }
}
