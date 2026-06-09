using OPLAPI.OIEL.UserElementsControl;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPLAPI.OIEL.CORE.Browser
{
    public class ApplicationPage
    {
        /// <summary>
        /// Визуальный элемент представления объекта
        /// </summary>
        public readonly OPLVisualElementIM VisualELement;

        /// <summary>
        /// Тип приложения страницы
        /// </summary>
        public readonly Type TypeBrowserAppPage;

        private string _Name;
        /// <summary>
        /// Имя страничного приложения
        /// </summary>
        public string Name
        {
            get => _Name;
            set
            {
                _Name = value;
                VisualELement.Text = _Name;
            }
        }

        /// <summary>
        /// Событие активации страничного приложения
        /// </summary>
        internal event EventHandler<ApplicationPage>? ApplicationPageActivate;

        /// <summary>
        /// Инициализировать данные о страничном приложении
        /// </summary>
        /// <param name="SourceTypeAppPage">Тип страничного приложения</param>
        /// <param name="NameAppPage">Отображаемое имя страничного приложения</param>
        public ApplicationPage(Type SourceTypeAppPage, string NameAppPage)
        {
            if (SourceTypeAppPage.BaseType != typeof(PageBrowser))
                throw new ArgumentException($"Недопустимый тип приложения страницы. Тип должен быть наследованным от {nameof(PageBrowser)}");
            TypeBrowserAppPage = SourceTypeAppPage;

            VisualELement = new()
            {
                Text = NameAppPage,
            };
            VisualELement.OnActivateMouseLeft += (sender, e) => ApplicationPageActivate?.Invoke(VisualELement, this);

            _Name = NameAppPage;
        }
    }
}
