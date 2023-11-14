function [EncodeSignal] = Wavelet_processing_unity(signal,Bits_length,fs)
%% Wavelet_processing function
% 
b1 = 512; dwtlevel = 7; 
book = b1./(2.^([dwtlevel,dwtlevel:-1:1]))';
L = length(signal);
if mod(length(signal),b1) ~= 0
    L = length(signal)+b1-mod(length(signal),b1);
end
psignal = zeros(1,L);
for i = 1: length(signal)
    psignal(i) = signal(i);
end
% psignal(1:length(signal)) = signal;
% 
bitstream = cell(1,1);
contextstream = cell(1,1);
acstream = cell(1,1);
blocks = reshape(psignal,b1,[])';
numblocks = size(blocks,1);
bitalloc = cell(numblocks,1);
for i = 1:numblocks
    bitalloc{i} = zeros(1,length(book));
end

WaveletCoefficients = cell(numblocks,1);
bandenergy          = cell(numblocks,1);
SMR                 = cell(numblocks,1);
qSNR                = cell(numblocks,1);
MNR                 = cell(numblocks,1);
quant               = cell(numblocks,1);
intquant            = cell(numblocks,1);
bitblocks           = cell(numblocks,1);
context             = cell(numblocks,1);
ac_c                = cell(numblocks,1);
bitblockshead       = cell(numblocks,1);
achead       = cell(numblocks,1);
counter = [8,8,8,8,8,8,8];
counter_total = [16,16,16,16,16,16,16];

% 对每一个block分别进行编码
for j = 1:numblocks
    %% DWT 得到对应bolck的小波系数
    WaveletCoefficients{j} = wavecdf97(blocks(j,:),dwtlevel);

    %% 得到SMR数值和每个信号的能量
    [SMR{j},bandenergy{j}] = PsychohapticModel(blocks(j,:),b1,dwtlevel,fs);

    %% 计算小波系数的最大值，用于量化
    wavemax = max(abs(WaveletCoefficients{j}));
    if wavemax > 1
        wavemax = MaxQuant(wavemax-1,3,4); %-1标注一下
        wavmaxFlag = 1;
        bitwavmax = de2bi(wavemax.*2^4,7);
        wavemax = wavemax + 1;
    else
        wavemax = MaxQuant(wavemax,0,7);
        wavmaxFlag = 0;
        bitwavmax = de2bi(wavemax.*2^7,7);
    end

    noiseenergy = zeros(1,length(book));
    m = 0;
    for i = 1:length(book)
        quant{j}(m+1:m+book(i)) = DeadzoneQuant(WaveletCoefficients{j}(m+1:m+book(i)),wavemax,bitalloc{j}(i));
        noiseenergy(i) = sum(abs(WaveletCoefficients{j}(m+1:m+book(i))-quant{j}(m+1:m+book(i))).^2);
        m = m + book(i);
    end

    %% 量化
    while sum(bitalloc{j}) < Bits_length
        qSNR{j} = 10.*log10(bandenergy{j}./noiseenergy);
        MNR{j}  = qSNR{j} - SMR{j};

        MNR{j}(bitalloc{j}>=15) = Inf;
        [~,MinInd] = min(MNR{j});

        bitalloc{j}(MinInd) = bitalloc{j}(MinInd) + 1;

        m = 0;
        for i = 1:length(book)
            quant{j}(m+1:m+book(i)) = DeadzoneQuant(WaveletCoefficients{j}(m+1:m+book(i)),wavemax,bitalloc{j}(i));
            noiseenergy(i) = sum(abs(WaveletCoefficients{j}(m+1:m+book(i))-quant{j}(m+1:m+book(i))).^2);
            m = m + book(i);
        end
    end

    bitmax = max(bitalloc{j});
    intmax = 2^bitmax;
    rqmax  = repmat(wavemax,1,b1);
    intquant{j} = quant{j}.*intmax./rqmax;

    %% 编码
    [bitblocks{j},context{j}] = SPIHT_1D_Enc(intquant{j},b1,dwtlevel,bitwavmax,wavmaxFlag);
    [ac_c{j}] = AC_Enc(bitblocks{j},context{j},counter,counter_total); 
    %head
    switch b1
        case 32
            bitsize = 1;
        case 64
            bitsize = [0 1];
        case 128
            bitsize = [0 0 1];
        case 256
            bitsize = [0 0 0 1];
        case 512
            bitsize = [0 0 0 0];
    end
    bitblockshead{j} = [bitsize,de2bi(length(bitblocks{j}),14),bitblocks{j}];
    contexthead{j} = [bitsize,de2bi(length(context{j}),14),context{j}];
    achead{j} = [bitsize,de2bi(length(ac_c{j}),14),ac_c{j}];
    bitstream{1} = [bitstream{1},bitblockshead{j}];
    contextstream{1} = [contextstream{1},contexthead{j}];
    acstream{1} = [acstream{1},achead{j}];
end
EncodeSignal = bitstream{1};
ac_encodeSignal = acstream{1};


end