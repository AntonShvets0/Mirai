using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Mirai.ByteSaver.Attributes;
using Mirai.ByteSaver.Protocol;
using Mirai.Exceptions;

namespace Mirai.ByteSaver
{
    public class Serializer
    {
        private object _data;
        private bool _root;
        
        public Serializer(object classInstance, bool root = true)
        {
            _data = classInstance;
            _root = root;

            if (classInstance.GetType().GetCustomAttribute<ByteSerializableAttribute>() == null)
            {
                throw new PacketException("Class don't have byteserializable attribute");
            }
        }

        private PropertyInfo[] GetProperties() => _data.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        private byte[] ObjectToByteArray(object obj)
        {
            var bytes = new List<byte>();
            if (obj == null) return Array.Empty<byte>();
            var typeName = Packet.SerializeName(obj.GetType());
            
            bytes.Add((byte)typeName.Length);
            bytes.AddRange(Encoding.ASCII.GetBytes(typeName));

            if (obj is string stringObj)
            { 
                bytes.Add(0);
                bytes.AddRange(Encoding.UTF8.GetBytes(stringObj));

                return bytes.ToArray();
            }

            if (obj.GetType().IsArray)
            {
                var arr = obj as object[];
                var fields = new List<PacketField>();

                foreach (var o in arr)
                {
                    fields.Add(Packet.HandleBytes(ObjectToByteArray(o), false, null));
                }
                
                return new Packet(fields.ToArray()).ToBytes(arr.GetType().GetElementType());
            }
            
            if (!obj.GetType().IsValueType)
            {
                if (obj.GetType().GetCustomAttribute<ByteSerializableAttribute>() != null)
                {
                    return new Serializer(obj, false).ToBytes();
                }
                
                throw new PacketException("Only value types are available.");
            }

            var rawSize = Marshal.SizeOf(obj);
            var rawData = new byte[rawSize];

            var handle = GCHandle.Alloc(rawData,
                GCHandleType.Pinned);

            Marshal.StructureToPtr(obj,
                handle.AddrOfPinnedObject(),
                false);

            handle.Free();
            
            bytes.Add(0);
            bytes.AddRange(rawData);

            return bytes.ToArray();
        }

        public byte[] ToBytes()
        {
            var fields = new List<PacketField>();

            foreach (var prop in GetProperties())
            {
                if (prop.GetCustomAttribute<NonSerializedAttribute>() != null) continue;
                var value = prop.GetValue(_data);
                if (value == null)
                {
                    fields.Add(new PacketField());
                    continue;
                }
                
                var v = ObjectToByteArray(value);
                fields.Add(Packet.HandleBytes(v, false, null));
            }

            return new Packet(fields.ToArray()).ToBytes(_data.GetType(), _root);
        }
    }

    public class Serializer<T>
    {
        private Serializer _serializer;
        
        public Serializer(T classInstance)
        {
            _serializer = new Serializer(classInstance);
        }

        public byte[] ToBytes() => _serializer.ToBytes();
    }
}