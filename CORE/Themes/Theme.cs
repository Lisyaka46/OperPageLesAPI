using IEL.CORE.Classes;
using IEL.CORE.Themes;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

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
        /// Установленные объекты тем
        /// </summary>
        public static string[] InstalledThemes { get; private set; } = [];

        /// <summary>
        /// Активная директория файла темы
        /// </summary>
        private static string ActiveDirectoryFileTheme = string.Empty;

        /// <summary>
        /// Словарь всех спектров палитры
        /// </summary>
        private static ThemeInfo ActiveTheme = ThemeInfo.UnknownTheme;

        /// <summary>
        /// Выделенный тип для принимаемого ключа спектра
        /// </summary>
        public static Type? SelectEnumSpectrumType
        {
            get => ActiveTheme.TypeEnumPalette;
            private set
            {
                if (value == null || (value.GetType().IsEnum && Enum.GetUnderlyingType(value) != IEL.CORE.Themes.Theme.EnumUnderlyingTypePalette))
                    ActiveTheme.TypeEnumPalette = value;
                else throw new ArgumentException("Невозможно выделить тип, который не подходит под (Enum : uint)");
            }
        }

        /// <summary>
        /// Выделить тип перечисления для спектров палитры
        /// </summary>
        /// <remarks>
        /// Изменяется поведение объектов. Количество спектров изменится относительно заданному типу
        /// </remarks>
        /// <param name="NameType">Имя поискового типа</param>
        /// <param name="SourceAssembly">Сборка в которой хранится тип</param>
        public static void SetSelectEnumSpectrumType(Assembly SourceAssembly, string NameType)
        {
            uint[] OldValuesEnumType = SelectEnumSpectrumType != null ? [.. Enum.GetValues(SelectEnumSpectrumType).Cast<uint>()] : [];
            Type[] AllTypesCallAssembly = SourceAssembly.GetTypes();
            if (SelectEnumSpectrumType == null || !SelectEnumSpectrumType.Name.Equals(NameType))
                SelectEnumSpectrumType = AllTypesCallAssembly.FirstOrDefault((i) => i.Name.Equals(NameType)) ??
                    throw new Exception($"Ожидаемый тип \"{NameType}\" не существует в сборке \"{SourceAssembly.FullName}\", " +
                    "которая вызвала этот метод");
            uint[] ValuesEnumType = [.. Enum.GetValues(SelectEnumSpectrumType).Cast<uint>()];
            foreach (uint Key in ValuesEnumType)
                if (!ActiveTheme.DictionaryPalette.TryAdd(Key, PaletteSpectrum.UnknownPaletteSpectrum))
                    ActiveTheme.DictionaryPalette[Key] = PaletteSpectrum.UnknownPaletteSpectrum;
            foreach (uint Key in OldValuesEnumType.Except(ValuesEnumType))
                ActiveTheme.DictionaryPalette.Remove(Key);
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
                return ActiveTheme.DictionaryPalette[(uint)Key];
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
            return ActiveTheme.DictionaryPalette.ContainsKey((uint)Key);
        }

        /// <summary>
        /// Обновить список тем
        /// </summary>
        public static void UpdateListThemes() => UpdateListThemes(true);

        /// <summary>
        /// Обновить список тем
        /// </summary>
        /// <param name="InvokeEvent">Вызывать событие обновление списка или нет</param>
        private static void UpdateListThemes(bool InvokeEvent)
        {
            DirectoryThemesInfo.Refresh();
            InstalledThemes = [.. DirectoryThemesInfo.GetFiles().Where((i) => i.Extension.Equals(IEL.CORE.Themes.Theme.ExtensionThemeFile))
                .Select((i) => i.FullName)];
            if (InvokeEvent) ThemeListUpdated?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        /// Обновить тему
        /// </summary>
        public static async Task UpdateTheme(string PathFileTheme)
        {
            FileInfo Info = new(PathFileTheme);
            if (!Info.Exists || !Info.Extension.Equals(IEL.CORE.Themes.Theme.ExtensionThemeFile))
                throw new ArgumentException("Невозможно установить тему, так как файл не существует или не соответствует расширению");
            byte[] BytesDataTheme = await File.ReadAllBytesAsync(Info.FullName);
            ActiveTheme.DictionaryPalette.Clear();
            ActiveTheme.Path = PathFileTheme;
            ActiveTheme.DictionaryPalette = GetDictionaryPalette(BytesDataTheme, out Type TypeSelect);
            ActiveTheme.TypeEnumPalette = TypeSelect;
            SelectEnumSpectrumType = TypeSelect;
            ThemeUpdated?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        /// Узнать тип пересисления спектров палитры в теме
        /// </summary>
        /// <param name="BytesDataStringType">Массив строки типа перечисления UTF8</param>
        public static Type? GetTypePalette(ReadOnlySpan<byte> BytesDataStringType)
        {
            string NameEnumType = Encoding.UTF8.GetString(BytesDataStringType);
            return IEL.CORE.Themes.Theme.GetEnumSpectrumType(Assembly.GetCallingAssembly(), NameEnumType) ??
                throw new Exception("Тип перечисления использующийся в теме не найден");
        }

        /// <summary>
        /// Создать словарь палитры спектров по байтам данных
        /// </summary>
        /// <param name="BytesDataFile">Данные палитры</param>
        /// <param name="TypeEnumTheme">Тип перечисления, который используется в теме</param>
        /// <returns>Объект словаря палитры спектров</returns>
        public static Dictionary<uint, PaletteSpectrum> GetDictionaryPalette(byte[] BytesDataFile, out Type TypeEnumTheme)
        {
            Dictionary<uint, PaletteSpectrum> Result = [];

            #region ReadType
            ushort CountBytesNameType = BitConverter.ToUInt16(BytesDataFile.AsSpan()[0..2]);
            BytesDataFile = BytesDataFile[2..];
            TypeEnumTheme = GetTypePalette(BytesDataFile.AsSpan()[0..CountBytesNameType]) ??
                throw new Exception("Тип перечисления использующийся в теме не найден");
            uint[] ValuesEnumType = [.. Enum.GetValues(TypeEnumTheme).Cast<uint>()];
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
        /// <param name="SourceStream">Поток файла</param>
        /// <param name="Key">Ключ спектра палитры</param>
        /// <param name="Spectrum">Записываемый спектр палитры</param>
        public static async Task WritePaletteSpectrum(FileStream SourceStream, object Key, PaletteSpectrum Spectrum)
        {
            if (!(SourceStream.CanWrite && SourceStream.CanRead))
                throw new ArgumentException("Невозможно обработать данные содержащиеся в потоке", nameof(SourceStream));
            else if (SelectEnumSpectrumType == null) throw new Exception("Не выделен тип перечисления ключей для палитры спектров темы");
            else if (!CheckValue(Key))
                throw new ArgumentException("Невозможно изменить данные спектра палитры, так как ключ не соответствует ожидаемому типу", nameof(Key));
            SourceStream.Seek(Encoding.UTF8.GetBytes(SelectEnumSpectrumType.Name).Length + Unsafe.SizeOf<ushort>(), SeekOrigin.Begin);
            byte[] Buffer;
            try
            {
                while (true)
                {
                    Buffer = new byte[4];
                    await SourceStream.ReadExactlyAsync(Buffer, 0, Buffer.Length);
                    if (BitConverter.ToUInt32(Buffer) == (uint)Key) break;
                }
            }
            catch
            {
                SourceStream.Seek(0L, SeekOrigin.End);
                Buffer = BitConverter.GetBytes((uint)Key);
                await SourceStream.WriteAsync(Buffer);
            }
            finally
            {
                await SourceStream.WriteAsync(Spectrum.BG.GetSourceBytes());
                await SourceStream.WriteAsync(Spectrum.BB.GetSourceBytes());
                await SourceStream.WriteAsync(Spectrum.FG.GetSourceBytes());
            }
        }

        /// <summary>
        /// Создать и записать данные темы в файл
        /// </summary>
        /// <remarks>
        /// Указатель в файле перемещается в самое начало после добавления данных о теме
        /// <code>FileStream.Seek(0L, SeekOrigin.Begin);</code>
        /// </remarks>
        /// <param name="NameTheme">Имя создаваемой темы</param>
        /// <param name="DirectoryOriginPallete">Директория опорного файла темы</param>
        /// <returns>Поток файла в котором содержится все данные</returns>
        public static async Task<FileStream> CreateNewTheme(string NameTheme, string? DirectoryOriginPallete = null)
        {
            UpdateListThemes(false);
            string OriginPathTheme;
            if (InstalledThemes.Any((i) => i.Equals(NameTheme)))
                throw new ArgumentException("Невозможно создать тему, так как тема с таким именем уже создана", nameof(NameTheme));
            else if (!InstalledThemes.Contains(DirectoryOriginPallete) && DirectoryOriginPallete != null)
                throw new Exception($"Невозможно найти попорный файл темы \"{DirectoryOriginPallete}\"");
            OriginPathTheme = DirectoryOriginPallete ?? ActiveTheme.Path;
            FileStream StreamNewTheme;
            if (OriginPathTheme.Length == 0)
            {
                if (ActiveTheme.TypeEnumPalette == null)
                    throw new Exception("Невозможно создать новую тему не выделив тип перечисления для спектров палитры");
                OriginPathTheme = $"{DirectoryThemesApplication}{NameTheme}{IEL.CORE.Themes.Theme.ExtensionThemeFile}";
                StreamNewTheme = new(OriginPathTheme, FileMode.Create, FileAccess.ReadWrite);
                byte[] NameTypeBytes = Encoding.UTF8.GetBytes(ActiveTheme.TypeEnumPalette.Name);
                await StreamNewTheme.WriteAsync(BitConverter.GetBytes((ushort)NameTypeBytes.Length));
                await StreamNewTheme.WriteAsync(NameTypeBytes);
                foreach (KeyValuePair<uint, PaletteSpectrum> Element in ActiveTheme.DictionaryPalette)
                {
                    await StreamNewTheme.WriteAsync(Element.Value.BG.GetSourceBytes());
                    await StreamNewTheme.WriteAsync(Element.Value.BB.GetSourceBytes());
                    await StreamNewTheme.WriteAsync(Element.Value.FG.GetSourceBytes());
                }
            }
            else
            {
                FileStream StreamRead = new(OriginPathTheme, FileMode.Open, FileAccess.Read);
                OriginPathTheme = $"{DirectoryThemesApplication}{NameTheme}{IEL.CORE.Themes.Theme.ExtensionThemeFile}";
                StreamNewTheme = new(OriginPathTheme, FileMode.Create, FileAccess.ReadWrite);
                await StreamRead.CopyToAsync(StreamNewTheme);
                StreamRead.Close();
                await StreamRead.DisposeAsync();
            }
            InstalledThemes = [.. InstalledThemes.Append(OriginPathTheme)];
            StreamNewTheme.Seek(0L, SeekOrigin.Begin);
            ThemeListUpdated?.Invoke(null, EventArgs.Empty);
            return StreamNewTheme;
        }
    }
}
