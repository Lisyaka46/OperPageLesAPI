using Newtonsoft.Json;
using OPLAPI.CORE.Person;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace OPLAPI.CORE.Language
{
    /// <summary>
    /// Класс перевода
    /// </summary>
    public static class Lang
    {
        #region Data
        /// <summary>
        /// Главная директория файлов изображений
        /// </summary>
        internal static readonly string DirectoryLanguagesApplication = Settings.Setting.MainDirectoryApplication + @"Languages/";

        /// <summary>
        /// Главная директория файлов изображений, флагов языковых переводов
        /// </summary>
        public static readonly string DirectoryImageFlagsApplication = DirectoryLanguagesApplication + @"Images/";

        /// <summary>
        /// Информация о директории языковых переводов
        /// </summary>
        private static DirectoryInfo DirectoryLanguagesInfo = new(DirectoryLanguagesApplication);

        /// <summary>
        /// Имя параметра JSON для перевода, где хранится словарь
        /// </summary>
        private const string NameParameterTranslate = "Translation";

        /// <summary>
        /// Неизвестный перевод
        /// </summary>
        private const string InknownTranslate = "string.Empty";

        /// <summary>
        /// Активный текущий словарь
        /// </summary>
        private static Dictionary<string, Dictionary<ulong, string>> ActiveDictionaryLang = [];

        /// <summary>
        /// Активный установленный язык
        /// </summary>
        public static LangInfo ActiveLang { get; private set; } = LangInfo.Unknown;

        private static List<LangInfo> _InstalledLanguages = [];
        /// <summary>
        /// Установленные объекты перевода программы
        /// </summary>
        public static LangInfo[] InstalledLanguages => [.. _InstalledLanguages];

        #region Events
        /// <summary>
        /// Событие обновления списка языковых переводов
        /// </summary>
        public static event EventHandler? LanguageListUpdated;

        /// <summary>
        /// Событие обновления языкового перевода
        /// </summary>
        public static event EventHandler? LanguageUpdated;
        #endregion
        #endregion

        /// <summary>
        /// Взять строку перевода
        /// </summary>
        /// <param name="Key">Ключ строки перевода</param>
        /// <remarks>
        /// В качестве параметра принимается общий объект.<br/>
        /// Предполагается что в качестве ключа будет ожидаться тип <see cref="Enum"/> с определённым типом<br/>
        /// В зависимости от типа <see cref="Enum"/>, словарь перевода будет отличаться.<br/>
        /// При неизвестном типе, несоответствующем параметре, отсутствии числовой данной в словаре, будет выведена стандартная строка отсутствия перевода "string.Empty"
        /// </remarks>
        /// <returns>Строка, которая является переведённой для </returns>
        public static string GetValue(object Key)
        {
            Type KeyType = Key.GetType();
            if (Key == null || !KeyType.IsEnum) return InknownTranslate;
            string NameRootDictionary = KeyType.Name;
            if (ActiveDictionaryLang.TryGetValue(NameRootDictionary, out Dictionary<ulong, string>? SourceDictionary))
            {
                return SourceDictionary[(ulong)Key];
            }
            else return InknownTranslate;
        }

        /// <summary>
        /// Взять исключение указывающее на неудачу взятия параметра
        /// </summary>
        /// <param name="NameParameter">Имя параметра JSON</param>
        private static Exception GetExceptionInknownJSONParameter(string NameParameter) =>
            new($"Не удалось взять параметр JSON .{NameParameter}");

        /// <summary>
        /// Добавить новый вайл языкового перевода в директорию языковых переводов
        /// </summary>
        /// <remarks>
        /// Для имени файла используется параметр JSON <b>Locate</b> в корне JSON файла перевода
        /// </remarks>
        /// <param name="JSONContent">Содержимое JSON файла языкового перевода</param>
        public static LangInfo AppendNewLanguage(string JSONContent)
        {
            JsonDocument Document = JsonDocument.Parse(JSONContent);
            string NameFile = Document.RootElement.GetProperty("Locate").GetString() ??
                    throw GetExceptionInknownJSONParameter("Locate");
            string File = DirectoryLanguagesApplication + NameFile + ".json";
            System.IO.File.CreateText(File).Close();
            System.IO.File.WriteAllText(File, JSONContent);
            LangInfo? OriginInfo = _InstalledLanguages.FirstOrDefault((i) => i.Config.Locate.Equals(NameFile));
            if (OriginInfo == null)
                _InstalledLanguages.Add(GenerateLang(File));
            else
            {
                _InstalledLanguages.Remove(OriginInfo);
                _InstalledLanguages.Add(GenerateLang(File));
            }
            return OriginInfo ?? _InstalledLanguages[^1];
        }

        #region ManipulateListLanguages
        /// <summary>
        /// Получить установленный языковой перевод из списка
        /// </summary>
        /// <param name="Locate">Локализированное имя</param>
        public static LangInfo? GetLangFromLocate(string Locate) => 
            _InstalledLanguages.FirstOrDefault((i) => i.Config.Locate.Equals(Locate));

        /// <summary>
        /// Обновить список усановленных языковых переводов
        /// </summary>
        public static void UpdateListLanguages()
        {
            if (!DirectoryLanguagesInfo.Exists)
            {
                DirectoryLanguagesInfo.Create();
                return;
            }
            DirectoryLanguagesInfo.Refresh();
            List<string> PathLanguageFiles = [..DirectoryLanguagesInfo.EnumerateFiles().Select((i) => i.FullName)];
            LangInfo? Source;
            for (int i = 0; i < _InstalledLanguages.Count; i++)
            {
                Source = _InstalledLanguages.FirstOrDefault((j) => 
                    j.Config.Locate.Equals(Path.GetFileNameWithoutExtension(PathLanguageFiles[i])));
                if (Source == null) _InstalledLanguages.Add(GenerateLang(PathLanguageFiles[i]));
                else _InstalledLanguages[_InstalledLanguages.IndexOf(Source)] = GenerateLang(PathLanguageFiles[i]);
            }
            LanguageListUpdated?.Invoke(null, System.EventArgs.Empty);
        }

        /// <summary>
        /// Сгенерировать информационный объект о языковом переводе
        /// </summary>
        /// <param name="PathJSON">Директория JSON файла, языкового перевода</param>
        public static LangInfo GenerateLang(string PathJSON)
        {
            if (!Path.Exists(PathJSON) || !File.Exists(PathJSON))
                throw new Exception("Невозможно по данной директории найти JSON файл словарь перевода");
            string JSON = File.ReadAllText(PathJSON);
            JsonDocument Document = JsonDocument.Parse(JSON);
            LangInfo SourceInfo = new(JsonConvert.DeserializeObject<LangConfig>(JSON), PathJSON)
            {
                Name = Document.RootElement.GetProperty(nameof(LangInfo.Name)).Deserialize<string>() ??
                    throw GetExceptionInknownJSONParameter(nameof(LangInfo.Name)),
            };
            #region Autor
            try { SourceInfo.LangAutor.Name = Document.RootElement.GetProperty("Autor").Deserialize<string>() ?? Autor.UnknownAutor.Name; }
            catch { SourceInfo.LangAutor.Name = Autor.UnknownAutor.Name; }

            try { 
                SourceInfo.LangAutor.Contacts = [..Document.RootElement.GetProperty("Contacts").EnumerateArray().Select(element =>
                    {
                        string L, U, M;
                        L = element.GetProperty(nameof(Contact.Locate)).Deserialize<string>() ?? throw new Exception();
                        U = element.GetProperty(nameof(Contact.URL)).Deserialize<string>() ?? throw new Exception();
                        M = element.GetProperty(nameof(Contact.Mask)).Deserialize<string>() ?? throw new Exception();
                        return new Contact(L, U, M);
                    })]; }
            catch { SourceInfo.LangAutor.Contacts = Autor.UnknownAutor.Contacts; }
            #endregion
            return SourceInfo;
        }
        #endregion

        #region ManipulateDictionaryLang
        /// <summary>
        /// Обновить словарь языкового перевода по его информации
        /// </summary>
        /// <param name="Info">Информация о языковом переводе</param>
        /// <param name="InvalidKeys">Ключи которые являются неподдерживаемыми в языковом переводе</param>
        public static void UpdateLang(LangInfo Info, out string[] InvalidKeys)
        {
            string JSON = File.ReadAllText(Info.Path ??
                throw new Exception("Невозможно прочитать данный файл, так как путь к нему является нулевым"));
            JsonDocument Document = JsonDocument.Parse(JSON);
            ActiveLang = Info;
            ActiveDictionaryLang = AnalizeDictionaryLang(
                Assembly.GetCallingAssembly(), Document.RootElement.GetProperty(NameParameterTranslate), out InvalidKeys);
            LanguageUpdated?.Invoke(null, System.EventArgs.Empty);
        }

        /// <summary>
        /// Произвести анализ словаря перевода
        /// </summary>
        /// <param name="SourceAssemblyDataTypes">Сборка в которой хранятся типы для языкового перевода</param>
        /// <param name="JSONTranslateElement">параметр JSON в котором хранятся данные перевода</param>
        /// <param name="InvalidKeys">Ключи которые являются лишними для перевода</param>
        /// <returns>Словарь </returns>
        private static Dictionary<string, Dictionary<ulong, string>> AnalizeDictionaryLang(
            in Assembly SourceAssemblyDataTypes, in JsonElement JSONTranslateElement, out string[] InvalidKeys)
        {
            Type[] AssemblyTypes = SourceAssemblyDataTypes.GetTypes();
            List<string> ListInvalidKeys = [];
            Dictionary<string, Dictionary<ulong, string>> Result = [];
            ulong ValueTranslate = 0LU;
            Type TypeEnum;
            Dictionary<string, Dictionary<string, string>> SourceDictionaryJSONTranslate;
            try { SourceDictionaryJSONTranslate = JSONTranslateElement.Deserialize<Dictionary<string, Dictionary<string, string>>>() ?? []; }
            catch { SourceDictionaryJSONTranslate = []; }

            foreach (KeyValuePair<string, Dictionary<string, string>> Element in SourceDictionaryJSONTranslate)
            {
                try
                {
                    TypeEnum = AssemblyTypes.FirstOrDefault(t => t.Name.Equals(Element.Key)) ?? throw new Exception();
                    if (TypeEnum.GetEnumUnderlyingType() != ValueTranslate.GetType()) throw new Exception();
                    Result.Add(Element.Key, []);
                    foreach (KeyValuePair<string, string> OneTypeElement in Element.Value)
                    {
                        if (Enum.TryParse(TypeEnum, OneTypeElement.Key, out object? ValueEnum))
                        {
                            ValueTranslate = (ulong)ValueEnum;
                            Result[Element.Key].Add(ValueTranslate, OneTypeElement.Value);
                        }
                        else ListInvalidKeys.Add($"{Element.Key}.{OneTypeElement.Key}");
                    }
                }
                catch
                {
                    ListInvalidKeys.Add(Element.Key + ".*");
                }
            }
            InvalidKeys = [.. ListInvalidKeys];
            return Result;
        }
        #endregion
    }
}
