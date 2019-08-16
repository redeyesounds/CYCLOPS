// Willie Payne
// August 2019
// Script to hold all synthesizer information and functions
// When possible, function names should be lowercase versions of
// their equivalents in MusicTrigger.cs

// Built on Tone.js
// https://tonejs.github.io/

// ----------------- Variables and setup -----------------

// Customizable Synth Settings
let oscillatorTypes = ["sine", "triangle", "square", "sine12", "sawtooth"];
let delayTimes = ["16n", "8n", "8t", "4n", "2n"];
let chorusFrequencies = ["0.5", "1", "1.5", "2", "2.5"];

// We are using a polyphonic synth meaning pitches are allocated to multiple voices
let synth = new Tone.PolySynth(8, Tone.MonoSynth);
synth.set({
    "oscillator": {
        "type": "square"
    },
    "filterEnvelope": {
        "baseFrequency": 2000,
        "octaves": 2
    }
});

// Effects
let chorus = new Tone.Chorus({
    "frequency": 1.5,
    "delayTime": 3,
    "depth": 0.5,
    "type": "sine",
    "Spread": 60
});

//let crush = new Tone.BitCrusher(2);
let crush = new Tone.Distortion(0.95);

let accent = new Tone.Volume(-12);

let lopass = new Tone.Filter({
    "type": "lowpass",
    "frequency": 900,
    "Q": 0.5,
    "rolloff": -12
});

let currentDelayTime = 2;
let delay = new Tone.PingPongDelay({
    "delayTime": delayTimes[currentDelayTime],
    "feedback": 0.2,
    "maxDelay": 10
});

let masterVol = new Tone.PanVol(0, -12);

let compressor = new Tone.Compressor();

// By default, effects are turned off
chorus.wet.value = 0;
crush.wet.value = 0;
delay.wet.value = 0;

// Other global musical parameters
let effectRampTime = 0.2;

// Loop
let pitchPattern = [50, 52, 55, 57, 59, 62, 64, 67, 67, 64, 62, 59, 57, 55, 52, 50];
let rhythmPattern = [1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1];
let holdDurations = ["32n", "16n", "8n", "4n", "2n"];

let noteHeld = false;
let lastNote = 50;
let currentBeat = 0; // Max value is the least common multiple of the rhythm and pitch pattern lengths
let pitchPatternLength = 8;
let rhythmPatternLength = 8;
let currentPitchBeat = 0;
let currentRhythmBeat = 0;
let holdDuration = 1; // How long before triggering release during the loop
let randomMode = false;

// Create the signal chain and start!
synth.chain(crush, chorus, lopass, delay, accent, masterVol, compressor, Tone.Master);
Tone.Transport.bpm.value = 120;
Tone.Transport.start();

// ----------------- Functions -----------------

// Trigger Playback
// attack and release are triggered by user input
// attackRelease is triggered by the loop

function attack(pitch = "60", vel=0.7) {
    lastNote = pitch;
    noteHeld = true;
    synth.triggerAttack(Tone.Midi(pitch).toNote(), Tone.now(), vel); // Tone.
}

function release() {
    //if (noteHeld) synth.triggerRelease(Tone.Midi(lastNote).toNote());
    synth.releaseAll(); // Temporary solution to problem of notes being held
    noteHeld = false;
}

function attackRelease(pitch = "60", duration = "16n", vel = 0.7) {
    // Adding '0.05' is supposed to help performance, but does not meaningfully affect playback
    // https://github.com/Tonejs/Tone.js/wiki/Performance
    synth.triggerAttackRelease(Tone.Midi(pitch).toFrequency(), duration, '+0.05', vel);
}

// Loop
let loop = new Tone.Loop(function (time) {
    // First overwrite the current pitch in the loop if a user is holding down a note
    if (noteHeld) pitchPattern[currentPitchBeat] = lastNote;

    // Then check the current position in the rhythm pattern and play the note with the correct amplitude
    if (rhythmPattern[currentRhythmBeat] == 2) attackRelease(pitchPattern[currentPitchBeat], Tone.Time(holdDurations[holdDuration]), 1);
    if (rhythmPattern[currentRhythmBeat] == 1) attackRelease(pitchPattern[currentPitchBeat], Tone.Time(holdDurations[holdDuration]), 0.4);

    // Notify the C# code of the current beat
    window.external.notify((currentBeat).toString());

    // Increment the beat
    if (randomMode) randomBeat();
    else incrementBeat();

}, "8n") // Technically, the base duration is the eighth note

function startLoop() { loop.start(); }
function stopLoop() { loop.stop(); }

// Update functions for the entire list or just a single value
function setPitchPattern() {
    let numberOfArgs = Math.min(arguments.length, pitchPattern.length);
    for (let i = 0; i < numberOfArgs; i++) pitchPattern[i] = arguments[i];
}
function setNote(noteNum, midiVal) { pitchPattern[noteNum] = midiVal; }

function setRhythmPattern() {
    let numberOfArgs = Math.min(arguments.length, rhythmPattern.length);
    for (let i = 0; i < arguments.length; i++) rhythmPattern[i] = arguments[i];
}
function setRhythm(beatNum, beatVal) { rhythmPattern[beatNum] = parseInt(beatVal); }

