using System;
using System.Collections;
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
    #region Enums

    public enum CopyCutMode
    {
        None = 0,
        Copy = 1,
        Cut = 2
    }
    public enum CopyCutLastFrom
    {
        None = 0,
        Items = 1,
        Files = 2
    }

    #endregion

    public partial class MultimediaCenterForm : Form
    {
        #region Variables

        string resourcesFolder = "D:\\Multimedia";
        string arcFolder = String.Empty;
        string backupFolder = String.Empty;

        float bigFontSize = 12.0F;
        float normalFontSize = 8.0F;
        float smallFontSize = 7.0F;
        float importantFontSize = 7.75F;
        float shortcutFontSize = 7.0F;

        #region SQL

        private string connectStringHome = @"Data Source=.;Initial Catalog=MultiMedia;Integrated Security=True";
        private string connectString = String.Empty;

        #endregion

        #region Lists

        List<Album> albums = null;

        #endregion

        #region Currents

        private Album currentAlbum;
        private ViewLink currentViewLink;
        private Item currentItem;
        private ResourceFolder currentFolder;
        private ResourceFile currentFile;

        //private DateTime lastTimeItemSelected = DateTime.MinValue;
        private bool itemsRowChanging = false;
        private int itemsPerRow = 2;
        //private DateTime lastTimeFileSelected = DateTime.MinValue;
        private bool filesRowChanging = false;
        //private int filesPerRow = 8;
        //private int lastTimeSelectedMilisecondsDelay = 200;

        #endregion

        #region History

        private TreeNode[] albumsHistory = new TreeNode[8];
        private int albumsHistoryNdx = -1;
        private bool albumsHistoryGoingBackOrNext = false;
        private TreeNode[] foldersHistory = new TreeNode[8];
        private int foldersHistoryNdx = -1;
        private bool foldersHistoryGoingBackOrNext = false;
        private TreeNode folderWNode = null;
        private ResourceFile fileW = null;
        private bool afterGoToFileW = false;
        private TreeNode folderJNode = null;
        private ResourceFile fileJ = null;
        private bool afterGoToFileJ = false;

        #endregion

        #region Images and icons

        private const int iconNrAlbum = 0;
        private const int iconNrView = 1;
        private const int iconNrFolder = 0;

        private ImageList albumsTreeImageList;
        private ImageList itemsListImageList;
        private ImageList foldersTreeImageList;
        private ImageList filesListImageList;

        private int treeIconSize = 15;
        private int thumbnailSize = 35;
        private Image.GetThumbnailImageAbort myCallback = null;

        private DescriptionBubbleForm descriptionBubble = new DescriptionBubbleForm();

        #endregion

        #region Play

        private bool playViewMode = true;

        #endregion

        #region Full screen

        private double initialFullScreenZoomFactorCoeff = 0.75;
        private double initialFullScreenMoveDelta = 64.0;

        #endregion

        #region Clipboard

        private ViewLink clipboardViewLink;
        private CopyCutMode CopyCutViewMode = CopyCutMode.None;
        private List<Item> clipboardItems;
        private CopyCutMode CopyCutItemsMode = CopyCutMode.None;
        private List<ResourceFile> clipboardFiles;        
        private CopyCutMode CopyCutFilesMode = CopyCutMode.None;
        private CopyCutLastFrom CopyCutLastFrom = CopyCutLastFrom.None;

        #endregion

        #region Edit

        private string TextEditorFSpec1 = String.Empty;
        private string TextEditorFSpec2 = String.Empty;
        private string TextEditorsWorkingFolder = String.Empty;
        private string PhotoEditorFSpec1 = String.Empty;
        private string PhotoEditorFSpec2 = String.Empty;        
        private string PhotoEditorFSpec3 = String.Empty;
        private string PhotoEditorsWorkingFolder = String.Empty;
        private string AudioEditorFSpec1 = String.Empty;
        private string AudioEditorFSpec2 = String.Empty;
        private string AudioEditorsWorkingFolder = String.Empty;
        private string VideoEditorFSpec1 = String.Empty;
        private string VideoEditorFSpec2 = String.Empty;
        private string VideoEditorsWorkingFolder = String.Empty;

        #endregion

        private Utils utils = new Utils();

        private bool onLoad = true;

        private InsertMode insertMode = InsertMode.ByName;

        private ListViewItem lviGroupPoint = null;

        #endregion


        #region Constructors

        public MultimediaCenterForm()
        {
            InitializeComponent();
        }

        #endregion


        #region Load

        #region Window

        protected override void OnLoad(EventArgs e)
        {
            onLoad = true;

            base.OnLoad(e);

            ReadConfig();
            itemsListView.ListViewItemSorter = new ListViewItemComparer();
            LoadAll();
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            SetTooltips();
            ShowCurrentInsertMode();

            this.WindowState = FormWindowState.Maximized;

            onLoad = false;            
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            albumsTreeView.Select();
        }

        private void LoadAll()
        {
            albumsTreeImageList = new ImageList();
            albumsTreeImageList.Images.Add(MultiMediaCenter.Properties.Resources.Album);
            albumsTreeImageList.Images.Add(MultiMediaCenter.Properties.Resources.View);
            albumsTreeImageList.ImageSize = new Size(treeIconSize, treeIconSize);
            albumsTreeView.ImageList = albumsTreeImageList;

            itemsListImageList = new ImageList();
            itemsListImageList.ImageSize = new Size(thumbnailSize, thumbnailSize);
            itemsListView.LargeImageList = itemsListImageList;

            foldersTreeImageList = new ImageList();
            foldersTreeImageList.Images.Add(MultiMediaCenter.Properties.Resources.ResourceFolder);
            //foldersTreeImageList.Images.Add(MultiMediaCenter.Properties.Resources.ContentUnknownThumbnail);
            foldersTreeImageList.ImageSize = new Size(treeIconSize, treeIconSize);
            foldersTreeView.ImageList = foldersTreeImageList;

            filesListImageList = new ImageList();
            filesListImageList.ImageSize = new Size(thumbnailSize, thumbnailSize);
            filesListView.LargeImageList = filesListImageList;

            myCallback = new Image.GetThumbnailImageAbort(ThumbnailCallback);

            clipboardItems = new List<Item>();
            clipboardFiles = new List<ResourceFile>();

            SetCurrentViewLink(null);
            SetCurrentItem(null);
            SetCurrentFolder(null);
            SetCurrentFile(null);

            LoadAlbumsTreeView();
            LoadResourcesTreeView();

            this.DisplayNOfLinks();
        }

        private void SetTooltips()
        {
            this.SetTooltip(addAlbumButton, "Add album (Shift+F7, Shift+Insert)");
            this.SetTooltip(addViewButton, "Add view (F7, Insert)");
            this.SetTooltip(chgAlbumOrViewButton, "Change album or view (Ctrl+Enter, F2, F3, F4)");
            this.SetTooltip(delAlbumOrViewButton, "Delete album or view (Delete)");
            this.SetTooltip(copyViewButton, "Copy view");
            this.SetTooltip(cutViewButton, "Cut view");
            this.SetTooltip(pasteViewButton, "Paste view");
            this.SetTooltip(upAlbumOrViewButton, "Move up album or view (Ctrl+Up)");
            this.SetTooltip(downAlbumOrViewButton, "Move down album or view (Ctrl+Down)");
            this.SetTooltip(albumsHistoryBackButton, "Go to previous history view/album (Ctrl+Left)");
            this.SetTooltip(albumsHistoryNextButton, "Go to next history view/album (Ctrl+Right)");
            this.SetTooltip(impAlwaysCheck, "Always show important items (when not hidden)");
            this.SetTooltip(lowQualityCheck, "Show low quality items");
            this.SetTooltip(normalQualityCheck, "Show normal quality items");
            this.SetTooltip(goodQualityCheck, "Show good quality items");
            this.SetTooltip(bestQualityCheck, "Show best quality items");
            this.SetTooltip(extraQualityCheck, "Show eXtrA quality items");
            this.SetTooltip(artOnlyCheck, "Show only artistic items");
            this.SetTooltip(hideHiddenCheck, "Hide items marked as hidden");
            this.SetTooltip(copySettingsButton, "Copy settings (isArt, Quality, isHidden) to all items of this file(s) (S)");

            this.SetTooltip(delItemsButton, "Delete items (Delete)");
            this.SetTooltip(upItemButton, "Move up item (Ctrl+Up)");
            this.SetTooltip(downItemButton, "Move down item (Ctrl+Down)");
            this.SetTooltip(copyItemsButton, "Copy items (Ctrl+C)");
            this.SetTooltip(cutItemsButton, "Cut items (Ctrl+X)");
            this.SetTooltip(pasteItemsButton, "Paste items (Ctrl+V)");
            this.SetTooltip(playItemsButton, "Play items (Enter, MouseLeft2, F11)");
            this.SetTooltip(autoRefresh, "Auto refresh items list after moving items, adding items, etc.");
            this.SetTooltip(this.insertModeText, "insert items mode: ^N = ByName, ^F = OnFirst, ^S = OnSelected, ^L = OnLast");

            this.SetTooltip(addFolderButton, "Add folder (Insert, Ctrl+F7)");
            this.SetTooltip(renFolderButton, "Rename folder (Ctrl+Enter, F2, F4)");
            this.SetTooltip(delFolderButton, "Delete folder (Delete)");
            this.SetTooltip(foldersHistoryBackButton, "Go to previous history folder (Ctrl+Left)");
            this.SetTooltip(foldersHistoryNextButton, "Go to next history folder (Ctrl+Right)");
            this.SetTooltip(setFolderCButton, "Set current (working) folder (Alt+W)");
            this.SetTooltip(setFolderJButton, "Set jump (related) folder (Alt+J)");
            this.SetTooltip(gotoFolderCButton, "Go to current (working) folder (Alt+Left)");
            this.SetTooltip(gotoFolderJButton, "Go to jump (related) folder (Alt+Right)");
            this.SetTooltip(loadFullResourcesTreeCheck, "Load full files tree");

            this.SetTooltip(chgFileButton, "Change file (F2, F3, F4)");
            this.SetTooltip(crtSfxFileButton, "Create and edit suffixed file (Ctrl+Insert)");
            this.SetTooltip(sfxFileButton, "Add/Remove file prefix");
            this.SetTooltip(editFileContentButton, "Edit file content (Ctrl+F2)");
            this.SetTooltip(delFilesButton, "Delete files (Delete)");
            this.SetTooltip(delFilesNoCheckButton, "Delete files with no check (Shift+Delete)");
            this.SetTooltip(importShortcutButton, "Move real file here and delete its link");
            this.SetTooltip(linkFilesAddButton, "Link files (Backspace)");
            this.SetTooltip(copyFilesButton, "Copy files (Ctrl+C)");
            this.SetTooltip(cutFilesButton, "Cut files (Ctrl+X)");
            this.SetTooltip(pasteFilesButton, "Paste files (Ctrl+V)");
            this.SetTooltip(pasteShortcutsButton, "Paste shortcuts (Alt+V)");
            this.SetTooltip(pasteFilesWithLeaveShortcutsButton, "Move files here and create shortcuts in old location");
            this.SetTooltip(playFilesButton, "Play files (Enter, MouseLeft2, F11)");

            this.SetTooltip(textBox, "Double click to play");
            this.SetTooltip(saveTextButton, "Save text (F2)");
            this.SetTooltip(pictureBox, "Double click to play");
            this.SetTooltip(AVPlayerBox, "Double click to play");
        }
        private void SetTooltip(Control _c, string _tooltipText)
        {
            ToolTip tt = new ToolTip();
            tt.SetToolTip(_c, _tooltipText);
        }

        #region Konfiguracja

        private void ReadConfig()
        {
            string cnfFSpec = "MultimediaCenter.cnf";
            if (!System.IO.File.Exists(cnfFSpec))
            {
                System.Windows.Forms.MessageBox.Show("Brak pliku konfiguracji MultimediaCenter.cnf.");
                return;
            }
            string cnfText = System.IO.File.ReadAllText(cnfFSpec, Encoding.GetEncoding(1250));
            this.connectStringHome = this.ReadCnfParam(cnfText, "ConnectStringHome");
            this.connectString = this.connectStringHome;
            this.resourcesFolder = this.ReadCnfParam(cnfText, "ResourcesFolder");
            this.arcFolder = this.ReadCnfParam(cnfText, "ArcFolder");
            this.backupFolder = this.ReadCnfParam(cnfText, "BackupFolder");
            this.itemsPerRow = Convert.ToInt32(this.ReadCnfParam(cnfText, "itemsPerRow"));
            //this.filesPerRow = Convert.ToInt32(this.ReadCnfParam(cnfText, "filesPerRow"));
            filesPerRowSpin.Value = Convert.ToInt32(this.ReadCnfParam(cnfText, "filesPerRow"));
            this.TextEditorFSpec1 = this.ReadCnfParam(cnfText, "TextEditorFSpec1");
            this.TextEditorFSpec2 = this.ReadCnfParam(cnfText, "TextEditorFSpec2");
            this.PhotoEditorFSpec1 = this.ReadCnfParam(cnfText, "PhotoEditorFSpec1");
            this.PhotoEditorFSpec2 = this.ReadCnfParam(cnfText, "PhotoEditorFSpec2");
            this.PhotoEditorsWorkingFolder = this.ReadCnfParam(cnfText, "PhotoEditorsWorkingFolder");
            this.PhotoEditorFSpec3 = this.ReadCnfParam(cnfText, "PhotoEditorFSpec3");
            this.AudioEditorFSpec1 = this.ReadCnfParam(cnfText, "AudioEditorFSpec1");
            this.AudioEditorFSpec2 = this.ReadCnfParam(cnfText, "AudioEditorFSpec2");
            this.VideoEditorFSpec1 = this.ReadCnfParam(cnfText, "VideoEditorFSpec1");
            this.VideoEditorFSpec2 = this.ReadCnfParam(cnfText, "VideoEditorFSpec2");
            string valStr;
            this.treeIconSize = Convert.ToInt32(this.ReadCnfParam(cnfText, "initialTreeIconSize"));
            this.thumbnailSize = Convert.ToInt32(this.ReadCnfParam(cnfText, "initialThumbnailSize"));
            System.Globalization.CultureInfo cultinfo = new System.Globalization.CultureInfo("en-US");
            valStr = this.ReadCnfParam(cnfText, "initialFullScreenZoomFactorCoeff");
            this.initialFullScreenZoomFactorCoeff = Convert.ToDouble(valStr, cultinfo);
            valStr = this.ReadCnfParam(cnfText, "initialFullScreenMoveDelta");
            this.initialFullScreenMoveDelta = Convert.ToDouble(valStr, cultinfo);
            valStr = this.ReadCnfParam(cnfText, "lastTimeSelectedMilisecondsDelay");
            //this.lastTimeSelectedMilisecondsDelay = Convert.ToInt32(valStr);
            return;
        }
        private string ReadCnfParam(string _cnfText, string _parName)
        {
            string retVal = String.Empty;
            string parName = _parName + " = ";
            int p = _cnfText.IndexOf(parName);
            if (p >= 0)
            {
                int pEnter = _cnfText.IndexOf("\r\n", p + 1);
                if (pEnter >= 0)
                {
                    int l = parName.Length;
                    retVal = _cnfText.Substring(p + l, pEnter - (p + l));
                }
            }
            return retVal;
        }

        #endregion

        #endregion


        #region Albums tree and Items list

        private void LoadAlbumsTreeView()
        {
            albums = new List<Album>();
            albums.Clear();
            albumsTreeView.Nodes.Clear();            
            SqlUtils su = new SqlUtils(this.connectString);
            SqlDataReader rd = su.GetSqlReader("SELECT * FROM dbo.Albums ORDER BY A_Lp");
            while (rd.Read())
            {
                int id = Convert.ToInt32(rd["A_ID"]);
                Album a = new Album(id, this.connectString);
                a.LoadFromReader(rd, 2);
                albums.Add(a);
                if (currentAlbum == null)
                    currentAlbum = a;
            }
            rd.Close();
            su.Close();
            foreach (Album a in albums)
                PopulateAlbum(a);
        }
        private void PopulateAlbum(Album album)
        {
            TreeNode albumNode = new TreeNode(album.Name);
            albumNode.Tag = album;
            albumNode.ImageIndex = iconNrAlbum;
            albumsTreeView.Nodes.Add(albumNode);
            if (album.Loaded)
            {
                foreach (ViewLink viewLink in album.ViewsLinks)
                    this.PopulateViewLink(albumNode, viewLink, true);
            }
            else
            {
                TreeNode notLoadedNode = new TreeNode("(not loaded)");
                notLoadedNode.ImageIndex = foldersTreeImageList.Images.Count;
                albumNode.Nodes.Add(notLoadedNode);
            }
        }
        private void PopulateViewLink(TreeNode populatedNode, ViewLink populatedViewLink, bool _recursive)
        {
            if (hideHiddenCheck.Checked && populatedViewLink.View.IsHidden)
                return;
            string name = populatedViewLink.Name;
            if (populatedViewLink.View.IsHidden)
                name = "(H)" + name;
            TreeNode viewNode = new TreeNode(name);
            viewNode.Tag = populatedViewLink;
            viewNode.ImageIndex = iconNrView;
            populatedNode.Nodes.Add(viewNode);
            if (_recursive)
            {
                foreach (ViewLink subLink in populatedViewLink.SubLinks)
                    this.PopulateViewLink(viewNode, subLink, true);
            }
            else
            {
                TreeNode notLoadedNode = new TreeNode("(not loaded)");
                notLoadedNode.ImageIndex = foldersTreeImageList.Images.Count;
                viewNode.Nodes.Add(notLoadedNode);
            }
        }
        private void LoadCurrentViewItems()
        {
            SetCurrentItem(null);
            itemsListView.Clear();
            itemsListImageList.Images.Clear();
            /*
            clipboardItems.Clear();
            this.ItemsShowClipboardMode(CopyCutMode.None);
            */
            if (currentViewLink == null)
                return;
            if (!currentViewLink.View.Loaded)
                currentViewLink.View.Load(true);
            if (currentViewLink == null || currentViewLink.View.Items.Count == 0)
                return;
            int i = 0;
            foreach (Item item in currentViewLink.View.Items)
            {
                if (hideHiddenCheck.Checked && item.IsHidden)
                    continue;
                if((!impAlwaysCheck.Checked || impAlwaysCheck.Checked && !item.IsImportant) &&
                    ((!lowQualityCheck.Checked && item.Quality == ItemQuality.Low ||
                     !normalQualityCheck.Checked && item.Quality == ItemQuality.Normal ||
                     !goodQualityCheck.Checked && item.Quality == ItemQuality.Good ||
                     !bestQualityCheck.Checked && item.Quality == ItemQuality.Best ||
                     !extraQualityCheck.Checked && item.Quality == ItemQuality.Extra) ||
                    artOnlyCheck.Checked && !item.IsArt))
                    continue;
                this.AddImageToItemsImageList(item);
                this.AddItemToList(item, i, false, false);
                i++;
            }
            this.ShowCurrentItemLabel();
        }

        private void RefreshCurrentView()
        {
            TreeNode n0 = null;
            if(currentViewLink != null)
                n0 = FindNode(albumsTreeView.Nodes, currentViewLink);
            else
                n0 = FindNode(albumsTreeView.Nodes, currentAlbum);
            if (n0 == null)
                return;
            TreeNode n = albumsTreeView.SelectedNode;
            if (n.Tag == null)
            {
                //Odświeżamy węzeł (not loaded)
            }
            n0.Nodes.Clear();
            if (currentViewLink != null)
            {
                currentViewLink.Load(2, false);
                string name = currentViewLink.Name;
                if (currentViewLink.View.IsHidden)
                    name = "(H)" + name;
                if (currentViewLink.View.IsHidden)
                    name = "(A)" + name;
                n.Text = name;
                foreach (ViewLink viewLink in currentViewLink.SubLinks)
                {
                    viewLink.View.Load(true);
                    this.PopulateViewLink(n0, viewLink, true);
                }
            }
            else
            {
                currentAlbum.Load(2);
                n.Text = currentAlbum.Name;
                foreach (ViewLink viewLink in currentAlbum.ViewsLinks)
                {
                    viewLink.View.Load(true);
                    this.PopulateViewLink(n0, viewLink, true);
                }
            }
            this.RefreshItemsListView();
            albumsTreeView.SelectedNode = n0;
            albumsTreeView.Select();
        }
        private void DisplayNOfLinks()
        {
            SqlUtils su = new SqlUtils(this.connectString);
            nOfLinksLabel.Text = "(" + Convert.ToString(su.GetSqlScalar("SELECT COUNT(*) FROM dbo.Items")) + " links)";
            su.Close();
        }
        private void ReloadViewInAllAlbums(View _reloadView, bool _reloadInCurrentAlbum)
        {
            foreach (Album a in this.albums)
            {
                if (!_reloadInCurrentAlbum && a == currentAlbum)
                    continue;
                foreach (ViewLink vl in a.ViewsLinks)
                    this.ReloadView(_reloadView, vl);
            }
        }
        private void ReloadView(View _reloadView, ViewLink _inViewLink)
        {
            if (_inViewLink.View.ID == _reloadView.ID)
                _inViewLink.Load(2, true);
            foreach (ViewLink vl in _inViewLink.SubLinks)
                this.ReloadView(_reloadView, vl);
        }
        private void RefreshAlbumsTreeView(bool _fullRefresh)
        {
            object currentObject = null;
            if (albumsTreeView.SelectedNode != null)
                currentObject = albumsTreeView.SelectedNode.Tag;
            if (_fullRefresh)
                LoadAlbumsTreeView();
            else
            {
                TreeNodeCollection nodesColl = albumsTreeView.SelectedNode.Nodes;
                this.RefreshCurrentView();
            }
            TreeNode n = FindNode(albumsTreeView.Nodes, currentObject);
            if (n != null)
                albumsTreeView.SelectedNode = n;
        }
        private void RefreshItemsListView()
        {
            if (currentViewLink == null)
                return;
            int currentItemID = -1;
            if(currentItem != null)
                currentItemID = currentItem.ID;
            currentViewLink.View.LoadItems();
            this.LoadCurrentViewItems();
            itemsListView.SelectedItems.Clear();
            if (currentItemID == -1 && itemsListView.Items.Count > 0)
                currentItemID = (itemsListView.Items[0].Tag as Item).ID;
            int i = 0;
            foreach (ListViewItem lvi in itemsListView.Items)
            {
                Item item = lvi.Tag as Item;
                if (item.ID == currentItemID)
                {
                    lvi.Selected = true;
                    lvi.Focused = true;
                    SetCurrentItem(item);
                    itemsListView.EnsureVisible(i);
                    break;
                }
                i++;
            }
            this.DisplayNOfLinks();
        }

        #endregion


        #region Resources (Folders tree and Files list)

        private void LoadResourcesTreeView()
        {
            ResourceFolder rf = new ResourceFolder(resourcesFolder, this.resourcesFolder);
            rf.Load(this.loadFullResourcesTreeCheck.Checked, true);
            this.PopulateResourceFolder(rf);
            currentFolder = rf;
            if(foldersTreeView.Nodes.Count > 0)
                foldersTreeView.SelectedNode = foldersTreeView.Nodes[0];
        }
        private void PopulateResourceFolder(ResourceFolder _rf)
        {
            List<ResourceFolder> resourceFolders = new List<ResourceFolder>();
            resourceFolders.Clear();
            foldersTreeView.Nodes.Clear();
            PopulateFolder(null, _rf, this.loadFullResourcesTreeCheck.Checked);
        }
        private void PopulateFolder(TreeNode parentNode, ResourceFolder resourceFolder, bool _recursive)
        {
            TreeNode folderNode = new TreeNode(resourceFolder.Name);
            folderNode.Tag = resourceFolder;
            if (parentNode == null)
            {
                folderNode.Text = resourceFolder.Name;
                folderNode.ImageIndex = iconNrFolder;
                foldersTreeView.Nodes.Add(folderNode);
            }
            else
            {
                folderNode.Text = resourceFolder.Name;
                folderNode.ImageIndex = iconNrFolder;
                parentNode.Nodes.Add(folderNode);
            }
            if (!resourceFolder.Loaded)
                resourceFolder.Load(_recursive, true);
            if (_recursive)
            {
                foreach (ResourceFolder rf in resourceFolder.subFolders)
                    this.PopulateFolder(folderNode, rf, this.loadFullResourcesTreeCheck.Checked);
            }
            else
            {
                TreeNode notLoadedNode = new TreeNode("(not loaded)");
                notLoadedNode.ImageIndex = foldersTreeImageList.Images.Count;
                folderNode.Nodes.Add(notLoadedNode);
            }
        }

        private void RefreshResourcesCurrentFolder(bool _refreshThumbs)
        {
            if (currentFolder == null)
                return;
            TreeNode n0 = FindNode(foldersTreeView.Nodes, currentFolder);
            if (n0 == null)
                return;
            TreeNode n = foldersTreeView.SelectedNode;
            if (n.Tag == null)
            {
                //Odświeżamy węzeł (not loaded)
            }
            n0.Nodes.Clear();
            currentFolder.Load(false, false);
            foreach (ResourceFolder rf in currentFolder.subFolders)
                this.PopulateFolder(n0, rf, false);
            if (_refreshThumbs)
            {
                string[] files = System.IO.Directory.GetFiles(utils.GetThumbSpec(currentFolder.FolderSpec));
                foreach(string file in files)
                {
                    if (utils.ComputeContentType(file) == ContentType.Picture)
                    {
                        if (System.IO.File.Exists(file))
                            System.IO.File.Delete(file);
                    }
                }
            }                
            this.RefreshFilesListView(_refreshThumbs);
            foldersTreeView.SelectedNode = n0;
            foldersTreeView.Select();
        }

        private void RefreshResourcesTreeView()
        {
            object currentObject = null;
            if (foldersTreeView.SelectedNode != null)
                currentObject = foldersTreeView.SelectedNode.Tag;
            LoadResourcesTreeView();
            TreeNode n = FindNode(foldersTreeView.Nodes, currentObject);
            if (n != null)
                foldersTreeView.SelectedNode = n;
            foldersTreeView.Select();
            //albumsTreeView.ExpandAll();
        }
        private void RefreshFilesListView(bool _refreshThumbs)
        {
            ResourceFile currentObject = currentFile;
            currentFolder.Refresh(this.loadFullResourcesTreeCheck.Checked, true);
            resourcesTreeView_AfterSelect(null, null);
            if (currentObject != null)
            {
                int i = 0;
                foreach (ListViewItem lvi in filesListView.Items)
                {
                    ResourceFile file = (ResourceFile)lvi.Tag;
                    if (file.FileSpec == currentObject.FileSpec)
                    {
                        lvi.Selected = true;
                        lvi.Focused = true;
                        filesListView.EnsureVisible(i);
                        if (_refreshThumbs)
                            file.Thumbnail = utils.SaveThumb(file.RealFileSpec);
                        break;
                    }
                    i++;
                }
            }
            filesListView.Select();
            this.DisplayNOfLinks();
        }

        #endregion

        #endregion


        #region Events

        #region Window

        private void MultimediaCenterForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F11)
            {
                if(this.playViewMode)
                    playItemsButton.PerformClick();
                else
                    playFilesButton.PerformClick();
            }
            else if ((e.KeyCode == Keys.N || e.KeyCode == Keys.F || e.KeyCode == Keys.S || e.KeyCode == Keys.L) && e.Modifiers == Keys.Control)
            {
                if (e.KeyCode == Keys.N)
                    insertMode = InsertMode.ByName;
                else if (e.KeyCode == Keys.F)
                    insertMode = InsertMode.OnFirst;
                else if (e.KeyCode == Keys.S)
                    insertMode = InsertMode.OnSelected;
                else if (e.KeyCode == Keys.L)
                    insertMode = InsertMode.OnLast;
                this.ShowCurrentInsertMode();
            }
            else if ((e.KeyCode == Keys.R) && e.Modifiers != Keys.Control && e.Modifiers != Keys.Alt)
            {
                this.CopyItems2Roboczy();
            }
        }

        private void displayPictureThumbnailsCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (displayPictureThumbnailsCheck.Checked)
            {
                this.RefreshItemsListView();
                this.RefreshFilesListView(false);
            }
        }
        private void thumbnailSizeSpin_Validated(object sender, EventArgs e)
        {
            thumbnailSize = Convert.ToInt32(thumbnailSizeSpin.Value);
            if (thumbnailSize > 0)
            {
                itemsListImageList.ImageSize = new Size(thumbnailSize, thumbnailSize);
                filesListImageList.ImageSize = new Size(thumbnailSize, thumbnailSize);
            }
            this.RefreshLists();
        }
        private void playAVCheck_CheckedChanged(object sender, EventArgs e)
        {
            this.Play(playViewMode);
        }
        private void hideLinkedCheck_CheckedChanged(object sender, EventArgs e)
        {
            this.RefreshFilesListView(false);
        }

        private void RefreshLists()
        {
            this.RefreshFilesListView(false); //Musi być przed RefreshItemsListView, bo tu mogą się generować thumbsy
            this.RefreshItemsListView();
        }

        private void loadFullAlbumsTreeCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (onLoad)
                return;
            this.RefreshAlbumsTreeView(true);
        }

        private void impAlwaysCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (onLoad)
                return;
            this.RefreshItemsListView();
        }
        private void lowQualityCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (onLoad)
                return;
            this.RefreshItemsListView();
        }
        private void normalQualityCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (onLoad)
                return;
            this.RefreshItemsListView();
        }
        private void goodQualityCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (onLoad)
                return;
            this.RefreshItemsListView();
        }
        private void bestQualityCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (onLoad)
                return;
            this.RefreshItemsListView();
        }
        private void extraQualityCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (onLoad)
                return;
            this.RefreshItemsListView();
        }
        private void artOnlyCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (onLoad)
                return;
            this.RefreshItemsListView();
        }
        private void hideHiddenCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (onLoad)
                return;
            this.RefreshAlbumsTreeView(true);
        }

        private void loadFullResourcesCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (onLoad)
                return;
            this.RefreshResourcesTreeView();
        }

        private void noCheckDeleteFDCheck_CheckedChanged(object sender, EventArgs e)
        {
            if(noCheckDeleteFDCheck.Checked)
                System.Windows.Forms.MessageBox.Show("From now on, when deleting directory or moving file or directory, program will not check if there exist any shortcuts or view items links of deleted/moved files!", "WARNING!");
        }

        #endregion


        #region Albums tree

        #region Select

        private void albumsTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (albumsTreeView.SelectedNode == null)
                return;
            object tag = albumsTreeView.SelectedNode.Tag;
            if (tag == null)
            {
                tag = albumsTreeView.SelectedNode.Parent.Tag;
                if (tag == null)
                    return;
            }
            Album album = null;
            ViewLink viewLink = null;
            if (tag is Album)
                album = (Album)tag;
            else if (tag is ViewLink)
            {
                viewLink = (ViewLink)tag;
                album = viewLink.ParentAlbum;
            }
            currentAlbum = album;
            currentAlbumLabel.Text = "(" + albumsTreeView.Nodes.Count + ") " + currentAlbum.Name;
            bool viewChanged = (viewLink != currentViewLink);
            SetCurrentViewLink(viewLink);
            if (viewChanged)
            {
                this.LoadCurrentViewItems();
                this.Play(true);
            }
        }
        private void albumsTreeView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Insert && e.Modifiers == Keys.Shift || e.KeyCode == Keys.F7 && e.Modifiers == Keys.Shift)
                addAlbumButton.PerformClick();
            if (e.KeyCode == Keys.Insert && e.Modifiers != Keys.Shift || e.KeyCode == Keys.F7 && e.Modifiers != Keys.Shift)
            {
                if (albumsTreeView.SelectedNode != null)
                    addViewButton.PerformClick();
            }
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Enter || e.Modifiers != Keys.Control && (e.KeyCode == Keys.F2 || e.KeyCode == Keys.F3 || e.KeyCode == Keys.F4))
                chgAlbumOrViewButton.PerformClick();
            else if (e.KeyCode == Keys.Delete)
                delAlbumOrViewButton.PerformClick();
            else if (e.KeyCode == Keys.Up && e.Modifiers == Keys.Control)
                upAlbumOrViewButton.PerformClick();
            else if (e.KeyCode == Keys.Down && e.Modifiers == Keys.Control)
                downAlbumOrViewButton.PerformClick();
            else if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
                pasteItemsButton.PerformClick();
            else if (e.KeyCode == Keys.Enter && e.Modifiers != Keys.Control)
                playItemsButton.PerformClick();
            else if (e.KeyCode == Keys.Left && e.Modifiers == Keys.Control)
                albumsHistoryBackButton.PerformClick();
            else if (e.KeyCode == Keys.Right && e.Modifiers == Keys.Control)
                albumsHistoryNextButton.PerformClick();
            else if (e.KeyCode == Keys.F5 && e.Modifiers != Keys.Control)
                this.RefreshAlbumsTreeView(false);
            else if (e.KeyCode == Keys.F5 && e.Modifiers == Keys.Control)
                this.RefreshAlbumsTreeView(true);
        }
        private void albumsTreeView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            chgAlbumOrViewButton.PerformClick();
        }

        private void albumsTreeViewBack_Click(object sender, EventArgs e)
        {
            if (albumsHistoryNdx > 0 && albumsHistory[albumsHistoryNdx - 1] != null)
            {
                albumsHistoryGoingBackOrNext = true;
                albumsTreeView.SelectedNode = albumsHistory[--albumsHistoryNdx];
            }
            albumsTreeView.Select();
        }
        private void albumsTreeViewNext_Click(object sender, EventArgs e)
        {
            if (albumsHistoryNdx < albumsHistory.Length - 1 && albumsHistory[albumsHistoryNdx + 1] != null)
            {
                albumsHistoryGoingBackOrNext = true;
                albumsTreeView.SelectedNode = albumsHistory[++albumsHistoryNdx];
            }
            albumsTreeView.Select();
        }

        private void SetCurrentViewLink(ViewLink _viewLink)
        {
            currentViewLink = _viewLink;
            if (currentViewLink != null)
                currentViewLabel.Text = currentViewLink.ViewPath;
            else
                currentViewLabel.Text = String.Empty;
            if (albumsHistoryGoingBackOrNext)
                albumsHistoryGoingBackOrNext = false;
            else
            {
                if (albumsTreeView.SelectedNode != null && albumsTreeView.SelectedNode.Tag != null)
                {
                    if(albumsHistoryNdx == -1 || albumsTreeView.SelectedNode != albumsHistory[albumsHistoryNdx])
                    {
                        if (albumsHistoryNdx < albumsHistory.Length - 1)
                            albumsHistory[++albumsHistoryNdx] = albumsTreeView.SelectedNode;
                        else
                        {
                            for (int i = 0 + 1; i < albumsHistory.Length-1; i++)
                                albumsHistory[i] = albumsHistory[i+1];
                            albumsHistory[albumsHistoryNdx] = albumsTreeView.SelectedNode;
                        }
                    }
                }
            }
        }

        #endregion

        #region Edit

        private void addAlbumButton_Click(object sender, EventArgs e)
        {
            Album album = new Album(0, this.connectString);
            AlbumForm albumForm = new AlbumForm();
            albumForm.album = album;
            albumForm.ShowDialog();
            if (albumForm.OK)
            {
                this.PopulateAlbum(album);
                TreeNode node = this.FindNode(albumsTreeView.Nodes, album);
                albumsTreeView.SelectedNode = node;
            }
            albumsTreeView.Select();
        }
        private void addViewButton_Click(object sender, EventArgs e)
        {
            View view = null;
            ViewLink viewLink = null;
            if (albumsTreeView.SelectedNode == null)
                return;
            object parentTag = albumsTreeView.SelectedNode.Tag;
            if (parentTag == null)
                return;
            view = new View(0, this.connectString);
            ViewForm viewForm = new ViewForm();
            viewForm.view = view;
            viewForm.viewLink = null;
            viewForm.ShowDialog();
            if (viewForm.OK)
            {
                if (parentTag is Album)
                    viewLink = new ViewLink((Album)parentTag, view, this.connectString);
                else if (parentTag is ViewLink)
                    viewLink = new ViewLink((ViewLink)parentTag, view, this.connectString);
                viewLink.Lp = viewForm.Lp;
                viewLink.Save(false);
                TreeNode parentNode = null;
                if (parentTag is Album)
                    parentNode = this.FindNode(albumsTreeView.Nodes, parentTag as Album);
                else if (parentTag is ViewLink)
                    parentNode = this.FindNode(albumsTreeView.Nodes, parentTag as ViewLink);
                this.PopulateViewLink(parentNode, viewLink, true);
                TreeNode node = this.FindNode(albumsTreeView.Nodes, viewLink);
                albumsTreeView.SelectedNode = node;
            }
            albumsTreeView.Select();
        }
        private void chgAlbumOrViewButton_Click(object sender, EventArgs e)
        {
            ChgOrViewAlbumOrViewButton(true);
        }
        private void ChgOrViewAlbumOrViewButton(bool _edit)
        {
            if (albumsTreeView.SelectedNode == null)
                return;
            object tag = albumsTreeView.SelectedNode.Tag;
            if (tag == null)
                return;
            if (tag is Album)
            {
                AlbumForm albumForm = new AlbumForm();
                Album album = (Album)tag;
                albumForm.album = album;
                albumForm.ViewOnly = !_edit;
                albumForm.ShowDialog();
                if (albumForm.OK)
                {
                    TreeNode node = this.FindNode(albumsTreeView.Nodes, album);
                    node.Text = album.Name;
                }
            }
            else if (tag is ViewLink)
            {
                ViewForm viewForm = new ViewForm();
                ViewLink viewLink = (ViewLink)tag;
                viewForm.view = viewLink.View;
                viewForm.viewLink = viewLink;
                viewForm.ViewOnly = !_edit;
                viewForm.ShowDialog();
                if (viewForm.OK)
                {
                    TreeNode node = this.FindNode(albumsTreeView.Nodes, viewLink);
                    node.Text = viewLink.Name;
                    if (viewForm.renumberedAfter)
                        AfterRenumberAfterView(node, viewLink);
                }
            }
            albumsTreeView.Select();
        }
        private void AfterRenumberAfterView(TreeNode _node, ViewLink _viewLink)
        {
            TreeNode parentNode = this.FindNode(albumsTreeView.Nodes, _viewLink.ParentViewLink);
            albumsTreeView.SelectedNode = parentNode;
            this.RefreshCurrentView();
            foreach (TreeNode n in parentNode.Nodes)
            {
                if ((n.Tag as ViewLink).ID == (_node.Tag as ViewLink).ID)
                {
                    albumsTreeView.SelectedNode = n;
                    break;
                }
            }
        }
        private void delAlbumOrViewButton_Click(object sender, EventArgs e)
        {
            if (albumsTreeView.SelectedNode.Tag is Album)
            {
                Album album = (albumsTreeView.SelectedNode.Tag as Album);
                if (album.Delete())
                {
                    albumsTreeView.Nodes.Remove(albumsTreeView.SelectedNode);
                    this.ClearKeyboardBuffer();
                }
            }
            else if (albumsTreeView.SelectedNode.Tag is ViewLink)
            {
                ViewLink viewLink = (albumsTreeView.SelectedNode.Tag as ViewLink);
                if (viewLink.Delete(true))
                {
                    this.ClearKeyboardBuffer();
                    albumsTreeView.Nodes.Remove(albumsTreeView.SelectedNode);
                }
            }
            albumsTreeView.Select();
        }

        #endregion

        #region Up/Down

        private void upAlbumOrViewButton_Click(object sender, EventArgs e)
        {
            this.UpDownCurrentAlbumOrView(-1);
            albumsTreeView.Select();
        }
        private void downAlbumOrViewButton_Click(object sender, EventArgs e)
        {
            this.UpDownCurrentAlbumOrView(1);
            albumsTreeView.Select();
        }
        private void UpDownCurrentAlbumOrView(int _direction)
        {
            if (albumsTreeView.SelectedNode == null)
                return;
            object tag = albumsTreeView.SelectedNode.Tag;
            if (tag == null)
                return;
            if (tag is Album)
                this.UpDownCurrentAlbum(_direction);
            else if (tag is ViewLink)
                this.UpDownCurrentView(_direction);
        }
        private void UpDownCurrentAlbum(int _direction)
        {
            bool ok = (_direction < 0 ? currentAlbum.Up() : currentAlbum.Down());
            if (!ok)
                return;
            TreeNode currentNode = this.FindNode(albumsTreeView.Nodes, currentAlbum);
            this.UpDownCurrentAlbumOrViewNode(albumsTreeView.Nodes, currentNode, _direction);
        }
        private void UpDownCurrentView(int _direction)
        {
            bool ok = (_direction < 0 ? currentViewLink.Up() : currentViewLink.Down());
            if (!ok)
                return;
            TreeNode currentNode = this.FindNode(albumsTreeView.Nodes, currentViewLink);
            this.UpDownCurrentAlbumOrViewNode(currentNode.Parent.Nodes, currentNode, _direction);
        }
        private void UpDownCurrentAlbumOrViewNode(TreeNodeCollection _parentNodes, TreeNode _currentNode, int _direction)
        {
            int currentNodeNdx = -1;
            TreeNode previousNode = null;
            int previousNodeNdx = -1;
            TreeNode nextNode = null;
            int nextNodeNdx = -1;
            bool currentNodeFound = false;
            TreeNode[] nodes = new TreeNode[_parentNodes.Count];
            int i = 0;
            foreach (TreeNode node in _parentNodes)
            {
                nodes[i] = node;
                if (node == _currentNode)
                {
                    currentNodeFound = true;
                    currentNodeNdx = i;
                }
                else if (!currentNodeFound)
                {
                    previousNode = node;
                    previousNodeNdx = i;
                }
                else if (currentNodeFound && nextNodeNdx == -1)
                {
                    nextNode = node;
                    nextNodeNdx = i;
                }
                i++;
            }
            if (_direction == -1 && previousNodeNdx == -1 || _direction == 1 && nextNodeNdx == -1)
                return;
            _parentNodes.Clear();
            if (_direction == -1)
            {
                for (int j = 0; j < nodes.Length; j++)
                {
                    TreeNode node = nodes[j];
                    if (j < previousNodeNdx)
                        _parentNodes.Add(node);
                    else if (j == previousNodeNdx)
                        _parentNodes.Add(_currentNode);
                    else if (j == currentNodeNdx)
                    {
                        (previousNode.Tag as ViewLink).Load(0, false);
                        _parentNodes.Add(previousNode);
                    }
                    else
                        _parentNodes.Add(node);
                }
            }
            else if (_direction == 1)
            {
                for (int j = 0; j < nodes.Length; j++)
                {
                    TreeNode node = nodes[j];
                    if (j < currentNodeNdx)
                        _parentNodes.Add(node);
                    else if (j == currentNodeNdx)
                    {
                        (nextNode.Tag as ViewLink).Load(0, false);
                        _parentNodes.Add(nextNode);
                    }
                    else if (j == nextNodeNdx)
                        _parentNodes.Add(_currentNode);
                    else
                        _parentNodes.Add(node);
                }
            }
            albumsTreeView.SelectedNode = _currentNode;
            /*
             * Tak nie zadziała - nie wiedzieć dlaczego zduplikują się węzły
            if (_direction == -1)
            {
                parentNode.Nodes[previousNodeNdx] = currentNode;
                parentNode.Nodes[currentNodeNdx] = previousNode;
            }
            else if (_direction == 1)
            {
                parentNode.Nodes[nextNodeNdx] = currentNode;
                parentNode.Nodes[currentNodeNdx] = nextNode;
            }
            */
            albumsTreeView.Refresh();
        }

        #endregion

        #endregion


        #region Items list

        #region Select

        private void itemsListView_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
                itemsRowChanging = this.IsFirstItemInRow();
            else if (e.KeyCode == Keys.Right)
                itemsRowChanging = this.IsLastItemInRow();
        }
        private bool IsFirstItemInRow()
        {
            return (this.CurrentItemNdx() % itemsPerRow) == 0;
        }
        private bool IsLastItemInRow()
        {
            return (this.CurrentItemNdx() % itemsPerRow) == (itemsPerRow - 1);
        }
        private int CurrentItemNdx()
        {
            int retVal = -1;
            int i = 0;
            foreach (ListViewItem lvi in itemsListView.Items)
            {
                if ((lvi.Tag as Item) == currentItem)
                {
                    retVal = i;
                    break;
                }
                i++;
            }
            return retVal;
        }

        private void itemsListView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A && e.Modifiers == Keys.Control)
                this.SelectAllItems();
            else if ((e.KeyCode == Keys.I || e.KeyValue == 49) && e.Modifiers != Keys.Control && e.Modifiers != Keys.Alt)
                this.SwitchItemIsImportant();
            else if (e.KeyCode == Keys.L && e.Modifiers != Keys.Control && e.Modifiers != Keys.Alt)
                this.SetItemsIsLow();
            else if (e.KeyCode == Keys.N && e.Modifiers != Keys.Control && e.Modifiers != Keys.Alt)
                this.SetItemsIsNormal();
            else if (e.KeyCode == Keys.G && e.Modifiers != Keys.Control && e.Modifiers != Keys.Alt)
                this.SetItemsIsGood();
            else if (e.KeyCode == Keys.B && e.Modifiers != Keys.Control && e.Modifiers != Keys.Alt)
                this.SetItemsIsBest();
            else if (e.KeyCode == Keys.E && e.Modifiers != Keys.Control && e.Modifiers != Keys.Alt)
                this.SetItemsIsExtra();
            else if (e.KeyCode == Keys.A && e.Modifiers != Keys.Control && e.Modifiers != Keys.Alt)
                this.SwitchItemIsArt();
            else if (e.KeyCode == Keys.H && e.Modifiers != Keys.Control && e.Modifiers != Keys.Alt)
                this.SwitchItemsIsHidden();
            else if (e.KeyCode == Keys.S && e.Modifiers != Keys.Control && e.Modifiers != Keys.Alt)
                this.copySettingsButton.PerformClick();
            else if ((e.Modifiers == Keys.Control && e.KeyCode == Keys.Enter) || e.KeyCode == Keys.F3)
                this.changeItem(true);
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.F2)
                this.RunEditorOnItem(1);
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.F3)
                this.RunEditorOnItem(2);
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.F4)
                this.RunEditorOnItem(3);
            else if (e.KeyCode == Keys.Delete)
                delItemsButton.PerformClick();
            else if (e.KeyCode == Keys.Up && e.Modifiers == Keys.Control)
                upItemButton.PerformClick();
            else if (e.KeyCode == Keys.Down && e.Modifiers == Keys.Control)
                downItemButton.PerformClick();
            else if (e.KeyCode == Keys.C && e.Modifiers == Keys.Control)
                copyItemsButton.PerformClick();
            else if (e.KeyCode == Keys.X && e.Modifiers == Keys.Control)
                cutItemsButton.PerformClick();
            else if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
                pasteItemsButton.PerformClick();
            else if (e.KeyCode == Keys.Home)
                this.GoToFirstItem();
            else if (e.KeyCode == Keys.Left /*&& e.Modifiers == Keys.Alt*/)
            {
                //TimeSpan dt = DateTime.Now.Subtract(lastTimeItemSelected);
                //if (dt.Hours > 0 || dt.Minutes > 0 || dt.Seconds > 0 || dt.Milliseconds > lastTimeSelectedMilisecondsDelay)
                //    this.SetPrevItem();
                if (itemsRowChanging)
                {
                    this.GoToPrevItem();
                    itemsRowChanging = false;
                }
            }
            else if (e.KeyCode == Keys.Right /*&& e.Modifiers == Keys.Alt*/)
            {
                //TimeSpan dt = DateTime.Now.Subtract(lastTimeItemSelected);
                //if (dt.Hours > 0 || dt.Minutes > 0 || dt.Seconds > 0 || dt.Milliseconds > lastTimeSelectedMilisecondsDelay)
                //this.SetNextItem();
                if (itemsRowChanging)
                {
                    this.GoToNextItem();
                    itemsRowChanging = false;
                }
            }
            else if (e.KeyCode == Keys.End)
                this.GoToLastItem();
            else if (e.KeyCode == Keys.Enter && e.Modifiers != Keys.Control)
                playItemsButton.PerformClick();
            else if (e.KeyCode == Keys.F5)
                this.RefreshItemsListView();
        }
        private void itemsListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            playItemsButton.PerformClick();
        }

        private void itemsListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e != null && e.Item != null && e.Item.Selected)
                SetCurrentItem(e.Item.Tag as Item);
            //if (e != null && e.Item.Selected)
            this.ItemsListSelected();
            //lastTimeItemSelected = DateTime.Now;
        }

        private void SwitchItemIsImportant()
        {
            foreach (ListViewItem lvi in itemsListView.SelectedItems)
            {
                Item item = lvi.Tag as Item;
                item.IsImportant = !item.IsImportant;
                item.Save();
                //this.SetListItemText(lvi);
                this.SetListItem(lvi);
            }
            //this.RefreshItemsListView();
        }

        private void SetItemsIsLow()
        {
            this.SetItemsQuality(ItemQuality.Low);
        }
        private void SetItemsIsNormal()
        {
            this.SetItemsQuality(ItemQuality.Normal);
        }
        private void SetItemsIsGood()
        {
            this.SetItemsQuality(ItemQuality.Good);
        }
        private void SetItemsIsBest()
        {
            this.SetItemsQuality(ItemQuality.Best);
        }
        private void SetItemsIsExtra()
        {
            this.SetItemsQuality(ItemQuality.Extra);
        }
        private void SetItemsQuality(ItemQuality _quality)
        {
            foreach (ListViewItem lvi in itemsListView.SelectedItems)
            {
                Item item = lvi.Tag as Item;
                item.Quality = _quality;
                item.Save();
                //this.SetListItemText(lvi);
                this.SetListItem(lvi);
            }
            //this.RefreshItemsListView();
        }
        private void SwitchItemIsArt()
        {
            foreach (ListViewItem lvi in itemsListView.SelectedItems)
            {
                Item item = lvi.Tag as Item;
                item.IsArt = !item.IsArt;
                item.Save();
                //this.SetListItemText(lvi);
                this.SetListItem(lvi);
            }
            //this.RefreshItemsListView();
        }
        private void SwitchItemsIsHidden()
        {
            foreach (ListViewItem lvi in itemsListView.SelectedItems)
            {
                Item item = lvi.Tag as Item;
                item.IsHidden = !item.IsHidden;
                item.Save();
                this.SetListItem(lvi);
            }
        }

        private void copySettingsButton_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in itemsListView.SelectedItems)
            {
                Item item = lvi.Tag as Item;
                List<Item> Items = null;
                Items = utils.GetFileSpecItems(item.FileSpec, this.connectString, item);
                foreach (Item it in Items)
                {
                    if (it.ID == item.ID)
                        continue;
                    it.IsArt = item.IsArt;
                    it.Quality = item.Quality;
                    it.IsHidden = item.IsHidden;
                    it.Save(0);
                }
            }
        }

        private void setGroupItemsPointButton_Click(object sender, EventArgs e)
        {
            lviGroupPoint = null;
            if(itemsListView.SelectedItems.Count > 0)
                lviGroupPoint = itemsListView.SelectedItems[0];
        }

        private void groupItemsButton_Click(object sender, EventArgs e)
        {
            if (currentViewLink == null || itemsListView.SelectedItems.Count < 2)
                return;
            int firstLp = -1;
            if (lviGroupPoint != null && itemsListView.SelectedItems.Contains(lviGroupPoint))
                firstLp = (lviGroupPoint.Tag as Item).Lp;
            else
                firstLp = (itemsListView.SelectedItems[0].Tag as Item).Lp;
            lviGroupPoint = null;
            foreach (ListViewItem lvi in itemsListView.SelectedItems)
            {
                Item item = lvi.Tag as Item;
                item.Lp = firstLp;
                item.Save(firstLp);
            }
            if (currentViewLink.View.RenumItems("LP"))
                this.RefreshItemsListView();
        }

        private void SetCurrentItem(Item _item)
        {
            currentItem = _item;
            this.Play(true);
            this.ShowCurrentItemLabel();
        }
        private void ShowCurrentItemLabel()
        {
            if (currentViewLink != null)
            {
                currentItemLabel.Text = "(" + itemsListView.Items.Count + ")";
                if (currentItem != null)
                    currentItemLabel.Text += (" " + currentItem.FileName);
            }
            else
                currentItemLabel.Text = String.Empty;
            currentItemLabel.Show();
        }

        private void SelectAllItems()
        {
            foreach (ListViewItem lvi in itemsListView.Items)
                lvi.Selected = true;
        }

        #endregion

        #region Edit

