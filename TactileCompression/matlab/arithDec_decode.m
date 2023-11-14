function [s,counter,counter_total,t] = arithDec_decode(context,counter,counter_total,t)
range_diff = t.range_diff;
range_lower = t.range_lower;
range_upper = t.range_upper;
in_index = t.in_index;
max_index = t.max_index;
in_leading = t.in_leading;
instream = t.stream;

p = round(counter(context+1) / counter_total(context+1) * 1024);
compare = round((range_diff * p)/1024);
if compare == 0
    compare = 1;
elseif compare == range_diff
    compare = range_diff - 1;
end
value = in_leading - range_lower;
s = 0;
if value < compare
    range_upper = range_lower + compare;
else
     s = 1;
     range_lower = range_lower + compare;
end
while true
   if range_upper <= 512
       range_lower = range_lower * 2;
       range_upper = range_upper * 2;
       if in_index < max_index
           in_leading = (in_leading * 2) + instream(in_index);
           in_index = in_index + 1;
       else
           in_leading = in_leading * 2;
       end
   elseif range_lower >= 512
       range_lower = (range_lower - 512) * 2;
       range_upper = (range_upper - 512) * 2;
       if in_index < max_index
           in_leading = ((in_leading - 512) * 2) + instream(in_index);
           in_index = in_index + 1;
       else
           in_leading = (in_leading - 512) * 2;
       end
   elseif range_lower >= 256 && range_upper <= 768
       range_lower = (range_lower - 256) * 2;
       range_upper = (range_upper - 256) * 2;
       if in_index < max_index
           in_leading = ((in_leading - 256) * 2) + instream(in_index);
           in_index = in_index + 1;
       else
           in_leading = (in_leading - 256) * 2;
       end
   else
       break;
   end
end

range_diff = range_upper - range_lower;
   
if s == 0 
    counter(context+1) = counter(context+1) + 1;
end
counter_total(context+1) = counter_total(context+1) + 1;


t.range_diff = range_diff;
t.range_lower = range_lower;
t.range_upper = range_upper;
t.in_index = in_index;
t.in_leading = in_leading;

end

