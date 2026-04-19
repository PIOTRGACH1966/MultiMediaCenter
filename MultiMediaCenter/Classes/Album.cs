using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace MultiMediaCenter
{
    public class Album
    {
        public int ID;
        public string Name;
        public int Lp;

        public List<ViewLink> ViewsLinks = null;

        public string ConnectionString = String.Empty;

        public bool Loaded = false;

        public Album(int _ID, string _connectString)
        {
            this.ID = _ID;
            ViewsLinks = new List<ViewLink>();
            this.ConnectionString = _connectString;
        }

        public void Load(int _recursiveLevel)
        {
            SqlUtils su = new SqlUtils(this.ConnectionString);
            SqlDataReader rd = su.GetSqlReader("SELECT * FROM dbo.Albums WHERE A_ID = " + this.ID);
            while (rd.Read())
            {
                this.LoadFromReader(rd, _recursiveLevel);
                break;
            }
            rd.Close();
            su.Close();
        }

        public void LoadFromReader(SqlDataReader _rd, int _recursiveLevel)
        {
            this.Name = Convert.ToString(_rd["A_Name"]);
            this.Lp = Convert.ToInt32(_rd["A_Lp"]);

            if (_recursiveLevel >= 1)
            {
                this.ViewsLinks.Clear();
                //List<int> viewsLinksList = new List<int>();
                SqlUtils su = new SqlUtils(this.ConnectionString);
                SqlDataReader rd = su.GetSqlReader("SELECT * FROM dbo.ViewsLinks WHERE VL_ParentAID = " + this.ID + " AND VL_ParentVID Is Null ORDER BY VL_Lp");
                while (rd.Read())
                {
                    //viewsLinksList.Add(Convert.ToInt32(rd["VL_VID"]));
                    View view = new View(Convert.ToInt32(rd["VL_VID"]), this.ConnectionString);
                    ViewLink viewLink = new ViewLink(this, view, this.ConnectionString, rd);
                    if (_recursiveLevel == 2)
                        viewLink.LoadFromReader(rd, _recursiveLevel, true);
                    ViewsLinks.Add(viewLink);
                }
                rd.Close();
                su.Close();

                Loaded = true;
            }

        }

        public void Save()
        {
            SqlUtils su = new SqlUtils(this.ConnectionString);
            string q;
            if (this.ID == 0)
            {                
                q = "SELECT IsNull(MAX(A_Lp), 0) FROM dbo.Albums";
                this.Lp = Convert.ToInt32(su.GetSqlScalar(q)) + 1;
                q = "INSERT INTO dbo.Albums (A_Name, A_Lp) " + 
                    "VALUES('" + this.Name + "', " + this.Lp + ")";
                su.ExecuteQuery(q);
                this.ID = Convert.ToInt32(su.GetSqlScalar("SELECT IDENT_CURRENT('dbo.Albums')"));
            }
            else
            {
                q = "UPDATE dbo.Albums SET A_Name = '" + this.Name + "', A_Lp = " + this.Lp +
                    " WHERE A_ID = " + this.ID;
                su.ExecuteQuery(q);
            }
            su.Close();
        }

        public bool Delete()
        {
            SqlUtils su = new SqlUtils(this.ConnectionString);
            string q;
            if (this.ID == 0)
            {
                System.Windows.Forms.MessageBox.Show("Cannot delete a new, not saved album.");
                return false;
            }
            for(int ii = 0; ii < 2; ii++)
                if (MessageBox.Show("ARE YOU SURE?\r\nOPERATION WILL DELETE ALBUM [" + this.Name + "] AND ALL ITS VIEWS AND ITEMS!", "Warning",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                    return false;
            List<ViewLink> viewsLinksToDel = new List<ViewLink>();
            foreach (ViewLink viewLink in this.ViewsLinks)
                viewsLinksToDel.Add(viewLink);
            int i = 0;
            foreach (ViewLink viewLink in viewsLinksToDel)
            {
                ViewLink viewLinkOrg = this.ViewsLinks[0];
                if (!viewLink.Delete(false))
                    return false;
                i++;
            }
            q = "DELETE dbo.ViewsLinks WHERE VL_ParentAID = " + this.ID;
            su.ExecuteQuery(q);
            q = "DELETE dbo.Views WHERE NOT EXISTS (SELECT * FROM dbo.ViewsLinks WHERE VL_VID = V_ID)";
            su.ExecuteQuery(q);
            q = "DELETE dbo.Items WHERE NOT EXISTS (SELECT * FROM dbo.ViewsLinks WHERE VL_VID = I_VID)";
            su.ExecuteQuery(q);
            q = "DELETE dbo.Albums WHERE A_ID = " + this.ID;
            su.ExecuteQuery(q);
            return true;
        }

        public bool Up()
        {
            SqlUtils su = new SqlUtils(this.ConnectionString);
            string q;
            int prevID = -1;
            int prevLp = -1;
            q = "SELECT TOP 1 A_ID, A_Lp FROM dbo.Albums WHERE A_Lp < " + this.Lp + " ORDER BY A_Lp DESC";
            SqlDataReader rd = su.GetSqlReader(q);
            while (rd.Read())
            {
                prevID = Convert.ToInt32(rd["A_ID"]);
                prevLp = Convert.ToInt32(rd["A_Lp"]);
            }
            rd.Close();
            if(prevID == -1 || prevLp == -1)
                return false;
            q = "UPDATE dbo.Albums SET A_Lp = " + this.Lp + " WHERE A_ID = " + prevID + "; " +
                "UPDATE dbo.Albums SET A_Lp = " + prevLp + " WHERE A_ID = " + this.ID;
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
            q = "SELECT TOP 1 A_ID, A_Lp FROM dbo.Albums WHERE A_Lp > " + this.Lp + " ORDER BY A_Lp ASC";
            SqlDataReader rd = su.GetSqlReader(q);
            while (rd.Read())
            {
                nextID = Convert.ToInt32(rd["A_ID"]);
                nextLp = Convert.ToInt32(rd["A_Lp"]);
            }
            rd.Close();
            if (nextID == -1 || nextLp == -1)
                return false;
            q = "UPDATE dbo.Albums SET A_Lp = " + this.Lp + " WHERE A_ID = " + nextID + "; " +
                "UPDATE dbo.Albums SET A_Lp = " + nextLp + " WHERE A_ID = " + this.ID;
            su.ExecuteQuery(q);
            this.Lp = nextLp;
            return true;
        }
    }
}
