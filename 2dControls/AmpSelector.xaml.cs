using Microsoft.Toolkit.Uwp.Input.GazeInteraction;
using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
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
    public sealed partial class AmpSelector : UserControl
    {
        public int State = 1;
        public SolidColorBrush ActiveColorBrush;

        public String Number
        {
            get { return num.Text;  }
            set { num.Text = value; }
        }
        public static readonly DependencyProperty NumberProp = DependencyProperty.Register("Number", typeof(string), typeof(AmpSelector), null);

        public event EventHandler OnToggled = delegate { };

        public AmpSelector()
        {
            this.InitializeComponent();
            ActiveColorBrush = new SolidColorBrush(Windows.UI.Colors.HotPink);

            GetGazeButtonControl(sel0).DwellProgressFeedback += RemoveDwellProgressFeedback;
            GetGazeButtonControl(sel1).DwellProgressFeedback += RemoveDwellProgressFeedback;
            GetGazeButtonControl(sel2).DwellProgressFeedback += RemoveDwellProgressFeedback;

            GetGazeButtonControl(sel0).StateChanged += GazeElement_StateChanged;
            GetGazeButtonControl(sel1).StateChanged += GazeElement_StateChanged;
            GetGazeButtonControl(sel2).StateChanged += GazeElement_StateChanged;
        }

        public void SetActiveColorScheme()
        {
            bcg0.Background = (SolidColorBrush)grid.Resources["on0"];
            bcg1.Background = (SolidColorBrush)grid.Resources["on1"];
            bcg2.Background = (SolidColorBrush)grid.Resources["on2"];

            circle.Fill = (SolidColorBrush)grid.Resources["circleOn"];
        }

        public void SetInactiveColorScheme()
        {
            bcg0.Background = (SolidColorBrush)grid.Resources["off0"];
            bcg1.Background = (SolidColorBrush)grid.Resources["off1"];
            bcg2.Background = (SolidColorBrush)grid.Resources["off2"];

            circle.Fill = (SolidColorBrush)grid.Resources["circleOff"];
        }

        public void Bump()
        {
            if (State == 0) return;

            float sz;
            float r = (float)(circleBehind.Width / 2);
            double attackDur = 40;
            double releaseDur = 400;


            if (State == 2) sz = 1.3f; else sz = 1.1f;
            circleBehind.Scale(sz, sz, r, r, attackDur).Then().Scale(1, 1, r, r, releaseDur).Start();

        }

        public void SetState(int newState)
        {
            State = newState;
            Animate(1 - State);
        }

        private void Button_Click0(object sender, RoutedEventArgs e)
        {
            SetState(2);
            OnToggled(this, EventArgs.Empty);
            AnimateActivate(sender as Button);
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            SetState(1);
            OnToggled(this, EventArgs.Empty);
            AnimateActivate(sender as Button);
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            SetState(0);
            OnToggled(this, EventArgs.Empty);
            AnimateActivate(sender as Button);
        }

        private void Animate(int position)
        {
            float pos = (float)(brdr.ActualHeight * position);

            brdr.Offset(0, pos, 300, 0, EasingType.Default).Start();
        }

        private void AnimateActivate(Button btn)
        {
            float wd = (float)(btn.ActualWidth / 2);
            float ht = (float)(btn.ActualHeight / 2);
            float big = 1.5f;
            float small = 1.0f;

            btn.Scale(big, big, wd, ht, 100, 0, EasingType.Default).Then().Scale(small, small, wd, ht, 100, 0, EasingType.Default).Start();
        }

        private void GazeElement_StateChanged(object sender, Microsoft.Toolkit.Uwp.Input.GazeInteraction.StateChangedEventArgs e)
        {
            Button btn = (sender as Button);

            float wd = (float)(btn.ActualWidth / 2);
            float ht = (float)(btn.ActualHeight / 2);
            float enterSize = 1.1f;

            if (e.PointerState == PointerState.Enter)
            {
                animateNumberIn();
                btn.Scale(enterSize, enterSize, wd, ht, 100, 0, EasingType.Default).Start();
            }
                

            if (e.PointerState == PointerState.Exit)
            {
                animateNumberOut();
                btn.Scale(1, 1, wd, ht, 500, 0, EasingType.Default).Start();
            }    
        }

        private void animateNumberIn() { num.Offset(0, -40, 200).Fade(1, 200).Start(); }
        private void animateNumberOut() { num.Fade(0, 3000).Offset(0, 0, 3000).Start(); }

        private void RemoveDwellProgressFeedback(object sender, DwellProgressEventArgs e)
        {
            e.Handled = true;
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
