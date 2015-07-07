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

using ClearCanvas.Desktop;
using System.ComponentModel;
using ClearCanvas.Desktop.Configuration;


namespace ClearCanvas.ImageViewer.ShelfComponentTools.PrintTool
{
    public class TilesComponent : IDisposable
    {
        // Fields
        private string _format = @"STANDARD\1,1";
        private PreviewTileCollection _tiles = new PreviewTileCollection();
        private EventHandler<PreviewTileArgs> _tileAddedEvents;
        private EventHandler _layoutChanged;
       

        // Events
        //public event EventHandler LayoutChanged;

        //public event EventHandler<PreviewTileArgs> OnTileAdded;

        public event EventHandler TileActiveChanged
        {
            add
            {
                this._tiles.ActiveChanged += value;
            }
            remove
            {
                this._tiles.ActiveChanged -= value;
            }
        }

        public event EventHandler LayoutChanged
        {
            add
            {
                this._layoutChanged += value;
            }
            remove
            {
                this._layoutChanged -= value;
            }
        }

        public event  EventHandler<PreviewTileArgs> OnTileAdded
        {
            add
            {
                this._tileAddedEvents += value;
            }
            remove
            {
                this._tileAddedEvents -= value;
            }
        }


        // Methods
        public TilesComponent()
        {
            this.SetGrid(@"STANDARD\1,1");
        }

        internal void AddImageToAcitveTile(IPresentationImage presentationImage)
        {
            int num = 0;
            foreach (PreviewTile tile in this._tiles)
            {
                num++;
                if (tile.Selected)
                {
                    tile.SetImage(presentationImage);
                    if (num < this.TileCount)
                    {
                        this._tiles[num].Selected = true;
                    }
                    else
                    {
                        this._tiles[0].Selected = true;
                    }
                    EventsHelper.Fire(this._tileAddedEvents, this, new PreviewTileArgs(tile));
                    break;
                }
            }
        }

        internal void AddImageToAcitveTile(IPresentationImage imageData, RectangleF range)
        {
            int num = 0;
            foreach (PreviewTile tile in this._tiles)
            {
                num++;
                if (tile.Selected)
                {
                    tile.SetImage(imageData, range);
                    if (num < this.TileCount)
                    {
                        this._tiles[num].Selected = true;
                    }
                    else
                    {
                        this._tiles[0].Selected = true;
                    }
                    EventsHelper.Fire(this._tileAddedEvents, this, new PreviewTileArgs(tile));
                    break;
                }
            }
        }

        private void ClearAllTiles()
        {
            foreach (PreviewTile tile in this._tiles)
            {
                tile.Dispose();
            }
            this._tiles.Clear();
        }

        public void Dispose()
        {
            foreach (PreviewTile tile in this._tiles)
            {
                tile.Dispose();
            }
            this._tiles.Clear();
        }

        public void ExportSelectedImage(string path)
        {
            PreviewTile tile = null;
            foreach (PreviewTile tile2 in this._tiles)
            {
                if (tile2.Selected)
                {
                    tile = tile2;
                    break;
                }
            }
            if ((tile != null) && (tile.ImageData != null))
            {
                Bitmap printImagePixel = tile.GetPrintImagePixel(true);
                //switch (Path.GetExtension(path))
                //{
                //    case ".jpg":
                //        printImagePixel.Save(path, ImageFormat.Jpeg);
                //        return;

                //    case ".bmp":
                //        printImagePixel.Save(path, ImageFormat.Bmp);
                //        break;
                //}
            }
        }

        public PreviewTileCollection GetImages()
        {
            PreviewTileCollection tiles = new PreviewTileCollection();
            foreach (PreviewTile tile in this._tiles)
            {
                tiles.Add(tile);
            }
            return tiles;
        }

        public double GetSelectedTileRatio()
        {
            foreach (PreviewTile tile in this._tiles)
            {
                if (tile.Selected)
                {
                    return tile.TileRatio;
                }
            }
            return 0.0;
        }

        public PreviewTile GetTile(int index)
        {
            if ((index < 0) || (index >= this._tiles.Count))
            {
                throw new Exception("数据越界");
            }
            return this._tiles[index];
        }

