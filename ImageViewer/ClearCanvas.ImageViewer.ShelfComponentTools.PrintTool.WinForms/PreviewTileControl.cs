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
using System.Windows.Forms;
using System.Drawing;

using ClearCanvas.ImageViewer.ShelfComponentTools.PrintTool;

namespace ClearCanvas.ImageViewer.ShelfComponentTools.PrintTool.WinForms
{
    public class PreviewTileControl : UserControl
    {
        // Fields
        private PreviewTileView _tile;
        private IContainer components;

        // Methods
        public PreviewTileControl(PreviewTileView tile, Rectangle parentRectangle, int parentImageBoxInsetWidth)
        {
            EventHandler handler = null;
            this._tile = tile;
            if (handler == null)
            {
                handler = delegate(object sender, EventArgs e)
                {
                    
                    Bitmap iconImage = ((PreviewTile)sender).IconImage;
                    this.BackgroundImage = iconImage;
                    base.Invalidate();
                };
            }
            this._tile.ImageChanged += handler;
            this._tile.Actived += new EventHandler(this.OnActived);
            this.InitializeComponent();
            this.SetParentImageBoxRectangle(parentRectangle, parentImageBoxInsetWidth);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            base.SuspendLayout();
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = SystemColors.ControlText;
            this.BackgroundImageLayout = ImageLayout.Zoom;
            base.Name = "PreviewTileControl";
            base.Size = new Size(0xa6, 0xa8);
            base.Click += new EventHandler(this.PreviewTileControl_Click);
            base.ResumeLayout(false);
        }

        private void OnActived(object sender, EventArgs e)
        {
        }

        private void PreviewTileControl_Click(object sender, EventArgs e)
        {
            if (!this._tile.Selected)
            {
                this._tile.Selected = true;
            }
        }

        public void SetParentImageBoxRectangle(Rectangle parentImageBoxRectangle, int parentImageBoxBorderWidth)
        {
            int num = parentImageBoxRectangle.Width - (2 * parentImageBoxBorderWidth);
            int num2 = parentImageBoxRectangle.Height - (2 * parentImageBoxBorderWidth);
            int num3 = ((int)(this._tile.NormalizedRectangle.Left * num)) + PreviewTileView.InsetWidth;
            int num4 = ((int)(this._tile.NormalizedRectangle.Top * num2)) + PreviewTileView.InsetWidth;
            int num5 = ((int)(this._tile.NormalizedRectangle.Right * num)) - PreviewTileView.InsetWidth;
            int num6 = ((int)(this._tile.NormalizedRectangle.Bottom * num2)) - PreviewTileView.InsetWidth;
            base.SuspendLayout();
            base.Location = new Point(num3 + parentImageBoxBorderWidth, num4 + parentImageBoxBorderWidth);
            base.Size = new Size(num5 - num3, num6 - num4);
            base.ResumeLayout(false);
        }

        // Properties
        public PreviewTileView Tile
        {
            get
            {
                return this._tile;
            }
        }
    }
}
