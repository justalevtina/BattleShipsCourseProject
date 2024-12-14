using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Mastermind_Client
{
    public class PagButton : Button
    {
        private float border_radius = 10;
        public float border_size = 2;
        
        private Color border_color = Color.Black;

        public PagButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            Size size = new Size(150, 40);
            BackColor = Color.Transparent;
        }

        private GraphicsPath GetFugurePath(RectangleF rectangle, float radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddEllipse(rectangle);
            path.CloseFigure();
            return path;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            RectangleF rectangleF = new RectangleF(0, 0, Width, Height);
            RectangleF rectangleBorder = new RectangleF(0, 0, Width, Height);

            using (GraphicsPath pathSurface = GetFugurePath(rectangleF, border_radius))
            using (GraphicsPath pathBorder = GetFugurePath(rectangleBorder, border_radius))
            using (Pen penSurface = new Pen(Parent.BackColor, 2))
            using (Pen penBorder = new Pen(border_color, border_size))
            {
                penBorder.Alignment = PenAlignment.Inset;
                Region = new Region(pathSurface);
                pevent.Graphics.DrawPath(penSurface, pathSurface);

                if (border_size > 1)
                {
                    pevent.Graphics.DrawPath(penBorder, pathBorder);
                }
            }   
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            Parent.BackColorChanged += new EventHandler(Container_BackColorChanged);
        }

        private void Container_BackColorChanged(object sender ,EventArgs e)
        {
            if (DesignMode)
            {
                Invalidate();
            }
        }
    }
}