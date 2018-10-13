using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Monte_Carlo_PI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ulong update_count = 10000;
            int MAX_SIZE = 5000;
            Random rnd = new Random();
            Bitmap bmp = new Bitmap(MAX_SIZE, MAX_SIZE);
            uint x = 0;
            uint y = 0;
            ulong dx = 0;
            ulong dy = 0;
            ulong count = (ulong)MAX_SIZE * update_count;
            ulong insideCount = 0;
            ulong outsideCount = 0;
            pictureBox1.Image = bmp;

            while (count > 0)
            {
                count--;
                x = (uint)rnd.Next(MAX_SIZE);
                y = (uint)rnd.Next(MAX_SIZE);
                dx = x;
                dy = y;

                //Debug.WriteLine(count);

                if ((dx * dx + dy * dy) > (ulong)(MAX_SIZE * MAX_SIZE))
                {
                    bmp.SetPixel((int)x, (int)y, Color.Red);
                    insideCount++;
                }
                else
                {
                    bmp.SetPixel((int)x, (int)y, Color.Gray);
                    outsideCount++;
                }

                if (count % update_count == 0)
                {
                    label1.Text = string.Format("{0}", count);
                    label2.Text = string.Format("{0}", insideCount);
                    label3.Text = string.Format("{0}", outsideCount);
                    label4.Text = string.Format("{0:0.0000000000}", (decimal)outsideCount / ((decimal)insideCount + (decimal)outsideCount) * 4);
                    pictureBox1.Image = bmp;
                    System.Windows.Forms.Application.DoEvents();
                }
            }
        }
    }
}
