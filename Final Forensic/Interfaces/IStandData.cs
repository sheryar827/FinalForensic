using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_Forensic.Interfaces
{
    interface IStandData
    {
        DataTable standContacts(int caseId , DataTable rawContacts);
        DataTable standCalls(int caseId , DataTable rawCalls);

        string standerizeRawData(string msisdn);
    }
}
