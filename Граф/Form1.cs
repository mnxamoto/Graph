using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Граф
{
    public partial class Form1 : Form
    {
        int[] data;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string[] dataS = textBox2.Text.Split(',');
            int countData = dataS.Length;
            data = new int[countData];

            for (int i = 0; i < countData; i++)
            {
                data[i] = Convert.ToInt32(dataS[i]);
            }

            Graph graph = new Graph();
            graph.draw(data, dataGridView12, dataGridView1, pictureBox1);
        }

        private void button9_Click(object sender, EventArgs e)
        {

        }

        private void tabPage4_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image.Save(saveFileDialog1.FileName + ".png");
            }
        }
    }
}
