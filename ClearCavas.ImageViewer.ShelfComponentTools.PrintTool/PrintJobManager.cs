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




namespace ClearCanvas.ImageViewer.ShelfComponentTools.PrintTool
{
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
            if ((this.sendThread == null) || (this.sendThread.ThreadState == ThreadState.Stopped))
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
                            Bitmap printImagePixel = tile.GetPrintImagePixel(false);
                            float x = tile.NormalizedRectangle.X * size.Width;
                            float y = tile.NormalizedRectangle.Y * size.Height;
                            float width = tile.NormalizedRectangle.Width * size.Width;
                            float height = tile.NormalizedRectangle.Height * size.Height;
                            g.DrawImage(printImagePixel, x, y, width, height);
                            g.DrawRectangle(Pens.White, x, y, width, height);
                            Rectangle destination = new Rectangle(((int)x) + 2, ((int)y) + 2, ((int)width) - 4, ((int)height) - 4);
                            IconCreator.DrawTextOverlay(g, destination, tile.ImageData);
                            printImagePixel.Dispose();
                            if (context.CancelRequested)
                            {
                                userCancelled = true;
                                break;
                            }
                        }
                    }
                    g.Dispose();
                    iod2.AddBitmap(image);
                    //image.Save("d:\\test.jpg");
                    image.Dispose();
                    item.BasicGrayscaleImageSequenceList.Add(iod2);
                    imageBoxPixelModuleIods.Add(item);
                    if (userCancelled)
                    {
                        Platform.Log(LogLevel.Info, SR.UserCancel);
                    }
                    else
                    {
                        progress = new BackgroundTaskProgress(80, SR.BeginSendImage);
                        context.ReportProgress(progress);
                        BasicGrayscalePrintScu scu = new BasicGrayscalePrintScu();
                        scu.Print(job.Printer.AET, job.Printer.CalledAET, job.Printer.Host, job.Printer.Port, basicFilmSessionModuleIod, basicFilmBoxModuleIod, imageBoxPixelModuleIods);
                        if (scu.ResultStatus == DicomState.Success)
                        {
                            this.UpdateStudyPrintStatus(this.GetStudyInstanceUIDs(job.Images));
                            PrintToolComponent.TilesComponent.ResetTiles();
                        }
                        else
                        {
                            this._component.ShowMessageBox(SR.FilmError);
                        }
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
            if ((this.sendThread == null) || (this.sendThread.ThreadState == ThreadState.Stopped))
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
            if ((this.sendThread == null) || (this.sendThread.ThreadState == ThreadState.Stopped))
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
