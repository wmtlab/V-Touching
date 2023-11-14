import matlab
import numpy as np
import pandas as pd
import matlab.engine
import struct

from binary_serializer import BinarySerializer


class Decoder:
    def __init__(self):
        self.eng = matlab.engine.start_matlab()
        self.eng.cd(
            "./matlab",
            nargout=0,
        )
        self.eng

    def deserialize_and_decode(self, receive_buffer, start, point_count):
        # * Deserialization
        # * 反序列化
        read_index = start
        ret = []
        for i in range(0, point_count):
            read_index, binart_codes = BinarySerializer.read_bytes(
                receive_buffer, read_index
            )

            read_index, extrmum_Index_bytes = BinarySerializer.read_bytes(
                receive_buffer, read_index
            )
            extrmum_Index_num = len(extrmum_Index_bytes) // 8
            extrmum_Index = struct.unpack(f"{extrmum_Index_num}d", extrmum_Index_bytes)

            read_index, extrmum_Value_bytes = BinarySerializer.read_bytes(
                receive_buffer, read_index
            )
            extrmum_Value_num = len(extrmum_Value_bytes) // 8
            extrmum_Value = struct.unpack(f"{extrmum_Value_num}d", extrmum_Value_bytes)

            read_index, length = BinarySerializer.read_int(receive_buffer, read_index)

            # * Decoding
            # * 解码
            value = self.decode_py(binart_codes, extrmum_Index, extrmum_Value, length)
            int_value = [int(x * (1 << 15)) for x in value]
            ret.append(int_value)
        return ret

    def decode_py(self, binart_codes, extrmum_Index, extrmum_Value, l):
        num_elements = len(binart_codes)
        unpacked_ints = struct.unpack(f"{num_elements}b", binart_codes)

        binary_arrays = [
            [int(bit) for bit in bin((num + 256) % 256)[2:].zfill(8)]
            for num in unpacked_ints
        ]

        flattened_array = [
            bit for binary_array in binary_arrays for bit in binary_array
        ]
        flattened_array = flattened_array[:l]

        data = matlab.double(np.array(flattened_array).tolist())
        extrmum_Index = matlab.double(np.array(extrmum_Index).tolist())
        extrmum_Value = matlab.double(np.array(extrmum_Value).tolist())
        [sig] = self.eng.decode(data, extrmum_Index, extrmum_Value, nargout=1)

        return sig
