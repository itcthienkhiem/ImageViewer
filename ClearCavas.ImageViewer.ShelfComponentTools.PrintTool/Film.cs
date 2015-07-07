using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.Dicom.Iod.Modules;


namespace ClearCanvas.ImageViewer.ShelfComponentTools.PrintTool
{
    public class Film
    {
        // Fields
        private string _format;
        private string _id;
        private FilmOrientation _orientation;
        private FilmSize _size;

        // Methods
        public Film(string id, string format, FilmSize size, FilmOrientation orientation)
        {
            this._id = id;
            this._format = format;
            this._size = size;
            this._orientation = orientation;
        }

        // Properties
        public string Format
        {
            get
            {
                return this._format;
            }
            set
            {
                this._format = value;
            }
        }

        public string ID
        {
            get
            {
                return this._id;
            }
            set
            {
                this._id = value;
            }
        }

        public FilmOrientation Orientation
        {
            get
            {
                return this._orientation;
            }
            set
            {
                this._orientation = value;
            }
        }

        public FilmSize Size
        {
            get
            {
                return this._size;
            }
            set
            {
                this._size = value;
            }
        }
    }


    public class OrientationArgs : EventArgs
    {
        // Fields
        private double _filmRatio;
        private FilmOrientation _key;

        // Methods
        public OrientationArgs(FilmOrientation key, double filmRatio)
        {
            this._key = key;
            this._filmRatio = filmRatio;
        }

        // Properties
        public double FilmRatio
        {
            get
            {
                return this._filmRatio;
            }
        }

        public FilmOrientation Orietation
        {
            get
            {
                return this._key;
            }
        }
    }

 

}
