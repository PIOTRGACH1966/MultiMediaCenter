using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace MultiMediaCenter
{
    public class Item
    {
        public int ID;
        public View ParentView = null;        
        public string FileSpec;
        public int Lp;
        public bool IsImportant;
        public ItemQuality Quality;        
        public bool IsArt;
        public bool IsHidden;        
        public string CurrentNotes = String.Empty;

        public bool Loaded = false;        

        public string ConnectionString = String.Empty;

        public Item(View _parentView, int _ID, string _connectString)
        {
            this.ParentView = _parentView;
            this.ID = _ID;
            this.ConnectionString = _connectString;
        }

        public string FileName
        {
            get { return System.IO.Path.GetFileName(this.FileSpec); }
        }

        public ContentType ContentType
        {
            get { Utils u = new Utils(); return u.ComputeContentType(this.FileSpec); }
        }

        public void Load()
        {
            SqlUtils su = new SqlUtils(this.ConnectionString);
            SqlDataReader rd = su.GetSqlReader("SELECT * FROM dbo.Items WHERE I_ID = " + this.ID);
            while (rd.Read())
            {
                this.FileSpec = Convert.ToString(rd["I_FileSpec"]);
                this.Lp = Convert.ToInt32(rd["I_Lp"]);
                this.IsImportant = Convert.ToBoolean(rd["I_IsImportant"]);
                this.Quality = (ItemQuality)(Convert.ToInt32(rd["I_Quality"]));                
                this.IsArt = Convert.ToBoolean(rd["I_IsArt"]);                
                this.IsHidden = Convert.ToBoolean(rd["I_IsHidden"]);
            }
            rd.Close();
            su.Close();

            Loaded = true;
        }

        public void LoadFromReader(SqlDataReader _rd)
        {
            this.FileSpec = Convert.ToString(_rd["I_FileSpec"]);
            this.Lp = Convert.ToInt32(_rd["I_Lp"]);
            this.IsImportant = Convert.ToBoolean(_rd["I_IsImportant"]);
            this.Quality = (ItemQuality)Convert.ToInt32(_rd["I_Quality"]);            
            this.IsArt = Convert.ToBoolean(_rd["I_IsArt"]);
            this.IsHidden = Convert.ToBoolean(_rd["I_IsHidden"]);

            Loaded = true;
        }

        public bool Save()
        {
            return this.Save(0);
        }
        public bool Save(int _Lp)
        {
            SqlUtils su = new SqlUtils(this.ConnectionString);
            string q;
            if (this.ID == 0)
            {
                q = "SELECT IsNull(COUNT(I_ID), 0) FROM dbo.Items WHERE I_VID = " + (int)(this.ParentView.ID) +
                    " AND I_FileSpec = '" + this.FileSpec + "'";
                int cnt = Convert.ToInt32(su.GetSqlScalar(q));
                if(cnt>0)
                {
                    System.Windows.Forms.MessageBox.Show("Item already exists in the view.");
                    su.Close();
                    return false;
                }

                //q = "SELECT IsNull(MAX(I_Lp), 0) FROM dbo.Items WHERE I_VID = " + (int)(this.ParentView.ID);
                //this.Lp = Convert.ToInt32(su.GetSqlScalar(q)) + 1;
                if (_Lp == 0)
                {
                    q = "SELECT TOP 1 IsNull(I_Lp, 0) FROM dbo.Items WHERE I_VID = " + (int)(this.ParentView.ID) +
                        " AND I_FileName < '" + this.FileName + "' ORDER BY I_FileName DESC";
                    this.Lp = Convert.ToInt32(su.GetSqlScalar(q)) + 1;
                }
                else
                    this.Lp = _Lp;
                q = "IF EXISTS (SELECT * FROM dbo.Items WHERE I_Lp = " + this.Lp + ") " +
                    " UPDATE dbo.Items SET I_Lp = I_Lp + 1 WHERE I_VID = " + (int)(this.ParentView.ID) +
                    " AND I_Lp >= " + this.Lp;
                su.ExecuteQuery(q);
                q = "INSERT INTO dbo.Items (I_VID, I_FileSpec, I_FileName, I_Lp, I_IsImportant, I_Quality, I_IsArt, I_IsHidden) " +
                  "VALUES(" + this.ParentView.ID + ", '" + this.FileSpec + "', '" + this.FileName + "', " +
                  this.Lp + ", " + 
                  (this.IsImportant ? "1" : "0") + ", " + Convert.ToInt32(this.Quality) + ", " + (this.IsArt ? "1" : "0") + ", " + (this.IsHidden ? "1" : "0") + 
                  ")";
                su.ExecuteQuery(q);
                this.ID = Convert.ToInt32(su.GetSqlScalar("SELECT IDENT_CURRENT('dbo.Items')"));
                if (this.ParentView != null)
                    this.ParentView.Items.Add(this);
            }
            else
            {
                q = "UPDATE dbo.Items SET I_VID = " + this.ParentView.ID +
                    ", I_FileSpec = '" + this.FileSpec + "', I_FileName = '" + this.FileName + "', " + 
                    "I_Lp = " + this.Lp +
                    ", I_IsImportant = " + (this.IsImportant ? "1" : "0") +
                    ", I_Quality = " + Convert.ToInt32(this.Quality) +                    
                    ", I_IsArt = " + (this.IsArt ? "1" : "0") +
                    ", I_IsHidden = " + (this.IsHidden ? "1" : "0") +
                    " WHERE I_ID = " + this.ID;
                su.ExecuteQuery(q);
            }
            su.Close();
            return true;
        }
        public bool ChgFileSpec(string _toSpec)
        {
            this.FileSpec = _toSpec;
            SqlUtils su = new SqlUtils(this.ConnectionString);
            string q;
            q = "UPDATE dbo.Items SET I_FileSpec = '" + this.FileSpec + "', I_FileName = '" + this.FileName + "' " + 
                " WHERE I_ID = " + this.ID;
            su.ExecuteQuery(q);
            su.Close();            
            return true;
        }

        public bool Delete(bool _ask)
        {
            SqlUtils su = new SqlUtils(this.ConnectionString);
            string q;
            if (this.ID == 0)
            {
                System.Windows.Forms.MessageBox.Show("Cannot delete a new, not saved album view item.");
                return false;
            }
            if (_ask)
            {
                if (MessageBox.Show("Are you sure?\r\nOperation will delete the item [" + 
                    this.ParentView.Name + "\\" + this.FileName + "].", "Warning",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                    return false;
            }
            if (!this.Loaded)
                this.Load();
            q = "DELETE dbo.Items WHERE I_ID = " + this.ID;
            su.ExecuteQuery(q);            
            if (this.ParentView != null)
                this.ParentView.Items.Remove(this);
            su.Close();
            return true;
        }

        public bool Up()
        {
            SqlUtils su = new SqlUtils(this.ConnectionString);
            string q = "SELECT TOP 1 prevID = I_ID, prevLp = I_Lp FROM dbo.Items WHERE I_VID = " + (int)(this.ParentView.ID) +
                " AND I_Lp < " + this.Lp + " ORDER BY I_Lp DESC";
            SqlDataReader rd = su.GetSqlReader(q);
            int prevID = 0;
            int prevLp = 0;
            while (rd.Read())
            {
                prevID = Convert.ToInt32(rd["prevID"]);
                prevLp = Convert.ToInt32(rd["prevLp"]);
            }
            rd.Close();

            if (prevLp <= 0 || prevID <= 0)
                return false;

            q = "UPDATE dbo.Items SET I_Lp = " + prevLp +
                " WHERE I_ID = " + this.ID + "; " +
                "UPDATE dbo.Items SET I_Lp = " + this.Lp +
                " WHERE I_ID = " + prevID;
            su.ExecuteQuery(q);

            Item prevItem = new Item(this.ParentView, prevID, this.ConnectionString);
            prevItem.Load();
            prevItem.Lp = this.Lp;
            prevItem.Save();
            this.Lp = prevLp;
            this.Save();

            return true;
        }
        public bool Down()
        {
            SqlUtils su = new SqlUtils(this.ConnectionString);
            string q = "SELECT TOP 1 nextID = I_ID, nextLp = I_Lp FROM dbo.Items WHERE I_VID = " + (int)(this.ParentView.ID) +
                " AND I_Lp > " + this.Lp + " ORDER BY I_Lp ASC";
            SqlDataReader rd = su.GetSqlReader(q);
            int nextID = 0;
            int nextLp = 0;
            while (rd.Read())
            {
                nextID = Convert.ToInt32(rd["nextID"]);
                nextLp = Convert.ToInt32(rd["nextLp"]);
            }
            rd.Close();

            if (nextLp <= 0 || nextID <= 0)
                return false;

            q = "UPDATE dbo.Items SET I_Lp = " + nextLp +
                " WHERE I_ID = " + this.ID + "; " +
                "UPDATE dbo.Items SET I_Lp = " + this.Lp +
                " WHERE I_ID = " + nextID;
            su.ExecuteQuery(q);

            Item nextItem = new Item(this.ParentView, nextID, this.ConnectionString);
            nextItem.Load();
            nextItem.Lp = this.Lp;
            nextItem.Save();
            this.Lp = nextLp;
            this.Save();

            return true;
        }
    }
}
