using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

 
using ClearCanvas.Desktop;
using System.ComponentModel;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Desktop.Actions;
using System.Windows.Forms;


using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.Common.Utilities; 

using ClearCanvas.Desktop;
using System.ComponentModel;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.ShelfComponentTools.PrintTool;
using System.Drawing;
namespace ClearCanvas.ImageViewer.ShelfComponentTools.PrintTool.WinForms
{
    public class PrintPreviewControl : UserControl
    {
        // Fields
        private TilesComponent _component;
        private static Color _selectedColor = Color.Orange;
        private PreviewTileViewCollection _tileViews;
        private static Color _unselectedColor = Color.DarkGray;
        private const int BorderWidth = 2;
        private IContainer components;
        private const int InsetWidth = 5;

        // Methods
        public PrintPreviewControl()
        {
            this.InitializeComponent();
        }

        private void AddTileControl(PreviewTileView tile)
        {
            PreviewTileControl control = new PreviewTileControl(tile, base.ClientRectangle, 5);
            control.SuspendLayout();
            base.Controls.Add(control);
            control.ResumeLayout(false);
        }

        private void AddTileControls(PreviewTileViewCollection tiles)
        {
            base.SuspendLayout();
            foreach (PreviewTileView view in tiles)
            {
                this.AddTileControl(view);
            }
            base.ResumeLayout(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void DisposeControls()
        {
            this.DisposeControls(this.GetTileControls());
        }

        private void DisposeControls(IEnumerable<PreviewTileControl> controls)
        {
            foreach (PreviewTileControl control in controls)
            {
                base.Controls.Remove(control);
                control.Dispose();
            }
        }

        private void DisposeTiles()
        {
            if (this._tileViews != null)
            {
                foreach (PreviewTileView view in this._tileViews)
                {
                    view.Dispose();
                }
                this._tileViews.Clear();
            }
        }

        private void DrawBorder(System.Drawing.Graphics graphics, Rectangle rectangle, Color borderColor, int borderWidth, int insetWidth)
        {
            int num = insetWidth / 2;
            Rectangle rect = Rectangle.Inflate(rectangle, -num, -num);
            using (Pen pen = new Pen(borderColor, (float)borderWidth))
            {
                graphics.DrawRectangle(pen, rect);
            }
        }

        private void DrawImageBoxBorder(PaintEventArgs e)
        {
            this.DrawBorder(e.Graphics, base.ClientRectangle, _selectedColor, 2, 5);
        }

        private void DrawTileBorders(PaintEventArgs e)
        {
            if (base.Controls.Count > 1)
            {
                foreach (PreviewTileControl control in base.Controls)
                {
                    Rectangle tileRectangle = this.GetTileRectangle(control);
                    this.DrawBorder(e.Graphics, tileRectangle, control.Tile.BorderColor, PreviewTileView.BorderWidth, PreviewTileView.InsetWidth);
                }
            }
        }

        private List<PreviewTileControl> GetTileControls()
        {
            List<PreviewTileControl> list = new List<PreviewTileControl>();
            foreach (PreviewTileControl control in base.Controls)
            {
                list.Add(control);
            }
            return list;
        }

        private Rectangle GetTileRectangle(PreviewTileControl control)
        {
            Rectangle rect = new Rectangle(control.Location, control.Size);
            return Rectangle.Inflate(rect, PreviewTileView.InsetWidth, PreviewTileView.InsetWidth);
        }

        private void InitializeComponent()
        {
            base.SuspendLayout();
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            //base.AutoScaleMode = AutoScaleMode.Font;
            base.Name = "PrintPreviewControl";
            base.Size = new Size(0x197, 0x18b);
            base.ClientSizeChanged += new EventHandler(this.PrintPreviewControl_ClientSizeChanged);
            base.ResumeLayout(false);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Black);
            this.DrawImageBoxBorder(e);
            this.DrawTileBorders(e);
            base.OnPaint(e);
        }

        private void OnTileActiveChanged(object sender, EventArgs e)
        {
            base.Invalidate();
            base.Update();
        }

        private void PrintPreviewControl_ClientSizeChanged(object sender, EventArgs e)
        {
            base.OnSizeChanged(e);
            base.SuspendLayout();
            foreach (PreviewTileControl control in base.Controls)
            {
                control.SetParentImageBoxRectangle(base.ClientRectangle, 2);
            }
            base.ResumeLayout(false);
            base.Invalidate();
        }

        private void UpdateTiles()
        {
            RectangleF[] tileLayout = this.Component.GetTileLayout();
            for (int i = 0; i < this.Component.TileCount; i++)
            {
                PreviewTileView item = new PreviewTileView(this.Component.GetTile(i))
                {
                    NormalizedRectangle = tileLayout[i]
                };
                this._tileViews.Add(item);
            }
            if (this._tileViews.Count > 0)
            {
                this._tileViews[0].Selected = true;
            }
        }

        private void UpdateView()
        {
            base.SuspendLayout();
            this.DisposeTiles();
            this.DisposeControls();
            this.UpdateTiles();
            this.AddTileControls(this._tileViews);
            base.ResumeLayout(true);
            base.Invalidate();
            base.Update();
        }

        // Properties
        public TilesComponent Component
        {
            get
            {
                return this._component;
            }
            set
            {
                if (value != null)
                {
                    this._component = value;
                    //this._component.LayoutChanged += (,) => this.UpdateView();
                    this._component.LayoutChanged += (object sender, EventArgs e) => this.UpdateView();
                    this._component.TileActiveChanged += new EventHandler(this.OnTileActiveChanged);
                    this._tileViews = new PreviewTileViewCollection();
                    this.UpdateView();
                }
            }
        }
    }

}
