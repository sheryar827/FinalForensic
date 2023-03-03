using ExcelDataReader;
using Final_Forensic.Classes;
using Final_Forensic.Classes_fore;
using Final_Forensic.Interfaces;
using Final_Forensic.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;


namespace Final_Forensic
{
    public partial class ForensicDashBoard : Form
    {
        List<StandContactsModel> listRawContacts;
        List<StandContactsModel> listRawCalls;
        List<StandContactsModel> list_unique_calls;

        DataTableCollection dt_col;
        DataTable dtRawContacts = null;
        DataTable dtRawCalls = null;
        DataTable dtStandContacts = new DataTable();
        DataTable dtStandCalls = new DataTable();


        IDeleteCase interfaceDeleteCase = new DeleteCase(); 
        ISearchRecord interfaceSearchData = new SearchRecord();
        IStandData interfaceStandContacts = new StandContacts();

        public int caseId;
        string social_ID;
        bool isCollapsed = true;
        bool searchByCaseID = false;
        bool searchByOtherOptions = true;
        List<string> msisdn_search_list= new List<string>();
        List<string> name_search_list = new List<string>();
        DataTable bulk_search_dt = new DataTable();
        public ForensicDashBoard()
        {
            InitializeComponent();
        }



        private void btn_submit_Click(object sender, EventArgs e)
        {
            
            if (txt_name.Text == "")
            {
                MessageBox.Show("Please Enter suspect Name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txt_name.Focus();

            }

            else if (txt_imei.Text == "")
            {
                MessageBox.Show("Please Enter IMEI", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txt_imei.Focus();
            }
            else if (txt_ofr_submit.Text == "")
            {

                MessageBox.Show("Please enter officer name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txt_ofr_submit.Focus();
            }
            else if (txt_remarks.Text == "")
            {

                MessageBox.Show("Please Enter your remarks", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txt_remarks.Focus();
            }
            else
            {
                using (SqlConnection con = new SqlConnection(AppSqlCon.getconsting()))

                {
                    using (SqlCommand cmd = new SqlCommand("sp_biodata", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Name", txt_name.Text.ToString().Trim());
                        cmd.Parameters.AddWithValue("@Imei", txt_imei.Text.ToString().Trim());
                        cmd.Parameters.AddWithValue("@For_exp", txt_For_expert.Text.ToString().Trim());
                        cmd.Parameters.AddWithValue("@OfrSubmit", txt_ofr_submit.Text.ToString().Trim());
                        cmd.Parameters.AddWithValue("@Remarks", txt_remarks.Text.ToString().Trim());

                        if (con.State != ConnectionState.Open)
                        {
                            con.Open();

                        }
                        try
                        {

                            DataTable dt = new DataTable();
                            SqlDataReader sdr = cmd.ExecuteReader();
                            dt.Load(sdr);
                            caseId = Int32.Parse(dt.Rows[0][0].ToString());
                            lbl_case_submit.Text = "Case Submitted Successfully against Case ID : " + caseId.ToString();
                            lbl_case_submit.Visible = true;
                            btn_submit.Enabled = false;
                            //  MessageBox.Show(dt.Rows[0][0].ToString());



                        }
                        catch (Exception ex)
                        {

                            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                   


                    }
                }


            }
        }

        private void btn_browse_Click(object sender, EventArgs e)
        {
        
            using (FileDialog openFileDialog = new OpenFileDialog() { Filter = "All Worksheets|*.xls;*.xlsx;*.csv;|Ms 97-2003|*.xls;|Ms Office 2007|*.xlsx;|CSV file|*.csv;|All Files|*.*" })
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {


                    try
                    {
                        using (var stream = System.IO.File.Open(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                        {
                            using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
                            {
                                DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
                                {
                                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true }
                                });

                                dt_col = result.Tables;
                                //Console.WriteLine(dt_col[2].ToString());
                                /*Getting first sheet from the excel file no need to select
                                 sheet from the combobox*/
                                dtRawContacts = dt_col["Contacts Contacts"];
                                dtRawCalls = dt_col["Calls"];

                                
                                showStandContatcs();
                                showStandCalls();
                       

                            }
                        }

                    }
                    catch (IOException excep)
                    {

                        MessageBox.Show(excep.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }


                }
            }



        }




        private void showStandCalls()
        {
            if (interfaceStandContacts.standCalls(caseId, dtRawCalls).Rows.Count > 0)
            {
                gvCalls.DataSource = interfaceStandContacts.standCalls(caseId, dtRawCalls);
            }
            else
            {
                MessageBox.Show("No Calls Data To Show", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void showStandContatcs()
        {
            if (interfaceStandContacts.standContacts(caseId, dtRawContacts).Rows.Count > 0)
            {
                gvCon.DataSource = interfaceStandContacts.standContacts(caseId, dtRawContacts);
            }
            else
            {
                MessageBox.Show("No Contacts Data To Show", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

       

        private void saveToDataBankContacts()
        {
            pbContacts.Value = 0;
            proglbContacts.Text = "";
            pbContacts.Maximum = dtStandContacts.Rows.Count;
            try
            {
                using (SqlConnection con = new SqlConnection(AppSqlCon.getconsting()))
                {
                    using (SqlBulkCopy objBulk = new SqlBulkCopy(con))
                    {
                        objBulk.BatchSize = dtStandContacts.Rows.Count / 100;
                        objBulk.NotifyAfter = dtStandContacts.Rows.Count / 100;

                        objBulk.SqlRowsCopied += (sender, eventArgs) =>
                        {
                            if ((pbContacts.Maximum - pbContacts.Value) < objBulk.NotifyAfter)
                            {
                                pbContacts.Value = pbContacts.Maximum;
                                //proglbContacts.Text = "Contacts Upload Success!";
                            }
                            else
                            {
                                pbContacts.Value = Convert.ToInt32(eventArgs.RowsCopied);
                            }



                        };

                        // Console.WriteLine("TContacts: {0} PContacts: {1}", stand_contacts_dt.Rows.Count, pbContacts.Value);

                        if (pbContacts.Value == 0)
                        {
                            proglbContacts.Text = "Contacts Upload Success!";

                           /* if (proglbContacts.Text == "Contacts Upload Success!")
                            {
                                stand_contacts_dt.Clear();
                            }*/
                            //pbCalls.Value = 0;
                            //Console.WriteLine("T: {0} P: {1}", stand_calls_dt.Rows.Count, pbCalls.Value);
                        }

                        objBulk.DestinationTableName = "Contacts_db";
                        objBulk.ColumnMappings.Add("Forensic_ID", "Forensic_ID");
                        objBulk.ColumnMappings.Add("Rel_App", "Rel_App");
                        objBulk.ColumnMappings.Add("Fetch_name", "Fetch_name");
                        objBulk.ColumnMappings.Add("Calls", "Calls");
                        objBulk.ColumnMappings.Add("DB", "DB");

                        if (con.State != ConnectionState.Open)
                        {
                            con.Open();
                            try
                            {

                                objBulk.WriteToServer(dtStandContacts);

                            }
                            catch (Exception excep)
                            {
                                MessageBox.Show(excep.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            }

                            finally
                            {

                                con.Close();

                                if (proglbContacts.Text == "Contacts Upload Success!")
                                {
                                    dtStandContacts.Clear();
                                    dtStandCalls.Clear();
                                    caseId = 0;

                                }

                                // pbContacts.Value = 0;

                            }


                        }

                    }
                }
            }catch(Exception excep) {
                MessageBox.Show(excep.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        

        private void Btn_Upload_file_Click(object sender, EventArgs e)
        {
            btn_submit.Enabled = true;
            
            /*int callsCount = 0;*/
            //int contactsCount = 0;
            try 
            {
                dtStandContacts.Merge(dtStandCalls);


                if (dtStandContacts.Rows.Count > 0)
                {
                    /*contactsCount = stand_contacts_dt.Rows.Count;*/

                    if (caseId != 0)
                    {
                        bulkSearchBeforeUpload();
                    }
                    else
                    {
                        MessageBox.Show("Please Update \"Case ID!!\"", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                    }
                    //saveToDataBankContacts();

                }
                else
                {
                    MessageBox.Show("No data to upload!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                /*if (stand_calls_dt.Rows.Count > 0)
                {
                    callsCount = stand_calls_dt.Rows.Count;
                    saveToDataBankCalls();
                }*/

                /*if(callsCount == 0)
                {
                    MessageBox.Show("No data to upload in calls table!");
                }
                if(contactsCount == 0)
                {
                    MessageBox.Show("No data to upload in contacts table!");
                }*/

            }
            catch(Exception excep)
            {
                    MessageBox.Show(excep.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void bulkSearchBeforeUpload()
        {
            List<string> allContactsList = new List<string>();

            List<StandContactsModel> stand_Contacts= new List<StandContactsModel>();

            stand_Contacts.AddRange(listRawContacts);
            stand_Contacts.AddRange(list_unique_calls);

            DataTable contactTable = new DataTable();
            DataTable contactsFound = new DataTable();

            contactTable.Columns.Add("Contacts", typeof(string));

            foreach(var contact in stand_Contacts)
            {
                if (contact.Calls != "" && contact.Calls != "300" && contact.Calls != "15"
                   && contact.Calls != "16" && contact.Calls != "115")
                {
                    contactTable.Rows.Add(contact.Calls);
                }
            }

            string connection = AppSqlCon.getconsting();

            using(SqlConnection con = new SqlConnection(connection))
            {
                using(SqlCommand cmd = new SqlCommand("get_contactsinfo", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MSISDN", contactTable);

                    if(con.State != ConnectionState.Open)
                        con.Open();

                    DataTable matchFound = new DataTable();

                    SqlDataReader sdr = cmd.ExecuteReader();

                    matchFound.Load(sdr);

                    if (matchFound.Rows.Count > 0)
                    {
                        new MatchFoundForm(matchFound).ShowDialog();
                       
                    }


                   

                }
            }

            saveToDataBankContacts();
            /* Console.WriteLine("Contacts Count: {0} Calls Count: {1}  All Count: {2}", list_contact.Count, list_unique_calls.Count, stand_Contacts.Count);*/

        }

        private void gv_social_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = gv_social.Rows[e.RowIndex];
                //forensic_ID = Convert.ToInt32(row.Cells[3].Value.ToString());
                //100015041264190
                social_ID = row.Cells[3].Value.ToString();
                MessageBox.Show("Social ID: " + "\"" + social_ID + "\"" + " selected for image upload", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
               
            }
        }


        public void insertImage(string socialUrl, byte[] image)
        {
            try
            {
                string connection = AppSqlCon.getconsting();
                using(SqlConnection con = new SqlConnection(connection))
                {
                    using(SqlCommand  cmd  = new SqlCommand("sp_insertsocialdata", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (isFormValid(social_ID, socialUrl, image))
                        {
                            cmd.Parameters.AddWithValue("@SocialID", social_ID);
                            cmd.Parameters.AddWithValue("@SocialURL", txtSocialURL.Text.Trim());
                            cmd.Parameters.AddWithValue("@SocialImage", image);

                            if (con.State != ConnectionState.Open)
                            {
                                con.Open();
                            }

                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Updated Successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                image = null;
                                txtSocialURL.Text = "";
                                picBoxSocial.Image = null;
                                social_ID = "";
                            }
                            else
                            {
                                MessageBox.Show("Some Error Occur", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }

            }catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); 
            }
        }

        private bool isFormValid(string social_ID, string socialUrl, byte[] image)
        {
            var valid = true;
            if (txtSocialURL.Text == "")
            {
                errorSocialUrlProvider.SetError(txtSocialURL, "Please Enter Url");
                valid= false;
            }
            
            else if (social_ID == "" || social_ID == null)
            {
                MessageBox.Show("Pleaase Select Social ID", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                valid = false;
            }
            else if(picBoxSocial.Image == null)
            {
                MessageBox.Show("Pleaase Select Image to upload", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                valid = false;
            }

            else
            {
                errorSocialUrlProvider.Clear();

            }


            return valid;
        }

        byte[] convertImageToBytes(System.Drawing.Image img)
        {
            using(MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }

        public System.Drawing.Image convertBytesArrayToImage(byte[] data)
        {
            using(MemoryStream ms = new MemoryStream(data))
            {
                return System.Drawing.Image.FromStream(ms);
            }
        }

        private void picBoxSocial_Click(object sender, EventArgs e)
        {
            // open file dialog   
            //OpenFileDialog open = new OpenFileDialog();
           
            
            using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "Image Files(*.jpg; *.jpeg;)|*.jpg; *.jpeg;", Multiselect =false })
            {
               
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    picBoxSocial.Image = System.Drawing.Image.FromFile(ofd.FileName);

                    insertImage(txtSocialURL.Text, convertImageToBytes(picBoxSocial.Image));
                }
            }
        }

        private void btnCreateRecord_Click(object sender, EventArgs e)
        {
            bunifuPages1.SetPage("tabPage1");
        }

        private void btnSocial_Click(object sender, EventArgs e)
        {
            bunifuPages1.SetPage("tabPage4"); 
        }

        
        private void btnViewRecord_Click(object sender, EventArgs e)
        {
            bunifuPages1.SetPage("tabPage2");
            timer.Start();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            //"sp_searchForensicID"
            if (searchByCaseID)
            {
                DataTable searchResultsDt = new DataTable();
                searchResultsDt.Merge(interfaceSearchData.searchData("sp_searchForensicID", txtSearch.Text.Trim()));

                if (searchResultsDt.Rows.Count > 0)
                {
                    gvSearchResults.DataSource = searchResultsDt;
                }
                
            }
            else if(searchByOtherOptions)
            {
                string[] storedProc = { "sp_searchAnyID", "sp_searchFetchName", "sp_searchRelApp" };

                DataTable searchResultsDt = new DataTable();
                foreach (var sp in storedProc)
                {
                    searchResultsDt.Merge(interfaceSearchData.searchData(sp, txtSearch.Text.Trim()));
                }


                if (searchResultsDt.Rows.Count > 0)
                {
                    gvSearchResults.DataSource = searchResultsDt;
                }
               
            }

        }


        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSearch.PerformClick();
            }
        }

        /*private void txtSearch_TextChange(object sender, EventArgs e)
        {
            btnSearch.PerformClick();
        }*/

        private void timer_Tick(object sender, EventArgs e)
        {
            if(isCollapsed)
            {
                btnViewRecord.IdleIconRightImage = Resources.collapse_arrow;
                panelDropDown.Height += 10;
                if(panelDropDown.Size == panelDropDown.MaximumSize) { 
                    timer.Stop();
                    isCollapsed = false;
                }

            }
            else
            {
                btnViewRecord.IdleIconRightImage = Resources.expand_arrow;
                panelDropDown.Height -= 10;
                if (panelDropDown.Size == panelDropDown.MinimumSize)
                {
                    timer.Stop();
                    isCollapsed = true;
                }
            }
        }

        private void btnCaseIDSearch_Click(object sender, EventArgs e)
        {
            searchByCaseID = true;
            searchByOtherOptions = false;
        }

        private void btnOtherSearch_Click(object sender, EventArgs e)
        {
            searchByCaseID = false;
            searchByOtherOptions = true;
        }

        private void txtAnyID_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            btnSearchAnyID.PerformClick();
        }

        private void btnSearchAnyID_Click(object sender, EventArgs e)
        {

            try
            {
                string connection = AppSqlCon.getconsting();

                using (SqlConnection con = new SqlConnection(connection))
                {

                    using (SqlCommand cmd = new SqlCommand("sp_searchAnyID", con))
                    {

                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@AnyID", txtAnyID.Text.Trim());

                        if (con.State != ConnectionState.Open)
                        {
                            con.Open();
                        }

                        DataTable td_biodata = new DataTable();
                        SqlDataReader sdr = cmd.ExecuteReader();
                        td_biodata.Load(sdr);

                        if (td_biodata.Rows.Count > 0)
                        {
                            gv_social.DataSource = td_biodata;
                        }
                        else
                        {
                            MessageBox.Show("Some Error occur \"No Data to show\""
                                , "Information"
                                , MessageBoxButtons.OK
                                , MessageBoxIcon.Information);

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBulkSearch_Click(object sender, EventArgs e)
        {
            using (FileDialog openFileDialog = new OpenFileDialog() { Filter = "All Worksheets|*.xls;*.xlsx;*.csv;|Ms 97-2003|*.xls;|Ms Office 2007|*.xlsx;|CSV file|*.csv;|All Files|*.*" })
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {


                    try
                    {
                        using (var stream = System.IO.File.Open(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                        {
                            using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
                            {
                                DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
                                {
                                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true }
                                });

                                dt_col = result.Tables;
                                //Console.WriteLine(dt_col[2].ToString());
                                /*Getting first sheet from the excel file no need to select
                                 sheet from the combobox*/
                                dtRawContacts = dt_col[0];

                                standBulkContatcs();
                                //stand_calls();


                            }
                        }

                    }
                    catch (IOException excep)
                    {

                        MessageBox.Show(excep.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }


                }
            }
        }

        private void standBulkContatcs()
        {
            try
            {

                if (dtRawContacts != null)
                {
                    DataTable bulkSearchDT = new DataTable();
                    bulkSearchDT.Columns.Add("MSISDN", typeof(string));
                    
                    List<string> uniqueNumList = dtRawContacts.AsEnumerable().Select(x => x[0].ToString()).Distinct().ToList();
                    foreach (var num in uniqueNumList)
                    {
                       
                        bulkSearchDT.Rows.Add(interfaceStandContacts.standerizeRawData(num));
                        
                    }

                    string connection = AppSqlCon.getconsting();

                    using (SqlConnection con = new SqlConnection(connection))
                    {
                        using (SqlCommand cmd = new SqlCommand("get_contactsinfo", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@MSISDN", bulkSearchDT);

                            if (con.State != ConnectionState.Open)
                                con.Open();

                            DataTable contactsFound = new DataTable();


                            SqlDataReader sdr = cmd.ExecuteReader();

                            contactsFound.Load(sdr);

                            //Console.WriteLine(contactsFound.Rows.Count);    

                            if (contactsFound.Rows.Count > 0)
                                gvSearchResults.DataSource = contactsFound;
                            var mh = MessageBox.Show($"{contactsFound.Rows.Count} Match found "
                                , "Information"
                                , MessageBoxButtons.OK
                                , MessageBoxIcon.Information);

                        }
                    }


                }
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void btnViewCase_Click(object sender, EventArgs e)
        {
            bunifuPages1.SetPage("tabPage3");
 
        }

        private void btnIDSearch_Click(object sender, EventArgs e)
        {
            try
            {
                var txtToSearch = txtIDToSearch.Text.Trim();
                if (txtToSearch != ""
                && txtToSearch.All(char.IsDigit)) {
                    string connection = AppSqlCon.getconsting();

                    using (SqlConnection con = new SqlConnection(connection))
                    {

                        using (SqlCommand cmd = new SqlCommand("sp_search_cassID", con))
                        {

                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@AnyID", txtIDToSearch.Text.Trim());

                            if (con.State != ConnectionState.Open)
                            {
                                con.Open();
                            }

                            DataTable td_biodata = new DataTable();
                            SqlDataReader sdr = cmd.ExecuteReader();
                            td_biodata.Load(sdr);

                            if (td_biodata.Rows.Count > 0)
                            {
                                gvCases.DataSource = td_biodata;
                            }
                            else
                            {
                                MessageBox.Show("Some Error occur \"No Data to show\""
                                    , "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }

                    }
                }

                else
                {
                    MessageBox.Show("Please Enter Correct Case ID", "Error"
                        , MessageBoxButtons.OK
                        , MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtIDToSearch_KeyDown(object sender, KeyEventArgs e)
        {
            var txtToSearch = txtIDToSearch.Text.Trim();
            if (e.KeyCode == Keys.Enter)
            {
                if (txtToSearch != ""
                && txtToSearch.All(char.IsDigit))
                {
                    btnIDSearch.PerformClick();
                }
                else
                {
                    MessageBox.Show("Please Enter Correct Case ID", "Error"
                        , MessageBoxButtons.OK
                        , MessageBoxIcon.Error);
                }
            }
            
        }


        private void gvCases_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = gvCases.Rows[e.RowIndex];
                //forensic_ID = Convert.ToInt32(row.Cells[3].Value.ToString());
                //100015041264190
                var case_ID = Convert.ToInt32(row.Cells[0].Value.ToString());
                string message = "Are you sure to delete Case: " + case_ID;
                string title = "Delete Case";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                var result = MessageBox.Show(message,title , buttons, MessageBoxIcon.Information);


                // If the no button was pressed ...
                if (result == DialogResult.Yes)
                {
                    interfaceDeleteCase.deletCase(case_ID);
                    
                }
                

            }
        }

        private void gvSearchResults_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    DataGridViewRow row = gvSearchResults.Rows[e.RowIndex];

                    var image = convertBytesArrayToImage((byte[])row.Cells[5].Value);

                    new ImageForm(image).ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No Image To Show", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
