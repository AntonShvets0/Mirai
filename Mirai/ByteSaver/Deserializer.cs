using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Mirai.ByteSaver.Attributes;
using Mirai.ByteSaver.Protocol;
using Mirai.Exceptions;

namespace Mirai.ByteSaver
{
    public class Deserializer
    {
        private byte[] _bytes;
        private Type _type;
        private bool _isRoot = true;
        
        public Deserializer(byte[] bytes, Type type)
        {
            _bytes = bytes;
            _type = type;
            
            if (_type.GetCustomAttribute<ByteSerializableAttribute>() == null)
            {
                throw new PacketException("Class is not serializable attribute");
            }
        }

        public object Deserialize()
        {
            var packet = Packet.ParsePacket(_bytes, _isRoot);
            
            var instance = Activator.CreateInstance(_type);
            var type = instance?.GetType();
            if (type == null) throw new PacketException("Cannot instantiate class");

            var props = type.GetProperties();
            for (int i = 0; i < props.Length; i++)
            {
                if (props[i].GetCustomAttribute<NonSerializedAttribute>() != null) continue;
                if (packet.Fields.Length - 1 < i) break;
                if (packet.Fields[i].Value == null) continue;
                
                packet.Fields[i].Value = packet.Fields[i].Value.Skip(1).ToArray();
                SetValue(packet.Fields[i], props[i], instance);
            }

            return instance;
        }

        private void SetValue(PacketField value, PropertyInfo prop, object instance)
        {
            if (value.Fields != null)
            {
                var rank = 1;
                var arr = (GetDynamicObject(value, rank) as object[]).ToArray();
                var newArray = CreateArray(arr, value.Type);
                
                prop.SetValue(instance, newArray);
                return;
            }

            prop.SetValue(instance, GetValue(value.Value, value.Type));
        }

        private Array CreateArray(object[] array, Type t)
        {
            var newArray = Array.CreateInstance(t, array.Length);

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] is object[] arr) newArray.SetValue(CreateArray(arr, GetElementType(t)), i);
                else
                {
                    Array.Copy(array, newArray, newArray.Length);
                    break;
                }
            }

            return newArray;
        }
      
        private object GetDynamicObject(PacketField field, int maxRank, int rank = 0)
        {
            if (field.Fields == null || maxRank == rank) return GetValue(field.Value, field.Type);

            var list = new List<object>();

            foreach (var f in field.Fields)
            {
                list.Add(GetDynamicObject(f, maxRank, rank + 1));
            }

            return list.ToArray();
        }

        private object GetValue(byte[] value, Type type)
        {
            if (type == typeof(string) || type == typeof(string[])) return Encoding.UTF8.GetString(value);
            else if (type.IsValueType)
                return ByteArrayToFixedObject(value, type);
            else if ((type.IsArray 
                      && (GetElementType(type))?.GetCustomAttribute<ByteSerializableAttribute>() != null) || 
                type.GetCustomAttribute<ByteSerializableAttribute>() != null)
                return new Deserializer(value, type.IsArray ? GetElementType(type) : type)
                {
                    _isRoot = false
                }.Deserialize();
            else throw new PacketException("Unknown type");
        }

        private Type GetElementType(Type type)
        {
            return type.IsArray ? GetElementType(type.GetElementType()) : type;
        }
        
        
        private object ByteArrayToFixedObject(byte[] bytes, Type type)
        {
            // Я не знаю как это работает, но это работает. Просто не трогайте.
            
            object structure;

            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

            try
            {
                structure = Marshal.PtrToStructure(handle.AddrOfPinnedObject(), type);
            }
            finally
            {
                handle.Free();
            }

            return structure;
        }
    }
    
    public class Deserializer<T> where T : class
    {
        private Deserializer _deserializer;
        
        public Deserializer(byte[] bytes)
        {
            _deserializer = new Deserializer(bytes, typeof(T));
        }

        public T Deserialize() => _deserializer.Deserialize() as T;
    }
}