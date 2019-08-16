using Microsoft.Toolkit.Uwp.Input.GazeInteraction;
using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace _2dControls
{
    // This class ensures that all of the KeyGrids (FullKeyGrid, MiniKeyboard, PentatonicKeyGrid) animate the same way
    class KeyGridAnimations
    {
        public static void RemoveDwellProgressFeedback(object sender, DwellProgressEventArgs e)
        {
            e.Handled = true;
        }

        public static void GazeAnimate(object sender, StateChangedEventArgs e)
        {
            Button btn = sender as Button;
            float cX = (float)(btn.ActualWidth / 2);
            float cY = (float)(btn.ActualHeight / 2);
            float scaleVal = 1.2f;

            if (e.PointerState == PointerState.Fixation)
                btn.Background = btn.BorderBrush;

            if (e.PointerState == PointerState.Dwell)
                btn.Scale(scaleVal, scaleVal, cX, cY, 100, 0, EasingType.Elastic).Start();

            if (e.PointerState == PointerState.Exit)
            {
                btn.Scale(1, 1, cX, cY, 1000, 0, EasingType.Quartic).Start();
                btn.Background = new SolidColorBrush((Windows.UI.Color)Application.Current.Resources["SystemBaseLowColor"]);
            }
        }

        public static GazeElement GetGazeButtonControl(Control c)
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
