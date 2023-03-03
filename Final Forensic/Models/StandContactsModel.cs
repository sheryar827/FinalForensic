using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_Forensic.Classes_fore
{
    public class StandContactsModel
    {
        public int Forensic_ID { get; set; }
        public string Rel_App { get; set; }
        public string Fetch_name { get; set; }
        public string Calls { get; set; }

        public string DB { get; set; }

        public StandContactsModel() { }

        public StandContactsModel(int forensic_id, string rel_app, string fetch_name, string calls, string db)
        {
            this.Forensic_ID = forensic_id;
            this.Rel_App = rel_app;
            this.Fetch_name= fetch_name;
            this.Calls = calls;
            this.DB = db;
        }

            
    }
}
