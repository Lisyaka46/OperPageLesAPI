using OPLAPI.OIEL.CORE.Browser.Base;
using OPLAPI.OIEL.UserElementsControl;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace OPLAPI.OIEL.CORE.Browser
{
    /// <summary>
    /// Класс данных страничного приложения для отображения
    /// </summary>
    public class AppPage : AppPageBase
    {
        /// <summary>
        /// Визуальный элемент представления объекта
        /// </summary>
        public readonly OPLVisualElementIM VisualELement;

        private string _TitlePage = "Неизвестный";
        /// <summary>
        /// Имя страничного приложения
        /// </summary>
        public virtual string TitlePage
        {
            get => _TitlePage;
            protected set
            {
                _TitlePage = value;
                VisualELement?.Text = value;
            }
        }

        /// <summary>
        /// Инициализировать данные о страничном приложении
        /// </summary>
        /// <param name="SourceType">Тип страничного приложения</param>
        public AppPage(Type SourceType) : base(SourceType)
        {
            VisualELement = new();
            SetPropetriesFromObjectPage(SourceType);
            VisualELement.OnActivateMouseLeft += AppPage_Activate;
        }

        /// <summary>
        /// Инициализировать данные по умолчанию о страничном приложении
        /// </summary>
        protected internal AppPage() : base(typeof(PageBrowser))
        {
            VisualELement = new();
            VisualELement.OnActivateMouseLeft += AppPage_Activate;
        }

        /// <summary>
        /// Обработчик события активации страничного приложения
        /// </summary>
        private void AppPage_Activate(object Source, System.Windows.Input.MouseButtonEventArgs eventArgs)
        {
            ActivateAppPage(Source as UIElement ?? throw new Exception("Невозможно преобразовать нажатую иконку страничного приложения"));
        }

        /// <summary>
        /// Установить свойства страничного приложения из его главной страницы
        /// </summary>
        /// <param name="TypePage">Тип главной страницы страничного приложения</param>
        /// <param name="SourceAssembly">Сборка, содержащая тип главной страницы</param>
        protected internal void SetPropetriesFromObjectPage(Type TypePage, Assembly? SourceAssembly = null)
        {
            this.TypePage = TypePage;
            PropertyInfo PropetryTitle = TypePage.GetProperty(nameof(PageBrowser.Title)) ??
                throw new InvalidOperationException($"Главная страница страничного приложения не содержит обязательного свойства \"{nameof(PageBrowser.Title)}\"");
            PropertyInfo PropetryIcon = TypePage.GetProperty(nameof(PageBrowser.Icon)) ??
                throw new InvalidOperationException($"Главная страница страничного приложения не содержит обязательного свойства \"{nameof(PageBrowser.Icon)}\"");
            object ElementPage =
                (SourceAssembly != null ? SourceAssembly.CreateInstance(TypePage.FullName ?? string.Empty) : Activator.CreateInstance(TypePage)) ??
                throw new InvalidOperationException($"Неудалось создать экземпляр главной страницы страничного приложения \"{TypePage.FullName}\"");
            TitlePage = (string?)PropetryTitle.GetValue(ElementPage) ?? "Неизвестный";
            VisualELement.Source = (ImageSource?)PropetryIcon.GetValue(ElementPage);
        }
    }
}
