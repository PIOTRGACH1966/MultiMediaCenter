using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace MultiMediaCenter
{
    public class ResourceFile
    {
        public string ResourcesFolder;
        public ResourceFolder ParentFolder;
        public string FileName;
        public string FileSpec;
        public bool isShortcut;        
        public ContentType ContentType;
        public object Thumbnail;
        public List<Item> linkedItemsList;

        private Utils utils = new Utils();

        public ResourceFile(ResourceFolder _parentFolder, string _fileName, string _resourcesFolder)
        {
            this.ResourcesFolder = _resourcesFolder;
            this.ParentFolder = _parentFolder;
            this.FileName = _fileName;
            this.FileSpec = this.ParentFolder.FolderSpec + "\\" + this.FileName;
            this.isShortcut = this.IsShortcut(this.FileSpec);
            this.ContentType = utils.ComputeContentType(this.RealFileSpec);
        }
        private bool IsShortcut(string _fSpec)
        {
            return utils.IsShortcut(_fSpec);
        }
        public string RealFileSpec
        {
            get { return (this.isShortcut ? utils.ShortcutTargetFile(this.FileSpec) : this.FileSpec); }
        }
        public string RealFileName
        {
            get { return System.IO.Path.GetFileName(this.RealFileSpec); }
        }
        public string RealFolderSpec
        {
            get { return System.IO.Path.GetDirectoryName(this.RealFileSpec); }
        }
        public ResourceFolder RealFolder
        {
            get { return new ResourceFolder(this.RealFolderSpec, this.ResourcesFolder); }
        }
        public ResourceFile RealFile
        {
            get { return (this.isShortcut ? new ResourceFile(this.RealFolder, this.RealFileName, this.ResourcesFolder) : this); }
        }
        public string GetDisplayName()
        {
            string retVal = String.Empty;
            if (this.linkedItemsList == null)
                retVal = "(?)" + this.FileName;
            else if (this.linkedItemsList.Count == 0)
                retVal = this.FileName;
            else
            {                
                bool isImportant = false;
                bool isArt = false;
                string qualityDesc = String.Empty;
                foreach (Item it in this.linkedItemsList)
                {
                    if (it.IsImportant)
                        isImportant = true;
                    if (it.IsArt)
                        isArt = true;
                    qualityDesc += utils.ItemQualityDescription(it.Quality, true, false);
                }
                if (isArt)
                    qualityDesc = "*" + qualityDesc;
                if (isImportant)
                    qualityDesc = "!" + qualityDesc;
                if (!isArt && !isImportant)
                    qualityDesc = " " + qualityDesc;
                retVal = qualityDesc;
                retVal += " " + this.FileName;
            }
            return retVal;
        }
        public bool Copy(string _fromSpec, string _toSpec)
        {
            if (System.IO.File.Exists(_toSpec))
            {
                MessageBox.Show("File [" + _toSpec + "] already exists");
                return false;
            }
            File.Copy(_fromSpec, _toSpec);
            string thumbSpecFrom = utils.GetThumbSpec(_fromSpec);
            string thumbSpecTo = utils.GetThumbSpec(_toSpec);
            if (System.IO.File.Exists(thumbSpecFrom))
                System.IO.File.Copy(thumbSpecFrom, thumbSpecTo, true);
            return true;
        }
        public bool Move(string _toSpec, string _connectionString, bool _noCheck)
        {
            if (System.IO.File.Exists(_toSpec))
            {
                MessageBox.Show("File [" + _toSpec + "] already exists");
                return false;
            }

            //Fizyczne przeniesienie pliku
            System.IO.File.Move(this.FileSpec, _toSpec);

            if (!isShortcut)
            {
                if (!_noCheck)
                {
                    //Przepięcie linków do itemów w albumach
                    this.MoveItems(_toSpec, _connectionString);

                    //Przepięcie skrótów wskazujących na plik
                    this.MoveShortcuts(_toSpec);
                }

                //Przeniesienie miniaturki
                this.MoveThumb(_toSpec);
            }

            this.FileSpec = _toSpec;

            return true;
        }
            //Przepięcie linków do itemów w albumach
        public void MoveItems(string _toSpec, string _connectionString)
        {
            if (this.isShortcut)
                return;
            List<Item> itList = this.GetLinkedItems(_connectionString, true);
            foreach (Item it in itList)
                it.ChgFileSpec(_toSpec);
        }
            //Przepięcie skrótów wskazujących na plik
        private void MoveShortcuts(string _toSpec)
        {            
            List<string> shList = this.GetShortcuts();
            foreach (string sh in shList)
                utils.ChangeShortcutTargetFile(sh, _toSpec);
        }
            //Przeniesienie miniaturki
        private void MoveThumb(string _toSpec)
        {
            string thumbSpecFrom = utils.GetThumbSpec(this.FileSpec);
            string thumbSpecTo = utils.GetThumbSpec(_toSpec);
            try
            {
                if (System.IO.File.Exists(thumbSpecFrom))
                    System.IO.File.Move(thumbSpecFrom, thumbSpecTo);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Cannot move thumb file [" + thumbSpecFrom + "] to file [" + thumbSpecTo + "] because of the following error: " + ex.Message);
            }
        }

        public bool Delete(bool _ask, string _connectString, bool _noCheck)
        {
            if (!System.IO.File.Exists(this.FileSpec))
            {
                MessageBox.Show("File does not exist");
                return false;
            }
            if (_ask)
            {
                string shrTxt = (isShortcut ? "\r\n(File shortcuts will not be deleted!)" : String.Empty);
                string wrnTxt = "\r\nOPERATION WILL PERMANENTLY DELETE FILE [" + this.FileSpec + "] FROM DISC!";
                if (_noCheck)
                    wrnTxt += "\r\nProgram will not check if there exist any shortcuts or view items links to this file.";
                else
                    wrnTxt += "\r\nAll shortcuts and view items links to this file will be deleted too.";
                if (MessageBox.Show("ARE YOU SURE?" + wrnTxt + shrTxt,
                    "WARNING",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                    return false;
            }

            if (!_noCheck)
            {
                if(!this.isShortcut)
                    this.DeleteItems(_connectString);

                if (!this.DeleteShortcuts())
                    return false;
            }

            System.IO.File.Delete(this.FileSpec);

            if(this.ParentFolder != null)
                this.ParentFolder.Files.Remove(this);

            this.DeleteThumb();

            return true;
        }
        private void DeleteItems(string _connectString)
        {
            List<Item> itList = this.GetLinkedItems(_connectString, true);
            foreach (Item it in itList)
                it.Delete(false);
        }
        private bool DeleteShortcuts()
        {
            if (this.isShortcut)
                return true;
            List<string> shList = this.GetShortcuts();
            foreach (string sh in shList)
            {
                try
                {
                    System.IO.File.Delete(sh);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Error deleting shortcut [" + sh + "]. " + ex.Message);
                    return false;
                }
            }
            return true;
        }
        private void DeleteThumb()
        {
            if (this.isShortcut)
                return;
            string thumbSpec = utils.GetThumbSpec(this.FileSpec);
            if (System.IO.File.Exists(thumbSpec))
            {
                try
                {
                    File.Delete(thumbSpec);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Cannot delete thumb file " + thumbSpec + " because of the following error: " + ex.Message);
                }
            }
        }

        public List<Item> GetLinkedItems(string _connectString, bool _skipShortcuts)
        {
            List<Item> retVal = new List<Item>();
            if (_skipShortcuts && this.isShortcut)
                return retVal;
            SqlUtils su = new SqlUtils(_connectString);
            string fSpec = (_skipShortcuts ? this.FileSpec : this.RealFileSpec);
            SqlDataReader rd = su.GetSqlReader("SELECT I_ID, I_VID, I_Quality, I_IsArt, I_IsImportant FROM dbo.Items WHERE I_FileSpec = '" + fSpec + "'");
            while (rd.Read())
            {
                View view = new View(Convert.ToInt32(rd["I_VID"]), _connectString);
                Item item = new Item(view, Convert.ToInt32(rd["I_ID"]), _connectString);
                item.IsImportant = Convert.ToBoolean(rd["I_IsImportant"]);
                item.Quality = (ItemQuality)(Convert.ToInt32(rd["I_Quality"]));                
                item.IsArt = Convert.ToBoolean(rd["I_IsArt"]);
                retVal.Add(item);
            }
            rd.Close();
            su.Close();
            if(!_skipShortcuts)
                this.linkedItemsList = retVal;
            return retVal;
        }
        public int GetLinksCount(string _connectString)
        {
            if (this.linkedItemsList == null)
                this.GetLinkedItems(_connectString, false);
            return this.linkedItemsList.Count;
        }

        public List<string> GetShortcuts()
        {
            List<string> retVal = new List<string>();
            if (this.isShortcut)
                return retVal;
            return this.FindShortcutsInFolder(this.ResourcesFolder);
        }
        private List<string> FindShortcutsInFolder(string _fldSpec)
        {
            List<string> retVal = new List<string>();
            string[] files = System.IO.Directory.GetFiles(_fldSpec, "*.LNK");
            foreach (string file in files)
            {
                if (utils.ShortcutTargetFile(file).ToUpper() == this.FileSpec.ToUpper())
                    retVal.Add(file);
            }
            string[] subFolders = System.IO.Directory.GetDirectories(_fldSpec);
            foreach (string subFolder in subFolders)
            {
                List<string> shList = this.FindShortcutsInFolder(subFolder);
                foreach (string sh in shList)
                    retVal.Add(sh);
            }
            return retVal;
        }
    }
}
