using Microsoft.Toolkit.Uwp.Input.GazeInteraction;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Microsoft.Xaml.Interactivity;
using Microsoft.Toolkit.Uwp.UI.Animations.Behaviors;
using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarSymbols;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Input.Preview;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Text;
using System.Drawing;

namespace _2dControls
{
    // -------------------- SETUP --------------------
    public sealed partial class MainPage : Page
    {
        readonly GazeInputSourcePreview gazeInputSource;
        private List<SolidColorBrush> chaosColors;

        private MusicTrigger GlobalMusicTrigger;

        public WebView WebSynth;

        public MainPage()
        {
            gazeInputSource = GazeInputSourcePreview.GetForCurrentView();
            gazeInputSource.GazeMoved += GazeMoved; // Used only for the Chaos Controls

            Application.Current.Resources["ToggleButtonBackgroundChecked"] = Application.Current.Resources["SystemBaseMediumColor"];

            this.Loaded += MainPage_Loaded;
            this.InitializeComponent();
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Don't show animation when the app is first loaded - this still goes into effect too soon
            Implicit.SetShowAnimations(instrument, (AnimationCollection)Resources["PullFromLeft"]);

            chaosColors = new List<SolidColorBrush>()
            {
                (SolidColorBrush)Resources["red"], (SolidColorBrush)Resources["orange"],
                (SolidColorBrush)Resources["yellow"], (SolidColorBrush)Resources["lime"],
            };

            List<(string, string, string)> chaosText = new List<(string, string, string)>()
            {
                ("Attack + Release", "Attack", "Release"),  ("Delay", "Delay Feedback", "Delay Volume"),
                ("Volume + Filter", "Volume", "Filter Cutoff"), ("Chorus + Crush", "Chorus", "Crush")
            };

            for (int i = 0; i < 4; i++)
            {
                ChaosControl cc = new ChaosControl(500, 80, chaosColors[i], new SolidColorBrush((Windows.UI.Color)Application.Current.Resources["SystemBaseMediumColor"]), chaosText[i].Item1, chaosText[i].Item2, chaosText[i].Item3);
                Implicit.SetShowAnimations(cc.ChaosCanvas, (AnimationCollection)Resources["Shower"]);
                Implicit.SetHideAnimations(cc.ChaosCanvas, (AnimationCollection)Resources["Hider"]);
                Implicit.SetShowAnimations(cc.GhostCanvas, (AnimationCollection)Resources["Ghost-Shower"]);
                Implicit.SetHideAnimations(cc.GhostCanvas, (AnimationCollection)Resources["Ghost-Hider"]);

                Chaos.Children.Add(cc.GhostCanvas);
                Chaos.Children.Add(cc.ChaosCanvas);

                ChaosControlsPanel.Children.Add(cc.ChaosButtons);
            }

            // Navigate to the Web Synth
            // Since this only hanldes audio processing - it does not get added to the grid
            string src = "ms-appx-web:///Assets/web/index.html";
            WebSynth = new WebView(WebViewExecutionMode.SeparateProcess); // This seems to help reduce UI/Audio hangs
            WebSynth.NavigationCompleted += Navigation_Completed;
            WebSynth.Navigate(new Uri(src));
        }

