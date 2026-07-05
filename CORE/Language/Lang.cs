using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Windows.Input;

namespace OPLAPI.CORE.Language
{
    /// <summary>
    /// Класс перевода
    /// </summary>
    public static class Lang
    {
        #region Data
        /// <summary>
        /// Имя параметра JSON для перевода
        /// </summary>
        private const string NameParameterTranslate = "Translation";

        /// <summary>
        /// Словарь с пустыми значениями перевода
        /// </summary>
        private static Dictionary<EnumLanguage, string> EmptyDictionaryLang =
            Enum.GetValues<EnumLanguage>().Cast<EnumLanguage>().ToDictionary(
                key => key,
                value => "string.Empty"
            );

        /// <summary>
        /// Активный текущий словарь
        /// </summary>
        public static Dictionary<EnumLanguage, string> ActiveDictionaryLang = new([.. EmptyDictionaryLang]);

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
        /// <returns>Строка, которая является переведённой для </returns>
        public static string GetValue(EnumLanguage Key) => ActiveDictionaryLang[Key];

        /// <summary>
        /// Взять исключение указывающее на неудачу взятия параметра
        /// </summary>
        /// <param name="NameParameter">Имя параметра JSON</param>
        private static Exception GetExceptionInknownJSONParameter(string NameParameter) =>
            new($"Не удалось взять параметр JSON .{NameParameter}");

        #region PreInstalled
        /// <summary>
        /// Активировать предустановленный язык (ru-ru)
        /// </summary>
        public static void ActivatePreInstalledDictionaryLang()
        {
            ActiveDictionaryLang = new()
            {
                { EnumLanguage.HeadNameProgram, "OperPageLes" },
                { EnumLanguage.MainWindowTitle, "Главное меню" },
            };
            LanguageUpdated?.Invoke(null, EventArgs.Empty);
        }
        #endregion

        #region ManipulateListLanguages
        /// <summary>
        /// Обновить список усановленных языковых переводов
        /// </summary>
        public static void UpdateListLanguages()
        {
            for (int i = 0; i < _InstalledLanguages.Count; i++)
            {
                if (!File.Exists(_InstalledLanguages[i].Path))
                    _InstalledLanguages.RemoveAt(i);
                else
                {
                    GenerateLang(_InstalledLanguages[i].Path);
                }
            }
            LanguageListUpdated?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        /// Сгенерировать информационный объект о языковом переводе
        /// </summary>
        /// <param name="PathJSON">Директория JSON файла, языкового перевода</param>
        private static LangInfo GenerateLang(string PathJSON)
        {
            if (!Path.Exists(PathJSON) || !File.Exists(PathJSON))
                throw new Exception("Невозможно по данной директории найти JSON файл словарь перевода");
            string JSON = File.ReadAllText(PathJSON);
            JsonDocument Document = JsonDocument.Parse(JSON);
            LangInfo SourceInfo = new(JsonConvert.DeserializeObject<LangConfig>(JSON))
            {
                Path = PathJSON,
                Name = Document.RootElement.GetProperty(nameof(LangInfo.Name)).GetString() ??
                    throw GetExceptionInknownJSONParameter(nameof(LangInfo.Name))
            };
            return SourceInfo;
        }
        #endregion

        #region ManipulateDictionaryLang
        /// <summary>
        /// Обновить словарь языкового перевода
        /// </summary>
        /// <param name="KeyValuePairs">Текущий словарь языкового перевода</param>
        public static void UpdateDictionaryLang(ref ReadOnlyDictionary<EnumLanguage, string> KeyValuePairs)
        {
            ActiveDictionaryLang = new([.. KeyValuePairs]);
            LanguageUpdated?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        /// Получить словарь языкового перевода по его информации
        /// </summary>
        /// <param name="Info">Информация о языковом переводе</param>
        /// <param name="InvalidKeys">Ключи которые являются неподдерживаемыми в языковом переводе</param>
        public static ReadOnlyDictionary<EnumLanguage, string> GetDictionaryLang(LangInfo Info, out string[] InvalidKeys)
        {
            string JSON = File.ReadAllText(Info.Path ??
                throw new Exception("Невозможно прочитать данный файл, так как путь к нему является нулевым"));
            JsonDocument Document = JsonDocument.Parse(JSON);
            Dictionary<string, string> RootDictionary =
                Document.RootElement.GetProperty(NameParameterTranslate).Deserialize<Dictionary<string, string>>() ??
                    throw GetExceptionInknownJSONParameter(NameParameterTranslate);
            return AnalizeDictionaryLang(in RootDictionary, out InvalidKeys).AsReadOnly();
        }

        /// <summary>
        /// Произвести анализ словаря перевода
        /// </summary>
        /// <param name="DictionaryLang">Словарь ключей JSON для перевода</param>
        /// <param name="InvalidKeys">Ключи которые являются лишними для перевода</param>
        /// <returns>Словарь </returns>
        private static Dictionary<EnumLanguage, string> AnalizeDictionaryLang(
            in Dictionary<string, string> DictionaryLang, out string[] InvalidKeys)
        {
            List<string> ListInvalidKeys = [];
            Dictionary<EnumLanguage, string> Result = [];
            string[] Keys = Enum.GetNames<EnumLanguage>();
            foreach (KeyValuePair<string, string> Element in DictionaryLang)
            {
                if (Keys.Contains(Element.Key))
                    Result[Enum.Parse<EnumLanguage>(Element.Key)] = Element.Value;
                else
                    ListInvalidKeys.Add(Element.Key);
            }
            InvalidKeys = [.. ListInvalidKeys];
            return Result;
        }
        #endregion
    }
}
