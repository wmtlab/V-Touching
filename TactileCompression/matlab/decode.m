function [signal] = decode(binartCodes, extrmumIndex, extrmumValue)
signal = Decoder(binartCodes);
signal_low = 1:1:length(signal);
signal_low = interp1(extrmumIndex,extrmumValue,signal_low','linear');
signal = signal + signal_low';
end
