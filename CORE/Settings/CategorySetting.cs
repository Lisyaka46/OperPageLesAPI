using OPLAPI.CORE.Settings.Base;
using OPLAPI.CORE.Settings.Interfaces;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace OPLAPI.CORE.Settings
{
    /// <summary>
    /// Класс категории настроек
    /// </summary>
    /// <typeparam name="T">Тип <b>ENUM</b> ключей по которым обрабатываются параметры категории</typeparam>
    public class CategorySetting<T> : CategorySettingBase where T : Enum
    {
        /// <summary>
        /// Имя категории настроек
        /// </summary>
        public new string NameCategory
        {
            get => base.NameCategory;
            set
            {
                base.NameCategory = value;
            }
        }

        /// <summary>
        /// Словарь всех параметров в текущей категории настроек
        /// </summary>
        public ReadOnlyDictionary<uint, ParameterSettingBase> Parameters => SourceParameters.AsReadOnly();

        /// <summary>
        /// Инициализировать новую пустую категорию настроек
        /// </summary>
        public CategorySetting(string Key) : base(typeof(T), Key)
        {
            if (Enum.GetUnderlyingType(typeof(T)) != typeof(uint))
                throw new ArgumentException($"Тип словаря параметров Enum ({typeof(T)}) не имеет доступный тип перечисления ключей (uint)");
        }

        /// <summary>
        /// Инициализировать новую пустую категорию настроек
        /// </summary>
        public CategorySetting(string Key, string Name) : base(typeof(T), Key)
        {
            if (Enum.GetUnderlyingType(typeof(T)) != typeof(uint))
                throw new ArgumentException($"Тип словаря параметров Enum ({typeof(T)}) не имеет доступный тип перечисления ключей (uint)");
            NameCategory = Name;
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
        public void AddParameter(T Key, ParameterSettingBase SourceParameter)
        {
            AddParameter(Convert.ToUInt32(Key), SourceParameter);
        }

        /// <summary>
        /// Взять из категории настроек параметр
        /// </summary>
        /// <param name="index">Преобразовываемое значение</param>
        public object this[T index] => SourceParameters[Convert.ToUInt32(index)];
    }
}
