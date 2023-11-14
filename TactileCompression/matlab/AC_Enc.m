function [out] = AC_Enc(instream,context,counter,counter_total)
range_lower = 0;
range_upper = 1024;
bits_to_follow = 0;
out = [];
for i= 1:length(instream)
    range_diff = range_upper - range_lower;
    new_symbol = instream(i);
    c = context(i);
    p = round(counter(c+1) / counter_total(c+1) * 1024);
    range_add = round((range_diff * p) / 1024);
    if range_add == 0
        range_add = 1;
    elseif range_add == range_diff
        range_add = range_diff - 1;
    end
    if new_symbol == 0
        range_upper = range_lower + range_add;
    else
        range_lower = range_lower + range_add;
    end
    while true
        if range_upper <= 512
            if bits_to_follow > 0
                out = [out,0];
                for j = 1:bits_to_follow
                    out = [out,1];
                end
                bits_to_follow = 0;
            else
                out = [out,0];
            end
        elseif range_lower >= 512
            if bits_to_follow > 0
                out = [out,1];
                for j=1:bits_to_follow
                    out = [out,0];
                end
                bits_to_follow = 0;
            else
                out = [out,1];
            end
            range_lower = range_lower-512;
            range_upper = range_upper-512;
        elseif (range_lower >= 256 && range_upper <= 768)
            bits_to_follow = bits_to_follow+1;
            range_lower = range_lower-256;
            range_upper = range_upper - 256;
        else
            break;
        end
        range_lower = range_lower*2;
        range_upper = range_upper*2;
    end
    if instream(i) == 0
        counter(c+1) = counter(c+1)+1;
    end
     counter_total(c+1) =  counter_total(c+1) + 1;
end
if bits_to_follow > 0
    out = [out,1];
else
    val = 512;
    while range_lower > 0
        if val < range_upper
           out = [out,1];
           range_lower = range_lower - val;
           range_upper = range_upper - val;
        else
            out = [out,0];
        end
        val = val / 2;
    end
end
index_end =length(out);
while out(index_end) == 0 && index_end >= 0
    index_end = index_end - 1;
end
out = out(1:index_end);

% for i = 1:7
%     counter(i) = (counter(i) / counter_total(i)) * 32;
%     if counter(i) == 0
%         counter(i) = 1;
%     end
%     counter_total(i) = 32;
%     if counter(i) == counter_total(i)
%         counter(i) = counter_total(i) - 1;
%     end
% end

end

