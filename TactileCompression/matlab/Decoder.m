function [recsignal] = Decoder(stream)
%UNTITLED Summary of this function goes here
%   Detailed explanation goes here

m = 1;

while ~isempty(stream)
    if bi2de(stream([1]))==1
        origlength = 32;
    elseif bi2de(stream([1 2]))==2
        origlength = 64;
    elseif bi2de(stream([1 2 3]))==4
        origlength = 128;
    elseif bi2de(stream([1 2 3 4]))==8
        origlength = 256;
    elseif bi2de(stream([1 2 3 4]))==0
        origlength = 512;
    end
    segmentlength = bi2de(stream(5:18));
    stream = stream(19:end);
    recbitblocks{m} = stream(1:segmentlength);
    stream = stream(segmentlength+1:end);
    m = m+1;
end
numrecblocks = length(recbitblocks);
intrecblocks = cell(numrecblocks,1);
recwav = cell(numrecblocks,1);
recbitmax = zeros(1,numrecblocks);
recwavmax = zeros(1,numrecblocks);
for j = 1:numrecblocks
    [intrecblocks{j},recbitmax(j),recdwtlevel,recwavmax(j)] = SPIHT_1D_Dec(recbitblocks{j},origlength);
    recwav{j} = intrecblocks{j}.*recwavmax(j)./2^(recbitmax(j));
end

bl = length(recwav{1});

recblocks = zeros(numrecblocks,bl);
for j = 1:numrecblocks
    recblocks(j,:) = wavecdf97(recwav{j},-recdwtlevel);
end
recsignal = reshape(recblocks',[],1)';

end

