using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Toolkit.Uwp.Input.GazeInteraction;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarSymbols;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace _2dControls
{
    public class ChaosControl
    {
        // Static Variables
        public static int ActiveControl = 0;

        private static int NumberOfControls = 0;
        private static readonly List<ChaosControl> AllControls = new List<ChaosControl>();
        private static double CanvasSizeIncrease = 1.05;

        // Static Methods
        public static ChaosControl GetActiveControl() { return AllControls[ActiveControl]; }
        public static List<ChaosControl> GetAllControls() { return AllControls; }

        private static void ShowGhosts() { AllControls.ForEach(cc => { cc.GhostCanvas.Visibility = Visibility.Visible; }); }
        private static void HideGhosts() { AllControls.ForEach(cc => { cc.GhostCanvas.Visibility = Visibility.Collapsed; }); }

        // Instance Variables
        public int InstanceNumber;
        public bool Active = false;
        public bool GazeOverCanvas = false;
        private bool CanAnimate = true;

        public double EllipseX = 0; // Should range from 0 to 1
        public double EllipseY = 0;

        private List<Tuple<double, double>> Snapshots = new List<Tuple<double, double>>();
        private List<Tuple<double, double>> Temporary_Snapshots = new List<Tuple<double, double>>();
        private int CurrentSnapshot = 0;
        public bool PlaySnapshots = false;
        private readonly DispatcherTimer SnapshotPlayer;

        private string JSFunctionName = "";
        private WebView Synth;

        // Instance UI Elements (Not all these need to be public)
        private SolidColorBrush ChaosForegroundColor;
        private SolidColorBrush ChaosBackgroundColor;

        public Canvas ChaosCanvas;
        public Border ChaosBorder;
        public DropShadowPanel ChaosShadow;
        public Ellipse ChaosEllipse;
        private Ellipse TargetEllipse;
        private TextBlock InnerNotifier;
        public Button ChaosButton;
        public TextBlock XLabel;
        public TextBlock YLabel;

        public Canvas GhostCanvas;
        private DropShadowPanel GhostShadow;
        private Ellipse GhostEllipse;

        public Canvas TinyCanvas;
        public Border TinyBorder;
        public Ellipse TinyEllipse;
        public Button TinyButton;

        public ToggleButton SnapshotButton;

        private StackPanel ChaosButtonsNoLabel;
        public StackPanel ChaosButtons;
        public TextBlock ChaosTitle;

        public InAppNotification ChaosNotifier;

        // Events
        // This is used to ensure settings are correctly hidden when the user clicks a button that
        // should effect chaos controls
        public event EventHandler<EventArgs> ControlActivatedOrDeactivated;

        public ChaosControl(int largeSize, int tinySize, SolidColorBrush foregroundColor, SolidColorBrush backgroundColor, string title="", string xLabel="", string yLabel="")
        {
            // -------- Parameters --------
            InstanceNumber = NumberOfControls;
            NumberOfControls++;

            // -------- UI Elements --------
            ChaosForegroundColor = foregroundColor;
            ChaosBackgroundColor = backgroundColor;

            ChaosCanvas = new Canvas();
            ChaosCanvas.Width = largeSize;
            ChaosCanvas.Height = largeSize;
            ChaosCanvas.Visibility = Visibility.Collapsed;

            ChaosBorder = new Border();
            ChaosBorder.Width = largeSize;
            ChaosBorder.Height = largeSize;
            ChaosBorder.Background = backgroundColor;
            ChaosBorder.CornerRadius = new CornerRadius(15);

            ChaosShadow = new DropShadowPanel();
            ChaosShadow.Color = Windows.UI.Colors.Black;
            ChaosShadow.BlurRadius = 7;
            ChaosShadow.ShadowOpacity = 1;
            ChaosShadow.OffsetX = 0;
            ChaosShadow.OffsetY = 0;

            ChaosEllipse = new Ellipse();
            ChaosEllipse.Fill = foregroundColor;
            ChaosEllipse.Height = 30;
            ChaosEllipse.Width = 30;
            ChaosEllipse.Stroke = new SolidColorBrush((Color)Application.Current.Resources["SystemAltHighColor"]);
            ChaosEllipse.StrokeThickness = 4;

            TargetEllipse = new Ellipse();
            TargetEllipse.Fill = new SolidColorBrush(Colors.Transparent);
            TargetEllipse.Height = 30;
            TargetEllipse.Width = 30;
            TargetEllipse.Stroke = new SolidColorBrush((Color)Application.Current.Resources["SystemAltHighColor"]);
            TargetEllipse.StrokeThickness = 4;
            TargetEllipse.Opacity = 0;

            InnerNotifier = new TextBlock();
            InnerNotifier.Margin = new Thickness(0, TargetEllipse.Height, 0, 0);
            InnerNotifier.Text = "recording";
            InnerNotifier.Foreground = new SolidColorBrush((Color)Application.Current.Resources["SystemAltHighColor"]);
            InnerNotifier.Style = (Style)Application.Current.Resources["SubtitleTextBlockStyle"];
            InnerNotifier.Opacity = 0;

            ChaosButton = new Button();
            ChaosButton.Width = largeSize;
            ChaosButton.Height = largeSize;
            ChaosButton.Background = new SolidColorBrush(Windows.UI.Colors.Transparent);

            XLabel = new TextBlock();
            Canvas.SetTop(XLabel, ChaosCanvas.Height);
            XLabel.Text = xLabel;
            XLabel.Style = (Style)Application.Current.Resources["SubheaderTextBlockStyle"];

            YLabel = new TextBlock();
            Canvas.SetTop(YLabel, ChaosCanvas.Height);
            YLabel.Text = yLabel;
            YLabel.Style = (Style)Application.Current.Resources["SubheaderTextBlockStyle"];
            Canvas.SetLeft(YLabel, -50);
            YLabel.Rotate(-90, 0, 0, 0).Start();

            ChaosCanvas.Children.Add(ChaosBorder);
            ChaosCanvas.Children.Add(XLabel);
            ChaosCanvas.Children.Add(YLabel);
            ChaosCanvas.Children.Add(TargetEllipse);
            ChaosCanvas.Children.Add(InnerNotifier);
            ChaosCanvas.Children.Add(ChaosShadow);
            ChaosShadow.Content = ChaosEllipse;
            ChaosCanvas.Children.Add(ChaosButton);

            // -------- The Ghosts --------
            GhostCanvas = new Canvas();
            GhostCanvas.Width = ChaosCanvas.Width;
            GhostCanvas.Height = ChaosCanvas.Height;
            GhostCanvas.Background = new SolidColorBrush(Windows.UI.Colors.Transparent);
            GhostCanvas.Visibility = Visibility.Visible;

            GhostShadow = new DropShadowPanel();
            GhostShadow.Color = foregroundColor.Color;
            GhostShadow.BlurRadius = ChaosShadow.BlurRadius;
            GhostShadow.ShadowOpacity = ChaosShadow.ShadowOpacity;
            GhostShadow.OffsetX = ChaosShadow.OffsetX;
            GhostShadow.OffsetY = ChaosShadow.OffsetY;

            GhostEllipse = new Ellipse();
            GhostEllipse.Fill = ChaosEllipse.Fill;
            GhostEllipse.Height = ChaosEllipse.Height;
            GhostEllipse.Width = ChaosEllipse.Width;
            GhostEllipse.Stroke = ChaosEllipse.Stroke;
            GhostEllipse.StrokeThickness = ChaosEllipse.StrokeThickness;
            GhostEllipse.Fade((float)0.6, 0).Start();

            GhostShadow.Content = GhostEllipse;
            GhostCanvas.Children.Add(GhostShadow);

            // -------- Now the Tiny stuff --------
            ChaosButtonsNoLabel = new StackPanel();
            ChaosButtonsNoLabel.Orientation = Orientation.Horizontal;

            ChaosButtons = new StackPanel();
            ChaosButtons.Margin = new Thickness(10);

            ChaosTitle = new TextBlock();
            ChaosTitle.Style = (Style)Application.Current.Resources["SubtitleTextBlockStyle"];
            ChaosTitle.Margin = new Thickness(10, 0, 10, 0);
            ChaosTitle.Text = title;

            TinyCanvas = new Canvas();
            TinyCanvas.Width = tinySize;
            TinyCanvas.Height = tinySize;
            TinyCanvas.Margin = new Thickness(10);

            TinyBorder = new Border();
            TinyBorder.Width = tinySize;
            TinyBorder.Height = tinySize;
            TinyBorder.Background = backgroundColor;
            TinyBorder.CornerRadius = new CornerRadius(10);

            TinyEllipse = new Ellipse();
            TinyEllipse.Fill = foregroundColor;
            TinyEllipse.Height = 15;
            TinyEllipse.Width = 15;
            TinyEllipse.Stroke = new SolidColorBrush((Color)Application.Current.Resources["SystemAltHighColor"]);
            TinyEllipse.StrokeThickness = 2;

            TinyButton = new Button();
            TinyButton.Width = tinySize;
            TinyButton.Height = tinySize;
            TinyButton.Background = new SolidColorBrush(Windows.UI.Colors.Transparent);

            TinyCanvas.Children.Add(TinyBorder);
            TinyCanvas.Children.Add(TinyEllipse);
            TinyCanvas.Children.Add(TinyButton);

            SnapshotButton = new ToggleButton();
            SnapshotButton.Width = tinySize;
            SnapshotButton.Height = tinySize;
            SnapshotButton.Margin = new Thickness(10);
            SnapshotButton.FontFamily = new FontFamily("Segoe MDL2 Assets");
            SnapshotButton.Content = "\uEA3A";
            SnapshotButton.FontWeight = FontWeights.Medium;

            ChaosNotifier = new InAppNotification();
            ChaosNotifier.Background = foregroundColor;
            ChaosNotifier.Foreground = new SolidColorBrush((Color)Application.Current.Resources["SystemAltHighColor"]);
            ChaosNotifier.ShowDismissButton = false;
            ChaosNotifier.HorizontalAlignment = HorizontalAlignment.Left;
            ChaosNotifier.VerticalOffset = 0;
            ChaosNotifier.HorizontalOffset = -200;
            ChaosNotifier.FontFamily = new FontFamily("Segoe MDL2 Assets");
            ChaosNotifier.HorizontalContentAlignment = HorizontalAlignment.Center;
            ChaosNotifier.FontSize = 32;
            ChaosNotifier.Margin = new Thickness(0, 0, 25, 25);

            ChaosButtonsNoLabel.Children.Add(SnapshotButton);
            ChaosButtonsNoLabel.Children.Add(TinyCanvas);
            ChaosButtons.Children.Add(ChaosButtonsNoLabel);
            ChaosButtons.Children.Add(ChaosTitle);
            TinyCanvas.Children.Add(ChaosNotifier);

            // -------- Now the events --------
            TinyButton.Click += TinyButton_Click;

            GazeElement chaosButtonControl = new GazeElement();
            GazeInput.SetGazeElement(ChaosButton, chaosButtonControl);
            chaosButtonControl.DwellProgressFeedback += GazeElement_DwellProgressFeedback;
            chaosButtonControl.StateChanged += ChaosButtonControl_StateChanged;

            GazeElement snapshotButtonControl = new GazeElement();
            GazeInput.SetGazeElement(SnapshotButton, snapshotButtonControl);
            snapshotButtonControl.StateChanged += SnapshotButtonControl_StateChanged;
            SnapshotButton.Click += SnapshotButton_Click;

            // All automation is hardcoded at one second now
            // Faster times make it harder to move off the grid and accidentally trigger the ellipse
            // However, possibly in the future we will want to change this timing and/or sync it to the beat
            SnapshotPlayer = new DispatcherTimer();
            SnapshotPlayer.Tick += MoveEllipse;
            SnapshotPlayer.Interval = new TimeSpan(0, 0, 1);

            // -------- Finally add to our list --------
            AllControls.Add(this);
        }

        // --------------- Input Events ---------------
        private void TinyButton_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveControl == InstanceNumber && ChaosCanvas.Visibility == Visibility.Visible)
            {
                ShowGhosts();
                Deactivate();
            }
            else
            {
                HideGhosts();
                AllControls[ActiveControl].Deactivate();
                Activate();
            }
        }

        // Because we are using StateChanged and gaze position - the chaos controls do not support
        // mouse input yet...
        // There is a giant transparent button overlayed on the canvas
        private void ChaosButtonControl_StateChanged(object sender, StateChangedEventArgs e)
        {
            // Entering should cause a visual effect
            if (e.PointerState == PointerState.Enter)
                ChaosCanvas.Scale((float)CanvasSizeIncrease, (float)CanvasSizeIncrease, (float)(ChaosCanvas.Width / 2), (float)(ChaosCanvas.Height / 2), duration: 300, easingType: EasingType.Back).Start();

            // Dwell is required to start moving the ellipse - this prevents accidental triggering
            if (e.PointerState == PointerState.Dwell)
            {
                BeginRecordingSnapshots();
                GazeOverCanvas = true;
            }

            // Upon exiting
            if (e.PointerState == PointerState.Exit)
            {
                ChaosCanvas.Scale((float)1, (float)1, (float)(ChaosCanvas.Width / 2), (float)(ChaosCanvas.Height / 2), duration: 500, easingType: EasingType.Elastic).Start();
                GazeOverCanvas = false;
                TryAndSaveSnapshots();
                
                if (PlaySnapshots) SnapshotPlayer.Start();
            }
        }

        private void SnapshotButtonControl_StateChanged(object sender, StateChangedEventArgs e)
        {
            if (e.PointerState == PointerState.Dwell)
            {
                // Dwelling gives the previous value of the toggle - not what it is now
                PlaySnapshots = !((sender as ToggleButton).IsChecked.HasValue && (sender as ToggleButton).IsChecked.Value);
                if (PlaySnapshots) SnapshotPlayer.Start();
                else SnapshotPlayer.Stop();
            }
        }

        private void SnapshotButton_Click(object sender, RoutedEventArgs e)
        {
            PlaySnapshots = (sender as ToggleButton).IsChecked.HasValue && (sender as ToggleButton).IsChecked.Value;
            if (PlaySnapshots) SnapshotPlayer.Start();
            else SnapshotPlayer.Stop();
        }

        // --------------- Functionality ---------------
        private void Activate()
        {
            Active = true;
            ActiveControl = InstanceNumber;
            TinyCanvas.Scale((float)1.2, (float)1.2, (float)(TinyCanvas.Width / 2), (float)(TinyCanvas.Height / 2), duration: 500).Start();
            ChaosCanvas.Visibility = Visibility.Visible;

            ControlActivatedOrDeactivated?.Invoke(this, EventArgs.Empty);
        }

        private void Deactivate()
        {
            Active = false;
            TinyCanvas.Scale(1, 1, (float)(TinyCanvas.Width / 2), (float)(TinyCanvas.Height / 2), duration: 500).Start();
            ChaosCanvas.Visibility = Visibility.Collapsed;

            ControlActivatedOrDeactivated?.Invoke(this, EventArgs.Empty);
        }

        // This sets the ellipse position manually without gaze
        // It should be called after the WebView has been loaded to affect the underlying js synth
        public void SetEllipsePosition(double xOffset = 0, double yOffset = 0)
        {
            EllipseX = xOffset;
            EllipseY = yOffset;

            AnimateTinyEllipse(EllipseX, EllipseY).Start();
            AnimateChaosEllipse(EllipseX, EllipseY).Start();
            AnimateGhostEllipse(EllipseX, EllipseY).Start();

            RunJS(EllipseX, EllipseY);
        }

        // This gets called when the user looks over the canvas
        public void MoveEllipse(double rawGazeX, double rawGazeY)
        {
            if (GazeOverCanvas && CanAnimate)
            {
                bool updated = Update_Ellipse_Position(rawGazeX, rawGazeY);
                if (!updated) return;

                AnimateTinyEllipse(EllipseX, EllipseY).Start();
                AnimateGhostEllipse(EllipseX, EllipseY).Start();
                AnimateTargetEllipse(EllipseX, EllipseY).Start();
                if (Temporary_Snapshots.Count == 1) AnimateInnerNotifier(EllipseX, EllipseY).Start();
                var anim = AnimateChaosEllipse(EllipseX, EllipseY);

                anim.Completed += (s, e) => { CanAnimate = true; };
                anim.Start();
                CanAnimate = false;

                RunJS(EllipseX, EllipseY);

                AddSnapshot(EllipseX, EllipseY);
            }
        }

        // This gets called by a dispatcher timer when playing through the list of snapshots
        private void MoveEllipse(object sender, object e)
        {
            if (GazeOverCanvas || (Snapshots.Count <= 1)) return;

            double snapshotX = Snapshots[CurrentSnapshot].Item1;
            double snapshotY = Snapshots[CurrentSnapshot].Item2;

            AnimateTinyEllipse(snapshotX, snapshotY).Start();
            AnimateGhostEllipse(snapshotX, snapshotY).Start();
            AnimateChaosEllipse(snapshotX, snapshotY).Start();

            RunJS(snapshotX, snapshotY);

            CurrentSnapshot = (CurrentSnapshot + 1) % Snapshots.Count();
        }

        private AnimationSet AnimateChaosEllipse(double ellipseX, double ellipseY)
        {
            double width = ChaosCanvas.Width;
            double height = ChaosCanvas.Height;
            double ElRadius = ChaosEllipse.Width / 2;

            // Animations are hardcoded at 1000 ms now - so the durations need to add up to that
            AnimationSet anim = ChaosShadow.Scale((float)1.5, (float)1.5, (float)ElRadius, (float)ElRadius, 100).Then().Scale(1, 1, (float)ElRadius, (float)ElRadius, 100).Then().Offset(offsetX: (float)((ellipseX * width) - ElRadius), offsetY: (float)((ellipseY * height) - ElRadius), duration: 800, easingType: EasingType.Quadratic);

            return anim;
        }

        private AnimationSet AnimateGhostEllipse(double ellipseX, double ellipseY)
        {
            double width = GhostCanvas.Width;
            double height = GhostCanvas.Height;
            double ElRadius = GhostEllipse.Width / 2;

            AnimationSet anim = GhostShadow.Scale((float)1.5, (float)1.5, (float)ElRadius, (float)ElRadius, 100).Then().Scale(1, 1, (float)ElRadius, (float)ElRadius, 100).Then().Offset(offsetX: (float)((ellipseX * width) - ElRadius), offsetY: (float)((ellipseY * height) - ElRadius), duration: 800, easingType: EasingType.Quadratic);

            return anim;
        }

        private AnimationSet AnimateTinyEllipse(double ellipseX, double ellipseY)
        {
            double tinyWidth = TinyCanvas.Width;
            double tinyHeight = TinyCanvas.Height;
            double tinyElRadius = TinyEllipse.Width / 2;

            AnimationSet anim = TinyEllipse.Scale((float)1.5, (float)1.5, (float)tinyElRadius, (float)tinyElRadius, 100).Then().Scale(1, 1, (float)tinyElRadius, (float)tinyElRadius, 100).Then().Offset(offsetX: (float)((ellipseX * tinyWidth - tinyElRadius) * 0.9), offsetY: (float)((ellipseY * tinyHeight - tinyElRadius) * 0.9), duration: 800, easingType: EasingType.Quadratic);

            return anim;
        }

        private AnimationSet AnimateTargetEllipse(double ellipseX, double ellipseY)
        {
            float s = 1.2f;
            float r = (float)(TargetEllipse.Width / 2);
            double t = 200;
            double de = 800;

            float w = (float)ChaosCanvas.Width;
            float h = (float)ChaosCanvas.Height;
            float xPos = (float)(ellipseX * w - r);
            float yPos = (float)(ellipseY * h - r);

            AnimationSet anim = TargetEllipse.Offset(xPos, yPos, 0).Then().Scale(s, s, r, r, t).Fade(1, t).Then().Scale(1, 1, r, r, t, de).Fade(0, t, de);
            return anim;
        }

        private AnimationSet AnimateInnerNotifier(double ellipseX, double ellipseY)
        {
            double t = 200;
            double de = 4000;
            double iH = InnerNotifier.ActualHeight / 2;
            double iW = InnerNotifier.ActualWidth / 2;

            float w = (float)ChaosCanvas.Width;
            float h = (float)ChaosCanvas.Height;
            float xPos = (float)(ellipseX * w - iW);
            float yPos = (float)(ellipseY * h - iH);

            AnimationSet anim = InnerNotifier.Offset(xPos, yPos, 0).Then().Fade(1, t).Then().Fade(0, de, t);
            return anim;
        }

        public void AddLabels(string xLabel, string yLabel)
        {
            XLabel.Text = xLabel;
            YLabel.Text = yLabel;
        }

        public void AddTitle(string newTitle)
        {
            ChaosTitle.Text = newTitle;
        }

        // --------------- Snapshots ---------------
        private void BeginRecordingSnapshots()
        {
            SnapshotButton.Fade((float)0.2).Start();
            SnapshotPlayer.Stop();
        }

        private void AddSnapshot(double ellipseX, double ellipseY)
        {
            Temporary_Snapshots.Add(new Tuple<double, double>(EllipseX, EllipseY));
        }

        private void TryAndSaveSnapshots()
        {
            // Only record after at least two movements
            // This prevents accidental triggering/overwriting
            if (Temporary_Snapshots.Count > 1)
            {
                CurrentSnapshot = 0;
                Snapshots.Clear();
                Temporary_Snapshots.ForEach((item) => { Snapshots.Add(item); });

                SnapshotButton.Fade((float)1, duration: 200).Then().Scale((float)1.2, (float)1.2, (float)(SnapshotButton.Width / 2), (float)(SnapshotButton.Height / 2), 100).Then().Scale(1, 1, (float)(SnapshotButton.Width / 2), (float)(SnapshotButton.Height / 2), 100).Start();
                SnapshotButton.Content = "\uEA3B"; ;

                ChaosNotifier.Show("\uE96A", 2000); // Tape icon
            }
            else SnapshotButton.Fade((float)1).Start();

            Temporary_Snapshots.Clear();
        }

        // --------------- Javascript ---------------
        public void AddJSFunction(WebView NewSynth, string FunctionName)
        {
            Synth = NewSynth;
            JSFunctionName = FunctionName;
        }

        public void RemoveJSFunction()
        {
            Synth = null;
            JSFunctionName = "";
        }

        private async void RunJS(double XVal, double YVal)
        {
            if (JSFunctionName.Length == 0) return;

            // Everything gets cast to a 0--127 int range like regular MIDI
            string xParam = Convert.ToInt32(MapToRange(XVal, 0, 1, 0, 127)).ToString();
            string yParam = Convert.ToInt32(127 - MapToRange(YVal, 0, 1, 0, 127)).ToString();

            string[] args = { xParam, yParam }; // I may want to include ramp times here
            // Debug.WriteLine(JSFunctionName + " : " + xParam + " : " + yParam);
            await Synth.InvokeScriptAsync(JSFunctionName, args);
        }

        // --------------- Helpers ---------------
        private void GazeElement_DwellProgressFeedback(object sender, DwellProgressEventArgs e)
        {
            e.Handled = true;
        }

        private bool Update_Ellipse_Position(double rawGazeX, double rawGazeY)
        {
            double width = ChaosCanvas.Width;
            double height = ChaosCanvas.Height;

            Point absoluteCanvasPos = ChaosCanvas.TransformToVisual(Window.Current.Content).TransformPoint(new Point(0, 0));
            double canvasX = absoluteCanvasPos.X;
            double canvasY = absoluteCanvasPos.Y;

            // Scaling the canvas makes these calculations a little trickier
            double tryX = (rawGazeX - canvasX) / width / CanvasSizeIncrease;
            double tryY = (rawGazeY - canvasY) / height / CanvasSizeIncrease;

            // Right now I am just ignoring values where the user is off the canvas
            if (tryX > 1 || tryX < 0 || tryY > 1 || tryY < 0) return false;

            EllipseX = tryX;
            EllipseY = tryY;

            return true;
        }

        private double MapToRange(double value, double fromLow, double fromHigh, double toLow, double toHigh)
        {
            return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
        }
    }
}
