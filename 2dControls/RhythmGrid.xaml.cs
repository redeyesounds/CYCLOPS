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

    public class RhythmGridChangedEventArgs : EventArgs
    {

        private readonly int _toggleNumber;
        private readonly int _toggleState;

        public RhythmGridChangedEventArgs(int toggleNumber, int toggleState)
        {
            _toggleNumber = toggleNumber;
            _toggleState = toggleState;
        }

        public int ToggleNumber { get { return _toggleNumber; } }
        public int ToggleState { get { return _toggleState; } }
    }

    public sealed partial class RhythmGrid : UserControl
    {
        private readonly AmpSelector[] ampSelectors;
        public event EventHandler<RhythmGridChangedEventArgs> RhythmGridChanged;

        public RhythmGrid()
        {
            this.InitializeComponent();
            ampSelectors = new AmpSelector[]
            {
                a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15
            };
            SetSequenceLength(8);
        }

        public void SetSequenceLength(int sequenceLength)
        {
            for (int i = 0; i < ampSelectors.Length; i++)
            {
                if (i < sequenceLength) ampSelectors[i].SetActiveColorScheme();
                else ampSelectors[i].SetInactiveColorScheme();
            }
        }

        public void Bump(int beatToBump) { ampSelectors[beatToBump].Bump(); }

        private void A_OnToggled(object sender, EventArgs e)
        {
            int toggleState = (sender as AmpSelector).State;
            int toggleNumber = Int32.Parse((sender as AmpSelector).Name.Substring(1));

            RhythmGridChanged?.Invoke(this, new RhythmGridChangedEventArgs(toggleNumber, toggleState));
        }
    }
}
