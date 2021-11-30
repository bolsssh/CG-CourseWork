using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Linq;


namespace КурсоваяКГ
{
    public class Polygon
    {
        //private List<Vector> points = new List<Vector>();
        public List<Vector> Points;

        //private double _z;

        public Polygon(List<Vector> points)
        {
            Points = points;
            Z = Points.Select(p => p.Z).Average();
        }
        public double Z;

        //public PointF[] ToPointFArray()
        //{
        //    return Points.Select(dot => dot.ToPointF()).ToArray();
        //}
    }
}