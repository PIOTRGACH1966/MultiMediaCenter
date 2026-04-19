using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace MultiMediaCenter
{
    public class SqlUtils
    {
        public string connectString = String.Empty;
        SqlConnection cn = null;

        public SqlUtils(string _connectString)
        {
            this.connectString = _connectString;
            cn = new System.Data.SqlClient.SqlConnection(this.connectString);
            cn.Open();
        }

        public SqlDataReader GetSqlReader(string q)
        {            
            SqlCommand cmd = new System.Data.SqlClient.SqlCommand(q, cn);
            System.Data.SqlClient.SqlDataReader rd = cmd.ExecuteReader();
            return rd;
        }

        public object GetSqlScalar(string q)
        {
            SqlCommand cmd = new System.Data.SqlClient.SqlCommand(q, cn);
            return cmd.ExecuteScalar();
        }

        public void ExecuteQuery(string q)
        {
            SqlCommand cmd = new System.Data.SqlClient.SqlCommand(q, cn);
            cmd.ExecuteNonQuery();
            return;
        }

        public void Close()
        {
            cn.Close();
        }
    }
}
