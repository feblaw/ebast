using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Migration
{
    public class Data
    {

        public String Table { get; set; }

        public int CountData { get; set; }

        public Data(String _table)
        {
            Table = _table;
        }

        public List<string> GetColumn()
        {
            List<string> Data = new List<string>();
            try
            {
                var db = Connection.Instance();
                var Rows = db.Query("SHOW columns FROM "+Table);
                if (Rows.HasRows)
                {
                    while (Rows.Read())
                    {
                        Data.Add(Rows.GetString("Field"));
                    }
                }
                db.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return Data;
        }

        public List<Object> GetData()
        {
            List<Object> Data = new List<Object>();
            try
            {
                var Column = GetColumn();
                var db = Connection.Instance();
                var Rows = db.Query("SELECT * FROM "+Table);
                if (Rows.HasRows)
                {
                    while (Rows.Read())
                    {

                        List<string> Temp = new List<string>();
                        foreach (var row in Column)
                        {
                            Temp.Add(Rows.GetString(row));
                           // Console.WriteLine(row);
                        }
                        Data.Add(Temp);
                    }
                }
                db.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return Data;
        }


    }
}