function setPitchPatternLength(newLength) { pitchPatternLength = parseInt(newLength); }
function setRhythmPatternLength(newLength) { rhythmPatternLength = parseInt(newLength); }

// Emulates DATO behavior - keep track if a note is held down and if a loop is playing
// update the current state of the loop accordingly
function userHoldingNote(midiVal) {
    lastNote = parseInt(midiVal);
    noteHeld = true;
}
function userStopsHoldingNote() { noteHeld = false; }

function incrementBeat() {
    currentBeat = (currentBeat + 1) % lcm(pitchPatternLength, rhythmPatternLength);
    currentPitchBeat = currentBeat % pitchPatternLength;
    currentRhythmBeat = currentBeat % rhythmPatternLength;
}

function randomBeat() {
    currentBeat = Math.floor(Math.random() * lcm(pitchPatternLength, rhythmPatternLength));
    currentPitchBeat = currentBeat % pitchPatternLength;
    currentRhythmBeat = currentBeat % rhythmPatternLength;
}

function updateTempo(bpm) {
    Tone.Transport.bpm.value = parseInt(bpm);
    changeDelayTime(currentDelayTime);
}

// Synth Characteristics
function changeOscillator(synthVal) {

    synth.set({ "oscillator": { "type": oscillatorTypes[synthVal] } });
}

// Any function with midiVal as an input means a string containing an integer ranging from 0-127
// Typically, that midi value is mapped to a more musical range
function changeRelease(midiVal) {
    let releaseVal = scale(parseInt(midiVal), 0, 127, 0.01, 5); // Figure out how to ramp
    synth.set({ "envelope": { "release": releaseVal } });
}

function changeAttack(midiVal) {
    let holdDuration = Math.floor(scale(parseInt(midiVal), 0, 127, 0, holdDurations.length));
    let attackVal = scale(parseInt(midiVal), 0, 127, 0.01, 1);
    synth.set({ "envelope": { "attack": attackVal } });
}

// Effect Characteristics
function changeChorusDepth(midiVal, rampTime = "1") {
    let chorusDepth = scale(parseInt(midiVal), 0, 127, 0.1, 0.8);
    let chorusWet;
    if (parseInt(midiVal) < 20) chorusWet = 0; // Make it easier to disable entirely
    else chorusWet = scale(parseInt(midiVal), 20, 127, 0, 1);

    chorus.depth.value = chorusDepth;
    chorus.wet.rampTo(chorusWet, parseFloat(rampTime));
}

function changeChorusFrequency(freqVal) {
    chorus.frequency.value = chorusFrequencies[freqVal];
}

function changeFilterFreq(midiVal, rampTime = "1") {
    let filterVal = scale(parseInt(midiVal), 0, 127, 100, 1700);
    lopass.frequency.rampTo(filterVal, parseFloat(rampTime));
}

function changeDelayTime(delayVal) {
    currentDelayTime = parseInt(delayVal);
    let newDelayTime = Tone.Time(delayTimes[currentDelayTime]);
    delay.delayTime.rampTo(newDelayTime, 0.1);
}

function changeDelayFeedback(midiVal, rampTime = "1") {
    let feedbackVal = scale(parseInt(midiVal), 0, 127, 0, 0.7);
    delay.feedback.rampTo(feedbackVal, parseFloat(rampTime));
}

function changeDelayVolume(midiVal, rampTime = "1") {
    let delayVol = scale(parseInt(midiVal), 0, 127, 0, 0.5);
    delay.wet.rampTo(delayVol, parseFloat(rampTime));
}

function changeAccentVolume(midiVal, rampTime = "1") {
    let accentVol = scale(parseInt(midiVal), 0, 127, -12, 0);
    accent.volume.rampTo(accentVol, rampTime);
}

function changeCrush(midiVal, rampTime = "1") {
    let crushVal = scale(parseInt(midiVal), 0, 127, 0, 1);
    crush.wet.rampTo(crushVal, parseFloat(rampTime));
}

// 2d Controls
// These simply call two of the above functions
function changeAttackRelease(xVal, yVal) {
    changeAttack(xVal);
    changeRelease(yVal);
}

function changeDelayFeedbackVolume(xVal, yVal) {
    changeDelayFeedback(xVal);
    changeDelayVolume(yVal);
}

function changeAccentFilter(xVal, yVal) {
    changeFilterFreq(xVal);
    changeAccentVolume(yVal);
}

function changeChorusCrush(xVal, yVal) {
    changeChorusDepth(xVal);
    changeCrush(yVal);
}

// Helper Functions
function scale(num, in_min, in_max, out_min, out_max) {
    let scaledVal = (num - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    if (scaledVal < out_min) scaledVal = out_min;
    else if (scaledVal > out_max) scaledVal = out_max;

    return scaledVal
}

// Lowest Common Multiple
function lcm(x, y) {
    if ((typeof x !== 'number') || (typeof y !== 'number')) return false;
    return (!x || !y) ? 0 : Math.abs((x * y) / gcd(x, y));
}

// Greatest Common Denominator
function gcd(x, y) {
    x = Math.abs(x);
    y = Math.abs(y);
    while (y) {
        let t = y;
        y = x % y;
        x = t;
    }
    return x;
}