function [out, context] = SPIHT_1D_Enc(m, block_size, level, bitwavmax, wavmaxFlag)


%-----------   Initialization  -----------------
n_max = floor(log2(max(abs(m))'));
%-----------   output bitstream header   ----------------
% sequence size, number of bit plane, wavelet decomposition level should be
% written as bit stream header.
bitn = de2bi(n_max+1,4);
% bitlevel = de2bi(level,3);
out(1,[1 2 3 4 5]) = [ bitn wavmaxFlag];
out = [out, bitwavmax];

% out = [out,bitsize];
context = zeros(1,12);

%-----------   Initialize LIP, LSP, LIS   ----------------
bandsize = 2.^(log2(size(m, 2)) - level + 1);
temp = 1 : bandsize;
LIP(:, 1) = ones(bandsize,1);
LIP(:, 2) = temp';

LIS(:, 1) = LIP(bandsize/2+1:end, 1);
LIS(:, 2) = LIP(bandsize/2+1:end, 2);
LIS(:, 3) = zeros(length(LIP(bandsize/2+1:end, 1)), 1);
LSP = [];

n = n_max;

%-----------   coding   ----------------
while(0 <= n) % loop over bitplanes

    LSP_idx = size(LSP,1); % to be used in refinement pass
    % Sorting Pass
    LIPtemp = LIP; temp = 0;
    for i = 1:size(LIPtemp,1)
        temp = temp+1;
        if abs(m(LIPtemp(i,1),LIPtemp(i,2))) >= 2^n % 1: positive; 0: negative
            out = [out, 1]; 
            context = [context, 2];
            sgn = m(LIPtemp(i,1),LIPtemp(i,2))>=0;
            out = [out, sgn];
            context = [context, 1];
            LSP = [LSP; LIPtemp(i,:)];
            LIP(temp,:) = []; temp = temp - 1;
        else
            out = [out, 0]; 
            context = [context, 2];
        end
    end

    LIStemp = LIS; temp = 0; i = 1;
    while ( i <= size(LIStemp,1))
        temp = temp + 1;
        % If type A
        if LIStemp(i,3) == 0 
            max_d = max_Descendant(LIStemp(i,1),LIStemp(i,2),LIStemp(i,3),m);
            if max_d >= 2^n
                out = [out, 1]; 
                context = [context, 3];
                x = LIStemp(i,1); y = LIStemp(i,2);
                % Childeren
                if abs(m(x,2*y-1)) >= 2^n
                    LSP = [LSP; x 2*y-1];
                    out = [out, 1]; 
                    context = [context, 4];
                    sgn = m(x,2*y-1)>=0;
                    out = [out, sgn]; 
                    context = [context, 1];
                else
                    out = [out, 0];
                    context = [context, 4];
                    LIP = [LIP; x 2*y-1];
                end
                if abs(m(x,2*y)) >= 2^n
                    LSP = [LSP; x 2*y];
                    out = [out, 1];
                    context = [context, 4];
                    sgn = m(x,2*y)>=0;
                    out = [out, sgn];    
                    context = [context, 1];
                else
                    out = [out, 0];
                    context = [context, 4];
                    LIP = [LIP; x 2*y];
                end     
                % If there exist Grandchilderen
                if ((2*(2*y)-1) < size(m,2))
                    LIS = [LIS; LIStemp(i,1) LIStemp(i,2) 1];  % change it to type B and append to the list
                    LIStemp = [LIStemp; LIStemp(i,1) LIStemp(i,2) 1]; % change it to type B and append to the list
                end
                LIS(temp,:) = []; temp = temp-1;
                
            else
                out = [out, 0];
                context = [context, 3];
            end
        % If type B
        else
            max_d = max_Descendant(LIStemp(i,1),LIStemp(i,2),LIStemp(i,3),m);
            if max_d >= 2^n
                out = [out, 1]; 
                context = [context, 5];
                x = LIStemp(i,1); y = LIStemp(i,2);
                LIS = [LIS; x 2*y-1 0; x 2*y 0];
                LIStemp = [LIStemp; x 2*y-1 0; x 2*y 0];
                LIS(temp,:) = []; temp = temp - 1;
            else
                out = [out, 0];
                context = [context, 5];
            end
        end
        i = i+1;
    end
    
    
   
    % Refinement Pass
   temp = 1;
   while (temp<=LSP_idx)
       s = bitget(floor(abs(m(LSP(temp,1),LSP(temp,2)))),n+1);
       out = [out, s];  
       context = [context, 6];
       temp = temp + 1;
   end
    

    n = n - 1;
end
end