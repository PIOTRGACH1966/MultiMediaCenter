using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace MultiMediaCenter
{
    public partial class ItemForm : Form
    {
        public Item Item = null;
        public List<Album> Albums = null;
        public bool ViewOnly = false;
        public string ResourcesFolder = String.Empty;

        private List<Item> Items = null;

        public string ConnectString = String.Empty;

        public bool dataChanged = false;
        public bool linksChanged = false;
        public bool thisLinkDeleted = false;
        public bool otherLinksDeleted = false;
        public bool fileDeleted = false;
        public bool OK = true;

        private Utils u = new Utils();

        public ItemForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            fileSpecTextBox.Text = this.Item.FileSpec;
            lpNumericUpDown.Value = Item.Lp;
            IsImportantCheck.Checked = Item.IsImportant;
            qualityNumericUpDown.Value = Convert.ToInt32(Item.Quality);
            IsArtCheck.Checked = Item.IsArt;
            IsHiddenCheck.Checked = Item.IsHidden;
            this.LoadItems();
            if (ViewOnly)
            {
                fileSpecTextBox.ReadOnly = true;
                lpNumericUpDown.Enabled = false;
                IsImportantCheck.Enabled = false;
                qualityNumericUpDown.Enabled = false;
                IsArtCheck.Enabled = false;
                IsHiddenCheck.Enabled = false;                
                deleteItemButton.Enabled = false;
                deleteFileButton.Enabled = false;
                OKButton.Enabled = false;
                cancelButton.Select();
            }
            else
                lpNumericUpDown.Select();
            
            fileSizeTextBox.Text = u.FileSizeDisplay(this.Item.FileSpec);
            dataChanged = false;

            this.SetQualityDescription();
        }

        private void LoadItems()
        {
            itemsListBox.Items.Clear();
            this.Items = u.GetFileSpecItems(this.Item.FileSpec, this.ConnectString, this.Item);
            foreach(Item item in this.Items)
                itemsListBox.Items.Add(item.CurrentNotes);
        }

        private void FormFolder_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                OKButton.PerformClick();
            else if (e.KeyCode == Keys.Escape)
                this.controlCancelForm();
        }
        private void fileSpecTextBox_TextChanged(object sender, EventArgs e)
        {
            dataChanged = true;
        }
        private void lpNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            dataChanged = true;
        }
        private void IsImportantCheck_CheckedChanged(object sender, EventArgs e)
        {
            dataChanged = true;
        }
        private void qualityNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            dataChanged = true;
            this.SetQualityDescription();
        }
        private void SetQualityDescription()
        {
            qualityDescLabel.Text = u.ItemQualityDescription((ItemQuality)(Convert.ToInt32(qualityNumericUpDown.Value)), false, true);
        }
        private void IsArtCheck_CheckedChanged(object sender, EventArgs e)
        {
            dataChanged = true;
        }
        private void hiddenCheck_CheckedChanged(object sender, EventArgs e)
        {
            dataChanged = true;
        }
        private void itemsListBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                deleteItemButton.PerformClick();
                itemsListBox.Select();
            }
            if (e.KeyCode == Keys.L)
                this.SetSelectedItemQuality(ItemQuality.Low);
            else if (e.KeyCode == Keys.N)
                this.SetSelectedItemQuality(ItemQuality.Normal);
            else if (e.KeyCode == Keys.G)
                this.SetSelectedItemQuality(ItemQuality.Good);
            else if (e.KeyCode == Keys.B)
                this.SetSelectedItemQuality(ItemQuality.Best);
            else if (e.KeyCode == Keys.E)
                this.SetSelectedItemQuality(ItemQuality.Extra);
            else if (e.KeyCode == Keys.A)
                this.SwitchSelectedItemIsArt();
            else if (e.KeyCode == Keys.I || e.KeyValue == 49)
                this.SwitchSelectedItemIsImportant();
            else if (e.KeyCode == Keys.H)
                this.SwitchSelectedItemIsHidden();
        }
        private void SetSelectedItemQuality(ItemQuality _q)
        {
            if (itemsListBox.SelectedIndex >= 0)
            {
                Item item = this.Items[itemsListBox.SelectedIndex];
                item.Load();
                item.Quality = _q;
                item.Save();
                linksChanged = true;
                this.RefreshItemsList();
                if (item.ID == Item.ID && _q != Item.Quality)
                {
                    Item.Quality = item.Quality;
                    qualityNumericUpDown.Value = Convert.ToInt32(Item.Quality);
                    dataChanged = true;
                    cancelButton.Enabled = false;
                }
            }
        }
        private void SwitchSelectedItemIsArt()
        {
            if (itemsListBox.SelectedIndex >= 0)
            {
                Item item = this.Items[itemsListBox.SelectedIndex];
                item.Load();
                item.IsArt = !item.IsArt;
                item.Save();
                linksChanged = true;
                this.RefreshItemsList();
                if (item.ID == Item.ID)
                {
                    Item.IsArt = item.IsArt;
                    IsArtCheck.Checked = Item.IsArt;
                    dataChanged = true;
                    cancelButton.Enabled = false;
                }
            }
        }
        private void SwitchSelectedItemIsImportant()
        {
            if (itemsListBox.SelectedIndex >= 0)
            {
                Item item = this.Items[itemsListBox.SelectedIndex];
                item.Load();
                item.IsImportant = !item.IsImportant;
                item.Save();
                linksChanged = true;
                this.RefreshItemsList();
                if (item.ID == Item.ID)
                {
                    Item.IsImportant = item.IsImportant;
                    IsImportantCheck.Checked = Item.IsImportant;
                    dataChanged = true;
                    cancelButton.Enabled = false;
                }
            }
        }
        private void SwitchSelectedItemIsHidden()
        {
            if (itemsListBox.SelectedIndex >= 0)
            {
                Item item = this.Items[itemsListBox.SelectedIndex];
                item.Load();
                item.IsHidden = !item.IsHidden;
                item.Save();
                linksChanged = true;
                this.RefreshItemsList();
                if (item.ID == Item.ID)
                {
                    Item.IsHidden = item.IsHidden;
                    IsHiddenCheck.Checked = Item.IsHidden;
                    dataChanged = true;
                    cancelButton.Enabled = false;
                }
            }
        }
        private void RefreshItemsList()
        {
            int selectedIndex = itemsListBox.SelectedIndex;
            this.LoadItems();
            itemsListBox.Select();
            itemsListBox.SelectedIndex = selectedIndex;
        }
        private void goToItemButton_Click(object sender, EventArgs e)
        {
            if (itemsListBox.SelectedIndex < 0)
                return;
            if (this.Items[itemsListBox.SelectedIndex].ID == this.Item.ID)
                return;
            ItemForm form = new ItemForm();
            form.Item = this.Items[itemsListBox.SelectedIndex];
            form.ResourcesFolder = this.ResourcesFolder;
            form.Albums = this.Albums;
            form.ConnectString = this.ConnectString;
            form.ViewOnly = this.ViewOnly;
            form.ShowDialog();
            if (form.OK)
                this.LoadItems();
            if (!this.fileDeleted && form.fileDeleted)
            {
                this.fileDeleted = form.fileDeleted;
                this.DisableEditAfterFileDelete();
            }
        }
        private void deleteItemButton_Click(object sender, EventArgs e)
        {
            if (itemsListBox.SelectedIndex >= 0)
            {
                if (this.Items[itemsListBox.SelectedIndex].Delete(true))
                {
                    if (this.Items[itemsListBox.SelectedIndex].ID == this.Item.ID)
                        thisLinkDeleted = true;
                    else
                        otherLinksDeleted = true;
                    this.LoadItems();
                    linksChanged = true;
                }
            }
            if (thisLinkDeleted)
                this.DisableEditAfterFileDelete();
        }
        private void deleteFileButton_Click(object sender, EventArgs e)
        {
            ResourceFolder folder = new ResourceFolder(System.IO.Path.GetDirectoryName(Item.FileSpec), this.ResourcesFolder);
            ResourceFile file = new ResourceFile(folder, Item.FileName, this.ResourcesFolder);
            if (file.Delete(true, this.ConnectString, false))
            {
                fileDeleted = true;
                dataChanged = true;
                linksChanged = true;
            }
            if (fileDeleted)
                this.DisableEditAfterFileDelete();
        }
        private void DisableEditAfterFileDelete()
        {
            this.DisableEditAfterChanges();
            itemsListBox.Items.Clear();
            goToItemButton.Enabled = false;
            deleteItemButton.Enabled = false;
            deleteFileButton.Enabled = false;
        }
        private void DisableEditAfterChanges()
        {
            lpNumericUpDown.Enabled = false;
            IsImportantCheck.Enabled = false;
            qualityNumericUpDown.Enabled = false;
            IsArtCheck.Enabled = false;
            IsHiddenCheck.Enabled = false;
            cancelButton.Enabled = false;
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            this.SaveForm();
        }
        private void cancelButton_Click(object sender, EventArgs e)
        {
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
            if (thisLinkDeleted || fileDeleted)
            {
                OK = true;
                this.Close();
            }
            if (otherLinksDeleted)
                OK = true;
            Item.FileSpec = fileSpecTextBox.Text;
            Item.Lp = Convert.ToInt32(lpNumericUpDown.Value);
            Item.IsImportant = IsImportantCheck.Checked;
            Item.Quality = (ItemQuality)(qualityNumericUpDown.Value);
            Item.IsArt = IsArtCheck.Checked;
            Item.IsHidden = IsHiddenCheck.Checked;
            if (!fileDeleted)
            {
                if (Item.Save())
                {
                    OK = true;
                    this.Close();
                }
            }
            else
            {
                OK = true;
                this.Close();
            }
            ClearKeyboardBuffer();
        }
        private void CancelForm()
        {
            if (otherLinksDeleted || fileDeleted)
                OK = true;
            else
                OK = false;
            this.Close();
            ClearKeyboardBuffer();
        }

        private void ClearKeyboardBuffer()
        {
            this.Enabled = false;
            Application.DoEvents();
            this.Enabled = true;
        }
    }
}
