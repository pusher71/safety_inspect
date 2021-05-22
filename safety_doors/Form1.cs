using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace safety_doors
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private List<Position> readPCD()
        {
            char[] sep = new char[1] { ' ' };
            List<Position> pos = new List<Position>();
            StreamReader sr = new StreamReader(textBox1.Text);
            for (int i = 0; i < 11; i++)
                sr.ReadLine();
            while (!sr.EndOfStream)
            {
                string[] strs = sr.ReadLine().Replace('.', ',').Split(sep);
                double x = double.Parse(strs[0]);
                double y = double.Parse(strs[1]);
                double z = double.Parse(strs[2]);
                pos.Add(new Position(x, y, z));
            }
            return pos;
        }

        //вид сверху с прямыми
        private void button4_Click(object sender, EventArgs e)
        {
            List<Position> list = readPCD();
            Bitmap image = new Bitmap(4000, 4000);
            for (int i = 0; i < list.Count; i++)
            {
                Position current = list[i];
                image.SetPixel((int)((current.x + 2) * 1000), (int)((current.y + 2) * 1000), Color.FromArgb(0, 0, (int)(current.z * 32)));
            }
            Graphics g1 = Graphics.FromImage(image);
            Pen pen = new Pen(Color.Red, 5);
            g1.DrawLine(pen, 0, (float)y(-2) * 1000 + 2000, 4000, (float)y(2) * 1000 + 2000);
            g1.DrawLine(pen, 0, (float)y1(-2) * 1000 + 2000, 4000, (float)y1(2) * 1000 + 2000);
            g1.DrawLine(pen, 0, (float)y2(-2) * 1000 + 2000, 4000, (float)y2(2) * 1000 + 2000);
            image.Save(textBox1.Text.Replace(".pcd", "_LINES.png"));
        }

        //определить безопасность
        private void button3_Click(object sender, EventArgs e)
        {
            List<Position> list = readPCD();

            const int minCountDetected = 40; //минимальное количество для утверждения опасности
            int countDetected = 0; //количество точек в плоскости
            for (int i = 0; i < list.Count; i++)
                if (inspect(list[i]))
                    countDetected++;

            bool safety = countDetected < minCountDetected;
            label2.Text = safety ? "Безопасно" : "Опасно";
            label2.ForeColor = safety ? Color.Lime : Color.Red;
        }

        private const double baseOffset = 0.28; //смещение плоскости от вертикальной оси
        private const double inspectWeight = 0.04; //толщина плоскости

        //прямая - центр плоскости
        private double y(double x)
        {
            return -x / 12.4 - baseOffset;
        }

        //прямая - нижняя граница плоскости по Y
        private double y1(double x)
        {
            return -x / 12.4 - baseOffset - inspectWeight / 2;
        }

        //прямая - верхняя граница плоскости по Y
        private double y2(double x)
        {
            return -x / 12.4 - baseOffset + inspectWeight / 2;
        }

        //точка находится внутри инспектируемой плоскости
        private bool inspect(Position pos)
        {
            return pos.y > y1(pos.x) && pos.y < y2(pos.x);
        }
    }

    //3D точка
    public class Position
    {
        public double x;
        public double y;
        public double z;

        public Position(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}
