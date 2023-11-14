import os
import matlab
import numpy as np
import pandas as pd
import struct
import matlab.engine
import datetime


def encode_py(sig):
    eng = matlab.engine.start_matlab()
    sig = matlab.double(np.array(sig).tolist())
    [binart_codes, key_frames] = eng.encode(sig, nargout=2)
    # print(binart_codes[0])
    binart_codes = [int(x) for x in binart_codes[0]]
    binart_codes = merge_binary_to_decimal(binart_codes)
    binart_codes = bytes(binart_codes)
    return binart_codes, key_frames


def merge_binary_to_decimal(binary_array):
    decimal_array = []

    for i in range(0, len(binary_array), 8):
        eight_bits = binary_array[i : i + 8]
        binary_str = "".join(map(str, eight_bits))
        decimal_value = int(binary_str, 2)
        decimal_array.append(decimal_value)
    return decimal_array


def decode_py(binart_codes, key_frames):
    num_elements = len(binart_codes)
    unpacked_ints = struct.unpack(f"{num_elements}b", binart_codes)

    binary_arrays = [
        [int(bit) for bit in bin((num + 256) % 256)[2:].zfill(8)]
        for num in unpacked_ints
    ]

    flattened_array = [bit for binary_array in binary_arrays for bit in binary_array]

    eng = matlab.engine.start_matlab()
    data = matlab.double(np.array(flattened_array).tolist())
    [sig] = eng.decode(data, nargout=1)

    return sig


if __name__ == "__main__":
    data_path = "./matlab/vivo_data/data.txt"
    data = pd.read_table(data_path, header=None, delim_whitespace=True)
    binart_codes, key_frames = encode_py(data)

    sig = decode_py(binart_codes, key_frames)

    print(sig)
