using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Final_Forensic
{
    public partial class MatchFoundForm : Form
    {
        public MatchFoundForm(DataTable matchFound)
        {
            
            InitializeComponent();
            gvMatchFound.DataSource = matchFound;
        }

        public Image convertBytesArrayToImage(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                return Image.FromStream(ms);
            }
        }

        private void gvMatchFound_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    DataGridViewRow row = gvMatchFound.Rows[e.RowIndex];

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
