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
    public class PrintJob
    {
        // Fields
        private int _copies;
        private FilmOrientation _filmOrien;
        private FilmSize _filmSize;
        private string _format;
        private int _id;
        private PreviewTileCollection _images;
        private bool _isonSend;
        private Printer _printer;

        // Methods
        public PrintJob(int id, PreviewTileCollection images, string format, FilmSize size, FilmOrientation orien, int copies, Printer printer)
        {
            this._id = id;
            this._images = images;
            this._format = format;
            this._filmSize = size;
            this._filmOrien = orien;
            this._copies = copies;
            this._printer = printer;
        }

        // Properties
        public int Copies
        {
            get
            {
                return this._copies;
            }
        }

        public FilmDestination FilmDestination
        {
            get
            {
                return FilmDestination.Processor;
            }
        }

        public FilmOrientation FilmOrientation
        {
            get
            {
                return this._filmOrien;
            }
        }

        public FilmSize FilmSize
        {
            get
            {
                return this._filmSize;
            }
        }

        public string Format
        {
            get
            {
                return this._format;
            }
        }

        public int ID
        {
            get
            {
                return this._id;
            }
        }

        public ushort Illumination
        {
            get
            {
                return 0x7d0;
            }
        }

        public PreviewTileCollection Images
        {
            get
            {
                return this._images;
            }
        }

        public bool IsOnSend
        {
            get
            {
                return this._isonSend;
            }
            set
            {
                this._isonSend = value;
            }
        }

        public MagnificationType MagnificationType
        {
            get
            {
                return MagnificationType.Cubic;
            }
        }

        public MediumType MediumType
        {
            get
            {
                return MediumType.BlueFilm;
            }
        }

        public PhotometricInterpretation MonochormeType
        {
            get
            {
                return PhotometricInterpretation.Monochrome2;
            }
        }

        public Printer Printer
        {
            get
            {
                return this._printer;
            }
        }

        public ushort ReflectedAmbientLight
        {
            get
            {
                return 10;
            }
        }
    }
}
