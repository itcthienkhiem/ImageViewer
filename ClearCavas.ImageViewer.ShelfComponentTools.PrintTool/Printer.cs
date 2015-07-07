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
using ClearCanvas.Common;


namespace ClearCanvas.ImageViewer.ShelfComponentTools.PrintTool
{
    public class Printer
    {
        // Fields
        private string _aet;
        private string _calledAet;
        private string _host;
        private string _name;
        private int _port;
        private bool _selected;

        // Events
        public event EventHandler ActiveChanged
        {
            add { _activeChanged += value; }
            remove { _activeChanged -= value; }
       }

        private EventHandler _activeChanged;


        // Methods
        public Printer(string name, string aet, string calledAet, string host, int port)
        {
            this._name = name;
            this._aet = aet;
            this._calledAet = calledAet;
            this._host = host;
            this._port = port;
            this._selected = false;
        }

        // Properties
        public string AET
        {
            get
            {
                return this._aet;
            }
        }

        public string CalledAET
        {
            get
            {
                return this._calledAet;
            }
        }

        public string Host
        {
            get
            {
                return this._host;
            }
        }

        public int Port
        {
            get
            {
                return this._port;
            }
        }

        public string PrinterName
        {
            get
            {
                return this._name;
            }
        }

        public bool Selected
        {
            get
            {
                return this._selected;
            }
            set
            {
                if (this._selected != value)
                {
                    this._selected = value;
                    if (value)
                    {
                        EventsHelper.Fire(this._activeChanged, this, EventArgs.Empty);
                    }
                }
            }
        }
    }


    public class PrinterCollection : ObservableList<Printer>
    {
        // Events
        private event EventHandler _OnActiveChanged;

        public event EventHandler ActiveChanged
        {
            add { _OnActiveChanged += value; }
            remove { _OnActiveChanged -= value; }
        }

        // Methods
        public override void Add(Printer printer)
        {
            base.Add(printer);
            printer.ActiveChanged += new EventHandler(this.OnActiveChanged);
        }

        private void OnActiveChanged(object sender, EventArgs e)
        {
            Printer printer1 = (Printer)sender;
            foreach (Printer printer in this)
            {
                if ((sender != printer) && printer.Selected)
                {
                    printer.Selected = false;
                    break;
                }
            }
            EventsHelper.Fire(this._OnActiveChanged, sender, e);
        }
    }

    internal static class PrinterProviderFactory
    {
        // Methods
        public static IPrinterProvider CreateProvider()
        {
            return new DefaultPrinterProvider( );
        }
    }

    public interface IPrinterProvider
    {
        // Methods
        IEnumerable<string> GetAllPrinters();
        PrinterCollection GetPrinterCollection();
    }


    public class DefaultPrinterProvider : IPrinterProvider
    {
        // Fields
        private List<Printer> _printers = new List<Printer>();
        private List<string> _printersName = new List<string>();

        // Methods
        public DefaultPrinterProvider( )
        {
            //for (int i = 0; i < table.Count; i++)
            //{
            //    //TypedDataDesign.PrintViewRow row = table[i];
            //    Printer item = new Printer(row.PrinterName, row.LocalAETitle, row.PrinterAETitle, row.IPAddress, row.Port)
            //    {
            //        Selected = row.Default
            //    };
            //    this._printersName.Add(row.PrinterName);
            //    this._printers.Add(item);
            //}
            Printer item = new Printer("test", "AETITLE", "MYAETITLE", "127.0.0.1", 7000);
            item.Selected = true;
            this._printersName.Add("test");
            this._printers.Add(item);
        }

        public IEnumerable<string> GetAllPrinters()
        {
            return this._printersName;
        }

        public PrinterCollection GetPrinterCollection()
        {
            PrinterCollection printers = new PrinterCollection();
            foreach (Printer printer in this._printers)
            {
                printers.Add(printer);
            }
            return printers;
        }
    }



 }
