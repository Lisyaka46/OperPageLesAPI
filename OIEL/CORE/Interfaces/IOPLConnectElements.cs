using IEL.UserElementsControl;
using OPLAnimation.CORE.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperPageLes.CORE
{
    public interface IOPLConnectElements : IOPLAnimate
    {
        /// <summary>
        /// Объект панели действий подключаемый к элементу отображения OPL
        /// </summary>
        protected IELPanelAction? SourcePanelAction { get; }
    }
}
