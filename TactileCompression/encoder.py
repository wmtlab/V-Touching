import struct
import matlab
import numpy as np
import pandas as pd
import matlab.engine

from binary_serializer import BinarySerializer


class Encoder:
    def __init__(self):
        self.eng = matlab.engine.start_matlab()
        self.eng.cd(
            "./matlab",
            nargout=0,
        )
        self.eng

    def encode_and_serialize(self, received_data, data_to_send):
        index = 0
        point_count = len(received_data)
        for i in range(0, point_count):
            # * Encoding
            # * 编码
            binart_codes, extrmum_Index, extrmum_Value, length = self.encode_py(
                received_data[i]
            )

            # * Serialization
            # * 序列化
            index = BinarySerializer.write_bytes(data_to_send, index, binart_codes)

            extrmum_Index = extrmum_Index[0]

            extrmum_Index_bytes = struct.pack(f"{len(extrmum_Index)}d", *extrmum_Index)
            index = BinarySerializer.write_bytes(
                data_to_send, index, extrmum_Index_bytes
            )

            extrmum_Value = extrmum_Value[0]
            extrmum_Value_bytes = struct.pack(f"{len(extrmum_Value)}d", *extrmum_Value)
            index = BinarySerializer.write_bytes(
                data_to_send, index, extrmum_Value_bytes
            )

            index = BinarySerializer.write_int(data_to_send, index, length)
        return index

    def encode_py(self, input):
        sig = matlab.double(np.array(input).tolist())
        [binart_codes, extrmum_Index, extrmum_Value] = self.eng.encode(sig, nargout=3)
        l = len(binart_codes[0])
        binart_codes = [int(x) for x in binart_codes[0]]
        binart_codes = Encoder.merge_binary_to_decimal(binart_codes)
        binart_codes = bytes(binart_codes)
        return binart_codes, extrmum_Index, extrmum_Value, l

    def merge_binary_to_decimal(binary_array):
        decimal_array = []

        for i in range(0, len(binary_array), 8):
            eight_bits = binary_array[i : i + 8]
            binary_str = "".join(map(str, eight_bits))
            decimal_value = int(binary_str, 2)
            decimal_array.append(decimal_value)
        return decimal_array