        private void Navigation_Completed(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            // Add JS functions to Chaos Controls
            ChaosControl.GetAllControls()[0].AddJSFunction(sender, "changeAttackRelease");
            ChaosControl.GetAllControls()[0].SetEllipsePosition(0.15, 0.5);

            ChaosControl.GetAllControls()[1].AddJSFunction(sender, "changeDelayFeedbackVolume");
            ChaosControl.GetAllControls()[1].SetEllipsePosition(0, 1);

            ChaosControl.GetAllControls()[2].AddJSFunction(sender, "changeAccentFilter");
            ChaosControl.GetAllControls()[2].SetEllipsePosition(0.5, 0.5);

            ChaosControl.GetAllControls()[3].AddJSFunction(sender, "changeChorusCrush");
            ChaosControl.GetAllControls()[3].SetEllipsePosition(0.5, 1);

            ChaosControl.GetAllControls()[0].ControlActivatedOrDeactivated += delegate (object s, EventArgs ea) { ToggleEffectsVisibility(EffectsSettingButton, true); };
            ChaosControl.GetAllControls()[1].ControlActivatedOrDeactivated += delegate (object s, EventArgs ea) { ToggleEffectsVisibility(EffectsSettingButton, true); };
            ChaosControl.GetAllControls()[2].ControlActivatedOrDeactivated += delegate (object s, EventArgs ea) { ToggleEffectsVisibility(EffectsSettingButton, true); };
            ChaosControl.GetAllControls()[3].ControlActivatedOrDeactivated += delegate (object s, EventArgs ea) { ToggleEffectsVisibility(EffectsSettingButton, true); };

            // Set up the Trigger for handling all sequencing and timing
            GlobalMusicTrigger = new MusicTrigger(sender);
            GlobalMusicTrigger.TurnOnJS();

            // Associate the visual element in the bottom of the screen with the Pitch Number Sequence
            Bumperz.SetPitches(GlobalMusicTrigger.PitchNumSequence);

            // Add UI event listeners to respond to musical events/changes
            GlobalMusicTrigger.OnStopLoop += delegate (object s, EventArgs ea) { Bumperz.BumpUp(); };
            GlobalMusicTrigger.OnAttackSequencerOff += delegate (object s, EventArgs ea) { Bumperz.BumpDown(GlobalMusicTrigger.CurrentPitchBeat); };
            GlobalMusicTrigger.OnRelease += delegate (object s, EventArgs ea) { Bumperz.BumpUp(); };
            GlobalMusicTrigger.OnRelease += delegate (object s, EventArgs ea) { Bumperz.SetActive(GlobalMusicTrigger.CurrentPitchBeat); };
            GlobalMusicTrigger.OnScriptNotify += delegate (object s, EventArgs ea) { Bumperz.Bump(GlobalMusicTrigger.CurrentPitchBeat); };
            GlobalMusicTrigger.OnGenerateRandomSequence += delegate (object s, EventArgs ea) { Bumperz.ResetPositions(); };
            GlobalMusicTrigger.OnPitchPatternLengthChange += delegate (object s, EventArgs ea) { Bumperz.SetNumberOfBumpers(GlobalMusicTrigger.PitchPatternLength); };

            GlobalMusicTrigger.OnRhythmPatternLengthChange += delegate (object s, EventArgs ea) { RhythmPatternStatus.SetSequenceLength(GlobalMusicTrigger.RhythmPatternLength); };
            GlobalMusicTrigger.OnRhythmPatternLengthChange += delegate (object s, EventArgs ea) { rhythmGrid.SetSequenceLength(GlobalMusicTrigger.RhythmPatternLength); };
            GlobalMusicTrigger.OnBeatChange += delegate (object s, EventArgs ea) { RhythmPatternStatus.Flash(GlobalMusicTrigger.CurrentRhythmBeat); };
            GlobalMusicTrigger.OnBeatChange += delegate (object s, EventArgs ea) { rhythmGrid.Bump(GlobalMusicTrigger.CurrentRhythmBeat); };

            GlobalMusicTrigger.OnPitchPatternLengthChange += delegate (object s, EventArgs ea) { PitchPatternStatus.SetSequenceLength(GlobalMusicTrigger.PitchPatternLength); };
            GlobalMusicTrigger.OnBeatChange += delegate (object s, EventArgs ea) { PitchPatternStatus.Flash(GlobalMusicTrigger.CurrentPitchBeat); };

            // Start the user off with a random pitch sequence
            GlobalMusicTrigger.GenerateRandomSequence();
        }

        // -------------------- User Input and Music Functions --------------------
        // -------------------- --------------------------- -----------------------

        // -------------------- All/Multiple Pages --------------------
        // Play Button
        private void SyncLoopButtons(bool loopOn)
        {
            MINI.UpdateLoopButton(loopOn);
            MINI2.UpdateLoopButton(loopOn);
            Command.UpdateLoopButton(loopOn);
        }

        private void ToggleLoop(bool loopOn)
        {
            if (GlobalMusicTrigger == null) return;

            if (loopOn) GlobalMusicTrigger.StartLoop();
            else GlobalMusicTrigger.StopLoop();
        }

        private void Loop_StateChanged(object sender, StateChangedEventArgs e)
        {
            if (e.PointerState == PointerState.Exit)
            {
                bool loopOn = (sender as ToggleButton).IsChecked.HasValue && (sender as ToggleButton).IsChecked.Value;
                SyncLoopButtons(loopOn);
                ToggleLoop(loopOn);
            }
        }

