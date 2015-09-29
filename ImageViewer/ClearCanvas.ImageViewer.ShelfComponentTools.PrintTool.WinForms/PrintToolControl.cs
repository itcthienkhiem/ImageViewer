using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using ClearCanvas.ImageViewer.PresentationStates;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using System.Windows.Forms;
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
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.ShelfComponentTools.PrintTool;



namespace ClearCanvas.ImageViewer.ShelfComponentTools.PrintTool.WinForms
{
   


    public class PrintToolControl : UserControl
    {
        // Fields
        private PrintToolComponent _component;
        private DynamicActionButton _FixedToolBtn;
        private DynamicActionButton _freeToolBtn;
        private Point _landscapePoint;
        private Point _portraitPoint;
        private Button btnExport;
        private Button BtnSinglePrint;
        private Button button2;
        private CheckBox checkBox1;
        private CheckBox chkCustom;
        private CustomComboBox cmbFilmOrientation;
        private CustomComboBox cmbFilmSize;
        private CustomComboBox cmbFilmType;
        private CustomComboBox cmbLayout;
        private ComboBox cmbPrinter;
        private IContainer components;
        private PictureBox customPictureBox1;
        private PictureBox customPictureBox2;
        private PictureBox customPictureBox4;
        private FilmsControl filmsControl;
        private Button ibExport;
        private Button ibImport;
        private Label label10;
        private Label label11;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label9;
        private NumericUpDown nupdPrintCount;
        private Panel panPrewControl;
        private PictureBox pictureBox1;
        private PrintPreviewControl printPreviewControl1;
        private SaveFileDialog saveFileDialog;
        private Button button1;
        private ToolTip toolTip;

        // Methods
        public PrintToolControl(PrintToolComponent component)
        {
            this._component = component;
            this.InitializeComponent();
            this.printPreviewControl1.Component = PrintToolComponent.TilesComponent;
            this.cmbFilmType.SetDataSouce("FilmType");
            this.cmbFilmSize.SetDataSouce("FilmSize");
            this.cmbLayout.SetDataSouce("PrintLayout");
            
            
            this.cmbFilmOrientation.SetDataSouce("FilmOrientation");
            BindingSource dataSource = new BindingSource
            {
                DataSource = this._component
            };
            this.cmbPrinter.DataSource = this._component.PrintersView;
            this.cmbPrinter.SelectedItem = this._component.CurrentPrinterValue;
            this.cmbPrinter.DataBindings.Add("SelectedValue", dataSource, "CurrentPrinterValue", true, DataSourceUpdateMode.OnPropertyChanged);
            this.nupdPrintCount.DataBindings.Add("Value", dataSource, "NumberOfCopies", true, DataSourceUpdateMode.OnPropertyChanged);
            this.filmsControl.GridLayoutChange += new EventHandler(this.filmsControl_GridLayoutChange);
            List<Film> films = this._component.GetFilms();

            if (films.Count > 0)
            {
                this.filmsControl.CreateFilmButton(films);
                this.UpdateFilmView(films[0]);
            }
            ////this.BuildActionButtons(this._component.FreeRadioPrintAction, this._component.FixedRadioPrintAction);
            //this._component.ActionChanged += new EventHandler(this.OnActionChanged);

            //this.btnExport.Visible = PrintToolSettings.Default.ExportImage;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            //if (this.saveFileDialog.ShowDialog() == DialogResult.OK)
            //{
            //    string fileName = this.saveFileDialog.FileName;
            //    PrintToolComponent.TilesComponent.ExportSelectedImage(fileName);
            //}

            IEnumerable<IPresentationImage> images = this._component.GetSelectedImages();
            foreach (IPresentationImage image in images)
            {
                PrintToolComponent.AddToClipboard(image);
            }
        }

