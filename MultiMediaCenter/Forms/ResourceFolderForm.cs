using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MultiMediaCenter
{
    public partial class ResourceFolderForm : Form
    {
        public ResourceFolder folder = null;

        public string ConnectString = String.Empty;

        public bool OK = false;
        public bool dataChanged = false;

        public ResourceFolderForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            nameTextBox.Text = folder.Name;
            dataChanged = false;
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            this.SaveForm();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.controlCancelForm();
        }

        private void FormFolder_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                OKButton.PerformClick();
            else if (e.KeyCode == Keys.Escape)
                this.controlCancelForm();
        }

        private void controlCancelForm()
        {
            if (!dataChanged)
            {
                this.CancelForm();
                return;
            }
            DialogResult dr = MessageBox.Show("Want to save changes?", "Cancelling form...",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.No)
                this.CancelForm();
            else if (dr == DialogResult.Yes)
                this.SaveForm();
        }
        private void SaveForm()
        {
            folder.Name = nameTextBox.Text;
            folder.Save(this.ConnectString, false);
            OK = true;
            this.Close();
        }
        private void CancelForm()
        {
            OK = false;
            this.Close();
        }

        private void nameTextBox_TextChanged(object sender, EventArgs e)
        {
            dataChanged = true;
        }

        private void resourcesFolderPathTextBox_TextChanged(object sender, EventArgs e)
        {
            dataChanged = true;
        }
    }
}
