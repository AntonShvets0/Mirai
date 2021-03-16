using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mirai.Exceptions;

namespace Mirai.ByteSaver.Protocol
{
    public class Packet
    {
        public PacketField[] Fields;
        public Packet(PacketField[] fields)
        {
            Fields = fields;
        }
        
        public byte[] ToBytes()
        {
            var list = new List<byte>()
            {
                (byte)'a'
            };

            for (int i = 0; i < Fields.Length; i++)
            {
                var length = BitConverter.GetBytes(Fields[i].Value.Length);

                list.Add((byte)length.Length);
                list.AddRange(length);
                list.AddRange(Fields[i].Value);
            }
            
            return list.ToArray();
        }

        public static Packet ParsePacket(byte[] inputBytes)
        {
            return new Packet(HandleBytes(inputBytes).Fields);
        }

        public static PacketField HandleBytes(byte[] bytes)
        {
            if (bytes.Length > 0 && bytes[0] != 'a')
            {
                return new PacketField
                {
                    Value = bytes
                };
            }

            var oldBytes = new byte[bytes.Length];
            bytes.CopyTo(oldBytes, 0);
            bytes = bytes.Skip(1).ToArray();

            var fields = new List<PacketField>();
            
            while (bytes.Length > 0)
            {
                var count = bytes[0];
                bytes = bytes.Skip(1).ToArray();

                var size = BitConverter.ToInt32(bytes.Take(count).ToArray());
                bytes = bytes.Skip(count).ToArray();

                var value = bytes.Take(size).ToArray();
                bytes = bytes.Skip(size).ToArray();
                
                fields.Add(HandleBytes(value));
            }

            return new PacketField
            {
                Value = oldBytes,
                Fields = fields.ToArray()
            };
        }
    }
}