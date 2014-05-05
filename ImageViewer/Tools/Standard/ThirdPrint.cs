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
   
    //[ButtonAction("open", "global-toolbars/ToolbarMpr/ToolbarThirdPrint", "LaunchPrint")]
    [MenuAction("openOne", "imageviewer-contextmenu/MenuThirdPrint/MenuPrintChooseOne", "LaunchOnePrint")]
    [MenuAction("open", "imageviewer-contextmenu/MenuThirdPrint/MenuPrintChooseDisplaySet", "LaunchPrint")]
    [KeyboardAction("DicomPrint", "imageviewer-keyboard/ToolsStandardPrint/DicomPrint", "LaunchOnePrint", KeyStroke = XKeys.Control | XKeys.PrintScreen)]
    //[MenuAction("open", "global-menus/MenuTools/MenuThirdPrint", "LaunchPrint")]
    //[IconSet("open", "Icons.PrintTool.png", "Icons.PrintTool.png", "Icons.PrintTool.png")]
    //[EnabledStateObserver("open", "Enabled", "EnabledChanged")]
    //[VisibleStateObserver("open", "Visible", null)]

    [ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    public class ThirdPrintTool : ImageViewerTool
	{
        public bool Visible { get; private set; }
        public const int WM_PRINTDICOM = USER + 1006;
        public const int USER = 0x0400;

        public override void Initialize()
        {
            base.Initialize(); 

            Context.Viewer.EventBroker.ImageBoxSelected += OnImageBoxSelected;
            Context.Viewer.EventBroker.DisplaySetSelected += OnDisplaySetSelected;
        }

        protected override void Dispose(bool disposing)
        {
            Context.Viewer.EventBroker.ImageBoxSelected -= OnImageBoxSelected;
            Context.Viewer.EventBroker.DisplaySetSelected -= OnDisplaySetSelected;

            base.Dispose(disposing);
        }

        public void LaunchOnePrint()
        {
            IPresentationImage currentImage = Context.Viewer.SelectedPresentationImage;
            IDisplaySet displaySet = currentImage.ParentDisplaySet;
            string strFiles = "";
            if (currentImage is IImageSopProvider)
            {
                Sop sop = ((IImageSopProvider)currentImage).Sop;
                strFiles += sop.SopInstanceUid + ";";
                LanchThirdPrint(strFiles);
            }
        }

        public void LaunchPrint()
        {
            IPresentationImage currentImage = Context.Viewer.SelectedPresentationImage;
            IDisplaySet displaySet = currentImage.ParentDisplaySet;

            if (displaySet.PresentationImages.Count > 64)
            {
                System.Windows.Forms.MessageBox.Show("图像序列太多...");
                return;
            }
            string strFiles = "";
            foreach (IPresentationImage image in displaySet.PresentationImages)
            {
                if (image is IImageSopProvider)
                {
                    Sop sop = ((IImageSopProvider)image).Sop;
                    strFiles += sop.SopInstanceUid + ";";
                }
            }
            LanchThirdPrint(strFiles);
        }

        private void LanchThirdPrint(string strFiles)
        {
            string lDicomFile = System.Windows.Forms.Application.StartupPath + @"\Third\EFilm.ini";
            IniFiles efilmini = new IniFiles(lDicomFile);
            efilmini.WriteString("dicomprintfile", "files", strFiles);

            string l3dPrnAppName = System.Windows.Forms.Application.StartupPath + @"\Third\3D.exe";
            try
            {
                //Call Dicom Print Process            
                System.Diagnostics.ProcessStartInfo Info = new System.Diagnostics.ProcessStartInfo();
                Info.FileName = l3dPrnAppName;
                Info.Arguments = "PRINT";

                IntPtr WINDOW_HANDLER = User32.FindWindow(null, @"DICOM胶片打印");
                if ((int)WINDOW_HANDLER == 0)
                {
                    System.Diagnostics.Process proc = System.Diagnostics.Process.Start(Info);
                }
                else
                {
                    User32.SendMessage(WINDOW_HANDLER, WM_PRINTDICOM, 0, new IntPtr(0));
                }
            }
            catch (Exception ex)
            {
                Platform.Log(LogLevel.Error, ex.ToString());
            }
        }

        protected override void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
        {
            if (e.SelectedPresentationImage != null)
                UpdateEnabled(e.SelectedPresentationImage.ParentDisplaySet);
            else
                UpdateEnabled(null);
        }

        private void OnImageBoxSelected(object sender, ImageBoxSelectedEventArgs e)
        {
            if (e.SelectedImageBox.DisplaySet == null)
                UpdateEnabled(null);
        }

        private void OnDisplaySetSelected(object sender, DisplaySetSelectedEventArgs e)
        {
            UpdateEnabled(e.SelectedDisplaySet);
        }

        private void UpdateEnabled(IDisplaySet selectedDisplaySet)
        {
            Enabled = selectedDisplaySet != null && selectedDisplaySet.PresentationImages.Count > 1;
        }
    }
}
