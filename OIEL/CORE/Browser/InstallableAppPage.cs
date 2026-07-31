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
        /// Инициализировать пустой объект устанавливаемого приложения
        /// </summary>
        private InstallableAppPage()
        {

        }

        /// <summary>
        /// Инициализировать данные о страничном приложении
        /// </summary>
        /// <remarks>
        /// Установочный файл должен быть файлом .dll, который содержит в себе тип, наследуемый от <see cref="PageBrowser"/><br/>
        /// Он будет являться главной страницей страничного приложения.<br/>
        /// Свойства <see cref="PageBrowser.Title"/> и <see cref="PageBrowser.Icon"/> будут использоваться для отображения информации об устанавливаемом страничном приложении
        /// </remarks>
        /// <param name="SourcePath">Директория к файлу станичного приложения .dll</param>
        public static async Task<InstallableAppPage> GetInstallableAppPage(string SourcePath)
        {
            if (!File.Exists(SourcePath) || !Path.GetExtension(SourcePath).Equals(".dll"))
                throw new FileNotFoundException("Данный файл не найден или его расширение не подходит под устанавливаемое страничное приложение .dll ...");
            Type SourceType = GetTypeAppPage(await File.ReadAllBytesAsync(SourcePath), out Assembly SourceAssembly);
            InstallableAppPage AppPage = new();
            AppPage.SetPropetriesFromObjectPage(SourceType, SourceAssembly);
            return AppPage;
        }

        /// <summary>
        /// Получить тип устанавливаемого страничного приложения<br/>
        /// Унаследованный от <see cref="PageBrowser"/>
        /// </summary>
        /// <param name="SourceData">Данные файла dll</param>
        /// <param name="OutAssembly">Сборка файла</param>
        private static Type GetTypeAppPage(byte[] SourceData, out Assembly OutAssembly)
        {
            OutAssembly = Assembly.Load(SourceData);
            Type[] PageBrowserExportedTypes = [.. OutAssembly.ExportedTypes.Where((i) => i.BaseType == typeof(PageBrowser))];
            if (PageBrowserExportedTypes.Length == 0)
                throw new InvalidOperationException("Неудалось получить тип главной страницы страничного приложения");
            else if (PageBrowserExportedTypes.Length > 1)
                throw new InvalidOperationException(
                    $"Загружаемое страничное приложение содержит множество типов наследованных от \"{nameof(PageBrowser)}\".\n" +
                    $"В загружаемом страничном приложении должен быть только один тип \"{nameof(PageBrowser)}\"");
            return PageBrowserExportedTypes[0];
        }
    }
}
