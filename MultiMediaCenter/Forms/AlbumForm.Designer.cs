namespace MultiMediaCenter
{
    partial class AlbumForm
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
            this.OKButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.nameLabel = new System.Windows.Forms.Label();
            this.lpNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.lpLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.lpNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // OKButton
            // 
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKButton.Location = new System.Drawing.Point(206, 12);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 4;
            this.OKButton.Text = "&OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(206, 41);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // nameTextBox
            // 
            this.nameTextBox.Location = new System.Drawing.Point(61, 15);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(139, 20);
            this.nameTextBox.TabIndex = 1;
            this.nameTextBox.TextChanged += new System.EventHandler(this.nameTextBox_TextChanged);
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(11, 15);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(38, 13);
            this.nameLabel.TabIndex = 0;
            this.nameLabel.Text = "&Name:";
            // 
            // lpNumericUpDown
            // 
            this.lpNumericUpDown.Location = new System.Drawing.Point(61, 41);
            this.lpNumericUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.lpNumericUpDown.Name = "lpNumericUpDown";
            this.lpNumericUpDown.Size = new System.Drawing.Size(59, 20);
            this.lpNumericUpDown.TabIndex = 3;
            this.lpNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.lpNumericUpDown.ValueChanged += new System.EventHandler(this.lpNumericUpDown_ValueChanged);
            // 
            // lpLabel
            // 
            this.lpLabel.AutoSize = true;
            this.lpLabel.Location = new System.Drawing.Point(11, 43);
            this.lpLabel.Name = "lpLabel";
            this.lpLabel.Size = new System.Drawing.Size(22, 13);
            this.lpLabel.TabIndex = 2;
            this.lpLabel.Text = "&Lp:";
            // 
            // AlbumForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 71);
            this.Controls.Add(this.lpNumericUpDown);
            this.Controls.Add(this.lpLabel);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.nameLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.OKButton);
            this.KeyPreview = true;
            this.Name = "AlbumForm";
            this.Text = "Album";
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FormAlbum_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.lpNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.NumericUpDown lpNumericUpDown;
        private System.Windows.Forms.Label lpLabel;
    }
}