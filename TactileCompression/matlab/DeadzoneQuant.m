function [q] = DeadzoneQuant(x,max,bits)
%UNTITLED Summary of this function goes here
%   Detailed explanation goes here

delta = max./(2^(bits));
x(abs(x)>=max) = sign(x(abs(x)>=max)).*max*0.999;
q = sign(x).*floor(abs(x)./delta).*delta;

end

