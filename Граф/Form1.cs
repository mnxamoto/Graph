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
        int[] dataDistinct;
        int[] vesaNodes;
        int[][] vesaEdges;

        struct Node
        {
            public int name;
            public Point coordinate;
            public int value;
            public int weight;
        }

        struct Edge
        {
            public Node start;
            public Node end;
            public int value;
            public int weight;
        }

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

            //Подсчёт количества повторений (весов вершин)
            dataDistinct = data.Distinct().ToArray();
            Array.Sort(dataDistinct);
            int countDataDistinct = dataDistinct.Length;
            vesaNodes = new int[countDataDistinct];

            for (int i = 0; i < countData; i++)
            {
                for (int k = 0; k < countDataDistinct; k++)
                {
                    if (data[i] == dataDistinct[k])
                    {
                        vesaNodes[k]++;
                        break;
                    }
                }
            }

            //Вывод
            dataGridView12.Rows.Clear();

            for (int k = 0; k < countDataDistinct; k++)
            {
                string[] row = new string[2];
                row[0] = dataDistinct[k].ToString();
                row[1] = vesaNodes[k].ToString();

                dataGridView12.Rows.Add(row);
            }

            //Подсчёт количества переходов (весов рёбер)
            vesaEdges = new int[countDataDistinct][];

            for (int i = 0; i < countDataDistinct; i++)
            {
                vesaEdges[i] = new int[countDataDistinct];
            }

            for (int i = 0; i < countData - 1; i++)
            {
                int x = 0;
                int y = 0;

                for (int k = 0; k < countDataDistinct; k++)
                {
                    if (data[i] == dataDistinct[k])
                    {
                        x = k;
                    }

                    if (data[i + 1] == dataDistinct[k])
                    {
                        y = k;
                    }
                }

                vesaEdges[x][y]++;
            }

            //Вывод
            dataGridView1.Columns.Clear();

            for (int i = 0; i < countDataDistinct; i++)
            {
                dataGridView1.Columns.Add("Columns_1_" + i, i.ToString());
                dataGridView1.Rows.Add();
                dataGridView1.Rows[i].HeaderCell.Value = i.ToString();
            }

            for (int i = 0; i < countDataDistinct; i++)
            {
                for (int k = 0; k < countDataDistinct; k++)
                {
                    dataGridView1.Rows[i].Cells[k].Value = vesaEdges[i][k];
                }
            }

        }

        private void button9_Click(object sender, EventArgs e)
        {
            int width = 1400;
            int height = 1400;
            pictureBox1.Width = width;
            pictureBox1.Height = height;

            Bitmap bmp = new Bitmap(width, height);
            pictureBox1.Image = bmp;
            Graphics g = Graphics.FromImage(pictureBox1.Image);

            Pen blackPen = new Pen(Color.Black);

            int countDataDistinct = dataDistinct.Length;  //Количество узлов

            Point centerImage = new Point(700, 700);  //Координаты центар изображения
            int rPolygon = 400;  //Как далеко от центра расположены узыл
            Point[] coordinatesNodes = DrawHelper.coordinatesPolygon(centerImage, rPolygon, countDataDistinct);

            Font font = new Font("Times New Roman", 14);
            SolidBrush brush = new SolidBrush(Color.Black);

            StringFormat format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Center;

            //Отрисовка узлов
            Node[] nodes = new Node[countDataDistinct];

            for (int i = 0; i < countDataDistinct; i++)
            {
                Node node = new Node();
                node.name = dataDistinct[i];
                node.coordinate = coordinatesNodes[i];
                node.value = vesaNodes[i];
                node.weight = Convert.ToInt32((node.value / (double) vesaNodes.Max()) * 100) + 20;  //+15 - мин размер (раудиус) узла

                g.DrawEllipse(blackPen, DrawHelper.rectangleFromCircle(node.coordinate, node.weight));
                g.DrawString($"{node.name}\r\n[{node.value}]", font, brush, node.coordinate, format);

                nodes[i] = node;
            }

            //Отрисовка рёбер
            Edge[] edges = new Edge[countDataDistinct * countDataDistinct];
            int maxVesaEdges = 0;

            for (int i = 0; i < countDataDistinct; i++)
            {
                int iMax = vesaEdges[i].Max();

                if (iMax > maxVesaEdges)
                {
                    maxVesaEdges = iMax;
                }
            }

            for (int i = 0; i < countDataDistinct; i++)
            {
                for (int k = 0; k < countDataDistinct; k++)
                {
                    Edge edge = new Edge();
                    edge.start = nodes[i];
                    edge.end = nodes[k];
                    edge.value = vesaEdges[i][k];
                    edge.weight = Convert.ToInt32((edge.value / (double)maxVesaEdges) * 10);

                    if (edge.value > 0)
                    {
                        if (i == k)
                        {
                            drawLoop(g, centerImage, edge);
                        }
                        else
                        {
                            drawPointer(g, edge);
                        }
                    }

                    edges[i * countDataDistinct + k] = edge;
                }
            }
        }

        private void drawLoop(Graphics g, Point centerImage, Edge edge)
        {
            int R = edge.start.weight;
            Point centerNode = edge.start.coordinate;

            Point[] p = new Point[4];

            int distance = Convert.ToInt32(DrawHelper.distanceBetweenTwoPoints(centerImage, centerNode)) + R;
            p[0] = DrawHelper.сoordinatesPointOnLine(centerImage, centerNode, distance);

            distance = R * 3;
            Point rLoop = DrawHelper.сoordinatesPointOnLine(centerNode, p[0], distance);

            distance = R;
            p[1] = DrawHelper.perpendicularToLine(p[0], rLoop, rLoop, -distance);
            p[2] = DrawHelper.perpendicularToLine(p[0], rLoop, rLoop, distance);
            p[3] = p[0];

            Pen pen = new Pen(Color.Black);
            pen.CustomEndCap = new AdjustableArrowCap(5, 5);
            pen.Width = edge.weight;

            g.DrawBeziers(pen, p);
            //g.DrawCurve(pen, p);

            //Добавить надпись - вес ребра
            Font font = new Font("Times New Roman", 14);
            SolidBrush brush = new SolidBrush(Color.Red);

            StringFormat format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Center;

            Point cocoordinateText = DrawHelper.perpendicularToLine(p[0], rLoop, DrawHelper.сoordinatesPointOnLine(p[0], rLoop, R / 2), R / 2);

            g.DrawString(edge.value.ToString(), font, brush, cocoordinateText, format);
        }

        private void drawPointer(Graphics g, Edge edge)
        {
            Point p1 = edge.start.coordinate;
            Point p2 = edge.end.coordinate;
            int R1 = edge.start.weight;
            int R2 = edge.end.weight;
            int widthPen = edge.weight;
            string text = edge.value.ToString();

            Point p3 = DrawHelper.сoordinatesPointOnLine(p1, p2, R1);
            Point p4 = DrawHelper.сoordinatesPointOnLine(p2, p1, R2);

            drawPointer(g, p3, p4, widthPen, text);
        }

        private void drawPointer(Graphics g, Point p1, Point p2, float widthPen, string text)
        {
            Pen pen = new Pen(Color.Black);
            pen.CustomEndCap = new AdjustableArrowCap(5, 5);
            pen.Width = widthPen;

            double R = DrawHelper.distanceBetweenTwoPoints(p1, p2) / 8;
            Point center = DrawHelper.center2point(p1, p2);

            Point[] p = new Point[4];
            p[0] = p1;
            p[1] = DrawHelper.perpendicularToLine(p1, p2, DrawHelper.center2point(p1, center), R);
            p[2] = DrawHelper.perpendicularToLine(p1, p2, DrawHelper.center2point(center, p2), R);
            p[3] = p2;

            g.DrawBeziers(pen, p);

            //Добавить надпись - вес ребра
            Font font = new Font("Times New Roman", 14);
            SolidBrush brush = new SolidBrush(Color.Red);

            StringFormat format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Center;

            Point perpendicularToLine = DrawHelper.perpendicularToLine(p1, p2, center, R);

            g.DrawString(text, font, brush, perpendicularToLine, format);
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
