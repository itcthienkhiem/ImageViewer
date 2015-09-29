using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using CSharpWin;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using Global.Data;
using Global.FtpSocketClient;
using ClearCanvas.Controls.WinForms.Native;
 
using System.Windows.Forms;
using System.ComponentModel;
using Microsoft.Win32;
 
namespace ClearCanvas.Desktop
{
    //[MenuAction("QueryPatient", "global-menus/视图/影像查找", "QueryPatient")]
    [MenuAction("ScreenCapture", "global-menus/视图/截屏", "ScreenCapture")]
    [MenuAction("SendPatient", "global-menus/视图/MenuSend", "SendPatient")]
    [KeyboardAction("ScreenCapture", "global-menus/视图/截屏", "ScreenCapture", KeyStroke = XKeys.Control | XKeys.X)]
  
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    class QeuryPatientTool : Tool<IDesktopToolContext>
    {   
        public const int WM_SNDRIS = USER + 1004;
        public const int USER = 0x0400;
        int noofscreens = 0;
        Form1[] grabwindow;
        Screen[] screens;
      

        //public void QueryPatient()
        //{
        //    ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, new QueryPatientComponent(), "查询提取病人影像");
        //}

        public void SendPatient()
        {
            ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, new SendPatientComponent(), "发送病人影像");
        }

        public void ScreenCapture()
        {
            screens = Screen.AllScreens;
            noofscreens = screens.Length; 
            grabwindow = new Form1[noofscreens];

            try
            {
                System.Threading.Thread.Sleep(100);
                int i = 0;

                foreach (var screen in System.Windows.Forms.Screen.AllScreens)
                {
                    grabwindow[i] = new Form1("捕捉屏幕图像 " + (i + 1).ToString(), screen.Bounds.X, screen.Bounds.Y, screen.Bounds.Width, screen.Bounds.Height, i);
                    grabwindow[i].HandleMsg = smallscreengrab;
                    grabwindow[i].Show();
                    i++;
                }
            }
            catch (Exception ex)
            {
                Platform.Log(LogLevel.Error, ex.ToString());
            }
        }

        public void smallscreengrab(int sc, int x, int y, int x1, int y1) // grab part of screen
        {
            for (int j = 0; j < noofscreens; j++) { grabwindow[j].Close(); grabwindow[j].Dispose(); }
            int finalx, finaly, finalwidth, finalheight;
            int X1 = Math.Min(x, x1), X2 = Math.Max(x, x1), Y1 = Math.Min(y, y1), Y2 = Math.Max(y, y1);
            finalx = X1 + screens[sc].Bounds.X;
            finaly = Y1 + screens[sc].Bounds.Y;
            finalwidth = X2 - X1 + 1;
            finalheight = Y2 - Y1 + 1;

            Image image = capture_class.CaptureScreentoClipboard(finalx, finaly, finalwidth, finalheight);
            //开始发送给
            ClassFtpSocketClient sk = new ClassFtpSocketClient();
            try
            {
                string ImagePath = GetRemoteFilePath();
                string ImageId = GetMaxIDFormImageBack();
                string sqlstr = string.Format("insert into imageback(imgid,id,imagepath,flag,modulename) values('{0}','{1}','{2}','1','{3}')",
                ImageId, GlobalData.RunParams.AccessionNumber, ImagePath, GlobalData.RunParams.RunMode);
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.Connection = GlobalData.MainConn.ChangeType();
                sqlCmd.CommandText = sqlstr;
                sqlCmd.ExecuteNonQuery();

                string JpegFile = GetRemoteFilePath() + ImageId + ".jpg";

                Image tmpImage = image;
                System.IO.Stream mStream = new System.IO.MemoryStream();
                tmpImage.Save(mStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                sk.PutFileIntoFtpServer(GlobalData.RunParams.RemoteIP,
                                                      GlobalData.RunParams.RemotePort,
                                                      JpegFile, mStream);
                mStream.Dispose();
                image.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("网络断开。。。请重启浏览器!!");
                return;
            }
            IntPtr WINDOW_HANDLER = new IntPtr(Convert.ToInt32(GlobalData.RunParams.CallerHwnd));
            if ((int)WINDOW_HANDLER == 0)
            {
            }
            else
            {
                User32.SendMessage(WINDOW_HANDLER, WM_SNDRIS, 0, new IntPtr(0));
            }
        }

        private string GetMaxIDFormImageBack()
        {
            string sImgid = GlobalData.RunParams.AccessionNumber + "001";
            string sqlstr = string.Format("select convert(varchar(10),convert(int,substring(isnull(max(imgid),0),13,3))+1) imgid from imageback where id = '{0}'",
                GlobalData.RunParams.AccessionNumber);
            SqlDataAdapter sqlDataAd = new SqlDataAdapter(sqlstr, GlobalData.MainConn.ChangeType());
            sqlDataAd.SelectCommand.CommandType = CommandType.Text;
            SqlDataReader ImageReader = sqlDataAd.SelectCommand.ExecuteReader();
            while (ImageReader.Read())
            {
                sImgid = GlobalData.RunParams.AccessionNumber + ((string)ImageReader["imgid"]).PadLeft(3, '0');
            }
            ImageReader.Close();
            sqlDataAd.Dispose();
            return sImgid;
        }

        private string GetRemoteFilePath()
        {
            return GlobalData.RunParams.RunMode + @"\" +
                   GlobalData.RunParams.AccessionNumber.Substring(0, 8) + @"\" +
                   GlobalData.RunParams.AccessionNumber + @"\";
        }
    }


    //[ExtensionPoint]
    //public sealed class QueryPatientComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    //{
    //}

    //[AssociateView(typeof(QueryPatientComponentViewExtensionPoint))]
    //public class QueryPatientComponent : ApplicationComponent
    //{

    //    public override void Start()
    //    {
    //        base.Start();
    //    }

    //    /// <summary>
    //    /// Does nothing unless the task has completed; closes the progress dialog.
    //    /// </summary>
    //    public override void Stop()
    //    {
    //        base.Stop();
    //    }
    //    // your component code here
    //}


    [ExtensionPoint]
    public sealed class SendPatientComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [AssociateView(typeof(SendPatientComponentViewExtensionPoint))]
    public class SendPatientComponent : ApplicationComponent
    {

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
