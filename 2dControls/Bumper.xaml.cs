using Microsoft.Toolkit.Uwp.UI.Animations;
using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarSymbols;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace _2dControls
{
    public sealed partial class Bumper : UserControl
    {
        private List<Border> Bumpers;
        private List<Border> AllBumpers;
        private int[] BumperPitches; // Could I pass this in?

        private int Bumped; // Bumping is liking a piano key being pressed
        private int Activated;

        private bool bumpersLoaded = false;
        private int maxPitch = 14; // The highest pitch is how tall each can be

        private static readonly int ActivateDuration = 50;
        private static readonly int DeactivateDuration = 400; // Should be ok except for really fast random

        // Extra bumpers
        Border b8;
        Border b9;
        Border b10;
        Border b11;
        Border b12;
        Border b13;
        Border b14;
        Border b15;

        public Bumper()
        {
            b8 = MakeBumper(8);
            b9 = MakeBumper(9);
            b10 = MakeBumper(10);
            b11 = MakeBumper(11);
            b12 = MakeBumper(12);
            b13 = MakeBumper(13);
            b14 = MakeBumper(14);
            b15 = MakeBumper(15);

            this.Loaded += Bumper_Loaded;
            this.InitializeComponent();
        }

        private void Bumper_Loaded(object sender, RoutedEventArgs e)
        {
            bumpersLoaded = true;
            Bumpers = new List<Border> { b0, b1, b2, b3, b4, b5, b6, b7 };
            AllBumpers = new List<Border> { b0, b1, b2, b3, b4, b5, b6, b7, b8, b9, b10, b11, b12, b13, b14, b15 };
            Bumped = -1;
        }

        public void SetPitches(int[] pitchList) { BumperPitches = pitchList; }

        public void ResetPositions()
        {
            if (bumpersLoaded)
                for (int i = 0; i < Bumpers.Count; i++) BumpUp(i);
        }

        // Push to the bottom of the screen
        public void BumpDown(int bumperNum)
        {
            if (!bumpersLoaded) return;
            float ht = (float)AllBumpers[bumperNum].ActualHeight;

            AllBumpers[bumperNum].Scale(scaleY: (float)0.1, duration: ActivateDuration, centerY: ht).Start();
            Bumped = bumperNum;
        }

        // Push back to its height
        public void BumpUp(int bumperNum)
        {
            if (BumperPitches == null) return;
            float ht = (float)AllBumpers[bumperNum].ActualHeight;
            
            // Add a check here to ensure that BumperPitches has been set
            float scaledHeight = ((float)BumperPitches[bumperNum] / (float)maxPitch) * (float)3.0 + (float)1.0;
            AllBumpers[bumperNum].Scale(scaleY: scaledHeight, duration: DeactivateDuration, centerY: ht).Start();
        }

        // bump up function with no arguments
        public void BumpUp()
        {
            if (Bumped > -1) BumpUp(Bumped);
            Bumped = -1;
        }

        // Fade in
        public void SetActive(int bumperNum)
        {
            if (Activated > -1 && Activated != bumperNum) SetInactive(Activated);

            AllBumpers[bumperNum].Fade(value: 1, duration: ActivateDuration).Start();
            Activated = bumperNum;
        }

        // Fade out
        public void SetInactive(int bumperNum)
        {
            AllBumpers[bumperNum].Fade(value: (float)0.5, duration: ActivateDuration).Start();
        }

        // This should get called by the sequencer and handles all behavior 
        // The parts get called manually when the user is simply inputting notes
        public void Bump(int bumperNum)
        {
            if (!bumpersLoaded) return;

            BumpUp();
            SetActive(bumperNum);
            BumpDown(bumperNum);
        }

        private Border MakeBumper(int gridNumber)
        {
            Border bN = new Border();
            bN.Background = (SolidColorBrush)Resources["SystemControlDisabledChromeDisabledLowBrush"];
            bN.Opacity = 0.5;
            bN.VerticalAlignment = VerticalAlignment.Stretch;
            bN.HorizontalAlignment = HorizontalAlignment.Stretch;

            Grid.SetRow(bN, 3);
            Grid.SetColumn(bN, gridNumber);

            return bN;
        }

        public void AddBumper()
        {
            int currentCount = Bumpers.Count;
            if (currentCount > 16) return;

            ColumnDefinition col = new ColumnDefinition();

            Bumpers.Add(AllBumpers[currentCount]);
            grid.ColumnDefinitions.Add(col);
            grid.Children.Add(AllBumpers[currentCount]);
        }

        public void RemoveBumper()
        {
            int currentCount = Bumpers.Count;
            if (currentCount < 1) return;

            grid.Children.Remove(Bumpers[currentCount - 1]);
            grid.ColumnDefinitions.RemoveAt(Bumpers.Count - 1);
            Bumpers.RemoveAt(currentCount - 1);
            
            return;
        }

        public void SetNumberOfBumpers(int newNumber)
        {
            if (newNumber == Bumpers.Count) return;

            if (newNumber > Bumpers.Count)
                while (Bumpers.Count < newNumber) AddBumper();
            else
                while (Bumpers.Count > newNumber) RemoveBumper();
        }

    }
}
