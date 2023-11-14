function [binartCodes, extrmumIndex, extrmumValue] = encode(InputWavArray)

InputWavArray = InputWavArray';
fs = 8000;
TotalBits = 32;
signal = rescale(InputWavArray, -1, 1);
n  = 8;     % Order
fc = 72.5;  % Cutoff Frequency
[z,p,k] = butter(n,2*fc/fs);
[b,a] = zp2tf(z,p,k);
signal_lowpass = filtfilt(b,a,signal);
[key_frames,extrmumValue,extrmumIndex] = Keyframe_extraction(signal_lowpass, fs);

% extrmumIndex = trans(1, extrmumIndex');
extrmumIndex = [1 extrmumIndex' length(signal)];
extrmumValue = [signal_lowpass(1) extrmumValue' signal_lowpass(length(signal))];

signal_low = 1:1:length(signal);
signal_low = interp1(extrmumIndex',extrmumValue',signal_low','linear');
signal_cancha = signal_lowpass - signal_low;
signal_highpass = signal - signal_lowpass;
signal_high = signal_highpass + signal_cancha;
[binartCodes] = Wavelet_processing_unity(signal_high,TotalBits,fs);
% binartCodes = [];
% key_frames = [];

end
