using System;
using System.Collections.Generic;
using System.Text;

namespace КурсоваяКГ
{
    public class Matrix
    {
        public double[,] arr = new double[4, 4];

        public Matrix()
        {
            for (var i = 0; i < 4; i++)
            for (var j = 0; j < 4; j++)
                arr[i, j] = default;
        }

        public static Matrix ViewPort(int x, int y, int w, int h)
        {
            var matrix = new Matrix();
            matrix.arr[0, 0] = w / 2.0;
            matrix.arr[1, 1] = h / -2.0;
            matrix.arr[2, 2] = 255.0 / 2.0;
            matrix.arr[3, 3] = 1;
            matrix.arr[0, 3] = x + w / 2.0;
            matrix.arr[1, 3] = y + h / 2.0;
            matrix.arr[2, 3] = 255.0 / 2.0;
            return matrix;
        }

        public static Matrix GetOrthogonalProjection(double l, double r, double t, double b, double n, double f)
        {
            var matrix = new Matrix();
            matrix.arr[0, 0] = 2.0 / (r - l);
            matrix.arr[1, 1] = 2.0 / (t - b);
            matrix.arr[2, 2] = -2.0 / (f - n);
            matrix.arr[0, 3] = -(r + l) / (r - l);
            matrix.arr[1, 3] = -(t + b) / (t - b);
            matrix.arr[2, 3] = -(f + n) / (f - n);
            matrix.arr[3, 3] = 1;
            return matrix;
        }

        public static Matrix LookAt(Vector eye, Vector center, Vector up)
        {
            var matrix = new Matrix();
            var Z = eye - center;
            Z.Normalize();
            var X = up ^ Z;
            X.Normalize();
            var Y = Z ^ X;
            Y.Normalize();
            matrix.arr[0, 0] = X.X;
            matrix.arr[0, 1] = X.Y;
            matrix.arr[0, 2] = X.Z;
            matrix.arr[0, 3] = -center.X;
            matrix.arr[1, 0] = Y.X;
            matrix.arr[1, 1] = Y.Y;
            matrix.arr[1, 2] = Y.Z;
            matrix.arr[1, 3] = -center.Y;
            matrix.arr[2, 0] = Z.X;
            matrix.arr[2, 1] = Z.Y;
            matrix.arr[2, 2] = Z.Z;
            matrix.arr[2, 3] = -center.Z;
            matrix.arr[3, 3] = 1;
            return matrix;
        }

        public static Matrix Translated(double tx, double ty, double tz)
        {
            var matrix = new Matrix
            {
                arr =
                {
                    [0, 3] = tx,
                    [1, 3] = ty,
                    [2, 3] = tz,
                    [0, 0] = 1,
                    [1, 1] = 1,
                    [2, 2] = 1,
                    [3, 3] = 1
                }
            };
            return matrix;
        }

        public static Matrix Turn(int degree)
        {
            var matrix = new Matrix();
            var angle = Math.PI * degree / 180;
            //var sinSign = 1;
            if (degree > 180)
            {

            }
            matrix.arr[0, 0] = Math.Cos(angle) ;
            matrix.arr[0, 2] = -Math.Sin(angle);
            matrix.arr[1, 1] = 1;
            matrix.arr[2, 0] = Math.Sin(angle);
            matrix.arr[2, 2] = Math.Cos(angle);
            matrix.arr[3, 3] = 1;
            return matrix;
        }

        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            var matrix = new Matrix();
            for (var i = 0; i < 4; i++)
            for (var j = 0; j < 4; j++)
                matrix.arr[i, j] = m1.arr[i, 0] * m2.arr[0, j] + m1.arr[i, 1] * m2.arr[1, j] +
                                   m1.arr[i, 2] * m2.arr[2, j] + m1.arr[i, 3] * m2.arr[3, j];
            return matrix;
        }
    }
}