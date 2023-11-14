function [m,n_real,level,wavmax] = SPIHT_1D_AC_Dec(stream,origlength,counter,counter_total)
    % arithDec.initDecoding
    in_index = 1;
    max_index = length(stream);
    in_leading = 0;
    shift = 9;
    for i = 1:10
         if i <= length(stream)
             in_leading = in_leading + stream(in_index) * (2 ^ shift);
             in_index = in_index + 1;
             shift = shift - 1;
         else
             break;
         end
    end
    range_diff = 1024;
    range_lower = 0;
    range_upper = 1024;
    
    t = struct('in_index', in_index, 'max_index', max_index,...
        'in_leading',in_leading, 'range_diff', range_diff, 'range_lower', range_lower,...
        'range_upper', range_upper, 'stream', stream);

    
    
    
    [maxallocBitsArray,counter,counter_total,t] = getBits(4,0,counter,counter_total,t);
    n_real = bi2de(maxallocBitsArray);
    [mode,counter,counter_total,t] = getBit(0,counter,counter_total,t);
    [wavMaxArray,counter,counter_total,t] = getBits(7,0,counter,counter_total,t);
    temp = bi2de(wavMaxArray);
    if mode == 0
        wavmax = temp * 2^(-7);
    else
        wavmax = temp * 2^(-4) + 1;
    end

if origlength==32
    level = 4;
else
    level = log2(origlength)-2;
end
m = zeros(1,origlength);


%-----------   Initialize LIP, LSP, LIS   ----------------
temp = [];
bandsize = 2^(log2(origlength) - level + 1);
temp = 1 : bandsize;
LIP(:, 1) = ones(bandsize,1);
LIP(:, 2) = temp';

LIS(:, 1) = LIP(bandsize/2+1:end, 1);
LIS(:, 2) = LIP(bandsize/2+1:end, 2);
LIS(:, 3) = zeros(length(LIP(bandsize/2+1:end, 1)), 1);
LSP = [];

%-----------   coding   ----------------
n = n_real-1;

while 0<=n
    compare = 1 * (2^n);
    LSP_idx = size(LSP,1);
    LIPtemp = LIP; temp = 0;
    for i = 1:size(LIPtemp,1)
        temp = temp+1;
        [tttt,counter,counter_total,t] = getBit(2,counter,counter_total,t);
        if tttt == 1
            [rrrr,counter,counter_total,t] = getBit(1,counter,counter_total,t);
            if rrrr == 1
                m(LIPtemp(i,1),LIPtemp(i,2)) = compare;
            else
                m(LIPtemp(i,1),LIPtemp(i,2)) = -2^n;
            end
            LSP = [LSP; LIPtemp(i,:)];
            LIP(temp,:) = []; temp = temp - 1;
        end
    end
    LIStemp = LIS; temp = 0; i = 1;
    while ( i <= size(LIStemp,1))
        temp = temp + 1;
        if LIStemp(i,3) == 0
            [tttt,counter,counter_total,t] = getBit(3,counter,counter_total,t);
            if tttt == 1
                x = LIStemp(i,1); y = LIStemp(i,2);
                [rrrr,counter,counter_total,t] = getBit(4,counter,counter_total,t);
                if rrrr == 1
                    LSP = [LSP; x 2*y-1];
                    [qqqq,counter,counter_total,t] = getBit(1,counter,counter_total,t);
                    if qqqq == 1
                        m(x,2*y-1) = 2^n;
                    else
                        m(x,2*y-1) = -2^n;
                    end
                else
                    LIP = [LIP; x 2*y-1];
                end
                [tttt,counter,counter_total,t] = getBit(4,counter,counter_total,t);
                if tttt == 1
                    LSP = [LSP; x 2*y];
                    [pppp,counter,counter_total,t] = getBit(1,counter,counter_total,t);
                    if pppp == 1
                        m(x,2*y) = compare;
                    else
                        m(x,2*y) = -compare;
                    end
                else
                    LIP = [LIP; x 2*y];
                end
                if ((2*(2*y)-1) < size(m,2))
                    LIS = [LIS; LIStemp(i,1) LIStemp(i,2) 1];
                    LIStemp = [LIStemp; LIStemp(i,1) LIStemp(i,2) 1];
                end    
                LIS(temp,:) = []; temp = temp-1;
            end
        else
            [rrrr,counter,counter_total,t] = getBit(5,counter,counter_total,t);
            if rrrr == 1
                x = LIStemp(i,1); y = LIStemp(i,2);
                LIS = [LIS; x 2*y-1 0; x 2*y 0];
                LIStemp = [LIStemp; x 2*y-1 0; x 2*y 0];
                LIS(temp,:) = []; temp = temp - 1;
            end
        end
        i = i + 1;          
    end
    temp = 1;
    while (temp<=LSP_idx)
        [rrrr,counter,counter_total,t] = getBit(6,counter,counter_total,t);
        m(LSP(temp,1),LSP(temp,2)) = m(LSP(temp,1),LSP(temp,2)) +sign(m(LSP(temp,1),LSP(temp,2)))*(compare)*rrrr;
        temp = temp + 1;
    end
    n = n - 1;
end
  
    
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

