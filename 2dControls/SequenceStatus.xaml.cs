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
    public sealed partial class SequenceStatus : UserControl
    {
        public SolidColorBrush ActiveGridColor;
        public SolidColorBrush InactiveGridColor;

        private SolidColorBrush _currentBeatColor;
        public SolidColorBrush CurrentBeatColor
        {
            get { return _currentBeatColor;  }
            set
            {
                _currentBeatColor = value;
                ibt0.Background = value;
                ibt1.Background = value;
                ibt2.Background = value;
                ibt3.Background = value;
                ibt4.Background = value;
                ibt5.Background = value;
                ibt6.Background = value;
                ibt7.Background = value;
                ibt8.Background = value;
                ibt9.Background = value;
                ibt10.Background = value;
                ibt11.Background = value;
                ibt12.Background = value;
                ibt13.Background = value;
                ibt14.Background = value;
                ibt15.Background = value;
            }
        }
        public static readonly DependencyProperty CurrentBeatColorProp = DependencyProperty.Register("CurrentBeatColor", typeof(SolidColorBrush), typeof(SequenceStatus), null);

        private readonly Border[] GridArray;
        private readonly Border[] InnerGridArray;
        private int CurrentlyActivatedBeat = 0;

        private int ActiveElements = 8;

        public SequenceStatus()
        {
            this.InitializeComponent();

            ActiveGridColor = (SolidColorBrush)bt1.Background;
            InactiveGridColor = (SolidColorBrush)bt15.Background;

            GridArray = new Border[] { bt0, bt1, bt2, bt3, bt4, bt5, bt6, bt7, bt8, bt9, bt10, bt11, bt12, bt13, bt14, bt15 };
            InnerGridArray = new Border[] { ibt0, ibt1, ibt2, ibt3, ibt4, ibt5, ibt6, ibt7, ibt8, ibt9, ibt10, ibt11, ibt12, ibt13, ibt14, ibt15 };
        }

        public void SetSequenceLength(int sequenceLength)
        {
            ActiveElements = sequenceLength;
            for (int i = 0; i < GridArray.Length; i++)
            {
                if (i < ActiveElements) GridArray[i].Background = ActiveGridColor;
                else GridArray[i].Background = InactiveGridColor;
            }
        }

        public void Flash(int currentBeat)
        {
            if (CurrentlyActivatedBeat != currentBeat) DeactivatePrevious();
            InnerGridArray[currentBeat].Fade(1, 40).Start();
            CurrentlyActivatedBeat = currentBeat;
        }

        private void DeactivatePrevious()
        {
            InnerGridArray[CurrentlyActivatedBeat].Fade(0, 400).Start();
        }

        public void SetColorScheme(SolidColorBrush currentBeatColor, SolidColorBrush activeGridColor, SolidColorBrush inactiveGridColor)
        {
            CurrentBeatColor = currentBeatColor;
            ActiveGridColor = activeGridColor;
            InactiveGridColor = inactiveGridColor;

            GridArray[CurrentlyActivatedBeat].Background = CurrentBeatColor;
            SetSequenceLength(ActiveElements);
            for (int i = 0; i < InnerGridArray.Length; i++)
                InnerGridArray[i].Background = currentBeatColor;
        }
    }
}
