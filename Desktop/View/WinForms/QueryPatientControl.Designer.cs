namespace ClearCanvas.Desktop.View.WinForms
{
    partial class QueryPatientControl
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmbModality = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.edtPatientName = new System.Windows.Forms.TextBox();
            this.edtPatientID = new System.Windows.Forms.TextBox();
            this.dtEnd = new System.Windows.Forms.DateTimePicker();
            this.dtStart = new System.Windows.Forms.DateTimePicker();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnDownImage = new System.Windows.Forms.Button();
            this.btnQuery = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmbModality);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.edtPatientName);
            this.groupBox1.Controls.Add(this.edtPatientID);
            this.groupBox1.Controls.Add(this.dtEnd);
            this.groupBox1.Controls.Add(this.dtStart);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(703, 104);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "查询条件";
            // 
            // cmbModality
            // 
            this.cmbModality.FormattingEnabled = true;
            this.cmbModality.Items.AddRange(new object[] {
            "ALL",
            "DX",
            "CR",
            "CT",
            "MR",
            "RF",
            "MG",
            "DR",
            "DSA",
            "US"});
            this.cmbModality.Location = new System.Drawing.Point(571, 21);
            this.cmbModality.Name = "cmbModality";
            this.cmbModality.Size = new System.Drawing.Size(121, 20);
            this.cmbModality.TabIndex = 17;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(282, 70);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 12);
            this.label4.TabIndex = 16;
            this.label4.Text = "到";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(502, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 13;
            this.label5.Text = "设备类型";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 11;
            this.label3.Text = "诊断日期:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(264, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 10;
            this.label2.Text = "病人姓名:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = "影像号:";
            // 
            // edtPatientName
            // 
            this.edtPatientName.Location = new System.Drawing.Point(339, 21);
            this.edtPatientName.Name = "edtPatientName";
            this.edtPatientName.Size = new System.Drawing.Size(143, 21);
            this.edtPatientName.TabIndex = 8;
            // 
            // edtPatientID
            // 
            this.edtPatientID.Location = new System.Drawing.Point(105, 22);
            this.edtPatientID.Name = "edtPatientID";
            this.edtPatientID.Size = new System.Drawing.Size(140, 21);
            this.edtPatientID.TabIndex = 7;
            // 
            // dtEnd
            // 
            this.dtEnd.Location = new System.Drawing.Point(339, 64);
            this.dtEnd.Name = "dtEnd";
            this.dtEnd.Size = new System.Drawing.Size(143, 21);
            this.dtEnd.TabIndex = 5;
            // 
            // dtStart
            // 
            this.dtStart.Location = new System.Drawing.Point(104, 64);
            this.dtStart.Name = "dtStart";
            this.dtStart.Size = new System.Drawing.Size(140, 21);
            this.dtStart.TabIndex = 4;
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7});
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.Location = new System.Drawing.Point(0, 113);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(706, 203);
            this.listView1.TabIndex = 3;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "影像号";
            this.columnHeader1.Width = 76;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "姓名";
            this.columnHeader2.Width = 43;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "出生年月";
            this.columnHeader3.Width = 90;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "姓别";
            this.columnHeader4.Width = 48;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "诊断日期";
            this.columnHeader5.Width = 98;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "设备类型";
            this.columnHeader6.Width = 86;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "AccesstionNum";
            this.columnHeader7.Width = 149;
            // 
            // btnDownImage
            // 
            this.btnDownImage.Location = new System.Drawing.Point(287, 322);
            this.btnDownImage.Name = "btnDownImage";
            this.btnDownImage.Size = new System.Drawing.Size(87, 25);
            this.btnDownImage.TabIndex = 17;
            this.btnDownImage.Text = "提取";
            this.btnDownImage.UseVisualStyleBackColor = true;
            this.btnDownImage.Click += new System.EventHandler(this.btnDownImage_Click);
            // 
            // btnQuery
            // 
            this.btnQuery.Location = new System.Drawing.Point(45, 322);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(87, 25);
            this.btnQuery.TabIndex = 16;
            this.btnQuery.Text = "查询";
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click_1);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(537, 322);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(87, 23);
            this.btnClose.TabIndex = 18;
            this.btnClose.Text = "返回";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // QueryPatientControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnDownImage);
            this.Controls.Add(this.btnQuery);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.groupBox1);
            this.Name = "QueryPatientControl";
            this.Size = new System.Drawing.Size(711, 354);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cmbModality;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox edtPatientName;
        private System.Windows.Forms.TextBox edtPatientID;
        private System.Windows.Forms.DateTimePicker dtEnd;
        private System.Windows.Forms.DateTimePicker dtStart;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.Button btnDownImage;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.Button btnClose;
    }
}
