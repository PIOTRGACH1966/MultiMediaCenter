namespace MultiMediaCenter
{
    partial class ViewForm
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
            this.viewNameTextBox = new System.Windows.Forms.TextBox();
            this.viewNameLabel = new System.Windows.Forms.Label();
            this.changeViewCheck = new System.Windows.Forms.CheckBox();
            this.isHiddenCheck = new System.Windows.Forms.CheckBox();
            this.linkNameTextBox = new System.Windows.Forms.TextBox();
            this.linkNameLabel = new System.Windows.Forms.Label();
            this.lpNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.lpLabel = new System.Windows.Forms.Label();
            this.renumberAfterCheck = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.lpNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // OKButton
            // 
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKButton.Location = new System.Drawing.Point(236, 13);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 9;
            this.OKButton.Text = "&OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(236, 42);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 10;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // viewNameTextBox
            // 
            this.viewNameTextBox.Location = new System.Drawing.Point(80, 15);
            this.viewNameTextBox.Name = "viewNameTextBox";
            this.viewNameTextBox.Size = new System.Drawing.Size(150, 20);
            this.viewNameTextBox.TabIndex = 1;
            this.viewNameTextBox.TextChanged += new System.EventHandler(this.viewNameTextBox_TextChanged);
            // 
            // viewNameLabel
            // 
            this.viewNameLabel.AutoSize = true;
            this.viewNameLabel.Location = new System.Drawing.Point(12, 18);
            this.viewNameLabel.Name = "viewNameLabel";
            this.viewNameLabel.Size = new System.Drawing.Size(62, 13);
            this.viewNameLabel.TabIndex = 0;
            this.viewNameLabel.Text = "&View name:";
            // 
            // changeViewCheck
            // 
            this.changeViewCheck.AutoSize = true;
            this.changeViewCheck.Location = new System.Drawing.Point(165, 93);
            this.changeViewCheck.Name = "changeViewCheck";
            this.changeViewCheck.Size = new System.Drawing.Size(146, 17);
            this.changeViewCheck.TabIndex = 8;
            this.changeViewCheck.Text = "Change &view and all links";
            this.changeViewCheck.UseVisualStyleBackColor = true;
            // 
            // isHiddenCheck
            // 
            this.isHiddenCheck.AutoSize = true;
            this.isHiddenCheck.Location = new System.Drawing.Point(12, 91);
            this.isHiddenCheck.Name = "isHiddenCheck";
            this.isHiddenCheck.Size = new System.Drawing.Size(60, 17);
            this.isHiddenCheck.TabIndex = 7;
            this.isHiddenCheck.Text = "&Hidden";
            this.isHiddenCheck.UseVisualStyleBackColor = true;
            this.isHiddenCheck.CheckedChanged += new System.EventHandler(this.isHiddenCheck_CheckedChanged);
            // 
            // linkNameTextBox
            // 
            this.linkNameTextBox.Location = new System.Drawing.Point(80, 41);
            this.linkNameTextBox.Name = "linkNameTextBox";
            this.linkNameTextBox.Size = new System.Drawing.Size(150, 20);
            this.linkNameTextBox.TabIndex = 3;
            this.linkNameTextBox.TextChanged += new System.EventHandler(this.linkNameTextBox_TextChanged);
            // 
            // linkNameLabel
            // 
            this.linkNameLabel.AutoSize = true;
            this.linkNameLabel.Location = new System.Drawing.Point(12, 44);
            this.linkNameLabel.Name = "linkNameLabel";
            this.linkNameLabel.Size = new System.Drawing.Size(59, 13);
            this.linkNameLabel.TabIndex = 2;
            this.linkNameLabel.Text = "&Link name:";
            // 
            // lpNumericUpDown
            // 
            this.lpNumericUpDown.Location = new System.Drawing.Point(80, 65);
            this.lpNumericUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.lpNumericUpDown.Name = "lpNumericUpDown";
            this.lpNumericUpDown.Size = new System.Drawing.Size(59, 20);
            this.lpNumericUpDown.TabIndex = 5;
            this.lpNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.lpNumericUpDown.ValueChanged += new System.EventHandler(this.lpNumericUpDown_ValueChanged);
            // 
            // lpLabel
            // 
            this.lpLabel.AutoSize = true;
            this.lpLabel.Location = new System.Drawing.Point(12, 67);
            this.lpLabel.Name = "lpLabel";
            this.lpLabel.Size = new System.Drawing.Size(22, 13);
            this.lpLabel.TabIndex = 4;
            this.lpLabel.Text = "&Lp:";
            // 
            // renumberAfterCheck
            // 
            this.renumberAfterCheck.AutoSize = true;
            this.renumberAfterCheck.Location = new System.Drawing.Point(145, 67);
            this.renumberAfterCheck.Name = "renumberAfterCheck";
            this.renumberAfterCheck.Size = new System.Drawing.Size(169, 17);
            this.renumberAfterCheck.TabIndex = 6;
            this.renumberAfterCheck.Text = "&Renumber views after this one";
            this.renumberAfterCheck.UseVisualStyleBackColor = true;
            // 
            // ViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(323, 112);
            this.Controls.Add(this.renumberAfterCheck);
            this.Controls.Add(this.lpNumericUpDown);
            this.Controls.Add(this.lpLabel);
            this.Controls.Add(this.linkNameTextBox);
            this.Controls.Add(this.linkNameLabel);
            this.Controls.Add(this.isHiddenCheck);
            this.Controls.Add(this.changeViewCheck);
            this.Controls.Add(this.viewNameTextBox);
            this.Controls.Add(this.viewNameLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.OKButton);
            this.KeyPreview = true;
            this.Name = "ViewForm";
            this.Text = "View";
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FormAlbum_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.lpNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TextBox viewNameTextBox;
        private System.Windows.Forms.Label viewNameLabel;
        private System.Windows.Forms.CheckBox changeViewCheck;
        private System.Windows.Forms.CheckBox isHiddenCheck;
        private System.Windows.Forms.TextBox linkNameTextBox;
        private System.Windows.Forms.Label linkNameLabel;
        private System.Windows.Forms.NumericUpDown lpNumericUpDown;
        private System.Windows.Forms.Label lpLabel;
        private System.Windows.Forms.CheckBox renumberAfterCheck;
    }
}