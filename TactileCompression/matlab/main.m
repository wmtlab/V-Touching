clc
clear variables
close all

%% 加载数据
filename = 'ACTK-vib-pantheongrandstarfall-8kHz-16-nopad.wav';
% filename = 'IDCC-vib-Rain_chan2-8kHz-16-nopad.wav';
% filename = 'IDCC-vib-Rain-8kHz-16-nopad.wav';  %%% NumChannels = 4;
filename = 'IDCC-vib-Towel-8kHz-16-nopad.wav';
TotalBits = 32;

rootpath = './vivo_data/';
filepath = strcat(rootpath, filename);
Input_WAV_Info = audioinfo(filepath);
[Input_WAV_Array, Input_WAV_Rate] = audioread(filepath, [1,inf], 'native'); 
signal = rescale(Input_WAV_Array, -1, 1);
% signal = sum(signal,2)/4;
subplot(3,2,1)
plot(signal),grid on;
title('原始信号')

%% 程序结构
% 1、利用巴特沃兹滤波器将信号进行分解
fs = Input_WAV_Info.SampleRate;
n  = 8;     % Order
fc = 72.5;  % Cutoff Frequency
[z,p,k] = butter(n,2*fc/fs);
[b,a] = zp2tf(z,p,k);
signal_lowpass = filtfilt(b,a,signal);
subplot(3,2,2)
plot(signal_lowpass),grid on;
title('低频信号')

% 2、提取低频进行编码重构（keyframe），计算残差
[key_frames,extrmum_Value,extrmum_Index] = Keyframe_extraction(signal_lowpass, fs);
subplot(3,2,3)
plot(extrmum_Index,extrmum_Value,'Linewidth',1.5),hold on;


% 3、将残差加到原始高频部分
% 将低频关键帧进行线性插值
% extrmum_Index = [1 extrmum_Index' length(signal)]';
% extrmum_Index = [1, extrmum_Index'];
% extrmum_Index = [extrmum_Index, length(signal)]';

% extrmum_Value = [signal_lowpass(1) extrmum_Value' signal_lowpass(length(signal))]';
signal_low = 1:1:length(signal);
signal_low = interp1(extrmum_Index',extrmum_Value',signal_low','linear');
plot(signal_low),grid on;
title('低频关键帧')

% 计算低频信号和低频关键帧之间的差别
signal_cancha = signal_lowpass - signal_low;
subplot(3,2,4)
plot(signal_cancha),grid on;
title('低频信号残差')

% 计算原始高频信号
signal_highpass = signal - signal_lowpass;
subplot(3,2,5)
plot(signal_highpass),grid on;
title('原始高频信号')

% 得到新的高频信号
signal_high = signal_highpass + signal_cancha;
subplot(3,2,6)
plot(signal_high),grid on;
title('最终的高频信号')

% 4、进行小波变换（Wavelet Transforms）

[rec,dist,PSNR,SNR, ac_decodeSignal] = Wavelet_processing(signal_high,TotalBits,fs);
disp(PSNR)
figure
subplot(3,1,1),plot(signal_high(1:length(signal))),hold on;title('高频信号')
subplot(3,1,2),plot(rec(1:length(signal))),hold on;title('恢复信号')
subplot(3,1,3),plot(ac_decodeSignal(1:length(signal))),hold on;title('编码信号')
