using OPLAPI.OIEL.CORE.Browser.Base;
using OPLAPI.OIEL.UserElementsControl;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPLAPI.OIEL.CORE.Browser
{
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
        /// Событие активации страничного приложения
        /// </summary>
        public event EventHandler<AppPage>? ApplicationPageActivate;

        /// <summary>
        /// Инициализировать данные о страничном приложении
        /// </summary>
        /// <param name="SourceType">Тип страничного приложения</param>
        /// <param name="SourceTitle">Отображаемое имя страничного приложения</param>
        public AppPage(Type SourceType, string SourceTitle) : base(SourceType)
        {
            VisualELement = new()
            {
                Text = SourceTitle,
            };
            VisualELement.OnActivateMouseLeft += (sender, e) => ApplicationPageActivate?.Invoke(VisualELement, this);
        }
    }
}
