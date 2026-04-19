using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace MultiMediaCenter
{
    public class ViewLink
    {
        public int ID;
        public Album ParentAlbum = null;
        public ViewLink ParentViewLink = null;
        public View View = null;
        public string name = String.Empty;
        public int Lp;

        public List<ViewLink> SubLinks = null;

        public bool Loaded = false;

        public string ConnectionString = String.Empty;

        public ViewLink(Album _parentAlbum, View _view, string _connectString)
        {
            this.ParentAlbum = _parentAlbum;
            this.ParentViewLink = null;
            this.View = _view;
            this.SubLinks = new List<ViewLink>();
            this.ConnectionString = _connectString;
            this.SetIDAndName();
        }

        public ViewLink(Album _parentAlbum, View _view, string _connectString, SqlDataReader _rd)
        {
            this.ParentAlbum = _parentAlbum;
            this.ParentViewLink = null;
            this.View = _view;
            this.SubLinks = new List<ViewLink>();
            this.ConnectionString = _connectString;
            this.SetIDAndName(_rd);
        }

        public ViewLink(ViewLink _parentViewLink, View _view, string _connectString)
        {
            if(_parentViewLink != null)
                this.ParentAlbum = _parentViewLink.ParentAlbum;
            this.ParentViewLink = _parentViewLink;
            this.View = _view;
            this.SubLinks = new List<ViewLink>();
            this.ConnectionString = _connectString;
            this.SetIDAndName();
        }

        public ViewLink(ViewLink _parentViewLink, View _view, string _connectString, SqlDataReader _rd)
        {
            if (_parentViewLink != null)
                this.ParentAlbum = _parentViewLink.ParentAlbum;
            this.ParentViewLink = _parentViewLink;
            this.View = _view;
            this.SubLinks = new List<ViewLink>();
            this.ConnectionString = _connectString;
            this.SetIDAndName(_rd);
        }

        private void SetIDAndName()
        {
            SqlUtils su = new SqlUtils(this.ConnectionString);
            string q;
            if(this.ParentViewLink == null)
                q = "SELECT VL_ID, VL_Name FROM dbo.ViewsLinks WHERE VL_ParentAID = " + this.ParentAlbum.ID + " AND VL_ParentVID Is Null " + 
                    "AND VL_VID = " + this.View.ID;
            else
                q = "SELECT VL_ID, VL_Name FROM dbo.ViewsLinks WHERE VL_ParentAID = " + this.ParentAlbum.ID + " AND VL_ParentVID = " + this.ParentViewLink.View.ID +
                    "AND VL_VID = " + this.View.ID;
            SqlDataReader rd = su.GetSqlReader(q);
            while (rd.Read())
            {
                this.SetIDAndName(rd);
                break;
            }
            rd.Close();
            su.Close();
        }

        private void SetIDAndName(SqlDataReader _rd)
        {
            this.ID = Convert.ToInt32(_rd["VL_ID"]);
            this.name = Convert.ToString(_rd["VL_Name"]);
        }

        public string Name
        {
            get { if (!String.IsNullOrEmpty(name)) return name; else return this.View.Name; }
            set { name = value; }
        }

        public string ViewPath
        {
            get
            {
                string retVal = this.Name;
                SqlUtils su = new SqlUtils(this.ConnectionString);
                string q;

                int id = this.ID;
                while(true)
                {
                    q = "SELECT parentAID = vl1.VL_ParentAID, parentVLID = vl2.VL_ID, parentName = CASE WHEN vl2.VL_Name <> '' THEN vl2.VL_Name ELSE V_Name END " + 
                        "FROM dbo.ViewsLinks vl1 LEFT OUTER JOIN dbo.ViewsLinks vl2 ON vl2.VL_VID = vl1.VL_ParentVID LEFT OUTER JOIN dbo.Views ON V_ID = vl2.VL_VID " +
                        "WHERE vl1.VL_ID = " + id;
                    SqlDataReader rd = su.GetSqlReader(q);
                    while (rd.Read())
                    {
                        try
                        {
                            id = Convert.ToInt32(rd["parentVLID"]);
                        }
                        catch
                        {
                            id = 0;
                        }
                        if (id == 0)
                            break;
                        else
                            retVal = Convert.ToString(rd["parentName"]) + "\\" + retVal;
                    }
                    rd.Close();
                    if (id == 0)
                        break;
                }

                /*
                ViewLink viewLink = this;
                if (!viewLink.Loaded)
                    viewLink.Load(0, false);
                while (viewLink.ParentViewLink != null)
                {
                    retVal = viewLink.ParentViewLink.Name + "\\" + retVal;
                    viewLink = viewLink.ParentViewLink;
                    viewLink.Load(0, false);
                }
                */ 
                return retVal;
            }
        }

        public void Load(int _recursiveLevel, bool _loadItems)
        {
            SqlUtils su = new SqlUtils(this.ConnectionString);
            string q;
            q = "SELECT VL_ParentAID, VL_ParentVID, VL_Name, VL_Lp FROM dbo.ViewsLinks WHERE VL_ID = " + this.ID;
            SqlDataReader rd = su.GetSqlReader(q);
            while (rd.Read())
            {
                if (this.ParentAlbum == null)
                {
                    int aID = 0;
                    try
                    {
                        aID = Convert.ToInt32(rd["VL_ParentAID"]);
                    }
                    catch { }
                    if (aID != 0)
                    {
                        this.ParentAlbum = new Album(aID, this.ConnectionString);
                        this.ParentAlbum.Load(0);
                    }
                }
                if (this.ParentViewLink == null)
                {
                    int vID = 0;
                    try
                    {
                        vID = Convert.ToInt32(rd["VL_ParentVID"]);
                    }
                    catch { }
                    if (vID != 0)
                    {
                        View v = new View(vID, this.ConnectionString);
                        this.ParentViewLink = new ViewLink(this.ParentAlbum, v, this.ConnectionString);
                        this.ParentViewLink.Load(0, false);
                    }
                }
                name = Convert.ToString(rd["VL_Name"]);
                this.Lp = Convert.ToInt32(rd["VL_Lp"]);
                break;
            }
            rd.Close();
            su.Close();

            this.LoadSubLinksAndItems(_recursiveLevel, _loadItems);
        }

        public void LoadFromReader(SqlDataReader _rd, int _recursiveLevel, bool _loadItems)
        {
            name = Convert.ToString(_rd["VL_Name"]);
            this.Lp = Convert.ToInt32(_rd["VL_Lp"]);

            this.LoadSubLinksAndItems(_recursiveLevel, _loadItems);
        }

        private void LoadSubLinksAndItems(int _recursiveLevel, bool _loadItems)
        {
            if (_recursiveLevel >= 1)
            {
                if (_loadItems)
                    this.View.LoadItems();

                this.SubLinks.Clear();
                //List<int> viewsIDs = new List<int>();
                string q = "SELECT * FROM dbo.ViewsLinks WHERE VL_ParentAID = " + this.ParentAlbum.ID +
                    " AND VL_ParentVID = " + this.View.ID + " ORDER BY VL_Lp";
                SqlUtils su = new SqlUtils(this.ConnectionString);
                SqlDataReader rd = su.GetSqlReader(q);
                while (rd.Read())
                {
                    //viewsIDs.Add(Convert.ToInt32(rd["VL_VID"]));
                    View view = new View(Convert.ToInt32(rd["VL_VID"]), this.ConnectionString);
                    ViewLink subLink = new ViewLink(this, view, this.ConnectionString);
                    if (_recursiveLevel == 2)
                        subLink.LoadFromReader(rd, _recursiveLevel, _loadItems);
                    SubLinks.Add(subLink);
                }
                rd.Close();
                su.Close();

                Loaded = true;
            }
        }

        public void Save(bool _renumberAfter)
        {
            SqlUtils su = new SqlUtils(this.ConnectionString);
            string q;
            string parentVID = String.Empty;
            if (this.ParentViewLink == null)
                parentVID = "VL_ParentVID Is Null";
            else
                parentVID = "VL_ParentVID = " + Convert.ToInt32(this.ParentViewLink.View.ID);
            q = "IF EXISTS (SELECT * FROM dbo.ViewsLinks WHERE VL_ParentAID = " + this.ParentAlbum.ID + " AND " + parentVID + 
                " AND VL_VID = " + this.View.ID + ") SELECT 1 ELSE SELECT 0";
            bool isNew = (Convert.ToInt32(su.GetSqlScalar(q)) == 0);
            if (this.ID == 0)
            {
                if (this.Lp == 0)
                {
                    q = "SELECT IsNull(MAX(VL_Lp), 0) FROM dbo.ViewsLinks WHERE VL_ParentAID = " + this.ParentAlbum.ID + " AND " + parentVID;
                    this.Lp = Convert.ToInt32(su.GetSqlScalar(q)) + 1;
                }
                q = "INSERT INTO dbo.ViewsLinks (VL_ParentAID, VL_ParentVID, VL_VID, VL_Name, VL_Lp) " +
                    "VALUES(" + this.ParentAlbum.ID + ", " + (this.ParentViewLink == null ? "null" : this.ParentViewLink.View.ID.ToString()) + 
                    ", " + this.View.ID + ", '" + this.Name + "', " + this.Lp + ")";
                su.ExecuteQuery(q);
                this.ID = Convert.ToInt32(su.GetSqlScalar("SELECT IDENT_CURRENT('dbo.ViewsLinks')"));
                if (this.ParentViewLink != null)
                    this.ParentViewLink.SubLinks.Add(this);
                else if (this.ParentAlbum != null)
                    this.ParentAlbum.ViewsLinks.Add(this);
            }
            else
            {
                q = "UPDATE dbo.ViewsLinks SET VL_ParentAID = " + this.ParentAlbum.ID + 
                    ", VL_ParentVID = " + (this.ParentViewLink == null ? "null" : this.ParentViewLink.View.ID.ToString()) +
                    ", VL_VID = " + this.View.ID + ", VL_Name = '" + this.Name + "', VL_Lp = " + this.Lp + 
                    " WHERE VL_ID = " + this.ID;
                su.ExecuteQuery(q);
            }
            if (_renumberAfter)
            {
                q = "UPDATE dbo.ViewsLinks SET VL_Lp = VL_Lp + 1 WHERE VL_ParentAID = " + this.ParentAlbum.ID + " AND " + parentVID +
                    " AND VL_ID <> " + this.ID + " AND VL_Lp >= " + this.Lp;
                su.ExecuteQuery(q);
            }
            su.Close();
        }

        public bool Delete(bool _ask)
        {
            SqlUtils su = new SqlUtils(this.ConnectionString);
            string q;
                q = "SELECT COUNT(*) FROM dbo.ViewsLinks WHERE VL_VID = " + this.View.ID;
                bool isTheOnlyLinkOfView = (Convert.ToInt32(su.GetSqlScalar(q)) <= 1);
            if (_ask)
            {
                if (MessageBox.Show("ARE YOU SURE?\r\nOPERATION WILL DELETE SELECTED LINK OF VIEW [" + this.Name + "].", "Warning",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                    return false;
                if (isTheOnlyLinkOfView)
                {
                    if (MessageBox.Show("ARE YOU SURE?\r\nTHIS IS THE ONLY LINK OF THE VIEW  [" + this.Name + "]. OPERATION WILL DELETE SELECTED LINK OF THE VIEW AS WELL AS THE VIEW ITSELF.", "Warning",
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                        return false;
                }
            }
            if (isTheOnlyLinkOfView)
            {
                if (!this.Loaded)
                    this.Load(2, true);
            }
            if (this.ParentViewLink != null)
                this.ParentViewLink.SubLinks.Remove(this);
            else if (this.ParentAlbum != null)
                this.ParentAlbum.ViewsLinks.Remove(this);
            q = "DELETE dbo.ViewsLinks WHERE VL_ID = " + this.ID;
            su.ExecuteQuery(q);
            if (isTheOnlyLinkOfView)
            {
                if (!this.View.Loaded)
                    this.View.Load(true);
                List<Item> itemsToDelete = new List<Item>();
                foreach (Item item in this.View.Items)
                    itemsToDelete.Add(item);
                foreach (Item item in itemsToDelete)
                    item.Delete(false);
                List<ViewLink> linksToDelete = new List<ViewLink>();
                foreach (ViewLink link in this.SubLinks)
                    linksToDelete.Add(link);
                foreach (ViewLink link in linksToDelete)
                    link.Delete(false);
                q = "DELETE dbo.Views WHERE V_ID = " + this.View.ID;
                su.ExecuteQuery(q);
            }            
            return true;
        }

        public bool Up()
        {
            SqlUtils su = new SqlUtils(this.ConnectionString);
            string q;
            int prevID = -1;
            int prevLp = -1;
            if(this.ParentViewLink == null)
                q = "SELECT TOP 1 VL_ID, VL_Lp FROM dbo.ViewsLinks " +
                    "WHERE VL_ParentAID = " + this.ParentAlbum.ID + " AND VL_ParentVID Is Null AND VL_Lp < " + this.Lp + 
                    " ORDER BY VL_Lp DESC";
            else
                q = "SELECT TOP 1 VL_ID, VL_Lp FROM dbo.ViewsLinks " +
                    "WHERE VL_ParentAID = " + this.ParentAlbum.ID + " AND VL_ParentVID = " + this.ParentViewLink.View.ID + " AND VL_Lp < " + this.Lp + 
                    " ORDER BY VL_Lp DESC";
            SqlDataReader rd = su.GetSqlReader(q);
            while (rd.Read())
            {
                prevID = Convert.ToInt32(rd["VL_ID"]);
                prevLp = Convert.ToInt32(rd["VL_Lp"]);
            }
            rd.Close();
            if (prevID == -1 || prevLp == -1)
                return false;
            q = "UPDATE dbo.ViewsLinks SET VL_Lp = " + this.Lp + " WHERE VL_ID = " + prevID + "; " +
                "UPDATE dbo.ViewsLinks SET VL_Lp = " + prevLp + " WHERE VL_ID = " + this.ID;
            su.ExecuteQuery(q);
            this.Lp = prevLp;
            return true;
        }
        public bool Down()
        {
            SqlUtils su = new SqlUtils(this.ConnectionString);
            string q;
            int nextID = -1;
            int nextLp = -1;
            if(this.ParentViewLink == null)
                q = "SELECT TOP 1 VL_ID, VL_Lp FROM dbo.ViewsLinks " +
                    "WHERE VL_ParentAID = " + this.ParentAlbum.ID + " AND VL_ParentVID Is Null AND VL_Lp > " + this.Lp + 
                    " ORDER BY VL_Lp ASC";
            else
                q = "SELECT TOP 1 VL_ID, VL_Lp FROM dbo.ViewsLinks " +
                    "WHERE VL_ParentAID = " + this.ParentAlbum.ID + " AND VL_ParentVID = " + this.ParentViewLink.View.ID + " AND VL_Lp > " + this.Lp + 
                    " ORDER BY VL_Lp ASC";
            SqlDataReader rd = su.GetSqlReader(q);
            while (rd.Read())
            {
                nextID = Convert.ToInt32(rd["VL_ID"]);
                nextLp = Convert.ToInt32(rd["VL_Lp"]);
            }
            rd.Close();
            if (nextID == -1 || nextLp == -1)
                return false;
            q = "UPDATE dbo.ViewsLinks SET VL_Lp = " + this.Lp + " WHERE VL_ID = " + nextID + "; " +
                "UPDATE dbo.ViewsLinks SET VL_Lp = " + nextLp + " WHERE VL_ID = " + this.ID;
            su.ExecuteQuery(q);
            this.Lp = nextLp;
            return true;
        }
        public bool RenumItems(string _mode)
        {
            bool retVal = this.View.RenumItems(_mode);
            SqlUtils su = new SqlUtils(this.ConnectionString);
            string q = "SELECT I_Lp FROM dbo.Items WHERE I_ID = " + this.ID;
            this.Lp = Convert.ToInt32(su.GetSqlScalar(q));
            return true;
        }
    }
}
