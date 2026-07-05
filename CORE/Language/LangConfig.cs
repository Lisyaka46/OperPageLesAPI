using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPLAPI.CORE.Language
{
    /// <summary>
    /// Класс содержащий информацию о переводе
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public readonly record struct LangConfig
    {
        /// <summary>
        /// Версия перевода
        /// </summary>
        [JsonProperty]
        public string Version { get; }

        /// <summary>
        /// Локальное название для перевода (ru-ru)
        /// </summary>
        [JsonProperty]
        public string Locate { get; }
    }
}
