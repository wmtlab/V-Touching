using System;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TouchMaterial
{
    public static class BinarySerializer
    {
        #region Read
        public static int ReadBool(this byte[] buffer, int index, out bool value)
        {
            value = default;
            int end = index + sizeof(bool);
            if (end > buffer.Length)
            {
                throw new IndexOutOfRangeException();
            }
            value = BitConverter.ToBoolean(buffer, index);
            return end;
        }

        public static int ReadInt(this byte[] buffer, int index, out int value)
        {
            value = default;
            int end = index + sizeof(int);
            if (end > buffer.Length)
            {
                throw new IndexOutOfRangeException();
            }
            value = BitConverter.ToInt32(buffer, index);
            return end;
        }

        public static int ReadLong(this byte[] buffer, int index, out long value)
        {
            value = default;
            int end = index + sizeof(long);
            if (end > buffer.Length)
            {
                throw new IndexOutOfRangeException();
            }
            value = BitConverter.ToInt64(buffer, index);
            return end;
        }

        public static int ReadFloat(this byte[] buffer, int index, out float value)
        {
            value = default;
            int end = index + sizeof(float);
            if (end > buffer.Length)
            {
                throw new IndexOutOfRangeException();
            }
            value = BitConverter.ToSingle(buffer, index);
            return end;
        }

        public static int ReadDouble(this byte[] buffer, int index, out double value)
        {
            value = default;
            int end = index + sizeof(double);
            if (end > buffer.Length)
            {
                throw new IndexOutOfRangeException();
            }
            value = BitConverter.ToDouble(buffer, index);
            return end;
        }

        public static int ReadString(this byte[] buffer, int index, out string value)
        {
            value = default;
            int lengthEnd = index + sizeof(int);
            if (lengthEnd > buffer.Length)
            {
                throw new IndexOutOfRangeException();
            }
            ReadInt(buffer, index, out int strBytesLength);
            int end = lengthEnd + strBytesLength;
            if (end > buffer.Length)
            {
                throw new IndexOutOfRangeException();
            }

            value = Encoding.UTF8.GetString(buffer, lengthEnd, strBytesLength);
            return end;
        }

        public static int ReadBytes(this byte[] buffer, int index, out byte[] value)
        {
            value = default;
            int lengthEnd = index + sizeof(int);
            if (lengthEnd > buffer.Length)
            {
                throw new IndexOutOfRangeException();
            }
            ReadInt(buffer, index, out int bytesLength);
            int end = lengthEnd + bytesLength;
            if (end > buffer.Length)
            {
                throw new IndexOutOfRangeException();
            }

            value = buffer.Skip(lengthEnd).Take(bytesLength).ToArray();
            return end;
        }

        public static int ReadVector3(this byte[] buffer, int index, out Vector3 value)
        {
            value = default;
            int end = index + 3 * sizeof(float);
            if (end > buffer.Length)
            {
                throw new IndexOutOfRangeException();
            }
            index = ReadFloat(buffer, index, out float x);
            index = ReadFloat(buffer, index, out float y);
            index = ReadFloat(buffer, index, out float z);
            value = new Vector3(x, y, z);
            return end;
        }

        public static int ReadQuaternion(this byte[] buffer, int index, out Quaternion value)
        {
            value = default;
            int end = index + 4 * sizeof(float);
            if (end > buffer.Length)
            {
                throw new IndexOutOfRangeException();
            }
            index = ReadFloat(buffer, index, out float x);
            index = ReadFloat(buffer, index, out float y);
            index = ReadFloat(buffer, index, out float z);
            index = ReadFloat(buffer, index, out float w);
            value = new Quaternion(x, y, z, w);
            return end;
        }
        #endregion

        #region Write
        public static int WriteBool(this byte[] buffer, int index, bool value)
        {
            int end = index + sizeof(bool);
            if (end > buffer.Length)
            {
                throw new IndexOutOfRangeException();
            }
            BitConverter.GetBytes(value).CopyTo(buffer, index);
            return end;
        }

        public static int WriteInt(this byte[] buffer, int index, int value)
        {
            int end = index + sizeof(int);
            if (end > buffer.Length)
            {
                throw new IndexOutOfRangeException();
            }
            BitConverter.GetBytes(value).CopyTo(buffer, index);
            return end;
        }

        public static int WriteLong(this byte[] buffer, int index, long value)
        {
            int end = index + sizeof(long);
            if (end > buffer.Length)
            {
                throw new IndexOutOfRangeException();
            }
            BitConverter.GetBytes(value).CopyTo(buffer, index);
            return end;
        }

        public static int WriteFloat(this byte[] buffer, int index, float value)
        {
            int end = index + sizeof(float);
            if (end > buffer.Length)
            {
                throw new IndexOutOfRangeException();
            }
            BitConverter.GetBytes(value).CopyTo(buffer, index);
            return end;
        }

        public static int WriteDouble(this byte[] buffer, int index, double value)
        {
            int end = index + sizeof(double);
            if (end > buffer.Length)
            {
                throw new IndexOutOfRangeException();
            }
            BitConverter.GetBytes(value).CopyTo(buffer, index);
            return end;
        }

        public static int WriteString(this byte[] buffer, int index, string value)
        {
            byte[] strBytes = Encoding.UTF8.GetBytes(value);
            int end = index + sizeof(int) + strBytes.Length;
            if (end > buffer.Length)
            {
                throw new IndexOutOfRangeException();
            }

            index = WriteInt(buffer, index, strBytes.Length);
            strBytes.CopyTo(buffer, index);
            return end;
        }

        public static int WriteBytes(this byte[] buffer, int index, byte[] value)
        {
            int end = index + sizeof(int) + value.Length;
            if (end > buffer.Length)
            {
                throw new IndexOutOfRangeException();
            }

            index = WriteInt(buffer, index, value.Length);
            value.CopyTo(buffer, index);
            return end;
        }

        public static int WriteVector3(this byte[] buffer, int index, Vector3 value)
        {
            int end = index + 3 * sizeof(float);
            if (end > buffer.Length)
            {
                throw new IndexOutOfRangeException();
            }
            index = WriteFloat(buffer, index, value.x);
            index = WriteFloat(buffer, index, value.y);
            index = WriteFloat(buffer, index, value.z);
            return end;
        }

        public static int WriteQuaternion(this byte[] buffer, int index, Quaternion value)
        {
            int end = index + 4 * sizeof(float);
            if (end > buffer.Length)
            {
                throw new IndexOutOfRangeException();
            }
            index = WriteFloat(buffer, index, value.x);
            index = WriteFloat(buffer, index, value.y);
            index = WriteFloat(buffer, index, value.z);
            index = WriteFloat(buffer, index, value.w);
            return end;
        }
        #endregion
    }

}
