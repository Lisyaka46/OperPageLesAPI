using IEL.CORE.Classes;
using OPLAPI.CORE.Language;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using WnColor = System.Windows.Media.Color;

namespace OPLAPI.CORE.Themes
{
    /// <summary>
    /// Главный класс управления темами
    /// </summary>
    public static class Theme
    {
        /// <summary>
        /// Словарь всех 
        /// </summary>
        private static Dictionary<uint, PaletteSpectrum> ActivePalette = [];

        /// <summary>
        /// Активная директория файла темы
        /// </summary>
        private static string ActiveDirectoryFileTheme = string.Empty;

        /// <summary>
        /// Значение неизвестного спектра палитры
        /// </summary>
        private static readonly PaletteSpectrum UnknownPaletteSpectrum = new()
        {
            BG = new(Colors.White, Colors.Gray, Colors.LightGray, Colors.DarkRed),
            BB = new(Colors.Black, Colors.DarkGray, Colors.Gray, Colors.Black),
            FG = new(Colors.Black, Colors.Black, Colors.DarkCyan, Colors.Black),
        };

        private static List<string> _InstalledThemes = [];
        /// <summary>
        /// Установленные объекты тем
        /// </summary>
        public static string[] InstalledThemes => [.. _InstalledThemes];
    }
}
