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
    public class FilmsControl : UserControl
    {
        // Fields
        private Button _btnActive;
        private IContainer components;
       
        private FlowLayoutPanel flowLayoutPanel;

        // Events
        private event EventHandler _gridLayoutChange;

        public event EventHandler GridLayoutChange
        {
            add { _gridLayoutChange += value; }
            remove { _gridLayoutChange -= value; }
        }



        // Methods
        public FilmsControl()
        {
            this.InitializeComponent();
        }

        public void CreateFilmButton(List<Film> films)
        {
            int num = 0;
            base.SuspendLayout();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FilmsControl));
       
            foreach (Film film in films)
            {
                //ContainerButton button = new ContainerButton
                //{
                //    ImgNormal = (Bitmap)SR.ResourceManager.GetObject(string.Format("{0}{1}", film.ID, "_normal"), SR.Culture),
                //    ImgDown = (Bitmap)SR.ResourceManager.GetObject(string.Format("{0}{1}", film.ID, "_selected"), SR.Culture),
                //    ImgOver = (Bitmap)SR.ResourceManager.GetObject(string.Format("{0}{1}", film.ID, "_hover"), SR.Culture),
                //    ImgDisable = (Bitmap)SR.ResourceManager.GetObject(string.Format("{0}{1}", film.ID, "_disabled"), SR.Culture),
                //    Tag = film.ID,
                //    Index = num,
                //    IsTabButton = true
                //};
                //button.Click += new EventHandler(this.FilmButton_Click);
                //this.flowLayoutPanel.Controls.Add(button);
                //if (PrintToolViewSettings.Default.DefaultFilmSizeIndex == num)
                //{
                //    this._btnActive = button;
                //}
                Button btn = new Button();
                btn.Size = new Size(74, 40);
                 
                //btn.Location = new Point(3 + num * 74, 3);
                string strFormat = string.Format("button{0}.Image", num+1);
                btn.Image = ((System.Drawing.Image)(resources.GetObject(strFormat)));
                btn.Tag = film.ID;
                btn.TabIndex = num;
                
                this.flowLayoutPanel.Controls.Add(btn);
                
                btn.Click += new EventHandler(this.FilmButton_Click);
                num++;
            }
            base.ResumeLayout(false);
            if (this._btnActive != null)
            {
                //this._btnActive.ReDraw(true);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void FilmButton_Click(object sender, EventArgs e)
        {
            EventsHelper.Fire(this._gridLayoutChange, sender, EventArgs.Empty);
            //if (this._btnActive != null)
            //{
            //    this._btnActive.ReDraw(false);
            //}
            //this._btnActive = sender as ContainerButton;
            //if (this._btnActive != null)
            //{
            //    this._btnActive.ReDraw(true);
            //}
        }

        private void InitializeComponent()
        {
            this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // flowLayoutPanel
            // 
            this.flowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel.Location = new System.Drawing.Point(0, 3);
            this.flowLayoutPanel.Name = "flowLayoutPanel";
            this.flowLayoutPanel.Size = new System.Drawing.Size(239, 180);
            this.flowLayoutPanel.TabIndex = 121;
            // 
            // FilmsControl
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(231)))), ((int)(((byte)(246)))));
            this.Controls.Add(this.flowLayoutPanel);
            this.Name = "FilmsControl";
            this.Size = new System.Drawing.Size(240, 186);
            this.ResumeLayout(false);

        }


        
    }
}
