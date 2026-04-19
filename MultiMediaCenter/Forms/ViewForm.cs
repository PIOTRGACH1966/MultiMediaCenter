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
    public partial class ViewForm : Form
    {
        public View view = null;
        public ViewLink viewLink = null;
        public int Lp { get { return Convert.ToInt32(lpNumericUpDown.Value); } }

        public bool ViewOnly = false;

        public bool OK = false;
        public bool dataChanged = false;
        public bool renumberedAfter = false;
        public bool onLoad = true;

        public ViewForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);            
            changeViewCheck.Checked = (viewLink == null);
            isHiddenCheck.Checked = (view != null && view.ID != 0 ? view.IsHidden : false);
            viewNameTextBox.Text = view.Name;
            if (viewLink != null)
            {
                linkNameTextBox.Text = viewLink.Name;
                linkNameTextBox.Select();
                lpNumericUpDown.Value = viewLink.Lp;
            }
            else
            {
                linkNameTextBox.Enabled = false;
                changeViewCheck.Checked = true;
                changeViewCheck.Enabled = false;
                lpNumericUpDown.Value = 0;
                //lpNumericUpDown.Enabled = false;
            }
            if (ViewOnly)
            {
                viewNameTextBox.ReadOnly = true;
                linkNameTextBox.ReadOnly = true;
                isHiddenCheck.Enabled = false;
                changeViewCheck.Enabled = false;
                lpNumericUpDown.Enabled = false;
                OKButton.Enabled = false;
            }
            dataChanged = false;
            onLoad = false;
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
            if (changeViewCheck.Checked)
            {
                view.Name = viewNameTextBox.Text;
                if (viewLink != null)
                    viewLink.Lp = Convert.ToInt32(lpNumericUpDown.Value);
                view.IsHidden = isHiddenCheck.Checked;
                view.Save(); //Zmieni też (uzgodni) nazwy wszystkich linków tego widoku
            }
            else
            {
                viewLink.Name = linkNameTextBox.Text;
                viewLink.Lp = Convert.ToInt32(lpNumericUpDown.Value);
                viewLink.Save(renumberAfterCheck.Checked);
                renumberedAfter = renumberAfterCheck.Checked;
            }
            OK = true;
            this.Close();
        }
        private void CancelForm()
        {
            OK = false;
            this.Close();
        }

        private void viewNameTextBox_TextChanged(object sender, EventArgs e)
        {
            dataChanged = true;
            if (!onLoad)
            {
                changeViewCheck.Checked = true;
                linkNameTextBox.Text = viewNameTextBox.Text;
            }
        }
        private void linkNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if(!onLoad)
                dataChanged = true;
        }
        private void lpNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!onLoad)
            {
                dataChanged = true;
                renumberAfterCheck.Checked = true;
            }
        }
        private void isHiddenCheck_CheckedChanged(object sender, EventArgs e)
        {
            dataChanged = true;
            if (!onLoad)
                changeViewCheck.Checked = true;
        }
    }
}
