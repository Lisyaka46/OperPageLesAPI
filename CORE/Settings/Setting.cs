using OPLAPI.CORE.Settings.Base;

namespace OPLAPI.CORE.Settings
{
    /// <summary>
    /// Главный класс управления настройками
    /// </summary>
    public static class Setting
    {
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
            SourceCategories.Add(e.NameCategory, e);
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
        #endregion
    }
}
