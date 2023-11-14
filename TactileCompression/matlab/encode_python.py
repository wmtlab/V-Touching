import os
import matlab
import numpy as np
import pandas as pd
import struct
import matlab.engine


def encode_py(sig):
    eng = matlab.engine.start_matlab()
    eng.cd(
        "D:\\SvnWorkSpaces\\Unity\\TouchMaterial\\TactileCompression\\matlab", nargout=0
    )

    sig = matlab.double(np.array(sig).tolist())

    [binart_codes, key_frames] = eng.encode(sig, nargout=2)
    binart_codes = [int(x) for x in binart_codes[0]]
    l = len(binart_codes)
    binart_codes = merge_binary_to_decimal(binart_codes)
    binart_codes = bytes(binart_codes)
    return binart_codes, key_frames, l


def merge_binary_to_decimal(binary_array):
    decimal_array = []

    for i in range(0, len(binary_array), 8):
        eight_bits = binary_array[i : i + 8]
        binary_str = "".join(map(str, eight_bits))
        decimal_value = int(binary_str, 2)
        decimal_array.append(decimal_value)
    return decimal_array


def decode_py(binart_codes, l):
    # binart_codes = b"\x03\xa4:\xc0(\xfd\xc9\xf7(\xb5\xbe\xcc\xba\xa0\x02\x1cG\x97L+\xfc\x0c2\x87\x14\x0124\x86@=\xc5f\x0c\x10}\x12\xb3\xd2\xf6\x00\x00\x02\xdc\x83\x13\xc8\x06f\x00\x00\x0c\xde2\x19E\xf2u\x86\x00\x0b\x14\x1f\x03\x02\xb9$<\x00\x00\x00\xf4\x0c\x00\x01\x00`\x00"
    num_elements = len(binart_codes)
    unpacked_ints = struct.unpack(f"{num_elements}b", binart_codes)

    binary_arrays = [
        [int(bit) for bit in bin((num + 256) % 256)[2:].zfill(8)]
        for num in unpacked_ints
    ]

    flattened_array = [bit for binary_array in binary_arrays for bit in binary_array]
    flattened_array = flattened_array[:l]
    eng = matlab.engine.start_matlab()
    eng.cd(
        "D:\\SvnWorkSpaces\\Unity\\TouchMaterial\\TactileCompression\\matlab", nargout=0
    )
    data = matlab.double(np.array(flattened_array).tolist())
    [sig] = eng.decode(data, nargout=1)

    return sig


if __name__ == "__main__":
    data_path = "./matlab/vivo_data/data.txt"
    data = pd.read_table(data_path, header=None, delim_whitespace=True)
    binart_codes, key_frames, l = encode_py(data)
    sig = decode_py(binart_codes, l)
    print(sig)
