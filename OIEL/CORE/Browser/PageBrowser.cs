using IEL.UserElementsControl.Base;
using OPLAPI.OIEL.CORE.Interfaces.Browser;
using OPLAnimation.CORE.Animation;
using OPLAnimation.CORE.Interfaces;
using System.Windows;
using System.Windows.Controls;

namespace OPLAPI.OIEL.CORE.Browser
{
    /// <summary>
    /// Класс отображаемой страницы в браузере
    /// </summary>
    public class PageBrowser : Page, IPageBrowser, IOPLAnimate
    {
        #region Title
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly new DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(PageBrowser),
                new(string.Empty,
                    (sender, e) =>
                    {
                    }));

        /// <summary>
        /// Текст заголовка
        /// </summary>
        public new string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
        #endregion

        #region Description
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(PageBrowser),
                new(string.Empty,
                    (sender, e) =>
                    {
                    }));

        /// <summary>
        /// Текст описания
        /// </summary>
        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }
        #endregion

        /// <summary>
        /// Данные менеджера анимаций
        /// </summary>
        protected OPLAnimationManager? SourceManagerAnimation;
        /// <summary>
        /// Объект менеджера анимаций настроек OPL
        /// </summary>
        public virtual OPLAnimationManager? ManagerAnimation
        {
            get => SourceManagerAnimation;
            set
            {
                SourceManagerAnimation = value;
            }
        }

        /// <summary>
        /// Cобытие фокусировки на элемент страницы браузера
        /// </summary>
        public new event IPageBrowser.BrowserEventHandler? GotFocus;

        /// <summary>
        /// Cобытие отключения фокусировки на элемент страницы браузера
        /// </summary>
        public new event IPageBrowser.BrowserEventHandler? LostFocus;

        /// <summary>
        /// Событие отключение/закрытия/удаления страницы из браузера
        /// </summary>
        public event IPageBrowser.BrowserEventHandler? Disposed;

        /// <summary>
        /// Инициализация класса браузерной страницы
        /// </summary>
        public PageBrowser()
        {
            base.SetValue(Page.ContentProperty, Content);
        }

        /// <summary>
        /// Очистка ресурсов под событие очистки
        /// </summary>
        public void Dispose()
        {
            Disposed?.Invoke(this);
            GC.SuppressFinalize(this);
        }
    }
}
