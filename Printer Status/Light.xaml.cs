using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Printer_Status
{
    public partial class Light : UserControl
    {
        #region DependencyProperties
        ///<summary>Identifies the OnTime dependency property.</summary>
        public static readonly DependencyProperty OnTimeProperty = DependencyProperty.Register("OnTime", typeof(double), typeof(Light), new PropertyMetadata(OnOnTimePropertyChanged));
        ///<summary>Identifies the OffTime dependency property.</summary>
        public static readonly DependencyProperty OffTimeProperty = DependencyProperty.Register("OffTime", typeof(double), typeof(Light), new PropertyMetadata(OnOffTimePropertyChanged));
        ///<summary>Identifies the Color dependency property.</summary>
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(Light), new PropertyMetadata(OnColorPropertyChanged));
        #endregion
        #region Properties
        /// <summary>
        /// The time in milliseconds the light will be on.
        /// </summary>
        public double OnTime
        {
            get { return (double)GetValue(OnTimeProperty); }
            set { SetValue(OnTimeProperty, value); }
        }
        /// <summary>
        /// The time in milliseconds the light will be off.
        /// </summary>
        public double OffTime
        {
            get { return (double)GetValue(OffTimeProperty); }
            set { SetValue(OffTimeProperty, value); }
        }
        /// <summary>
        /// The colour of the light.
        /// </summary>
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        #endregion
        #region Fields
        private DispatcherTimer _onTimer;
        private DispatcherTimer _offTimer;
        private SolidColorBrush _onBrush;
        private SolidColorBrush _offBrush;
        #endregion
        #region PropertyChanged
        private static void OnColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Light light = (Light)d;
            Color newColor = (Color)e.NewValue;
            light.Color = newColor;
            light._onBrush = new SolidColorBrush(newColor);
            light._offBrush = new SolidColorBrush(Color.FromRgb(25, 25, 25));
        }

        private static void OnOnTimePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Light light = (Light)d;
            double newTime = (double) e.NewValue;
            light.OnTime = newTime;
            light._onTimer.Interval = TimeSpan.FromMilliseconds(newTime);
            //Start the onTimer unless the offTimer is going to tick (and do it at a different time)
            if (!light._offTimer.IsEnabled) light._onTimer.Start();
        }
        private static void OnOffTimePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Light light = (Light)d;
            double newTime = (double)e.NewValue;
            light.OffTime = newTime;
            light._offTimer.Interval = TimeSpan.FromMilliseconds(newTime);
            //Start the offTimer unless the onTimer is going to tick (and do it at a different time)
            if (!light._onTimer.IsEnabled) light._offTimer.Start();
        }
        #endregion
        #region Constructor
        /// <summary>
        /// Initialises a new instance of the Light UserControl.
        /// </summary>
        public Light()
        {
            _onTimer = new DispatcherTimer();
            _offTimer = new DispatcherTimer();
            _onTimer.Tick += OnTimer_Tick;
            _offTimer.Tick += OffTimer_Tick;

            InitializeComponent();
        }
        #endregion
        #region Timer Ticks
        private void OffTimer_Tick(object sender, EventArgs e)
        {
            _offTimer.Stop();
            LED.Fill = _offBrush;
            //If the light needs to be on afterwards (is flashing), start the timer for the light to switch on
            if (_onTimer.Interval != TimeSpan.Zero) _onTimer.Start();
        }

        private void OnTimer_Tick(object sender, EventArgs e)
        {
            _onTimer.Stop();
            LED.Fill = _onBrush;
            //If the light needs to be off afterwards (is flashing), start the timer for the light to switch off
            if (_offTimer.Interval != TimeSpan.Zero) _offTimer.Start();
        }
        #endregion
    }
}
