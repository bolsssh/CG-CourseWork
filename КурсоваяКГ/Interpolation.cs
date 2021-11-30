using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace КурсоваяКГ
{
    public class Line
    {
        public Point From { get; set; }
        public Point To { get; set; }

        public Line(Point from, Point to)
        {
            From = from;
            To = to;
        }
    }

    public class Triangle
    {
        public List<Point> Points;
        public List<Line> Lines;

        public Triangle(List<Point> points)
        {
            Lines = new List<Line>();
            Points = points;
            for (var i = 1; i < points.Count; i++)
            {
                Lines.Add(new Line(points[i - 1], points[i]));
            }
        }

        public Triangle(params Point[] points)
        {
            if (points.Length < 3)
                return;

            Lines = new List<Line>();
            Points = points.ToList();
            for (var i = 1; i < Points.Count; i++)
            {
                Lines.Add(new Line(Points[i - 1], Points[i]));
            }

            Lines.Add(new Line(Points[Points.Count - 1], Points[0]));
        }

        public double GetSquare()
        {
            //var a = GetSide(Lines[0]);
            //var b = GetSide(Lines[1]);
            //var c = GetSide(Lines[2]);
            //var p = a + b + c;
            //p /= 2d;
            //return Math.Sqrt(p * (p - a) * (p - b) * (p - c));
            return 0.5 * Math.Abs((Points[1].X - Points[0].X) * (Points[2].Y - Points[0].Y) -
                                  (Points[2].X - Points[0].X) * (Points[1].Y - Points[0].Y));
            //     return .5 * Math.Abs((double)(x0) * ((double)(y1) - y2) + (double)(x1) * ((double)(y2) - y0) + (double)(x2) * ((double)(y0) - y1));
        }
        //private double GetSide(Line line)
        //{
        //    return Math.Sqrt(Math.Pow(line.To.X - line.From.X, 2) + Math.Pow(line.To.Y - line.From.Y, 2));
        //}
    }

    public struct RGB
    {
        public int R;
        public int G;
        public int B;

        public RGB(int r, int g, int b)
        {
            R = r;
            G = g;
            B = b;
        }
    }

    public struct DoubleRGB
    {
        public double R;
        public double G;
        public double B;

        public DoubleRGB(double r, double g, double b)
        {
            R = r;
            G = g;
            B = b;
        }
    }

    public class Interpolation
    {
        private Rasterizer rasterizer;
        private int gridSize;

        public Interpolation(int gridSize)
        {
            this.gridSize = gridSize;
            rasterizer = new Rasterizer(gridSize);
        }

        //public void SetGridSize(int value)
        //{
        //    gridSize = value;
        //    rasterizer=new Rasterizer(value);
        //}
        private List<List<int>> pallette = new List<List<int>>
        {
            new List<int> { 255, 0 },
            new List<int> { 0, 255 },
        };

        public struct Pixel
        {
            public RGB Color;
            public Point Point;

            public Pixel(RGB col, Point point)
            {
                Color = col;
                Point = point;
            }
        }

        public List<Pixel> GetGouradTriangle(Triangle figure, int offset)
        {
            var pixels = new List<Pixel>();
            var bounds = GetBounds(figure.Points, gridSize);

            var colors = new Dictionary<Point, RGB>();
            //map<pair<int, int>, vector<int>> colors;

            var AB = rasterizer.RastorizeWithBresenham(figure.Points[0], figure.Points[1]).ToHashSet();
            //Bresenham(triangle[0][0], triangle[0][1], triangle[1][0], triangle[1][1]);
            var BC = rasterizer.RastorizeWithBresenham(figure.Points[1], figure.Points[2]).ToHashSet();
            //var BC = Bresenham(triangle[1][0], triangle[1][1], triangle[2][0], triangle[2][1]);
            var AC = rasterizer.RastorizeWithBresenham(figure.Points[0], figure.Points[2]).ToHashSet();
            //var AC = Bresenham(triangle[0][0], triangle[0][1], triangle[2][0], triangle[2][1]);

            var lines = new List<HashSet<Point>> { AB, BC, AC };
            //vector<set<pair<int, int>>> lines = {AB, BC, AC};


            var counter = 0;
            foreach (var line in lines /*.OrderBy(l => l.Max(p=>p.X)).ThenBy(l=> l.Max(p => p.Y))*/)
            {
                var gradient = 255 / (line.Count + 1);
                int R = pallette[counter % 2][0],
                    G = pallette[counter % 2][1],
                    B = 0;

                foreach (var point in line)
                {
                    var color = new RGB(R, G, B);
                    var pixel = new Pixel(color, point);
                    pixels.Add(pixel);
                    if (colors.ContainsKey(point))
                        colors[point] = color;
                    else colors.Add(point, color);


                    if (counter == 0)
                    {
                        R -= gradient;
                        G += gradient;
                    }
                    else if (counter == 1)
                    {
                        B += gradient;
                        G -= gradient;
                    }
                    else
                    {
                        R -= gradient;
                        B += gradient;
                    }
                }

                counter++;
            }

            for (var i = bounds.Top; i <= bounds.Bottom; i++)
            {
                var painted = false;
                var hor = new List<int>();
                for (var j = bounds.Left; j <= bounds.Right; j++)
                {
                    if (colors.ContainsKey(new Point(j, i)) && painted)
                        break;

                    if (colors.ContainsKey(new Point(j - 1, i)) && !colors.ContainsKey(new Point(j, i)))
                        painted = true;

                    if (painted)
                    {
                        hor.Add(j);
                    }

                    if (j == bounds.Right)
                    {
                        hor.Clear();
                    }
                }

                if (hor.Any())
                {
                    int left = hor[0] - 1, right = hor[hor.Count-1] + 1;
                    var p = colors[new Point(left, i)];
                    var q = colors[new Point(right, i)];

                    var R = (q.R - p.R) / hor.Count;
                    var G = (q.G - p.G) / hor.Count;
                    var B = (q.B - p.B) / hor.Count;

                    foreach (var j in hor)
                    {
                        pixels.Add(new Pixel(p, new Point(j, i)));

                        p.R += R;
                        p.G += G;
                        p.B += B;
                    }
                }
            }

            return pixels;
        }


        private RGB GetBaricentricCoordinatesColor(Triangle triangle, Point point)
        {
            var x = point.X;
            var y = point.Y;
            var points = triangle.Points;
            var ABC = triangle.GetSquare();


            var ABX = (int)new Triangle(points[0], points[1], new Point(x * gridSize, y * gridSize)).GetSquare();
            var ACX = (int)new Triangle(new Point(x * gridSize, y * gridSize), points[1], points[2]).GetSquare();
            var BCX = (int)new Triangle(points[0], new Point(x * gridSize, y * gridSize), points[2]).GetSquare();

            var R = (int)(255 * Math.Min(ABX, ABC) / Math.Max(ABX, ABC));
            var G = (int)(255 * Math.Min(ACX, ABC) / Math.Max(ACX, ABC));
            var B = (int)(255 * Math.Min(BCX, ABC) / Math.Max(BCX, ABC));

            return new RGB(R, G, B);
        }

        public List<Pixel> GetBccTriangle(Triangle figure, int offset)
        {
            var pixels = new List<Pixel>();

            var bounds = GetBounds(figure.Points, gridSize);
            var AB = rasterizer.RastorizeWithBresenham(figure.Points[0], figure.Points[1]).ToHashSet();
            var BC = rasterizer.RastorizeWithBresenham(figure.Points[1], figure.Points[2]).ToHashSet();
            var AC = rasterizer.RastorizeWithBresenham(figure.Points[0], figure.Points[2]).ToHashSet();
            var plots = AB.ToList();
            plots.AddRange(BC);
            plots.AddRange(AC);

            foreach (var plot in plots)
            {
                var color = GetBaricentricCoordinatesColor(figure, plot);
                var pixel = new Pixel(color, plot);
                pixels.Add(pixel);
            }

            for (var i = bounds.Top; i <= bounds.Bottom; i++)
            {
                var painted = false;
                var hor = new List<int>();
                for (var j = bounds.Left; j <= bounds.Right; j++)
                {
                    if (plots.Contains(new Point(j, i)) && painted)
                        break;

                    if (plots.Contains(new Point(j - 1, i)) && !plots.Contains(new Point(j, i)))
                        painted = true;


                    if (painted)
                    {
                        hor.Add(j);
                    }

                    if (j == bounds.Right)
                    {
                        hor.Clear();
                    }
                }

                foreach (var j in hor)
                {
                    var color = GetBaricentricCoordinatesColor(figure, new Point(j, i));
                    var pixel = new Pixel(color, new Point(j + offset, i));
                    pixels.Add(pixel);
                }
            }

            return pixels;
        }

        private Rectangle GetBounds(List<Point> points, int div)
        {
            var leftX = points.Min(p => p.X) / div;
            var rightX = points.Max(p => p.X) / div;
            var upY = points.Min(p => p.Y) / div;
            var downY = points.Max(p => p.Y) / div;
            return new Rectangle(leftX, upY, rightX - leftX, downY - upY);
        }


        public Bitmap Bilinear(Bitmap image, double scale = 1)
        {
            var xMax = image.Width;
            var yMax = image.Height;
            var colors = new RGB[xMax, yMax];
            var resizeXmax = Convert.ToInt32(Math.Floor((image.Width * scale)));
            var resizeYmax = Convert.ToInt32(Math.Floor((image.Height * scale)));
            var resizedPicture = new Bitmap(resizeXmax, resizeYmax);
            for (var yCur = 0; yCur < yMax; yCur++)
            {
                for (var xCur = 0; xCur < xMax; xCur++)
                {
                    var color = image.GetPixel(xCur, yCur);
                    colors[xCur, yCur].R = color.R;
                    colors[xCur, yCur].G = color.G;
                    colors[xCur, yCur].B = color.B;
                }
            }


            if (resizeXmax < 1) resizeXmax = 1;
            if (resizeYmax < 1) resizeYmax = 1;

            var newColors = new DoubleRGB[resizeXmax, resizeYmax];

            var xStep = Convert.ToDouble(xMax - 1) / Convert.ToDouble(resizeXmax - 1);
            var yStep = Convert.ToDouble(yMax - 1) / Convert.ToDouble(resizeYmax - 1);

            if (double.IsInfinity(xStep)) xStep = 1;

            if (double.IsInfinity(yStep)) yStep = 1;


            for (double yCur = 0; Math.Round(yCur, 1) <= yMax - 1; yCur += yStep)
            {
                for (double xCur = 0; Math.Round(xCur, 1) <= xMax - 1; xCur += xStep)
                {
                    var x = Math.Round(xCur / xStep, 0);
                    var y = Math.Round(yCur / yStep, 0);
                    var x1 = Math.Floor(xCur);
                    var y1 = Math.Floor(yCur);
                    if (Math.Abs(xCur - Math.Floor(xCur)) < double.Epsilon & xCur >= 1 || xCur > xMax - 1)
                        x1--;


                    if (Math.Abs(yCur - Math.Floor(yCur)) < double.Epsilon & yCur >= 1 || yCur > yMax - 1)
                        y1--;


                    var x2 = x1 + 1;
                    var y2 = y1 + 1;
                    var xi = Convert.ToInt32(x);
                    var yi = Convert.ToInt32(y);
                    var xi1 = Convert.ToInt32(x1);
                    var yi1 = Convert.ToInt32(y1);
                    var xi2 = Convert.ToInt32(x2);
                    var yi2 = Convert.ToInt32(y2);

                    newColors[xi, yi].R = colors[xi1, yi1].R * (x2 - xCur) * (y2 - yCur) +
                                          colors[xi2, yi1].R * (xCur - x1) * (y2 - yCur) +
                                          colors[xi1, yi2].R * (x2 - xCur) * (yCur - y1) +
                                          colors[xi2, yi2].R * (xCur - x1) * (yCur - y1);
                    newColors[xi, yi].G = colors[xi1, yi1].G * (x2 - xCur) * (y2 - yCur) +
                                          colors[xi2, yi1].G * (xCur - x1) * (y2 - yCur) +
                                          colors[xi1, yi2].G * (x2 - xCur) * (yCur - y1) +
                                          colors[xi2, yi2].G * (xCur - x1) * (yCur - y1);
                    newColors[xi, yi].B = colors[xi1, yi1].B * (x2 - xCur) * (y2 - yCur) +
                                          colors[xi2, yi1].B * (xCur - x1) * (y2 - yCur) +
                                          colors[xi1, yi2].B * (x2 - xCur) * (yCur - y1) +
                                          colors[xi2, yi2].B * (xCur - x1) * (yCur - y1);
                }
            }

            for (var j = 0; j < resizeYmax; j++)
            for (var i = 0; i < resizeXmax; i++)
            {
                resizedPicture.SetPixel(i, j,
                    Color.FromArgb(Convert.ToInt32(newColors[i, j].R), Convert.ToInt32(newColors[i, j].G),
                        Convert.ToInt32(newColors[i, j].B)));
            }

            return resizedPicture;
        }
    }
}