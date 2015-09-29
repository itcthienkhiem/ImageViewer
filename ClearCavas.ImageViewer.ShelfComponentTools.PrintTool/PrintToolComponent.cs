using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

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
using System.ComponentModel;
using ClearCanvas.Desktop.Configuration;


namespace ClearCanvas.ImageViewer.ShelfComponentTools.PrintTool
{
    [ExtensionPoint]
    public sealed class PrintToolComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
      
    }


    //[AssociateView(typeof(PrintToolComponentViewExtensionPoint)), AssociatePageButtonView(typeof(PrintToolPageButtonViewExtensionPoint))]

    [AssociateView(typeof(PrintToolComponentViewExtensionPoint))]

    public class PrintToolComponent : ApplicationComponent
    {
        // Fields
        //private IClientSetting _clientSetting;
        private string _currentPrinter;
        private static FilmOrientation _filmOrientation = FilmOrientation.Portrait;
        private static double _filmRatio = 0.8;
        private List<Film> _films;
        private FilmOrientation _lastFilmOrientation;
        private FilmSize _lastFilmSize;
        private string _lastFormat;
        private decimal _numberOfCopies;
        private PrinterCollection _printerList;
        private PrintJobManager _printJobManager;
        private Printer _selectedPrinter;
        private static FilmSize _Size = FilmSize.Dimension_8in_x_10in;

        private static TilesComponent _tilesComponent = new TilesComponent();
        private IDesktopWindow _window;
        private int printListKey;
        private IPrinterProvider provider;

        //// Events
        //public event EventHandler ActionChanged
        //{
        //    add
        //    {
        //        this._window.MenuModelUpdated += value;
        //    }
        //    remove
        //    {
        //        this._window.MenuModelUpdated -= value;
        //    }
        //}

        //public event EventHandler MenuModelUpdated
        //{
        //    add
        //    {
        //        this._window.MenuModelUpdated += value;
        //    }
        //    remove
        //    {
        //        this._window.MenuModelUpdated -= value;
        //    }
        //}

        // Methods
        public PrintToolComponent(IDesktopWindow window)
        {
            EventHandler handler = null;
            this._lastFilmOrientation =  FilmOrientation.Portrait;
            this._numberOfCopies = 1M;
            this.printListKey = 1;
            this._window = window;
            this.provider = PrinterProviderFactory.CreateProvider();
            this._printerList = this.provider.GetPrinterCollection();
            this._printerList.ActiveChanged += handler;

            foreach (Printer printer in this._printerList)
            {
                if (printer.Selected)
                {
                    this._selectedPrinter = printer;
                    this._currentPrinter = printer.PrinterName;
                    break;
                }
            }
            this._lastFilmSize =  FilmSize.Dimension_8in_x_10in;
            this._printJobManager = new PrintJobManager(this._window, this);
        }

        public static void AddToClipboard(IPresentationImage image)
        {
            IPresentationImage presentationImage = image.Clone();
            _tilesComponent.AddImageToAcitveTile(presentationImage);
        }

        public static void AddToClipboard(IPresentationImage image, RectangleF range)
        {
            IPresentationImage imageData = image.Clone();
            _tilesComponent.AddImageToAcitveTile(imageData, range);
        }

        public override bool CanExit()
        {
            if (this._printJobManager.IsPrinting)
            {
                return false;
            }
            return base.PrepareExit();
        }

        public PrintJob CreateJob(PreviewTileCollection tiles)
        {
            return new PrintJob(this.printListKey++, tiles, TilesComponent.Format, this.PrintedFilmSize, FilmOrientation, Convert.ToInt32(this.NumberOfCopies), this._selectedPrinter);
        }

        public List<Film> GetFilms()
        {
            if (this._films == null)
            {
                this._films = new List<Film>();
                string filename = string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, "Film.xml");
                XmlDocument document = new XmlDocument();
                document.Load(filename);
                foreach (XmlElement element in document.SelectNodes("films/film"))
                {
                    if (Convert.ToBoolean(element.GetAttribute("enabled")))
                    {
                        string attribute = element.GetAttribute("id");
                        string format = element.GetAttribute("format");
                       // FilmSize size = (FilmSize)Enum.Parse(typeof(FilmSize), element.GetAttribute("size"));
                        FilmSize size = FilmSize.FromDicomString(element.GetAttribute("size"));
                         //FilmSize size = FilmSize.Dimension_14in_x_17in;
                        FilmOrientation orientation = (FilmOrientation)Enum.Parse(typeof(FilmOrientation), element.GetAttribute("orientation"));
                        Film item = new Film(attribute, format, size, orientation);
                        this._films.Add(item);
                    }
                }
            }
            return this._films;
        }

        public IPresentationImage GetSelectedPI()
        {
            IPresentationImage selectedPresentationImage = null;
            if (this._window.ActiveWorkspace.Component is IImageViewer)
            {
                IImageViewer component = (IImageViewer)this._window.ActiveWorkspace.Component;
                selectedPresentationImage = component.SelectedPresentationImage;
            }
            return selectedPresentationImage;
        }

        public IEnumerable<IPresentationImage> GetSelectedImages()
        {
            if (this._window.ActiveWorkspace.Component is IImageViewer)
            {
                IImageViewer component = (IImageViewer)this._window.ActiveWorkspace.Component;
                IImageBox imageBox = component.SelectedImageBox;

                foreach (IPresentationImage image in imageBox.DisplaySet.PresentationImages)
                {
                    if (image == null)
                        continue;
                  	yield return image;
				}
            }
        }

        public static double GetSelectedTileRatio()
        {
            return _tilesComponent.GetSelectedTileRatio();
        }

        public override bool PrepareExit()
        {
            if (this._printJobManager.IsPrinting)
            {
                return false;
            }
            return base.PrepareExit();
        }

        public void ShowMessageBox(string message)
        {
            Application.ShowMessageBox(message,MessageBoxActions.Ok);
            //base.Host.ShowMessageBox(message, 1);
        }

        public override void Start()
        {
            _tilesComponent.Reset();
            base.Start();
        }

        public override void Stop()
        {
            _tilesComponent.Dispose();
            base.Stop();
        }

        // Properties
        public string CurrentPrinterValue
        {
            get
            {
                return this._currentPrinter;
            }
            set
            {
                if (this._currentPrinter != value)
                {
                    this._currentPrinter = value;
                    foreach (Printer printer in this._printerList)
                    {
                        if (printer.PrinterName == this._currentPrinter)
                        {
                            printer.Selected = true;
                            this._selectedPrinter = printer;
                            break;
                        }
                    }
                   
                }
            }
        }

        public static FilmOrientation FilmOrientation
        {
            get
            {
                return _filmOrientation;
            }
            set
            {
                if (_filmOrientation != value)
                {
                    _filmOrientation = value;
                }
            }
        }

        public static double FilmRatio
        {
            get
            {
                return _filmRatio;
            }
        }

        //public IAction FixedRadioPrintAction
        //{
        //    get
        //    {
        //        return this._window.FindAction(SR.FixedCutToolPath);
        //    }
        //}

        //public IAction FreeRadioPrintAction
        //{
        //    get
        //    {
        //        return this._window.FindAction(SR.FreeCutToolPath);
        //    }
        //}

        //public ActionModelNode GlobalMenuModel
        //{
        //    get
        //    {
        //        return this._window.MenuModel;
        //    }
        //}

        public PrintJobManager JobManager
        {
            get
            {
                return this._printJobManager;
            }
        }

        public FilmOrientation LasFilmOrientation
        {
            get
            {
                return this._lastFilmOrientation;
            }
            set
            {
                this._lastFilmOrientation = value;
            }
        }

        public FilmSize LastFilmSize
        {
            get
            {
                return this._lastFilmSize;
            }
            set
            {
                this._lastFilmSize = value;
            }
        }

        public string LastFormat
        {
            get
            {
                return this._lastFormat;
            }
            set
            {
                this._lastFormat = value;
            }
        }

        public decimal NumberOfCopies
        {
            get
            {
                return this._numberOfCopies;
            }
            set
            {
                this._numberOfCopies = value;
            }
        }

        public FilmSize PrintedFilmSize
        {
            get
            {
                return _Size;
            }
            set
            {
                if (_Size != value)
                {
                    _Size = value;
                    string str = value.ToString().Replace("IN", "").Replace("CM", "");
                    string[] strArray = str.Split(new char[] { 'X' });
                    if (strArray.Length != 2)
                    {
                        throw new Exception(string.Format("Unknow film size string: {0}", str));
                    }
                    _filmRatio = Convert.ToDouble(strArray[0]) / Convert.ToDouble(strArray[1]);
                }
            }
        }

        public IEnumerable<string> PrintersView
        {
            get
            {
                return this.provider.GetAllPrinters();
            }
        }

        public Printer SelectedPrinter
        {
            get
            {
                return this._selectedPrinter;
            }
        }

        public static TilesComponent TilesComponent
        {
            get
            {
                return _tilesComponent;
            }
        }
    }


}
