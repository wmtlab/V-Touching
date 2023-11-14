class BinarySerializer:
    @staticmethod
    def sizeof_int():
        return 4

    @staticmethod
    def read_int(buffer: bytes, index: int):
        end = index + BinarySerializer.sizeof_int()
        if end > len(buffer):
            raise IndexError()
        value = int.from_bytes(buffer[index:end], byteorder="little", signed=True)
        return (end, value)

    @staticmethod
    def write_int(buffer: bytearray, index: int, value: int):
        size = BinarySerializer.sizeof_int()
        end = index + size
        if end > len(buffer):
            raise IndexError()
        int_bytes = int.to_bytes(value, size, byteorder="little", signed=True)
        for i in range(0, size):
            buffer[index + i] = int_bytes[i]
        return end

    @staticmethod
    def read_bytes(buffer: bytes, index: int):
        index, count = BinarySerializer.read_int(buffer, index)
        end = index + count
        if end > len(buffer):
            raise IndexError()
        value = buffer[index:end]
        return (end, value)

    @staticmethod
    def write_bytes(buffer: bytearray, index: int, value: bytes):
        count = len(value)
        index = BinarySerializer.write_int(buffer, index, count)
        end = index + count
        if end > len(buffer):
            raise IndexError()
        for i in range(0, count):
            buffer[index + i] = value[i]
        return end