        private void BuildActionButtons(IAction freeTool, IAction fixedTool)
        {
            base.SuspendLayout();
            IClickAction action = freeTool as IClickAction;
            if (action != null)
            {
                if (this._freeToolBtn == null)
                {
                    this._freeToolBtn = new DynamicActionButton(action);
                    this._freeToolBtn.Location = new Point(0x56, this.BtnSinglePrint.Location.Y);
                    this._freeToolBtn.Size = new Size(0x2e, 0x2e);
                    this.toolTip.SetToolTip(this._freeToolBtn, action.Tooltip);
                    base.Controls.Add(this._freeToolBtn);
                }
                else
                {
                    this._freeToolBtn.Action = action;
                }
            }
            IClickAction action2 = fixedTool as IClickAction;
            if (action2 != null)
            {
                if (this._FixedToolBtn == null)
                {
                    this._FixedToolBtn = new DynamicActionButton(action2);
                    this._FixedToolBtn.Location = new Point(13, this.BtnSinglePrint.Location.Y);
                    this._FixedToolBtn.Size = new Size(0x2e, 0x2e);
                    this.toolTip.SetToolTip(this._FixedToolBtn, action2.Tooltip);
                    base.Controls.Add(this._FixedToolBtn);
                }
                else
                {
                    this._FixedToolBtn.Action = action2;
                }
            }
            base.ResumeLayout(true);
        }

        private Point CalculateLocation(FilmOrientation Status, double size)
        {
            if (Status == FilmOrientation.Portrait)
            {
                this._portraitPoint = new Point(Convert.ToInt32(Math.Round((double)((((double)this.panPrewControl.Width) / 2.0) - (size / 2.0)), 0)), 0);
                return this._portraitPoint;
            }
            this._landscapePoint = new Point(0, Convert.ToInt32(Math.Round((double)((((double)this.panPrewControl.Height) / 2.0) - (size / 2.0)), 0)));
            return this._landscapePoint;
        }

        private void chkCustom_CheckedChanged(object sender, EventArgs e)
        {
            this.filmsControl.Enabled = !this.chkCustom.Checked;
            this.cmbFilmSize.Enabled = this.chkCustom.Checked;
            this.cmbLayout.Enabled = this.chkCustom.Checked;
            this.cmbFilmOrientation.Enabled = this.chkCustom.Checked;
            if (!this.chkCustom.Checked)
            {
                this._component.PrintedFilmSize = this._component.LastFilmSize;
                PrintToolComponent.FilmOrientation = this._component.LasFilmOrientation;
                PrintToolComponent.TilesComponent.SetGrid(this._component.LastFormat);
            }
            else
            {
                this._component.LastFilmSize = this._component.PrintedFilmSize;
                this._component.LasFilmOrientation = PrintToolComponent.FilmOrientation;
                this.cmbLayout_SelectedValueChanged(sender, e);
                this.cmbFilmSize_SelectedValueChanged(sender, e);
                this.cmbFilmOrientation_SelectedValueChanged(sender, e);
            }
            this.UpdatePrinterControl(PrintToolComponent.FilmOrientation, PrintToolComponent.FilmRatio);
        }

        private void cmbFilmOrientation_SelectedValueChanged(object sender, EventArgs e)
        {
            if (((this.cmbFilmOrientation.SelectedValue != null) && this.cmbFilmOrientation.Enabled) && (this.cmbLayout.SelectedValue != null))
            {
                try
                {
                    PrintToolComponent.FilmOrientation = (FilmOrientation)Enum.Parse(typeof(FilmOrientation), this.cmbFilmOrientation.SelectedValue.ToString(), true);
                }
                catch (Exception exception)
                {
                    Platform.Log(LogLevel.Error, exception);
                    PrintToolComponent.FilmOrientation = FilmOrientation.Portrait;
                }
                string format = this.cmbLayout.SelectedValue.ToString();
                PrintToolComponent.TilesComponent.SetGrid(format);
                this.UpdatePrinterControl(PrintToolComponent.FilmOrientation, PrintToolComponent.FilmRatio);
            }
        }

        private void cmbPrinter_SelectedValueChanged(object sender, EventArgs e)
        {
            if (this.cmbPrinter.SelectedValue != null)
            {
                this._component.CurrentPrinterValue = this.cmbPrinter.SelectedValue.ToString();
            }
        }

        private void cmbFilmSize_SelectedValueChanged(object sender, EventArgs e)
        {
            if (((this.cmbFilmSize.SelectedValue != null) && this.cmbFilmSize.Enabled) && (this.cmbLayout.SelectedValue != null))
            {
                try
                {
                   // this._component.PrintedFilmSize = (FilmSize)Enum.Parse(typeof(FilmSize), this.cmbFilmSize.SelectedValue.ToString(), true);
                    this._component.PrintedFilmSize = FilmSize.FromDicomString(this.cmbFilmSize.SelectedValue.ToString());
                }
                catch (Exception exception)
                {
                    Platform.Log(LogLevel.Error, exception);
                    this._component.PrintedFilmSize = FilmSize.Dimension_8in_x_10in;
                }
                string format = this.cmbLayout.SelectedValue.ToString();
                PrintToolComponent.TilesComponent.SetGrid(format);
                this.UpdatePrinterControl(PrintToolComponent.FilmOrientation, PrintToolComponent.FilmRatio);
            }
        }

