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
    class DeleteCase : IDeleteCase
    {
        //public int Id { get; set; }
        
        public void deletCase(int id)
        {
            try
            {

                string connection = AppSqlCon.getconsting();

                using (SqlConnection con = new SqlConnection(connection))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_deleteCase", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@id", id);

                        if (con.State != ConnectionState.Open)
                        {
                            con.Open();
                        }

                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            MessageBox.Show("Case Deleted Successfully"
                                , "Information"
                                , MessageBoxButtons.OK
                                , MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show($"Case {id} does not exist"
                                , "Error"
                                , MessageBoxButtons.OK
                                , MessageBoxIcon.Error);
                        }
                    }
                }
            }catch(Exception ex)
            {
                MessageBox.Show (ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
