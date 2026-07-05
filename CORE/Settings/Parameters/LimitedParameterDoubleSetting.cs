using Newtonsoft.Json;

namespace OPLAPI.CORE.Settings.Parameters
{
    /// <summary>
    /// Класс ограниченного числового <b>DOUBLE</b> параметра настроек
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class LimitedParameterDoubleSetting : ParameterSetting<double>
    {
        /// <summary>
        /// Минимальное допустимое значение для параметра
        /// </summary>
        [JsonProperty]
        public double MinimumValue { get; }

        /// <summary>
        /// Максимальное допустимое значение для параметра
        /// </summary>
        [JsonProperty]
        public double MaximumValue { get; }

        /// <summary>
        /// Значение параметра настроек зависимого типа
        /// </summary>
        /// <remarks>
        /// <i>Значение будет присваиваться при любом входном значении.<br/>
        /// Если входное значение не входит в ограничения параметра, то<br/>
        /// присваиваемое значение будет ограничиваться граничными значениями.<br/>
        /// В зависимости от того, превышает или наоборот уступает входное значение граничным.</i>
        /// </remarks>
        [JsonProperty]
        public new double Value
        {
            get => base.Value;
            set
            {
                base.Value = Math.Clamp(value, MinimumValue, MaximumValue);
            }
        }

        /// <summary>
        /// Инициализировать ограниченный параметр настроек
        /// </summary>
        /// <param name="SourceMinimumValue">Минимальное допустимое значение для параметра</param>
        /// <param name="SourceMaximumValue">Максимальное допустимое значение для параметра</param>
        /// <param name="SourceDefaultValue">Значение по умолчанию для параметра</param>
        /// <exception cref="Exception">Исключение несоответствия значения с ограничениями параметра</exception>
        public LimitedParameterDoubleSetting(double SourceMinimumValue, double SourceMaximumValue, double SourceDefaultValue) :
            base(SourceDefaultValue)
        {
            if (SourceDefaultValue < MinimumValue)
                throw new Exception("Значение по умолчанию не может быть меньше чем минимальное");
            else if (SourceDefaultValue > MaximumValue)
                throw new Exception("Значение по умолчанию не может быть больше чем максимальное");
            MinimumValue = SourceMinimumValue;
            MaximumValue = SourceMaximumValue;
        }
    }
}
