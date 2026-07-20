using OPLAPI.CORE.Settings.Base;

namespace OPLAPI.CORE.Settings
{
    /// <summary>
    /// Главный класс управления настройками
    /// </summary>
    public static class Setting
    {
        #region DirectoryResources
        /// <summary>
        /// Главная директория ресурсов OperPageLes
        /// </summary>
        internal static readonly string MainDirectoryApplication = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"/OperPageLes/";
        #endregion

        #region Category
        /// <summary>
        /// Словарь всех категорий настроек
        /// </summary>
        private static Dictionary<string, CategorySettingBase> SourceCategories = [];

        #region AppendCategoryEvent
        /// <summary>
        /// Событие добавления новой категории
        /// </summary>
        public static event EventHandler<CategorySettingBase> AppendCategory = HandlerEventAppendCategory;

        /// <summary>
        /// Обработчик события добавления категории настроек
        /// </summary>
        /// <param name="sender">Объект настроек добавляющий категорию, Нулевой</param>
        /// <param name="e">Объект данных о добавляемой категории</param>
        private static void HandlerEventAppendCategory(object? sender, CategorySettingBase e)
        {
            SourceCategories.Add(e.KeyCategory, e);
        }
        #endregion

        /// <summary>
        /// Добавить новую категорию в настройки
        /// </summary>
        /// <param name="NewCategory">Добавляемая категория</param>
        public static void AddCategory(CategorySettingBase NewCategory)
        {
            AppendCategory.Invoke(null, NewCategory);
        }

        /// <summary>
        /// Взять объект категории из настроек
        /// </summary>
        /// <param name="CategoryName">Название поисковой категории настроек</param>
        /// <returns></returns>
        public static CategorySettingBase GetCategory(string CategoryName) => SourceCategories[CategoryName];

        /// <summary>
        /// Установить всем параметрам в категории значения
        /// </summary>
        /// <remarks>
        /// Если указать массив с избыточным количеством значений параметров, то будет выведено исключение
        /// </remarks>
        /// <param name="KeyCategory">Ключ категории</param>
        /// <param name="Values">Значения каждого параметра в этой категории</param>
        public static void SetAllParametersInCategory(string KeyCategory, object[] Values)
        {
            CategorySettingBase SourceCategory = SourceCategories[KeyCategory];
            uint Length = (uint)Enum.GetValues(SourceCategory.TypeEnum).Length;
            if (Length < Values.Length)
                throw new Exception(
                    "Невозможно присвоить значения параметрам в категории если данных больше чем разнообразия параметров");
            for (uint i = 0u; i < Length; i++)
                ParameterSettingBase.SetValue(SourceCategory.GetParameter(i), Values[i]);
        }
        #endregion
    }
}
