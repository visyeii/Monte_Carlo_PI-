using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
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
            ulong update_count = 1000;
            int MAX_SIZE = 2500;
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
                    outsideCount++;
                }
                else
                {
                    bmp.SetPixel((int)x, (int)y, Color.Gray);
                    insideCount++;
                }

                if (count % update_count == 0)
                {
                    label1.Text = string.Format("{0}", count);
                    label2.Text = string.Format("{0}", insideCount);
                    label3.Text = string.Format("{0}", outsideCount);
                    label4.Text = string.Format("{0:0.0000000000}", (decimal)insideCount / ((decimal)insideCount + (decimal)outsideCount) * 4);
                    pictureBox1.Image = bmp;
                    Application.DoEvents();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Btn_Click(sender, e);
        }

        public class MCInfo
        {
            public List<MCPixcel> Pixcels { get; set; } = new List<MCPixcel>();
            public List<string> StrList { get; set; } = new List<string>();
            public ulong InsideCount { get; set; } = 0;
            public ulong OutsideCount { get; set; } = 0;
        }

        public class MCPixcel
        {
            public MCPixcel(uint _x, uint _y, Color _color)
            {
                x = _x;
                y = _y;
                color = _color;

            }
            public uint x { get; set; } = 0;
            public uint y { get; set; } = 0;
            public Color color { get; set; } = Color.Black;
            

        }

        async private void Btn_Click(object sender, EventArgs e)
        {
            int MAX_SIZE = 2500;
            ulong update_count = 10;
            Bitmap bmp = new Bitmap(MAX_SIZE, MAX_SIZE, PixelFormat.Format24bppRgb);
            int loop_count = 100;
            ulong insideCountSum = 0;
            ulong outsideCountSum = 0;

            while (loop_count > 0)
            {
                loop_count--;

                MCInfo inf = await Task.Run(() =>
                {
                    var mcInfo = new MCInfo();
                    Random rnd = new Random();
                    ulong count = (ulong)MAX_SIZE * update_count;

                    while (count > 0)
                    {
                        count--;
                        uint x = (uint)rnd.Next(MAX_SIZE);
                        uint y = (uint)rnd.Next(MAX_SIZE);
                        ulong dx = x;
                        ulong dy = y;

                        if ((dx * dx + dy * dy) > (ulong)(MAX_SIZE * MAX_SIZE))
                        {
                            mcInfo.Pixcels.Add(new MCPixcel(x, y, Color.Red));
                            mcInfo.OutsideCount++;
                        }
                        else
                        {
                            mcInfo.Pixcels.Add(new MCPixcel(x, y, Color.Gray));
                            mcInfo.InsideCount++;
                        }
                    }

                    return mcInfo;
                });

                BitmapData lockedBmp = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);

                byte[] bmpPixcels = new byte[lockedBmp.Stride * lockedBmp.Height];
                Marshal.Copy(lockedBmp.Scan0, bmpPixcels, 0, bmpPixcels.Length);

                //foreach (var px in inf.Pixcels)
                //{
                //    bmpPixcels[px.x * 3 + px.y * lockedBmp.Stride + 0] = px.color.B;
                //    bmpPixcels[px.x * 3 + px.y * lockedBmp.Stride + 1] = px.color.G;
                //    bmpPixcels[px.x * 3 + px.y * lockedBmp.Stride + 2] = px.color.R;
                //}

                Parallel.ForEach(inf.Pixcels, px =>
                {
                    bmpPixcels[px.x * 3 + px.y * lockedBmp.Stride + 0] = px.color.B;
                    bmpPixcels[px.x * 3 + px.y * lockedBmp.Stride + 1] = px.color.G;
                    bmpPixcels[px.x * 3 + px.y * lockedBmp.Stride + 2] = px.color.R;
                });

                Marshal.Copy(bmpPixcels, 0, lockedBmp.Scan0, bmpPixcels.Length);
                bmp.UnlockBits(lockedBmp);

                pictureBox1.Image = bmp;
                insideCountSum += inf.InsideCount;
                outsideCountSum += inf.OutsideCount;

                label1.Text = string.Format("{0}", (ulong)MAX_SIZE * (ulong)update_count * (ulong)loop_count);
                label2.Text = string.Format("{0}", insideCountSum);
                label3.Text = string.Format("{0}", outsideCountSum);
                label4.Text = string.Format("{0:0.0000000000}", (decimal)insideCountSum / ((decimal)insideCountSum + (decimal)outsideCountSum) * 4);

                Application.DoEvents();
            }
        }
    }
}