        private void Loop_Click(object sender, RoutedEventArgs e)
        {
            bool loopOn = (sender as ToggleButton).IsChecked.HasValue && (sender as ToggleButton).IsChecked.Value;
            SyncLoopButtons(loopOn);
            ToggleLoop(loopOn);
        }

        // Header
        private void Header_Instrument_Click(object sender, RoutedEventArgs e)
        {
            // Ensure that the middle tab animates correctly
            Implicit.SetShowAnimations(sequence, (AnimationCollection)Resources["PullFromRight"]);
            Implicit.SetHideAnimations(sequence, (AnimationCollection)Resources["PushRight"]);

            instrument.Visibility = Visibility.Visible;
            sequence.Visibility = Visibility.Collapsed;
            effects.Visibility = Visibility.Collapsed;

            _instrument.FontWeight = FontWeights.Bold;
            _sequence.FontWeight = FontWeights.Normal;
            _effects.FontWeight = FontWeights.Normal;

            HeaderAnim.Offset(0).Start();

        }

        private void Header_Sequence_Click(object sender, RoutedEventArgs e)
        {
            instrument.Visibility = Visibility.Collapsed;
            sequence.Visibility = Visibility.Visible;
            effects.Visibility = Visibility.Collapsed;

            _instrument.FontWeight = FontWeights.Normal;
            _sequence.FontWeight = FontWeights.Bold;
            _effects.FontWeight = FontWeights.Normal;


            HeaderAnim.Offset((float)HeaderAnim.ActualWidth).Start();
        }

        private void Header_Effects_Click(object sender, RoutedEventArgs e)
        {
            Implicit.SetShowAnimations(sequence, (AnimationCollection)Resources["PullFromLeft"]);
            Implicit.SetHideAnimations(sequence, (AnimationCollection)Resources["PushLeft"]);

            instrument.Visibility = Visibility.Collapsed;
            sequence.Visibility = Visibility.Collapsed;
            effects.Visibility = Visibility.Visible;

            _instrument.FontWeight = FontWeights.Normal;
            _sequence.FontWeight = FontWeights.Normal;
            _effects.FontWeight = FontWeights.Bold;

            HeaderAnim.Offset((float)(HeaderAnim.ActualWidth * 2)).Start();
        }

        // -------------------- Instrument Page --------------------
        private void DwellSpeed_Click(object sender, RoutedEventArgs e)
        {
            if (Command.DwellSpeed == 0)
            {
                FullKeys.SlowDwellTime();
                PentatonicKeys.SlowDwellTime();
            }
            else if (Command.DwellSpeed == 1)
            {
                FullKeys.FastDwellTime();
                PentatonicKeys.FastDwellTime();
            }
            else
            {
                FullKeys.FastestDwellTime();
                PentatonicKeys.FastestDwellTime();
            }
        }

        private void Key_Gaze(object sender, StateChangedEventArgs e)
        {
            if (GlobalMusicTrigger == null) return;

            if (e.PointerState == PointerState.Dwell)
            {
                int pitchNum = GetPitchNumFromButton((sender as Button).Name.ToString());
                GlobalMusicTrigger.Attack(pitchNum);
            }
                
            if (e.PointerState == PointerState.Exit) GlobalMusicTrigger.Release();
        }

        // The pitch number is behind an "_"
        private int GetPitchNumFromButton(string keyID)
        {
            return Convert.ToInt32(keyID.Split("_")[1]);
        }

        // The button numbering system is different on the pentatonic key grid than the other two
        // Ultimately for future expansions - this system is probably more robust
        private void Pentatonic_Key_Gaze(object sender, StateChangedEventArgs e)
        {
            if (GlobalMusicTrigger == null) return;

            if (e.PointerState == PointerState.Dwell)
            {
                int pitchNum = PentatonicKeys.GetKeyNum((sender as Button).Name.ToString());
                GlobalMusicTrigger.Attack(pitchNum);
            }

            if (e.PointerState == PointerState.Exit) GlobalMusicTrigger.Release();
        }

        private void Set_Octave(object sender, EventArgs e)
        {
            if (GlobalMusicTrigger != null) GlobalMusicTrigger.SetOctave((int)sender);
        }

