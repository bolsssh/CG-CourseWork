using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;


namespace КурсоваяКГ
{
    public class Polygon
    {
        public List<Vector> dots = new List<Vector>();
        public double Z;

        public PointF[] ToPointFArray()
        {
            var list = new List<PointF>();
            foreach (var dot in dots)
            {
                list.Add(dot.ToPointF());
            }
            return list.ToArray();
        }
    }

}
