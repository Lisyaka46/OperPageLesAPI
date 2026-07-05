using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using static OPLAPI.CORE.Settings.Interfaces.IParameterSettingBase;

namespace OPLAPI.CORE.Settings.Base
{
    /// <summary>
    /// Базовый класс категории настроек
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public abstract class CategorySettingBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Ключ категории настроек
        /// </summary>
        [JsonProperty]
        public string KeyCategory { get; }

        private string _NameCategory;
        /// <summary>
        /// Имя категории настроек
        /// </summary>
        public string NameCategory
        {
            get => _NameCategory;
            protected set
            {
                _NameCategory = value;
                OnPropertyChanged(nameof(NameCategory));
            }
        }

        /// <summary>
        /// хранимый тип перечисления ключей для категории параметров
        /// </summary>
        public Type TypeEnum { get; }

        /// <summary>
        /// Управляемый словарь параметрами настроек в категории
        /// </summary>
        [JsonProperty]
        protected Dictionary<uint, ParameterSettingBase> SourceParameters { get; private set; }

        #region AppendParameterEvent
        /// <summary>
        /// Класс данных события добавления нового параметра в категорию настроек
        /// </summary>
        public class AppendParameterEventArgs
        {
            /// <summary>
            /// Ключ перечисления
            /// </summary>
            public uint Key { get; internal set; }

            /// <summary>
            /// Новый параметр
            /// </summary>
            public ParameterSettingBase NewParameter { get; internal set; }

            /// <summary>
            /// Инициализировать класс параметров данных добавления нового параметра в категорию
            /// </summary>
            /// <param name="SourceKey">Ключ параметра</param>
            /// <param name="SourceNewParameter">Объект добавляемого параметра</param>
            internal AppendParameterEventArgs(uint SourceKey, ParameterSettingBase SourceNewParameter)
            {
                Key = SourceKey;
                NewParameter = SourceNewParameter;
            }
        }

        /// <summary>
        /// Обработчик события добавления нового параметра в категорию настроек
        /// </summary>
        /// <param name="sender">Объект категории, в которую добаляется параметр</param>
        /// <param name="e">Объект данных о добавляемом параметре</param>
        private void HandlerEventAppendParameter(object? sender, AppendParameterEventArgs e)
        {
            SourceParameters.Add(e.Key, e.NewParameter);
        }

        /// <summary>
        /// Событие добавления нового параметра в категорию
        /// </summary>
        public event EventHandler<AppendParameterEventArgs> ParameterAppend;
        #endregion

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
        /// Инициализировать новую пустую категорию настроек
        /// </summary>
        protected CategorySettingBase(Type SourceType, string Key)
        {
            TypeEnum = SourceType;
            KeyCategory = Key;
            _NameCategory = "string.Empty";
            SourceParameters = [];
            ParameterAppend += HandlerEventAppendParameter;
        }

        /// <summary>
        /// Добавить новый параметр в категорию
        /// </summary>
        /// <remarks>
        /// <i>Если в текущую категорию уже был добавлен параметр по данному ключу, то<br/>
        /// будет выведено исключение. Параметры в категории могут быть созданы только 1 раз.</i>
        /// </remarks>
        /// <param name="Key">Ключ доступа к параметру</param>
        /// <param name="SourceParameter">Добавляемый параметр</param>
        /// <exception cref="Exception"></exception>
        protected void AddParameter(uint Key, ParameterSettingBase SourceParameter)
        {
            if (SourceParameters.ContainsKey(Key))
                throw new Exception("Невозможно добавить новый параметр в категорию, так как по данному ключу уже содержится параметр");
            ParameterAppend.Invoke(this, new(Key, SourceParameter));
        }

        /// <summary>
        /// Взять объект параметра по его ключу
        /// </summary>
        /// <param name="Key">Числовой ключ параметра в категории</param>
        /// <returns></returns>
        public ParameterSettingBase GetParameter(uint Key) => SourceParameters[Key];
    }
}
