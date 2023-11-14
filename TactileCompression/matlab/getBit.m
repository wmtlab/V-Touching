function [out,counter,counter_total,t] = getBit(context,counter,counter_total,t)
    [s,counter,counter_total,t] = arithDec_decode(context,counter,counter_total,t);
    out = s;
%     t.stream = t.stream(2:end);
end

