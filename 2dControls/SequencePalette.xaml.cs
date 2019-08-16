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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace _2dControls
{
    public sealed partial class SequencePalette : UserControl
    {
        public event EventHandler PitchPatternLengthChanged = delegate { };
        private int _pitchPatternLength;
        public int PitchPatternLength
        {
            get { return _pitchPatternLength; }
            set
            {
                _pitchPatternLength = value;
                UpdatePitchPatternLengthText(value);
                PitchPatternLengthChanged(value, EventArgs.Empty);
            }
        }

        public event EventHandler RhythmPatternLengthChanged = delegate { };
        private int _rhythmPatternLength;
        public int RhythmPatternLength
        {
            get { return _rhythmPatternLength; }
            set
            {
                _rhythmPatternLength = value;
                UpdateRhythmPatternLengthText(value);
                RhythmPatternLengthChanged(value, EventArgs.Empty);
            }
        }

        public event EventHandler TempoChanged = delegate { };
        private int _tempo;
        public int Tempo
        {
            get { return _tempo; }
            set
            {
                _tempo = value;
                UpdateTempoText(value);
                TempoChanged(value, EventArgs.Empty);
            }
        }

        public SequencePalette()
        {
            this.InitializeComponent();
            Tempo = 120;
            PitchPatternLength = 8;
            RhythmPatternLength = 8;
        }

        private void UpdatePitchPatternLengthText(int value)
        {
            PitchPatternLengthText.Text = value.ToString();
        }

        private void UpdateRhythmPatternLengthText(int value)
        {
            RhythmPatternLengthText.Text = value.ToString();
        }

        public void UpdateTempoText(int tempo)
        {
            TempoText.Text = tempo.ToString();
        }

        private void Decrement_Click(object sender, RoutedEventArgs e) { Tempo = Math.Max(Tempo - 5, 5); }
        private void Increment_Click(object sender, RoutedEventArgs e) { Tempo = Math.Min(Tempo + 5, 500); }

        private void Decrement_ClickP(object sender, RoutedEventArgs e) { PitchPatternLength = Math.Max(PitchPatternLength - 1, 1); }
        private void Increment_ClickP(object sender, RoutedEventArgs e) { PitchPatternLength = Math.Min(PitchPatternLength + 1, 16); }

        private void Decrement_ClickR(object sender, RoutedEventArgs e) { RhythmPatternLength = Math.Max(RhythmPatternLength - 1, 1); }
        private void Increment_ClickR(object sender, RoutedEventArgs e) { RhythmPatternLength = Math.Min(RhythmPatternLength + 1, 16); }
    }
}
