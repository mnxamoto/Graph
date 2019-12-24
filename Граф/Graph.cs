using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Граф
{
    class Graph
    {
        int[] dataDistinct;
        int[] weightsNodes;
        int[][] weightsEdges;

        DrawHelper dh = new DrawHelper();

        public void draw(int[] data, DataGridView table1, DataGridView table2, PictureBox pictureBox)
        {
            computeMatrixAdjacency(data, table1, table2);
            draw(pictureBox);
        }

        public void computeMatrixAdjacency(int[] data, DataGridView table1, DataGridView table2)
        {
            int countData = data.Length;

            //Подсчёт количества повторений (весов вершин)
            dataDistinct = data.Distinct().ToArray();
            Array.Sort(dataDistinct);
            int countDataDistinct = dataDistinct.Length;
            weightsNodes = new int[countDataDistinct];

            for (int i = 0; i < countData; i++)
            {
                for (int k = 0; k < countDataDistinct; k++)
                {
                    if (data[i] == dataDistinct[k])
                    {
                        weightsNodes[k]++;
                        break;
                    }
                }
            }

            //Вывод
            table1.Columns.Clear();
            table1.Columns.Add("Columns_t1_Node", "Узел");
            table1.Columns.Add("Columns_t1_Weight", "Вес");

            for (int k = 0; k < countDataDistinct; k++)
            {
                string[] row = new string[2];
                row[0] = dataDistinct[k].ToString();
                row[1] = weightsNodes[k].ToString();

                table1.Rows.Add(row);
            }

            //Подсчёт количества переходов (весов рёбер)
            weightsEdges = new int[countDataDistinct][];

            for (int i = 0; i < countDataDistinct; i++)
            {
                weightsEdges[i] = new int[countDataDistinct];
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

                weightsEdges[x][y]++;
            }

            //Вывод
            table2.Columns.Clear();

            for (int i = 0; i < countDataDistinct; i++)
            {
                table2.Columns.Add("Columns_t2_Node" + i, i.ToString());
                table2.Rows.Add();
                table2.Rows[i].HeaderCell.Value = i.ToString();
            }

            for (int i = 0; i < countDataDistinct; i++)
            {
                for (int k = 0; k < countDataDistinct; k++)
                {
                    table2.Rows[i].Cells[k].Value = weightsEdges[i][k];
                }
            }

        }

        public void draw(PictureBox pictureBox, int[] dataDistinct, int[] weightsNodes, int[][] weightsEdges)
        {
            this.dataDistinct = dataDistinct;
            this.weightsNodes = weightsNodes;
            this.weightsEdges = weightsEdges;

            draw(pictureBox);
        }

        public void draw(PictureBox pictureBox)
        {
            int width = 1400;
            int height = 1400;
            pictureBox.Width = width;
            pictureBox.Height = height;

            Bitmap bmp = new Bitmap(width, height);
            pictureBox.Image = bmp;
            Graphics g = Graphics.FromImage(pictureBox.Image);

            Pen blackPen = new Pen(Color.Black);

            int countDataDistinct = dataDistinct.Length;  //Количество узлов

            Point centerImage = new Point(700, 700);  //Координаты центар изображения
            int rPolygon = 400;  //Как далеко от центра расположены узыл
            Point[] coordinatesNodes = dh.coordinatesPolygon(centerImage, rPolygon, countDataDistinct);

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
                node.value = weightsNodes[i];
                node.weight = Convert.ToInt32((node.value / (double)weightsNodes.Max()) * 100) + 20;  //+15 - мин размер (раудиус) узла

                g.DrawEllipse(blackPen, dh.rectangleFromCircle(node.coordinate, node.weight));
                g.DrawString($"{node.name}\r\n[{node.value}]", font, brush, node.coordinate, format);

                nodes[i] = node;
            }

            //Отрисовка рёбер
            Edge[] edges = new Edge[countDataDistinct * countDataDistinct];
            int maxVesaEdges = 0;

            for (int i = 0; i < countDataDistinct; i++)
            {
                int iMax = weightsEdges[i].Max();

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
                    edge.value = weightsEdges[i][k];
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

            int distance = Convert.ToInt32(dh.distanceBetweenTwoPoints(centerImage, centerNode)) + R;
            p[0] = dh.сoordinatesPointOnLine(centerImage, centerNode, distance);

            distance = R * 3;
            Point rLoop = dh.сoordinatesPointOnLine(centerNode, p[0], distance);

            distance = R;
            p[1] = dh.perpendicularToLine(p[0], rLoop, rLoop, -distance);
            p[2] = dh.perpendicularToLine(p[0], rLoop, rLoop, distance);
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

            Point cocoordinateText = dh.perpendicularToLine(p[0], rLoop, dh.сoordinatesPointOnLine(p[0], rLoop, R / 2), R / 2);

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

            Point p3 = dh.сoordinatesPointOnLine(p1, p2, R1);
            Point p4 = dh.сoordinatesPointOnLine(p2, p1, R2);

            drawPointer(g, p3, p4, widthPen, text);
        }

        private void drawPointer(Graphics g, Point p1, Point p2, float widthPen, string text)
        {
            Pen pen = new Pen(Color.Black);
            pen.CustomEndCap = new AdjustableArrowCap(5, 5);
            pen.Width = widthPen;

            double R = dh.distanceBetweenTwoPoints(p1, p2) / 8;
            Point center = dh.center2point(p1, p2);

            Point[] p = new Point[4];
            p[0] = p1;
            p[1] = dh.perpendicularToLine(p1, p2, dh.center2point(p1, center), R);
            p[2] = dh.perpendicularToLine(p1, p2, dh.center2point(center, p2), R);
            p[3] = p2;

            g.DrawBeziers(pen, p);

            //Добавить надпись - вес ребра
            Font font = new Font("Times New Roman", 14);
            SolidBrush brush = new SolidBrush(Color.Red);

            StringFormat format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Center;

            Point perpendicularToLine = dh.perpendicularToLine(p1, p2, center, R);

            g.DrawString(text, font, brush, perpendicularToLine, format);
        }
    }

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
}
