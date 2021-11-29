using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace КурсоваяКГ
{
    public class Controller
    {
        private ObjectPainter painter;

        public Controller(ObjectPainter painter)
        {
            this.painter = painter;
        }
        public void MouseClickControl(MouseEventArgs e)
        {
        }
        public void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Right:
                    painter.Degree += 3;
                    break;
                case Keys.Left:
                    painter.Degree -= 3;
                    break;
            }
        }

    }
}
