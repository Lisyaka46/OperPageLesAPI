using IEL.CORE.Themes;
using LibraryIEL.CORE.Themes.Palettes;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input;

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
        /// Числовой тип, который используется для обозначения длинны имени типа в файле темы <code>USHORT</code>
        /// </summary>
        public static readonly Type EnumTypeNameLength = typeof(ushort);

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
                if (value == null || (!value.IsEnum && Enum.GetUnderlyingType(value) != IEL.CORE.Themes.Theme.EnumUnderlyingTypePalette))
                    throw new ArgumentException("Невозможно выделить тип, который не подходит под (Enum : uint)");
                else ActiveTheme.TypeEnumPalette = value;
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
                if (!ActiveTheme.DictionaryPalette.TryAdd(Key, PaletteData.UnknownPaletteData))
                    ActiveTheme.DictionaryPalette[Key] = PaletteData.UnknownPaletteData;
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
        public static PaletteData GetValue(object Key)
        {
            if (Key == null) return PaletteData.UnknownPaletteData;
            else if (Key.GetType() != SelectEnumSpectrumType) return PaletteData.UnknownPaletteData;
            try
            {
                return new(ActiveTheme.DictionaryPalette[(uint)Key]);
            }
            catch
            {
                return PaletteData.UnknownPaletteData;
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
        /// Создать словарь палитры спектров по байтам данных
        /// </summary>
        /// <param name="BytesDataFile">Данные палитры</param>
        /// <param name="TypeEnumTheme">Тип перечисления, который используется в теме</param>
        /// <returns>Объект словаря палитры спектров</returns>
        public static Dictionary<uint, byte[]> GetDictionaryPalette(Span<byte> BytesDataFile, out Type TypeEnumTheme)
        {
            Dictionary<uint, byte[]> Result;
            int Offset = 0, LengthByte;

            LengthByte = Marshal.SizeOf(EnumTypeNameLength);
            ushort CountBytesNameType = BitConverter.ToUInt16(BytesDataFile.Slice(Offset, LengthByte));
            Offset += LengthByte;

            string NameEnumType = Encoding.UTF8.GetString(BytesDataFile.Slice(Offset, CountBytesNameType));
            Offset += CountBytesNameType;
            TypeEnumTheme = IEL.CORE.Themes.Theme.GetEnumSpectrumType(Assembly.GetCallingAssembly(), NameEnumType) ??
                throw new Exception("Тип перечисления использующийся в теме не найден");
            uint[] ValuesEnumType = [.. Enum.GetValues(TypeEnumTheme).Cast<uint>()];
            Result = new(ValuesEnumType.Length);
            foreach (uint Key in ValuesEnumType)
                Result[Key] = PaletteData.UnknownPaletteData;

            LengthByte = Marshal.SizeOf(IEL.CORE.Themes.Theme.EnumUnderlyingTypePalette);
            uint SourceKey;
            while (Offset < BytesDataFile.Length)
            {
                SourceKey = BitConverter.ToUInt32(BytesDataFile.Slice(Offset, LengthByte));
                Offset += LengthByte;
                Result[SourceKey] = BytesDataFile.Slice(Offset, PaletteData.CountBytes).ToArray();
                Offset += PaletteData.CountBytes;
            }

            return Result;
        }

        /// <summary>
        /// Записать в поток данных файла данные <see cref="PaletteSpectrum"/>
        /// </summary>
        /// <param name="SourceStream">Поток файла</param>
        /// <param name="Key">Ключ спектра палитры</param>
        /// <param name="SourceData">Записываемый спектр палитры</param>
        public static void WritePaletteSpectrum(FileStream SourceStream, object Key, PaletteData SourceData)
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
                    SourceStream.ReadExactly(Buffer, 0, Buffer.Length);
                    if (BitConverter.ToUInt32(Buffer) == (uint)Key) break;
                }
            }
            catch
            {
                SourceStream.Seek(0L, SeekOrigin.End);
                Buffer = BitConverter.GetBytes((uint)Key);
                SourceStream.Write(Buffer);
            }
            finally
            {
                SourceStream.Write(SourceData.BackGroundData);
                SourceStream.Write(SourceData.BorderGroundData);
                SourceStream.Write(SourceData.ForeGroundData);
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
                foreach (KeyValuePair<uint, byte[]> Element in ActiveTheme.DictionaryPalette)
                {
                    await StreamNewTheme.WriteAsync(Element.Value);
                    await StreamNewTheme.WriteAsync(Element.Value);
                    await StreamNewTheme.WriteAsync(Element.Value);
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
