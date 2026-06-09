using Newtonsoft.Json;
using System.IO;

namespace OPLAPI.CORE
{
    /// <summary>
    /// Класс конвертации объектов в ожидаемый массив типов
    /// </summary>
    public class ConvertJSON
    {
        /// <summary>
        /// Функция перевода файла .json в ожидаемый объект.<br/>
        /// При отсутствии файла в директории создаёт новый экземпляр и возвращает предусматривающее значение объекта по умолчанию.<br/>
        /// <b>Такая логика задействуется также при ошибке перевода.</b>
        /// </summary>
        /// <typeparam name="T">Ожидаемый тип объекта.<br/><b>Преобразуется в массив ожидаемого типа</b></typeparam>
        /// <param name="PathJSON">Директория читаемого .json файла преобразовываемый в массив ожидаемого типа</param>
        public static T[] DeserializeObjectJson<T>(string PathJSON)
        {
            if (!File.Exists(PathJSON))
            {
                string DirectoryFolder = Path.GetDirectoryName(PathJSON) ?? string.Empty;
                if (!Directory.Exists(DirectoryFolder)) Directory.CreateDirectory(DirectoryFolder);
                File.WriteAllText(PathJSON, JsonConvert.SerializeObject(Array.Empty<T>()));
                return [];
            }
            return JsonConvert.DeserializeObject<T[]>(File.ReadAllText(PathJSON)) ?? [];
        }
    }

}
