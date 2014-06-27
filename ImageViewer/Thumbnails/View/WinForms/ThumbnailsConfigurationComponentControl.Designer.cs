namespace ClearCanvas.ImageViewer.Thumbnails.View.WinForms {
	partial class ThumbnailsConfigurationComponentControl {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ThumbnailsConfigurationComponentControl));
            this._chkAutoOpenThumbnails = new System.Windows.Forms.CheckBox();
            this._chkLeftThumbnails = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // _chkAutoOpenThumbnails
            // 
            resources.ApplyResources(this._chkAutoOpenThumbnails, "_chkAutoOpenThumbnails");
            this._chkAutoOpenThumbnails.Name = "_chkAutoOpenThumbnails";
            this._chkAutoOpenThumbnails.UseVisualStyleBackColor = true;
            // 
            // _chkLeftThumbnails
            // 
            resources.ApplyResources(this._chkLeftThumbnails, "_chkLeftThumbnails");
            this._chkLeftThumbnails.Name = "_chkLeftThumbnails";
            this._chkLeftThumbnails.UseVisualStyleBackColor = true;
            // 
            // ThumbnailsConfigurationComponentControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._chkLeftThumbnails);
            this.Controls.Add(this._chkAutoOpenThumbnails);
            this.Name = "ThumbnailsConfigurationComponentControl";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox _chkAutoOpenThumbnails;
        private System.Windows.Forms.CheckBox _chkLeftThumbnails;
	}
}
