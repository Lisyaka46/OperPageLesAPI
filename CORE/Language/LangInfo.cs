using OPLAPI.CORE.Person;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace OPLAPI.CORE.Language
{
    /// <summary>
    /// Класс информации о переводе
    /// </summary>
    public class LangInfo : INotifyPropertyChanged
    {
        /// <summary>
        /// Информация об пустом языке
        /// </summary>
        public static LangInfo Inknown => new(new(), string.Empty);

        /// <summary>
        /// Данные конфигурации перевода
        /// </summary>
        public readonly LangConfig Config;

        /// <summary>
        /// Директория файла
        /// </summary>
        public string Path { get; internal set; }

        private string _Name;
        /// <summary>
        /// Название перевода
        /// </summary>
        public string Name
        {
            get => _Name;
            set
            {
                _Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        /// <summary>
        /// Автор перевода
        /// </summary>
        public Autor LangAutor { get; internal set; }

        #region PropertyChanged
        /// <summary>
        /// Событие изменения свойства параметра
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Запустить событие изменения свойства объекта
        /// </summary>
        /// <param name="Name">Имя изменяемого свойства</param>
        protected void OnPropertyChanged([CallerMemberName] string? Name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        #endregion

        /// <summary>
        /// Инициализировать объект данных о переводе
        /// </summary>
        /// <param name="SourceConfig">Данные концигурации</param>
        /// <param name="SourcePath">Путь к файлу перевода</param>
        internal LangInfo(LangConfig SourceConfig, string SourcePath)
        {
            if (!File.Exists(SourcePath) && !SourcePath.Equals(string.Empty)) throw new ArgumentException("Невозможно найти файл языкового перевода", nameof(SourcePath));
            Config = SourceConfig;
            _Name = $"Language {SourceConfig.Locate}";
            Path = SourcePath;
            LangAutor = Autor.UnknownAutor;
        }
    }
}