        public RectangleF[] GetTileLayout()
        {
            string str = this._format.Substring(0, 3);
            List<RectangleF> list = new List<RectangleF>();
            if (str.Equals("ROW"))
            {
                string[] strArray = this._format.Substring(4).Split(new char[] { ',' });
                int length = strArray.Length;
                float height = 1f / ((float)length);
                int num3 = 0;
                foreach (string str2 in strArray)
                {
                    int num4 = Convert.ToInt32(str2);
                    float y = num3 * height;
                    float width = 1f / ((float)num4);
                    for (int i = 0; i < num4; i++)
                    {
                        float x = i * width;
                        RectangleF item = new RectangleF(x, y, width, height);
                        list.Add(item);
                    }
                    num3++;
                }
            }
            else if (str.Equals("COL"))
            {
                string[] strArray2 = this._format.Substring(4).Split(new char[] { ',' });
                int num9 = strArray2.Length;
                float num10 = 1f / ((float)num9);
                int num11 = 0;
                foreach (string str3 in strArray2)
                {
                    int num12 = Convert.ToInt32(str3);
                    float num13 = num11 * num10;
                    float num14 = 1f / ((float)num12);
                    for (int j = 0; j < num12; j++)
                    {
                        float num16 = j * num14;
                        RectangleF ef2 = new RectangleF(num13, num16, num10, num14);
                        list.Add(ef2);
                    }
                    num11++;
                }
            }
            else
            {
                string[] strArray3 = this._format.Substring(9).Split(new char[] { ',' });
                int num17 = Convert.ToInt32(strArray3[0]);
                int num18 = Convert.ToInt32(strArray3[1]);
                float num19 = 1f / ((float)num17);
                float num20 = 1f / ((float)num18);
                for (int k = 0; k < num18; k++)
                {
                    for (int m = 0; m < num17; m++)
                    {
                        float num23 = m * num19;
                        float num24 = k * num20;
                        RectangleF ef3 = new RectangleF(num23, num24, num19, num20);
                        list.Add(ef3);
                    }
                }
            }
            return list.ToArray();
        }

        public double GetTileRatio(int index)
        {
            double num = 0.0;
            string str = this._format.Substring(0, 3);
            if (!str.Equals("ROW") && !str.Equals("COL"))
            {
                string[] strArray2 = this._format.Substring(9).Split(new char[] { ',' });
                num = Convert.ToDouble(strArray2[0]) / Convert.ToDouble(strArray2[1]);
            }
            else
            {
                string[] strArray = this._format.Substring(4).Split(new char[] { ',' });
                int length = strArray.Length;
                foreach (string str2 in strArray)
                {
                    int num3 = Convert.ToInt32(str2);
                    index -= num3;
                    if (index < 0)
                    {
                        num = ((double)num3) / ((double)length);
                        break;
                    }
                }
                if (str.Equals("COL"))
                {
                    num = 1.0 / num;
                }
            }
            switch (PrintToolComponent.FilmOrientation)
            {
                case FilmOrientation.Portrait:
                    return ((1.0 / PrintToolComponent.FilmRatio) * num);

                case FilmOrientation.Landscape:
                    return (PrintToolComponent.FilmRatio * num);
            }
            return num;
        }

        public void RemoveActiveTile()
        {
            foreach (PreviewTile tile in this._tiles)
            {
                if (tile.Selected)
                {
                    tile.RemoveImage();
                    break;
                }
            }
        }

        public void Reset()
        {
            this.ResetGrid(@"STANDARD\1,1");
        }

        private void ResetGrid(string format)
        {
            this._format = format;
            this.ClearAllTiles();
            for (int i = 0; i < this.TileCount; i++)
            {
                this._tiles.Add(new PreviewTile((ushort)(i + 1), this.GetTileRatio(i)));
            }
            EventsHelper.Fire(this._layoutChanged, this, EventArgs.Empty);
        }

        public void ResetTiles()
        {
            foreach (PreviewTile tile in this._tiles)
            {
                tile.RemoveImage();
            }
        }

        public void SetGrid(string format)
        {
            this.ResetGrid(format);
        }

        // Properties
        public string Format
        {
            get
            {
                return this._format;
            }
        }

        public int TileCount
        {
            get
            {
                string str = this._format.Substring(0, 3);
                int num = 0;
                if (str.Equals("ROW") || str.Equals("COL"))
                {
                    foreach (string str2 in this._format.Substring(4).Split(new char[] { ',' }))
                    {
                        num += Convert.ToInt32(str2);
                    }
                    return num;
                }
                string[] strArray = this._format.Substring(9).Split(new char[] { ',' });
                return (Convert.ToInt32(strArray[0]) * Convert.ToInt32(strArray[1]));
            }
        }
    }

}
