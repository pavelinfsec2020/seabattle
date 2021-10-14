using System;
using System.Windows.Forms;

namespace SeaBattle.Viewers
{
    public partial class BattleViewer : Form
    {
        public BattleViewer()
        {
            InitializeComponent();
        }
        [STAThread]
        private void BattleViewer_Load(object sender, EventArgs e)
        {
            dataGridView1.RowTemplate.Height = 35;
            dataGridView2.RowTemplate.Height = 35;
            for (int i = 0; i < 10; i++)
            {
                dataGridView1.Rows.Add();
                dataGridView2.Rows.Add();
            }
            dataGridView1.Rows[0].Cells[0].Selected = false;
            dataGridView2.Rows[0].Cells[0].Selected = false;    
        }

    }
}
