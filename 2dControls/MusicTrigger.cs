using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace _2dControls
{
    public class MusicTrigger
    {
        // Global Music Parameters
        private bool pitchHeld = false;
        private bool sequencerOn = false;
        private int mostRecentPitchNum;
        private bool isWebSynthLoaded = false;

        // Because the rhythmic pattern can have a different length than the pitch pattern,
        // We need to have a global beat and then local beats that derive from it
        public int CurrentBeat = 0;
        public int CurrentPitchBeat = 0;
        public int CurrentRhythmBeat = 0;

        public int PitchPatternLength = 8;
        public int RhythmPatternLength = 8;
        public int[] PitchNumSequence = new int[16];
        public int[] RhythmSequence = new int[16];

        private readonly Random rand = new Random();

        private List<int> globalScale = new List<int>() { 0, 2, 4, 5, 7, 9, 11 }; // Major scale
        private int root = 48;
        private int key = 0;

        // The Javascript Websynth that all of these functions call
        private WebView WebSynth;

        // Event Handlers mainly used for making the UI respond to music events
        // Rather than create custom event args, I generally pass in 'this' and empty args
        // allowing the function to be called with whatever param it needs.
        public event EventHandler OnStartLoop = delegate { };
        public event EventHandler OnStopLoop = delegate { };

        public event EventHandler OnAttackSequencerOn = delegate { };
        public event EventHandler OnAttackSequencerOff = delegate { };
        public event EventHandler OnAttack = delegate { };

        public event EventHandler OnRelease = delegate { };

        public event EventHandler OnGenerateRandomSequence = delegate { };

        public event EventHandler OnPitchPatternLengthChange = delegate { };
        public event EventHandler OnRhythmPatternLengthChange = delegate { };

        public event EventHandler OnBeatChange = delegate { };
        public event EventHandler OnScriptNotify = delegate { };

        public MusicTrigger(WebView webSynth)
        {
            WebSynth = webSynth;
            WebSynth.ScriptNotify += WebSynth_ScriptNotify;
            for (int i = 0; i < PitchNumSequence.Length; i++) PitchNumSequence[i] = 0;
            for (int i = 0; i < RhythmSequence.Length; i++) RhythmSequence[i] = 1;
        }

        public void TurnOnJS() { isWebSynthLoaded = true; }

        public void UpdateMusicParams(int newRoot, int newKey, List<int> newScale)
        {
            root = newRoot;
            key = newKey;
            globalScale = newScale;

            SetPitchPattern();
        }

        public void SetOctave(int newRoot)
        {
            // I am using root and octave interchangeably here
            root = newRoot;
            SetPitchPattern();
        }

        // --------------- Sequencer Functions ---------------

        // In most cases, these functions call corresponding js functions
        // with the proper arguments. They should have the same name but begin with
        // a capital letter.
        public async void StartLoop()
        {
            if (!isWebSynthLoaded) return;

            sequencerOn = true;
            OnStartLoop(this, EventArgs.Empty);

            string[] args = { };
            await WebSynth.InvokeScriptAsync("startLoop", args);
        }

        public async void StopLoop()
        {
            if (!isWebSynthLoaded) return;

            sequencerOn = false;
            OnStopLoop(this, EventArgs.Empty);

            string[] args = { };
            await WebSynth.InvokeScriptAsync("stopLoop", args);
        }

        public async void UpdateTempo(int newTempoVal)
        {
            if (!isWebSynthLoaded) return;

            string[] args = { newTempoVal.ToString() };
            await WebSynth.InvokeScriptAsync("updateTempo", args);
        }

        private void WebSynth_ScriptNotify(object sender, NotifyEventArgs e)
        {
            SetBeat(Convert.ToInt32(e.Value));
            SetNoteInCSharp(); // This is used in case a note is held down to correctly update the pitch array

            OnScriptNotify(this, EventArgs.Empty);
        }

        private async void SetPitchPattern()
        {
            if (!isWebSynthLoaded) return;

            string[] args = new string[PitchNumSequence.Length];
            for (int i = 0; i < PitchNumSequence.Length; i++)
                args[i] = GetPitch(PitchNumSequence[i]).ToString();

            await WebSynth.InvokeScriptAsync("setPitchPattern", args);
        }

        private async void SetRhythmPattern()
        {
            if (!isWebSynthLoaded) return;

            string[] args = new string[RhythmSequence.Length];
            for (int i = 0; i < RhythmSequence.Length; i++)
                args[i] = RhythmSequence[i].ToString();

            await WebSynth.InvokeScriptAsync("setRhythmPattern", args);
        }

        public async void SetRhythm(int beatPosition, int beatValue)
        {
            if (!isWebSynthLoaded) return;

            RhythmSequence[beatPosition] = beatValue;

            string[] jsArgs = { beatPosition.ToString(), beatValue.ToString() };
            await WebSynth.InvokeScriptAsync("setRhythm", jsArgs);
        }

        public async void SetPitchPatternLength(int newLength)
        {
            if (!isWebSynthLoaded) return;

            PitchPatternLength = newLength;
            OnPitchPatternLengthChange(this, EventArgs.Empty);
            await WebSynth.InvokeScriptAsync("setPitchPatternLength", new String[] { newLength.ToString() } );
        }

        public async void SetRhythmPatternLength(int newLength)
        {
            RhythmPatternLength = newLength;
            OnRhythmPatternLengthChange(this, EventArgs.Empty);
            await WebSynth.InvokeScriptAsync("setRhythmPatternLength", new String[] { newLength.ToString() });
        }

        public void GenerateRandomSequence()
        {
            if (!isWebSynthLoaded) return;

            for (int i = 0; i < PitchNumSequence.Count(); i++) PitchNumSequence[i] = rand.Next(7);

            OnGenerateRandomSequence(this, EventArgs.Empty);
            SetPitchPattern();
        }

        private void IncrementBeat()
        {
            CurrentBeat = (CurrentBeat + 1) % LCM(PitchPatternLength, RhythmPatternLength);
            CurrentPitchBeat = CurrentBeat % PitchPatternLength;
            CurrentRhythmBeat = CurrentBeat % RhythmPatternLength;

            OnBeatChange(this, EventArgs.Empty);
        }

        private void SetBeat(int beatNum)
        {
            CurrentBeat = beatNum;
            CurrentPitchBeat = CurrentBeat % PitchPatternLength;
            CurrentRhythmBeat = CurrentBeat % RhythmPatternLength;

            OnBeatChange(this, EventArgs.Empty);
        }

        // --------------- Note Input Functions ---------------
        public async void Attack(int pitchNum)
        {
            if (!isWebSynthLoaded) return;

            mostRecentPitchNum = pitchNum;
            pitchHeld = true;
            int pitch = GetPitch(pitchNum);
            string[] args = { pitch.ToString() };

            OnAttack(this, EventArgs.Empty);

            if (sequencerOn)
            {
                OnAttackSequencerOn(this, EventArgs.Empty);
                await this.WebSynth.InvokeScriptAsync("userHoldingNote", args);
            }
                
            else
            {
                OnAttackSequencerOff(this, EventArgs.Empty);
                SetNoteInJS(pitchNum);
                await this.WebSynth.InvokeScriptAsync("attack", args);
            }
        }

        public async void Release()
        {
            if (!isWebSynthLoaded) return;

            pitchHeld = false;
            string[] args = { };

            OnRelease(this, EventArgs.Empty);

            if (sequencerOn) await this.WebSynth.InvokeScriptAsync("userStopsHoldingNote", args);
            else await this.WebSynth.InvokeScriptAsync("release", args);
        }

        // This gets called when the user plays a note - it updates the C# and javascript pitch lists and beats
        // It should NOT get called when the sequencer is on
        private async void SetNoteInJS(int pitchNum)
        {
            if (!isWebSynthLoaded) return;

            PitchNumSequence[CurrentPitchBeat] = pitchNum;
            string[] jsArgs = { CurrentPitchBeat.ToString(), GetPitch(PitchNumSequence[CurrentPitchBeat]).ToString() };
            await WebSynth.InvokeScriptAsync("setNote", jsArgs);

            IncrementBeat();
            await WebSynth.InvokeScriptAsync("incrementBeat", new String[0]);
        }

        // This gets called by the javascript timer during the loop if a pitch is currently held down
        // It should ONLY get called when the sequencer is on
        private void SetNoteInCSharp()
        {
            if (pitchHeld) PitchNumSequence[CurrentPitchBeat] = mostRecentPitchNum;
        }


        // --------------- Synthesizer Param Functions ---------------
        public async void ChangeDelayTime(int selectorVal)
        {
            if (!isWebSynthLoaded) return;

            string[] args = new string[1];
            args[0] = selectorVal.ToString();

            await WebSynth.InvokeScriptAsync("changeDelayTime", args);
        }

        public async void ChangeOscillator(int selectorVal)
        {
            if (!isWebSynthLoaded) return;

            string[] args = new string[1];
            args[0] = selectorVal.ToString();

            await WebSynth.InvokeScriptAsync("changeOscillator", args);
        }

        public async void ChangeChorusFrequency(int selectorVal)
        {
            if (!isWebSynthLoaded) return;

            string[] args = new string[1];
            args[0] = selectorVal.ToString();

            await WebSynth.InvokeScriptAsync("changeChorusFrequency", args);
        }

        // --------------- Helper Functions ---------------
        private int GetPitch(int pitchNum)
        {
            int scaleNum = pitchNum % globalScale.Count;
            int octaves = pitchNum / globalScale.Count;

            return globalScale[scaleNum] + root + key + (12 * octaves);
        }

        private int GCF(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        private int LCM(int a, int b)
        {
            return (a / GCF(a, b)) * b;
        }

    }
}
