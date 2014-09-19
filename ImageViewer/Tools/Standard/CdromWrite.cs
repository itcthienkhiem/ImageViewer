using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Tools.Standard.Configuration;
using System.IO;
namespace ClearCanvas.ImageViewer.Tools.Standard
{
    [MenuAction("activate", "global-menus/MenuView/CDROM", "WriteCD")]
    [ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    public class CdromWrite : ImageViewerTool
    {
        public void WriteCD()
        {
            string strBurnPath = System.Windows.Forms.Application.StartupPath + "\\Nero\\";
            DirectoryInfo Dir = new DirectoryInfo(strBurnPath);
            string strImagePath = strBurnPath + "\\Images\\";
            string strSoucePath = System.Windows.Forms.Application.StartupPath + @"\DicomFiles\";
            Dir = new DirectoryInfo(strImagePath);
            if (!Dir.Exists)
                Dir.Create();
            else
            {
                foreach (FileInfo file in Dir.GetFiles())
                {
                    file.Delete();
                }
            }
            foreach (IImageSet imageSet in Context.Viewer.LogicalWorkspace.ImageSets)
            {
                foreach (IDisplaySet displaySet in imageSet.DisplaySets)
                {
                    foreach (IPresentationImage image in displaySet.PresentationImages)
                    {
                        if (image is IImageSopProvider)
                        {
                            Sop sop = ((IImageSopProvider)image).Sop;
                            
                            File.Copy(strSoucePath + sop.SopInstanceUid + ".dcm", strImagePath +sop.SopInstanceUid + ".dcm" );
                        }
                    }
                }
            }
            string appname = System.Windows.Forms.Application.StartupPath + "\\nero.exe";
            try
            {
                System.Diagnostics.Process.Start(appname);
            }
            catch
            {
            }
        }
    }
}
