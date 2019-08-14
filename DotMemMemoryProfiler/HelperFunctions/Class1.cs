using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperFunctions
{
    public class Class1
    {

        public static bool checkIfReadyToRead()
        {
            List<string> namespaces = new List<string>();
            while (true)
            {
                try
                {
                    SQLiteConnection conn = new SQLiteConnection(connection);
                    conn.Open();
                    string sql = "SELECT * FROM Identifier";
                    SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            namespaces.Add(reader["Indicator"].ToString());
                        }
                    }
                    conn.Close();
                    break;
                }
                catch (SQLiteException e1)
                {
                }
            }
            return true;
        }
    }
    
}
