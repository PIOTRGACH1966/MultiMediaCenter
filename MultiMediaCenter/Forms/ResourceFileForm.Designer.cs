namespace MultiMediaCenter
{
    partial class ResourceFileForm
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
            this.folderSpecTextBox = new System.Windows.Forms.TextBox();
            this.fSpecLabel = new System.Windows.Forms.Label();
            this.itemsListBox = new System.Windows.Forms.ListBox();
            this.itemsListBoxLabel = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.descrTextBox = new System.Windows.Forms.TextBox();
            this.deleteItemButton = new System.Windows.Forms.Button();
            this.fileSizeLabel = new System.Windows.Forms.Label();
            this.fileSizeTextBox = new System.Windows.Forms.TextBox();
            this.shortcutsListBoxLabel = new System.Windows.Forms.Label();
            this.shortcutsListBox = new System.Windows.Forms.ListBox();
            this.deleteShortcutButton = new System.Windows.Forms.Button();
            this.loadShortcutsButton = new System.Windows.Forms.Button();
            this.goToItemButton = new System.Windows.Forms.Button();
            this.deleteFileButton = new System.Windows.Forms.Button();
            this.realFSpecTextBox = new System.Windows.Forms.TextBox();
            this.realFSpecLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // OKButton
            // 
            this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.OKButton.Location = new System.Drawing.Point(485, 13);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 17;
            this.OKButton.Text = "&OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // folderSpecTextBox
            // 
            this.folderSpecTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.folderSpecTextBox.Location = new System.Drawing.Point(66, 15);
            this.folderSpecTextBox.Name = "folderSpecTextBox";
            this.folderSpecTextBox.ReadOnly = true;
            this.folderSpecTextBox.Size = new System.Drawing.Size(243, 20);
            this.folderSpecTextBox.TabIndex = 1;
            // 
            // fSpecLabel
            // 
            this.fSpecLabel.AutoSize = true;
            this.fSpecLabel.Location = new System.Drawing.Point(11, 16);
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
            this.itemsListBox.Location = new System.Drawing.Point(15, 289);
            this.itemsListBox.Name = "itemsListBox";
            this.itemsListBox.Size = new System.Drawing.Size(460, 199);
            this.itemsListBox.TabIndex = 11;
            this.itemsListBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.itemsListBox_KeyUp);
            // 
            // itemsListBoxLabel
            // 
            this.itemsListBoxLabel.AutoSize = true;
            this.itemsListBoxLabel.Location = new System.Drawing.Point(12, 272);
            this.itemsListBoxLabel.Name = "itemsListBoxLabel";
            this.itemsListBoxLabel.Size = new System.Drawing.Size(175, 13);
            this.itemsListBoxLabel.TabIndex = 10;
            this.itemsListBoxLabel.Text = "File is &linked to following view items:";
            // 
            // nameTextBox
            // 
            this.nameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nameTextBox.Location = new System.Drawing.Point(315, 15);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(159, 20);
            this.nameTextBox.TabIndex = 2;
            this.nameTextBox.TextChanged += new System.EventHandler(this.nameTextBox_TextChanged);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(485, 35);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 25);
            this.cancelButton.TabIndex = 18;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // descrTextBox
            // 
            this.descrTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.descrTextBox.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.descrTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.descrTextBox.Location = new System.Drawing.Point(14, 494);
            this.descrTextBox.Multiline = true;
            this.descrTextBox.Name = "descrTextBox";
            this.descrTextBox.ReadOnly = true;
            this.descrTextBox.Size = new System.Drawing.Size(541, 85);
            this.descrTextBox.TabIndex = 16;
            this.descrTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // deleteItemButton
            // 
            this.deleteItemButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteItemButton.Location = new System.Drawing.Point(482, 321);
            this.deleteItemButton.Name = "deleteItemButton";
            this.deleteItemButton.Size = new System.Drawing.Size(75, 53);
            this.deleteItemButton.TabIndex = 13;
            this.deleteItemButton.Text = "Delete selected view item";
            this.deleteItemButton.UseVisualStyleBackColor = true;
            this.deleteItemButton.Click += new System.EventHandler(this.deleteItemButton_Click);
            // 
            // fileSizeLabel
            // 
            this.fileSizeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.fileSizeLabel.AutoSize = true;
            this.fileSizeLabel.Location = new System.Drawing.Point(495, 451);
            this.fileSizeLabel.Name = "fileSizeLabel";
            this.fileSizeLabel.Size = new System.Drawing.Size(47, 13);
            this.fileSizeLabel.TabIndex = 14;
            this.fileSizeLabel.Text = "File size:";
            // 
            // fileSizeTextBox
            // 
            this.fileSizeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.fileSizeTextBox.Location = new System.Drawing.Point(482, 468);
            this.fileSizeTextBox.Name = "fileSizeTextBox";
            this.fileSizeTextBox.ReadOnly = true;
            this.fileSizeTextBox.Size = new System.Drawing.Size(75, 20);
            this.fileSizeTextBox.TabIndex = 15;
            this.fileSizeTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // shortcutsListBoxLabel
            // 
            this.shortcutsListBoxLabel.AutoSize = true;
            this.shortcutsListBoxLabel.Location = new System.Drawing.Point(12, 59);
            this.shortcutsListBoxLabel.Name = "shortcutsListBoxLabel";
            this.shortcutsListBoxLabel.Size = new System.Drawing.Size(102, 13);
            this.shortcutsListBoxLabel.TabIndex = 5;
            this.shortcutsListBoxLabel.Text = "&Shortcuts to this file:";
            // 
            // shortcutsListBox
            // 
            this.shortcutsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.shortcutsListBox.FormattingEnabled = true;
            this.shortcutsListBox.Location = new System.Drawing.Point(15, 73);
            this.shortcutsListBox.Name = "shortcutsListBox";
            this.shortcutsListBox.Size = new System.Drawing.Size(460, 186);
            this.shortcutsListBox.TabIndex = 6;
            this.shortcutsListBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.shortcutsListBox_KeyUp);
            // 
            // deleteShortcutButton
            // 
            this.deleteShortcutButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteShortcutButton.Location = new System.Drawing.Point(481, 119);
            this.deleteShortcutButton.Name = "deleteShortcutButton";
            this.deleteShortcutButton.Size = new System.Drawing.Size(80, 53);
            this.deleteShortcutButton.TabIndex = 8;
            this.deleteShortcutButton.Text = "Delete selected shortcut";
            this.deleteShortcutButton.UseVisualStyleBackColor = true;
            this.deleteShortcutButton.Click += new System.EventHandler(this.deleteShortcutButton_Click);
            // 
            // loadShortcutsButton
            // 
            this.loadShortcutsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.loadShortcutsButton.Location = new System.Drawing.Point(481, 73);
            this.loadShortcutsButton.Name = "loadShortcutsButton";
            this.loadShortcutsButton.Size = new System.Drawing.Size(82, 40);
            this.loadShortcutsButton.TabIndex = 7;
            this.loadShortcutsButton.Text = "Load shortcuts";
            this.loadShortcutsButton.UseVisualStyleBackColor = true;
            this.loadShortcutsButton.Click += new System.EventHandler(this.loadShortcutsButton_Click);
            // 
            // goToItemButton
            // 
            this.goToItemButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.goToItemButton.Location = new System.Drawing.Point(482, 289);
            this.goToItemButton.Name = "goToItemButton";
            this.goToItemButton.Size = new System.Drawing.Size(75, 26);
            this.goToItemButton.TabIndex = 12;
            this.goToItemButton.Text = "&Go to link";
            this.goToItemButton.UseVisualStyleBackColor = true;
            this.goToItemButton.Click += new System.EventHandler(this.goToItemButton_Click);
            // 
            // deleteFileButton
            // 
            this.deleteFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteFileButton.Location = new System.Drawing.Point(481, 178);
            this.deleteFileButton.Name = "deleteFileButton";
            this.deleteFileButton.Size = new System.Drawing.Size(83, 81);
            this.deleteFileButton.TabIndex = 9;
            this.deleteFileButton.Text = "DELETE FILE AND ALL ITS SHORTCUTS AND LINKS";
            this.deleteFileButton.UseVisualStyleBackColor = true;
            this.deleteFileButton.Click += new System.EventHandler(this.deleteFileButton_Click);
            // 
            // realFSpecTextBox
            // 
            this.realFSpecTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.realFSpecTextBox.Location = new System.Drawing.Point(66, 38);
            this.realFSpecTextBox.Name = "realFSpecTextBox";
            this.realFSpecTextBox.ReadOnly = true;
            this.realFSpecTextBox.Size = new System.Drawing.Size(408, 20);
            this.realFSpecTextBox.TabIndex = 4;
            // 
            // realFSpecLabel
            // 
            this.realFSpecLabel.AutoSize = true;
            this.realFSpecLabel.Location = new System.Drawing.Point(12, 41);
            this.realFSpecLabel.Name = "realFSpecLabel";
            this.realFSpecLabel.Size = new System.Drawing.Size(48, 13);
            this.realFSpecLabel.TabIndex = 3;
            this.realFSpecLabel.Text = "Real file:";
            // 
            // ResourceFileForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(567, 581);
            this.Controls.Add(this.realFSpecTextBox);
            this.Controls.Add(this.realFSpecLabel);
            this.Controls.Add(this.deleteFileButton);
            this.Controls.Add(this.goToItemButton);
            this.Controls.Add(this.loadShortcutsButton);
            this.Controls.Add(this.deleteShortcutButton);
            this.Controls.Add(this.shortcutsListBoxLabel);
            this.Controls.Add(this.shortcutsListBox);
            this.Controls.Add(this.fileSizeTextBox);
            this.Controls.Add(this.fileSizeLabel);
            this.Controls.Add(this.deleteItemButton);
            this.Controls.Add(this.descrTextBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.itemsListBoxLabel);
            this.Controls.Add(this.itemsListBox);
            this.Controls.Add(this.folderSpecTextBox);
            this.Controls.Add(this.fSpecLabel);
            this.Controls.Add(this.OKButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.KeyPreview = true;
            this.Name = "ResourceFileForm";
            this.Text = "Resource file";
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FormFolder_KeyUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.TextBox folderSpecTextBox;
        private System.Windows.Forms.Label fSpecLabel;
        private System.Windows.Forms.ListBox itemsListBox;
        private System.Windows.Forms.Label itemsListBoxLabel;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TextBox descrTextBox;
        private System.Windows.Forms.Button deleteItemButton;
        private System.Windows.Forms.Label fileSizeLabel;
        private System.Windows.Forms.TextBox fileSizeTextBox;
        private System.Windows.Forms.Label shortcutsListBoxLabel;
        private System.Windows.Forms.ListBox shortcutsListBox;
        private System.Windows.Forms.Button deleteShortcutButton;
        private System.Windows.Forms.Button loadShortcutsButton;
        private System.Windows.Forms.Button goToItemButton;
        private System.Windows.Forms.Button deleteFileButton;
        private System.Windows.Forms.TextBox realFSpecTextBox;
        private System.Windows.Forms.Label realFSpecLabel;
    }
}