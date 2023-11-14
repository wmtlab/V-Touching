function [m,n_real,level,wavmax] = SPIHT_1D_Dec(in,origlength)
% Matlab implementation of SPIHT (without Arithmatic coding stage)
%
% Decoder
%
% input:    in : bit stream
%
% output:   m : reconstructed image in wavelet domain
%

%-----------   Initialization  -----------------
% image size, number of bit plane, wavelet decomposition level should be
% written as bit stream header.

n_max = bi2de(in(1,[1 2 3 4]))-1;
n_real = n_max +1;
% level = bi2de(in(1,[ 7 8 9]));
wavmaxFlag = in(1,[5]);
% wavmax = bi2de(in(1,[13 14 15 16 17 18 19]))*2^(-4);
if wavmaxFlag == 1
    wavmax = bi2de(in(1,[6 7 8 9 10 11 12]))*2^(-4)+1;
else
    wavmax = bi2de(in(1,[6 7 8 9 10 11 12]))*2^(-7);
end
ctr = 13;
if origlength==32
    level = 4;
else
    level = log2(origlength)-2;
end
m = zeros(1,origlength);


%-----------   Initialize LIP, LSP, LIS   ----------------
temp = [];
bandsize = 2.^(log2(origlength) - level + 1);
temp = 1 : bandsize;
LIP(:, 1) = ones(bandsize,1);
LIP(:, 2) = temp';

LIS(:, 1) = LIP(bandsize/2+1:end, 1);
LIS(:, 2) = LIP(bandsize/2+1:end, 2);
LIS(:, 3) = zeros(length(LIP(bandsize/2+1:end, 1)), 1);
LSP = [];

%-----------   coding   ----------------
n = n_max;
while (ctr <= size(in,2))
    
    LSP_idx = size(LSP,1); % to be used in refinement pas
    
    %Sorting Pass
    LIPtemp = LIP; temp = 0;
    for i = 1:size(LIPtemp,1)
        temp = temp+1;
        if ctr > size(in,2)
            return
        end
        if in(1,ctr) == 1
            ctr = ctr + 1;
            if in(1,ctr) > 0
                m(LIPtemp(i,1),LIPtemp(i,2)) = 2^n;
            else
                m(LIPtemp(i,1),LIPtemp(i,2)) = -2^n;
            end
            LSP = [LSP; LIPtemp(i,:)];
            LIP(temp,:) = []; temp = temp - 1;
        end
        ctr = ctr + 1;
    end
    
    LIStemp = LIS; temp = 0; i = 1;
    while ( i <= size(LIStemp,1))
        temp = temp + 1;
        if ctr > size(in,2)
            return
        end
        if LIStemp(i,3) == 0
            if in(1,ctr) == 1
                ctr = ctr + 1;
                x = LIStemp(i,1); y = LIStemp(i,2);
                
                if ctr > size(in,2)
                    return
                end
                if in(1,ctr) == 1
                    LSP = [LSP; x 2*y-1];
                    ctr = ctr + 1;
                    if in(1,ctr) == 1
                        m(x,2*y-1) = 2^n;
                    else
                        m(x,2*y-1) = -2^n;
                    end
                    ctr = ctr + 1;
                else
                    LIP = [LIP; x 2*y-1];
                    ctr = ctr + 1;
                end
                
                if ctr > size(in,2)
                    return
                end
                if in(1,ctr) == 1
                    ctr = ctr + 1;
                    LSP = [LSP; x 2*y];
                    if in(1,ctr) == 1
                        m(x,2*y) = 2^n;
                    else
                        m(x,2*y) = -2^n;
                    end
                    ctr = ctr + 1;
                else
                    LIP = [LIP; x 2*y];
                    ctr = ctr + 1;
                end
                if ((2*(2*y)-1) < size(m,2))
                    LIS = [LIS; LIStemp(i,1) LIStemp(i,2) 1];
                    LIStemp = [LIStemp; LIStemp(i,1) LIStemp(i,2) 1];
                end
                LIS(temp,:) = []; temp = temp-1;
                
            else
                ctr = ctr + 1;
            end
        else
            if in(1,ctr) == 1
                x = LIStemp(i,1); y = LIStemp(i,2);
                LIS = [LIS; x 2*y-1 0; x 2*y 0];
                LIStemp = [LIStemp; x 2*y-1 0; x 2*y 0];
                LIS(temp,:) = []; temp = temp - 1;
            end
            ctr = ctr + 1;
        end
        i = i+1;
    end
    
    % Refinement Pass
    temp = 1;
    while (temp<=LSP_idx)
        if ctr > size(in,2)
            return
        end
        m(LSP(temp,1),LSP(temp,2)) = m(LSP(temp,1),LSP(temp,2)) +sign(m(LSP(temp,1),LSP(temp,2)))*(2^n)*in(1,ctr);
        ctr = ctr + 1;
        temp = temp + 1;
    end
    
    %     % Refinement Pass
    %     temp = 1;
    %     value = m(LSP(temp,1), LSP(temp,2));
    %     while (abs(value) >= 2^(n+1) & (temp <= size(LSP,1)))
    %         if ctr > size(in,2)
    %             return
    %         end
    %
    %         value = value + ((-1)^(in(1,ctr) + 1)) * (2^(n-1))*sign(m(LSP(temp,1),LSP(temp,2)));
    %         m(LSP(temp,1),LSP(temp,2)) = value;
    %         ctr = ctr + 1;
    %         temp = temp + 1;
    %         if temp <= size(LSP,1)
    %             value = m(LSP(temp,1),LSP(temp,2));
    %         end
    %     end
    
    n = n-1;
end