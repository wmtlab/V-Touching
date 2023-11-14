function [q] = MaxQuant(x,b1,b2)
%UNTITLED2 Summary of this function goes here
%   Detailed explanation goes here

max = (2^(b1+b2)-1)/2^b2;
x(abs(x)>=max) = sign(x(abs(x)>=max)).*max*0.999;   %?
delta = 2^(-b2);                                    %量化间隔
q = ceil(abs(x)./delta).*delta;

end

