namespace OPLAPI.CORE.Settings.Interfaces
{
    /// <summary>
    /// Интерфейс объекта параметра настроек
    /// </summary>
    public interface IParameterSettingBase
    {
        /// <summary>
        /// Делегат события изменения параметра настроек
        /// </summary>
        /// <param name="OldValue">Старое значение параметра</param>
        /// <param name="NewValue">Новое значение параметра</param>
        public delegate void ChangeValueHandler<T>(T OldValue, T NewValue);

        /// <summary>
        /// Хранимое значение параметра по умолчанию
        /// </summary>
        public object DefaultValue { get; }

        /// <summary>
        /// Хранимое значение параметра
        /// </summary>
        public object Value { get; }
    }
}
