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
        private PictureBox customPictureBox3;
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

            //if (films.Count > 0)
            //{
            //    this.filmsControl.CreateFilmButton(films);
            //    this.UpdateFilmView(films[PrintToolViewSettings.Default.DefaultFilmSizeIndex]);
            //}
            ////this.BuildActionButtons(this._component.FreeRadioPrintAction, this._component.FixedRadioPrintAction);
            //this._component.ActionChanged += new EventHandler(this.OnActionChanged);

            //this.btnExport.Visible = PrintToolSettings.Default.ExportImage;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (this.saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = this.saveFileDialog.FileName;
                PrintToolComponent.TilesComponent.ExportSelectedImage(fileName);
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

        private void cmbFilmSize_SelectedValueChanged(object sender, EventArgs e)
        {
            if (((this.cmbFilmSize.SelectedValue != null) && this.cmbFilmSize.Enabled) && (this.cmbLayout.SelectedValue != null))
            {
                try
                {
                    this._component.PrintedFilmSize = (FilmSize)Enum.Parse(typeof(FilmSize), this.cmbFilmSize.SelectedValue.ToString(), true);
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
            this.components = new Container();
            ComponentResourceManager manager = new ComponentResourceManager(typeof(PrintToolControl));
            this.toolTip = new ToolTip(this.components);
            this.cmbPrinter = new ComboBox();
            this.label5 = new Label();
            this.label7 = new Label();
            this.label8 = new Label();
            this.nupdPrintCount = new NumericUpDown();
            this.label9 = new Label();
            this.checkBox1 = new CheckBox();
            this.chkCustom = new CheckBox();
            this.label10 = new Label();
            this.panPrewControl = new Panel();
            this.pictureBox1 = new PictureBox();
            this.label11 = new Label();
            this.btnExport = new Button();
            this.saveFileDialog = new SaveFileDialog();
            this.button2 = new Button();
            this.label6 = new Label();
            this.filmsControl = new FilmsControl();
            this.BtnSinglePrint = new Button();
            this.customPictureBox1 = new PictureBox();
            this.printPreviewControl1 = new PrintPreviewControl();
            this.cmbFilmOrientation = new CustomComboBox();
            this.cmbFilmSize = new CustomComboBox();
            this.cmbLayout = new CustomComboBox();
            this.cmbFilmType = new CustomComboBox();
            this.ibImport = new Button();
            this.ibExport = new Button();
            this.customPictureBox4 = new PictureBox();
            this.customPictureBox3 = new PictureBox();
            this.customPictureBox2 = new PictureBox();
            this.nupdPrintCount.BeginInit();
            this.panPrewControl.SuspendLayout();
            ((ISupportInitialize)this.pictureBox1).BeginInit();
            ((ISupportInitialize)this.customPictureBox1).BeginInit();
            ((ISupportInitialize)this.customPictureBox4).BeginInit();
            ((ISupportInitialize)this.customPictureBox3).BeginInit();
            ((ISupportInitialize)this.customPictureBox2).BeginInit();
            base.SuspendLayout();
            this.cmbPrinter.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbPrinter.FormattingEnabled = true;
            this.cmbPrinter.Location = new Point(0x71, 0x19);
            this.cmbPrinter.Name = "cmbPrinter";
            this.cmbPrinter.Size = new Size(0x79, 0x15);
            this.cmbPrinter.TabIndex = 0x60;
            this.label5.AutoSize = true;
            this.label5.Location = new Point(0x2f, 0x3f);
            this.label5.Name = "label5";
            this.label5.Size = new Size(0x1f, 13);
            this.label5.TabIndex = 0x6b;
            this.label5.Text = "份数";
            this.label7.AutoSize = true;
            this.label7.Location = new Point(0x15, 0x14c);
            this.label7.Name = "label7";
            this.label7.Size = new Size(0x1f, 13);
            this.label7.TabIndex = 0x6d;
            this.label7.Text = "布局";
            this.label8.AutoSize = true;
            this.label8.Location = new Point(50, 0x1c);
            this.label8.Name = "label8";
            this.label8.Size = new Size(0x2b, 13);
            this.label8.TabIndex = 110;
            this.label8.Text = "打印机";
            this.nupdPrintCount.Location = new Point(0x71, 0x3d);
            int[] bits = new int[4];
            bits[0] = 1;
            this.nupdPrintCount.Minimum = new decimal(bits);
            this.nupdPrintCount.Name = "nupdPrintCount";
            this.nupdPrintCount.Size = new Size(0x79, 20);
            this.nupdPrintCount.TabIndex = 0x70;
            int[] numArray2 = new int[4];
            numArray2[0] = 1;
            this.nupdPrintCount.Value = new decimal(numArray2);
            this.label9.AutoSize = true;
            this.label9.Location = new Point(0x16, 0x163);
            this.label9.Name = "label9";
            this.label9.Size = new Size(0x1f, 13);
            this.label9.TabIndex = 0x73;
            this.label9.Text = "尺寸";
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new Point(-15, -15);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new Size(80, 0x11);
            this.checkBox1.TabIndex = 0x75;
            this.checkBox1.Text = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.chkCustom.AutoSize = true;
            this.chkCustom.Location = new Point(12, 0x137);
            this.chkCustom.Name = "chkCustom";
            this.chkCustom.Size = new Size(50, 0x11);
            this.chkCustom.TabIndex = 0x76;
            this.chkCustom.Text = "其他";
            this.chkCustom.UseVisualStyleBackColor = true;
            this.chkCustom.CheckedChanged += new EventHandler(this.chkCustom_CheckedChanged);
            this.label10.AutoSize = true;
            this.label10.Location = new Point(0x16, 380);
            this.label10.Name = "label10";
            this.label10.Size = new Size(0x1f, 13);
            this.label10.TabIndex = 120;
            this.label10.Text = "方向";
            this.panPrewControl.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.panPrewControl.Controls.Add(this.printPreviewControl1);
            this.panPrewControl.Location = new Point(0, 0x1cf);
            this.panPrewControl.Name = "panPrewControl";
            this.panPrewControl.Size = new Size(250, 230);
            this.panPrewControl.TabIndex = 0x7a;
            //this.pictureBox1.Image = SR.打印机;
            this.pictureBox1.Location = new Point(7, 11);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new Size(0x2d, 0x30);
            this.pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0x7b;
            this.pictureBox1.TabStop = false;
            this.label11.AutoSize = true;
            this.label11.Location = new Point(10, 0x5f);
            this.label11.Name = "label11";
            this.label11.Size = new Size(0x1f, 13);
            this.label11.TabIndex = 0x7c;
            this.label11.Text = "胶片";
            this.btnExport.Location = new Point(0x1f, 420);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new Size(0x2f, 0x21);
            this.btnExport.TabIndex = 0x84;
            this.btnExport.Text = "导出";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Visible = false;
            this.btnExport.Click += new EventHandler(this.btnExport_Click);
            this.saveFileDialog.DefaultExt = "jpg";
            this.saveFileDialog.Filter = "JPEG files(*.jpg)|*.jpg|Bitmap files (*.bmp)|*.bmp|All files (*.*)|*.*";
            this.saveFileDialog.RestoreDirectory = true;
            this.button2.Location = new Point(0xc2, 730);
            this.button2.Name = "button2";
            this.button2.Size = new Size(0x2f, 0x17);
            this.button2.TabIndex = 0x86;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new EventHandler(this.SinglePrintImage);
            this.label6.AutoSize = true;
            this.label6.Location = new Point(0x26, 0xb9);
            this.label6.Name = "label6";
            this.label6.Size = new Size(0x37, 13);
            this.label6.TabIndex = 0x6c;
            this.label6.Text = "Film Type:";
            this.label6.Visible = false;
            this.filmsControl.BackColor = Color.FromArgb(220, 0xe7, 0xf6);
            this.filmsControl.Location = new Point(3, 0x6f);
            this.filmsControl.Name = "filmsControl";
            this.filmsControl.Size = new Size(240, 0xba);
            this.filmsControl.TabIndex = 0x87;
            this.BtnSinglePrint.DialogResult = DialogResult.None;
         
            this.BtnSinglePrint.Location = new Point(0xa9, 0x2ce);
            this.BtnSinglePrint.Name = "BtnSinglePrint";
            this.BtnSinglePrint.Size = new Size(0x48, 0x23);
            this.BtnSinglePrint.TabIndex = 0x6f;
            this.BtnSinglePrint.Text = "imageButton4";
            this.BtnSinglePrint.Click += new EventHandler(this.SinglePrintImage);

            //this.customPictureBox1.BackgroundImage = (Image)manager.GetObject("customPictureBox1.BackgroundImage");
            //this.customPictureBox1.Image = (Image)manager.GetObject("customPictureBox1.Image");
            this.customPictureBox1.Location = new Point(7, 3);
            this.customPictureBox1.Name = "customPictureBox1";
            this.customPictureBox1.Size = new Size(0xe5, 2);
            this.customPictureBox1.TabIndex = 0x3e;
            this.customPictureBox1.TabStop = false;
            this.customPictureBox1.Text = "customPictureBox1";
            this.printPreviewControl1.Component = null;
            this.printPreviewControl1.Location = new Point(0x35, 0);
            this.printPreviewControl1.Name = "printPreviewControl1";
            this.printPreviewControl1.Size = new Size(0x91, 230);
            this.printPreviewControl1.TabIndex = 0x5d;
            this.cmbFilmOrientation.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbFilmOrientation.Enabled = false;
            this.cmbFilmOrientation.FormattingEnabled = true;
            this.cmbFilmOrientation.Location = new Point(0x6a, 0x179);
            this.cmbFilmOrientation.Name = "cmbFilmOrientation";
            this.cmbFilmOrientation.Size = new Size(0x79, 0x15);
            this.cmbFilmOrientation.TabIndex = 0x79;
            this.cmbFilmOrientation.SelectedValueChanged += new EventHandler(this.cmbFilmOrientation_SelectedValueChanged);
            this.cmbFilmSize.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbFilmSize.Enabled = false;
            this.cmbFilmSize.FormattingEnabled = true;
            this.cmbFilmSize.Location = new Point(0x6a, 0x161);
            this.cmbFilmSize.Name = "cmbFilmSize";
            this.cmbFilmSize.Size = new Size(0x79, 0x15);
            this.cmbFilmSize.TabIndex = 0x74;
            this.cmbFilmSize.SelectedValueChanged += new EventHandler(this.cmbFilmSize_SelectedValueChanged);
            this.cmbLayout.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbLayout.Enabled = false;
            this.cmbLayout.FormattingEnabled = true;
            this.cmbLayout.Location = new Point(0x6a, 0x148);
            this.cmbLayout.Name = "cmbLayout";
            this.cmbLayout.Size = new Size(0x79, 0x15);
            this.cmbLayout.TabIndex = 0x72;
            this.cmbLayout.SelectedValueChanged += new EventHandler(this.cmbLayout_SelectedValueChanged);
            this.cmbFilmType.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbFilmType.FormattingEnabled = true;
            this.cmbFilmType.Location = new Point(0x71, 0xb6);
            this.cmbFilmType.Name = "cmbFilmType";
            this.cmbFilmType.Size = new Size(0x79, 0x15);
            this.cmbFilmType.TabIndex = 0x71;
            this.cmbFilmType.Visible = false;
            this.ibImport.DialogResult = DialogResult.None;
          
            this.ibImport.Location = new Point(0x69, 0x1a0);
            this.ibImport.Name = "ibImport";
            this.ibImport.Size = new Size(60, 0x24);
            this.ibImport.TabIndex = 80;
            this.toolTip.SetToolTip(this.ibImport, "导入");
            this.ibImport.Click += new EventHandler(this.ibImport_Click);
            this.ibExport.DialogResult = DialogResult.None;
           
            this.ibExport.Location = new Point(0xa9, 0x1a0);
            this.ibExport.Name = "ibExport";
            this.ibExport.Size = new Size(60, 0x24);
            this.ibExport.TabIndex = 0x4f;
            this.toolTip.SetToolTip(this.ibExport, "导出");
            this.ibExport.Click += new EventHandler(this.ibExport_Click);
            //this.customPictureBox4.BackgroundImage = (Image)manager.GetObject("customPictureBox4.BackgroundImage");
            //this.customPictureBox4.Image = (Image)manager.GetObject("customPictureBox4.Image");
            this.customPictureBox4.Location = new Point(12, 0x2bb);
            this.customPictureBox4.Name = "customPictureBox4";
            this.customPictureBox4.Size = new Size(0xe5, 2);
            this.customPictureBox4.TabIndex = 0x44;
            this.customPictureBox4.TabStop = false;
            this.customPictureBox4.Text = "customPictureBox4";
            //this.customPictureBox3.BackgroundImage = (Image)manager.GetObject("customPictureBox3.BackgroundImage");
            //this.customPictureBox3.Image = (Image)manager.GetObject("customPictureBox3.Image");
            this.customPictureBox3.Location = new Point(8, 0x197);
            this.customPictureBox3.Name = "customPictureBox3";
            this.customPictureBox3.Size = new Size(0xe5, 2);
            this.customPictureBox3.TabIndex = 0x43;
            this.customPictureBox3.TabStop = false;
            this.customPictureBox3.Text = "customPictureBox3";
            //this.customPictureBox2.BackgroundImage = (Image)manager.GetObject("customPictureBox2.BackgroundImage");
            //this.customPictureBox2.Image = (Image)manager.GetObject("customPictureBox2.Image");
            this.customPictureBox2.Location = new Point(8, 90);
            this.customPictureBox2.Name = "customPictureBox2";
            this.customPictureBox2.Size = new Size(0xe5, 2);
            this.customPictureBox2.TabIndex = 0x42;
            this.customPictureBox2.TabStop = false;
            this.customPictureBox2.Text = "customPictureBox2";
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(220, 0xe7, 0xf6);
            base.Controls.Add(this.filmsControl);
            base.Controls.Add(this.BtnSinglePrint);
            base.Controls.Add(this.button2);
            base.Controls.Add(this.btnExport);
            base.Controls.Add(this.label11);
            base.Controls.Add(this.customPictureBox1);
            base.Controls.Add(this.pictureBox1);
            base.Controls.Add(this.panPrewControl);
            base.Controls.Add(this.cmbFilmOrientation);
            base.Controls.Add(this.label10);
            base.Controls.Add(this.chkCustom);
            base.Controls.Add(this.checkBox1);
            base.Controls.Add(this.cmbFilmSize);
            base.Controls.Add(this.label9);
            base.Controls.Add(this.cmbLayout);
            base.Controls.Add(this.cmbFilmType);
            base.Controls.Add(this.nupdPrintCount);
            base.Controls.Add(this.label8);
            base.Controls.Add(this.label7);
            base.Controls.Add(this.label6);
            base.Controls.Add(this.label5);
            base.Controls.Add(this.cmbPrinter);
            base.Controls.Add(this.ibImport);
            base.Controls.Add(this.ibExport);
            base.Controls.Add(this.customPictureBox4);
            base.Controls.Add(this.customPictureBox3);
            base.Controls.Add(this.customPictureBox2);
            base.Name = "PrintToolControl";
            base.Size = new Size(250, 0x307);
            this.nupdPrintCount.EndInit();
            this.panPrewControl.ResumeLayout(false);
            ((ISupportInitialize)this.pictureBox1).EndInit();
            ((ISupportInitialize)this.customPictureBox1).EndInit();
            ((ISupportInitialize)this.customPictureBox4).EndInit();
            ((ISupportInitialize)this.customPictureBox3).EndInit();
            ((ISupportInitialize)this.customPictureBox2).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
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
                this._component.ShowMessageBox(string.Format("无图像", new object[0]));
            }
            //else if (this._component.SelectedPrinter == null)
            //{
            //    this._component.ShowMessageBox("无打印机");
            //}
            //else
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
    }

}
