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

 
namespace ClearCanvas.Desktop
{
    [MenuAction("QueryPatient", "global-menus/视图/影像查找", "QueryPatient")]
    [MenuAction("ScreenCapture", "global-menus/视图/截屏", "ScreenCapture")]
    [KeyboardAction("ScreenCapture", "global-menus/视图/截屏", "ScreenCapture", KeyStroke = XKeys.Control | XKeys.X)]
  
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    class QeuryPatientTool : Tool<IDesktopToolContext>
    {   
        public const int WM_SNDRIS = USER + 1004;
        public const int USER = 0x0400;

        public void QueryPatient()
        {
            ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, new QueryPatientComponent(), "查询提取病人影像");
        }

        public void ScreenCapture()
        {
            try
            {
                System.Threading.Thread.Sleep(30);
                CaptureImageTool capture = new CaptureImageTool();//截图的工具

                if (capture.ShowDialog() == DialogResult.OK)
                {
                    Image image = capture.Image;//截取的图片
                    //开始发送给
                    ClassFtpSocketClient sk = new ClassFtpSocketClient();
                    string ImagePath = GetRemoteFilePath();
                    string ImageId = GetMaxIDFormImageBack();
                    string sqlstr = string.Format("insert into imageback(imgid,id,imagepath,flag,modulename) values('{0}','{1}','{2}','1','{3}')",
                    ImageId, GlobalData.RunParams.AccessionNumber, ImagePath, GlobalData.RunParams.RunMode);
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.Connection = GlobalData.MainConn.ChangeType();
                    sqlCmd.CommandText = sqlstr;
                    sqlCmd.ExecuteNonQuery();

                    string JpegFile = GetRemoteFilePath() + ImageId + ".jpg";

                    Image tmpImage = capture.Image;
                    System.IO.Stream mStream = new System.IO.MemoryStream();
                    tmpImage.Save(mStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    sk.PutFileIntoFtpServer(GlobalData.RunParams.RemoteIP,
                                                          GlobalData.RunParams.RemotePort,
                                                          JpegFile, mStream);
                    mStream.Dispose();
                    IntPtr WINDOW_HANDLER = new IntPtr(Convert.ToInt32(GlobalData.RunParams.CallerHwnd));
                    if ((int)WINDOW_HANDLER == 0)
                    {

                    }
                    else
                    {
                        User32.SendMessage(WINDOW_HANDLER, WM_SNDRIS, 0, new IntPtr(0));
                    }
                }
            }
            catch (Exception ex)
            {
                Platform.Log(LogLevel.Error, ex.ToString());
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


    [ExtensionPoint]
    public sealed class QueryPatientComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [AssociateView(typeof(QueryPatientComponentViewExtensionPoint))]
    public class QueryPatientComponent : ApplicationComponent
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
