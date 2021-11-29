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
                ChangeMatrix(value);
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
            ChangeMatrix(_degree);
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

        private void ChangeMatrix(int degree)
        {
            tModel = new Model(model);
            tModel.Change(_mt * Matrix.Turn(degree));
        }

        public void DrawScene(Graphics graphics)
        {
            graphics.Clear(Color.White);
            var pols = Math.Abs(180-Math.Abs(Degree))<=90 ? 
                tModel.Polygons.OrderBy(p => p.Z).ToList() : tModel.Polygons;

            foreach (var pol in pols)
            {
                var a = (int)(pol.Z);
                var brush = new SolidBrush(Color.FromArgb(100 + 5 * a, 100 + 5 * a, 100 + 5 * a));
                graphics.FillPolygon(brush, pol.ToPointFArray());
            }
        }
    }
}