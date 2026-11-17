using System.Windows.Forms;

namespace TimSort.Services
{
    /// <summary>
    /// Panel con doble búfer activado para renderizado gráfico suave y sin parpadeo (flickering).
    /// </summary>
    public class DoubleBufferedPanel : Panel
    {
        public DoubleBufferedPanel()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
            UpdateStyles();
        }
    }
}
