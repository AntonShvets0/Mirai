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
        
        public Serializer(object classInstance)
        {
            _data = classInstance;

            if (classInstance.GetType().GetCustomAttribute<ByteSerializableAttribute>() == null)
            {
                throw new PacketException("Class don't have byteserializable attribute");
            }
        }

        private PropertyInfo[] GetProperties() => _data.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        private byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return Array.Empty<byte>();

            if (obj is string stringObj)
            { 
                return Encoding.UTF8.GetBytes(stringObj);
            }

            if (obj.GetType().IsArray)
            {
                var arr = obj as object[];
                var fields = new List<PacketField>();

                foreach (var o in arr)
                {
                    fields.Add(Packet.HandleBytes(ObjectToByteArray(o)));
                }
                
                return new Packet(fields.ToArray()).ToBytes();
            }
            
            if (!obj.GetType().IsValueType)
            {
                if (obj.GetType().GetCustomAttribute<ByteSerializableAttribute>() != null)
                {
                    return new Serializer(obj).ToBytes();
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
            
            return rawData;
        }

        public byte[] ToBytes()
        {
            var fields = new List<PacketField>();

            foreach (var prop in GetProperties())
            {
                var value = prop.GetValue(_data);
                var v = ObjectToByteArray(value);
                fields.Add(Packet.HandleBytes(v));
            }
            
            return new Packet(fields.ToArray()).ToBytes();
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