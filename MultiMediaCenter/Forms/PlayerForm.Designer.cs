namespace MultiMediaCenter
{
    partial class PlayerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlayerForm));
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.AVPlayerBox = new AxWMPLib.AxWindowsMediaPlayer();
            this.textBox = new System.Windows.Forms.TextBox();
            this.fileNameLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.AVPlayerBox)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this.pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox.Location = new System.Drawing.Point(0, 0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(283, 260);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            // 
            // AVPlayerBox
            // 
            this.AVPlayerBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AVPlayerBox.Enabled = true;
            this.AVPlayerBox.Location = new System.Drawing.Point(0, 0);
            this.AVPlayerBox.Name = "AVPlayerBox";
            this.AVPlayerBox.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("AVPlayerBox.OcxState")));
            this.AVPlayerBox.Size = new System.Drawing.Size(283, 260);
            this.AVPlayerBox.TabIndex = 1;
            this.AVPlayerBox.KeyUpEvent += new AxWMPLib._WMPOCXEvents_KeyUpEventHandler(this.AVPlayerBox_KeyUpEvent);
            // 
            // textBox
            // 
            this.textBox.Location = new System.Drawing.Point(0, 19);
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(283, 241);
            this.textBox.TabIndex = 2;
            // 
            // fileNameLabel
            // 
            this.fileNameLabel.AutoSize = true;
            this.fileNameLabel.Location = new System.Drawing.Point(2, 3);
            this.fileNameLabel.Name = "fileNameLabel";
            this.fileNameLabel.Size = new System.Drawing.Size(35, 13);
            this.fileNameLabel.TabIndex = 3;
            this.fileNameLabel.Text = "label1";
            // 
            // PlayerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.fileNameLabel);
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.AVPlayerBox);
            this.Controls.Add(this.pictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "PlayerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "FormFullScreenPlayer";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FormFullScreenPlayer_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.AVPlayerBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox;
        private AxWMPLib.AxWindowsMediaPlayer AVPlayerBox;
        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Label fileNameLabel;
    }
}