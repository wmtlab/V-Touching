function [SMR,bandenergy] = PsychohapticModel(block,bl,dwtlevel,fs)
%UNTITLED8 Summary of this function goes here
%   Detailed explanation goes here

book = bl./(2.^([dwtlevel,dwtlevel:-1:1]))';

spect = 20.*log10(abs(1/sqrt(bl).*fft(block,2*bl)));
spect = spect(1:bl);
globalmask = GlobalMaskingThreshold(spect,bl,fs);
bandenergy = zeros(1,length(book));
maskenergy = zeros(1,length(book));
m = 0;
for i = 1:length(book)
    bandenergy(i) = sum(10.^(spect(m+1:m+book(i))./10));
    maskenergy(i) = sum(10.^(globalmask(m+1:m+book(i))./10));
    m = m+book(i);
end
    SMR = 10.*log10(bandenergy./maskenergy);
end

function [globalmask] = GlobalMaskingThreshold(spect,bl,fs)
%UNTITLED6 Summary of this function goes here
%   Detailed explanation goes here

freq = linspace(0,fs,2*bl);
freq = freq(1:bl);

percthres = PerceptualThreshold(bl,fs);

% [pks,ploc] = findpeaks(spect,'MinPeakProminence',12,'MinPeakHeight',-42);
[pks,ploc] = findpeaks(spect,'MinPeakProminence',12,'MinPeakHeight',max(spect)-45);
if isempty(pks)
    globalmask = percthres;
    return
end
masks = zeros(length(pks),bl);
for i = 1:length(pks)
    masks(i,:) = pks(i) - 5 + 10/fs*freq(ploc(i)) - 30/freq(ploc(i))^2.*(freq(1:bl)-freq(ploc(i))).^2;
%     masks(i,:) = pks(i) - 5 - 15/1400*freq(ploc(i)) - 30/freq(ploc(i))^2.*(freq(1:bl)-freq(ploc(i))).^2;
end
% mask = 10.*log10(sum(10.^(masks./10),1));
mask = max(masks,[],1);   
globalmask = 10.*log10(10.^(mask./10)+10.^(percthres./10));

%         figure;
%         semilogx(freq,spect);
%         hold on;
%         semilogx(freq, masks,'Color','r');
%         semilogx(freq, percthres,'Color',[0.47,0.67,0.19],'LineWidth',3);
%         semilogx(freq, globalmask,'Color','black','LineWidth',3);
%         ylim([-100,0]);
%         hold off;

end

function [percthres] = PerceptualThreshold(bl,fs)
%UNTITLED4 Summary of this function goes here
%   Detailed explanation goes here

freq = linspace(0,fs,2*bl);

% percthres = abs(60/(log10(3/7))^3*(log10(3/(7*300).*freq(1:bl)+3/7)).^3)-80;
a = 50;
c = 1/550;
b = 6/11;
e = 45;
percthres = abs(a/(log10(b))^2*(log10(c.*freq(1:bl)+b)).^2)-e;

end