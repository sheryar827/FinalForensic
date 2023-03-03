namespace Final_Forensic
{
    partial class ImageForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.picBoxSocial = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxSocial)).BeginInit();
            this.SuspendLayout();
            // 
            // picBoxSocial
            // 
            this.picBoxSocial.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picBoxSocial.Location = new System.Drawing.Point(0, 0);
            this.picBoxSocial.Name = "picBoxSocial";
            this.picBoxSocial.Size = new System.Drawing.Size(800, 450);
            this.picBoxSocial.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picBoxSocial.TabIndex = 0;
            this.picBoxSocial.TabStop = false;
            // 
            // ImageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.picBoxSocial);
            this.Name = "ImageForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ImageForm";
            ((System.ComponentModel.ISupportInitialize)(this.picBoxSocial)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picBoxSocial;
    }
}