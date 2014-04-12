using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop.View.WinForms
{

    [ExtensionOf(typeof(QueryPatientComponentViewExtensionPoint))]
    public class QueryPatientView : WinFormsView, IApplicationComponentView
    {
        private QueryPatientComponent _component;
        private QueryPatientControl _control;

        public QueryPatientView()
        {
        }

        #region IApplicationComponentView Members
        public void SetComponent(IApplicationComponent component)
        {
            _component = (QueryPatientComponent)component;
        }
        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new QueryPatientControl(_component);
                }
                return _control;
            }
        }
    }
}
