using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

using ClearCanvas.Desktop;
using System.ComponentModel;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.ShelfComponentTools.PrintTool;
using System.Drawing;

namespace ClearCanvas.ImageViewer.ShelfComponentTools.PrintTool.WinForms
{
    public class PreviewTileView : IDisposable
    {
        // Fields
        private static int _BorderWidth = 1;
        private static int _InsetWidth = 5;
        private static Color _selectedColor = Color.Yellow;
        private PreviewTile _tile;
        private static Color _unselectedColor = Color.Gray;

        // Events
        public event EventHandler Actived
        {
            add
            {
                this._tile.Actived += value;
            }
            remove
            {
                this._tile.Actived -= value;
            }
        }

        public event EventHandler ImageChanged
        {
            add
            {
                this._tile.ImageChanged += value;
            }
            remove
            {
                this._tile.ImageChanged -= value;
            }
        }

        // Methods
        public PreviewTileView(PreviewTile tile)
        {
            this._tile = tile;
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
        }

        // Properties
        public Color BorderColor
        {
            get
            {
                if (this.Selected)
                {
                    return _selectedColor;
                }
                return _unselectedColor;
            }
        }

        public static int BorderWidth
        {
            get
            {
                return _BorderWidth;
            }
            set
            {
                _BorderWidth = value;
            }
        }

        public Bitmap IconImage
        {
            get
            {
                return this._tile.IconImage;
            }
        }

        public static int InsetWidth
        {
            get
            {
                return _InsetWidth;
            }
            set
            {
                _InsetWidth = value;
            }
        }

        public RectangleF NormalizedRectangle
        {
            get
            {
                return this._tile.NormalizedRectangle;
            }
            set
            {
                this._tile.NormalizedRectangle = value;
            }
        }

        public bool Selected
        {
            get
            {
                return this._tile.Selected;
            }
            set
            {
                this._tile.Selected = value;
            }
        }
    }


    public class PreviewTileViewCollection : ObservableList<PreviewTileView>
    {
    }

 

}
