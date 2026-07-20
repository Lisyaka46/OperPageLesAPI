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
    public record struct LangConfig
    {
        /// <summary>
        /// Версия перевода
        /// </summary>
        [JsonProperty]
        public string Version { get; internal set; }

        /// <summary>
        /// Локальное название для перевода (Russian)
        /// </summary>
        [JsonProperty]
        public string Locate { get; internal set; }
    }
}
