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


namespace ClearCanvas.ImageViewer.ShelfComponentTools.PrintTool
{
    public class PreviewTile : IDisposable
    {
        // Fields
        private Bitmap _icon = null;
        private IPresentationImage _imageData;
        private RectangleF _normalizedRectangle;
        private ushort _position;
        private RectangleF _range;
        private bool _selected;
        private double _tileRatio;

        // Events
        private event EventHandler _OnActived;

        private event EventHandler _OnImageChanged;

        public event EventHandler Actived
        {
            add
            {
                _OnActived += value;
            }
            remove
            {
                _OnActived -= value;
            }
        }

        public event EventHandler ImageChanged
        {
            add
            {
                _OnImageChanged += value;
            }
            remove
            {
                _OnImageChanged -= value;
            }
        }

        // Methods
        public PreviewTile(ushort position, double tileRatio)
        {
            this._position = position;
            this._tileRatio = tileRatio;
        }

        public void Dispose()
        {
            try
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }
            catch (Exception exception)
            {
                Platform.Log(LogLevel.Error, exception);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._imageData != null)
                {
                    this._imageData.Dispose();
                    this._imageData = null;
                }
                if (this._icon != null)
                {
                    this._icon.Dispose();
                    this._icon = null;
                }
            }
        }

        public Bitmap GetPrintImagePixel(bool withAnnotation)
        {
            if (this._range != Rectangle.Empty)
            {
                Platform.Log(LogLevel.Error, "this._range != Rectangle.Empty");
                return IconCreator.CreatePresentationImagePrintData(this._imageData, this._range, this._tileRatio, 1f, withAnnotation);
            }
            return IconCreator.CreatePresentationImagePrintData(this._imageData, this._tileRatio, withAnnotation);
        }

        public void RemoveImage()
        {
            if (this.RemovePresentImage())
            {
                EventsHelper.Fire(this._OnImageChanged, this, EventArgs.Empty);
            }
        }

        private bool RemovePresentImage()
        {
            bool flag = false;
            if (this._icon != null)
            {
                flag = true;
            }
            this.Dispose(true);
            return flag;
        }

        public void SetImage(IPresentationImage imageData)
        {
            this.RemovePresentImage();
            this._range = Rectangle.Empty;
            this._imageData = imageData;
               
            this._icon = IconCreator.CreatePresentationImageIcon(this._imageData, this._tileRatio);
            EventsHelper.Fire(this._OnImageChanged, this, EventArgs.Empty);
        }

        public void SetImage(IPresentationImage pImage, RectangleF range)
        {
            this.RemovePresentImage();
            this._range = range;
            this._imageData = pImage;
            this._icon = IconCreator.CreatePresentationImageIcon(this._imageData, this._range, this._tileRatio);
            EventsHelper.Fire(this._OnImageChanged, this, EventArgs.Empty);
        }

        // Properties
        public Bitmap IconImage
        {
            get
            {
                return this._icon;
            }
        }

        public IPresentationImage ImageData
        {
            get
            {
                return this._imageData;
            }
        }

        public RectangleF NormalizedRectangle
        {
            get
            {
                return this._normalizedRectangle;
            }
            set
            {
                this._normalizedRectangle = value;
            }
        }

        public ushort Position
        {
            get
            {
                return this._position;
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
                this._selected = value;
                if (this._selected)
                {
                    EventsHelper.Fire(this._OnActived, this, EventArgs.Empty);
                }
            }
        }

        public double TileRatio
        {
            get
            {
                return this._tileRatio;
            }
        }
    }


    public class PreviewTileArgs : EventArgs
    {
        // Fields
        private PreviewTile _tile;

        // Methods
        public PreviewTileArgs(PreviewTile tile)
        {
            this._tile = tile;
        }

        // Properties
        public PreviewTile Tile
        {
            get
            {
                return this._tile;
            }
        }
    }



    public class PreviewTileCollection : ObservableList<PreviewTile>
    {
        // Events
         
        private EventHandler _OnActiveChanged;

        //public event EventHandler ActiveChanged;

        // Methods

        public event EventHandler ActiveChanged
        {
            add
            {
                this._OnActiveChanged += value;
            }
            remove
            {
                this._OnActiveChanged -= value;
            }
        }

        public override void Add(PreviewTile tile)
        {
            base.Add(tile);
            tile.Actived += delegate(object sender, EventArgs e)
            {
                foreach (PreviewTile tile1 in this)
                {
                    if ((tile1 != tile) && tile1.Selected)
                    {
                        tile1.Selected = false;
                        break;
                    }
                }
                EventsHelper.Fire(this._OnActiveChanged, sender, e);
            };
        }
    }


}
