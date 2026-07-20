using System;
using System.Collections.Generic;
using System.Text;

namespace OPLAPI.CORE.Person
{
    public class Autor
    {
        /// <summary>
        /// Неизвестный автор
        /// </summary>
        public static Autor UnknownAutor => new("Unknown");

        /// <summary>
        /// Имя автора
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Контакты автора
        /// </summary>
        public Contact[] Contacts { get; internal set; }

        /// <summary>
        /// Создать автора
        /// </summary>
        /// <param name="SourceName">Наименование автора</param>
        internal Autor(string SourceName)
        {
            Name = SourceName;
            Contacts = [];
        }
    }
}
