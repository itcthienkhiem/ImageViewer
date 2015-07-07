using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.Desktop;
using System.ComponentModel;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.ShelfComponentTools.PrintTool
{

    //[MenuAction("show", "global-menus/MenuView/MenuShowThumbnails", "Show")]

    [MenuAction("show", "global-menus/MenuTools/原始胶片打印", "Show")]
    
   
    [IconSet("show", "Icons.ShowThumbnailsToolSmall.png", "Icons.ShowThumbnailsToolMedium.png", "Icons.ShowThumbnailsToolLarge.png")]


    [ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    public class PrintTool : ImageViewerTool
    {
        // Fields
        private static readonly Dictionary<IDesktopWindow, IShelf> _shelves = new Dictionary<IDesktopWindow, IShelf>();
        private IDesktopWindow _window = null;

        public PrintTool()
		{

		}


        // Methods
        private void OnShelfClosed(object sender, ClosedEventArgs e)
        {
            _shelves[this._window].Closed -= new EventHandler<ClosedEventArgs>(this.OnShelfClosed);
            _shelves.Remove(this._window);
            this._window = null;
        }

        public  void Show()
        {
            if (_shelves.ContainsKey(base.Context.DesktopWindow))
            {
                _shelves[base.Context.DesktopWindow].Activate();
            }
            else
            {
                this._window = base.Context.DesktopWindow;
                //IClientSetting clientSetting = new ClientSettingExtensionPoint().CreateExtension() as IClientSetting;
                PrintToolComponent component = new PrintToolComponent(this._window);
                IShelf shelf = ApplicationComponent.LaunchAsShelf(this._window, component, SR.Name, SR.Title, ShelfDisplayHint.DockAutoHide | ShelfDisplayHint.DockLeft);
                _shelves[this._window] = shelf;
                _shelves[this._window].Closed += new EventHandler<ClosedEventArgs>(this.OnShelfClosed);
            }
        }

        public override void Initialize()
        {
            base.Initialize();


            //this.Show();
        }
    }

}
