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

// BUG - Setting state in XAML will not animated the ActiveBorder to the proper position

namespace _2dControls
{
    public class SelectorChangedEventArgs: EventArgs
    {
        private readonly int _selectedID;
        private readonly string _selectedName;

        public SelectorChangedEventArgs(int selectedID, string selectedName)
        {
            _selectedID = selectedID;
            _selectedName = selectedName;
        }

        public int SelectedID { get { return _selectedID; } }
        public string SelectedName { get { return _selectedName; } }
    }

    public sealed partial class FiveSelector : UserControl
    {
        public event EventHandler<SelectorChangedEventArgs> SelectorChanged;

        private int _state = 2;
        public int State
        {
            get { return _state; }
            set
            {
                _state = Math.Max(Math.Min(value, 4), 0);
                SelectorChanged?.Invoke(this, new SelectorChangedEventArgs(State, GetLabel(State)));
                Animate(_state - 2);
            }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                title.Text = $"{_title}: {GetLabel(State)}";
            }
        }

        public String Label0
        {
            get { return l0.Text; }
            set { l0.Text = value; }
        }
        public String Label1
        {
            get { return l1.Text; }
            set { l1.Text = value; }
        }
        public String Label2
        {
            get { return l2.Text; }
            set { l2.Text = value; }
        }
        public String Label3
        {
            get { return l3.Text; }
            set { l3.Text = value; }
        }
        public String Label4
        {
            get { return l4.Text; }
            set { l4.Text = value; }
        }

        public static readonly DependencyProperty stateProp = DependencyProperty.Register("State", typeof(string), typeof(FiveSelector), null);

        public static readonly DependencyProperty titleProp = DependencyProperty.Register("Title", typeof(string), typeof(FiveSelector), null);

        public static readonly DependencyProperty l0Prop = DependencyProperty.Register("Label0", typeof(string), typeof(FiveSelector), null);
        public static readonly DependencyProperty l1Prop = DependencyProperty.Register("Label1", typeof(string), typeof(FiveSelector), null);
        public static readonly DependencyProperty l2Prop = DependencyProperty.Register("Label2", typeof(string), typeof(FiveSelector), null);
        public static readonly DependencyProperty l3Prop = DependencyProperty.Register("Label3", typeof(string), typeof(FiveSelector), null);
        public static readonly DependencyProperty l4Prop = DependencyProperty.Register("Label4", typeof(string), typeof(FiveSelector), null);


        public FiveSelector()
        {
            this.InitializeComponent();
            this.Loaded += FiveSelector_Loaded;

            GetGazeButtonControl(sel0).DwellProgressFeedback += RemoveDwellProgressFeedback;
            GetGazeButtonControl(sel1).DwellProgressFeedback += RemoveDwellProgressFeedback;
            GetGazeButtonControl(sel2).DwellProgressFeedback += RemoveDwellProgressFeedback;
            GetGazeButtonControl(sel3).DwellProgressFeedback += RemoveDwellProgressFeedback;
            GetGazeButtonControl(sel4).DwellProgressFeedback += RemoveDwellProgressFeedback;

            GetGazeButtonControl(sel0).StateChanged += GazeElement_StateChanged;
            GetGazeButtonControl(sel1).StateChanged += GazeElement_StateChanged;
            GetGazeButtonControl(sel2).StateChanged += GazeElement_StateChanged;
            GetGazeButtonControl(sel3).StateChanged += GazeElement_StateChanged;
            GetGazeButtonControl(sel4).StateChanged += GazeElement_StateChanged;
        }

        private void FiveSelector_Loaded(object sender, RoutedEventArgs e)
        {
            State = State; // This will not trigger the Offset function!
            Title = _title; // This updates the title with the currently selected label

        }

        private void Sel_Click(object sender, RoutedEventArgs e)
        {
            State = GetButtonNumber(sender as Button);
            Title = _title;
            AnimateActivate(sender as Button);
        }


        private void Animate(int position)
        {
            float pos = (float)(ActiveBorder.ActualWidth * position);
            ActiveBorder.Offset(pos, 0, 400, 0, EasingType.Default).Start();
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
                btn.Scale(enterSize, enterSize, wd, ht, 100, 0, EasingType.Default).Start();
            }


            if (e.PointerState == PointerState.Exit)
            {
                btn.Scale(1, 1, wd, ht, 500, 0, EasingType.Default).Start();
            }
        }

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

        private int GetButtonNumber(Button senderButton)
        {
            return Convert.ToInt32(senderButton.Name.ToString().Substring(3));
        }

        private string GetLabel(int selected)
        {
            if (selected == 0) return Label0;
            if (selected == 1) return Label1;
            if (selected == 2) return Label2;
            if (selected == 3) return Label3;
            if (selected == 4) return Label4;
            else return ("");
        }
    }
}
