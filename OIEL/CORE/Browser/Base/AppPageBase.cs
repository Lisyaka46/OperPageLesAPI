using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;

namespace OPLAPI.OIEL.CORE.Browser.Base
{
    /// <summary>
    /// Базовый класс для всех страничных приложений
    /// </summary>
    public abstract class AppPageBase
    {
        /// <summary>
        /// Хранимый тип инициализации для страничного приложния
        /// </summary>
        public Type TypePage { get; protected set; }

        /// <summary>
        /// Исключение недопустимого типа для создания страничного приложения
        /// </summary>
        public static readonly ArgumentException ExceptionTypeAppPage =
            new($"Недопустимый тип приложения страницы. Тип должен быть наследованным от {typeof(PageBrowser).FullName}");

        /// <summary>
        /// Инициализировать базовое страничное приложение
        /// </summary>
        /// <param name="SourceType">Тип страничного приложения</param>
        protected AppPageBase(Type SourceType)
        {
            if (!CheckTypeFromAppPage(in SourceType)) throw ExceptionTypeAppPage;
            TypePage = SourceType;
        }

        /// <summary>
        /// Узнать, подходит ли тип под создание страничного приложения
        /// </summary>
        /// <param name="SourceType">Проверяемый тип</param>
        /// <returns></returns>
        public static bool CheckTypeFromAppPage(in Type? SourceType) =>
            SourceType == typeof(PageBrowser) || SourceType?.BaseType == typeof(PageBrowser);
    }
}
