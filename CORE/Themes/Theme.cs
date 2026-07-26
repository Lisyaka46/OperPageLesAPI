using IEL.CORE.Classes;
using OPLAPI.CORE.Language;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
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
        /// Главная директория файлов тем
        /// </summary>
        internal static readonly string DirectoryThemesApplication = Settings.Setting.MainDirectoryApplication + @"Themes/";

        /// <summary>
        /// Информация о директории файлов тем
        /// </summary>
        private static DirectoryInfo DirectoryThemesInfo = new(DirectoryThemesApplication);

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

        /// <summary>
        /// Установленные объекты тем
        /// </summary>
        public static string[] InstalledThemes { get; private set; } = [];

        private static Type? _SelectEnumSpectrumType = null;
        /// <summary>
        /// Выделенный тип для принимаемого ключа спектра
        /// </summary>
        public static Type? SelectEnumSpectrumType
        {
            get => _SelectEnumSpectrumType;
            set
            {
                if (value == null || (value.GetType().IsEnum && Enum.GetUnderlyingType(value) != typeof(uint)))
                    _SelectEnumSpectrumType = value;
                else throw new ArgumentException("Невозможно выделить тип, который не подходит под (Enum : uint)");
            }
        }

        #region Events
        /// <summary>
        /// Событие обновления списка файлов тем
        /// </summary>
        public static event EventHandler? ThemeListUpdated;

        /// <summary>
        /// Событие обновления темы
        /// </summary>
        public static event EventHandler? ThemeUpdated;
        #endregion

        /// <summary>
        /// Взять спектр темы
        /// </summary>
        /// <param name="Key">Ключ спектра</param>
        /// <remarks>
        /// В качестве параметра принимается общий объект.<br/>
        /// Предполагается что в качестве ключа будет ожидаться тип <see cref="Enum"/> с определённым типом<br/>
        /// При неизвестном типе, несоответствующем параметре, отсутствии числовой данной в словаре, будет выведен стандартный спектр темы<br/>
        /// Если <see cref="Theme.SelectEnumSpectrumType"/> = null; то всегда будет выводиться стандартный спектр темы
        /// </remarks>
        /// <returns>Спектр, который хранится в текущей теме</returns>
        public static PaletteSpectrum GetValue(object Key)
        {
            if (Key == null) return UnknownPaletteSpectrum;
            else if (Key.GetType() != SelectEnumSpectrumType) return UnknownPaletteSpectrum;
            try
            {
                return ActivePalette[(uint)Key];
            }
            catch
            {
                return UnknownPaletteSpectrum;
            }
        }

        /// <summary>
        /// Обновить список тем
        /// </summary>
        public static void UpdateListThemes()
        {
            DirectoryThemesInfo.Refresh();
            InstalledThemes = [.. DirectoryThemesInfo.GetFiles().Select((i) => i.FullName)];
            ThemeListUpdated?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        /// Обновить тему
        /// </summary>
        public static async Task UpdateTheme(string PathFileTheme)
        {
            FileInfo Info = new(PathFileTheme);
            if (SelectEnumSpectrumType == null)
                throw new Exception("Невозможно установить тему, так как не выделен тип перечисления");
            else if (!Info.Exists || !Info.Extension.Equals(".qd"))
                throw new ArgumentException("Невозможно установить тему, так как файл не существует или не соответствует расширению");
            byte[] BytesDataTheme = await File.ReadAllBytesAsync(Info.FullName);
            uint[] ValuesEnumType = [..Enum.GetValues(SelectEnumSpectrumType).Cast<uint>()];

            int CountChunks = BytesDataTheme.Length / PaletteSpectrum.CountQDataSpectrum;
            if (CountChunks > ValuesEnumType.Length)
            {
                BytesDataTheme = BytesDataTheme[..(PaletteSpectrum.CountQDataSpectrum * ValuesEnumType.Length)];
                CountChunks = ValuesEnumType.Length;
            }

            byte[][] ChunkDataTheme = new byte[CountChunks][];
            for (int i = 0; i < CountChunks; i++)
            {
                ChunkDataTheme[i] = new byte[PaletteSpectrum.CountQDataSpectrum];
                Array.Copy(BytesDataTheme, i * PaletteSpectrum.CountQDataSpectrum, ChunkDataTheme[i], 0, PaletteSpectrum.CountQDataSpectrum);
            }

            for (int i = 0; i < ValuesEnumType.Length; i++)
            {
                if (i > ChunkDataTheme.Length)
                    ActivePalette.Remove(ValuesEnumType[i]);
                else if (ActivePalette.ContainsKey(ValuesEnumType[i]))
                    ActivePalette[ValuesEnumType[i]] = new(ChunkDataTheme[i]);
                else
                    ActivePalette.Add(ValuesEnumType[i], new(ChunkDataTheme[i]));
            }
        }
    }
}
