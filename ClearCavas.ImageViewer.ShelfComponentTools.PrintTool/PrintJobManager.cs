using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

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
 

using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.BaseTools;
 
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.RoiGraphics;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom.Iod.Sequences;
using System.Threading;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scu;

using Leadtools;
using Leadtools.Dicom;
using Leadtools.Drawing;
using Leadtools.Codecs; 
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Specialized;

using System.Data.OracleClient;
using System.Data.SqlClient;
using Global.Data;

namespace ClearCanvas.ImageViewer.ShelfComponentTools.PrintTool
{

    public class IniFiles
    {
        public string FileName;
        //声明读写INI文件的API函数
        [DllImport("kernel32")]
        private static extern bool WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, byte[] retVal, int size, string filePath);
        public IniFiles(string AFileName)
        {
            // 判断文件是否存在
            FileInfo fileInfo = new FileInfo(AFileName);
            if ((!fileInfo.Exists))
            {
                //文件不存在，建立文件
                System.IO.StreamWriter sw = new System.IO.StreamWriter(AFileName, false, System.Text.Encoding.Default);
                try
                {
                    sw.Write("#配置档案");
                    sw.Close();
                }
                catch
                {
                    throw (new ApplicationException("Ini文件不存在"));
                }
            }
            FileName = fileInfo.FullName;
        }
        //写INI文件
        public void WriteString(string Section, string Ident, string Value)
        {
            if (!WritePrivateProfileString(Section, Ident, Value, FileName))
            {

                throw (new ApplicationException("写Ini文件出错"));
            }
        }
        //读取INI文件指定
        public string ReadString(string Section, string Ident, string Default)
        {
            Byte[] Buffer = new Byte[65535];
            int bufLen = GetPrivateProfileString(Section, Ident, Default, Buffer, Buffer.GetUpperBound(0), FileName);

            string s = Encoding.GetEncoding(0).GetString(Buffer, 0, bufLen);
            //Encoding.GetEncoding("GB2312")

            return s.Trim();
        }

        //读整数
        public int ReadInteger(string Section, string Ident, int Default)
        {
            string intStr = ReadString(Section, Ident, Convert.ToString(Default));
            try
            {
                return Convert.ToInt32(intStr);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Default;
            }
        }

        //写整数
        public void WriteInteger(string Section, string Ident, int Value)
        {
            WriteString(Section, Ident, Value.ToString());
        }

