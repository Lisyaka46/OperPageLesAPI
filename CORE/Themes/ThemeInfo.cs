using IEL.CORE.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace IEL.CORE.Themes
{
    /// <summary>
    /// Класс информации о палитре
    /// </summary>
    public class ThemeInfo
    {
        /// <summary>
        /// Объект пустой/неизвестной темы
        /// </summary>
        public static ThemeInfo UnknownTheme => new();

        /// <summary>
        /// Директория файла темы
        /// </summary>
        public string Path { get; internal set; }

        /// <summary>
        /// Словарь спектров палитры
        /// </summary>
        internal Dictionary<uint, PaletteSpectrum> DictionaryPalette { get; set; }

        /// <summary>
        /// Тип перечисления спектров палитры для темы
        /// </summary>
        internal Type? TypeEnumPalette { get; set; }

        /// <summary>
        /// Инициализировать пустой объект информации о теме
        /// </summary>
        private ThemeInfo()
        {
            Path = string.Empty;
            DictionaryPalette = [];
            TypeEnumPalette = null;
        }
    }
}
