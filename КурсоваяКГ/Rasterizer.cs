using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace КурсоваяКГ
{
    public class Rasterizer
    {
        public int GridSize;

        public Rasterizer(int gridSize)
        {
            GridSize = gridSize;
        }

        public List<Point> RastorizeWithCda(Point from, Point to)
        {
            from = new Point(from.X / GridSize, from.Y / GridSize);
            to = new Point(to.X / GridSize, to.Y / GridSize);
            var rasterized = new List<Point> { from };
            var delta = new Point(to.X - from.X, to.Y - from.Y);
            var s = Math.Max(Math.Abs(delta.X), Math.Abs(delta.Y));

            var xi = (double)delta.X / s;
            var yi = (double)delta.Y / s;

            var x = (double)from.X;
            var y = (double)from.Y;

            for (var i = 0; i < s; i++)
            {
                x += xi;
                y += yi;
                rasterized.Add(new Point((int)Math.Floor(x), (int)Math.Floor(y)));
            }

            return rasterized;
        }


        public List<Point> RastorizeWithBresenham(Point from, Point to)
        {
            var rasterized = new List<Point>();
            var delta = new Point((to.X - from.X), (to.Y - from.Y));
            var incx = Math.Sign(delta.X);
            var incy = Math.Sign(delta.Y);

            delta.X = Math.Abs(delta.X);
            delta.Y = Math.Abs(delta.Y);
            var el = Math.Max(delta.X, delta.Y) / GridSize;
            var es = Math.Min(delta.X, delta.Y) / GridSize;
            var pdx = delta.X > delta.Y ? incx * GridSize : 0;
            var pdy = delta.X > delta.Y ? 0 : incy * GridSize;

            var x = from.X;
            var y = from.Y;
            var err = el / 2;

            rasterized.Add(new Point((int)Math.Floor((double)x / GridSize),
                (int)Math.Floor((double)y / GridSize)));

            for (var t = 0; t < el; t++)
            {
                err -= es;
                if (err < 0)
                {
                    err += el;
                    x += incx * GridSize;
                    y += incy * GridSize;
                }
                else
                {
                    x += pdx;
                    y += pdy;
                }

                rasterized.Add(new Point((int)Math.Floor((double)x / GridSize),
                    (int)Math.Floor((double)y / GridSize)));
            }

            return rasterized;
        }
    }
}