/*
        private void addItems_Click(object sender, EventArgs e)
        {
            if (currentViewLink == null)
                return;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = resourcesFolder;
            ofd.Multiselect = true;
            if (ofd.ShowDialog() != DialogResult.OK)
                return;
            foreach (string fspec in ofd.FileNames)
                this.LinkFileToCurrentView(fspec);
            this.RefreshItemsListView();
            itemsListView.Select();
        }
*/
        private void chgItem_Click(object sender, EventArgs e)
        {
            this.changeItem(true);
        }
        private void changeItem(bool _editMode)
        {
            if (currentItem == null)
                return;
            ItemForm form = new ItemForm();
            form.Item = currentItem;
            form.ResourcesFolder = this.resourcesFolder;
            form.Albums = albums;
            form.ConnectString = this.connectString;
            form.ViewOnly = !_editMode;
            form.ShowDialog();
            if(form.OK)
            {
                RefreshFilesListView(false);
                RefreshItemsListView();
                this.ReloadViewInAllAlbums(currentViewLink.View, false);
                itemsListView.Select();
            }
            this.DisplayNOfLinks();
        }
        private void RunEditorOnItem(int _editorNo)
        {
            if (currentItem == null)
                return;
            this.RunEditor(_editorNo, null, currentItem.FileSpec, false);
        }

        private void delItems_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Are you sure? Operation will delete selected items.", "Warning",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            this.ClearKeyboardBuffer();
            if (dr != DialogResult.OK)
            {
                itemsListView.Select();
                return;
            }
            List<string> filesToRefresh = new List<string>();
            foreach (ListViewItem lvi in itemsListView.SelectedItems)
            {
                Item item = lvi.Tag as Item;
                if (item.Delete(false))
                    itemsListView.Items.Remove(lvi);
                if (System.IO.Path.GetDirectoryName(item.FileSpec) == currentFolder.FolderSpec)
                    filesToRefresh.Add(item.FileSpec);
            }
            if (itemsListView.Items.Count > 0)
            {
                if (itemsListView.FocusedItem != null)
                    SetCurrentItem(itemsListView.FocusedItem.Tag as Item);
                else
                    SetCurrentItem(itemsListView.Items[0].Tag as Item);
            }
            else
                SetCurrentItem(null);
            itemsListView.Select();
            foreach (string fSpec in filesToRefresh)
            {
                foreach (ListViewItem lvi in filesListView.Items)
                {
                    if ((lvi.Tag as ResourceFile).RealFileSpec == fSpec)
                    {
                        ResourceFile rf = lvi.Tag as ResourceFile;
                        lvi.Text = this.FileDisplayName(lvi.Tag as ResourceFile);
                        this.SetStyleOfFileInList(lvi);
                    }
                }
            }
            this.ReloadViewInAllAlbums(currentViewLink.View, false);
            this.DisplayNOfLinks();
        }

        #endregion

        #region Up/Down

        private void upItemButton_Click(object sender, EventArgs e)
        {
            this.UpDownItem(-1);
            itemsListView.Select();
        }
        private void downItemButton_Click(object sender, EventArgs e)
        {
            this.UpDownItem(1);
            itemsListView.Select();
        }
        private void UpDownItem(int _direction)
        {
            if (itemsListView.SelectedItems.Count == 0)
                return;
            bool atLeastOneOK = false;
            if (itemsListView.SelectedItems.Count == 1)
            {
                Item item = (itemsListView.SelectedItems[0].Tag) as Item;
                atLeastOneOK = (_direction < 0 ? item.Up() : item.Down());
                if (atLeastOneOK)
                {
                    int ndxCurr = this.FindListViewItemNdx(itemsListView, currentItem);
                    if (ndxCurr >= 0)
                    {
                        ListViewItem lviCurr = itemsListView.Items[ndxCurr];
                        ListViewItem lviTmp = new ListViewItem("temp");
                        lviTmp.Tag = currentItem;
                        if (_direction == -1 && ndxCurr >= 0)
                        {
                            int ndxPrev = ndxCurr - 1;
                            ListViewItem lviPrev = itemsListView.Items[ndxPrev];
                            (lviPrev.Tag as Item).Load();
                            itemsListView.Items[ndxPrev] = lviTmp;
                            itemsListView.Items[ndxCurr] = lviPrev;
                            itemsListView.Items[ndxPrev] = lviCurr;
                        }
                        else if (_direction == 1 && ndxCurr <= itemsListView.Items.Count - 1)
                        {
                            int ndxNext = ndxCurr + 1;
                            ListViewItem lviNext = itemsListView.Items[ndxNext];
                            (lviNext.Tag as Item).Load();
                            itemsListView.Items[ndxNext] = lviTmp;
                            itemsListView.Items[ndxCurr] = lviNext;
                            itemsListView.Items[ndxNext] = lviCurr;
                        }
                    }
                }
                return;
            }
            else
            {
                Item[] items = new Item[itemsListView.SelectedItems.Count];
                int i = 0;
                foreach (ListViewItem lvi in itemsListView.SelectedItems)
                    items[i++] = (lvi.Tag as Item);
                if (_direction < 0)
                {
                    for (int j = 0; j < i; j++)
                    {
                        Item item = items[j];
                        bool ok = item.Up();
                        if (ok)
                            atLeastOneOK = true;
                    }
                }                
                else
                {
                    for (int j = i - 1; j >= 0;  j--)
                    {
                        Item item = items[j];
                        bool ok = item.Down();
                        if (ok)
                            atLeastOneOK = true;
                    }
                }
            }
            if (!atLeastOneOK)
                return;
            if(autoRefresh.Checked)
                this.RefreshItemsListView();
        }
        private void GoToFirstItem()
        {
            if (itemsListView.Items.Count == 0)
                return;
            itemsListView.SelectedItems.Clear();
            itemsListView.Items[0].Selected = true;
            itemsListView.Items[0].Focused = true;
            SetCurrentItem(itemsListView.Items[0].Tag as Item);
            itemsListView_ItemSelectionChanged(null, null);
        }
        private void GoToPrevItem()
        {
            if (itemsListView.Items.Count < 1)
                return;
            int focused = -1;
            int i = 0;
            foreach (ListViewItem lvi in itemsListView.Items)
            {
                if (lvi.Focused)
                {
                    focused = i;
                    break;
                }
                i++;
            }
            if (focused < 0 || focused == 0)
                return;
            focused--;
            itemsListView.SelectedItems.Clear();
            itemsListView.Items[focused].Selected = true;
            itemsListView.Items[focused].Focused = true;
            SetCurrentItem(itemsListView.Items[focused].Tag as Item);
            itemsListView_ItemSelectionChanged(null, null);
        }
        private void GoToNextItem()
        {
            if (itemsListView.Items.Count < 1)
                return;
            int focused = -1;
            int i = 0;
            foreach (ListViewItem lvi in itemsListView.Items)
            {
                if (lvi.Focused)
                {
                    focused = i;
                    break;
                }
                i++;
            }
            if (focused < 0 || focused == itemsListView.Items.Count-1)
                return;
            focused++;
            itemsListView.SelectedItems.Clear();
            itemsListView.Items[focused].Selected = true;
            itemsListView.Items[focused].Focused = true;
            SetCurrentItem(itemsListView.Items[focused].Tag as Item);
            itemsListView_ItemSelectionChanged(null, null);
        }
        private void GoToLastItem()
        {
            if (itemsListView.Items.Count == 0)
                return;
            itemsListView.SelectedItems.Clear();
            itemsListView.Items[itemsListView.Items.Count-1].Selected = true;
            itemsListView.Items[itemsListView.Items.Count - 1].Focused = true;
            SetCurrentItem(itemsListView.Items[itemsListView.Items.Count - 1].Tag as Item);
            itemsListView_ItemSelectionChanged(null, null);
        }

        private void renumItemsFNAMEButton_Click(object sender, EventArgs e)
        {
            if (currentViewLink == null)
                return;
            if (currentViewLink.View.RenumItems("FNAME"))
                this.RefreshItemsListView();
        }
        private void renumItemsMMDDButton_Click(object sender, EventArgs e)
        {
            if (currentViewLink == null)
                return;
            if (currentViewLink.View.RenumItems("MMDD"))
                this.RefreshItemsListView();
        }
        private void renumItemsLPButton_Click(object sender, EventArgs e)
        {
            if (currentViewLink == null)
                return;
            if (currentViewLink.View.RenumItems("LP"))
                this.RefreshItemsListView();
        }

        #endregion

        #region Clipboard

        private void copyViewButton_Click(object sender, EventArgs e)
        {
            this.CopyCutViewLink(CopyCutMode.Copy);
        }
        private void cutViewButton_Click(object sender, EventArgs e)
        {
            this.CopyCutViewLink(CopyCutMode.Cut);
        }
        private void CopyCutViewLink(CopyCutMode _mode)
        {
            clipboardViewLink = this.currentViewLink;
            CopyCutViewMode = _mode;
        }
        private void pasteViewButton_Click(object sender, EventArgs e)
        {
            if (clipboardViewLink == null || currentViewLink == clipboardViewLink)
                return;
            if (CopyCutViewMode == CopyCutMode.Copy)
            {
                ViewLink newLink = null;
                if(currentViewLink == null)
                    newLink = new ViewLink(currentAlbum, clipboardViewLink.View, this.connectString);
                else
                    newLink = new ViewLink(currentViewLink, clipboardViewLink.View, this.connectString);
                newLink.Save(false);
                TreeNode node = null;
                if (currentViewLink != null)
                {
                    node = this.FindNode(albumsTreeView.Nodes, currentViewLink);
                    this.PopulateViewLink(node, newLink, true);
                }
                else
                {
                    node = this.FindNode(albumsTreeView.Nodes, currentAlbum);
                    this.PopulateViewLink(node, newLink, true);
                }
            }
            else if (CopyCutViewMode == CopyCutMode.Cut)
            {
                ViewLink prevParentViewLink = clipboardViewLink.ParentViewLink;
                Album prevParentAlbum = clipboardViewLink.ParentAlbum;
                clipboardViewLink.ParentAlbum = currentAlbum;
                clipboardViewLink.ParentViewLink = currentViewLink;
                clipboardViewLink.Save(false);
                TreeNode clipboardNode = this.FindNode(albumsTreeView.Nodes, clipboardViewLink);
                TreeNode prevParentNode = null;
                if (prevParentViewLink != null)
                {
                    ViewLink sublinkToDel = null;
                    foreach (ViewLink vl in prevParentViewLink.SubLinks)
                    {
                        if (vl.ID == clipboardViewLink.ID)
                        {
                            sublinkToDel = vl;
                            break;
                        }
                    }
                    if (sublinkToDel != null)
                    {
                        if (!sublinkToDel.Loaded)
                            sublinkToDel.Load(1, true);
                        if (prevParentViewLink != null)
                            prevParentViewLink.SubLinks.Remove(sublinkToDel);
                        else if (prevParentAlbum != null)
                            prevParentAlbum.ViewsLinks.Remove(sublinkToDel);
                    }
                    prevParentNode = this.FindNode(albumsTreeView.Nodes, prevParentViewLink);
                }
                else
                    prevParentNode = this.FindNode(albumsTreeView.Nodes, prevParentAlbum);
                TreeNode currParentNode = this.FindNode(albumsTreeView.Nodes, currentViewLink);
                if (currParentNode == null)
                    currParentNode = this.FindNode(albumsTreeView.Nodes, currentAlbum);
                prevParentNode.Nodes.Remove(clipboardNode);
                currParentNode.Nodes.Add(clipboardNode);
            }
            this.DisplayNOfLinks();
        }

        private void copyItemsButton_Click(object sender, EventArgs e)
        {
            this.CopyCutItems(CopyCutMode.Copy);
        }
        private void cutItemsButton_Click(object sender, EventArgs e)
        {
            this.CopyCutItems(CopyCutMode.Cut);
        }
        private void CopyCutItems(CopyCutMode _mode)
        {
            clipboardItems.Clear();
            foreach (ListViewItem lvi in itemsListView.SelectedItems)
            {
                Item item = lvi.Tag as Item;
                clipboardItems.Add(item);
            }
            CopyCutItemsMode = _mode;
            CopyCutLastFrom = CopyCutLastFrom.Items;
            this.ItemsShowClipboardMode(_mode);
        }
        private void pasteItemsButton_Click(object sender, EventArgs e)
        {            
            if (this.CopyCutItemsMode == CopyCutMode.None && this.CopyCutFilesMode == CopyCutMode.None)
                return;
            if (this.CopyCutLastFrom == CopyCutLastFrom.Items)
            {
                int lp = this.SetInsertLp();
                itemsListView.SelectedItems.Clear();
                foreach (Item item in clipboardItems)
                {
                    Item itemAdded = new Item(currentViewLink.View, 0, this.connectString);
                    itemAdded.FileSpec = item.FileSpec;
                    itemAdded.IsImportant = item.IsImportant;
                    itemAdded.Quality = item.Quality;
                    itemAdded.IsArt = item.IsArt;
                    itemAdded.IsHidden = item.IsHidden;
                    itemAdded.Save(lp);
                    lp = item.Lp + 1; //Jeśli kopiujemy itemy z widoku do innego widoku, to nawet w przypadku wstawiania po nazwie chcemy zachować kolejność, bo w widoku źródłowym mogliśmy poprzestawiać
                    this.AddItemToList(itemAdded, -1, false, true);
                }
                if (CopyCutItemsMode == CopyCutMode.Cut)
                {
                    itemsCopyCutLabel.Text = "";
                    if (clipboardItems.Count == 0)
                        return;
                    List<Item> itemsToDel = new List<Item>();
                    foreach (Item item in clipboardItems)
                        itemsToDel.Add(item);
                    int i = 0;
                    View parentView = itemsToDel[0].ParentView;
                    foreach (Item item in itemsToDel)
                        clipboardItems[i++].Delete(false);
                }                
            }
            else if (this.CopyCutLastFrom == CopyCutLastFrom.Files)
            {
                int lp = this.SetInsertLp();
                itemsListView.SelectedItems.Clear();
                foreach (ResourceFile file in clipboardFiles)
                {
                    Item itemAdded = new Item(currentViewLink.View, 0, this.connectString);
                    itemAdded.FileSpec = file.RealFileSpec;
                    itemAdded.IsImportant = false;
                    itemAdded.Quality = ItemQuality.Normal;
                    itemAdded.IsArt = false;
                    itemAdded.IsHidden = false;                    
                    //itemAdded.Save();
                    itemAdded.Save(lp);
                    if (insertMode != InsertMode.ByName)
                        lp = itemAdded.Lp + 1; //Jeśli kopiujemy itemy z widoku do innego widoku, to chcemy zachować kolejność, bo w widoku źródłowym mogliśmy poprzestawiać
                    this.AddItemToList(itemAdded, -1, false, true);
                }
            }
            if (autoRefresh.Checked)
                this.RefreshItemsListView();
            if (this.CopyCutLastFrom == CopyCutLastFrom.Items)
                itemsListView.Select();
            else if (this.CopyCutLastFrom == CopyCutLastFrom.Files)
                filesListView.Select();
            this.DisplayNOfLinks();
        }
        private int SetInsertLp()
        {
            int lp = 0;
            if (insertMode == InsertMode.ByName)            //Wg nazwy pierwszego
                lp = 0;
            else if (insertMode == InsertMode.OnFirst)      //Na początku (przed pierwszym)
            {
                lp = 1;
                foreach (ListViewItem lvi in itemsListView.Items)
                    (lvi.Tag as Item).Lp++;
            }
            else if (insertMode == InsertMode.OnSelected)  //Od wskazanego
            {
                if (itemsListView.SelectedItems.Count > 0)
                {
                    lp = (itemsListView.SelectedItems[0].Tag as Item).Lp;
                    (itemsListView.SelectedItems[0].Tag as Item).Lp += 1;
                }
            }
            else if (insertMode == InsertMode.OnLast)       //Na końcu (po ostatnim)
            {
                if (itemsListView.Items.Count > 0)
                    lp = (itemsListView.Items[itemsListView.Items.Count - 1].Tag as Item).Lp + 1;
            }
            return lp;
        }
        private void ItemsShowClipboardMode(CopyCutMode _mode)
        {
            if (clipboardItems.Count > 0)
            {
                itemsCopyCutLabel.Text = (_mode == CopyCutMode.Copy ? "C" : "X") + clipboardItems.Count;
                if (itemsCopyCutLabel.ForeColor == System.Drawing.Color.Blue)
                    itemsCopyCutLabel.ForeColor = System.Drawing.Color.Red;
                else
                    itemsCopyCutLabel.ForeColor = System.Drawing.Color.Blue;
            }
            else
                itemsCopyCutLabel.Text = "";
        }

        private void ShowCurrentInsertMode()
        {
            switch (insertMode)
            {
                case InsertMode.ByName:
                    {
                        insertModeText.Text = "ByName";
                        break;
                    }
                case InsertMode.OnFirst:
                    {
                        insertModeText.Text = "OnFirst";
                        break;
                    }
                case InsertMode.OnSelected:
                    {
                        insertModeText.Text = "OnSelected";
                        break;
                    }
                case InsertMode.OnLast:
                    {
                        insertModeText.Text = "OnLast";
                        break;
                    }
            }
        }

        private void copyToRoboczyButton_Click(object sender, EventArgs e)
        {
            this.CopyItems2Roboczy();
        }
        private void CopyItems2Roboczy()
        {
            foreach (ListViewItem lvi in itemsListView.SelectedItems)
            {
                Item item = lvi.Tag as Item;
                string newFSpec = @"D:\Multimedia\Moje\!Roboczy\!go\" + System.IO.Path.GetFileName(item.FileSpec);
                System.IO.File.Copy(item.FileSpec, newFSpec);
            }
            itemsListView.Focus();
        }

        #endregion

        #region Play

        private void itemsListView_Enter(object sender, EventArgs e)
        {
            this.ItemsListSelected();
        }
        private void ItemsListSelected()
        {
            this.Play(true);
        }

        private void playItemsButton_Click(object sender, EventArgs e)
        {
            //if(currentItem == null)
            //    return;
            List<PlayableObject> objectsToPlay = new List<PlayableObject>();
            int currentNdx = -1;
            if(currentViewLink == null)
                currentNdx = this.playItemsButton_AddAlbum(currentAlbum, objectsToPlay);
            else
                currentNdx = this.playItemsButton_AddView(currentViewLink, objectsToPlay);
            if (objectsToPlay.Count == 0)
                return;
            PlayerForm fsp = new PlayerForm(this.initialFullScreenZoomFactorCoeff, this.initialFullScreenMoveDelta, uxShowTextNotes.Checked);
            fsp.objectsToPlay = objectsToPlay;
            fsp.startNdx = (currentNdx >= 0 ? currentNdx : 0);
            fsp.ShowDialog();
            itemsListView.Select();
        }
        private int playItemsButton_AddAlbum(Album _album, List<PlayableObject> _objectsToPlay)
        {
            if (_album == null)
                return -1;
            int i = 0;
            int currentNdx = -1;
            foreach (ViewLink viewLink in _album.ViewsLinks)
            {
                this.playItemsButton_AddView(viewLink, _objectsToPlay);
                if (currentViewLink != null && currentViewLink.View.Name == viewLink.Name)
                    currentNdx = i;
                i++;
            }
            return currentNdx;
        }
        private int playItemsButton_AddView(ViewLink _viewLink, List<PlayableObject> _objectsToPlay)
        {
            int i = 0;
            int currentNdx = -1;
            foreach (Item item in _viewLink.View.Items)
            {
                if (!impAlwaysCheck.Checked || !item.IsImportant)
                {
                    switch (item.Quality)
                    {
                        case ItemQuality.Low:
                            {
                                if (!lowQualityCheck.Checked)
                                    continue;
                                break;
                            }
                        case ItemQuality.Normal:
                            {
                                if (!normalQualityCheck.Checked)
                                    continue;
                                break;
                            }
                        case ItemQuality.Good:
                            {
                                if (!goodQualityCheck.Checked)
                                    continue;
                                break;
                            }
                        case ItemQuality.Best:
                            {
                                if (!bestQualityCheck.Checked)
                                    continue;
                                break;
                            }
                        case ItemQuality.Extra:
                            {
                                if (!extraQualityCheck.Checked)
                                    continue;
                                break;
                            }
                    }
                }
                if (artOnlyCheck.Checked && !item.IsArt)
                    continue;
                _objectsToPlay.Add(new PlayableObject(item.FileSpec, item.Quality));
                if (currentItem != null && item.FileSpec == currentItem.FileSpec)
                    currentNdx = i;
                i++;
            }
            foreach (ViewLink subViewLink in _viewLink.SubLinks)
                this.playItemsButton_AddView(subViewLink, _objectsToPlay);
            return currentNdx;
        }

        #endregion

        #endregion


        #region Resources Folders tree

        #region Select

        private void foldersTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode n = e.Node;
            if (n == null) return;
            // Interesuje nas tylko wezel z pojedynczym placeholderem "(not loaded)" (Tag==null)
            if (n.Nodes.Count != 1 || n.Nodes[0].Tag != null) return;

            ResourceFolder rf = n.Tag as ResourceFolder;
            if (rf == null) return;

            Cursor prev = this.Cursor;
            this.Cursor = Cursors.WaitCursor;
            try
            {
                // Zaladuj aktualny poziom (subfoldery + pliki) jesli jeszcze nie zaladowany
                if (!rf.Loaded)
                    rf.Load(false, true);

                n.Nodes.Clear();
                foreach (ResourceFolder sub in rf.subFolders)
                    this.PopulateFolder(n, sub, false);
            }
            finally { this.Cursor = prev; }
        }

        private void resourcesTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (foldersTreeView.SelectedNode == null)
                return;
            object tag = foldersTreeView.SelectedNode.Tag;
            if (tag == null)
            {
                tag = foldersTreeView.SelectedNode.Parent.Tag;
                if (tag == null)
                    return;
            }
            SetCurrentFolder((ResourceFolder)tag);
            SetCurrentFile(null);
            
            filesListView.Items.Clear();
            filesListImageList.Images.Clear();
            int i = 0;
            foreach (ResourceFile file in currentFolder.Files)
            {                
                try
                {
                    object imgico = file.Thumbnail;
                    if (imgico == null)
                    {
                        imgico = this.GetThumbnail(file.RealFileSpec, file.ContentType); 
                        file.Thumbnail = imgico;
                    }
                    if (imgico is Icon)
                        filesListImageList.Images.Add(imgico as Icon);
                    else if (imgico is Image)
                        filesListImageList.Images.Add(imgico as Image);
                }
                catch (Exception) { }
                this.AddFileToList(file, i, false, false);
                if (currentFile == null)
                    SetCurrentFile(file);
                i++;
            }
            if (afterGoToFileW)
            {
                this.SetGivenFile(fileW);
                afterGoToFileW = false;
            }
            else if (afterGoToFileJ)
            {
                this.SetGivenFile(fileJ);
                afterGoToFileJ = false;
            }
            this.ShowCurrentFileLabel();
            this.Play(false);
        }

        private void resourcesTreeView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Insert || e.KeyCode == Keys.F7 && e.Modifiers != Keys.Control)
                addFolderButton.PerformClick();
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Enter || e.Modifiers != Keys.Control && (e.KeyCode == Keys.F2 || e.KeyCode == Keys.F4))
                renFolderButton.PerformClick();
            else if (e.KeyCode == Keys.Delete)
                delFolderButton.PerformClick();
            else if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
                pasteFilesButton.PerformClick();
            else if (e.KeyCode == Keys.V && e.Modifiers == Keys.Alt)
                pasteShortcutsButton.PerformClick();
            else if (e.KeyCode == Keys.Left && e.Modifiers == Keys.Control)
                foldersHistoryBackButton.PerformClick();
            else if (e.KeyCode == Keys.Right && e.Modifiers == Keys.Control)
                foldersHistoryNextButton.PerformClick();
            else if (e.KeyCode == Keys.W && e.Modifiers == Keys.Alt)
                setFolderCButton.PerformClick();
            else if (e.KeyCode == Keys.J && e.Modifiers == Keys.Alt)
                setFolderJButton.PerformClick();
            else if (e.KeyCode == Keys.Left && e.Modifiers == Keys.Alt)
                gotoFolderCButton.PerformClick();
            else if (e.KeyCode == Keys.Right && e.Modifiers == Keys.Alt)
                gotoFolderJButton.PerformClick();
            else if (e.KeyCode == Keys.F5)
            {
                if(loadFullResourcesTreeCheck.Checked)
                    this.RefreshResourcesTreeView();
                else
                    this.RefreshResourcesCurrentFolder(e.Modifiers == Keys.Control);
            }
            SendKeys.Flush();
        }
        private void resourcesTreeView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            renFolderButton.PerformClick();
        }

        private void setFolderWButton_Click(object sender, EventArgs e)
        {
            folderWNode = foldersTreeView.SelectedNode;
            if (filesListView.FocusedItem != null && filesListView.FocusedItem.Tag != null)
                fileW = (filesListView.FocusedItem.Tag as ResourceFile);
            foldersTreeView.Select();
        }
        private void setFolderJButton_Click(object sender, EventArgs e)
        {
            folderJNode = foldersTreeView.SelectedNode;
            if (filesListView.FocusedItem != null && filesListView.FocusedItem.Tag != null)
                fileJ = (filesListView.FocusedItem.Tag as ResourceFile);
            foldersTreeView.Select();
        }
        private void foldersTreeViewBack_Click(object sender, EventArgs e)
        {
            if (foldersHistoryNdx > 0 && foldersHistory[foldersHistoryNdx - 1] != null)
            {
                foldersHistoryGoingBackOrNext = true;
                foldersTreeView.SelectedNode = foldersHistory[--foldersHistoryNdx];
            }
            foldersTreeView.Select();
        }
        private void foldersTreeViewNext_Click(object sender, EventArgs e)
        {
            if (foldersHistoryNdx < foldersHistory.Length - 1 && foldersHistory[foldersHistoryNdx + 1] != null)
            {
                foldersHistoryGoingBackOrNext = true;
                foldersTreeView.SelectedNode = foldersHistory[++foldersHistoryNdx];                
            }
            foldersTreeView.Select();
        }
        private void gotoFolderWButton_Click(object sender, EventArgs e)
        {
            if (folderWNode == null)
                return;
            afterGoToFileW = true;
            foldersTreeView.SelectedNode = folderWNode;
            foldersTreeView.Select();
        }
        private void gotoFolderJButton_Click(object sender, EventArgs e)
        {
            if (folderJNode == null)
                return;
            afterGoToFileJ = true;
            foldersTreeView.SelectedNode = folderJNode;
            foldersTreeView.Select();
        }

        private void SetCurrentFolder(ResourceFolder _folder)
        {
            currentFolder = _folder;
            if (currentFolder != null)
                currentFolderLabel.Text = currentFolder.Path;
            else
                currentFolderLabel.Text = String.Empty;
            if (foldersHistoryGoingBackOrNext)
                foldersHistoryGoingBackOrNext = false;
            else
            {
                if (foldersTreeView.SelectedNode != null && foldersTreeView.SelectedNode.Tag != null)
                {
                    if (foldersHistoryNdx == -1 || foldersTreeView.SelectedNode != foldersHistory[foldersHistoryNdx])
                    {
                        if (foldersHistoryNdx < foldersHistory.Length - 1)
                            foldersHistory[++foldersHistoryNdx] = foldersTreeView.SelectedNode;
                        else
                        {
                            for (int i = 0 + 1; i < foldersHistory.Length-1; i++)
                                foldersHistory[i] = foldersHistory[i+1];
                            foldersHistory[foldersHistoryNdx] = foldersTreeView.SelectedNode;
                        }
                    }
                }
            }
        }

        #endregion

        #region Edit

        private void addFolderButton_Click(object sender, EventArgs e)
        {
            ResourceFolder folder = null;
            if (foldersTreeView.SelectedNode == null)
                return;
            object parentTag = foldersTreeView.SelectedNode.Tag;
            if (parentTag == null)
                return;
            folder = new ResourceFolder(parentTag as ResourceFolder, String.Empty, this.resourcesFolder);
            ResourceFolderForm formFolder = new ResourceFolderForm();
            formFolder.folder = folder;
            formFolder.ConnectString = this.connectString;
            formFolder.ShowDialog();
            if (formFolder.OK)
            {
                TreeNode parentNode = null;
                parentNode = this.FindNode(foldersTreeView.Nodes, parentTag as ResourceFolder);
                this.PopulateFolder(parentNode, folder, true);
                TreeNode node = this.FindNode(foldersTreeView.Nodes, folder);
                foldersTreeView.SelectedNode = node;
            }
            foldersTreeView.Select();
        }
        private void renFolderButton_Click(object sender, EventArgs e)
        {
            if (foldersTreeView.SelectedNode == null)
                return;
            object tag = foldersTreeView.SelectedNode.Tag;
            if (tag == null)
                return;
            ResourceFolderForm formFolder = new ResourceFolderForm();
            ResourceFolder folder = (ResourceFolder)tag;
            formFolder.folder = folder;
            string oldFolderSpec = folder.FolderSpec;
            formFolder.ConnectString = this.connectString;
            formFolder.ShowDialog();
            if (formFolder.OK)
            {
                TreeNode node = this.FindNode(foldersTreeView.Nodes, folder);
                node.Text = folder.Name;
            }
            foreach (Album album in albums)
            {
                foreach (ViewLink viewLink in album.ViewsLinks)
                    this.RenameFolderInViewItems(viewLink, oldFolderSpec, folder.FolderSpec);
            }
            foldersTreeView.Select();
        }
        private void RenameFolderInViewItems(ViewLink _viewLink, string _oldFolderSpec, string _newFolderSpec)
        {
            foreach (ViewLink viewLink in _viewLink.SubLinks)
                this.RenameFolderInViewItems(viewLink, _oldFolderSpec, _newFolderSpec);
            foreach (Item item in _viewLink.View.Items)
            {
                if (item.FileSpec == (_oldFolderSpec + "\\" + item.FileName))
                {
                    item.FileSpec = _newFolderSpec + "\\" + item.FileName;
                    item.Save();
                }
            }
        }
        private void delFolderButton_Click(object sender, EventArgs e)
        {
            ResourceFolder parentFolder = null;
            TreeNode node = this.FindNode(foldersTreeView.Nodes, currentFolder);
            if (node.Parent == null)
            {
                MessageBox.Show("Cannot delete main resources folder. You can do it manually, outside the program.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            parentFolder = (node.Parent.Tag) as ResourceFolder;

            DialogResult dr = MessageBox.Show("Are you sure? OPERATION WILL PERMANENTLY DELETE SELECTED FOLDER, ALL ITS SUBFOLDERS, FILES, SHORTCUTS AND VIEW ITEMS (LINKS) OF THESE FILES.", "Warning",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            this.ClearKeyboardBuffer();
            if (dr != DialogResult.OK)
            {
                foldersTreeView.Select();
                return;
            }

            if (currentFolder.Delete(false, this.connectString, this.noCheckDeleteFDCheck.Checked))
                foldersTreeView.Nodes.Remove(node);

            SetCurrentFolder(parentFolder);

            this.DeleteAlbumsViewsItemsOfFolder(currentFolder);
            foldersTreeView.Select();
        }
        private void DeleteAlbumsViewsItemsOfFolder(ResourceFolder _folder)
        {
            foreach (TreeNode albumNode in albumsTreeView.Nodes)
            {
                Album album = albumNode.Tag as Album;
                foreach (ViewLink viewLink in album.ViewsLinks)
                    this.DeleteViewItemsOfFolder(viewLink, _folder);
            }
            return;
        }
        private void DeleteViewItemsOfFolder(ViewLink _viewLink, ResourceFolder _folder)
        {
            foreach (ResourceFolder folder in _folder.subFolders)
                this.DeleteViewItemsOfFolder(_viewLink, folder);
            foreach (ResourceFile file in _folder.Files)
                this.DeleteViewItemsOfFile(_viewLink, file);
        }

        #endregion

        #endregion


        #region Resource Files list

        #region Select

        private void filessListView_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
                filesRowChanging = this.IsFirstFileInRow();
            else if (e.KeyCode == Keys.Right)
                filesRowChanging = this.IsLastFileInRow();
        }
        private bool IsFirstFileInRow()
        {
            return (this.CurrentFileNdx() % Convert.ToInt32(filesPerRowSpin.Value)) == 0;
        }
        private bool IsLastFileInRow()
        {
            return (this.CurrentFileNdx() % Convert.ToInt32(filesPerRowSpin.Value)) == Convert.ToInt32(filesPerRowSpin.Value) - 1;
        }
        private int CurrentFileNdx()
        {
            int retVal = -1;
            int i = 0;
            foreach (ListViewItem lvi in filesListView.Items)
            {
                if ((lvi.Tag as ResourceFile) == currentFile)
                {
                    retVal = i;
                    break;
                }
                i++;
            }
            return retVal;
        }

        private void filesListView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && e.Modifiers != Keys.Shift)
                delFilesButton.PerformClick();
            else if (e.KeyCode == Keys.Delete && e.Modifiers == Keys.Shift)
                delFilesNoCheckButton.PerformClick();
            else if (e.KeyCode == Keys.L && e.Modifiers != Keys.Control && e.Modifiers != Keys.Alt && e.Modifiers != Keys.Shift)
                this.LinkFiles(false, ItemQuality.Low, false, false);
            else if (e.KeyCode == Keys.L && e.Modifiers == Keys.Shift)
                this.LinkFiles(false, ItemQuality.Low, false, true);
            else if (e.KeyCode == Keys.N && e.Modifiers != Keys.Control && e.Modifiers != Keys.Alt && e.Modifiers != Keys.Shift)
                this.LinkFiles(false, ItemQuality.Normal, false, false);
            else if (e.KeyCode == Keys.N && e.Modifiers == Keys.Shift)
                this.LinkFiles(false, ItemQuality.Normal, false, true);
            else if (e.KeyCode == Keys.G && e.Modifiers != Keys.Control && e.Modifiers != Keys.Alt && e.Modifiers != Keys.Shift)
                this.LinkFiles(false, ItemQuality.Good, false, false);
            else if (e.KeyCode == Keys.G && e.Modifiers == Keys.Shift)
                this.LinkFiles(false, ItemQuality.Good, false, true);
            else if (e.KeyCode == Keys.B && e.Modifiers != Keys.Control && e.Modifiers != Keys.Alt && e.Modifiers != Keys.Shift)
                this.LinkFiles(false, ItemQuality.Best, false, false);
            else if (e.KeyCode == Keys.B && e.Modifiers == Keys.Shift)
                this.LinkFiles(false, ItemQuality.Best, false, true);
            else if (e.KeyCode == Keys.E && e.Modifiers != Keys.Control && e.Modifiers != Keys.Alt && e.Modifiers != Keys.Shift)
                this.LinkFiles(false, ItemQuality.Extra, false, false);
            else if (e.KeyCode == Keys.E && e.Modifiers == Keys.Shift)
                this.LinkFiles(false, ItemQuality.Extra, false, true);
            else if (e.KeyCode == Keys.A && e.Modifiers == Keys.Control)
                this.SelectAllFiles();
            else if (e.KeyCode == Keys.Back)
                linkFilesAddButton.PerformClick();
            else if (e.KeyCode == Keys.C && e.Modifiers == Keys.Control)
                copyFilesButton.PerformClick();
            else if (e.KeyCode == Keys.X && e.Modifiers == Keys.Control)
                cutFilesButton.PerformClick();
            else if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
                pasteFilesButton.PerformClick();
            else if (e.KeyCode == Keys.V && e.Modifiers == Keys.Alt)
                pasteShortcutsButton.PerformClick();
            else if (e.KeyCode == Keys.Home)
                this.SetFirstFile();
            else if (e.KeyCode == Keys.Left /*&& e.Modifiers == Keys.Alt*/)
            {
                //TimeSpan dt = DateTime.Now.Subtract(lastTimeFileSelected);
                //if (dt.Hours > 0 || dt.Minutes > 0 || dt.Seconds > 0 || dt.Milliseconds > lastTimeSelectedMilisecondsDelay)
                //    this.SetPrevFile();
                if (filesRowChanging)
                {
                    this.SetPrevFile();
                    filesRowChanging = false;
                }                
            }
            else if (e.KeyCode == Keys.Right /*&& e.Modifiers == Keys.Alt*/)
            {
                //TimeSpan dt = DateTime.Now.Subtract(lastTimeFileSelected);
                //if (dt.Hours > 0 || dt.Minutes > 0 || dt.Seconds > 0 || dt.Milliseconds > lastTimeSelectedMilisecondsDelay)
                //    this.SetNextFile();
                if (filesRowChanging)
                {
                    this.SetNextFile();
                    filesRowChanging = false;
                }
            }
            else if (e.KeyCode == Keys.End)
                this.SetLastFile();
            if (e.KeyCode == Keys.Enter && e.Modifiers != Keys.Control)
                playFilesButton.PerformClick();
            //if (e.KeyCode == Keys.F2 || e.KeyCode == Keys.Enter && e.Modifiers == Keys.Control)
            //    renFileButton.PerformClick();
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Insert)
                crtSfxFileButton.PerformClick();
            else if ((e.Modifiers == Keys.Control && e.KeyCode == Keys.Enter) || e.Modifiers != Keys.Control && (e.KeyCode == Keys.F2 || e.KeyCode == Keys.F3 || e.KeyCode == Keys.F4))
                chgFileButton.PerformClick();
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.F2)
                this.RunEditorOnFile(1);
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.F3)
                this.RunEditorOnFile(2);
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.F4)
                this.RunEditorOnFile(3);
            else if (e.KeyCode == Keys.F5)
            {
                if (loadFullResourcesTreeCheck.Checked)
                    this.RefreshResourcesTreeView();
                else
                    this.RefreshResourcesCurrentFolder(e.Modifiers == Keys.Control);
                filesListView.Select();
            }
            SendKeys.Flush();
        }
        private void filesListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            playFilesButton.PerformClick();
        }

        private void filesListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e != null && e.Item != null && e.Item.Selected)
                SetCurrentFile((ResourceFile)(e.Item.Tag));
            //if (e != null && e.Item.Selected)
            this.FilesListSelected();
            //lastTimeFileSelected = DateTime.Now;
        }

        private void SetCurrentFile(ResourceFile _file)
        {
            currentFile = _file;
            Play(false);
            this.ShowCurrentFileLabel();
        }
        private void ShowCurrentFileLabel()
        {
            if (currentFolder != null)
            {
                currentFileLabel.Text = "(" + filesListView.Items.Count + ")";
                if (currentFile != null)
                    currentFileLabel.Text += (" " + currentFile.FileName);
            }
            else
                currentFileLabel.Text = String.Empty;
            currentFileLabel.Show();
        }

        private void SelectAllFiles()
        {
            foreach (ListViewItem lvi in filesListView.Items)
                lvi.Selected = true;
        }

        #endregion

        #region Navigate

        private void SetFirstFile()
        {
            if (filesListView.Items.Count == 0)
                return;
            filesListView.SelectedItems.Clear();
            filesListView.Items[0].Selected = true;
            filesListView.Items[0].Focused = true;
            SetCurrentFile(filesListView.Items[0].Tag as ResourceFile);
            filesListView_ItemSelectionChanged(null, null);
        }
        private void SetPrevFile()
        {
            if (filesListView.Items.Count < 1)
                return;
            int focused = -1;
            int i = 0;
            foreach (ListViewItem lvi in filesListView.Items)
            {
                if (lvi.Focused)
                {
                    focused = i;
                    break;
                }
                i++;
            }
            if (focused < 0 || focused == 0)
                return;
            focused--;
            filesListView.SelectedItems.Clear();
            filesListView.Items[focused].Selected = true;
            filesListView.Items[focused].Focused = true;
            SetCurrentItem(filesListView.Items[focused].Tag as Item);
            filesListView_ItemSelectionChanged(null, null);
        }
        private void SetNextFile()
        {
            if (filesListView.Items.Count < 1)
                return;
            int focused = -1;
            int i = 0;
            foreach (ListViewItem lvi in filesListView.Items)
            {
                if (lvi.Focused)
                {
                    focused = i;
                    break;
                }
                i++;
            }
            if (focused < 0 || focused == filesListView.Items.Count - 1)
                return;
            focused++;
            filesListView.SelectedItems.Clear();
            filesListView.Items[focused].Selected = true;
            filesListView.Items[focused].Focused = true;
            SetCurrentItem(filesListView.Items[focused].Tag as Item);
            filesListView_ItemSelectionChanged(null, null);
        }
        private void SetLastFile()
        {
            if (filesListView.Items.Count == 0)
                return;
            filesListView.SelectedItems.Clear();
            filesListView.Items[filesListView.Items.Count - 1].Selected = true;
            filesListView.Items[filesListView.Items.Count - 1].Focused = true;
            SetCurrentItem(filesListView.Items[filesListView.Items.Count - 1].Tag as Item);
            filesListView_ItemSelectionChanged(null, null);
        }

        private void SetGivenFile(ResourceFile _file)
        {
            if (_file == null)
                return;
            int i = 0;
            bool found = false;
            foreach (ListViewItem lvi in filesListView.Items)
            {
                if ((lvi.Tag as ResourceFile).RealFileSpec == _file.RealFileSpec)
                {
                    found = true;
                    break;
                }
                i++;
            }
            if (!found)
                return;
            filesListView.SelectedItems.Clear();
            filesListView.Items[i].Selected = true;
            filesListView.Items[i].Focused = true;
            SetCurrentItem(filesListView.Items[i].Tag as Item);
            filesListView_ItemSelectionChanged(null, null);
            filesListView.EnsureVisible(i);
        }

        #endregion

        #region Edit

        private void chgFileButton_Click(object sender, EventArgs e)
        {
            chgFile(false);
        }
        private void editFileContentButton_Click(object sender, EventArgs e)
        {
            RunEditorOnFile(1);
        }
        private void crtSfxFileButton_Click(object sender, EventArgs e)
        {
            if (currentFile == null)
                return;
            ResourceFile file = currentFile;
            string fName = System.IO.Path.GetFileNameWithoutExtension(file.RealFileSpec);
            if (fName.Contains("_album"))
            {
                System.Windows.Forms.MessageBox.Show("File is alerady suffixed file.");
                return;
            }
            string sfxFName = fName + "_album" + System.IO.Path.GetExtension(file.RealFileSpec);
            string sfxSpec = file.RealFolder.FolderSpec + "\\" + sfxFName;
            System.IO.File.Copy(file.RealFileSpec, sfxSpec, true);

            ResourceFile sfxFile = new ResourceFile(file.RealFolder, sfxFName, this.resourcesFolder);

            RunEditor(1, sfxFile, String.Empty, false);

            if(file.isShortcut)
            {
                string shSpec = file.ParentFolder.FolderSpec + "\\" + sfxFName;
                shSpec = utils.MakeShortcutFileSpec(shSpec);
                utils.CreateShortcut(shSpec, sfxSpec);
                sfxFile = new ResourceFile(file.ParentFolder, utils.MakeShortcutFileSpec(sfxFName), this.resourcesFolder);
            }

            try
            {
                object imgico = sfxFile.Thumbnail;
                if (imgico == null)
                {
                    imgico = this.GetThumbnail(sfxFile.RealFileSpec, file.ContentType);
                    sfxFile.Thumbnail = imgico;
                }
                if (imgico is Icon)
                    filesListImageList.Images.Add(imgico as Icon);
                else if (imgico is Image)
                    filesListImageList.Images.Add(imgico as Image);
            }
            catch (Exception) { }
            this.AddFileToList(sfxFile, filesListView.Items.Count, false, false);
            if (sfxFile != null)
                SetCurrentFile(sfxFile);

            currentFolder.Refresh(false, true);
            this.RefreshFilesListView(false);
        }
        private void sfxFileButton_Click(object sender, EventArgs e)
        {
            chgFile(true);
        }
        private void chgFile(bool _sfx)
        {
            if (currentFile == null)
                return;
            ResourceFileForm form = new ResourceFileForm();
            ResourceFile file = currentFile;
            ListViewItem lvi = this.FindListViewItem(filesListView, file);
            string oldFSpec = file.RealFileSpec;
            form.File = file;
            form.Albums = albums;
            form.ConnectString = this.connectString;
            form.ViewOnly = false;
            form.sfxMode = _sfx;
            form.ResourcesFolder = this.resourcesFolder;
            form.ShowDialog();            
            if (form.OK)
            {
                if (file.RealFileSpec != oldFSpec)
                {
                    SqlUtils su = new SqlUtils(this.connectString);
                    su.ExecuteQuery("UPDATE dbo.Items SET I_FileSpec = '" + file.RealFileSpec + "' WHERE I_FileSpec = '" + oldFSpec + "'");
                    su.Close();
                    RenameFilesInAllItems(oldFSpec, file.RealFileSpec);
                }
            }
            if(form.FileDeleted)
            {
                RefreshItemsListView();
                RefreshFilesListView(false);
            }
            else if ((form.OK && form.dataChanged) || form.linksChanged)
            {
                file.GetLinkedItems(this.connectString, false);
                this.RefreshFileListViewItem(lvi, file);
            }
            filesListView.Select();
            this.DisplayNOfLinks();
        }
        private void RenameFilesInAllItems(string _oldFSpec, string _newFSpec)
        {
            foreach (TreeNode node in albumsTreeView.Nodes)
                RenameFilesInItems(node, _oldFSpec, _newFSpec);
        }
        private void RenameFilesInItems(TreeNode _node, string _oldFSpec, string _newFSpec)
        {
            if (_node.Tag is ViewLink && (_node.Tag as ViewLink).Loaded)
            {
                ViewLink viewLink = _node.Tag as ViewLink;
                foreach (Item item in viewLink.View.Items)
                {
                    if (item.FileSpec == _oldFSpec)
                        item.FileSpec = _newFSpec;
                    //nie trzeba Save, bo to już zrobilismy zapytaniem
                }
            }
            foreach (TreeNode node in _node.Nodes)
                RenameFilesInItems(node, _oldFSpec, _newFSpec);
        }
        private void RunEditorOnFile(int _editorNo)
        {
            if (currentFile == null)
                return;
            this.RunEditor(_editorNo, currentFile, null, false);
        }
        private void delFilesButton_Click(object sender, EventArgs e)
        {
            this.delFiles(false);
        }
        private void delFilesNoCheckButton_Click(object sender, EventArgs e)
        {
            this.delFiles(true);
        }
        private void delFiles(bool _noCheck)
        {
            string txt = "Are you sure? OPERATION WILL PERMANENTLY DELETE SELECTED FILES AND ALL ITS SHORTCUTS AND VIEW ITEMS (LINKS).";
            if (_noCheck)
                txt += "\r\nProgram will not check if there exist any shortcuts or view items links of deleted/moved files!";
            DialogResult dr = MessageBox.Show(txt, "Warning",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            this.ClearKeyboardBuffer();
            if (dr != DialogResult.OK)
            {
                filesListView.Select();
                return;
            }
            foreach (ListViewItem lvi in filesListView.SelectedItems)
            {
                ResourceFile file = lvi.Tag as ResourceFile;
                if (!file.isShortcut)
                    this.DeleteAlbumsViewsItemsOfFile(file);
                if (file.Delete(false, this.connectString, _noCheck)) //Usuwa plik wraz ze wszystkimi ewentualnymi skrótami do niego
                    filesListView.Items.Remove(lvi);
            }
            if (filesListView.Items.Count > 0)
            {
                if (filesListView.FocusedItem != null)
                    SetCurrentFile(filesListView.FocusedItem.Tag as ResourceFile);
                else
                    SetCurrentFile(filesListView.Items[0].Tag as ResourceFile);
            }
            filesListView.Select();
            this.DisplayNOfLinks();
        }

        private void DeleteAlbumsViewsItemsOfFile(ResourceFile _file)
        {
            foreach (TreeNode albumNode in albumsTreeView.Nodes)
            {
                Album album = albumNode.Tag as Album;
                foreach (ViewLink viewLink in album.ViewsLinks)
                    this.DeleteViewItemsOfFile(viewLink, _file);
            }
            return;
        }
        private void DeleteViewItemsOfFile(ViewLink _viewLink, ResourceFile _file)
        {
            foreach (ViewLink viewLink in _viewLink.SubLinks)
                this.DeleteViewItemsOfFile(viewLink, _file);
            List<Item> itemsToDelete = new List<Item>();
            foreach (Item item in _viewLink.View.Items)
            {
                if (item.FileSpec == _file.RealFileSpec)
                    itemsToDelete.Add(item);
            }
            foreach (Item item in itemsToDelete)
            {
                if (item.Delete(false))
                {
                    if (_viewLink == currentViewLink)
                    {
                        ListViewItem lvi = FindListViewItem(itemsListView, item);
                        if (lvi != null)
                            itemsListView.Items.Remove(lvi);
                    }
                }
            }
            this.DisplayNOfLinks();
            return;
        }
        private void importShortcutButton_Click(object sender, EventArgs e)
        {
            if(!currentFile.isShortcut)
            {
                MessageBox.Show("This file is not a shortcut.");
                return;
            }
            string shortcutFileSpec = currentFile.FileSpec;
            string oldRealFileSpec = currentFile.RealFileSpec;
            string oldRealThumbSpec = utils.GetThumbSpec(oldRealFileSpec);
            string newRealFileSpec = currentFile.ParentFolder.FolderSpec + "\\" + currentFile.RealFileName;
            string newRealThumbSpec = utils.GetThumbSpec(newRealFileSpec);
            System.IO.File.Copy(currentFile.RealFileSpec, newRealFileSpec);
            bool ok = true;
            List<Item> items = currentFile.RealFile.GetLinkedItems(this.connectString, true);
            foreach (Item item in items)
            {
                item.Load();
                item.FileSpec = newRealFileSpec;
                if (!item.Save())
                {
                    ok = false;
                    break;
                }
            }
            List<string> shortcuts = currentFile.RealFile.GetShortcuts();
            foreach (string shortcut in shortcuts)
                utils.ChangeShortcutTargetFile(shortcut, newRealFileSpec);
            if (ok)
            {
                System.IO.File.Delete(shortcutFileSpec);
                System.IO.File.Delete(oldRealFileSpec);
                try { System.IO.File.Move(oldRealThumbSpec, newRealThumbSpec); }
                catch { }
                currentFile.FileSpec = newRealFileSpec;
                currentFile.isShortcut = false;
            }
            this.RefreshFilesListView(false);
            this.RefreshItemsListView();
        }

        #endregion

        #region Link

        private void linkFilesAddButton_Click(object sender, EventArgs e)
        {
            ItemQuality quality = utils.GetItemQuality(comboBoxAddingQuality.SelectedItem);
            LinkFiles(false, quality, false, false);
        }
        private void LinkFiles(bool _isImportant, ItemQuality _quality, bool _isHidden, bool _isArt)
        {
            if (currentViewLink == null || (currentFile == null && filesListView.SelectedItems.Count == 0))
                return;
            if(_quality == ItemQuality.Default)
                _quality = utils.GetItemQuality(comboBoxAddingQuality.SelectedItem);
            bool linked = false;
            int lp = this.SetInsertLp();
            if (filesListView.SelectedItems.Count != 0)
            {
                itemsListView.SelectedItems.Clear();
                foreach (ListViewItem lviF in filesListView.SelectedItems)
                {
                    ResourceFile file = lviF.Tag as ResourceFile;
                    Item item = this.LinkFileToCurrentView(file.RealFileSpec, lp,
                        _isImportant, _quality, _isHidden, _isArt);
                    if (item == null)
                        continue;
                    if (insertMode != InsertMode.ByName)
                        lp = item.Lp + 1;
                    if (item != null)
                    {
                        linked = true;
                        SetCurrentItem(item);
                        if (!autoRefresh.Checked)
                            this.AddItemToList(item, -1, false, true);
                    }
                    file.GetLinkedItems(this.connectString, false);
                    lviF.Text = this.FileDisplayName(file);
                    this.SetStyleOfFileInList(lviF);
                }
            }
            if (linked && autoRefresh.Checked)
                this.RefreshItemsListView();
            this.ReloadViewInAllAlbums(currentViewLink.View, false);

            filesListView.Select();

            this.DisplayNOfLinks();
        }
        private Item LinkFileToCurrentView(string _fSpec, int _lp,
            bool _isImportant, ItemQuality _quality, bool _isHidden, bool _isArt)
        {
            Item item = new Item(currentViewLink.View, 0, this.connectString);
            item.FileSpec = _fSpec;
            item.IsImportant = _isImportant;
            item.Quality = _quality;
            item.IsHidden = _isHidden;
            item.IsArt = _isArt;            
            bool itemExists = false;
            foreach (Item it in currentViewLink.View.Items)
            {
                if (it.FileSpec == item.FileSpec)
                {
                    itemExists = true;
                    break;
                }
            }
            if (itemExists)
            {
                MessageBox.Show("Item [" + item.FileName + "] already exists in the view");
                this.ClearKeyboardBuffer();
                return null;
            }
            if (_lp == 0)
            {
                if (!item.Save())
                    return null;
            }
            else
            {
                if (!item.Save(_lp))
                    return null;
            }

            return item;
        }

        #endregion

        #region GoTo

        private void fileGoToItemButton_Click(object sender, EventArgs e)
        {
            if (currentFile == null)
                return;
            int i = 0;
            foreach (ListViewItem lvi in itemsListView.Items)
            {
                Item item = (lvi.Tag) as Item;
                if (item == null)
                    continue;
                if (item.FileSpec == currentFile.RealFileSpec)
                {
                    itemsListView.SelectedItems.Clear();
                    itemsListView.Items[i].Selected = true;
                    itemsListView.Items[i].Focused = true;
                    itemsListView.EnsureVisible(i);
                    itemsListView.Select();
                }
                i++;
            }
        }

        #endregion

        #region Clipboard

        private void copyFilesButton_Click(object sender, EventArgs e)
        {
            this.CopyCutFiles(CopyCutMode.Copy);
        }
        private void cutFilesButton_Click(object sender, EventArgs e)
        {
            this.CopyCutFiles(CopyCutMode.Cut);
        }
        private void CopyCutFiles(CopyCutMode _mode)
        {
            clipboardFiles.Clear();
            foreach (ListViewItem lvi in filesListView.SelectedItems)
            {
                ResourceFile file = lvi.Tag as ResourceFile;
                clipboardFiles.Add(file);
            }
            CopyCutFilesMode = _mode;
            CopyCutLastFrom = CopyCutLastFrom.Files;
            this.FilesShowClipboardMode(_mode);
        }
        private void pasteFilesButton_Click(object sender, EventArgs e)
        {
            if (this.CopyCutFilesMode == CopyCutMode.None)
                return;
            filesListView.SelectedItems.Clear();
            if (CopyCutFilesMode == CopyCutMode.Copy)
            {
                foreach (ResourceFile file in clipboardFiles)
                {
                    string newFileSpec = currentFolder.FolderSpec + "\\" + file.FileName;
                    file.Copy(file.FileSpec, newFileSpec);
                    ResourceFile fileAdded = new ResourceFile(currentFolder, file.FileName, this.resourcesFolder);
                    this.AddFileToList(fileAdded, -1, false, true);
                    string shortcutFileSpec = utils.MakeShortcutFileSpec(newFileSpec);
                    if (System.IO.File.Exists(shortcutFileSpec))
                        System.IO.File.Delete(shortcutFileSpec);
                }
            }
            else if (CopyCutFilesMode == CopyCutMode.Cut)
            {
                filesCopyCutLabel.Text = "";
                if (clipboardFiles.Count == 0)
                    return;
                List<ResourceFile> FilesToMove = new List<ResourceFile>();
                foreach (ResourceFile file in clipboardFiles)
                    FilesToMove.Add(file);
                foreach (ResourceFile file in FilesToMove)
                {
                    string newFileSpec = currentFolder.FolderSpec + "\\" + file.FileName;
                    file.Move(newFileSpec, this.connectString, false);
                    ResourceFile fileAdded = new ResourceFile(currentFolder, file.FileName, this.resourcesFolder);
                    this.AddFileToList(fileAdded, -1, false, true);
                    string shortcutFileSpec = utils.MakeShortcutFileSpec(newFileSpec);
                    if (System.IO.File.Exists(shortcutFileSpec))
                        System.IO.File.Delete(shortcutFileSpec);
                }
            }
            //RefreshItemsListView();
            if (autoRefresh.Checked)
                RefreshFilesListView(false);
            filesListView.Select();
        }
        private void pasteShortcutsButton_Click(object sender, EventArgs e)
        {
            if (this.CopyCutFilesMode == CopyCutMode.Cut)
            {
                System.Windows.Forms.MessageBox.Show("Invalid copy/cut mode.");
                return;
            }
            foreach (ResourceFile file in clipboardFiles)
            {
                string newFileSpec = currentFolder.FolderSpec + "\\" + file.FileName;
                string shortcutFileSpec = utils.MakeShortcutFileSpec(newFileSpec);
                utils.CreateShortcut(shortcutFileSpec, file.FileSpec);
                ResourceFile fileAdded = new ResourceFile(currentFolder, System.IO.Path.GetFileName(shortcutFileSpec), this.resourcesFolder);
                this.AddFileToList(fileAdded, filesListImageList.Images.Count - 1, false, true);
            }
            //RefreshItemsListView();
            if (autoRefresh.Checked)
                RefreshFilesListView(false);
            filesListView.Select();
        }
        private void pasteFilesWithLeaveShortcutsButton_Click(object sender, EventArgs e)
        {
            if (this.CopyCutFilesMode == CopyCutMode.Copy)
            {
                System.Windows.Forms.MessageBox.Show("Invalid copy/cut mode.");
                return;
            }
            filesListView.SelectedItems.Clear();
            foreach (ResourceFile file in clipboardFiles)
            {
                string oldFileSpec = file.FileSpec;
                string newFileSpec = currentFolder.FolderSpec + "\\" + file.FileName;
                file.Move(newFileSpec, this.connectString, false);
                ResourceFile fileAdded = new ResourceFile(currentFolder, file.FileName, this.resourcesFolder);
                this.AddFileToList(fileAdded, -1, false, true);
                string shortcutFileSpec = utils.MakeShortcutFileSpec(newFileSpec);
                if (System.IO.File.Exists(shortcutFileSpec))
                    System.IO.File.Delete(shortcutFileSpec);

                shortcutFileSpec = utils.MakeShortcutFileSpec(oldFileSpec);
                utils.CreateShortcut(shortcutFileSpec, newFileSpec);
            }
            //RefreshItemsListView();
            if (autoRefresh.Checked)
                RefreshFilesListView(false);
            filesListView.Select();
        }
        private void FilesShowClipboardMode(CopyCutMode _mode)
        {
            if (clipboardFiles.Count > 0)
            {
                filesCopyCutLabel.Text = (_mode == CopyCutMode.Copy ? "C" : "X") + clipboardFiles.Count;
                if (filesCopyCutLabel.ForeColor == System.Drawing.Color.Blue)
                    filesCopyCutLabel.ForeColor = System.Drawing.Color.Red;
                else
                    filesCopyCutLabel.ForeColor = System.Drawing.Color.Blue;
            }
            else
                filesCopyCutLabel.Text = "";
        }

        #endregion

        #region Play

        private void filesListView_Enter(object sender, EventArgs e)
        {
            this.FilesListSelected();
        }
        private void FilesListSelected()
        {
            this.Play(false);
        }

        private void playFilesButton_Click(object sender, EventArgs e)
        {
            if (currentFile == null)
                return;

            List<PlayableObject> objectsToPlay = new List<PlayableObject>();
            int currentNdx = -1;
            int i = 0;
            foreach (ResourceFile file in currentFolder.Files)
            {
                objectsToPlay.Add(new PlayableObject(file.RealFileSpec, ItemQuality.Normal));
                if (file.RealFileSpec == currentFile.RealFileSpec)
                    currentNdx = i;
                i++;
            }
            AVPlayerBox.close();
            PlayerForm fsp = new PlayerForm(this.initialFullScreenZoomFactorCoeff, this.initialFullScreenMoveDelta, uxShowTextNotes.Checked);
            fsp.objectsToPlay = objectsToPlay;
            fsp.startNdx = currentNdx;
            fsp.ShowDialog();
            filesListView.Select();
        }

        private void saveTextButton_Click(object sender, EventArgs e)
        {
            string fSpec = String.Empty;
            if (playViewMode)
            {
                if (currentItem != null)
                    fSpec = currentItem.FileSpec;
            }
            else
            {
                if (currentFile != null)
                    fSpec = currentFile.RealFileSpec;
            }
            if (String.IsNullOrEmpty(fSpec))
                return;
            System.IO.File.WriteAllText(fSpec, textBox.Text, Encoding.GetEncoding(1250));
            System.Windows.Forms.MessageBox.Show("File saved.");
        }

        #endregion

        #endregion


        #region Play

        private void pictureBox_DoubleClick(object sender, EventArgs e)
        {
            if (playViewMode)
                playItemsButton.PerformClick();
            else
                playFilesButton.PerformClick();
        }

        private void textBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
                saveTextButton_Click(sender, e);
        }

        #endregion

        #endregion


        #region Utils

        private TreeNode FindNode(TreeNodeCollection _nodesColl, object _tag)
        {
            //String tagTypeName = (tag.GetType()).Name;
            TreeNode retVal = null;
            foreach (TreeNode node in _nodesColl)
            {
                if (node.Tag == null)
                    continue;
                if (_tag is Album)
                {
                    if (((Album)(node.Tag)).ID == ((Album)_tag).ID)
                    {
                        retVal = node;
                        break;
                    }
                    continue;
                }
                else if (_tag is ViewLink)
                {
                    if (node.Tag is ViewLink)
                    {
                        if ((node.Tag as ViewLink).ID == (_tag as ViewLink).ID)
                        {
                            retVal = node;
                            return retVal;
                        }
                    }
                }
                else if (_tag is ResourceFolder)
                {
                    if (node.Tag is ResourceFolder && ((ResourceFolder)node.Tag).FolderSpec == ((ResourceFolder)_tag).FolderSpec)
                    {
                        retVal = node;
                        return retVal;
                    }
                }
                retVal = this.FindNode(node.Nodes, _tag);
                if (retVal != null)
                    break;
            }
            return retVal;
        }

        private ListViewItem FindListViewItem(ListView _listView, object _tag)
        {
            ListViewItem retVal = null;
            if (_listView.Items == null || _listView.Items.Count == 0)
                return null;
            foreach (ListViewItem lvi in _listView.Items)
            {
                if (lvi.Tag == _tag)
                {
                    retVal = lvi;
                    break;
                }
            }
            return retVal;
        }
        private int FindListViewItemNdx(ListView _listView, object _tag)
        {
            int retVal = -1;
            if (_listView.Items == null || _listView.Items.Count == 0)
                return retVal;
            retVal = 0;
            foreach (ListViewItem lvi in _listView.Items)
            {
                if (lvi.Tag == _tag)
                    break;
                retVal++;
            }
            return retVal;
        }

        private void Play(bool _viewMode)
        {            
            string fSpec = String.Empty;
            ContentType contentType = ContentType.Unknown;
            if (_viewMode)
            {
                if (currentItem != null)
                {
                    fSpec = currentItem.FileSpec;
                    contentType = currentItem.ContentType;
                }
            }
            else
            {
                if (currentFile != null)
                {
                    fSpec = currentFile.RealFileSpec;
                    contentType = currentFile.ContentType;
                }
            }
            if (String.IsNullOrEmpty(fSpec))
            {
                contentType = ContentType.Unknown;
                textBox.Visible = false;
                pictureBox.Visible = false;
                pictureBox.ImageLocation = fSpec;
                AVPlayerBox.Visible = false;
                AVPlayerBox.close();
                playFileNameLabel.Text = String.Empty;
                saveTextButton.Visible = false;
                descriptionBubble.Hide();
                return;
            }
            playViewMode = _viewMode;
            playFileNameLabel.Text = System.IO.Path.GetFileName(fSpec) + " (" + utils.FileSizeDisplay(fSpec) + ")";
            if(contentType == ContentType.Picture)
            {
                textBox.Visible = false;
                //if (System.IO.File.Exists(fSpec))
                {
                    pictureBox.Visible = true;
                    if (pictureBox.Image != null)
                    {
                        pictureBox.Image.Dispose();
                        pictureBox.Image = null;
                    }
                    pictureBox.ImageLocation = null;
                    string imgSpec = System.IO.File.Exists(fSpec) ? fSpec : this.GetArcFSpec(fSpec);
                    pictureBox.Image = utils.GetImageFromFile(imgSpec);
                }
                AVPlayerBox.Visible = false;
                AVPlayerBox.close();
                saveTextButton.Visible = false;
                ItemProps itemProps = utils.ReadSidecarProps(fSpec);
                if (itemProps != null && !string.IsNullOrEmpty(itemProps.Description))
                {
                    descriptionBubble.ShowFor(fSpec, itemProps.Description, this, pictureBox);
                }
                else
                {
                    descriptionBubble.Hide();
                }
                filesListView.Focus();
            }
            else if(contentType == ContentType.Audio || contentType == ContentType.Video)
            {
                textBox.Visible = false;
                pictureBox.Visible = false;
                //if (System.IO.File.Exists(fSpec))
                {
                    AVPlayerBox.Visible = true;
                    if (System.IO.File.Exists(fSpec))
                        AVPlayerBox.URL = fSpec;
                    else
                        AVPlayerBox.URL = this.GetArcFSpec(fSpec);
                }
                if (!playAVCheck.Checked)
                    AVPlayerBox.close();
                saveTextButton.Visible = false;
                ItemProps itemProps = utils.ReadSidecarProps(fSpec);
                if (itemProps != null && !string.IsNullOrEmpty(itemProps.Description))
                {
                    descriptionBubble.ShowFor(fSpec, itemProps.Description, this, pictureBox);
                }
                else
                {
                    descriptionBubble.Hide();
                }
                filesListView.Focus();
            }
            else if (contentType == ContentType.Text)
            {
                textBox.Visible = true;
                if (System.IO.File.Exists(fSpec))
                {
                    if (System.IO.File.Exists(fSpec))
                        textBox.Text = System.IO.File.ReadAllText(fSpec, Encoding.GetEncoding(1250));
                    else
                        textBox.Text = System.IO.File.ReadAllText(this.GetArcFSpec(fSpec), Encoding.GetEncoding(1250));
                    saveTextButton.Visible = true;
                }
                pictureBox.Visible = false;
                AVPlayerBox.Visible = false;
                AVPlayerBox.close();
                filesListView.Focus();
            }
            else
            {
                textBox.Visible = false;
                pictureBox.Visible = false;
                AVPlayerBox.Visible = false;
                saveTextButton.Visible = false;
            }
        }
        private string GetArcFSpec(string _fSpec)
        {
            string retVal = String.Empty;
            if (!String.IsNullOrEmpty(this.arcFolder))
                retVal = _fSpec.Replace(this.resourcesFolder, this.arcFolder);
            return retVal;
        }
        public bool ThumbnailCallback()
        {
            return false;
        }

        private object GetThumbnail(string _fSpec, ContentType _contentType)
        {
            //Image.GetThumbnailImageAbort myCallback = new Image.GetThumbnailImageAbort(ThumbnailCallback);
            //Bitmap myBitmap = new Bitmap(item.FileSpec);
            //Image myThumbnail = myBitmap.GetThumbnailImage(120, 120, myCallback, IntPtr.Zero);                    
            //itemsListImageList.Images.Add(myThumbnail);
            object retVal = null;
            if (_contentType == ContentType.Picture)
            {
                if (displayPictureThumbnailsCheck.Checked && thumbnailSizeSpin.Value > 0)
                {
                    string thumbSpec = utils.GetThumbSpec(_fSpec);
                    try
                    {
                        if (System.IO.File.Exists(thumbSpec))
                        {
                            Image image = utils.GetImageFromFile(thumbSpec);
                            if (image.Size.Width != thumbnailSize)
                            {
                                image = utils.GetImageFromFile(_fSpec).GetThumbnailImage(thumbnailSize, thumbnailSize, myCallback, IntPtr.Zero);
                                utils.SaveThumb(image, thumbSpec);
                            }
                            retVal = image;                            
                        }
                        else
                        {
                            Image srcImage = utils.GetImageFromFile(_fSpec);
                            retVal = srcImage.GetThumbnailImage(thumbnailSize, thumbnailSize, myCallback, IntPtr.Zero);
                            utils.SaveThumb((retVal as Image), thumbSpec);
                        }
                    }
                    catch (Exception)
                    {
                        retVal = MultiMediaCenter.Properties.Resources.Cancel;
                    }
                }
                else
                    retVal = MultiMediaCenter.Properties.Resources.Photo;
            }
            else if (_contentType == ContentType.Audio)
                retVal = MultiMediaCenter.Properties.Resources.Audio;
            else if (_contentType == ContentType.Video)
                retVal = MultiMediaCenter.Properties.Resources.Video;
            else if (_contentType == ContentType.Text)
                retVal = MultiMediaCenter.Properties.Resources.Document;
            else if (_contentType == ContentType.Unknown)
                retVal = MultiMediaCenter.Properties.Resources.ContentUnknownThumbnail;
            return retVal;
        }

        private void AddItemToList(Item _item, int _imgNdx, bool _setCurrent, bool _setSelectedAndEnsureVisible)
        {
            if (_imgNdx == -1)
            {
                this.AddImageToItemsImageList(_item);
                _imgNdx = itemsListImageList.Images.Count - 1;
            }
            ListViewItem lvi = new ListViewItem();
            this.SetListItem(lvi, _item, _imgNdx);
            itemsListView.Items.Add(lvi);
            if (_setCurrent || currentItem == null)
                SetCurrentItem(_item);
            if (_setSelectedAndEnsureVisible)
            {
                int i = 0;
                foreach (ListViewItem lvi1 in itemsListView.Items)
                {
                    if(lvi1 == lvi)
                    {
                        lvi1.Selected = true;
                        lvi1.Focused = true;
                        itemsListView.EnsureVisible(i);
                        break;
                    }
                    i++;
                }
            }
            return;
        }
        private void SetListItem(ListViewItem _lvi)
        {
            this.SetListItem(_lvi, _lvi.Tag as Item);
        }
        private void SetListItem(ListViewItem _lvi, Item _item)
        {
            this.SetListItem(_lvi, _item, -1);
        }
        private void SetListItem(ListViewItem _lvi, Item _item, int _imgNdx)
        {
            _lvi.Tag = _item;
            this.SetListItemText(_lvi);
            this.SetListItemTooltip(_lvi);
            this.SetListItemStyle(_lvi);
            this.SetListItemIcon(_lvi, _imgNdx);
        }
        private void SetListItemText(ListViewItem _lvi)
        {
            Item item = _lvi.Tag as Item;
            _lvi.Text = utils.ItemStateSign(item, true) + item.FileName;
        }
        private void SetListItemTooltip(ListViewItem _lvi)
        {
            Item item = _lvi.Tag as Item;
            _lvi.ToolTipText = this.FileTooltip(item.FileSpec);
        }
        private void SetListItemStyle(ListViewItem _lvi)
        {
            Item item = _lvi.Tag as Item;
            if (!this.ItemFileExists(item))
                _lvi.ForeColor = Color.Red;
            else
                _lvi.ForeColor = Color.Black;
            if (item.IsImportant)
                _lvi.Font = new Font(_lvi.Font.FontFamily.Name, importantFontSize, FontStyle.Bold);
            else
                _lvi.Font = new Font(_lvi.Font.FontFamily.Name, normalFontSize, FontStyle.Regular);
        }
        private void SetListItemIcon(ListViewItem _lvi, int _imgNdx)
        {
            if (_imgNdx >= 0)
                _lvi.ImageIndex = _imgNdx;
        }
        private void AddImageToItemsImageList(Item _item)
        {
            try
            {
                object imgico = this.GetThumbnail(_item.FileSpec, _item.ContentType);
                if (imgico is Icon)
                    itemsListImageList.Images.Add(imgico as Icon);
                else if (imgico is Image)
                    itemsListImageList.Images.Add(imgico as Image);
            }
            catch
            {
            }
        }
        private bool ItemFileExists(Item _item)
        {
            return System.IO.File.Exists(_item.FileSpec);
        }

        private void AddFileToList(ResourceFile _file, int _imgNdx, bool _setCurrent, bool _setSelectedAndEnsureVisible)
        {
            if (_imgNdx == -1)
            {
                this.AddImageToFilesImageList(_file);
                _imgNdx = filesListImageList.Images.Count - 1;
            }
            int linksCount = _file.GetLinksCount(this.connectString);
            if (hideLinkedCheck.Checked && linksCount > 0)
                return;
            ListViewItem lvi = new ListViewItem();
            this.RefreshFileListViewItem(lvi, _file);
            lvi.ImageIndex = _imgNdx;
            filesListView.Items.Add(lvi);
            if (_setCurrent || currentFile == null)
                SetCurrentFile(_file);
            if (_setSelectedAndEnsureVisible)
            {
                int i = 0;
                foreach (ListViewItem lvi1 in filesListView.Items)
                {
                    if (lvi1 == lvi)
                    {
                        lvi1.Selected = true;
                        lvi1.Focused = true;
                        filesListView.EnsureVisible(i);
                        break;
                    }
                    i++;
                }
            }
        }
        private void RefreshFileListViewItem(ListViewItem _lvi, ResourceFile _file)
        {
            _lvi.Tag = _file;
            _lvi.Text = this.FileDisplayName(_file);            
            _lvi.ToolTipText = this.FileTooltip(_file.isShortcut ? _file.FileSpec + " -> " + _file.RealFileSpec : _file.FileSpec);
            this.SetStyleOfFileInList(_lvi);
        }
        private string FileDisplayName(ResourceFile _file)
        {
            if(_file.linkedItemsList == null)
                _file.GetLinkedItems(this.connectString, false);
            return _file.GetDisplayName();
            /*
            string retVal = String.Empty;
            if (_linksCount == -1)
                retVal = "(?)" + _file.FileName;
            else if (_linksCount == 0)
                retVal = _file.FileName;
            else if (_linksCount > 0)
                retVal = "(" + _linksCount + ")" + _file.FileName;
            return retVal;
            */ 
        }
        private string FileTooltip(string _fileSpec)
        {
            return _fileSpec;
            /*
            double fSize = 0;
            try
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(_fileSpec);
                fSize = fi.Length / 1024.00 / 1024.00;
            }
            catch { }
            return _fileSpec + " (" + fSize.ToString("#.##") + "MB)";
            */ 
        }
        private void SetStyleOfFileInList(ListViewItem _lvi)
        {
            ResourceFile rf = _lvi.Tag as ResourceFile;
            if (rf.isShortcut)
                _lvi.Font = new Font(_lvi.Font.FontFamily.Name, shortcutFontSize, FontStyle.Italic);
            else
                _lvi.Font = new Font(_lvi.Font.FontFamily.Name, normalFontSize, FontStyle.Regular);
            int linksCount = rf.GetLinksCount(this.connectString);
            if (linksCount == -1)
                _lvi.ForeColor = Color.Green;
            else if (linksCount == 0)
                _lvi.ForeColor = Color.Blue;
            else
                _lvi.ForeColor = Color.Black;
            ItemProps itemProps = utils.ReadSidecarProps(rf.FileSpec);
            if (itemProps != null)
            {
                if (itemProps.Value == ItemValue.High)
                {
                    _lvi.Font = new Font(_lvi.Font.FontFamily.Name, bigFontSize, FontStyle.Bold);
                }
                else if (itemProps.Value == ItemValue.Low)
                {
                    _lvi.Font = new Font(_lvi.Font.FontFamily.Name, smallFontSize, FontStyle.Regular);
                }
            }
        }
        private void AddImageToFilesImageList(ResourceFile _file)
        {
            try
            {
                object imgico = this.GetThumbnail(_file.FileSpec, _file.ContentType);
                if (imgico is Icon)
                    filesListImageList.Images.Add(imgico as Icon);
                else if (imgico is Image)
                    filesListImageList.Images.Add(imgico as Image);
            }
            catch
            {
            }
        }
        private bool FileLinkedInView(ResourceFile _file, ViewLink _viewLink)
        {
            foreach (ViewLink viewLink in _viewLink.SubLinks)
            {
                if (this.FileLinkedInView(_file, viewLink))
                    return true;
            }
            foreach (Item item in _viewLink.View.Items)
            {
                if (item.FileSpec == _file.RealFileSpec)
                    return true;
            }
            return false;
        }

        private bool RunEditor(int _editorNo, ResourceFile _file, string _fSpec, bool _recursive)
        {
            bool fileChanged = false;
            string editorSpec = String.Empty;
            ContentType contentType;
            if (_file != null)
            {
                _fSpec = _file.RealFileSpec;
                contentType = _file.ContentType;
            }
            else
                contentType = utils.ComputeContentType(_fSpec);
            string workingFolder = String.Empty;
            switch (contentType)
            {
                case ContentType.Text:
                    {
                        editorSpec = (_editorNo == 1 ? TextEditorFSpec1 : TextEditorFSpec2);
                        workingFolder = TextEditorsWorkingFolder;
                        break;
                    }
                case ContentType.Picture:
                    {
                        switch (_editorNo)
                        {
                            case 1:
                                {
                                    editorSpec = PhotoEditorFSpec1;
                                    break;
                                }
                            case 2:
                                {
                                    editorSpec = PhotoEditorFSpec2;
                                    break;
                                }
                            case 3:
                                {
                                    editorSpec = PhotoEditorFSpec3;
                                    break;
                                }
                        }
                        workingFolder = PhotoEditorsWorkingFolder;
                        break;
                    }
                case ContentType.Audio:
                    {
                        editorSpec = (_editorNo == 1 ? AudioEditorFSpec1 : AudioEditorFSpec2);
                        workingFolder = AudioEditorsWorkingFolder;
                        break;
                    }
                case ContentType.Video:
                    {
                        editorSpec = (_editorNo == 1 ? VideoEditorFSpec1 : VideoEditorFSpec2);
                        workingFolder = VideoEditorsWorkingFolder;
                        break;
                    }
            }
            if (String.IsNullOrEmpty(editorSpec))
            {
                System.Windows.Forms.MessageBox.Show("No " + (_editorNo == 1 ? "primary" : "secondary") + " editor defined for this type of file.");
                return false;
            }
            System.IO.FileInfo fiOrg = new System.IO.FileInfo(_fSpec);
            string workFileSpec = _fSpec;
            if (!_recursive && !String.IsNullOrEmpty(workingFolder))
            {
                workFileSpec = workingFolder + "\\" + System.IO.Path.GetFileName(_fSpec);
                System.IO.File.Copy(_fSpec, workFileSpec, true);
            }            
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(editorSpec, "\"" + workFileSpec + "\"");
            p.StartInfo = psi;
            try
            {
                p.Start();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                return false;
            }
            p.WaitForExit();
            p.Close();

            System.IO.FileInfo fiTrg = new System.IO.FileInfo(workFileSpec);
            fileChanged = fiTrg.Length != fiOrg.Length;

            if (_recursive)
                return fileChanged;

            if (contentType == ContentType.Picture)
            {
                if (_editorNo == 1 && !String.IsNullOrEmpty(PhotoEditorFSpec2))
                {
                    bool fileChanged2 = this.RunEditor(2, null, workFileSpec, true);
                    fileChanged = (fileChanged || fileChanged2);
                }
            }
            if (fileChanged)
            {
                System.IO.File.Copy(workFileSpec, _fSpec, true);
                Image image = utils.SaveThumb(_fSpec);
                if (_file != null)
                {
                    _file.Thumbnail = image;
                    this.RefreshFilesListView(false);
                }
                else
                    this.RefreshItemsListView();
            }
            System.IO.File.Delete(workFileSpec);

            return fileChanged;
        }

        private void ClearKeyboardBuffer()
        {
            this.Enabled = false;
            Application.DoEvents();
            this.Enabled = true;
        }

        #region Backup

        private void backupDatabaseButton_Click(object sender, EventArgs e)
        {
            SqlUtils su = new SqlUtils(this.connectString);
            DateTime dt = DateTime.Now;
            if (!System.IO.Directory.Exists(this.backupFolder))
                System.IO.Directory.CreateDirectory(this.backupFolder);
            string currentBackupFolder = this.backupFolder + "\\" + dt.Year + "-" + dt.Month.ToString("00") + "-" + dt.Day.ToString("00") + " " +
                dt.Hour.ToString("00") + "." + dt.Minute.ToString("00") + "." + dt.Second.ToString("00");
            System.IO.Directory.CreateDirectory(currentBackupFolder);
            string fSpec = currentBackupFolder + "\\db.BAK";
            su.ExecuteQuery("BACKUP DATABASE MultiMedia TO DISK='" + fSpec + "'");
            this.SaveAlbumsTree(fSpec);
            MessageBox.Show("Full backup of database successfully created and saved in file " + fSpec + ".\r\n" +
                "Restore script db.SQL and albums views tree file db.TRE saved in the same directory.");
        }

        private string albumsTreeContent;
        private void SaveAlbumsTree(string _fSpec)
        {
            string sqlContent = String.Empty;

            sqlContent = "DELETE dbo.Albums;\r\nDELETE dbo.ViewsLinks;\r\nDELETE dbo.Views;\r\nDELETE dbo.Items;\r\n\r\n";

            SqlUtils su = new SqlUtils(this.connectString);
            SqlDataReader rd;

            rd = su.GetSqlReader("SELECT * FROM dbo.Albums ORDER BY A_ID");
            sqlContent += "SET IDENTITY_INSERT dbo.Albums ON;\r\n";
            while (rd.Read())
            {
                int id = Convert.ToInt32(rd["A_ID"]);
                string name = Convert.ToString(rd["A_Name"]);
                int lp = Convert.ToInt32(rd["A_Lp"]);
                sqlContent += "INSERT INTO dbo.Albums (A_ID,A_Name,A_Lp) VALUES(" + id + ",'" + name + "'," + lp + ");\r\n";
            }
            sqlContent += "SET IDENTITY_INSERT dbo.Albums OFF;\r\n";
            rd.Close();

            rd = su.GetSqlReader("SELECT * FROM dbo.Views ORDER BY V_ID");
            sqlContent += "\r\nSET IDENTITY_INSERT dbo.Views ON;\r\n";
            while (rd.Read())
            {
                int id = Convert.ToInt32(rd["V_ID"]);
                string name = Convert.ToString(rd["V_Name"]);
                int hidden = Convert.ToInt32(rd["V_IsHidden"]);
                sqlContent += "INSERT INTO dbo.Views (V_ID,V_Name,V_IsHidden) VALUES(" + id + ",'" + name + "'," + hidden + ");\r\n";
            }
            sqlContent += "SET IDENTITY_INSERT dbo.Views OFF;\r\n";
            rd.Close();

            sqlContent += "\r\n";
            rd = su.GetSqlReader("SELECT * FROM dbo.ViewsLinks ORDER BY VL_ID");
            //sqlContent += "SET IDENTITY_INSERT dbo.ViewsLinks ON;\r\n";
            while (rd.Read())
            {
                int id = Convert.ToInt32(rd["VL_ID"]);
                int parentAID = Convert.ToInt32(rd["VL_ParentAID"]);
                string ParentVID;
                ParentVID = Convert.ToString(rd["VL_ParentVID"]);
                if (ParentVID == String.Empty)
                    ParentVID = "null";
                int vID = Convert.ToInt32(rd["VL_VID"]);
                string name = Convert.ToString(rd["VL_Name"]);
                int lp = Convert.ToInt32(rd["VL_Lp"]);
                sqlContent += "INSERT INTO dbo.ViewsLinks (VL_ParentAID,VL_ParentVID,VL_VID,VL_Name,VL_Lp) " +
                    "VALUES(" + parentAID + "," + ParentVID + "," + vID + ",'" + name + "'," + lp + ");\r\n";
            }
            //sqlContent += "SET IDENTITY_INSERT dbo.ViewsLinks OFF; ";
            rd.Close();

            sqlContent += "\r\n";
            rd = su.GetSqlReader("SELECT * FROM dbo.Items ORDER BY I_ID");
            //sqlContent += "SET IDENTITY_INSERT dbo.Items ON;\r\n";
            while (rd.Read())
            {
                //int id = Convert.ToInt32(rd["I_ID"]);
                int vID = Convert.ToInt32(rd["I_VID"]);
                string fileSpec = Convert.ToString(rd["I_FileSpec"]);
                string fileName = Convert.ToString(rd["I_FileName"]);
                int lp = Convert.ToInt32(rd["I_Lp"]);
                int isImportant = Convert.ToInt32(rd["I_IsImportant"]);
                int quality = Convert.ToInt32(rd["I_Quality"]);
                int isArt = Convert.ToInt32(rd["I_IsArt"]);
                int isHidden = Convert.ToInt32(rd["I_IsHidden"]);

                sqlContent += "INSERT INTO dbo.Items (I_VID,I_FileSpec,I_FileName,I_Lp,I_IsImportant,I_Quality,I_IsArt,I_IsHidden) " +
                    "VALUES(" + vID + ", '" + fileSpec + "','" + fileName + "'," + lp + "," + isImportant + "," + quality + "," + isArt + "," + isHidden + ");\r\n";
            }
            //sqlContent += "SET IDENTITY_INSERT dbo.Items OFF; ";
            rd.Close();

            su.Close();

            string fSpec = System.IO.Path.ChangeExtension(_fSpec, ".SQL");
            System.IO.File.WriteAllText(fSpec, sqlContent, Encoding.GetEncoding(1250));

            //System.Windows.Forms.MessageBox.Show("Saved file " + fSpec);

            albumsTreeContent = String.Empty;
            foreach (TreeNode node in albumsTreeView.Nodes)
                this.SaveAlbumsTreeNode(node, 0);

            fSpec = System.IO.Path.ChangeExtension(_fSpec, ".TRE");
            System.IO.File.WriteAllText(fSpec, albumsTreeContent, Encoding.GetEncoding(1250));

            //System.Windows.Forms.MessageBox.Show("Saved file " + fSpec);
        }
        private void SaveAlbumsTreeNode(TreeNode _node, int _level)
        {
            if (!String.IsNullOrEmpty(albumsTreeContent))
                albumsTreeContent += "\r\n";
            for (int i = 1; i <= _level; i++)
                albumsTreeContent += "  ";
            albumsTreeContent += _node.Text;
            foreach (TreeNode node in _node.Nodes)
                this.SaveAlbumsTreeNode(node, _level + 1);
        }

        private void restoreDatabaseButton_Click(object sender, EventArgs e)
        {
            string createDatabaseFileSpec = this.backupFolder + "\\MultiMedia-Create.sql";
            if (!System.IO.File.Exists(createDatabaseFileSpec))
            {
                MessageBox.Show("No MultiMedia-Create.SQL file in backup directory " + this.backupFolder + ".");
                return;
            }
            string createDataBaseContent = System.IO.File.ReadAllText(this.backupFolder + "\\MultiMedia-Create.sql", Encoding.GetEncoding(1250));

            List<string> bakDirs = new List<string>();
            string[] dirs = System.IO.Directory.GetDirectories(this.backupFolder);
            foreach (string dir in dirs)
            {
                //if (System.IO.Path.GetExtension(file).ToUpper() == ".BAK")
                string dName = System.IO.Path.GetFileName(dir);
                if (dName.Substring(0, 1) != "2" || dName.Length < "2012-01-01".Length || dName.Substring(4, 1) != "-" || dName.Substring(7, 1) != "-")
                    continue;
                bakDirs.Add(dir);
            }
            bakDirs.Sort();
            string dSpec = String.Empty;
            if (bakDirs.Count > 0)
                dSpec = bakDirs[bakDirs.Count - 1];
            if (String.IsNullOrEmpty(dSpec))
            {
                MessageBox.Show("No one-backup directories in " + this.backupFolder + " directory.");
                return;
            }
            string loadDatabaseFileSpec = dSpec + "\\DB.SQL";
            if (!System.IO.File.Exists(loadDatabaseFileSpec))
            {
                MessageBox.Show("No backup script file (DB.SQL) in latest one-backup directory " + this.backupFolder + ".");
                return;
            }

            //if (MessageBox.Show("ARE YOU SURE?\r\nOPERATION WILL RESTORE DATABASE FROM THE NEWEST .BAK FILE.", "Warning", 
            if (MessageBox.Show("ARE YOU SURE?\r\nOPERATION WILL RESTORE DATABASE FROM THE NEWEST BACKUP SCRIPT FILE " + loadDatabaseFileSpec + ".", "Warning",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                return;

            string loadDataBaseContent = System.IO.File.ReadAllText(loadDatabaseFileSpec, Encoding.GetEncoding(1250));

            SqlUtils su = new SqlUtils(this.connectString);
            try
            {
                //su.ExecuteQuery("RESTORE DATABASE MultiMedia FROM DISK='" + fSpec + "'");
                createDataBaseContent = createDataBaseContent.Replace("\r\nGO", "|");
                string[] commands = createDataBaseContent.Split('|');
                if (commands.Length > 0)
                {
                    su.ExecuteQuery("BEGIN TRAN");
                    foreach (string command in commands)
                    {
                        su.ExecuteQuery(command);
                    }
                }
                else
                {
                    MessageBox.Show("No SQL batches in " + createDatabaseFileSpec + ". Exiting...");
                    return;
                }
            }
            catch (Exception ex)
            {
                su.ExecuteQuery("ROLLBACK TRAN");
                MessageBox.Show("Error (re-)creating database. Try it in Managment Studio.\r\n" + ex.Message);
                return;
            }
            try
            {
                //su.ExecuteQuery("RESTORE DATABASE MultiMedia FROM DISK='" + fSpec + "'");
                su.ExecuteQuery(loadDataBaseContent);
                su.ExecuteQuery("COMMIT");
                MessageBox.Show("Database restored successfully from file " + loadDatabaseFileSpec + ".");                
            }
            catch (Exception ex)
            {
                su.ExecuteQuery("ROLLBACK TRAN");
                MessageBox.Show("Error loading database from file . " + loadDatabaseFileSpec + ". Try it in Managment Studio.\r\n" + ex.Message);
            }
            su.Close();
            this.LoadAll();
        }

        /*
        protected void GoButton_Click(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;
            if (!System.IO.Directory.Exists(this.backupFolder))
                System.IO.Directory.CreateDirectory(this.backupFolder);
            string currentBackupFolder = this.backupFolder + "\\" + dt.Year + "-" + dt.Month.ToString("00") + "-" + dt.Day.ToString("00") + " " +
                dt.Hour.ToString("00") + "." + dt.Minute.ToString("00") + "." + dt.Second.ToString("00");
            System.IO.Directory.CreateDirectory(currentBackupFolder);
            //this.BackupBAK(currentBackupFolder);
            this.BackupSQL(currentBackupFolder);
            Response.Write("Full backup of database successfully created and saved in folder " + currentBackupFolder + ".\r\n" +
                "Restore script db.SQL and albums views tree file db.TRE saved in the same directory.");
        }

        protected void BackupBAK(string _currentBackupFolder)
        {
            SqlUtils sqlUtils = new SqlUtils();
            DateTime dt = DateTime.Now;
            if (!System.IO.Directory.Exists(this.backupFolder))
                System.IO.Directory.CreateDirectory(this.backupFolder);
            string currentBackupFolder = this.backupFolder + "\\" + dt.Year + "-" + dt.Month.ToString("00") + "-" + dt.Day.ToString("00") + " " +
                dt.Hour.ToString("00") + "." + dt.Minute.ToString("00") + "." + dt.Second.ToString("00");
            System.IO.Directory.CreateDirectory(_currentBackupFolder);
            string fSpec = currentBackupFolder + "\\db.BAK";
            sqlUtils.ExecuteQuery("BACKUP DATABASE ETC TO DISK='" + fSpec + "'");
        }

        protected void BackupSQL(string _currentBackupFolder)
        {
            string sqlContent = String.Empty;

            sqlContent = "DELETE dbo.Trees\r\nDELETE dbo.Genres\r\nDELETE dbo.Countries\r\nDELETE dbo.Continents\r\nDELETE dbo.Users\r\n" +
                "DELETE dbo.Config\r\nDELETE dbo.ConfigImages\r\n\r\n";

            SqlUtils sqlUtils = new SqlUtils();

            sqlContent += this.AddTable(sqlUtils, "Config");
            sqlContent += this.AddTable(sqlUtils, "Users");
            sqlContent += "\r\nSET IDENTITY_INSERT dbo.Continents ON\r\n";
            sqlContent += this.AddTable(sqlUtils, "Continents");
            sqlContent += "SET IDENTITY_INSERT dbo.Continents OFF\r\n";
            sqlContent += "\r\nSET IDENTITY_INSERT dbo.Countries ON\r\n";
            sqlContent += this.AddTable(sqlUtils, "Countries");
            sqlContent += "SET IDENTITY_INSERT dbo.Countries OFF\r\n";
            sqlContent += "\r\nSET IDENTITY_INSERT dbo.Regions ON\r\n";
            sqlContent += this.AddTable(sqlUtils, "Regions");
            sqlContent += "SET IDENTITY_INSERT dbo.Regions OFF\r\n";
            sqlContent += "\r\nSET IDENTITY_INSERT dbo.Localities ON\r\n";
            sqlContent += this.AddTable(sqlUtils, "Localities");
            sqlContent += "SET IDENTITY_INSERT dbo.Localities OFF\r\n";
            sqlContent += "\r\nSET IDENTITY_INSERT dbo.Places ON\r\n";
            sqlContent += this.AddTable(sqlUtils, "Places");
            sqlContent += "SET IDENTITY_INSERT dbo.Places OFF\r\n";
            sqlContent += "\r\nSET IDENTITY_INSERT dbo.Genres ON\r\n";
            sqlContent += this.AddTable(sqlUtils, "Genres");
            sqlContent += "SET IDENTITY_INSERT dbo.Genres OFF\r\n";
            sqlContent += "\r\nSET IDENTITY_INSERT dbo.Species ON\r\n";
            sqlContent += this.AddTable(sqlUtils, "Species");
            sqlContent += "SET IDENTITY_INSERT dbo.Species OFF\r\n";
            sqlContent += "\r\nSET IDENTITY_INSERT dbo.SubSpecies ON\r\n";
            sqlContent += this.AddTable(sqlUtils, "SubSpecies");
            sqlContent += "SET IDENTITY_INSERT dbo.SubSpecies OFF\r\n";
            sqlContent += "\r\nSET IDENTITY_INSERT dbo.Trees ON\r\n";
            sqlContent += this.AddTable(sqlUtils, "Trees");
            sqlContent += "SET IDENTITY_INSERT dbo.Trees OFF\r\n";
            sqlContent += "\r\nSET IDENTITY_INSERT dbo.TreesImages ON\r\n";
            sqlContent += this.AddTable(sqlUtils, "TreesImages");
            sqlContent += "SET IDENTITY_INSERT dbo.TreesImages OFF\r\n";
            sqlContent += "\r\nSET IDENTITY_INSERT dbo.TreesImagesComments ON\r\n";
            sqlContent += this.AddTable(sqlUtils, "TreesImagesComments");
            sqlContent += "SET IDENTITY_INSERT dbo.TreesImagesComments OFF\r\n";
            sqlContent += "\r\nSET IDENTITY_INSERT dbo.TreesImagesVotes ON\r\n";
            sqlContent += this.AddTable(sqlUtils, "TreesImagesVotes");
            sqlContent += "SET IDENTITY_INSERT dbo.TreesImagesVotes OFF\r\n";
            sqlContent += "\r\nSET IDENTITY_INSERT dbo.TreesVotes ON\r\n";
            sqlContent += this.AddTable(sqlUtils, "TreesVotes");
            sqlContent += "SET IDENTITY_INSERT dbo.TreesVotes OFF\r\n";
            sqlContent += "\r\nSET IDENTITY_INSERT dbo.TreesGirthMeasurments ON\r\n";
            sqlContent += this.AddTable(sqlUtils, "TreesGirthMeasurments");
            sqlContent += "SET IDENTITY_INSERT dbo.TreesGirthMeasurments OFF\r\n";
            sqlContent += "\r\nSET IDENTITY_INSERT dbo.TreesHeightMeasurments ON\r\n";
            sqlContent += this.AddTable(sqlUtils, "TreesHeightMeasurments");
            sqlContent += "SET IDENTITY_INSERT dbo.TreesHeightMeasurments OFF\r\n";

            string fSpec = _currentBackupFolder + "\\db.SQL";
            System.IO.File.WriteAllText(fSpec, sqlContent, Encoding.GetEncoding(1250));
        }

        private string AddTable(SqlUtils _sqlUtils, string _tableName)
        {
            string retVal = String.Empty;
            DataTable dtbl = _sqlUtils.GetSqlDataSet("SELECT * FROM dbo." + _tableName).Tables[0];
            foreach (DataRow dr in dtbl.Rows)
                retVal += this.AddRecord(_tableName, dtbl, dr);
            retVal += "\r\n";
            return retVal;
        }
        private string AddRecord(string _tableName, DataTable _dt, DataRow _dr)
        {
            string retVal = "INSERT INTO dbo." + _tableName + "(";
            Type[] colTypes = new Type[_dt.Columns.Count];
            int c = 0;
            foreach (DataColumn dc in _dt.Columns)
            {
                colTypes[c] = dc.DataType;
                if (c > 0)
                    retVal += ",";
                retVal += dc.ColumnName;
                c++;
            }
            retVal += ") VALUES(";
            c = 0;
            foreach (object o in _dr.ItemArray)
            {
                if (c > 0)
                    retVal += ",";
                if (colTypes[c].Name == "String")
                    retVal += ("'" + o.ToString() + "'");
                else
                    retVal += o.ToString();
                c++;
            }
            retVal += ")\r\n";
            return retVal;
        }
        */ 

        #endregion

        private void reLpSubViewsButton_Click(object sender, EventArgs e)
        {
            SqlUtils su = new SqlUtils(this.connectString);
            su.ExecuteQuery("EXEC [dbo].[ReLpSubViews] " + currentViewLink.ParentAlbum.ID + ", " + currentViewLink.View.ID);
            this.RefreshCurrentView();
        }

        #endregion

        private void uxShowTextNotes_CheckedChanged(object sender, EventArgs e)
        {
            DescriptionBubbleForm.GlobalEnabled = uxShowTextNotes.Checked;
        }
    }

    class ListViewItemComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            Item xItem = (x as ListViewItem).Tag as Item;
            Item yItem = (y as ListViewItem).Tag as Item;
            return xItem.Lp.CompareTo(yItem.Lp);
        }
    }
}