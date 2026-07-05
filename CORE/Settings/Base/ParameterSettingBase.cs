using Microsoft.Windows.Themes;
using Newtonsoft.Json;
using OPLAPI.CORE.Settings.Interfaces;
using OPLAPI.CORE.Settings.Parameters;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using static OPLAPI.CORE.Settings.Interfaces.IParameterSettingBase;

namespace OPLAPI.CORE.Settings.Base
{
    /// <summary>
    /// Общий класс объекта караметра настроек
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public abstract class ParameterSettingBase : IParameterSettingBase, INotifyPropertyChanged
    {
        private string _ParameterName;
        /// <summary>
        /// Имя параметра настроек
        /// </summary>
        public string ParameterName
        {
            get => _ParameterName;
            protected set
            {
                _ParameterName = value;
                OnPropertyChanged(nameof(ParameterName));
            }
        }

        private string? _ParameterDescription;
        /// <summary>
        /// Описание параметра настроек
        /// </summary>
        public string? ParameterDescription
        {
            get => _ParameterDescription;
            protected set
            {
                _ParameterDescription = value;
                OnPropertyChanged(nameof(ParameterDescription));
            }
        }

        /// <summary>
        /// Хранимый тип значения для параметра
        /// </summary>
        public Type TypeParameterValue { get; }

        /// <summary>
        /// Значение по умолчанию
        /// </summary>
        public object DefaultValue { get; }

        private object? _Value;
        /// <summary>
        /// Значение параметра настроек
        /// </summary>
        [JsonProperty]
        public object Value
        {
            get => _Value ?? DefaultValue;
            protected set
            {
                if (TypeParameterValue != value.GetType())
                    throw new Exception($"Недопустимый тип присвоения значения для парапметра с типом ({TypeParameterValue.Name})");
                ValueChanged?.Invoke(Value, value);
                OnPropertyChanged(nameof(Value));
                _Value = value;
            }
        }

        #region PropertyChanged
        /// <summary>
        /// Событие изменения значения объекта настроек
        /// </summary>
        public event ChangeValueHandler<object>? ValueChanged;

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
        /// Инициализировать общий объект параметра настроек
        /// </summary>
        /// <param name="SourceDefaultValue">Значение по умолчанию для параметра</param>
        /// <param name="ValueSet">Устанавливать ли значение текущего параметра</param>
        protected ParameterSettingBase(object SourceDefaultValue, bool ValueSet = true)
        {
            TypeParameterValue = SourceDefaultValue.GetType();
            DefaultValue = SourceDefaultValue;
            _Value = ValueSet ? SourceDefaultValue : null;
            _ParameterName = $"Parameter is {SourceDefaultValue.GetType().Name} type";
            _ParameterDescription = null;
        }

        /// <summary>
        /// Инициализировать общий объект параметра настроек
        /// </summary>
        /// <param name="SourceDefaultValue">Значение по умолчанию для параметра</param>
        /// <param name="SourceValue">Устанавливаемое текущее значение параметра</param>
        protected ParameterSettingBase(object SourceDefaultValue, object SourceValue)
        {
            TypeParameterValue = SourceDefaultValue.GetType();
            DefaultValue = SourceDefaultValue;
            _Value = SourceValue;
            _ParameterName = $"Parameter is {SourceDefaultValue.GetType().Name} type";
            _ParameterDescription = null;
        }

        /// <summary>
        /// Присвоить значение параметру настроек
        /// </summary>
        /// <remarks>
        /// Данная функция позволяет присвоить значение параметру настроек,<br/>
        /// при этом проверяя соответствие типов присваиваемого значения и хранимого значения параметра.
        /// </remarks>
        /// <typeparam name="T">Присваемый тип параметра</typeparam>
        /// <param name="Parameter">Объект параметра, которому присваивается значение</param>
        /// <param name="NewValue">Новое значение параметра</param>
        public static void SetValue<T>(object Parameter, T NewValue)
        {
            if (NewValue == null) throw new ArgumentNullException(nameof(NewValue));
            Type TypeValue = NewValue.GetType();
            Type g = Parameter.GetType();
            Type OriginTypeParameter = typeof(ParameterSetting<>).MakeGenericType(TypeValue);
            if (g != OriginTypeParameter && g.BaseType != OriginTypeParameter)
                throw new ArgumentException($"Тип хранимого значения не соответствует типу нового параметра", nameof(NewValue));
            else if (TypeValue == typeof(double) && Parameter.GetType() == typeof(LimitedParameterDoubleSetting))
            {
                ((LimitedParameterDoubleSetting)Parameter).Value = Convert.ToDouble(NewValue);
            }
            else if (TypeValue == typeof(int) && Parameter.GetType() == typeof(LimitedParameterIntSetting))
            {
                ((LimitedParameterIntSetting)Parameter).Value = Convert.ToInt32(NewValue);
            }
            else
            {
                PropertyInfo PropertyValue = OriginTypeParameter.GetProperty(
                    "Value", BindingFlags.Public | BindingFlags.Instance, null, TypeValue, Type.EmptyTypes, null) ??
                    throw new Exception($"Не удалось получить свойство Value для типа \"{OriginTypeParameter.Name}\"");
                PropertyValue.SetValue(Parameter, NewValue);
            }
        }
    }
}
