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
    public sealed partial class FullKeyGrid : UserControl
    {
        public FullKeyGrid()
        {
            this.InitializeComponent();
            this.KeysGazeStateChanged += KeyGridAnimations.GazeAnimate;
            this.KeysDwellProgressFeedback += KeyGridAnimations.RemoveDwellProgressFeedback;
        }

        public event RoutedEventHandler KeysClick
        {
            add
            {
                kf0_0.Click += value;
                kf0_1.Click += value;
                kf0_2.Click += value;
                kf0_3.Click += value;
                kf0_4.Click += value;
                kf0_5.Click += value;
                kf0_6.Click += value;
                kf0_7.Click += value;

                kf1_2.Click += value;
                kf1_3.Click += value;
                kf1_4.Click += value;
                kf1_5.Click += value;
                kf1_6.Click += value;
                kf1_7.Click += value;
                kf1_8.Click += value;
                kf1_9.Click += value;

                kf2_4.Click += value;
                kf2_5.Click += value;
                kf2_6.Click += value;
                kf2_7.Click += value;
                kf2_8.Click += value;
                kf2_9.Click += value;
                kf2_10.Click += value;
                kf2_11.Click += value;

                kf3_7.Click += value;
                kf3_8.Click += value;
                kf3_9.Click += value;
                kf3_10.Click += value;
                kf3_11.Click += value;
                kf3_12.Click += value;
                kf3_13.Click += value;
                kf3_14.Click += value;
            }

            remove
            {
                kf0_0.Click -= value;
                kf0_1.Click -= value;
                kf0_2.Click -= value;
                kf0_3.Click -= value;
                kf0_4.Click -= value;
                kf0_5.Click -= value;
                kf0_6.Click -= value;
                kf0_7.Click -= value;

                kf1_2.Click -= value;
                kf1_3.Click -= value;
                kf1_4.Click -= value;
                kf1_5.Click -= value;
                kf1_6.Click -= value;
                kf1_7.Click -= value;
                kf1_8.Click -= value;
                kf1_9.Click -= value;

                kf2_4.Click -= value;
                kf2_5.Click -= value;
                kf2_6.Click -= value;
                kf2_7.Click -= value;
                kf2_8.Click -= value;
                kf2_9.Click -= value;
                kf2_10.Click -= value;
                kf2_11.Click -= value;

                kf3_7.Click -= value;
                kf3_8.Click -= value;
                kf3_9.Click -= value;
                kf3_10.Click -= value;
                kf3_11.Click -= value;
                kf3_12.Click -= value;
                kf3_13.Click -= value;
                kf3_14.Click -= value;
            }
        }

        public event System.EventHandler<Microsoft.Toolkit.Uwp.Input.GazeInteraction.StateChangedEventArgs> KeysGazeStateChanged
        {
            add
            {
                KeyGridAnimations.GetGazeButtonControl(kf0_0).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(kf0_1).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(kf0_2).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(kf0_3).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(kf0_4).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(kf0_5).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(kf0_6).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(kf0_7).StateChanged += value;

                KeyGridAnimations.GetGazeButtonControl(kf1_2).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(kf1_3).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(kf1_4).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(kf1_5).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(kf1_6).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(kf1_7).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(kf1_8).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(kf1_9).StateChanged += value;

                KeyGridAnimations.GetGazeButtonControl(kf2_4).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(kf2_5).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(kf2_6).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(kf2_7).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(kf2_8).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(kf2_9).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(kf2_10).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(kf2_11).StateChanged += value;

                KeyGridAnimations.GetGazeButtonControl(kf3_7).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(kf3_8).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(kf3_9).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(kf3_10).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(kf3_11).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(kf3_12).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(kf3_13).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(kf3_14).StateChanged += value;
            }

            remove
            {
                KeyGridAnimations.GetGazeButtonControl(kf0_0).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(kf0_1).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(kf0_2).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(kf0_3).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(kf0_4).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(kf0_5).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(kf0_6).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(kf0_7).StateChanged -= value;

                KeyGridAnimations.GetGazeButtonControl(kf1_2).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(kf1_3).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(kf1_4).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(kf1_5).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(kf1_6).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(kf1_7).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(kf1_8).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(kf1_9).StateChanged -= value;

                KeyGridAnimations.GetGazeButtonControl(kf2_4).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(kf2_5).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(kf2_6).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(kf2_7).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(kf2_8).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(kf2_9).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(kf2_10).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(kf2_11).StateChanged -= value;

                KeyGridAnimations.GetGazeButtonControl(kf3_7).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(kf3_8).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(kf3_9).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(kf3_10).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(kf3_11).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(kf3_12).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(kf3_13).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(kf3_14).StateChanged -= value;
            }
        }

        public event System.EventHandler<Microsoft.Toolkit.Uwp.Input.GazeInteraction.DwellProgressEventArgs> KeysDwellProgressFeedback
        {
            add
            {
                KeyGridAnimations.GetGazeButtonControl(kf0_0).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(kf0_1).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(kf0_2).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(kf0_3).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(kf0_4).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(kf0_5).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(kf0_6).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(kf0_7).DwellProgressFeedback += value;

                KeyGridAnimations.GetGazeButtonControl(kf1_2).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(kf1_3).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(kf1_4).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(kf1_5).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(kf1_6).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(kf1_7).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(kf1_8).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(kf1_9).DwellProgressFeedback += value;

                KeyGridAnimations.GetGazeButtonControl(kf2_4).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(kf2_5).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(kf2_6).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(kf2_7).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(kf2_8).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(kf2_9).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(kf2_10).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(kf2_11).DwellProgressFeedback += value;

                KeyGridAnimations.GetGazeButtonControl(kf3_7).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(kf3_8).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(kf3_9).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(kf3_10).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(kf3_11).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(kf3_12).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(kf3_13).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(kf3_14).DwellProgressFeedback += value;
            }

            remove
            {
                KeyGridAnimations.GetGazeButtonControl(kf0_0).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(kf0_1).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(kf0_2).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(kf0_3).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(kf0_4).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(kf0_5).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(kf0_6).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(kf0_7).DwellProgressFeedback -= value;

                KeyGridAnimations.GetGazeButtonControl(kf1_2).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(kf1_3).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(kf1_4).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(kf1_5).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(kf1_6).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(kf1_7).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(kf1_8).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(kf1_9).DwellProgressFeedback -= value;

                KeyGridAnimations.GetGazeButtonControl(kf2_4).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(kf2_5).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(kf2_6).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(kf2_7).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(kf2_8).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(kf2_9).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(kf2_10).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(kf2_11).DwellProgressFeedback -= value;

                KeyGridAnimations.GetGazeButtonControl(kf3_7).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(kf3_8).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(kf3_9).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(kf3_10).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(kf3_11).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(kf3_12).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(kf3_13).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(kf3_14).DwellProgressFeedback -= value;
            }
        }

        public static void Bump(Button key)
        {
            float scaleAmnt = 1.1F;
            float cX = (float)(key.ActualWidth / 2);
            float cY = (float)(key.ActualHeight / 2);
            int dur = 50;

            key.Scale(scaleAmnt, scaleAmnt, cX, cY, dur).Then().Scale(1, 1, cX, cY, dur);
        }

        public void BumpGrid()
        {
            float scaleAmnt = 0.9F;
            float cX = (float)(KeyGrid.ActualWidth / 2);
            float cY = (float)(KeyGrid.ActualHeight / 2);
            int dur = 400;
            KeyGrid.Scale(scaleAmnt, scaleAmnt, cX, cY, dur).Then().Scale(1, 1, cX, cY, dur).Start();
        }

        public void SetMargins(Thickness mthick)
        {
            kf0_0.Margin = mthick;
            kf0_1.Margin = mthick;
            kf0_2.Margin = mthick;
            kf0_3.Margin = mthick;
            kf0_4.Margin = mthick;
            kf0_5.Margin = mthick;
            kf0_6.Margin = mthick;
            kf0_7.Margin = mthick;

            kf1_2.Margin = mthick;
            kf1_3.Margin = mthick;
            kf1_4.Margin = mthick;
            kf1_5.Margin = mthick;
            kf1_6.Margin = mthick;
            kf1_7.Margin = mthick;
            kf1_8.Margin = mthick;
            kf1_9.Margin = mthick;

            kf2_4.Margin = mthick;
            kf2_5.Margin = mthick;
            kf2_6.Margin = mthick;
            kf2_7.Margin = mthick;
            kf2_8.Margin = mthick;
            kf2_9.Margin = mthick;
            kf2_10.Margin = mthick;
            kf2_11.Margin = mthick;

            kf3_7.Margin = mthick;
            kf3_8.Margin = mthick;
            kf3_9.Margin = mthick;
            kf3_10.Margin = mthick;
            kf3_11.Margin = mthick;
            kf3_12.Margin = mthick;
            kf3_13.Margin = mthick;
            kf3_14.Margin = mthick;
        }

        public void SlowDwellTime() {
            GazeInput.SetFixationDuration(KeyGrid, new TimeSpan(0, 0, 0, 0, 200));
            GazeInput.SetDwellDuration(KeyGrid, new TimeSpan(0, 0, 0, 0, 200));
        }
        public void FastDwellTime() {
            GazeInput.SetFixationDuration(KeyGrid, new TimeSpan(0, 0, 0, 0, 150));
            GazeInput.SetDwellDuration(KeyGrid, new TimeSpan(0, 0, 0, 0, 0));
        }
        public void FastestDwellTime()
        {
            GazeInput.SetFixationDuration(KeyGrid, new TimeSpan(0, 0, 0, 0, 10));
            GazeInput.SetDwellDuration(KeyGrid, new TimeSpan(0, 0, 0, 0, 0));
        }

        private int GetPitchNumFromButton(string keyID)
        {
            return Convert.ToInt32(keyID.Split("_")[1]);
        }
    }
}
