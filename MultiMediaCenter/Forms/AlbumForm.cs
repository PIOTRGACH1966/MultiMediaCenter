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
    public partial class AlbumForm : Form
    {
        public Album album = null;

        public bool ViewOnly = false;

        public bool OK = false;
        public bool dataChanged = false;

        public AlbumForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            nameTextBox.Text = album.Name;
            lpNumericUpDown.Value = album.Lp;
            if (ViewOnly)
            {
                nameTextBox.ReadOnly = true;
                lpNumericUpDown.Enabled = false;
                OKButton.Enabled = false;
            }
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

        private void FormAlbum_KeyUp(object sender, KeyEventArgs e)
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
            album.Name = nameTextBox.Text;
            album.Lp = Convert.ToInt32(lpNumericUpDown.Value);
            album.Save();
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

        private void lpNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            dataChanged = true;
        }
    }
}
