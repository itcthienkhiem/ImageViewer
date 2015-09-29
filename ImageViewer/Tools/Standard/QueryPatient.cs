using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.Controls.WinForms.Native;

using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Tools.Standard.Configuration;
using ClearCanvas.ImageViewer.StudyManagement;
using System.IO;
using Global.Data;


namespace ClearCanvas.ImageViewer.Tools.Standard
{

    [MenuAction("open", "imageviewer-contextmenu/QueryPatient", "Activate")]

    [ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    public class QueryPatient : ImageViewerTool
    {
        private static Dictionary<IDesktopWindow, IShelf> _shelves;
        private static Dictionary<IDesktopWindow, IShelf> Shelves
        {
            get
            {
                if (_shelves == null)
                    _shelves = new Dictionary<IDesktopWindow, IShelf>();
                return _shelves;
            }
        }

        private static void LaunchShelf(IDesktopWindow desktopWindow, IApplicationComponent component, ShelfDisplayHint shelfDisplayHint)
        {
            IShelf shelf = ApplicationComponent.LaunchAsShelf(desktopWindow, component, SR.TitleQuery, "Cine", shelfDisplayHint);
            Shelves[desktopWindow] = shelf;
            Shelves[desktopWindow].Closed += OnShelfClosed;
        }

        private static void OnShelfClosed(object sender, ClosedEventArgs e)
        {
            // We need to cache the owner DesktopWindow (_desktopWindow) because this tool is an 
            // ImageViewer tool, disposed when the viewer component is disposed.  Shelves, however,
            // exist at the DesktopWindow level and there can only be one of each type of shelf
            // open at the same time per DesktopWindow (otherwise things look funny).  Because of 
            // this, we need to allow this event handling method to be called after this tool has
            // already been disposed (e.g. viewer workspace closed), which is why we store the 
            // _desktopWindow variable.

            IShelf shelf = (IShelf)sender;
            shelf.Closed -= OnShelfClosed;
            Shelves.Remove(shelf.DesktopWindow);
        }

        public void Activate()
        {
            //ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, new QueryPatientComponent(), "查询提取病人影像");
  
            IDesktopWindow desktopWindow = this.Context.DesktopWindow;

            // check if a layout component is already displayed
            if (Shelves.ContainsKey(desktopWindow))
            {
                Shelves[desktopWindow].Activate();
            }
            else
            {
                LaunchShelf(desktopWindow, new QueryPatientComponent(desktopWindow), ShelfDisplayHint.DockFloat);
            }
        }
    }


    [ExtensionPoint]
    public sealed class QueryPatientComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [AssociateView(typeof(QueryPatientComponentViewExtensionPoint))]
    public class QueryPatientComponent : ImageViewerToolComponent
    {

        public QueryPatientComponent(IDesktopWindow desktopWindow)
			: base(desktopWindow)
		{
			 
		}

        public override void Start()
        {
            base.Start();
        }

        /// <summary>
        /// Does nothing unless the task has completed; closes the progress dialog.
        /// </summary>
        public override void Stop()
        {
            base.Stop();
        }
        // your component code here
    }

}
