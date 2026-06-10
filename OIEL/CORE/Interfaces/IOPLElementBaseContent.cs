using IEL.UserElementsControl;
using OPLAPI.CORE.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperPageLes.CORE.Interfaces
{
    /// <summary>
    /// Интерфейс объекта реализующий настройку использования базовых элементов интерфейса OPL
    /// </summary>
    /// <remarks>
    /// Интерфейс предназначен для наследования классом к которому могут подключиться базовые элементы интерфейса окна OPL<br/>
    /// Каждое наследуемое окно OPL содержит базовые элементы интерфейса, но может не подключать их если того не желает пользователь
    /// </remarks>
    public interface IOPLElementBaseContent
    {
        /// <summary>
        /// Объект панели действий подключаемый к элементу отображения OPL
        /// </summary>
        public abstract IELPanelAction? SourcePanelAction { get; }
    }
}
