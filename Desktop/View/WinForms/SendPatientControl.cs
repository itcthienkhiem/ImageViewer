using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Dicom.Network.Scu;

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

            editIP.Text = "192.168.0.43";
            editPort.Text = "6008";
            editAE.Text = "AETITLE";
        }

       
        private void button1_Click(object sender, EventArgs e)
        {
            _storageScu = new StorageScu("localAe", "remoteAe", editIP.Text, Convert.ToInt16(editPort.Text));
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
                foreach (string strfile in viewer.getCurrentFiles())
                    _storageScu.AddFile(strfile);
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
