using OPLAPI.CORE;
using OPLAPI.OIEL.CORE.Browser.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace OPLAPI.OIEL.CORE.Browser
{
    /// <summary>
    /// Класс устанавливаемого страничного приложения
    /// </summary>
    public sealed class InstallableAppPage : AppPage
    {
        /// <summary>
        /// Сборка страничного рпиложения
        /// </summary>
        private Assembly AssemblyPage;

        /// <summary>
        /// Событие активации страничного приложения
        /// </summary>
        public new event EventHandler<InstallableAppPage>? ApplicationPageActivate;

        /// <summary>
        /// Инициализировать данные о страничном приложении
        /// </summary>
        /// <remarks>
        /// Установочный файл должен быть файлом .dll, который содержит в себе тип, наследуемый от PageBrowser.<br/>
        /// Он будет являться главной страницей страничного приложения.<br/>
        /// Свойства Title и Icon главной страницы будут использоваться для отображения информации об устанавливаемом страничном приложении в менеджере приложений страниц OPL.
        /// </remarks>
        /// <param name="SourcePath">Директория к файлу станичного приложения .dll</param>
        public InstallableAppPage(string SourcePath) : base()
        {
            if (!File.Exists(SourcePath) || !Path.GetExtension(SourcePath).Equals(".dll"))
                throw new Exception("Данный файл не найден или его расширение не подходит под устанавливаемое страничное приложение .dll ...");

            AssemblyPage = Assembly.LoadFrom(SourcePath);
            Type[] AssemplyTypesPage = [.. AssemblyPage.ExportedTypes];
            Type TypeMainPage = AssemplyTypesPage.FirstOrDefault((i) => i.BaseType == typeof(PageBrowser)) ??
                throw new Exception("Неудалось получить тип главной страницы страничного приложения");
            SetPropetriesFromObjectPage(TypeMainPage, AssemblyPage);

            VisualELement.OnActivateMouseLeft += (sender, e) => ApplicationPageActivate?.Invoke(VisualELement, this);
        }
    }
}
