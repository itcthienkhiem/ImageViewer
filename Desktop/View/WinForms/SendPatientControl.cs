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
            _storageScu.AddFile(@"D:\Images\CR\2012-03-30\2496421.3.51.0.7.54985983.58552.8007.42435.39666.57404.45936.dcm");
            //_storageScu.AddStorageInstance(new StorageInstance("AnotherFile.dcm"));
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
           
            System.Diagnostics.Debug.Write(storageScu.SuccessSubOperations);
        }

        
    }
}
