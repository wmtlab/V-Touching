function [temp] = trans(array)
% temp = cell(1, 32000);
dimensions = size(array);
if dimensions(1) == 1
    for i = 1 : length(array)
        temp = [temp; array(i)];
    end
elseif dimensions(2) == 1
    for i = 1 : length(array)
        temp = [temp, array(i)];
    end
end

end