function globalTH_db = GlobalMaskingThreshold1(spectrum_db,blocksize,fs)
    freqSpectrumX = fs*(0:(2*blocksize/2)-1)/(2*blocksize);
    absoTH_db = absoluteThreshold(freqSpectrumX);
    absoTH = db2pow(absoTH_db);
    [peakMag,peakLoc] = findpeaks(spectrum_db,'MinPeakProminence',12,'MinPeakHeight',-42);
    peakMag = peakMag(:)';
    peakLoc = peakLoc(:)';
    peakTH_db = zeros(length(peakLoc),length(freqSpectrumX));
    for i = 1:length(peakLoc)
        peakTH_db(i,:) = peakMaskingThreshold(freqSpectrumX,freqSpectrumX(peakLoc(i)),fs,peakMag(i));
    end
    peakTH = db2pow(peakTH_db);
    globalTH = sum([absoTH;peakTH],1);
    globalTH_db = pow2db(globalTH); 
    
%     semilogx(freqSpectrumX,spectrum_db);
%     hold on;
%     ylim([-85,0]);
%     semilogx(freqSpectrumX, peakTH_db,'Color','r');
%     semilogx(freqSpectrumX, absoTH_db,'Color',[0.47,0.67,0.19],'LineWidth',3);
%     semilogx(freqSpectrumX, globalTH_db,'Color','black','LineWidth',3);
%     hold off;
%     close all
end