using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace MultiMediaCenter
{
    public class ResourceFolder
    {
        public string ResourcesFolder = String.Empty;
        public ResourceFile File = null;
        public ResourceFolder ParentFolder;
        public string FolderSpec;
        private string name;
        public string Name
        {
            get { return name; }
            set 
            {
                name = value;
                if (this.ParentFolder != null)
                    this.FolderSpec = this.ParentFolder.FolderSpec + "\\" + name;
                else
                {
                    int p = this.FolderSpec.LastIndexOf("\\");
                    this.FolderSpec = this.FolderSpec.Substring(0, p+1) + name;
                }
            }
        }
        public bool Loaded;

        public List<ResourceFolder> subFolders = null;
        public List<ResourceFile> Files = null;

        private bool isNew = false;
        private string oldPath = String.Empty;

        private Utils utils = new Utils();

        public ResourceFolder(string _fullPath, string _resourcesFolder)
        {
            this.ResourcesFolder = _resourcesFolder;
            this.FolderSpec = _fullPath;
            isNew = !System.IO.Directory.Exists(this.FolderSpec);
            oldPath = this.FolderSpec;
            this.name = System.IO.Path.GetFileName(this.FolderSpec);
            subFolders = new List<ResourceFolder>();
            Files = new List<ResourceFile>();
            this.Loaded = false;
        }

        public ResourceFolder(ResourceFolder _parentFolder, string _name, string _resourcesFolder)
        {
            this.ResourcesFolder = _resourcesFolder;
            this.ParentFolder = _parentFolder;
            this.Name = _name;
            isNew = (String.IsNullOrEmpty(_name) || !System.IO.Directory.Exists(this.FolderSpec));
            oldPath = this.FolderSpec;
            subFolders = new List<ResourceFolder>();
            Files = new List<ResourceFile>();
            this.Loaded = false;
        }

        public string Path
        {
            get
            {
                string retVal = this.Name;
                ResourceFolder folder = this;
                while (folder.ParentFolder != null)
                {
                    retVal = folder.ParentFolder.Name + "\\" + retVal;
                    folder = folder.ParentFolder;
                }
                return retVal;
            }
        }

        public void Load(bool _recursive, bool _loadFiles)
        {
            this.subFolders.Clear();
            this.Files.Clear();
            if (String.IsNullOrEmpty(this.FolderSpec) || !System.IO.Directory.Exists(this.FolderSpec))
                return;
            string[] dirsIO = System.IO.Directory.GetDirectories(this.FolderSpec);
            List<string> dirs = new List<string>();
            foreach (string dir in dirsIO)
                dirs.Add(dir);
            dirs.Sort();
            foreach (string dir in dirs)
            {
                ResourceFolder f = new ResourceFolder(this, System.IO.Path.GetFileName(dir), this.ResourcesFolder);
                if (_recursive)
                    f.Load(_recursive, _loadFiles);
                this.subFolders.Add(f);
            }
            if (_loadFiles)
            {
                string[] filesIO = System.IO.Directory.GetFiles(this.FolderSpec);
                List<string> files = new List<string>();
                foreach (string file in filesIO)
                {
                    if(!utils.IsThumbSpec(file))
                        files.Add(file);
                }
                files.Sort();
                foreach (string file in files)
                {
                    ResourceFile f = new ResourceFile(this, System.IO.Path.GetFileName(file), this.ResourcesFolder);
                    this.Files.Add(f);
                }
            }
            this.Loaded = true;
        }

        public void Refresh(bool _recursive, bool _loadFiles)
        {
            this.subFolders.Clear();
            this.Files.Clear();
            this.Load(_recursive, _loadFiles);
        }

        public void Save(string _connectString, bool _noCheck)
        {
            if (this.isNew)
                Directory.CreateDirectory(this.FolderSpec);
            else
            {
                if (this.FolderSpec != this.oldPath)
                {
                    if (!_noCheck)
                        //Przepięcie linków do itemów w albumach (musi być przed fizycznym przeniesieniem folderu)
                        this.MoveLinkedItemsInFolder(this.oldPath, _connectString);

                    //np. zmieniliśmy folder d:\a\a1 na d:\a\aXYZ
                    Directory.Move(this.oldPath, this.FolderSpec); //Robimy to najpierw, bo inaczej nie uda się przepiąć skrótu poniżej

                    if (!_noCheck)
                    {
                        //Przepięcie skrótów wskazujących na pliki z tego folderu i z jego podfolderów
                        //(skróty mogą być w dowolnym miejscu w katalogu zasobów)
                        List<string> allShortcuts = this.FindShortcutsInFolder(this.ResourcesFolder); //ładujemy wszystkie skróty w folderze zasobów
                        string oldTargetFileSpec;
                        foreach (string shortcut in allShortcuts)
                        {
                            try
                            {
                                //Szukamy skrótów, które wskazują na plik docelowy w renamowanym folderze albo którymś z jego podfolderów
                                //np. link d:\b\link.lnk jest skrótem do d:\a\a1\a11\plik.jpg
                                oldTargetFileSpec = utils.ShortcutTargetFile(shortcut); //np. d:\a\a1\a11\plik.jpg
                                //Jeśli ścieżka pliku docelowego skrótu zawiera renamowany folder, znaczy to, że skrót wskazuje na plik
                                //w renamowanym folderze lub którymś z jego podfolderów. Taki skrót trzeba "przepiąć" na nowy folder.
                                if (oldTargetFileSpec.Length > this.oldPath.Length)
                                {
                                    if (oldTargetFileSpec.Substring(0, this.oldPath.Length + 1).ToUpper() == this.oldPath.ToUpper() + "\\")
                                    {
                                        //target zaczyna się od starej ścieżki zmienianego folderu, więc skrót wskazuje na plik w zmienianym folderze 
                                        //lub którymś z jego podfolderów
                                        string newTargetFileSpec = this.FolderSpec + oldTargetFileSpec.Substring(this.oldPath.Length);
                                        utils.ChangeShortcutTargetFile(shortcut, newTargetFileSpec);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Error replacing target file in shortcuts of files in changed folder and its subfolders. " + ex.Message);
                            }
                        }
                    }

                    this.oldPath = this.FolderSpec;
                }
            }
        }
        private void MoveLinkedItemsInFolder(string _folderSpec, string _connectString)
        {
            List<Item> retVal = new List<Item>();
            foreach (string subDir in System.IO.Directory.GetDirectories(_folderSpec))
                this.MoveLinkedItemsInFolder(subDir, _connectString);
            foreach (string file in System.IO.Directory.GetFiles(_folderSpec))
            {
                ResourceFolder resFolder = new ResourceFolder(_folderSpec, this.ResourcesFolder);
                ResourceFile resFile = new ResourceFile(resFolder, System.IO.Path.GetFileName(file), this.ResourcesFolder);
                //np. d:\Multimedia\aaa -> d:\Multimedia\b
                //d:\Multimedia\aaa\xxx\1.jpg -> d:\Multimedia\b\xxx\1.jpg
                string newFSpec = this.FolderSpec + file.Substring(this.oldPath.Length, file.Length - this.oldPath.Length);
                // "d:\Multimedia\b" + "\xxx\1.jpg"
                resFile.MoveItems(newFSpec, _connectString);
            }
        }

        private List<string> FindShortcutsInFolder(string _fldSpec)
        {
            List<string> retVal = new List<string>();
            string[] files = System.IO.Directory.GetFiles(_fldSpec, "*.LNK");
            foreach (string file in files)
                    retVal.Add(file);
            string[] subFolders = System.IO.Directory.GetDirectories(_fldSpec);
            foreach (string subFolder in subFolders)
            {
                List<string> shList = this.FindShortcutsInFolder(subFolder);
                foreach (string sh in shList)
                    retVal.Add(sh);
            }
            return retVal;
        }


        public bool Delete(bool _ask, string _connectString, bool _noCheck)
        {
            if (!Directory.Exists(this.FolderSpec))
            {
                MessageBox.Show("Directory does not exist");
                return false;
            }
            if (_ask)
            {
                for (int i = 0; i < 2; i++)
                {
                    string wrnTxt = "\r\nOPERATION WILL PERMANENTLY DELETE FOLDER [" + this.FolderSpec + "] AND ALL ITS SUBFOLDERS AND FILES!";
                    if (_noCheck)
                        wrnTxt += "\r\nProgram will not check if there exist any shortcuts or view items links to deleted files.";
                    else
                        wrnTxt += "\r\nAll shortcuts and view items links to deleted files will be deleted too.";
                    if (MessageBox.Show("ARE YOU SURE?" + wrnTxt,
                        "WARNING",
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                        return false;
                }
            }
            return this.DeleteDir(this.FolderSpec, _connectString, _noCheck); //Usuwamy pliki z podkatalogów aby usunąć także ewentualne skróty do plików oraz ich miniaturki
        }
        public bool DeleteDir(string _dir, string _connectString, bool _noCheck)
        {
            if (!Directory.Exists(_dir))
                return false;
            string[] files = System.IO.Directory.GetFiles(_dir);
            foreach (string file in files)
            {
                ResourceFolder rFolder = new ResourceFolder(_dir, this.ResourcesFolder);
                ResourceFile rFile = new ResourceFile(rFolder, System.IO.Path.GetFileName(file), this.ResourcesFolder);
                if (!rFile.Delete(false, _connectString, _noCheck)) //Usuwa nie tylko plik, ale także ewentualne linki, skróty do niego oraz jego miniaturkę
                    return false;
            }
            string[] subDirs = System.IO.Directory.GetDirectories(_dir);
            foreach (string subDir in subDirs)
            {
                if (!this.DeleteDir(subDir, _connectString, _noCheck))
                    return false;
            }            
            Directory.Delete(this.FolderSpec, true);
            return true;
        }
    }
}
