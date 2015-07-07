using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;

using ClearCanvas.Desktop;
using System.ComponentModel;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.View.WinForms;

using ClearCanvas.ImageViewer.ShelfComponentTools.PrintTool;

using System.Drawing;
using System.Windows.Forms;


namespace ClearCanvas.ImageViewer.ShelfComponentTools.PrintTool.WinForms
{
    [ExtensionOf(typeof(PrintToolComponentViewExtensionPoint))]
    public class PrintToolComponentView : WinFormsView, IApplicationComponentView, IView
    {
        // Fields
        private PrintToolComponent _component;
        private Control _control;

        // Methods
        public void SetComponent(IApplicationComponent component)
        {
            this._component = (PrintToolComponent)component;
        }

        // Properties
        public override object GuiElement
        {
            get
            {
                if (this._control == null)
                {
                    this._control = new PrintToolControl(this._component);
                }
                return this._control;
            }
        }
    }

}

