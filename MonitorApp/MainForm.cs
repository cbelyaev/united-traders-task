using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MonitorApp
{
    public partial class MainForm: Form
    {
        private const int ConcurrencyLevel = 100; // at least 100 sensors
        private readonly ConcurrentDictionary<int, SensorData> _sensors = new ConcurrentDictionary<int, SensorData>(ConcurrencyLevel, ConcurrencyLevel);
        private readonly Dictionary<int,SensorIndicator> _labels = new Dictionary<int, SensorIndicator>(ConcurrencyLevel);

        public MainForm()
        {
            InitializeComponent();
        }

        internal void OnMessage(object sender, MessageArgs args)
        {
            _sensors.AddOrUpdate(args.SensorId, new SensorData(args.Value), (id, data) => data.Update(args.Value));
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            _timer.Enabled = true;
        }

        protected override void OnClosed(EventArgs e)
        {
            _timer.Enabled = false;
            base.OnClosed(e);
        }

        private bool _updating;

        private void _timer_Tick(object sender, EventArgs e)
        {
            if (_updating)
                return;
            _updating = true;
            try
            {
                foreach (var pair in _sensors)
                {
                    if (!_labels.TryGetValue(pair.Key, out var indicator))
                    {
                        indicator = new SensorIndicator(pair.Key);
                        _labels.Add(pair.Key, indicator);
                        _panel.Controls.Add(indicator);
                    }
                    indicator.Update(pair.Value.Value, pair.Value.Diff);
                }

                Text = $"{nameof(MonitorApp)}: {PerfMonitor.GetValuesPerSecond():F3}";
            }
            finally
            {
                _updating = false;
            }
        }
    }
}