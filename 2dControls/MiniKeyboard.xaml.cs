using Microsoft.Toolkit.Uwp.Input.GazeInteraction;
using Microsoft.Toolkit.Uwp.UI.Animations;
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
    public sealed partial class MiniKeyboard : UserControl
    {
        public MiniKeyboard()
        {
            this.InitializeComponent();
            this.KeysGazeStateChanged += KeyGridAnimations.GazeAnimate;
            this.KeysDwellProgressFeedback += KeyGridAnimations.RemoveDwellProgressFeedback;
        }

        public event RoutedEventHandler PlayButtonClick
        {
            add { Loop.Click += value; }
            remove { Loop.Click -= value; }
        }

        public event System.EventHandler<Microsoft.Toolkit.Uwp.Input.GazeInteraction.StateChangedEventArgs> PlayButtonGazeStateChanged
        {
            add { KeyGridAnimations.GetGazeButtonControl(Loop).StateChanged += value; }
            remove { KeyGridAnimations.GetGazeButtonControl(Loop).StateChanged -= value; }
        }

        public event RoutedEventHandler KeysClick
        {
            add
            {
                km_0.Click += value;
                km_1.Click += value;
                km_3.Click += value;
                km_4.Click += value;
                km_5.Click += value;
            }
            remove
            {
                km_0.Click -= value;
                km_1.Click -= value;
                km_3.Click -= value;
                km_4.Click -= value;
                km_5.Click -= value;
            }
        }

        public event System.EventHandler<Microsoft.Toolkit.Uwp.Input.GazeInteraction.StateChangedEventArgs> KeysGazeStateChanged
        {
            add
            {
                KeyGridAnimations.GetGazeButtonControl(km_0).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(km_1).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(km_3).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(km_4).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(km_5).StateChanged += value;
            }
            remove
            {
                KeyGridAnimations.GetGazeButtonControl(km_0).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(km_1).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(km_3).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(km_4).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(km_5).StateChanged -= value;
            }
        }

        public event System.EventHandler<Microsoft.Toolkit.Uwp.Input.GazeInteraction.DwellProgressEventArgs> KeysDwellProgressFeedback
        {
            add
            {
                KeyGridAnimations.GetGazeButtonControl(km_0).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(km_1).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(km_3).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(km_4).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(km_5).DwellProgressFeedback += value;
            }
            remove
            {
                KeyGridAnimations.GetGazeButtonControl(km_0).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(km_1).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(km_3).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(km_4).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(km_5).DwellProgressFeedback -= value;
            }
        }

        // Ensure when one play button is pressed that all match its state
        public void UpdateLoopButton(bool loopOn)
        {
            Loop.IsChecked = loopOn;

            if (loopOn) Loop.Content = "\uE769";
            else Loop.Content = "\uE768";
        }

    }
}
