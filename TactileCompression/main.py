import socket

from binary_serializer import BinarySerializer
from encoder import Encoder
from decoder import Decoder


def main():
    udp_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    udp_socket.bind(("127.0.0.1", 5300))
    server_addr = ("127.0.0.1", 5301)
    client_addr = ("127.0.0.1", 5302)

    point_count = 3
    compress_frame = 512
    buffer_size = 8192
    SERVER = 0
    CLIENT = 1

    # An int is added at the beginning of the encoded tactile feedback to identify which side the data is from.
    # If you want to separate Encoder and Decoder, you can remove this int.
    # 编码的触觉反馈开头添加了一个int，用于标识是哪一边发来的数据，如果要把Encoder和Decoder分开，可以把这个int去掉

    # [ [compress_frame (512) frames of data for one touch point] ... ]
    # The tactile data on the python side are all ints instead of floats, and the range is -32768~32767, corresponding to floats -1.0~1.0
    # [ [一个触点compress_frame帧（512）的数据] ... ]
    # python端都是整数而不是浮点数，范围是-32768~32767，对应的浮点数是-1.0~1.0
    received_data = [
        [0 for i in range(0, compress_frame)] for j in range(0, point_count)
    ]
    data_to_send = bytearray([0 for i in range(0, buffer_size)])

    encoder = Encoder()
    decoder = Decoder()

    print("socket running")
    while True:
        # * Receiving data
        # * 接收数据
        receive_buffer, addr = udp_socket.recvfrom(buffer_size)
        start, side_type = BinarySerializer.read_int(receive_buffer, 0)
        # * Encoder
        # * 编码器
        if side_type == SERVER:
            # * Deserialization
            # * 反序列化
            read_index = start
            for i in range(0, point_count):
                for j in range(0, compress_frame):
                    read_index, received_data[i][j] = BinarySerializer.read_int(
                        receive_buffer, read_index
                    )

            # ** region Encoder customizable
            # start_time = time.perf_counter()
            write_index = encoder.encode_and_serialize(received_data, data_to_send)
            # print(f"{(time.perf_counter() - start_time) * 1000}ms")
            # ** endregion Encoder customizable

            # * Sending data
            # * 发送数据
            udp_socket.sendto(data_to_send[:write_index], server_addr)
        # * Decoder
        # * 解码器
        elif side_type == CLIENT:
            # ** region Decoder customizable
            values = decoder.deserialize_and_decode(receive_buffer, start, point_count)
            # ** endregion Decoder customizable

            # * Serialization
            # * 序列化
            write_index = 0
            for i in range(0, point_count):
                for j in range(0, compress_frame):
                    write_index = BinarySerializer.write_int(
                        data_to_send, write_index, values[i][j]
                    )

            # * Sending data
            # * 发送数据
            udp_socket.sendto(data_to_send[:write_index], client_addr)


if __name__ == "__main__":
    main()
