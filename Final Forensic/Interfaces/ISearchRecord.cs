using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_Forensic.Interfaces
{
    interface ISearchRecord
    {
        DataTable searchData(string procName, string valueToSearch);
    }
}
