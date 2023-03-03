using Final_Forensic.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Final_Forensic.Classes_fore
{
    class SearchRecord : ISearchRecord
    {
        public DataTable searchData(string procName, string valueToSearch)
        {
            DataTable tdSearchResult = new DataTable();

            try
            {

                string connection = AppSqlCon.getconsting();

                using (SqlConnection con = new SqlConnection(connection))
                {

                    using (SqlCommand cmd = new SqlCommand(procName, con))
                    {

                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@AnyID", valueToSearch);

                        if (con.State != ConnectionState.Open)
                        {
                            con.Open();
                        }


                        SqlDataReader sdr = cmd.ExecuteReader();
                        tdSearchResult.Load(sdr);



                        /* if (tdSearchResult.Rows.Count > 0)
                         {
                             //gv_social.DataSource = td_biodata;

                         }
                         else
                         {
                             MessageBox.Show("Some Error occur \"No Data to show\"");
                         }*/
                    }

                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return tdSearchResult;
        }
    }

}
