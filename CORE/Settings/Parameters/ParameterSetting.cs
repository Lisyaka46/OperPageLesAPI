using Newtonsoft.Json.Linq;
using OPLAPI.CORE.Settings.Base;
using System;
using System.Collections.Generic;
using System.Text;
using static OPLAPI.CORE.Settings.Interfaces.IParameterSettingBase;

namespace OPLAPI.CORE.Settings.Parameters
{
    /// <summary>
    /// Класс обычного параметра для категории настроек
    /// </summary>
    /// <typeparam name="T">Хранимый тип для настроек</typeparam>
    public class ParameterSetting<T> : ParameterSettingBase
    {
        /// <summary>
        /// Имя параметра настроек
        /// </summary>
        public new string ParameterName
        {
            get => base.ParameterName;
            set
            {
                base.ParameterName = value;
            }
        }

        /// <summary>
        /// Описание параметра настроек
        /// </summary>
        public new string? ParameterDescription
        {
            get => base.ParameterDescription;
            set
            {
                base.ParameterDescription = value;
            }
        }

        /// <summary>
        /// Значение параметра по умолчанию зависимого типа
        /// </summary>
        public new T DefaultValue => (T)base.DefaultValue;

        /// <summary>
        /// Значение параметра настроек зависимого типа
        /// </summary>
        public new T Value
        {
            get => (T)base.Value;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                base.Value = value;
            }
        }

        /// <summary>
        /// Событие изменения значения объекта настроек
        /// </summary>
        public new event ChangeValueHandler<T>? ValueChanged
        {
            add => base.ValueChanged += (Old, New) => value?.Invoke((T)Old, (T)New);
            remove => base.ValueChanged -= (Old, New) => value?.Invoke((T)Old, (T)New);
        }

        /// <summary>
        /// Инициализировать параметр настроек
        /// </summary>
        /// <param name="SourceDefaultValue">Значение по умолчанию</param>
        /// <exception cref="ArgumentNullException">Исключение при нулевом передаваемом значении</exception>
        public ParameterSetting(T SourceDefaultValue) : base(SourceDefaultValue ?? throw new ArgumentNullException(nameof(SourceDefaultValue)))
        {

        }

        /// <summary>
        /// Инициализировать параметр настроек с текущим значением
        /// </summary>
        /// <param name="SourceDefaultValue">Значение по умолчанию</param>
        /// <param name="SourceValue">Присваемое текущее значение</param>
        /// <exception cref="ArgumentNullException">Исключение при нулевом передаваемом значении</exception>
        public ParameterSetting(T SourceDefaultValue, T SourceValue) : 
            base(SourceDefaultValue ?? throw new ArgumentNullException(nameof(SourceDefaultValue)),
                 SourceValue ?? throw new ArgumentNullException(nameof(SourceValue)))
        {

        }

        /// <summary>
        /// Неявное преобразование хранимого типа данных в объект параметра
        /// </summary>
        /// <param name="value">Преобразовываемое значение</param>
        public static implicit operator ParameterSetting<T>(T value) => new(value);

        /// <summary>
        /// Неявное преобразование объекта параметра настроек в хранимый тип
        /// </summary>
        /// <param name="obj">Объект настроек</param>
        public static implicit operator T(ParameterSetting<T> obj) => obj.Value;
    }
}
