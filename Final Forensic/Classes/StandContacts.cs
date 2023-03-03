using Final_Forensic.Classes_fore;
using Final_Forensic.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Final_Forensic.Classes
{
    class StandContacts : IStandData
    {
        public DataTable standCalls(int caseId, DataTable dtRawCalls)
        {
            var dtStandCalls = new DataTable();
            try
            {
                
                //List<string>
                if (dtRawCalls != null)
                {
                    var listRawCalls = new List<StandContactsModel>();
                    for (int i = 0; i < dtRawCalls.Rows.Count; i++)
                    {
                        StandContactsModel sc = new StandContactsModel();
                        sc.Forensic_ID = caseId;
                        sc.Rel_App = dtRawCalls.Rows[i]["Related Application"].ToString();
                        if (dtRawCalls.Rows[i]["To"].ToString() != "")
                        {
                            sc.Calls = standerizeRawData(Regex.Match(dtRawCalls.Rows[i]["To"].ToString(), @"\d+(?!\D*\d)").Value.Trim());
                        }

                        sc.DB = "CALLS";

                        if (sc.Calls != null && sc.Calls.Length > 7)
                        {
                            listRawCalls.Add(sc);
                        }
                        //Console.WriteLine(sc.Call_To);
                    }

                    for (int i = 0; i < dtRawCalls.Rows.Count; i++)
                    {
                        StandContactsModel sc = new StandContactsModel();
                        sc.Forensic_ID = caseId;
                        sc.Rel_App = dtRawCalls.Rows[i]["Related Application"].ToString();
                        if (dtRawCalls.Rows[i]["From"].ToString() != "")
                        {
                            sc.Calls = standerizeRawData(Regex.Match(dtRawCalls.Rows[i]["From"].ToString(), @"\d+(?!\D*\d)").Value.Trim());
                        }

                        sc.DB = "CALLS";

                        if (sc.Calls != null && sc.Calls.Length > 7)
                        {
                            listRawCalls.Add(sc);
                        }

                    }

                    //make unique list
                    var listUniqueCalls = listRawCalls.GroupBy(i => i.Calls).Select(i => i.First()).ToList();
                    /*List<Stand_contacts> uniqueList = list_calls.GroupBy(i => i.Calls).Select(i => i.First()).ToList();*/

                    //remove empty strings from list
                    listUniqueCalls = listUniqueCalls.Where(s => !string.IsNullOrWhiteSpace(s.Calls)).Distinct().ToList();

                    ListtoDataTable ltdb = new ListtoDataTable();
                    dtStandCalls = ltdb.ToDataTable(listUniqueCalls);

                    
                    //gvCalls.DataSource = dtStandCalls;


                }

            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            return dtStandCalls;

        }

        public DataTable standContacts(int caseId, DataTable dtRawContacts)
        {
            DataTable dtStandContacts = new DataTable();
            try
            {

                if (dtRawContacts != null)
                {
                    var listRawContacts = new List<StandContactsModel>();


                    for (int i = 0; i < dtRawContacts.Rows.Count; i++)
                    {
                        StandContactsModel sc = new StandContactsModel();
                        sc.Forensic_ID = caseId;
                        sc.Rel_App = dtRawContacts.Rows[i]["Related Application"].ToString();
                        if (dtRawContacts.Rows[i]["Display Name"].ToString() != "")
                        {
                            sc.Fetch_name = dtRawContacts.Rows[i]["Name"].ToString() + " / " + dtRawContacts.Rows[i]["Display Name"].ToString();
                        }
                        else
                        {
                            sc.Fetch_name = dtRawContacts.Rows[i]["Name"].ToString();
                        }


                        sc.Calls = standerizeRawData(dtRawContacts.Rows[i]["Tel"].ToString().Replace(" ", "")).Length > 7 ? standerizeRawData(dtRawContacts.Rows[i]["Tel"].ToString().Replace(" ", "")) : "";

                        var list_of_contact_with_semicolon = new List<StandContactsModel>();


                        sc.DB = "CONTACTS";

                        if (sc.Calls.Contains(';'))
                        {
                            string[] numArray = sc.Calls.Split(';');
                            foreach (var num in numArray)
                            {
                                if (sc.Fetch_name != "" || num.Length > 7)
                                {

                                    var standnum = standerizeRawData(Regex.Match(num, @"\d+(?!\D*\d)").Value.Trim());
                                    sc.Fetch_name = new String(sc.Fetch_name.Where(Char.IsLetter).ToArray());
                                    Console.WriteLine(sc.Fetch_name);
                                    list_of_contact_with_semicolon.Add(new StandContactsModel(sc.Forensic_ID
                                    , sc.Rel_App, sc.Fetch_name, standnum, sc.DB));
                                }
                            }

                            listRawContacts.AddRange(list_of_contact_with_semicolon);
                        }
                        else if (sc.Fetch_name != "" || sc.Calls.Length > 7)
                        {
                            sc.Fetch_name = new String(sc.Fetch_name.Where(Char.IsLetter).ToArray());
                            Console.WriteLine(sc.Fetch_name);
                            sc.Calls = standerizeRawData(Regex.Match(sc.Calls, @"\d+(?!\D*\d)").Value.Trim());
                            listRawContacts.Add(sc);
                        }

                        //Console.WriteLine(sc.Telphone);
                    }

                    ListtoDataTable ltdb = new ListtoDataTable();
                    dtStandContacts = ltdb.ToDataTable(listRawContacts);
                    //gvCon.DataSource = dtStandContacts;
                    

                }
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            return dtStandContacts;
        }

        // by default private
        public string standerizeRawData(string msisdn)
        {
            string nsmsisdn = msisdn;
            /*if(msisdn.Length >= 10 && msisdn.Length <= 12 && msisdn.All(char.IsDigit)) 
            {
                msisdn = msisdn.Substring(msisdn.Length - 10);
                msisdn = $"92{msisdn}";
            }*/

            if (msisdn.Length == 14 && msisdn.StartsWith("64") && msisdn.EndsWith("7"))
            {
                msisdn = msisdn.Substring(msisdn.Length - 11);

                msisdn = msisdn.Substring(0, 10);

                msisdn = $"92{msisdn}";
            }


            if (msisdn.Length > 11 && msisdn.StartsWith("00"))
            {
                int j = 1;
                for (int i = 0; i <= 1; i++)
                {
                    if (msisdn[i] == '0' && msisdn.Length > 10)
                    {
                        msisdn = msisdn.Remove(i, ++j);
                    }
                }

            }


            if (msisdn.Length >= 10 && msisdn.IndexOf('0') == 0)
            {
                msisdn = msisdn.Remove(0, 1);
                msisdn = $"92{msisdn}";
            }

            /* if (msisdn.IndexOf('3') == 0)
             {
                 msisdn = $"92{msisdn}";
             }*/

            //Console.WriteLine("{0}, {1}",nsmsisdn,msisdn);
            return msisdn;

        }

    }
}
