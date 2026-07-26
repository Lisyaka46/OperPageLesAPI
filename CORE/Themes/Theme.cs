using IEL.CORE.Classes;
using OPLAPI.CORE.Language;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
        /// Расширение файлов тем
        /// </summary>
        internal static readonly string ExtensionThemeFile = ".qd";

        /// <summary>
        /// Словарь всех 
        /// </summary>
        private static Dictionary<uint, PaletteSpectrum> ActivePalette = [];

        /// <summary>
        /// Активная директория файла темы
        /// </summary>
        private static string ActiveDirectoryFileTheme = string.Empty;

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
            private set
            {
                if (value == null || (value.GetType().IsEnum && Enum.GetUnderlyingType(value) != typeof(uint)))
                    _SelectEnumSpectrumType = value;
                else throw new ArgumentException("Невозможно выделить тип, который не подходит под (Enum : uint)");
            }
        }

        /// <summary>
        /// Выделить тип перечисления для спектров палитры
        /// </summary>
        /// <param name="NameType">Имя поискового типа</param>
        /// <param name="SourceAssembly">Сборка в которой хранится тип</param>
        public static void SetSelectEnumSpectrumType(Assembly SourceAssembly, string NameType)
        {
            Type[] AllTypesCallAssembly = SourceAssembly.GetTypes();
            if (SelectEnumSpectrumType == null || !SelectEnumSpectrumType.Name.Equals(NameType))
                SelectEnumSpectrumType = AllTypesCallAssembly.FirstOrDefault((i) => i.Name.Equals(NameType)) ??
                    throw new Exception($"Ожидаемый тип \"{NameType}\" не существует в сборке \"{SourceAssembly.FullName}\", " +
                    "которая вызвала этот метод");
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
            if (Key == null) return PaletteSpectrum.UnknownPaletteSpectrum;
            else if (Key.GetType() != SelectEnumSpectrumType) return PaletteSpectrum.UnknownPaletteSpectrum;
            try
            {
                return ActivePalette[(uint)Key];
            }
            catch
            {
                return PaletteSpectrum.UnknownPaletteSpectrum;
            }
        }

        /// <summary>
        /// Узнать, содержится ли ключ спектра в палитре
        /// </summary>
        /// <param name="Key">Ключ спектра</param>
        /// <remarks>
        /// В качестве параметра принимается общий объект.<br/>
        /// Предполагается что в качестве ключа будет ожидаться тип <see cref="Enum"/> с определённым типом<br/>
        /// При неизвестном типе, несоответствующем параметре, отсутствии числовой данной в словаре, будет выведен стандартный спектр темы<br/>
        /// Если <see cref="Theme.SelectEnumSpectrumType"/> = null; то всегда будет выводиться стандартный спектр темы
        /// </remarks>
        /// <returns>Содержится ли ключ в палитре</returns>
        public static bool CheckValue(object Key)
        {
            if (Key == null) return false;
            else if (Key.GetType() != SelectEnumSpectrumType) return false;
            return ActivePalette.ContainsKey((uint)Key);
        }

        /// <summary>
        /// Обновить список тем
        /// </summary>
        public static void UpdateListThemes()
        {
            DirectoryThemesInfo.Refresh();
            InstalledThemes = [.. DirectoryThemesInfo.GetFiles().Where((i) => i.Extension.Equals(ExtensionThemeFile)).Select((i) => i.Name)];
            ThemeListUpdated?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        /// Обновить тему
        /// </summary>
        public static async Task UpdateTheme(string PathFileTheme)
        {
            FileInfo Info = new(PathFileTheme);
            if (!Info.Exists || !Info.Extension.Equals(ExtensionThemeFile))
                throw new ArgumentException("Невозможно установить тему, так как файл не существует или не соответствует расширению");
            byte[] BytesDataTheme = await File.ReadAllBytesAsync(Info.FullName);
            ActivePalette.Clear();
            ActivePalette = GetDictionaryPalette(BytesDataTheme);
            ThemeUpdated?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        /// Создать словарь палитры спектров по байтам данных
        /// </summary>
        /// <param name="BytesDataFile">Данные палитры</param>
        /// <param name="SetSelectType">Выделить ли хранящийся тип в данных</param>
        /// <returns>Объект словаря палитры спектров</returns>
        private static Dictionary<uint, PaletteSpectrum> GetDictionaryPalette(byte[] BytesDataFile, bool SetSelectType = true)
        {
            Dictionary<uint, PaletteSpectrum> Result = [];

            #region ReadType
            ushort CountBytesNameType = BitConverter.ToUInt16(BytesDataFile.AsSpan()[0..2]);
            BytesDataFile = BytesDataFile[2..];
            string NameEnumType = Encoding.UTF8.GetString(BytesDataFile.AsSpan()[0..CountBytesNameType]);
            BytesDataFile = BytesDataFile[CountBytesNameType..];
            if (SetSelectType) SetSelectEnumSpectrumType(Assembly.GetCallingAssembly(), NameEnumType);
            else if (SelectEnumSpectrumType == null) throw new Exception("Выделенный тип для палитры спектров не установлен");
#pragma warning disable CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
            uint[] ValuesEnumType = [.. Enum.GetValues(SelectEnumSpectrumType).Cast<uint>()];
#pragma warning restore CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
            #endregion

            int CountChunks = BytesDataFile.Length / PaletteSpectrum.CountQDataSpectrum;
            if (CountChunks > ValuesEnumType.Length)
            {
                BytesDataFile = BytesDataFile[..(PaletteSpectrum.CountQDataSpectrum * ValuesEnumType.Length)];
                CountChunks = ValuesEnumType.Length;
            }

            byte[][] ChunkDataTheme = new byte[CountChunks][];
            for (int i = 0; i < CountChunks; i++)
            {
                ChunkDataTheme[i] = new byte[PaletteSpectrum.CountQDataSpectrum];
                Array.Copy(BytesDataFile, i * PaletteSpectrum.CountQDataSpectrum, ChunkDataTheme[i], 0, PaletteSpectrum.CountQDataSpectrum);
            }

            for (int i = 0; i < ValuesEnumType.Length; i++)
                Result.Add(ValuesEnumType[i], new(ChunkDataTheme[i]));
            return Result;
        }

        /// <summary>
        /// Записать в поток данных файла данные <see cref="PaletteSpectrum"/>
        /// </summary>
        /// <param name="Stream">Поток файла</param>
        /// <param name="Spectrum">Элемент палитры, который записывается в файл</param>
        /// <returns></returns>
        /// <exception cref="Exception">Исключение несоответствия режима открытия файла</exception>
        public static void WritePalettespectrum(ref FileStream Stream, ref PaletteSpectrum Spectrum)
        {
            if (!Stream.CanWrite) throw new Exception("Поток работы с файлом не открыт для записи!");
            List<byte> BytesFromPaletteSpectrum = [];
            BytesFromPaletteSpectrum.AddRange(Spectrum.BG.GetSourceBytes());
            BytesFromPaletteSpectrum.AddRange(Spectrum.BB.GetSourceBytes());
            BytesFromPaletteSpectrum.AddRange(Spectrum.FG.GetSourceBytes());
            Stream.Write([.. BytesFromPaletteSpectrum], 0, BytesFromPaletteSpectrum.Count);
        }

        /// <summary>
        /// Создать и записать данные темы в файл
        /// </summary>
        /// <remarks>
        /// Указатель в файле перемещается в самое начало после добавления данных о теме
        /// <code>FileStream.Seek(0L, SeekOrigin.Begin);</code>
        /// </remarks>
        /// <param name="NameTheme">Имя создаваемой темы</param>
        /// <param name="OriginPallete">Палитра, на основе которой содаётся тема</param>
        /// <returns>Поток файла в котором содержится все данные</returns>
        public static async Task<FileStream> CreateNewTheme(string NameTheme, Dictionary<uint, PaletteSpectrum>? OriginPallete = null)
        {
            UpdateListThemes();
            if (InstalledThemes.Any((i) => i.Equals(NameTheme)))
                throw new ArgumentException("Невозможно создать тему, так как тема с таким именем уже создана", nameof(NameTheme));
            DirectoryThemesInfo.EnumerateFiles("*" + ExtensionThemeFile);
            Dictionary<uint, PaletteSpectrum> SourcePallete = OriginPallete ?? ActivePalette;
            if (SelectEnumSpectrumType == null)
                throw new Exception("Невозможно создать тему, так как не выделен тип перечисления для палитры спектров");
            uint[] ValuesEnumType = [.. Enum.GetValues(SelectEnumSpectrumType).Cast<uint>()];
            FileStream Result = File.Create($"{DirectoryThemesApplication}{NameTheme}{ExtensionThemeFile}");
            byte[] BytesData;
            PaletteSpectrum Spectrum;

            #region WriteNameType
            BytesData = BitConverter.GetBytes((ushort)SelectEnumSpectrumType.Name.Length);
            await Result.WriteAsync(BytesData);
            BytesData = Encoding.UTF8.GetBytes(SelectEnumSpectrumType.Name);
            await Result.WriteAsync(BytesData);
            #endregion

            foreach (uint Key in ValuesEnumType)
            {
                if (SourcePallete.TryGetValue(Key, out var value)) Spectrum = value;
                else Spectrum = PaletteSpectrum.UnknownPaletteSpectrum;
                await Result.WriteAsync(Spectrum.BG.GetSourceBytes());
                await Result.WriteAsync(Spectrum.BB.GetSourceBytes());
                await Result.WriteAsync(Spectrum.FG.GetSourceBytes());
            }
            Result.Seek(0L, SeekOrigin.Begin);
            return Result;
        }
    }
}
