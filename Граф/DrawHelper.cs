using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Граф
{
    class DrawHelper
    {
        public Point[] coordinatesPolygon(Point center, int R, int N)
        {
            Point[] result = new Point[N];

            for (int i = 0; i < N; i++)
            {
                int x = Convert.ToInt32(center.X + R * Math.Cos(2 * Math.PI * i / N));
                int y = Convert.ToInt32(center.Y + R * Math.Sin(2 * Math.PI * i / N));
                result[i] = new Point(x, y);
            }

            return result;
        }

        public Point сoordinatesPointOnLine(Point p1, Point p2, int R)
        {
            int x1 = p1.X;
            int y1 = p1.Y;
            int x2 = p2.X;
            int y2 = p2.Y;

            int a = x2 - x1;
            int b = y2 - y1;
            double c = distanceBetweenTwoPoints(p1, p2);

            double sin_alfa = a / c;
            double cos_alfa = b / c;

            int c2 = R;
            int a2 = Convert.ToInt32(c2 * sin_alfa);
            int b2 = Convert.ToInt32(c2 * cos_alfa);

            int x = x1 + a2;
            int y = y1 + b2;

            return new Point(x, y);
        }

        /// <summary>
        /// Получить прямоугольник круга, зная центр и радиус круга
        /// </summary>
        /// <param name="center">Центра круга</param>
        /// <param name="R">Радиус круга</param>
        /// <returns></returns>
        public Rectangle rectangleFromCircle(Point center, int R)
        {
            Point location = new Point(center.X - R, center.Y - R);
            Size size = new Size(R * 2, R * 2);

            return new Rectangle(location, size);
        }

        /// <summary>
        /// Вычисление координаты перпендикуляра к прямой
        /// Источник: http://www.cyberforum.ru/geometry/thread1359393.html
        /// </summary>
        /// <param name="A">Начало прямой</param>
        /// <param name="B">Конец прямой</param>
        /// <param name="R">Длина перпендикуляра</param>
        /// <returns></returns>
        public Point perpendicularToLine(Point A, Point B, Point C, double R)
        {
            int x_A = A.X;
            int y_A = A.Y;
            int x_B = B.X;
            int y_B = B.Y;
            int x_C = C.X;
            int y_C = C.Y;

            double sizeAB = distanceBetweenTwoPoints(A, B);
            //+- менять в зависимости от положения первендикуляра
            int x_D = Convert.ToInt32(x_C + R * ((y_A - y_B) / sizeAB));
            int y_D = Convert.ToInt32(y_C - R * ((x_A - x_B) / sizeAB));

            return new Point(x_D, y_D);
        }

        public Point center2point(Point p1, Point p2, double s)
        {
            int x = center2x(p1.X, p2.X, s);
            int y = center2x(p1.Y, p2.Y, s);
            return new Point(x, y);
        }

        public Point center2point(Point p1, Point p2)
        {
            int x = center2x(p1.X, p2.X);
            int y = center2x(p1.Y, p2.Y);
            return new Point(x, y);
        }

        public int center2x(int x1, int x2, double s)
        {
            return Convert.ToInt32(x1 + ((x2 - x1) * s));
        }

        /// <summary>
        /// Середина между двумя Х
        /// </summary>
        /// <param name="x1">Первый Х</param>
        /// <param name="x2">Второй Х</param>
        /// <returns></returns>
        public int center2x(int x1, int x2)
        {
            return x1 + ((x2 - x1) / 2);
        }

        /// <summary>
        /// Расстояние между двумя точками
        /// </summary>
        /// <param name="p1">Первая точка</param>
        /// <param name="p2">Вторая точка</param>
        /// <returns></returns>
        public double distanceBetweenTwoPoints(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }


    }
}
