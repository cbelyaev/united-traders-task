using System.Drawing;
using System.Windows.Forms;

namespace MonitorApp
{
    public class SensorIndicator: Label
    {
        public SensorIndicator(int id)
        {
            Id = id;
            InitializeComponent();
        }

        public int Id { get; }

        private void InitializeComponent()
        {
            Location = new Point(0, 0);
            Size = new Size(202, 19);
            Name = $"{nameof(SensorIndicator)}{Id}";
            TextAlign = ContentAlignment.MiddleLeft;
            Text = $"{Id}: ";
        }

        public void Update(int value, int diff)
        {
            var text = $"{Id}: {value}{diff:+0;-#}";
            var color = diff < 0 ? Color.OrangeRed : (diff > 0 ? Color.ForestGreen : Color.Black);

            if (Text == text && ForeColor == color)
                return;

            Text = text;
            ForeColor = color;
            Invalidate();
        }
    }
}