        //读布尔
        public bool ReadBool(string Section, string Ident, bool Default)
        {
            try
            {
                return Convert.ToBoolean(ReadString(Section, Ident, Convert.ToString(Default)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Default;
            }
        }

        //写Bool
        public void WriteBool(string Section, string Ident, bool Value)
        {
            WriteString(Section, Ident, Convert.ToString(Value));
        }

        //从Ini文件中，将指定的Section名称中的所有Ident添加到列表中
        public void ReadSection(string Section, StringCollection Idents)
        {
            Byte[] Buffer = new Byte[16384];
            //Idents.Clear();
            int bufLen = GetPrivateProfileString(Section, null, null, Buffer, Buffer.GetUpperBound(0),
             FileName);
            //对Section进行解析
            GetStringsFromBuffer(Buffer, bufLen, Idents);
        }
        private void GetStringsFromBuffer(Byte[] Buffer, int bufLen, StringCollection Strings)
        {
            Strings.Clear();
            if (bufLen != 0)
            {
                int start = 0;
                for (int i = 0; i < bufLen; i++)
                {
                    if ((Buffer[i] == 0) && ((i - start) > 0))
                    {
                        String s = Encoding.GetEncoding(0).GetString(Buffer, start, i - start);
                        Strings.Add(s);
                        start = i + 1;
                    }
                }
            }
        }
        //从Ini文件中，读取所有的Sections的名称
        public void ReadSections(StringCollection SectionList)
        {
            byte[] Buffer = new byte[65535];
            int bufLen = 0;
            bufLen = GetPrivateProfileString(null, null, null, Buffer,
             Buffer.GetUpperBound(0), FileName);
            GetStringsFromBuffer(Buffer, bufLen, SectionList);
        }
        //读取指定的Section的所有Value到列表中
        public void ReadSectionValues(string Section, NameValueCollection Values)
        {
            StringCollection KeyList = new StringCollection();
            ReadSection(Section, KeyList);
            Values.Clear();
            foreach (string key in KeyList)
            {
                Values.Add(key, ReadString(Section, key, ""));
            }
        }
        /**/
        ////读取指定的Section的所有Value到列表中，
        //public void ReadSectionValues(string Section, NameValueCollection Values,char splitString)
        //{　 string sectionValue;
        //　　string[] sectionValueSplit;
        //　　StringCollection KeyList = new StringCollection();
        //　　ReadSection(Section, KeyList);
        //　　Values.Clear();
        //　　foreach (string key in KeyList)
        //　　{
        //　　　　sectionValue=ReadString(Section, key, "");
        //　　　　sectionValueSplit=sectionValue.Split(splitString);
        //　　　　Values.Add(key, sectionValueSplit[0].ToString(),sectionValueSplit[1].ToString());

        //　　}
        //}
        //清除某个Section
        public void EraseSection(string Section)
        {
            if (!WritePrivateProfileString(Section, null, null, FileName))
            {
                throw (new ApplicationException("无法清除Ini文件中的Section"));
            }
        }
        //删除某个Section下的键
        public void DeleteKey(string Section, string Ident)
        {
            WritePrivateProfileString(Section, Ident, null, FileName);
        }

        public void UpdateFile()
        {
            WritePrivateProfileString(null, null, null, FileName);
        }

        //检查某个Section下的某个键值是否存在
        public bool ValueExists(string Section, string Ident)
        {
            //
            StringCollection Idents = new StringCollection();
            ReadSection(Section, Idents);
            return Idents.IndexOf(Ident) > -1;
        }

        //确保资源的释放
        ~IniFiles()
        {
            UpdateFile();
        }
    }
    public class PrintJobManager
    {
        // Fields
        private PrintToolComponent _component;
        //private IDBProvider _dbProvider = (new DBProviderFactoryExtensionPoint().CreateExtension() as IDBProviderFactory).Create();
        private List<PrintJob> _errorJobList;
        private List<PrintJob> _jobList;
        private int _maxSize = 10;
        //private TypedDataDesign.PrintQueueDataTable _printTable;
        private bool _stopKey = true;
        private static readonly object _stopSycKey = new object();
        private static readonly object _sycKey = new object();
        private IDesktopWindow _window;
        private Thread sendThread;

        // Events
        private event EventHandler _updateDateView;

        public event EventHandler UpdateDateView
        {
            add { _updateDateView += value; }
            remove { _updateDateView -= value; }
        }

        // Methods
        public PrintJobManager(IDesktopWindow window, PrintToolComponent component)
        {
            this._window = window;
            this._jobList = new List<PrintJob>();
            this._errorJobList = new List<PrintJob>();
            //this._printTable = new TypedDataDesign.PrintQueueDataTable();
            this._component = component;
        }

        public bool AddJob(PrintJob job)
        {
            bool flag = false;
            if ((this.sendThread == null) || (this.sendThread.ThreadState == System.Threading.ThreadState.Stopped))
            {
                this._jobList.InsertRange(0, this._errorJobList);
            }
            lock (_sycKey)
            {
                if ((this._jobList.Count + this._errorJobList.Count) < this.MaxPrintListSize)
                {
                    this._jobList.Add(job);
                    //TypedDataDesign.PrintQueueRow row = this._printTable.NewPrintQueueRow();
                    //row.ID = job.ID;
                    //row.PrintCount = job.Copies;
                    //row.Status = SR.ReadyToSend;
                    //this._printTable.Rows.Add(row);
                    flag = true;
                }
            }
            this.Start();
            return flag;
        }

        private void AddToErrorJob(int jobId)
        {
            lock (_sycKey)
            {
                foreach (PrintJob job in this._jobList)
                {
                    if (job.ID == jobId)
                    {
                        job.IsOnSend = false;
                        this._jobList.Remove(job);
                        if (!this._errorJobList.Contains(job))
                        {
                            this._errorJobList.Add(job);
                        }
                        break;
                    }
                }
            }
        }

        private Size CalcMaxSize(string format, PreviewTileCollection tiles)
        {
            Size size = new Size(0, 0);
            string str = format.Substring(0, 3);
            if (str.Equals("ROW"))
            {
                string[] strArray = format.Substring(4).Split(new char[] { ',' });
                int length = strArray.Length;
                int index = 0;
                foreach (string str2 in strArray)
                {
                    int count = Convert.ToInt32(str2);
                    Size size2 = this.CalcPartSize(tiles, count, index);
                    size.Width = (size.Width < (count * size2.Width)) ? (count * size2.Width) : size.Width;
                    size.Height = (size.Height < (length * size2.Height)) ? (length * size2.Height) : size.Height;
                    index += count;
                }
                return size;
            }
            if (str.Equals("COL"))
            {
                string[] strArray2 = format.Substring(4).Split(new char[] { ',' });
                int num4 = strArray2.Length;
                int num5 = 0;
                foreach (string str3 in strArray2)
                {
                    int num6 = Convert.ToInt32(str3);
                    Size size3 = this.CalcPartSize(tiles, num6, num5);
                    size.Width = (size.Width < (num4 * size3.Width)) ? (num4 * size3.Width) : size.Width;
                    size.Height = (size.Height < (num6 * size3.Height)) ? (num6 * size3.Height) : size.Height;
                    num5 += num6;
                }
                return size;
            }
            string[] strArray3 = format.Substring(9).Split(new char[] { ',' });
            int num7 = Convert.ToInt32(strArray3[0]);
            int num8 = Convert.ToInt32(strArray3[1]);
            Size size4 = this.CalcPartSize(tiles, tiles.Count, 0);
           
            size.Width = num7 * size4.Width;
            size.Height = num8 * size4.Height;

            if (size.Width > 5000  || size.Height > 5000)
            {
                 size.Width = (int)(size.Width * 0.3);
                 size.Height = (int)(size.Height * 0.3);
                 Platform.Log(LogLevel.Error, "the sizevalue is " + size.ToString());
            }
            return size;
        }

        private Size CalcPartSize(PreviewTileCollection tiles, int count, int index)
        {
            Size size = new Size(0, 0);
            int num = 0;
            while (num < count)
            {
                if (tiles[index].ImageData != null)
                {
                    Bitmap printImagePixel = tiles[index].GetPrintImagePixel(false);
                    int width = printImagePixel.Width;
                    int height = printImagePixel.Height;
                    size.Width = (size.Width < width) ? width : size.Width;
                    size.Height = (size.Height < height) ? height : size.Height;
                    printImagePixel.Dispose();

                
                }
                num++;
                index++;
            }
            return size;
        }

        public void ChangeSendStatus(int printID, string sendMessage)
        {
            //foreach (DataRow row in this._printTable.Rows)
            //{
            //    if (Convert.ToInt32(row["ID"]) == printID)
            //    {
            //        row["Status"] = sendMessage;
            //        break;
            //    }
            //}
        }

        private List<string> GetStudyInstanceUIDs(PreviewTileCollection tiles)
        {
            List<string> list = new List<string>();
            foreach (PreviewTile tile in tiles)
            {
                string item = string.Empty;
                IImageSopProvider imageData = tile.ImageData as IImageSopProvider;
                if (imageData != null)
                {
                    item = imageData.ImageSop.StudyInstanceUid;
                }
                if ((item != string.Empty) && !list.Contains(item))
                {
                    list.Add(item);
                }
            }
            return list;
        }

        private bool GoPrint(PrintJob printJob, out int successSend)
        {
            successSend = 0;
            try
            {
                BasicFilmSessionModuleIod basicFilmSessionModuleIod = new BasicFilmSessionModuleIod
                {
                    NumberOfCopies = printJob.Copies,
                    MediumType = printJob.MediumType,
                    FilmDestination = printJob.FilmDestination
                };
                BasicFilmBoxModuleIod basicFilmBoxModuleIod = new BasicFilmBoxModuleIod
                {
                    //ImageDisplayFormat = printJob.Format,
                    FilmSizeId = printJob.FilmSize,
                    Illumination = printJob.Illumination,
                    FilmOrientation = printJob.FilmOrientation,
                    ReflectedAmbientLight = printJob.ReflectedAmbientLight,
                    MagnificationType = printJob.MagnificationType
                };
                IList<ImageBoxPixelModuleIod> imageBoxPixelModuleIods = new List<ImageBoxPixelModuleIod>();
                ushort num = 1;
                foreach (PreviewTile tile in printJob.Images)
                {
                    this.ChangeSendStatus(printJob.ID, string.Format(SR.CreatingImageBuffer, num, printJob.Images.Count));
                    ImageBoxPixelModuleIod item = new ImageBoxPixelModuleIod
                    {
                        ImageBoxPosition = tile.Position
                    };
                    BasicGrayscaleImageSequenceIod iod4 = new BasicGrayscaleImageSequenceIod
                    {
                        PhotometricInterpretation = printJob.MonochormeType
                    };
                    Bitmap printImagePixel = tile.GetPrintImagePixel(true);
                    iod4.AddBitmap(printImagePixel);
                    printImagePixel.Dispose();
                    item.BasicGrayscaleImageSequenceList.Add(iod4);
                    imageBoxPixelModuleIods.Add(item);
                }
                if (this.StopKey)
                {
                    return false;
                }
                this.ChangeSendStatus(printJob.ID, SR.BeginSendImage);
                try
                {
                    for (int i = 0; i < printJob.Copies; i++)
                    {
                        BasicGrayscalePrintScu scu = new BasicGrayscalePrintScu();
                        this.ChangeSendStatus(printJob.ID, string.Format(SR.Sending, i.ToString(), printJob.Copies.ToString()));
                        if (scu.Print(printJob.Printer.AET, printJob.Printer.CalledAET, printJob.Printer.Host, printJob.Printer.Port, basicFilmSessionModuleIod, basicFilmBoxModuleIod, imageBoxPixelModuleIods) == DicomState.Success)
                        {
                            successSend++;
                        }
                    }
                }
                catch (Exception exception)
                {
                    Platform.Log(LogLevel.Error, exception);
                }
            }
            catch (Exception exception2)
            {
                Platform.Log(LogLevel.Error, exception2);
            }
            if (successSend == 0)
            {
                return false;
            }
            return true;
        }

        private void InternalPrint(PrintJob job)
        {
            BasicFilmSessionModuleIod basicFilmSessionModuleIod = new BasicFilmSessionModuleIod
            {
                NumberOfCopies = job.Copies,
                MediumType = job.MediumType,
                FilmDestination = job.FilmDestination
            };
            BasicFilmBoxModuleIod basicFilmBoxModuleIod = new BasicFilmBoxModuleIod
            {
                //ImageDisplayFormat = @"STANDARD\1,1",
                ImageDisplayFormat = ImageDisplayFormat.Standard_1x1,
                FilmSizeId = job.FilmSize,
                Illumination = job.Illumination,
                FilmOrientation = job.FilmOrientation,
                ReflectedAmbientLight = job.ReflectedAmbientLight,
                MagnificationType = job.MagnificationType
            };
            IList<ImageBoxPixelModuleIod> imageBoxPixelModuleIods = new List<ImageBoxPixelModuleIod>();
            bool userCancelled = false;
            BackgroundTask task = new BackgroundTask(delegate(IBackgroundTaskContext context)
            {
                try
                {
                    BackgroundTaskProgress progress;
                    ushort num = 1;
                    int percent = 0;
                    new List<string>();
                    ImageBoxPixelModuleIod item = new ImageBoxPixelModuleIod
                    {
                        ImageBoxPosition = 1
                    };
                    BasicGrayscaleImageSequenceIod iod2 = new BasicGrayscaleImageSequenceIod
                    {
                        PhotometricInterpretation = job.MonochormeType
                    };
                    Size size = this.CalcMaxSize(job.Format, job.Images);
                    Bitmap image = new Bitmap(size.Width, size.Height);
                    System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image);
                    foreach (PreviewTile tile in job.Images)
                    {
                        percent = (int)((((float)(num - 1)) / ((float)job.Images.Count)) * 80f);
                        num = (ushort)(num + 1);
                        progress = new BackgroundTaskProgress(percent, string.Format(SR.CreatingImageBuffer, num, job.Images.Count));
                        context.ReportProgress(progress);
                        if (tile.ImageData != null)
                        {
                            Platform.Log(LogLevel.Error, "TITLE IMAGEDATA");
                            Bitmap printImagePixel = tile.GetPrintImagePixel(false);
                            float x = tile.NormalizedRectangle.X * size.Width;
                            float y = tile.NormalizedRectangle.Y * size.Height;
                            float width = tile.NormalizedRectangle.Width * size.Width;
                            float height = tile.NormalizedRectangle.Height * size.Height;
                            g.DrawImage(printImagePixel, x, y, width, height);
                            g.DrawRectangle(Pens.White, x, y, width, height);
                            Rectangle destination = new Rectangle(((int)x) + 2, ((int)y) + 2, ((int)width) - 4, ((int)height) - 4);
                            try
                            {
                                IconCreator.DrawTextOverlay(g, destination, tile.ImageData);
                            }
                            catch (Exception ex)
                            {
                                Platform.Log(LogLevel.Error, " exception: " + ex.ToString ());
                            }
                            printImagePixel.Dispose();
                            if (context.CancelRequested)
                            {
                                userCancelled = true;
                                break;
                            }
                        }
                    }

                    //clear
                    foreach (PreviewTile tile in job.Images)
                    {

                        ImageSop sop = ((IImageSopProvider)tile.ImageData).ImageSop;
                        string strUID = sop.SopInstanceUid;
                        //获取UID 的 accession
                        try
                        {
                            if (Conn.isOracle())
                            {
                                string sqlstr = string.Format(" update examrecord set filmprint='{0}' where  id=(select AccessionNumber from images where SopInstanceUID='{1}') and modulename='RIS' ",
                                      "1", strUID);
                                OracleCommand sqlCmd = new OracleCommand();
                                sqlCmd.Connection = GlobalData.MainConn.ChangeTypeOracle();
                                sqlCmd.CommandText = sqlstr;
                                sqlCmd.ExecuteNonQuery();
                                sqlCmd.Dispose();
                            }
                            else
                            {
                                string sqlstr = string.Format(" update examrecord set filmprint='{0}' where  id=(select AccessionNumber from images where SopInstanceUID='{1}') and modulename='RIS' ",
                                   "1", strUID);
                                SqlCommand sqlCmd = new SqlCommand();
                                sqlCmd.Connection = GlobalData.MainConn.ChangeType();
                                sqlCmd.CommandText = sqlstr;
                                sqlCmd.ExecuteNonQuery();
                                sqlCmd.Dispose();
                                Platform.Log(LogLevel.Error, " sql is  " + sqlstr);
                            }
                        }
                        catch (Exception ex)
                        {
                            Platform.Log(LogLevel.Error, "exception is " + ex.ToString());
                        }
                        //tile.Dispose();
                        tile.RemoveImage();
                    }
                    
                    job.Images.Clear();


                    g.Dispose();
                    //iod2.AddBitmap(image);
                    Unlock();
                    RasterImage lRasterImage = null;
                    RasterCodecs lRasterCodecs = new RasterCodecs();

                    lRasterImage = RasterImageConverter.ConvertFromImage(image, ConvertFromImageOptions.None);
                    lRasterCodecs.Save(lRasterImage, System.Windows.Forms.Application.StartupPath + @"\print.jpg", RasterImageFormat.Tif, 8);
                    string lDicomFile = "1" + DateTime.Now.ToString("HHmmss", DateTimeFormatInfo.InvariantInfo);
                    lRasterCodecs.Save(lRasterImage, System.Windows.Forms.Application.StartupPath + @"\PrintFiles\" + lDicomFile, RasterImageFormat.DicomGray, 16);
                    //image.Save("d:\\test.jpg");
                    image.Dispose();
                    lRasterImage.Dispose();
                    lRasterCodecs.Dispose();
                    //item.BasicGrayscaleImageSequenceList.Add(iod2);
                    //imageBoxPixelModuleIods.Add(item);
                    if (userCancelled)
                    {
                        Platform.Log(LogLevel.Info, SR.UserCancel);
                    }
                    else
                    {
                        progress = new BackgroundTaskProgress(80, SR.BeginSendImage);
                        context.ReportProgress(progress);
                        PrintDicomFiles(lDicomFile, 1, 1, lDicomFile, job);

                        //BasicGrayscalePrintScu scu = new BasicGrayscalePrintScu();
                        //scu.Print(job.Printer.AET, job.Printer.CalledAET, job.Printer.Host, job.Printer.Port, basicFilmSessionModuleIod, basicFilmBoxModuleIod, imageBoxPixelModuleIods);
                        //if (scu.ResultStatus == DicomState.Success)
                        //{
                        //    this.UpdateStudyPrintStatus(this.GetStudyInstanceUIDs(job.Images));
                        //    PrintToolComponent.TilesComponent.ResetTiles();
                        //}
                        //else
                        //{
                        //    this._component.ShowMessageBox(SR.FilmError);
                        //}
                    }
                }
                catch (OutOfMemoryException)
                {
                    Platform.Log(LogLevel.Error, "内存不够");
                    BackgroundTaskProgress progress2 = new BackgroundTaskProgress(100, SR.OutOfMemory);
                    context.ReportProgress(progress2);
                    this._component.ShowMessageBox(SR.OutOfMemory);
                }
                catch (Exception exception)
                {
                    Platform.Log(LogLevel.Error, exception.Message);
                    BackgroundTaskProgress progress3 = new BackgroundTaskProgress(100, exception.Message);
                    context.ReportProgress(progress3);
                    this._component.ShowMessageBox(SR.PrinterError);
                }
                finally
                {
                    context.Complete(null);
                }
            }, true);
            ProgressDialog.Show(task, this._window, true, ProgressBarStyle.Blocks);
        }

        private static void Unlock()
        {
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
        }

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        public static extern int FindWindow(string lpClassName, string lpWindowName);
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(
                     int hWnd,      // handle to destination window
                     int Msg,       // message
                     int wParam,  // first message parameter
                     int lParam // second message parameter
         );
        public const int USER = 0x0400;
        public const int WM_PRINTDICOM = USER + 1006;
        private void PrintDicomFiles(string sMessage, int Rows, int Columns, string strPrintFileNames, PrintJob job)
        {
         
            string DcmPrintPath = System.Windows.Forms.Application.StartupPath + @"\PrintFiles\";
            string lPrtFile = DcmPrintPath + sMessage + ".prt";
            string lDicomPrnAppName = System.Windows.Forms.Application.StartupPath + @"\DcmPrnProtect.exe";
            
            IniFiles prtini = new IniFiles(lPrtFile);
            prtini.WriteString("DCMPRNT", "AETITLE", job.Printer.AET);
            prtini.WriteString("DCMPRNT", "AETITLESCU", job.Printer.CalledAET);
            prtini.WriteString("DCMPRNT", "IPADDRESS", job.Printer.Host);
            //prtini.WriteString("DCMPRNT", "PORT", job.Printer.Port);
            prtini.WriteInteger("DCMPRNT", "PORT", job.Printer.Port);
            prtini.WriteString("DCMPRNT", "IMAGEDISFORMAT", string.Format("STANDARD\\{0},{1}", Columns, Rows));
            if (job.FilmOrientation == FilmOrientation.Landscape)
                prtini.WriteString("DCMPRNT", "FILMORE", "landscape");
            else
                prtini.WriteString("DCMPRNT", "FILMORE", "portrait");
            prtini.WriteString("DCMPRNT", "FILMSIZE", job.FilmSize.ToString());
            prtini.WriteString("DCMPRNT", "NUMBERCOPYS", "1");
            prtini.WriteString("DCMPRNT", "FILES", strPrintFileNames);
            prtini.WriteString("DCMPRNT", "TEMPIMAGEPATH", DcmPrintPath);
            prtini.WriteString("DCMPRNT", "PATIENTID", "123");
            //prtini.WriteString("DCMPRNT", "MediumType", cmbFilmType.Text);
            try
            {
                //Call Dicom Print Process            
                ProcessStartInfo Info = new ProcessStartInfo();
                Info.FileName = lDicomPrnAppName;
                Info.Arguments = sMessage;
                int WINDOW_HANDLER = FindWindow(null, @"胶片打印");
                if (WINDOW_HANDLER == 0)
                {
                    Process proc = Process.Start(Info);
                }
                else
                {
                    SendMessage(WINDOW_HANDLER, WM_PRINTDICOM, Convert.ToInt32(sMessage), 0);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, ex.Source);
            }
        }

        public void Print(PrintJob job)
        {
            this.InternalPrint(job);
        }

        public void RemoveJob(int jobId)
        {
            bool flag = false;
            bool flag2 = false;
            lock (_sycKey)
            {
                foreach (PrintJob job in this._jobList)
                {
                    if (job.ID == jobId)
                    {
                        flag2 = true;
                        if (!job.IsOnSend)
                        {
                            this._jobList.Remove(job);
                            if (this._errorJobList.Contains(job))
                            {
                                this._errorJobList.Remove(job);
                            }
                            flag = true;
                            break;
                        }
                    }
                }
                if (!flag2)
                {
                    foreach (PrintJob job2 in this._errorJobList)
                    {
                        if ((job2.ID == jobId) && !job2.IsOnSend)
                        {
                            this._errorJobList.Remove(job2);
                            flag = true;
                            break;
                        }
                    }
                }

                //for (int i = this._printTable.Rows.Count - 1; i >= 0; i--)
                //{
                //    if ((Convert.ToInt32(this._printTable.Rows[i]["ID"]) == jobId) && flag)
                //    {
                //        this._printTable.Rows.Remove(this._printTable.Rows[i]);
                //        EventsHelper.Fire(this._updateDateView, this, new EventArgs());
                //        break;
                //    }
                //}
            }
        }

        public void Resend()
        {
            if ((this.sendThread == null) || (this.sendThread.ThreadState == System.Threading.ThreadState.Stopped))
            {
                this._jobList.InsertRange(0, this._errorJobList);
            }
            this.Start();
        }

        private void Run()
        {
            try
            {
                while (!this.StopKey)
                {
                    PrintJob job;
                    lock (_sycKey)
                    {
                        if (this._jobList.Count > 0)
                        {
                            job = this._jobList[0];
                            job.IsOnSend = true;
                            this.ChangeSendStatus(job.ID, SR.PrepareToSend);
                        }
                        else
                        {
                            return;
                        }
                    }
                    int successSend = 0;
                    if (this.GoPrint(job, out successSend))
                    {
                        this.ChangeSendStatus(job.ID, string.Format(SR.SendFinish, successSend.ToString(), job.Copies.ToString()));
                        foreach (PreviewTile tile in job.Images)
                        {
                            IImageSopProvider imageData = tile.ImageData as IImageSopProvider;
                            if (imageData != null)
                            {
                                string studyInstanceUid = imageData.ImageSop.StudyInstanceUid;
                                this.UpdateDataBaseStatus(studyInstanceUid);
                            }
                        }
                        job.IsOnSend = false;
                        this.RemoveJob(job.ID);
                    }
                    else
                    {
                        this.ChangeSendStatus(job.ID, string.Format(SR.SendFailure, Convert.ToString((int)(job.Copies - successSend)), job.Copies.ToString()));
                        this.AddToErrorJob(job.ID);
                    }
                }
            }
            catch (Exception exception)
            {
                Platform.Log(LogLevel.Error, exception);
            }
        }

        private void Start()
        {
            if (this.StopKey)
            {
                this.StopKey = false;
            }
            if ((this.sendThread == null) || (this.sendThread.ThreadState == System.Threading.ThreadState.Stopped))
            {
                this.sendThread = new Thread(new ThreadStart(this.Run));
                this.sendThread.Start();
            }
        }

        public void Stop()
        {
            this.StopKey = true;
        }

        private void UpdateDataBaseStatus(string instanceUid)
        {
            //string commandText = "update Study set PrintFlag='1' where StudyUid = '" + instanceUid + "'";
            //this._dbProvider.ExecuteNonQuery(commandText, new object[0]);
        }

        private void UpdateStudyPrintStatus(IList<string> uids)
        {
            if (uids.Count != 0)
            {
                StringBuilder builder = new StringBuilder();
                bool flag = true;
                foreach (string str in uids)
                {
                    if (flag)
                    {
                        builder.Append(string.Format("'{0}'", str));
                        flag = false;
                    }
                    else
                    {
                        builder.Append(string.Format(",'{0}'", str));
                    }
                }
                string commandText = string.Format("UPDATE Study SET PrintFlag = '1' WHERE StudyUid IN ({0})", builder.ToString());
                //this._dbProvider.ExecuteNonQuery(commandText, new object[0]);
            }
        }

        // Properties
        public bool HasJobs
        {
            get
            {
                lock (_sycKey)
                {
                    return ((this._errorJobList.Count > 0) || (this._jobList.Count > 0));
                }
            }
        }

        public bool IsPrinting
        {
            get
            {
                if (!this.HasJobs)
                {
                    return false;
                }
                if (this.StopKey)
                {
                    return false;
                }
                return true;
            }
        }

        public int MaxPrintListSize
        {
            get
            {
                return this._maxSize;
            }
            set
            {
                this._maxSize = value;
            }
        }
         

        private bool StopKey
        {
            get
            {
                lock (_stopSycKey)
                {
                    return this._stopKey;
                }
            }
            set
            {
                lock (_stopSycKey)
                {
                    this._stopKey = value;
                }
            }
        }
    }
}
