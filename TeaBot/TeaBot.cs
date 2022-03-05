using System;
using System.Net.Sockets;
using System.Text;
using TeaBot.DataType;
using System.Text.Json;
using System.Security.Cryptography;

namespace TeaBot
{
    public class TeaBot
    {

        public static byte[] GetVarInt(int theInt)
        {
            List<byte> bytes = new List<byte>();
            while ((theInt & -128) != 0)
            {
                bytes.Add((byte)(theInt & 127 | 128));
                theInt = (int)(((uint)theInt) >> 7);
            }
            bytes.Add((byte)theInt);
            return bytes.ToArray();
        }
        public static byte[] GetUShort(ushort theShort)
        {
            byte[] result = BitConverter.GetBytes(theShort);
            Array.Reverse(result);
            return result;
        }

        public static byte[] GetString(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            return Concat(GetVarInt(bytes.Length), bytes);
        }
        public static byte[] Concat(params byte[][] bytes)
        {
            List<byte> result = new List<byte>();
            foreach (byte[] array in bytes)
                result.AddRange(array);
            return result.ToArray();
        }

        public static Chat GetChat(string json)
        {
            Chat? result = JsonSerializer.Deserialize<Chat>(json);
            if(result == null)
            {
                result = new Chat();
                result.text = "null";
                Console.WriteLine($"[TeaBot] Chat-Component could not be serialized: {json} (Returning empty instead)");
            }
            return result;
        }

        public static byte[] ReadBytes(int offset, Queue<byte> data)
        {
            byte[] result = new byte[offset];
            for(int i = 0; i < offset; i++)
            {
                result[i] = data.Dequeue();
            }
            return result;
        }

        public static string ReadNextString(Queue<byte> data)
        {
            int lenght = ReadNextVarInt(data);
            string result = Encoding.UTF8.GetString(ReadBytes(lenght, data));
            return result;
        }
        public static int ReadNextVarInt(Queue<byte> data)
        {
            string rawData = BitConverter.ToString(data.ToArray());
            int i = 0;
            int j = 0;
            int k = 0;
            while (true)
            {
                k = data.Dequeue();
                i |= (k & 0x7F) << j++ * 7;
                if (j > 5) throw new OverflowException("VarInt too big " + rawData);
                if ((k & 0x80) != 128) break;
            }
            return i;
        }

        public static byte[] ReadNextByteArray(Queue<byte> data)
        {
            int lenght = ReadNextVarInt(data);
            return ReadBytes(lenght, data);
        }

        public static byte[] Compress(byte[] data)
        {
            //
            return data;
        }
    }
}