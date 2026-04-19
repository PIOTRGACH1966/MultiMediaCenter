using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace MultiMediaCenter
{
    public class View
    {
        public int ID;
        public string Name;
        public bool IsHidden;

        public List<Item> Items = null;

        public bool Loaded = false;

        public string ConnectionString = String.Empty;

        public View(int _ID, string _connectString)
        {
            this.ID = _ID;
            Items = new List<Item>();
            this.ConnectionString = _connectString;
            this.Load(false);
        }

        public void Load(bool _loadItems)
        {
            SqlUtils su = new SqlUtils(this.ConnectionString);
            SqlDataReader rd = su.GetSqlReader("SELECT * FROM dbo.Views WHERE V_ID = " + this.ID);
            while (rd.Read())
            {
                this.Name = Convert.ToString(rd["V_Name"]);
                this.IsHidden = Convert.ToBoolean(rd["V_IsHidden"]);
                break;
            }
            rd.Close();
            su.Close();

            if (_loadItems)
            {
                this.LoadItems();
                Loaded = true;
            }
        }
        public void LoadItems()
        {
            this.Items.Clear();
            //List<int> itemsIDs = new List<int>();
            SqlUtils su = new SqlUtils(this.ConnectionString);
            SqlDataReader rd = su.GetSqlReader("SELECT * FROM dbo.Items WHERE I_VID = " + this.ID + " ORDER BY I_Lp");
            while (rd.Read())
            {
                //itemsIDs.Add(Convert.ToInt32(rd["I_ID"]));
                Item item = new Item(this, Convert.ToInt32(rd["I_ID"]), this.ConnectionString);
                item.LoadFromReader(rd);
                this.Items.Add(item);
            }
            rd.Close();
            su.Close();
        }

        public void Save()
        {
            SqlUtils su = new SqlUtils(this.ConnectionString);
            string q;
            string vid = String.Empty;
            if (this.ID == 0)
            {
                q = "INSERT INTO dbo.Views (V_Name, V_IsHidden) " + 
                    "VALUES('" + this.Name + "', " + (this.IsHidden ? "1" : "0") + ")";
                su.ExecuteQuery(q);
                this.ID = Convert.ToInt32(su.GetSqlScalar("SELECT IDENT_CURRENT('dbo.Views')"));
            }
            else
            {
                q = "UPDATE dbo.Views SET V_Name = '" + this.Name + "', V_IsHidden = " + (this.IsHidden ? "1" : "0") + 
                    " WHERE V_ID = " + this.ID;
                q += "; UPDATE dbo.ViewsLinks SET VL_Name = '" + this.Name + "' WHERE VL_VID = " + this.ID;
                su.ExecuteQuery(q);
            }
        }

        public bool RenumItems(string _mode)
        {
            SqlUtils su = new SqlUtils(this.ConnectionString);
            string q;
            q = "EXEC dbo.RenumViewItems" + _mode + " " + this.ID;
            su.ExecuteQuery(q);
            return true;
        }

    }
}
