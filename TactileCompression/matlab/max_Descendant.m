function value = max_Descendant(i, j, type, m)
% Matlab implementation of SPIHT (without Arithmatic coding stage)
%
% Find the descendant with largest absolute value of pixel (i,j)
%
% input:    i : row coordinate
%           j : column coordinate
%           type : type of descendant
%           m : whole image
%
% output:   value : largest absolute value
%

s = size(m,2);

S = [];

index = 0; 
b = 0;

while ((2*j-1)<s)
    b = j-1;
    nind = 2*(b+1)-1:2*(b+2^index);
    chk = nind <= s;
    len = sum(chk);
    if len < length(nind)
        nind(len+1:length(nind)) = [];
    end
    S = [S reshape(m(i,nind),1,[])];
    index = index + 1;
    j = 2*b+1;
end

if type == 1
    S(:,1:2) = [];
end

value = max(abs(S));