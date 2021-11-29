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
    public partial class MainForm : Form
    {
        private ObjectPainter painter;

        private Controller controller;
        private Bitmap myBitmap;
        public int width;
        public int height;

        public MainForm(int x, int y)
        {
            width = x;
            height = y;
            InitializeComponent();
            ClientSize = new Size(x, y);
            painter = new ObjectPainter(ClientSize);
            controller = new Controller(painter);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Text = "Computer Graphics";
            DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            CenterToParent();
            myBitmap = new Bitmap(width, height);
            var graphics = Graphics.FromImage(myBitmap);
            painter.DrawScene(graphics);
            BackgroundImage = myBitmap;
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            controller.OnKeyDown(e);
            Invalidate();
        }
    }
}