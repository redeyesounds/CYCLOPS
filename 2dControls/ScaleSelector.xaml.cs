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
    public class KeyChangedEventArgs : EventArgs
    {
        private readonly int _root;
        private readonly int _key;
        private readonly string _keyLetter;
        private readonly List<int> _scale;
        private readonly string _scaleType;

        public KeyChangedEventArgs(int root, int key, string keyLetter, List<int> scale, string scaleType)
        {
            _root = root;
            _key = key;
            _keyLetter = keyLetter;
            _scale = scale;
            _scaleType = scaleType;
        }

        public int Root { get { return _root; } }
        public int Key { get { return _key; } }
        public string KeyLetter { get { return _keyLetter; } }
        public List<int> Scale { get { return _scale; } }
        public string ScaleType { get { return _scaleType;  } }
    }

    public sealed partial class ScaleSelector : UserControl
    {
        public event EventHandler<KeyChangedEventArgs> KeyChanged;

        public int Root = 48;
        public int Key = 0; // Ranges from 0 to 11

        private string scaleMode = "major";
        private bool selectedScaleInMode = true; // used for ensuring that minor/major selected scale is not confused

        private static readonly Dictionary<string, List<int>> DiatonicScales = new Dictionary<string, List<int>>()
        {
            {"major", new List<int>(){0, 2, 4, 5, 7, 9, 11 } },
            {"minor", new List<int>(){0, 2, 3, 5, 7, 8, 10 } }
        };

        private static readonly Dictionary<string, int> PitchClasses = new Dictionary<string, int>()
        {
            {"c", 0 }, {"db", 1 }, {"d", 2 }, {"eb", 3 }, {"e", 4 }, {"f", 5 },
            {"gb", 6 }, {"g", 7 }, {"ab", 8 }, {"a", 9 }, {"bb", 10 }, {"b", 11 }
        };

        private Button CurrentlyActivated;
        private List<Button> ScaleButtons = new List<Button>();

        private Random rand = new Random();

        public ScaleSelector()
        {
            this.InitializeComponent();
            SetMajorMode();
        }

        // --------------- User Input ---------------
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (sender as Button);
            if (CurrentlyActivated != btn) AnimateDeactivate(CurrentlyActivated);


            AnimateActivate(btn);
            Key = GetKey(btn);

            KeyChanged?.Invoke(this, new KeyChangedEventArgs(Root, Key, btn.Content.ToString(), DiatonicScales[scaleMode], scaleMode));

            selectedScaleInMode = true;
            CurrentlyActivated = (sender as Button);
        }

        private void ScaleToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleSwitch).IsOn) SetMinorMode();
            else SetMajorMode();

            selectedScaleInMode = !selectedScaleInMode;
            if (selectedScaleInMode) AnimateActivate(CurrentlyActivated);
            else AnimateDeactivate(CurrentlyActivated);

            ScaleButtons.ForEach(sb => { ToggleAnimation(sb); });
        }

        private void GazeElement_DwellProgressFeedback(object sender, Microsoft.Toolkit.Uwp.Input.GazeInteraction.DwellProgressEventArgs e)
        {
            e.Handled = true;
        }

        // Primarily handles visual appearance when user looks at each button
        private void GazeElement_StateChanged(object sender, Microsoft.Toolkit.Uwp.Input.GazeInteraction.StateChangedEventArgs e)
        {
            Button btn = (sender as Button);
            if (btn == CurrentlyActivated && selectedScaleInMode) return;

            float radius = (float)(btn.Width / 2);
            float enterSize = 1.1f;

            if (e.PointerState == PointerState.Enter)
                btn.Scale(enterSize, enterSize, radius, radius, 100, 0, EasingType.Default).Start();

            if (e.PointerState == PointerState.Exit)
                btn.Scale(1, 1, radius, radius, 500, 0, EasingType.Default).Start();
        }

        // --------------- Music Theory ---------------
        private void SetMajorMode()
        {
            s0.Label = "C";
            s1.Label = "G";
            s2.Label = "D";
            s3.Label = "A";
            s4.Label = "E";
            s5.Label = "B";
            s6.Label = "Gb";
            s7.Label = "Db";
            s8.Label = "Ab";
            s9.Label = "Eb";
            s10.Label = "Bb";
            s11.Label = "F";

            scaleMode = "major";
        }

        private void SetMinorMode()
        {
            s0.Label = "a";
            s1.Label = "e";
            s2.Label = "b";
            s3.Label = "gb";
            s4.Label = "db";
            s5.Label = "ab";
            s6.Label = "eb";
            s7.Label = "bb";
            s8.Label = "f";
            s9.Label = "c";
            s10.Label = "g";
            s11.Label = "d";

            scaleMode = "minor";
        }

        private int GetKey(Button selectedKey) { return PitchClasses[selectedKey.Content.ToString().ToLower()]; }


        // --------------- UI Animations ---------------
        private void AnimateActivate(Button btn)
        {
            float radius = (float)(btn.Width / 2);
            float big = 1.5f;
            float small = 1.2f;

            btn.Scale(big, big, radius, radius, 100, 0, EasingType.Default).Then().Scale(small, small, radius, radius, 100, 0, EasingType.Default).Start();

            btn.Background = new SolidColorBrush((Windows.UI.Color)Resources["SystemBaseMediumHighColor"]);
            btn.Foreground = new SolidColorBrush((Windows.UI.Color)Resources["SystemAltHighColor"]);
        }

        private void AnimateDeactivate(Button btn)
        {
            if (btn == null) return;

            float radius = (float)(btn.Width / 2);

            btn.Background = new SolidColorBrush((Windows.UI.Color)Resources["SystemBaseLowColor"]);
            btn.Foreground = new SolidColorBrush((Windows.UI.Color)Resources["SystemBaseHighColor"]);

            btn.Scale(1, 1, radius, radius, 100, 0, EasingType.Default).Start();
        }

        private void ToggleAnimation(Button btn)
        {
            float radius = (float)(btn.Width / 2);
            int delay = rand.Next(100);
            float resize = 1f;

            if (selectedScaleInMode && btn == CurrentlyActivated) resize = 1.2f;

            btn.Scale(0, 0, radius, radius, 200, delay, EasingType.Default).Then().Scale(resize, resize, radius, radius, 200, delay, EasingType.Default).Then().Start();
        }

        private void Button_Loaded(object sender, RoutedEventArgs e)
        {
            Button btn = (sender as Button);
            ScaleButtons.Add(btn);

            if (GetKey(btn) == Key)
            {
                CurrentlyActivated = btn;
                AnimateActivate(btn);
            }
        }
    }
}
