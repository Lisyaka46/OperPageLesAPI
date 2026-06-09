namespace OPLAPI.OIEL.CORE.Interfaces.Browser
{
    /// <summary>
    /// Интерфейс элемента браузера страниц
    /// </summary>
    public interface IPageBrowser : IDisposable
    {
        /// <summary>
        /// Делегат обычного события реализуемое браузером
        /// </summary>
        /// <param name="Source">Элемент который вызвал данное событие</param>
        public delegate void BrowserEventHandler(IPageBrowser Source);

        /// <summary>
        /// Событие отключения фокуса на элемент страницы браузера
        /// </summary>
        public event BrowserEventHandler? LostFocus;

        /// <summary>
        /// Событие добавления фокуса на элемент страницы браузера
        /// </summary>
        public event BrowserEventHandler? GotFocus;

        /// <summary>
        /// Событие отключение/закрытия/удаления страницы из браузера
        /// </summary>
        protected event BrowserEventHandler? Disposed;

        /// <summary>
        /// Название вкладки браузера страниц
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Описание вкладки браузера страниц
        /// </summary>
        public string Description { get; set; }
    }
}
