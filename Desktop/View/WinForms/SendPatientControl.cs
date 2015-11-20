using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Dicom.Network.Scu;


using Leadtools;
using Leadtools.Dicom;
using Leadtools.Drawing;
using Leadtools.Codecs; 


namespace ClearCanvas.Desktop.View.WinForms
{
    public partial class SendPatientControl : UserControl
    {
        private SendPatientComponent _component;

        StorageScu _storageScu = null;
        public SendPatientControl( SendPatientComponent component)
        {
            InitializeComponent();
            _component = component;

            //editIP.Text = "192.168.0.43";
            //editPort.Text = "6008";
            //editAE.Text = "AETITLE";
        }

       
        private void button1_Click(object sender, EventArgs e)
        {
            _storageScu = new StorageScu(editAE.Text, editAE.Text, editIP.Text, Convert.ToInt16(editPort.Text));
            _storageScu.ImageStoreCompleted += new EventHandler<StorageInstance>(storageScu_StoreCompleted);
         
            //_storageScu.AddStorageInstance(new StorageInstance("AnotherFile.dcm"));
            ClearCanvas.ImageViewer.ImageViewerComponent viewer = null;
            DesktopWindow desktopWindow = null;
            List<string> _filenames = new List<string>();

            foreach (DesktopWindow window in Application.DesktopWindows)
            {
                foreach (Workspace space in window.Workspaces)
                {
                    if (space.Title == "imageview")
                    {
                        desktopWindow = window;
                        viewer = space.Component as ClearCanvas.ImageViewer.ImageViewerComponent;
                    }
                }
            }
            if (viewer != null)
            {
                //foreach (string strfile in viewer.getCurrentFiles())
                //    _storageScu.AddFile(strfile);
                //先解压缩，然后再发送
                  //foreach (string strfile in viewer.getCurrentFiles())
                //    _storageScu.AddFile(strfile);
                

                RasterSupport.Unlock(RasterSupportType.Dicom, "y47S3rZv6U");
                RasterSupport.Unlock(RasterSupportType.Document, "HbQR9NSXQ3");
                RasterSupport.Unlock(RasterSupportType.DocumentWriters, "BhaNezSEBB");
                RasterSupport.Unlock(RasterSupportType.DocumentWritersPdf, "3b39Q3YMdX");
                RasterSupport.Unlock(RasterSupportType.ExtGray, "bpTmxSfx8R");
                RasterSupport.Unlock(RasterSupportType.Forms, "GpC33ZK78k");
                RasterSupport.Unlock(RasterSupportType.IcrPlus, "9vdKEtBhFy");
                RasterSupport.Unlock(RasterSupportType.IcrProfessional, "3p2UAxjTy5");
                RasterSupport.Unlock(RasterSupportType.J2k, "Hvu2PRAr3z");
                RasterSupport.Unlock(RasterSupportType.Jbig2, "43WiSV4YNB");
                RasterSupport.Unlock(RasterSupportType.Jpip, "YbGG7wWiVJ");
                RasterSupport.Unlock(RasterSupportType.Pro, "");
                RasterSupport.Unlock(RasterSupportType.LeadOmr, "J3vh828GC8");
                RasterSupport.Unlock(RasterSupportType.MediaWriter, "TpjDw2kJD2");
                RasterSupport.Unlock(RasterSupportType.Medical, "ZhyFRnk3sY");
                RasterSupport.Unlock(RasterSupportType.Medical3d, "DvuzH3ePeu");
                RasterSupport.Unlock(RasterSupportType.MedicalNet, "b4nBinY7tv");
                RasterSupport.Unlock(RasterSupportType.MedicalServer, "QbXwuZxA3h");
                RasterSupport.Unlock(RasterSupportType.Mobile, "");
                RasterSupport.Unlock(RasterSupportType.Nitf, "G37rmw5dTr");
                DicomEngine.Startup();

                foreach (string strfile in viewer.getCurrentFiles())
                {
                    DicomDataSet ds = new DicomDataSet();

                    try
                    {
                        ds.Load(strfile, DicomDataSetLoadFlags.None);
                        ds.ChangeTransferSyntax(DicomUidType.ImplicitVRLittleEndian, 2, ChangeTransferSyntaxFlags.None);
                        ds.Save(strfile + "1", DicomDataSetSaveFlags.None);
                       
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show(ex.ToString());
                    }
                    _storageScu.AddFile(strfile+"1");
                
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("没有合适的图像。。。");
            }
            IAsyncResult asyncResult = _storageScu.BeginSend(new AsyncCallback(SendComplete), _storageScu);
        }

        void storageScu_StoreCompleted(object sender, StorageInstance e)
        {
            StorageScu storageScu = (StorageScu)sender;
         
            System.Diagnostics.Debug.Write(e.SendStatus);
        }

        
        private void SendComplete(IAsyncResult ar)
        {
            StorageScu storageScu = (StorageScu)ar.AsyncState;
            string strFile = string.Format("发送完成 成功:[{0}] 失败:[{1}]", storageScu.SuccessSubOperations, storageScu.FailureSubOperations);
            System.Windows.Forms.MessageBox.Show(strFile);
		
            System.Diagnostics.Debug.Write(storageScu.SuccessSubOperations);
        }
    }
}
