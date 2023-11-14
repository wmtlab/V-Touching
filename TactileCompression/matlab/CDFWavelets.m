function [h ht g gt inds_h inds_ht inds_g inds_gt] = CDFWavelets(l,lt,varargin)
%--------------------------------------------------------------------------
% Syntax:   [h ht g gt inds_h inds_ht inds_g inds_gt] = CDFWavelets(l,lt);
%           [h ht g gt inds_h inds_ht inds_g inds_gt] = CDFWavelets(l,lt,disp);
%
% Inputs:   disp can be 'true' or 'false' and controls whether or not to
%           display H(w), Ht(w), G(w), and Gt(w). The default is 'true'.
%
% Outputs:  Computes the Cohen, Daubechies, and Feauveau (CDF) lowpass 
%           biorthogonal wavelet filters, h and ht, of length 2*(l+R)+1 and
%           2*(lt+C)+1, where R and C are the number of real and complex
%           zeros of P(t), deg(P)=l+lt-1, along with the corresponding
%           indices of the filter coefficients in inds_h and inds_ht,
%           respectively. The highpass filters, g and gt, are also returned
%           along with the filter indices, inds_g and inds_gt, respectively
%
% Note:     As per CDF, the real zeros of P(t) are placed in ht, and the
%           complex zeros of P(t) are placed in h.
%
% Author:   Brian Moore
%           brimoor@umich.edu
%--------------------------------------------------------------------------

% Parse user input
if isempty(varargin)
    disp = 'true';
else
    disp = varargin{1};
end

%--------------------------------------------------------------------------
% Compute left half of h
%--------------------------------------------------------------------------
N = 2*l;
h1 = sqrt(2)/(2^N)*ones(1,N+1);
for kk = 0:N
    h1(kk+1) = h1(kk+1) * nchoosek(N,kk);
end
%--------------------------------------------------------------------------

%--------------------------------------------------------------------------
% Compute left half of ht
%--------------------------------------------------------------------------
Nt = 2*lt;
ht1 = sqrt(2)/(2^Nt)*ones(1,Nt+1);
for kk = 0:Nt
    ht1(kk+1) = ht1(kk+1) * nchoosek(Nt,kk);
end
%--------------------------------------------------------------------------

%--------------------------------------------------------------------------
% Compute right halves of h and ht
%--------------------------------------------------------------------------
K = l + lt;
P = zeros(1,K);
for jj = 0:K-1
    P(K-jj) = nchoosek(K-1+jj,jj);
end
r = cplxpair(roots(P))';
jj = 1;
while jj <= length(r)
    if isreal(r(jj))
        break;
    end
    jj = jj + 1;
end

if ~mod(jj,2)
    error('All complex roots werent conjugates.');
elseif jj == 1
    % all real roots
    realRoots = r;
    complexRoots = [];
    C = 1;
    Ct = P(1);
elseif jj == length(r)+1
    % all complex roots
    realRoots = [];
    complexRoots = r;
    C = P(1);
    Ct = 1;
else
    % mix of real and complex roots
    realRoots = r(jj:end);
    complexRoots = r(1:jj-1);
    A = 1/prod(-r(jj:end));
    C = P(1)/A;
    Ct = A;
end

h2 = C;
for jj=1:2:length(complexRoots)
    a = real(complexRoots(jj));
    b = -imag(complexRoots(jj));
    h2 = conv([0.0625,a/2-0.25,a^2+b^2-a+0.375,a/2-0.25,0.0625],h2);
end

ht2 = Ct;
for jj=1:length(realRoots)
    ht2 = conv([-0.25,0.5-realRoots(jj),-0.25],ht2);
end
%--------------------------------------------------------------------------

%--------------------------------------------------------------------------
% Compute h and ht via convolution
%--------------------------------------------------------------------------
h = conv(h1,h2);
ht = conv(ht1,ht2);

inds_h = -(length(h)-1)/2:(length(h)-1)/2;
inds_ht = -(length(ht)-1)/2:(length(ht)-1)/2;
%--------------------------------------------------------------------------

%-------------------------------------------------------------------------- 
% Compute g and gt 
%-------------------------------------------------------------------------- 
inds_g = 1 - inds_ht(end:-1:1);
inds_gt = 1 - inds_h(end:-1:1);

g = ht(end:-1:1) .* (2*mod(inds_ht(end:-1:1),2) - 1);
gt = h(end:-1:1) .* (2*mod(inds_h(end:-1:1),2) - 1);
%-------------------------------------------------------------------------- 

%--------------------------------------------------------------------------
% Plot the frequency responses if desired
%--------------------------------------------------------------------------
if strcmp(disp,'true')
    
    fprintf(['\n',num2str(length(realRoots)),' real zeros\n']);
    fprintf([num2str(length(complexRoots)),' complex zeros\n\n']);

    res = 1000;
    w = linspace(0,pi,res);

    Hw = zeros(1,res);
    ind = 1;
    for k = inds_h
        Hw = Hw + h(ind)*exp(k*1i*w);
        ind = ind + 1;
    end

    Htw = zeros(1,res);
    ind = 1;
    for k = inds_ht
        Htw = Htw + ht(ind)*exp(k*1i*w);
        ind = ind + 1;
    end

    Gw = zeros(1,res);
    ind = 1;
    for k = inds_g
        Gw = Gw + g(ind)*exp(k*1i*w);
        ind = ind + 1;
    end

    Gtw = zeros(1,res);
    ind = 1;
    for k = inds_gt
        Gtw = Gtw + gt(ind)*exp(k*1i*w);
        ind = ind + 1;
    end

    figure
    
    subplot(2,2,1)
    plot(w,abs(Hw))
    title('H(\omega)')
    set(gca,'XTick',[0,pi/4,pi/2,3*pi/4,pi]);
    set(gca,'YTick',[0,sqrt(2)/4,sqrt(2)/2,3*sqrt(2)/4,sqrt(2)]);
    grid on
    axis tight

    subplot(2,2,2)
    plot(w,abs(Htw))
    title('Ht(\omega)')
    set(gca,'XTick',[0,pi/4,pi/2,3*pi/4,pi]);
    set(gca,'YTick',[0,sqrt(2)/4,sqrt(2)/2,3*sqrt(2)/4,sqrt(2)]);
    grid on
    axis tight

    subplot(2,2,3)
    plot(w,abs(Gw))
    title('G(\omega)')
    set(gca,'XTick',[0,pi/4,pi/2,3*pi/4,pi]);
    set(gca,'YTick',[0,sqrt(2)/4,sqrt(2)/2,3*sqrt(2)/4,sqrt(2)]);
    grid on
    axis tight

    subplot(2,2,4)
    plot(w,abs(Gtw))
    title('Gt(\omega)')
    set(gca,'XTick',[0,pi/4,pi/2,3*pi/4,pi]);
    set(gca,'YTick',[0,sqrt(2)/4,sqrt(2)/2,3*sqrt(2)/4,sqrt(2)]);
    grid on
    axis tight
    
    [~,handle] = suplabel(['Frequency Responses of ',num2str(length(h)),' / ',num2str(length(ht)),' CDF Wavelets'],'t');
    set(handle,'FontSize',14);

end
%--------------------------------------------------------------------------
