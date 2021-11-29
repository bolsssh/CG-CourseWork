using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace КурсоваяКГ
{
    public class Model
    {
        public List<Vector> Coordinates = new List<Vector>();
        public List<Polygon> Polygons = new List<Polygon>();
        public double MinX { get; set; }
        public double MinY { get; set; }
        public double MinZ { get; set; }
        public double MaxX { get; set; }
        public double MaxY { get; set; }
        public double MaxZ { get; set; }

        public void Read(string path)
        {
            MinX = 100000;
            MinY = 100000;
            MinZ = 100000;
            MaxX = -100000;
            MaxY = -100000;
            MaxZ = -100000;
            using (var streamReader = new System.IO.StreamReader(path))
            {
                var line = "";
                while ((line = streamReader.ReadLine()) != null)
                {
                    if (!line.StartsWith("v ")) continue;
                    var coords = line.Split(' ');
                    var point = new Vector()
                    {
                        X = double.Parse(coords[1], System.Globalization.CultureInfo.InvariantCulture),
                        Y = double.Parse(coords[2], System.Globalization.CultureInfo.InvariantCulture),
                        Z = double.Parse(coords[3], System.Globalization.CultureInfo.InvariantCulture)
                    };
                    Coordinates.Add(point);
                    MaxX = Math.Max(point.X, MaxX);
                    MaxY = Math.Max(point.Y, MaxY);
                    MaxZ = Math.Max(point.Z, MaxZ);
                    MinX = Math.Min(point.X, MinX);
                    MinY = Math.Min(point.Y, MinY);
                    MinZ = Math.Min(point.Z, MinZ);
                }
            }

            using (var streamReader = new System.IO.StreamReader(path))
            {
                var line = "";
                while ((line = streamReader.ReadLine()) != null)
                {
                    if (line.StartsWith("f"))
                    {
                        var info = line.Split(' ');
                        var pol = new Polygon();
                        double z = 0;
                        for (var i = 1; i < info.Length; i++)
                        {
                            var dot = Coordinates[int.Parse(info[i].Split('/')[0]) - 1];
                            z += dot.Z;
                            pol.dots.Add(dot);
                        }

                        pol.Z = z / 4;
                        Polygons.Add(pol);
                    }
                }
            }
        }

        public Model()
        {
        }

        public Model(Model model)
        {
            MinX = model.MinX;
            MinY = model.MinY;
            MinZ = model.MinZ;
            MaxX = model.MaxX;
            MaxY = model.MaxY;
            MaxZ = model.MaxZ;
            foreach (var a in model.Coordinates) Coordinates.Add(a);
            foreach (var b in model.Polygons) Polygons.Add(b);

        }

        public void Sort()
        {
            var flag = true;
            while (flag)
            {
                flag = false;
                for (var i = 0; i < Polygons.Count - 1; i++)
                {
                    if (Polygons[i].Z < Polygons[i + 1].Z)
                    {
                        var t = Polygons[i];
                        Polygons[i] = Polygons[i + 1];
                        Polygons[i + 1] = t;
                        // (Polygons[i], Polygons[i + 1]) = (Polygons[i + 1], Polygons[i]);
                        flag = true;
                    }
                }
            }
        }

        public void Change(Matrix matrix)
        {
            var pols = new List<Polygon>();
            foreach (var pol in Polygons)
            {
                var newPol = new Polygon();
                newPol.Z = pol.Z;
                foreach (var dot in pol.dots)
                {
                    var temp = dot;
                    var newDot = new Vector()
                    {
                        X = (matrix.arr[0, 0] * temp.X) + (matrix.arr[0, 1] * temp.Y) + (matrix.arr[0, 2] * temp.Z) + matrix.arr[0, 3],
                        Y = (matrix.arr[1, 0] * temp.X) + (matrix.arr[1, 1] * temp.Y) + (matrix.arr[1, 2] * temp.Z) + matrix.arr[1, 3],
                        Z = (matrix.arr[2, 0] * temp.X) + (matrix.arr[2, 1] * temp.Y) + (matrix.arr[2, 2] * temp.Z) + matrix.arr[2, 3],
                    };
                    newPol.dots.Add(newDot);
                }

                pols.Add(newPol);
            }

            Polygons = pols;
        }
    }
}