        private void cmbLayout_SelectedValueChanged(object sender, EventArgs e)
        {
            if (this.cmbLayout.SelectedValue != null)
            {
                try
                {
                    string format = this.cmbLayout.SelectedValue.ToString();
                    PrintToolComponent.TilesComponent.SetGrid(format);
                    this._component.LastFormat = format;
                }
                catch (Exception exception)
                {
                    Platform.Log(LogLevel.Error, exception);
                    PrintToolComponent.TilesComponent.SetGrid(@"STANDARD\1,1");
                }
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

        private void filmsControl_GridLayoutChange(object sender, EventArgs e)
        {
            //ContainerButton button = sender as ContainerButton;
            //if (button != null)
            //{
            //    foreach (Film film in this._component.GetFilms())
            //    {
            //        if (button.Tag.ToString().Equals(film.ID))
            //        {
            //            this.UpdateFilmView(film);
            //            break;
            //        }
            //    }
            //}
            Button button = sender as Button;
            foreach (Film film in this._component.GetFilms())
            {
                if (button.Tag.ToString().Equals(film.ID))
                {
                    this.UpdateFilmView(film);
                    break;
                }
            }
        }

        private void ibExport_Click(object sender, EventArgs e)
        {
            PrintToolComponent.TilesComponent.RemoveActiveTile();
        }

        private void ibImport_Click(object sender, EventArgs e)
        {
            IPresentationImage selectedPI = this._component.GetSelectedPI();
            if (selectedPI != null)
            {
                PrintToolComponent.AddToClipboard(selectedPI);
            }
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.ibImport = new System.Windows.Forms.Button();
            this.ibExport = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.cmbPrinter = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.nupdPrintCount = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.chkCustom = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.panPrewControl = new System.Windows.Forms.Panel();
            this.printPreviewControl1 = new ClearCanvas.ImageViewer.ShelfComponentTools.PrintTool.WinForms.PrintPreviewControl();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label11 = new System.Windows.Forms.Label();
            this.btnExport = new System.Windows.Forms.Button();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.button2 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.BtnSinglePrint = new System.Windows.Forms.Button();
            this.customPictureBox1 = new System.Windows.Forms.PictureBox();
            this.customPictureBox4 = new System.Windows.Forms.PictureBox();
            this.customPictureBox2 = new System.Windows.Forms.PictureBox();
            this.filmsControl = new ClearCanvas.ImageViewer.ShelfComponentTools.PrintTool.WinForms.FilmsControl();
            this.cmbFilmOrientation = new ClearCanvas.ImageViewer.ShelfComponentTools.PrintTool.WinForms.CustomComboBox();
            this.cmbFilmSize = new ClearCanvas.ImageViewer.ShelfComponentTools.PrintTool.WinForms.CustomComboBox();
            this.cmbLayout = new ClearCanvas.ImageViewer.ShelfComponentTools.PrintTool.WinForms.CustomComboBox();
            this.cmbFilmType = new ClearCanvas.ImageViewer.ShelfComponentTools.PrintTool.WinForms.CustomComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.nupdPrintCount)).BeginInit();
            this.panPrewControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.customPictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.customPictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.customPictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // ibImport
            // 
            this.ibImport.Location = new System.Drawing.Point(6, 443);
            this.ibImport.Name = "ibImport";
            this.ibImport.Size = new System.Drawing.Size(82, 33);
            this.ibImport.TabIndex = 80;
            this.ibImport.Text = "添 加";
            this.toolTip.SetToolTip(this.ibImport, "导入");
            this.ibImport.Click += new System.EventHandler(this.ibImport_Click);
            // 
            // ibExport
            // 
            this.ibExport.Location = new System.Drawing.Point(94, 442);
            this.ibExport.Name = "ibExport";
            this.ibExport.Size = new System.Drawing.Size(85, 33);
            this.ibExport.TabIndex = 79;
            this.ibExport.Text = "删 除";
            this.toolTip.SetToolTip(this.ibExport, "导出");
            this.ibExport.Click += new System.EventHandler(this.ibExport_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(94, 481);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(85, 33);
            this.button1.TabIndex = 136;
            this.button1.Text = "删除所有";
            this.toolTip.SetToolTip(this.button1, "导入");
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // cmbPrinter
            // 
            this.cmbPrinter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPrinter.FormattingEnabled = true;
            this.cmbPrinter.Location = new System.Drawing.Point(113, 25);
            this.cmbPrinter.Name = "cmbPrinter";
            this.cmbPrinter.Size = new System.Drawing.Size(121, 20);
            this.cmbPrinter.TabIndex = 96;
            this.cmbPrinter.SelectedValueChanged += new System.EventHandler(this.cmbPrinter_SelectedValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(47, 63);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 107;
            this.label5.Text = "份数";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(17, 371);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 12);
            this.label7.TabIndex = 109;
            this.label7.Text = "布局";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(50, 28);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(41, 12);
            this.label8.TabIndex = 110;
            this.label8.Text = "打印机";
            // 
            // nupdPrintCount
            // 
            this.nupdPrintCount.Location = new System.Drawing.Point(113, 61);
            this.nupdPrintCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nupdPrintCount.Name = "nupdPrintCount";
            this.nupdPrintCount.Size = new System.Drawing.Size(121, 21);
            this.nupdPrintCount.TabIndex = 112;
            this.nupdPrintCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(18, 394);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(29, 12);
            this.label9.TabIndex = 115;
            this.label9.Text = "尺寸";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(-15, -15);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(78, 16);
            this.checkBox1.TabIndex = 117;
            this.checkBox1.Text = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // chkCustom
            // 
            this.chkCustom.AutoSize = true;
            this.chkCustom.Location = new System.Drawing.Point(14, 352);
            this.chkCustom.Name = "chkCustom";
            this.chkCustom.Size = new System.Drawing.Size(48, 16);
            this.chkCustom.TabIndex = 118;
            this.chkCustom.Text = "其他";
            this.chkCustom.UseVisualStyleBackColor = true;
            this.chkCustom.CheckedChanged += new System.EventHandler(this.chkCustom_CheckedChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(18, 419);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(29, 12);
            this.label10.TabIndex = 120;
            this.label10.Text = "方向";
            // 
            // panPrewControl
            // 
            this.panPrewControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panPrewControl.Controls.Add(this.printPreviewControl1);
            this.panPrewControl.Location = new System.Drawing.Point(0, 98);
            this.panPrewControl.Name = "panPrewControl";
            this.panPrewControl.Size = new System.Drawing.Size(250, 231);
            this.panPrewControl.TabIndex = 122;
            // 
            // printPreviewControl1
            // 
            this.printPreviewControl1.Component = null;
            this.printPreviewControl1.Location = new System.Drawing.Point(49, 1);
            this.printPreviewControl1.Name = "printPreviewControl1";
            this.printPreviewControl1.Size = new System.Drawing.Size(145, 230);
            this.printPreviewControl1.TabIndex = 93;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(7, 11);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(45, 48);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 123;
            this.pictureBox1.TabStop = false;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 75);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(29, 12);
            this.label11.TabIndex = 124;
            this.label11.Text = "胶片";
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(6, 482);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(82, 33);
            this.btnExport.TabIndex = 132;
            this.btnExport.Text = "加序列";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "jpg";
            this.saveFileDialog.Filter = "JPEG files(*.jpg)|*.jpg|Bitmap files (*.bmp)|*.bmp|All files (*.*)|*.*";
            this.saveFileDialog.RestoreDirectory = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(194, 730);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(47, 23);
            this.button2.TabIndex = 134;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.SinglePrintImage);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(38, 185);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 108;
            this.label6.Text = "Film Type:";
            this.label6.Visible = false;
            // 
            // BtnSinglePrint
            // 
            this.BtnSinglePrint.Location = new System.Drawing.Point(185, 443);
            this.BtnSinglePrint.Name = "BtnSinglePrint";
            this.BtnSinglePrint.Size = new System.Drawing.Size(62, 70);
            this.BtnSinglePrint.TabIndex = 111;
            this.BtnSinglePrint.Text = "胶片打印";
            this.BtnSinglePrint.Click += new System.EventHandler(this.SinglePrintImage);
            // 
            // customPictureBox1
            // 
            this.customPictureBox1.Location = new System.Drawing.Point(7, 3);
            this.customPictureBox1.Name = "customPictureBox1";
            this.customPictureBox1.Size = new System.Drawing.Size(229, 2);
            this.customPictureBox1.TabIndex = 62;
            this.customPictureBox1.TabStop = false;
            this.customPictureBox1.Text = "customPictureBox1";
            // 
            // customPictureBox4
            // 
            this.customPictureBox4.Location = new System.Drawing.Point(12, 699);
            this.customPictureBox4.Name = "customPictureBox4";
            this.customPictureBox4.Size = new System.Drawing.Size(229, 2);
            this.customPictureBox4.TabIndex = 68;
            this.customPictureBox4.TabStop = false;
            this.customPictureBox4.Text = "customPictureBox4";
            // 
            // customPictureBox2
            // 
            this.customPictureBox2.Location = new System.Drawing.Point(8, 90);
            this.customPictureBox2.Name = "customPictureBox2";
            this.customPictureBox2.Size = new System.Drawing.Size(229, 2);
            this.customPictureBox2.TabIndex = 66;
            this.customPictureBox2.TabStop = false;
            this.customPictureBox2.Text = "customPictureBox2";
            // 
            // filmsControl
            // 
            this.filmsControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(231)))), ((int)(((byte)(246)))));
            this.filmsControl.Location = new System.Drawing.Point(3, 538);
            this.filmsControl.Name = "filmsControl";
            this.filmsControl.Size = new System.Drawing.Size(244, 186);
            this.filmsControl.TabIndex = 135;
            // 
            // cmbFilmOrientation
            // 
            this.cmbFilmOrientation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFilmOrientation.Enabled = false;
            this.cmbFilmOrientation.FormattingEnabled = true;
            this.cmbFilmOrientation.Location = new System.Drawing.Point(102, 416);
            this.cmbFilmOrientation.Name = "cmbFilmOrientation";
            this.cmbFilmOrientation.Size = new System.Drawing.Size(121, 20);
            this.cmbFilmOrientation.TabIndex = 121;
            this.cmbFilmOrientation.SelectedValueChanged += new System.EventHandler(this.cmbFilmOrientation_SelectedValueChanged);
            // 
            // cmbFilmSize
            // 
            this.cmbFilmSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFilmSize.Enabled = false;
            this.cmbFilmSize.FormattingEnabled = true;
            this.cmbFilmSize.Location = new System.Drawing.Point(102, 392);
            this.cmbFilmSize.Name = "cmbFilmSize";
            this.cmbFilmSize.Size = new System.Drawing.Size(121, 20);
            this.cmbFilmSize.TabIndex = 116;
            this.cmbFilmSize.SelectedValueChanged += new System.EventHandler(this.cmbFilmSize_SelectedValueChanged);
            // 
            // cmbLayout
            // 
            this.cmbLayout.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLayout.Enabled = false;
            this.cmbLayout.FormattingEnabled = true;
            this.cmbLayout.Location = new System.Drawing.Point(102, 367);
            this.cmbLayout.Name = "cmbLayout";
            this.cmbLayout.Size = new System.Drawing.Size(121, 20);
            this.cmbLayout.TabIndex = 114;
            this.cmbLayout.SelectedValueChanged += new System.EventHandler(this.cmbLayout_SelectedValueChanged);
            // 
            // cmbFilmType
            // 
            this.cmbFilmType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFilmType.FormattingEnabled = true;
            this.cmbFilmType.Location = new System.Drawing.Point(113, 182);
            this.cmbFilmType.Name = "cmbFilmType";
            this.cmbFilmType.Size = new System.Drawing.Size(121, 20);
            this.cmbFilmType.TabIndex = 113;
            this.cmbFilmType.Visible = false;
            // 
            // PrintToolControl
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(231)))), ((int)(((byte)(246)))));
            this.Controls.Add(this.button1);
            this.Controls.Add(this.filmsControl);
            this.Controls.Add(this.BtnSinglePrint);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.customPictureBox1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.panPrewControl);
            this.Controls.Add(this.cmbFilmOrientation);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.chkCustom);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.cmbFilmSize);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.cmbLayout);
            this.Controls.Add(this.cmbFilmType);
            this.Controls.Add(this.nupdPrintCount);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmbPrinter);
            this.Controls.Add(this.ibImport);
            this.Controls.Add(this.ibExport);
            this.Controls.Add(this.customPictureBox4);
            this.Controls.Add(this.customPictureBox2);
            this.Name = "PrintToolControl";
            this.Size = new System.Drawing.Size(250, 775);
            ((System.ComponentModel.ISupportInitialize)(this.nupdPrintCount)).EndInit();
            this.panPrewControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.customPictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.customPictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.customPictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void OnActionChanged(object sender, EventArgs e)
        {
            //this.BuildActionButtons(this._component.FreeRadioPrintAction, this._component.FixedRadioPrintAction);
        }

        private void SinglePrintImage(object sender, EventArgs e)
        {
            PreviewTileCollection images = PrintToolComponent.TilesComponent.GetImages();
            if (images.Count == 0)
            {
                this._component.ShowMessageBox("无图像");
            }
            else if (this._component.SelectedPrinter == null)
            {
                this._component.ShowMessageBox("无打印机");
            }
            else
            {
                PrintJob job = this._component.CreateJob(images);
                this._component.JobManager.Print(job);
            }
        }

        private void UpdateFilmView(Film film)
        {
            if (film != null)
            {
                PrintToolComponent.FilmOrientation = film.Orientation;
                this._component.PrintedFilmSize = film.Size;
                PrintToolComponent.TilesComponent.SetGrid(film.Format);
                this._component.LastFormat = film.Format;
                this.UpdatePrinterControl(film.Orientation, PrintToolComponent.FilmRatio);
            }
        }

        private void UpdatePrinterControl(FilmOrientation orientation, double filmRatio)
        {
            Size size;
            int width = Convert.ToInt32((double)(this.panPrewControl.Height * filmRatio));
            if (orientation == FilmOrientation.Portrait)
            {
                size = new Size(width, this.panPrewControl.Height);
            }
            else
            {
                size = new Size(this.panPrewControl.Width, width);
            }
            this.printPreviewControl1.Size = size;
            this.printPreviewControl1.Location = this.CalculateLocation(orientation, (double)width);
        }

        private bool VerifyPrintCondition()
        {
            if ((this.cmbPrinter.SelectedValue != null) && !(this.cmbPrinter.SelectedValue.ToString() == ""))
            {
                return true;
            }
            this._component.ShowMessageBox("无法打印");
            return false;
        }

        // Nested Types
        private delegate void UpdateViewDelegate();

        private void button1_Click(object sender, EventArgs e)
        {
            PrintToolComponent.TilesComponent.ResetTiles();
        }
    }


    public class CustomComboBox : ComboBox
    {
        // Fields
        private DataTable cmbDT;

        // Methods
        public void SetDataSouce(string ComboBoxName)
        {
            base.DisplayMember = "Display";
            base.ValueMember = "Value";
            this.cmbDT = GridViewSettings.Default.GetComboBoxValue(ComboBoxName);
            base.DataSource = this.cmbDT;
        }
    }


    public class BasicToolButton : Control
    {
        // Fields
        private Image _activeImage;
        private Image _disableImage;
        private Image _overImage;
        private Image _pressedImage;
        private string _toolTip;
        private bool isChecked;

        // Methods
        public BasicToolButton()
        {
            EventHandler handler = null;
            if (handler == null)
            {
                handler = delegate(object sender, EventArgs args)
                {
                    if ((base.Height != 0x2e) || (base.Width != 0x2e))
                    {
                        Platform.Log(LogLevel.Debug, "SizeChanged event: {0}, {1}", new object[] { base.Width, base.Height });
                        ((BasicToolButton)sender).Size = new Size(0x2e, 0x2e);
                    }
                };
            }
            base.SizeChanged += handler;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!this.isChecked)
            {
                this._activeImage = this._pressedImage;
                base.Invalidate();
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            if (!this.isChecked)
            {
                this._activeImage = this._overImage;
                base.Invalidate();
            }
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (!this.isChecked)
            {
                this._activeImage = this.BackgroundImage;
                base.Invalidate();
            }
            base.OnMouseLeave(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (!this.isChecked)
            {
                this._activeImage = this.BackgroundImage;
                base.Invalidate();
            }
            base.OnMouseUp(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (!base.Enabled && (this._disableImage != null))
            {
                e.Graphics.DrawImage(this._disableImage, 0, 0, 0x2e, 0x2e);
            }
            else if (this.isChecked && (this._pressedImage != null))
            {
                e.Graphics.DrawImage(this._pressedImage, 0, 0, 0x2e, 0x2e);
            }
            else if (!this.isChecked && (this._activeImage != null))
            {
                e.Graphics.DrawImage(this._activeImage, 0, 0, 0x2e, 0x2e);
            }
        }

        // Properties
        public bool Checked
        {
            get
            {
                return this.isChecked;
            }
            set
            {
                if (this.isChecked != value)
                {
                    this.isChecked = value;
                    this._activeImage = this.BackgroundImage;
                    base.Invalidate();
                }
            }
        }

        public Image ImgDisable
        {
            get
            {
                return this._disableImage;
            }
            set
            {
                this._disableImage = value;
                base.Invalidate();
            }
        }

        public Image ImgDown
        {
            get
            {
                return this._pressedImage;
            }
            set
            {
                this._pressedImage = value;
            }
        }

        public Image ImgNormal
        {
            get
            {
                return this.BackgroundImage;
            }
            set
            {
                this.BackgroundImage = value;
                this._activeImage = this.BackgroundImage;
                base.Invalidate();
            }
        }

        public Image ImgOver
        {
            get
            {
                return this._overImage;
            }
            set
            {
                this._overImage = value;
            }
        }

        public string ToolTip
        {
            get
            {
                return this._toolTip;
            }
            set
            {
                this._toolTip = value;
            }
        }
    }


    public class DynamicActionButton : BasicToolButton
    {
        // Fields
        private IClickAction _action;

        // Methods
        public DynamicActionButton(IClickAction action)
        {
            this._action = action;
            base.ToolTip = action.Tooltip;
            this.UpdateIcon();
            base.Click += new EventHandler(this.BindingAction);
            this.BindingActionEvents();
        }

        private void BindingAction(object sender, EventArgs args)
        {
            this._action.Click();
        }

        private void BindingActionEvents()
        {
            base.Enabled = this._action.Enabled;
            base.Checked = this._action.Checked;
            this._action.CheckedChanged += new EventHandler(this.UpdateChecked);
            this._action.EnabledChanged += new EventHandler(this.UpdateEnable);
        }

        private void RemoveActionBinding()
        {
            this._action.CheckedChanged -= new EventHandler(this.UpdateChecked);
            this._action.EnabledChanged -= new EventHandler(this.UpdateEnable);
        }

        private void UpdateChecked(object sender, EventArgs args)
        {
            base.Checked = this._action.Checked;
        }

        private void UpdateEnable(object sender, EventArgs args)
        {
            base.Enabled = this._action.Enabled;
        }

        private void UpdateIcon()
        {
            //if ((this._action.BtnIconSet != null) && (this._action.ResourceResolver != null))
            //{
            //    try
            //    {
            //        Image imgOver = base.ImgOver;
            //        Image imgDown = base.ImgDown;
            //        Image imgNormal = base.ImgNormal;
            //        Image imgDisable = base.ImgDisable;
            //        base.ImgOver = IconFactory.CreateIcon(this._action.BtnIconSet.OverImage, this._action.ResourceResolver);
            //        base.ImgNormal = IconFactory.CreateIcon(this._action.BtnIconSet.UnselectedImage, this._action.ResourceResolver);
            //        base.ImgDown = IconFactory.CreateIcon(this._action.BtnIconSet.SelectedImage, this._action.ResourceResolver);
            //        base.ImgDisable = IconFactory.CreateIcon(this._action.BtnIconSet.DisableImage, this._action.ResourceResolver);
            //        if (imgOver != null)
            //        {
            //            imgOver.Dispose();
            //        }
            //        if (imgDown != null)
            //        {
            //            imgDown.Dispose();
            //        }
            //        if (imgNormal != null)
            //        {
            //            imgNormal.Dispose();
            //        }
            //        if (imgDisable != null)
            //        {
            //            imgDisable.Dispose();
            //        }
            //        base.Invalidate();
            //    }
            //    catch (Exception exception)
            //    {
            //        Platform.Log(LogLevel.Error, exception);
            //    }
            //}
        }

        // Properties
        public IClickAction Action
        {
            get
            {
                return this._action;
            }
            set
            {
                this.RemoveActionBinding();
                this._action = value;
                base.ToolTip = this._action.Tooltip;
                this.BindingActionEvents();
            }
        }

        public string Path
        {
            get
            {
                if (this._action != null)
                {
                    return this._action.Path.ToString();
                }
                return string.Empty;
            }
        }
    }


}