        private void scaleMode_Click(object sender, RoutedEventArgs e)
        {
            string scaleModeText = Command.GetScaleMode();
            if (scaleModeText == "Pentatonic")
            {
                PentatonicKeys.Visibility = Visibility.Visible;
                FullKeys.Visibility = Visibility.Collapsed;
            }
            else
            {
                PentatonicKeys.Visibility = Visibility.Collapsed;
                FullKeys.Visibility = Visibility.Visible;
            }
        }

        private void ScaleSelector_KeyChanged(object sender, KeyChangedEventArgs e)
        {
            KeyExpander.Header = $"Current Key: {e.KeyLetter} {e.ScaleType}";
            PentatonicKeys.ChangeScale(e.ScaleType);

            if (GlobalMusicTrigger == null) return;
            GlobalMusicTrigger.UpdateMusicParams(e.Root, e.Key, e.Scale);
        }


        // -------------------- Sequencer Page --------------------
        private void Set_Tempo(object sender, EventArgs e)
        {
            if (GlobalMusicTrigger != null) GlobalMusicTrigger.UpdateTempo((int)sender);
        }

        private void SequencePalette_PitchPatternLengthChanged(object sender, EventArgs e)
        {
            int newPatternLength = (int)sender;

            if (GlobalMusicTrigger == null) return;
            GlobalMusicTrigger.SetPitchPatternLength(newPatternLength);
        }

        private void SequencePalette_RhythmPatternLengthChanged(object sender, EventArgs e)
        {
            int newPatternLength = (int)sender;

            if (GlobalMusicTrigger == null) return;
            GlobalMusicTrigger.SetRhythmPatternLength(newPatternLength);
        }

        private void RhythmGrid_RhythmGridChanged(object sender, RhythmGridChangedEventArgs e)
        {
            if (GlobalMusicTrigger == null) return;
            GlobalMusicTrigger.SetRhythm(e.ToggleNumber, e.ToggleState);
        }


        // -------------------- Effects Page --------------------
        private void GazeMoved(GazeInputSourcePreview sender, GazeMovedPreviewEventArgs args)
        {
            ChaosControl.GetActiveControl().MoveEllipse(args.CurrentPoint.EyeGazePosition.Value.X, args.CurrentPoint.EyeGazePosition.Value.Y);
        }

        private void ToggleEffectsVisibility(Button buttonToToggle, bool DefinitelyShowChaos=false)
        {
            if (DefinitelyShowChaos && Chaos.Visibility == Visibility.Visible) return;
            if (Chaos.Visibility == Visibility.Collapsed)
            {
                buttonToToggle.Background = new SolidColorBrush((Windows.UI.Color)Application.Current.Resources["SystemBaseLowColor"]);
                buttonToToggle.Foreground = new SolidColorBrush((Windows.UI.Color)Resources["SystemBaseHighColor"]);

                Chaos.Visibility = Visibility.Visible;
                EffectSettings.Visibility = Visibility.Collapsed;
            }
            else if (!DefinitelyShowChaos)
            {
                buttonToToggle.Background = new SolidColorBrush((Windows.UI.Color)Application.Current.Resources["SystemBaseMediumColor"]);
                buttonToToggle.Foreground = new SolidColorBrush((Windows.UI.Color)Resources["SystemAltHighColor"]);
                
                Chaos.Visibility = Visibility.Collapsed;
                EffectSettings.Visibility = Visibility.Visible;
            }
        }

        private void EffectsSettingButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleEffectsVisibility(sender as Button);
        }

        private void Osc_SelectorChanged(object sender, SelectorChangedEventArgs e)
        {
            if (GlobalMusicTrigger == null) return;
            GlobalMusicTrigger.ChangeOscillator(e.SelectedID);
        }

        private void DelayTime_SelectorChanged(object sender, SelectorChangedEventArgs e)
        {
            if (GlobalMusicTrigger == null) return;
            GlobalMusicTrigger.ChangeDelayTime(e.SelectedID);
        }

        private void ChorusFreq_SelectorChanged(object sender, SelectorChangedEventArgs e)
        {
            if (GlobalMusicTrigger == null) return;
            GlobalMusicTrigger.ChangeChorusFrequency(e.SelectedID);
        }
    }
}
