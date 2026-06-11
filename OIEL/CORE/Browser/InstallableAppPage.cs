using OPLAPI.CORE;
using OPLAPI.OIEL.CORE.Browser.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;

namespace OPLAPI.OIEL.CORE.Browser
{
    /// <summary>
    /// Класс устанавливаемого страничного приложения в OPL
    /// </summary>
    public sealed class InstallableAppPage : AppPage
    {
        /// <summary>
        /// Название страничного приложения
        /// </summary>
        public override string TitlePage { get; protected set; }

        /// <summary>
        /// Директория к файлу .dll страничного приложения
        /// </summary>
        private string PathFileAppPage;

        /// <summary>
        /// Событие активации страничного приложения
        /// </summary>
        public new event EventHandler<InstallableAppPage>? ApplicationPageActivate;

        /// <summary>
        /// Инициализировать данные о страничном приложении
        /// </summary>
        /// <param name="SourcePath">Директория к файлу станичного приложения .dll</param>
        public InstallableAppPage(string SourcePath) : base(typeof(PageBrowser), "Неизвестный")
        {
            if (!File.Exists(SourcePath) || !Path.GetExtension(SourcePath).Equals(".dll"))
                throw new Exception("Данный файл не найден или его расширение не подходит под устанавливаемое страничное приложение .dll ...");
            // Загружаем сборку
            Assembly AssemblyPage = Assembly.LoadFrom(SourcePath);

            // Получаем Type класса
            object ElementPage = AssemblyPage.CreateInstance("AppPageExample.MainPage") ??
                throw new Exception($"Неудалось создать экземпляр {typeof(OPLPageApplication).FullName}");

            //PropertyInfo PropertyMainPage = ElementPage.GetType().GetProperty(nameof(OPLPageApplication.MainPage)) ??
            //    throw new Exception("Не удалось получить свойство названия страничного приложения...");
            //object Page = PropertyMainPage.GetValue(ElementPage) ??
            //    throw new Exception("Не удалось получить объект отображения страничного приложения...");

            TitlePage = ((PageBrowser)ElementPage).Title ?? "Неизвестный";
            TypePage = ElementPage.GetType();
            PathFileAppPage = SourcePath;
            VisualELement.OnActivateMouseLeft += (sender, e) => ApplicationPageActivate?.Invoke(VisualELement, this);
            GC.KeepAlive(ElementPage);
        }

        /// <summary>
        /// Инициализировать страничное приложение
        /// </summary>
        internal PageBrowser InicializeAppPage() =>
            (PageBrowser)((Assembly.LoadFrom(PathFileAppPage).CreateInstance("AppPageExample.MainPage") ??
                throw new Exception($"Страничное приложение не наследует базовый класс отображения контента {typeof(PageBrowser).FullName}")) ??
                throw new Exception("Не удалось создать элемент страничного приложения..."));
    }
}
