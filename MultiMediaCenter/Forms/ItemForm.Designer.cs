namespace MultiMediaCenter
{
    partial class ItemForm
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
            this.fileSpecTextBox = new System.Windows.Forms.TextBox();
            this.fSpecLabel = new System.Windows.Forms.Label();
            this.itemsListBox = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.lpNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.lpLabel = new System.Windows.Forms.Label();
            this.deleteItemButton = new System.Windows.Forms.Button();
            this.OKButton = new System.Windows.Forms.Button();
            this.IsHiddenCheck = new System.Windows.Forms.CheckBox();
            this.deleteFileButton = new System.Windows.Forms.Button();
            this.fileSizeTextBox = new System.Windows.Forms.TextBox();
            this.fileSizeLabel = new System.Windows.Forms.Label();
            this.qualityNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.qualityLabel = new System.Windows.Forms.Label();
            this.qualityDescLabel = new System.Windows.Forms.Label();
            this.goToItemButton = new System.Windows.Forms.Button();
            this.IsArtCheck = new System.Windows.Forms.CheckBox();
            this.IsImportantCheck = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.lpNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.qualityNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // fileSpecTextBox
            // 
            this.fileSpecTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fileSpecTextBox.Location = new System.Drawing.Point(43, 15);
            this.fileSpecTextBox.Name = "fileSpecTextBox";
            this.fileSpecTextBox.Size = new System.Drawing.Size(420, 20);
            this.fileSpecTextBox.TabIndex = 1;
            this.fileSpecTextBox.TextChanged += new System.EventHandler(this.fileSpecTextBox_TextChanged);
            // 
            // fSpecLabel
            // 
            this.fSpecLabel.AutoSize = true;
            this.fSpecLabel.Location = new System.Drawing.Point(11, 15);
            this.fSpecLabel.Name = "fSpecLabel";
            this.fSpecLabel.Size = new System.Drawing.Size(26, 13);
            this.fSpecLabel.TabIndex = 0;
            this.fSpecLabel.Text = "&File:";
            // 
            // itemsListBox
            // 
            this.itemsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.itemsListBox.FormattingEnabled = true;
            this.itemsListBox.Location = new System.Drawing.Point(14, 86);
            this.itemsListBox.Name = "itemsListBox";
            this.itemsListBox.Size = new System.Drawing.Size(428, 251);
            this.itemsListBox.TabIndex = 11;
            this.itemsListBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.itemsListBox_KeyUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(203, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "&File of this item is linked to following items:";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(469, 37);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(69, 25);
            this.cancelButton.TabIndex = 20;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // lpNumericUpDown
            // 
            this.lpNumericUpDown.Location = new System.Drawing.Point(44, 41);
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
            this.lpLabel.Location = new System.Drawing.Point(12, 43);
            this.lpLabel.Name = "lpLabel";
            this.lpLabel.Size = new System.Drawing.Size(22, 13);
            this.lpLabel.TabIndex = 2;
            this.lpLabel.Text = "&Lp:";
            // 
            // deleteItemButton
            // 
            this.deleteItemButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteItemButton.Location = new System.Drawing.Point(447, 171);
            this.deleteItemButton.Name = "deleteItemButton";
            this.deleteItemButton.Size = new System.Drawing.Size(90, 40);
            this.deleteItemButton.TabIndex = 15;
            this.deleteItemButton.Text = "Delete selected item (file link)";
            this.deleteItemButton.UseVisualStyleBackColor = true;
            this.deleteItemButton.Click += new System.EventHandler(this.deleteItemButton_Click);
            // 
            // OKButton
            // 
            this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.OKButton.Location = new System.Drawing.Point(469, 12);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(69, 25);
            this.OKButton.TabIndex = 19;
            this.OKButton.Text = "&OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // IsHiddenCheck
            // 
            this.IsHiddenCheck.AutoSize = true;
            this.IsHiddenCheck.Location = new System.Drawing.Point(386, 63);
            this.IsHiddenCheck.Name = "IsHiddenCheck";
            this.IsHiddenCheck.Size = new System.Drawing.Size(60, 17);
            this.IsHiddenCheck.TabIndex = 9;
            this.IsHiddenCheck.Text = "&Hidden";
            this.IsHiddenCheck.UseVisualStyleBackColor = true;
            this.IsHiddenCheck.CheckedChanged += new System.EventHandler(this.hiddenCheck_CheckedChanged);
            // 
            // deleteFileButton
            // 
            this.deleteFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteFileButton.Location = new System.Drawing.Point(447, 217);
            this.deleteFileButton.Name = "deleteFileButton";
            this.deleteFileButton.Size = new System.Drawing.Size(90, 78);
            this.deleteFileButton.TabIndex = 16;
            this.deleteFileButton.Text = "DELETE THIS FILE AND ALL ITS SHORTCUTS AND LINKS";
            this.deleteFileButton.UseVisualStyleBackColor = true;
            this.deleteFileButton.Click += new System.EventHandler(this.deleteFileButton_Click);
            // 
            // fileSizeTextBox
            // 
            this.fileSizeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.fileSizeTextBox.Location = new System.Drawing.Point(457, 317);
            this.fileSizeTextBox.Name = "fileSizeTextBox";
            this.fileSizeTextBox.Size = new System.Drawing.Size(68, 20);
            this.fileSizeTextBox.TabIndex = 18;
            this.fileSizeTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // fileSizeLabel
            // 
            this.fileSizeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.fileSizeLabel.AutoSize = true;
            this.fileSizeLabel.Location = new System.Drawing.Point(468, 300);
            this.fileSizeLabel.Name = "fileSizeLabel";
            this.fileSizeLabel.Size = new System.Drawing.Size(47, 13);
            this.fileSizeLabel.TabIndex = 17;
            this.fileSizeLabel.Text = "File size:";
            // 
            // qualityNumericUpDown
            // 
            this.qualityNumericUpDown.Location = new System.Drawing.Point(271, 41);
            this.qualityNumericUpDown.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.qualityNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.qualityNumericUpDown.Name = "qualityNumericUpDown";
            this.qualityNumericUpDown.Size = new System.Drawing.Size(59, 20);
            this.qualityNumericUpDown.TabIndex = 6;
            this.qualityNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.qualityNumericUpDown.ValueChanged += new System.EventHandler(this.qualityNumericUpDown_ValueChanged);
            // 
            // qualityLabel
            // 
            this.qualityLabel.AutoSize = true;
            this.qualityLabel.Location = new System.Drawing.Point(225, 44);
            this.qualityLabel.Name = "qualityLabel";
            this.qualityLabel.Size = new System.Drawing.Size(42, 13);
            this.qualityLabel.TabIndex = 5;
            this.qualityLabel.Text = "&Quality:";
            // 
            // qualityDescLabel
            // 
            this.qualityDescLabel.AutoSize = true;
            this.qualityDescLabel.Location = new System.Drawing.Point(334, 43);
            this.qualityDescLabel.Name = "qualityDescLabel";
            this.qualityDescLabel.Size = new System.Drawing.Size(46, 13);
            this.qualityDescLabel.TabIndex = 7;
            this.qualityDescLabel.Text = "(Normal)";
            // 
            // goToItemButton
            // 
            this.goToItemButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.goToItemButton.Location = new System.Drawing.Point(447, 86);
            this.goToItemButton.Name = "goToItemButton";
            this.goToItemButton.Size = new System.Drawing.Size(90, 26);
            this.goToItemButton.TabIndex = 12;
            this.goToItemButton.Text = "&Go to link";
            this.goToItemButton.UseVisualStyleBackColor = true;
            this.goToItemButton.Click += new System.EventHandler(this.goToItemButton_Click);
            // 
            // IsArtCheck
            // 
            this.IsArtCheck.AutoSize = true;
            this.IsArtCheck.Location = new System.Drawing.Point(386, 43);
            this.IsArtCheck.Name = "IsArtCheck";
            this.IsArtCheck.Size = new System.Drawing.Size(39, 17);
            this.IsArtCheck.TabIndex = 8;
            this.IsArtCheck.Text = "&Art";
            this.IsArtCheck.UseVisualStyleBackColor = true;
            this.IsArtCheck.CheckedChanged += new System.EventHandler(this.IsArtCheck_CheckedChanged);
            // 
            // IsImportantCheck
            // 
            this.IsImportantCheck.AutoSize = true;
            this.IsImportantCheck.Location = new System.Drawing.Point(149, 43);
            this.IsImportantCheck.Name = "IsImportantCheck";
            this.IsImportantCheck.Size = new System.Drawing.Size(70, 17);
            this.IsImportantCheck.TabIndex = 4;
            this.IsImportantCheck.Text = "&Important";
            this.IsImportantCheck.UseVisualStyleBackColor = true;
            this.IsImportantCheck.CheckedChanged += new System.EventHandler(this.IsImportantCheck_CheckedChanged);
            // 
            // ItemForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(541, 343);
            this.Controls.Add(this.IsImportantCheck);
            this.Controls.Add(this.IsArtCheck);
            this.Controls.Add(this.goToItemButton);
            this.Controls.Add(this.qualityDescLabel);
            this.Controls.Add(this.qualityNumericUpDown);
            this.Controls.Add(this.qualityLabel);
            this.Controls.Add(this.fileSizeTextBox);
            this.Controls.Add(this.fileSizeLabel);
            this.Controls.Add(this.deleteFileButton);
            this.Controls.Add(this.IsHiddenCheck);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.deleteItemButton);
            this.Controls.Add(this.lpNumericUpDown);
            this.Controls.Add(this.lpLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.itemsListBox);
            this.Controls.Add(this.fileSpecTextBox);
            this.Controls.Add(this.fSpecLabel);
            this.KeyPreview = true;
            this.Name = "ItemForm";
            this.Text = "Item";
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FormFolder_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.lpNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.qualityNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox fileSpecTextBox;
        private System.Windows.Forms.Label fSpecLabel;
        private System.Windows.Forms.ListBox itemsListBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.NumericUpDown lpNumericUpDown;
        private System.Windows.Forms.Label lpLabel;
        private System.Windows.Forms.Button deleteItemButton;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.CheckBox IsHiddenCheck;
        private System.Windows.Forms.Button deleteFileButton;
        private System.Windows.Forms.TextBox fileSizeTextBox;
        private System.Windows.Forms.Label fileSizeLabel;
        private System.Windows.Forms.NumericUpDown qualityNumericUpDown;
        private System.Windows.Forms.Label qualityLabel;
        private System.Windows.Forms.Label qualityDescLabel;
        private System.Windows.Forms.Button goToItemButton;
        private System.Windows.Forms.CheckBox IsArtCheck;
        private System.Windows.Forms.CheckBox IsImportantCheck;
    }
}