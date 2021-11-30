using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace КурсоваяКГ
{
    public class ObjectPainter
    {
        private int _degree;

        public int Degree
        {
            get => _degree;
            set
            {
                _degree = value;
                if (_degree >= 360)
                    _degree -= 360;
                if (_degree <= -360)
                    _degree += 360;
                ChangeModelView(value);
            }
        }

        private Model model = new Model();
        private readonly Matrix _mt;
        private Model tModel;

        public ObjectPainter(Size windowSize)
        {
            _degree = 90;
            model.Read("face.obj");
            model.Sort();

            _mt = GetMatrixView(windowSize);
            ChangeModelView(_degree);
        }

        private Matrix GetMatrixView(Size windowSize)
        {
            var size = Matrix.ViewPort(0, 0, windowSize.Width - 300, windowSize.Width - 300);
            var k = windowSize.Width / windowSize.Height * 2;
            var mp = Matrix.GetOrthogonalProjection(model.MinX * k, model.MaxX * k, model.MinY * k, model.MaxY * k,
                model.MinZ * k, model.MaxZ * k);
            var mt = size * mp;
            var eye = new Vector() { X = 0, Y = 1, Z = 2 };
            var center = new Vector() { X = 0, Y = 0, Z = 0 };
            var up = new Vector() { X = 0, Y = 15, Z = 0 };
            mt *= Matrix.LookAt(eye, center, up) * Matrix.Translated(3, -25, -10);
            return mt;
        }

        private void ChangeModelView(int degree)
        {
            tModel = new Model(model);

            tModel.Change(_mt * Matrix.Turn(degree));
        }

        public void DrawScene(Graphics graphics)
        {
            graphics.Clear(Color.White);
            var pols = tModel.Polygons.OrderBy(p => p.Z).ToList();
            var max = tModel.Polygons.Max(p => p.Z);
            var min = tModel.Polygons.Min(p => p.Z);
            var exp = 255 / (max - min);
            foreach (var pol in pols)
            {
                var a = (int)((pol.Z - min) * exp);
                var brush = new SolidBrush(Color.FromArgb(a, a, a));
                graphics.FillPolygon(brush, pol.Points.Select(p => p.ToPointF()).ToArray());
            }
        }
    }
}