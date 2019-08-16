using Microsoft.Toolkit.Uwp.Input.GazeInteraction;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace _2dControls
{
    public sealed partial class CommandPalette : UserControl
    {

        public event EventHandler OctaveChanged = delegate { };
        private int _octave;
        public int Octave
        {
            get { return _octave; }
            set
            {
                _octave = Math.Min(Math.Max(24, value), 72);
                UpdateOctaveText(value);
                OctaveChanged(value, EventArgs.Empty);
            }
        }

        public int DwellSpeed = 0;

        public CommandPalette()
        {
            this.InitializeComponent();
            Octave = 48;
            
        }

        public void UpdateOctaveText(int octave)
        {
            OctaveText.Text = ((octave - 48) / 12).ToString();
        }

        public event RoutedEventHandler PlayButtonClick
        {
            add { Loop.Click += value; }
            remove { Loop.Click -= value; }
        }

        public event System.EventHandler<Microsoft.Toolkit.Uwp.Input.GazeInteraction.StateChangedEventArgs> PlayButtonGazeStateChanged
        {
            add { GetGazeButtonControl(Loop).StateChanged += value; }
            remove { GetGazeButtonControl(Loop).StateChanged -= value; }
        }

        public event RoutedEventHandler ScaleModeClick
        {
            add { scaleMode.Click += value; }
            remove { scaleMode.Click -= value; }
        }

        public event System.EventHandler<Microsoft.Toolkit.Uwp.Input.GazeInteraction.StateChangedEventArgs> ScaleModeGazeStateChanged
        {
            add { GetGazeButtonControl(scaleMode).StateChanged += value; }
            remove { GetGazeButtonControl(scaleMode).StateChanged -= value; }
        }

        public event RoutedEventHandler DwellButtonClick
        {
            add { dwellSpeed.Click += value; }
            remove { dwellSpeed.Click -= value; }
        }

        public event System.EventHandler<Microsoft.Toolkit.Uwp.Input.GazeInteraction.StateChangedEventArgs> DwellButtonGazeStateChanged
        {
            add { GetGazeButtonControl(dwellSpeed).StateChanged += value; }
            remove { GetGazeButtonControl(dwellSpeed).StateChanged -= value; }
        }

        private void Decrement_Click(object sender, RoutedEventArgs e) { Octave = Math.Max(Octave - 12, 24); }
        private void Increment_Click(object sender, RoutedEventArgs e) { Octave = Math.Min(Octave + 12, 72); }

        public void UpdateLoopButton(bool loopOn)
        {
            Loop.IsChecked = loopOn;

            if (loopOn) Loop.Content = "\uE769";
            else Loop.Content = "\uE768";
        }

        private void UpdateDwellSpeedIcon(object sender, RoutedEventArgs e)
        {
            DwellSpeed = (DwellSpeed + 1) % 3;

            if (DwellSpeed == 0) speedIcon.Glyph = "\uEA79";
            else if (DwellSpeed == 1) speedIcon.Glyph = "\uEA5E";
            else speedIcon.Glyph = "\uE81F";
        }

        private void UpdateScaleModeText(object sender, RoutedEventArgs e)
        {
            string scaleModeText = GetScaleMode();
            if (scaleModeText == "Pentatonic") scaleText.Text = "Diatonic";
            else scaleText.Text = "Pentatonic";
        }

        public string GetScaleMode()
        {
            return scaleText.Text;
        }

        private GazeElement GetGazeButtonControl(Control c)
        {
            GazeElement gazeButtonControl;

            // First try to get the control
            gazeButtonControl = GazeInput.GetGazeElement(c);
            if (gazeButtonControl != null) return gazeButtonControl;

            // If it doesn't exist - make it
            gazeButtonControl = new GazeElement();
            GazeInput.SetGazeElement(c, gazeButtonControl);
            return gazeButtonControl;
        }
    }
}
