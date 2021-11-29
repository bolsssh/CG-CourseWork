using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;


namespace КурсоваяКГ
{
    public class Vector
    {
        public double X, Y, Z;

        public double LenVec()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }
        public void Normalize()
        {
            var len = LenVec();
            X *= 1.0 / len;
            Y *= 1.0 / len;
            Z *= 1.0 / len;
        }
        public PointF ToPointF()
        {
            return new PointF((float)X, (float)Y);
        }
        public static Vector operator -(Vector v1, Vector v2)
        {
            return new Vector()
            {
                X = v1.X - v2.X,
                Y = v1.Y - v2.Y,
                Z = v1.Z - v2.Z
            };
        }
        public static Vector operator ^(Vector v1, Vector v2)
        {
            return new Vector()
            {
                X = v1.Y * v2.Z - v1.Z * v2.Y,
                Y = v1.X * v2.Z - v1.Z * v2.X,
                Z = v1.X * v2.Y - v1.Y * v2.X
            };
        }
    }

}
