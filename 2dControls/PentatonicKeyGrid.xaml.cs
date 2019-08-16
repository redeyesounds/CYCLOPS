using Microsoft.Toolkit.Uwp.Input.GazeInteraction;
using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public sealed partial class PentatonicKeyGrid : UserControl
    {
        private readonly Button[,] allKeys;

        // These are the scale degrees to use
        private static readonly int[,] MajorPent = new int[4, 6]
        {
            {1, 2, 3, 5, 6, 8 },
            {3, 5, 6, 8, 9, 10 },
            {5, 6, 8, 9, 10, 12 },
            {8, 9, 10, 12, 13, 15 }
        };

        private static readonly int[,] MinorPent = new int[4, 6]
        {
            {1, 3, 4, 5, 7, 8 },
            {3, 4, 5, 7, 8, 10 },
            {5, 7, 8, 10, 11, 12 },
            {8, 10, 11, 12, 14, 15 }
        };

        private static readonly Dictionary<string, int[,]> PentatonicScales = new Dictionary<string, int[,]>()
        {
            { "major",  MajorPent },
            { "minor",  MinorPent }
        };
        private string CurrentScale = "major";

        public PentatonicKeyGrid()
        {
            this.InitializeComponent();
            this.KeysDwellProgressFeedback += KeyGridAnimations.RemoveDwellProgressFeedback;
            this.KeysGazeStateChanged += KeyGridAnimations.GazeAnimate;

            allKeys = new Button[4, 6]
            {
                {k0_0, k0_1, k0_2, k0_3, k0_4, k0_5 },
                {k1_0, k1_1, k1_2, k1_3, k1_4, k1_5 },
                {k2_0, k2_1, k2_2, k2_3, k2_4, k2_5 },
                {k3_0, k3_1, k3_2, k3_3, k3_4, k3_5 }
            };
        }

        public void ChangeScale(string scaleMode)
        {
            if (scaleMode == CurrentScale) return;
            if (scaleMode != "major" && scaleMode != "minor") return;

            CurrentScale = scaleMode;

            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 6; c++)
                {
                    int scaleDegree = (PentatonicScales[CurrentScale][r, c] % 7);
                    if (scaleDegree == 0) scaleDegree = 7;
                    allKeys[r, c].Content = scaleDegree.ToString();
                }
            }     
        }

        public event RoutedEventHandler KeysClick
        {
            add
            {
                k0_0.Click += value;
                k0_1.Click += value;
                k0_2.Click += value;
                k0_3.Click += value;
                k0_4.Click += value;
                k0_5.Click += value;

                k1_0.Click += value;
                k1_1.Click += value;
                k1_2.Click += value;
                k1_3.Click += value;
                k1_4.Click += value;
                k1_5.Click += value;

                k2_0.Click += value;
                k2_1.Click += value;
                k2_2.Click += value;
                k2_3.Click += value;
                k2_4.Click += value;
                k2_5.Click += value;

                k3_0.Click += value;
                k3_1.Click += value;
                k3_2.Click += value;
                k3_3.Click += value;
                k3_4.Click += value;
                k3_5.Click += value;
            }

            remove
            {
                k0_0.Click -= value;
                k0_1.Click -= value;
                k0_2.Click -= value;
                k0_3.Click -= value;
                k0_4.Click -= value;
                k0_5.Click -= value;

                k1_0.Click -= value;
                k1_1.Click -= value;
                k1_2.Click -= value;
                k1_3.Click -= value;
                k1_4.Click -= value;
                k1_5.Click -= value;

                k2_0.Click -= value;
                k2_1.Click -= value;
                k2_2.Click -= value;
                k2_3.Click -= value;
                k2_4.Click -= value;
                k2_5.Click -= value;

                k3_0.Click -= value;
                k3_1.Click -= value;
                k3_2.Click -= value;
                k3_3.Click -= value;
                k3_4.Click -= value;
                k3_5.Click -= value;
            }
        }

        public event System.EventHandler<Microsoft.Toolkit.Uwp.Input.GazeInteraction.StateChangedEventArgs> KeysGazeStateChanged
        {
            add
            {
                KeyGridAnimations.GetGazeButtonControl(k0_0).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(k0_1).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(k0_2).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(k0_3).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(k0_4).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(k0_5).StateChanged += value;

                KeyGridAnimations.GetGazeButtonControl(k1_0).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(k1_1).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(k1_2).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(k1_3).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(k1_4).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(k1_5).StateChanged += value;

                KeyGridAnimations.GetGazeButtonControl(k2_0).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(k2_1).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(k2_2).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(k2_3).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(k2_4).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(k2_5).StateChanged += value;

                KeyGridAnimations.GetGazeButtonControl(k3_0).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(k3_1).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(k3_2).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(k3_3).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(k3_4).StateChanged += value;
                KeyGridAnimations.GetGazeButtonControl(k3_5).StateChanged += value;
            }

            remove
            {
                KeyGridAnimations.GetGazeButtonControl(k0_0).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(k0_1).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(k0_2).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(k0_3).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(k0_4).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(k0_5).StateChanged -= value;

                KeyGridAnimations.GetGazeButtonControl(k1_0).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(k1_1).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(k1_2).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(k1_3).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(k1_4).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(k1_5).StateChanged -= value;

                KeyGridAnimations.GetGazeButtonControl(k2_0).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(k2_1).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(k2_2).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(k2_3).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(k2_4).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(k2_5).StateChanged -= value;

                KeyGridAnimations.GetGazeButtonControl(k3_0).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(k3_1).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(k3_2).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(k3_3).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(k3_4).StateChanged -= value;
                KeyGridAnimations.GetGazeButtonControl(k3_5).StateChanged -= value;
            }
        }

        public event System.EventHandler<Microsoft.Toolkit.Uwp.Input.GazeInteraction.DwellProgressEventArgs> KeysDwellProgressFeedback
        {
            add
            {
                KeyGridAnimations.GetGazeButtonControl(k0_0).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(k0_1).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(k0_2).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(k0_3).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(k0_4).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(k0_5).DwellProgressFeedback += value;

                KeyGridAnimations.GetGazeButtonControl(k1_0).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(k1_1).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(k1_2).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(k1_3).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(k1_4).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(k1_5).DwellProgressFeedback += value;

                KeyGridAnimations.GetGazeButtonControl(k2_0).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(k2_1).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(k2_2).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(k2_3).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(k2_4).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(k2_5).DwellProgressFeedback += value;

                KeyGridAnimations.GetGazeButtonControl(k3_0).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(k3_1).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(k3_2).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(k3_3).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(k3_4).DwellProgressFeedback += value;
                KeyGridAnimations.GetGazeButtonControl(k3_5).DwellProgressFeedback += value;
            }

            remove
            {
                KeyGridAnimations.GetGazeButtonControl(k0_0).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(k0_1).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(k0_2).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(k0_3).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(k0_4).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(k0_5).DwellProgressFeedback -= value;

                KeyGridAnimations.GetGazeButtonControl(k1_0).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(k1_1).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(k1_2).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(k1_3).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(k1_4).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(k1_5).DwellProgressFeedback -= value;

                KeyGridAnimations.GetGazeButtonControl(k2_0).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(k2_1).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(k2_2).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(k2_3).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(k2_4).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(k2_5).DwellProgressFeedback -= value;

                KeyGridAnimations.GetGazeButtonControl(k3_0).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(k3_1).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(k3_2).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(k3_3).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(k3_4).DwellProgressFeedback -= value;
                KeyGridAnimations.GetGazeButtonControl(k3_5).DwellProgressFeedback -= value;
            }
        }

        // The next four functions are identical to counterparts in the Full KeyGrid
        public void BumpGrid()
        {
            float scaleAmnt = 0.9F;
            float cX = (float)(KeyGrid.ActualWidth / 2);
            float cY = (float)(KeyGrid.ActualHeight / 2);
            int dur = 400;
            KeyGrid.Scale(scaleAmnt, scaleAmnt, cX, cY, dur).Then().Scale(1, 1, cX, cY, dur).Start();
        }

        public void SlowDwellTime()
        {
            GazeInput.SetFixationDuration(KeyGrid, new TimeSpan(0, 0, 0, 0, 200));
            GazeInput.SetDwellDuration(KeyGrid, new TimeSpan(0, 0, 0, 0, 200));
        }
        public void FastDwellTime()
        {
            GazeInput.SetFixationDuration(KeyGrid, new TimeSpan(0, 0, 0, 0, 150));
            GazeInput.SetDwellDuration(KeyGrid, new TimeSpan(0, 0, 0, 0, 0));
        }
        public void FastestDwellTime()
        {
            GazeInput.SetFixationDuration(KeyGrid, new TimeSpan(0, 0, 0, 0, 10));
            GazeInput.SetDwellDuration(KeyGrid, new TimeSpan(0, 0, 0, 0, 0));
        }

        // This is a slightly different implementation than the full key grid 
        // since the numbers on the keys can change
        public int GetKeyNum(string keyID)
        {
            string keyPos = keyID.Substring(1);
            int row = Convert.ToInt32(keyPos.Split("_")[0]);
            int col = Convert.ToInt32(keyPos.Split("_")[1]);
            return PentatonicScales[CurrentScale][row, col] - 1;
        }
    }
}
