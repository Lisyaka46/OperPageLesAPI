using OPLAPI.CORE.Animation;

namespace OPLAPI.CORE.Interfaces
{
    /// <summary>
    /// Интерфейс объекта реализующего настройку менеджера анимаций OPL
    /// </summary>
    public interface IOPLAnimate
    {
        /// <summary>
        /// Объект менеджера анимаций настроек OPL
        /// </summary>
        public abstract OPLAnimationManager? ManagerAnimation { get; protected set; }
    }
}
