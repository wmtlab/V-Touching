function [key_frames,extrmum_Value,extrmum_Index] = Keyframe_extraction(y, fs)

extrmum_Value = y(find(diff(sign(diff(y)))~=0)+1);

extrmum_Index = find(diff(sign(diff(y)))~=0)+1;
time_stamp = extrmum_Index./fs;

key_frames = [1:length(extrmum_Index)];
key_frames = struct('amplitude_modulation',extrmum_Value, 'relative_position', extrmum_Index);

    
    