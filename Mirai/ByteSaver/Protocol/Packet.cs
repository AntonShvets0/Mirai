using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Mirai.Exceptions;

namespace Mirai.ByteSaver.Protocol
{
    public class Packet
    {
        public PacketField[] Fields;
        public static Assembly Assembly;
        
        
        public Packet(PacketField[] fields)
        {
            Fields = fields;
        }
        
        public byte[] ToBytes(Type type, bool root = false)
        {
            List<byte> list = new List<byte>();

            if (!root)
            {
                var name = SerializeName(type);
                list.Add((byte)name.Length);
                list.AddRange(Encoding.ASCII.GetBytes(name));
                list.Add(1);
            }
            
            for (int i = 0; i < Fields.Length; i++)
            {
                if (Fields[i].Value == null)
                {
                    list.Add(0);
                    continue;
                }
                
                var length = BitConverter.GetBytes(Fields[i].Value.Length);
                var typeName = SerializeName(Fields[i].Type);

                list.Add((byte)typeName.Length);
                list.AddRange(Encoding.ASCII.GetBytes(typeName));
                list.AddRange(length);
                list.AddRange(Fields[i].Value);
            }
            
            return list.ToArray();
        }

        public static Packet ParsePacket(byte[] inputBytes, bool isRoot)
        {
            return new Packet(HandleBytes(inputBytes, isRoot, "").Fields);
        }

        public static PacketField HandleBytes(byte[] bytes, bool isRoot, string defType)
        {
            if (bytes.Length == 0) return new PacketField();
            
            string type;

            if (!isRoot && defType == null) type = GetType(ref bytes);
            else
                type = defType;

            var nameType = DeserializeName(type).Replace("[", "").Replace("]", "");
            
            if (bytes.Length > 0 && bytes[0] != 1 && !isRoot)
            {
                return new PacketField
                {
                    Value = bytes,
                    Type = Type.GetType(nameType) 
                           ?? Assembly.GetTypes().FirstOrDefault(t => t.FullName == nameType)
                };
            }

            var oldBytes = new byte[bytes.Length];
            bytes.CopyTo(oldBytes, 0);

            var fields = new List<PacketField>();
            
            while (bytes.Length > 0)
            {
                var sizeType = bytes[0];
                bytes = bytes.Skip(1).ToArray();

                if (sizeType == 0)
                {
                    fields.Add(new PacketField());
                    continue;
                }
                
                var typeField = Encoding.ASCII.GetString(bytes.Take(sizeType).ToArray());
                bytes = bytes.Skip(sizeType).ToArray();

                var size = BitConverter.ToInt32(bytes.Take(4).ToArray());
                bytes = bytes.Skip(4).ToArray();

                var value = bytes.Take(size).ToArray();
                bytes = bytes.Skip(size).ToArray();
                
                fields.Add(HandleBytes(value, false, typeField));
            }

            return new PacketField
            {
                Value = oldBytes,
                Fields = fields.ToArray(),
                Type = Type.GetType(nameType) 
                       ?? Assembly.GetTypes().FirstOrDefault(t => t.FullName == nameType)
            };
        }

        public static string SerializeName(Type obj)
        {
            if (obj == typeof(string)) return "s";
            else if (obj == typeof(float)) return "f";
            else if (obj == typeof(char)) return "c";
            else if (obj == typeof(bool)) return "bb";
            else if (obj == typeof(int)) return "i";
            else if (obj == typeof(double)) return "d";
            else if (obj == typeof(decimal)) return "dd";
            else if (obj == typeof(long)) return "l";
            else if (obj == typeof(byte)) return "b";
            else if (obj.FullName == "System.RuntimeType") return "r";
            else if (obj == typeof(short)) return "ss";

            var name = obj.FullName;
            if (name.StartsWith("System.")) 
                return $"!{name.Substring(7)}";
            
            return name;
        }

        public static string DeserializeName(string s)
        {
            if (s.StartsWith("!")) 
                return $"System.{s.Substring(1)}";
            switch (s)
            {
                case "r": return "System.RuntimeType";
                case "s": return "System.String";
                case "f": return "System.Float";
                case "c": return "System.Char";
                case "bb": return "System.Boolean";
                case "i": return "System.Int32";
                case "d": return "System.Double";
                case "dd": return "System.Decimal";
                case "l": return "System.Int64";
                case "b": return "System.Byte";
                case "ss": return "System.Int16";
                default: return s;
            }
        }

        private static string GetType(ref byte[] bytes)
        {
            var count = bytes[0];
            bytes = bytes.Skip(1).ToArray();

            var type = bytes.Take(count).ToArray();
            bytes = bytes.Skip(count).ToArray();

            return Encoding.ASCII.GetString(type);
        }
    }
}