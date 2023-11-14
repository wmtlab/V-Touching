function [out,counter,counter_total,t] = getBits(length,context,counter,counter_total,t)
    out = [];
    for i = 1:length
       [s,counter,counter_total,t] = arithDec_decode(context,counter,counter_total,t);
       out = [out,s];
    end
end

