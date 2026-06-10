using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OPLAPI.CORE.Animation
{
    /// <summary>
    /// Класс менеджера анимаций
    /// </summary>
    public class OPLAnimationManager
    {
        /// <summary>
        /// Массив объектов анимаций
        /// </summary>
        private List<OPLAnimationTypeBase> ListAnimations;

        /// <summary>
        /// Словарь всех объектов управляемых анимаций по типам самих анимаций
        /// </summary>
        private Dictionary<Type, OPLAnimationTypeBase> DictionaryAnimationTypes;

        /// <summary>
        /// Словарь всех объектов управляемых анимаций по типам анимируемых значений
        /// </summary>
        private Dictionary<Type, OPLAnimationTypeBase> DictionaryAnimationValueTypes;

        /// <summary>
        /// Инициализировать объект менеджера анимаций
        /// </summary>
        public OPLAnimationManager()
        {
            ListAnimations =
            [
                new OPLThicknessAnimationType<ThicknessAnimation>(new(new Thickness(0), TimeSpan.FromMilliseconds(300d))
                {
                    DecelerationRatio = 0.6d,
                    EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseOut },
                    From = null
                }),
                new OPLDoubleAnimationType<DoubleAnimation>(new(0, TimeSpan.FromMilliseconds(250d))
                {
                    DecelerationRatio = 0.2d,
                    EasingFunction = new QuinticEase() { EasingMode = EasingMode.EaseOut },
                    From = null
                }),
                new OPLColorAnimationType<ColorAnimation>(new(Colors.Black, TimeSpan.FromMilliseconds(250d))
                {
                    DecelerationRatio = 0.2d,
                    EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseOut },
                    From = null
                }),
                new OPLPointAnimationType<PointAnimation>(new(new System.Windows.Point(0, 0), TimeSpan.FromMilliseconds(250d))
                {
                    DecelerationRatio = 0.2d,
                    EasingFunction = new QuinticEase() { EasingMode = EasingMode.EaseOut },
                    From = null
                }),
                new OPLRectAnimationType<RectAnimation>(new(new Rect(), TimeSpan.FromMilliseconds(250d))
                {
                    DecelerationRatio = 0.8d,
                    EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseOut },
                    From = null
                }),
            ];
            DictionaryAnimationTypes = new()
            {
                [typeof(ThicknessAnimation)] = ListAnimations[0],
                [typeof(DoubleAnimation)] = ListAnimations[1],
                [typeof(ColorAnimation)] = ListAnimations[2],
                [typeof(PointAnimation)] = ListAnimations[3],
                [typeof(RectAnimation)] = ListAnimations[4],
            };
            DictionaryAnimationValueTypes = new()
            {
                [typeof(Thickness)] = ListAnimations[0],
                [typeof(double)] = ListAnimations[1],
                [typeof(Color)] = ListAnimations[2],
                [typeof(System.Windows.Point)] = ListAnimations[3],
                [typeof(Rect)] = ListAnimations[4],
            };

        }

        /// <summary>
        /// Анимировать учитывая условность наличия менеджера анимаций
        /// </summary>
        /// <param name="SourceManager">Текущий менеджер анимаций</param>
        /// <param name="Element">Анимируемый элемент</param>
        /// <param name="PropertySet">Анимироваемое свойство</param>
        /// <param name="ValueFrom">Начальное значение</param>
        /// <param name="ValueTo">Конечное значение</param>
        /// <param name="Duration">Длительность анимации</param>
        public static void AnimateTakingZeroFromTo<T>(in OPLAnimationManager? SourceManager, in T Element,
            DependencyProperty PropertySet, object? ValueFrom, object? ValueTo, TimeSpan Duration) where T : IAnimatable
        {
            if (ValueTo == null) return;
            else if (SourceManager == null)
            {
                if (Element is Animatable AnimatableSetter) AnimatableSetter.SetValue(PropertySet, ValueTo);
                else if (Element is DependencyObject DependencyObjectSetter) DependencyObjectSetter.SetValue(PropertySet, ValueTo);
            }
            else
            {
                OPLAnimationTypeBase SourceTimeLine = SourceManager.DictionaryAnimationValueTypes[PropertySet.PropertyType];
                SourceTimeLine.From = ValueFrom;
                SourceTimeLine.To = ValueTo;
                SourceTimeLine.AnimateEffect(Element, PropertySet, Duration);
            }
        }

        /// <summary>
        /// Анимировать учитывая условность наличия менеджера анимаций
        /// </summary>
        /// <param name="SourceManager">Текущий менеджер анимаций</param>
        /// <param name="Element">Анимируемый элемент</param>
        /// <param name="PropertySet">Анимироваемое свойство</param>
        /// <param name="ValueTo">Конечное значение</param>
        /// <param name="Duration">Длительность анимации</param>
        public static void AnimateTakingZeroTo<T>(in OPLAnimationManager? SourceManager, in T Element,
            DependencyProperty PropertySet, object ValueTo, TimeSpan Duration) where T : IAnimatable =>
                AnimateTakingZeroFromTo(in SourceManager, in Element, PropertySet, null, ValueTo, Duration);

        /// <summary>
        /// Получить объект анимации для конкретного типа
        /// </summary>
        /// <typeparam name="T">Опорный тип анимации</typeparam>
        /// <returns></returns>
        public T GetCloneAnimationElementFromType<T>() where T : AnimationTimeline // T => ThicknessAnimation
        {
            if (DictionaryAnimationTypes.TryGetValue(typeof(T), out var anim))
                return (T)anim.SourceTimeLine.Clone();
            throw new NotImplementedException("Входной тип является не предусматриваемым в поддержке через анимацию OPL");
        }
    }
}
