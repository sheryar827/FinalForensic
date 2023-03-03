using Final_Forensic.Classes_fore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Final_Forensic
{
    public partial class login : Form
    {
        public login()
        {
            InitializeComponent();
        }

        private void txtName_Leave(object sender, EventArgs e)
        {
            if (txtName.Text=="") {
                error_signinName.SetError(this.txtName, "Please Enter Your name");
            }
            else
            {
                error_signinName.Clear();

            }
        }

        private void btn1_Click(object sender, EventArgs e)
        {
            if (formisvalid())
            {
                using (SqlConnection con = new SqlConnection(AppSqlCon.getconsting()))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_login_user", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@UserName",txtName.Text.ToString().Trim());
                        cmd.Parameters.AddWithValue("@Password",txtPass.Text.ToString().Trim());

                        if(con.State != ConnectionState.Open)
                        {
                            con.Open();
                        }

                        DataTable dt = new DataTable();

                        SqlDataReader sdr = cmd.ExecuteReader();

                        dt.Load(sdr);

                        if(dt.Rows.Count > 0)
                        {

                            this.Hide();
                            ForensicDashBoard mdd = new ForensicDashBoard();
                            mdd.Show();
                        }
                        
                    }
                }
            }
           /* else if (txtName.Text != "admin")
            {

                error_signinName.Clear();
                error_signinPass.Clear();
                label_Error.Text = "Incorrect Username/Password";
                label_Error.Visible = true;
                txtName.Focus();

            }
            else if (txtPass.Text != "admin")
            {
                label_Error.Text = "Incorrect Username/Password";
                label_Error.Visible = true;
                txtPass.Focus();


            }
            else {

                this.Hide();
                Main_dashboard mdd = new Main_dashboard();
                mdd.Show();            }
            */
        }

        private bool formisvalid()
        {
            var valid = true;
            if (txtName.Text == "")
            {
                error_signinName.SetError(txtName, "Please enter you name");
                txtName.Focus();
                valid = false;
            }
            if (txtName.Text.Length > 100)
            {
                error_signinName.SetError(txtName, "Name length not greater than 100 char");
                txtName.Focus();
                valid = false;
            }

            if (txtPass.Text == "")
            {
                error_signinPass.SetError(txtPass, "Please enter you name");
                txtName.Focus();
                valid = false;

            }

            if (txtPass.Text.Length > 100)
            {
                error_signinPass.SetError(txtPass, "Password length not greater than 100 char");
                txtName.Focus();
                valid= false;
            }

            return valid;
        }

        private void txtPass_Leave(object sender, EventArgs e)
        {
            if (txtPass.Text == "")
            {
                error_signinPass.SetError(this.txtPass, "Please Enter Your Password");
            }
            else
            {
                error_signinName.Clear();

            }

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void bunifuLabel4_Click(object sender, EventArgs e)
        {

        }

        private void bunifuTextBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void SignUpName_TextChanged(object sender, EventArgs e)
        {

        }

        private void SignUpName_Leave(object sender, EventArgs e)
        {
            if (SignUpName.Text == "")
            {
                //    error_signinPass.SetError(this.txtPass, "Please Enter Your Password");
                error_signUpName.SetError(SignUpName, "Please Enter you signup name");
            }
            else {
                error_signUpName.Clear();
            }
            
        }

        private void SignUpPass_Leave(object sender, EventArgs e)
        {
            if (SignUpPass.Text == "")
            {
                //    error_signinPass.SetError(this.txtPass, "Please Enter Your Password");
                error_signupPass.SetError(this.SignUpPass, "Please Enter you password");
            }
            else
            {
                error_signupPass.Clear();
            }



        }

        private void SignUprepass_Leave(object sender, EventArgs e)
        {
            if (SignUprepass.Text == "")
            {
                error_SignUprepass.SetError(this.SignUprepass, "Pleae re-enter your password");
            }
            else {
                error_SignUprepass.Clear();
            }

            
        }

        private void signup_card_Paint(object sender, PaintEventArgs e)
        {

        }

        private void signUpNew_Click(object sender, EventArgs e)
        {
            error_signinName.Clear();
            error_signinPass.Clear();
            signup_card.Visible = true;
        }

        private void Signin_back_Click(object sender, EventArgs e)
        {
            signup_card.Visible = false;
        }

        private void btn_signUp_Click(object sender, EventArgs e)
        {

            if (SignUpName.Text != "" && SignUpPass.Text == "" && SignUprepass.Text == "")
            {
                error_signUpName.Clear();
                error_signupPass.Clear();
                error_SignUprepass.Clear();
                signup_card.Visible = false;

            }
            else
            {
                using (SqlConnection con = new SqlConnection(AppSqlCon.getconsting()))
                {
                    using (SqlCommand cmd=new SqlCommand("sp_create_user", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserName",SignUpName.Text.ToString().Trim());
                        cmd.Parameters.AddWithValue("@Password", SignUpPass.Text.Trim().ToString().Trim());

                        if(con.State != ConnectionState.Open) {

                            con.Open();

                            if (cmd.ExecuteNonQuery() > 0)
                                lbStatus.Text = "User Created Successfully";
                           
                        }
                          

                       
                    }
                 

                }
            }
        }

        /*  private void Form1_Leave(object sender, EventArgs e)
          {

          }*/




    }
}
