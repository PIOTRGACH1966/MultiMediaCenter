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
    public partial class ResourceFileForm : Form
    {
        public ResourceFile File = null;        
        public List<Album> Albums = null;
        public bool ViewOnly = false;
        public bool sfxMode = false;
        public int sfxDirection = 0;
        public string ResourcesFolder = String.Empty;

        private List<string> Shortcuts = null;
        private List<Item> Items = null;

        public string ConnectString = String.Empty;

        public bool dataChanged = false;
        public bool shortcutsDeleted = false;
        public bool linksChanged = false;
        public bool FileDeleted = false;
        public bool OK = true;

        private string realFileSpec = String.Empty;

        private Utils u = new Utils();

        public ResourceFileForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            realFileSpec = this.File.RealFileSpec;
            folderSpecTextBox.Text = System.IO.Path.GetDirectoryName(this.File.FileSpec);
            if (!this.sfxMode)
                nameTextBox.Text = File.FileName;
            else
            {
                string fName = System.IO.Path.GetFileNameWithoutExtension(File.FileSpec);
                if (fName.Contains("_album"))
                {
                    sfxDirection = -1;
                    nameTextBox.Text = fName.Substring(0, fName.Length - "_album".Length) + System.IO.Path.GetExtension(File.FileSpec);
                }
                else
                {
                    sfxDirection = 1;
                    nameTextBox.Text = System.IO.Path.GetFileNameWithoutExtension(File.FileSpec) + "_album" + System.IO.Path.GetExtension(File.FileSpec);
                }
            }
            this.LoadItems();
            if (ViewOnly)
            {
                nameTextBox.ReadOnly = true;
                deleteItemButton.Enabled = false;
                OKButton.Enabled = false;
                itemsListBox.Select();
            }
            else
                nameTextBox.Select();
            if (ViewOnly)
                descrTextBox.Text = "\r\nFormularz otwarty w trybie tylko do podglądu.\r\n" + 
                                    "Nie można dokonywać żadnych zmian.";
            else
            {
                if (!sfxMode)
                {
                    descrTextBox.Text = "Formularz otwarty w trybie zmiany nazwy pliku oraz usuwania skrótów i linków.\r\n" +
                                        "W przypadku zatwierdzenia, zostanie zmieniona nazwa pliku na dysku,\r\n" +
                                        "a dodatkowo zostaną zaktualizowane wszystkie skróty oraz linki w bazie\r\n" + 
                                        "odwołujące się do dotychczasowej nazwy pliku.";
                }
                else
                {
                    deleteShortcutButton.Enabled = false;
                    deleteItemButton.Enabled = false;
                    if(sfxDirection == 1)
                        descrTextBox.Text = "Formularz otwarty w trybie przepięcia linków do nowego pliku z suffixowaną nazwą.\r\n" +
                                            "Zakładamy, że plik z suffixowaną nazwą (np. poprawiony na potrzeby albumów) już istnieje na dysku.\r\n" +
                                            "W przypadku zatwierdzenia, zostaną zaktualizowane wszystkie linki w bazie nazwy niesufixowanej\r\n" +
                                            "na sufixowaną. Plik bez suffixu nie zostanie renamowany (zakładamy, że suffixowany istnieje).";
                    else
                        descrTextBox.Text = "Formularz otwarty w trybie przepięcia linków do pliku z niesuffixowaną nazwą.\r\n" +
                                            "Zakładamy, że plik z niesuffixowaną nazwą (np. sprzed poprawy do albumów) już istnieje na dysku.\r\n" +
                                            "W przypadku zatwierdzenia, zostaną zaktualizowane wszystkie linki w bazie nazwy sufixowanej\r\n" +
                                            "na niesufixowaną. Plik z suffixem nie zostanie renamowany (zakładamy, że niesuffixowany istnieje).";
                }
            }
            if (File.isShortcut)
            {
                realFSpecTextBox.Text = this.realFileSpec;
                nameTextBox.ReadOnly = true;
                shortcutsListBox.Enabled = false;
                shortcutsListBoxLabel.Enabled = false;
                loadShortcutsButton.Enabled = false;
                deleteShortcutButton.Enabled = false;
                fileSizeLabel.Text = "Real file size";
                itemsListBoxLabel.Text = "Real (shortcut target) file is &linked to following view items:";
                for (int i = 1; i <= 7; i++)
                    shortcutsListBox.Items.Add("");
                shortcutsListBox.Items.Add("                                                          (file is shortcut itself)");                
            }
            else
            {
                realFSpecTextBox.Text = this.File.FileSpec;
                for (int i = 1; i <= 7; i++)
                    shortcutsListBox.Items.Add("");
                shortcutsListBox.Items.Add("                                                                 (not loaded)");                
            }
            Utils u = new Utils();
            fileSizeTextBox.Text = u.FileSizeDisplay(this.realFileSpec);
            dataChanged = false;
        }

        private void LoadShortcuts()
        {
            shortcutsListBox.Items.Clear();
            shortcutsListBox.Items.Add("loadding...");
            this.Shortcuts = new List<string>();
            List<string> shList = File.GetShortcuts();
            foreach (string sh in shList)
            {
                this.Shortcuts.Add(sh);
                shortcutsListBox.Items.Add(sh);
            }
            shortcutsListBox.Items.RemoveAt(0);
            if (shortcutsListBox.Items.Count == 0)
                shortcutsListBox.Items.Add("(no shortcuts)");
        }
        private void LoadItems()
        {
            itemsListBox.Items.Clear();
            this.Items = u.GetFileSpecItems(this.realFileSpec, this.ConnectString, null);
            foreach (Item item in this.Items)
                itemsListBox.Items.Add(item.CurrentNotes);
        }

        private void FormFolder_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                OKButton.PerformClick();
            else if (e.KeyCode == Keys.Escape)
                this.controlCancelForm();
        }
        private void nameTextBox_TextChanged(object sender, EventArgs e)
        {
            dataChanged = true;
        }
        private void loadShortcutsButton_Click(object sender, EventArgs e)
        {
            this.LoadShortcuts();
        }
        private void shortcutsListBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                deleteShortcutButton.PerformClick();
                shortcutsListBox.Select();
            }
        }
        private void deleteShortcutButton_Click(object sender, EventArgs e)
        {
            if (this.Shortcuts == null)
                return;
            if (System.Windows.Forms.MessageBox.Show("Are you sure?", "Deleting shortcut", MessageBoxButtons.OKCancel) != DialogResult.OK)
                return;
            if (shortcutsListBox.SelectedIndex >= 0)
            {
                try
                {
                    System.IO.File.Delete(this.Shortcuts[shortcutsListBox.SelectedIndex]);
                    shortcutsDeleted = true;
                    this.LoadShortcuts();
                    linksChanged = true;
                }
                catch(Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Error deleting shortcut [" + this.Shortcuts[shortcutsListBox.SelectedIndex] + "]. " + ex.Message);
                }
            }
        }
        private void deleteFileButton_Click(object sender, EventArgs e)
        {
            ResourceFile deletedFile = null;
            if(!this.File.isShortcut)
                deletedFile = this.File;
            else
            {
                ResourceFolder rf = new ResourceFolder(System.IO.Path.GetDirectoryName(this.realFileSpec), this.ResourcesFolder);
                deletedFile = new ResourceFile(rf, System.IO.Path.GetFileName(this.realFileSpec), this.ResourcesFolder);
            }
            if (deletedFile.Delete(true, this.ConnectString, false))
            {
                FileDeleted = true;
                linksChanged = true;
                dataChanged = true;
                this.Close();
            }
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
                this.RefreshItemsList();
                linksChanged = true;
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
                this.RefreshItemsList();
                linksChanged = true;
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
                this.RefreshItemsList();
                linksChanged = true;
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
                this.RefreshItemsList();
                linksChanged = true;
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
            ItemForm form = new ItemForm();
            form.Item = this.Items[itemsListBox.SelectedIndex];
            form.ResourcesFolder = this.ResourcesFolder;
            form.Albums = this.Albums;
            form.ConnectString = this.ConnectString;
            form.ViewOnly = this.ViewOnly;
            form.ShowDialog();
            if (form.OK)
                this.LoadItems();
            if (form.OK && form.dataChanged)
                dataChanged = true;
            if (form.linksChanged)
                linksChanged = true;
        }
        private void deleteItemButton_Click(object sender, EventArgs e)
        {
            if (itemsListBox.SelectedIndex >= 0)
            {
                if (this.Items[itemsListBox.SelectedIndex].Delete(true))
                {
                    linksChanged = true;
                    this.LoadItems();
                    dataChanged = true;
                }
            }
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
            File.FileName = nameTextBox.Text;
            if (shortcutsDeleted || linksChanged)
                OK = true;
            if (sfxMode)
            {
                File.FileSpec = System.IO.Path.GetDirectoryName(File.FileSpec) + "\\" + File.FileName;
                OK = true;
                this.Close();
            }
            else if (File.FileSpec != System.IO.Path.GetDirectoryName(File.FileSpec) + "\\" + nameTextBox.Text &&
                File.Move(System.IO.Path.GetDirectoryName(File.FileSpec) + "\\" + nameTextBox.Text, this.ConnectString, false))
            {
                File.FileSpec = System.IO.Path.GetDirectoryName(File.FileSpec) + "\\" + File.FileName;
                OK = true;
                this.Close();
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
            if (shortcutsDeleted || linksChanged)
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
