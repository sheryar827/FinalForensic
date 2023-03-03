using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_Forensic.Classes_fore
{
    public class AppSqlCon
    {
        public static string getconsting()
        {
            return ConfigurationManager.ConnectionStrings["Con_forensic"].ConnectionString;

        }
         
    }
}
