using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPLAPI.CORE.Person
{
    /// <summary>
    /// Объект контакта
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class Contact
    {
        /// <summary>
        /// Локализация контакта, название сети
        /// </summary>
        [JsonProperty]
        public string Locate { get; internal set; }

        /// <summary>
        /// Ссылка на контакт
        /// </summary>
        [JsonProperty]
        public string URL { get; internal set; }

        /// <summary>
        /// Маска для отображения ссылки на контакт <i>(например, @username)</i>
        /// </summary>
        [JsonProperty]
        public string Mask { get; internal set; }

        /// <summary>
        /// Инициализировать объект контакта
        /// </summary>
        internal Contact(string SourceLocate, string SourceURL, string SourceMask)
        {
            Locate = SourceLocate;
            URL = SourceURL;
            Mask = SourceMask;
        }
    }
}
