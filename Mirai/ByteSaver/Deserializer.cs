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
            var packet = Packet.ParsePacket(_bytes);
            
            var instance = Activator.CreateInstance(_type);
            var type = instance?.GetType();
            if (type == null) throw new PacketException("Cannot instantiate class");

            var props = type.GetProperties();
            for (int i = 0; i < props.Length; i++)
            {
                if (packet.Fields.Length - 1 < i) break;
                SetValue(packet.Fields[i], props[i], instance);
            }

            return instance;
        }

        private void SetValue(PacketField value, PropertyInfo prop, object instance)
        {
            if (prop.PropertyType.IsArray)
            {
                var rank = 2;
                var arr = (GetDynamicObject(prop, value, rank) as object[]).ToArray();
                var newArray = CreateArray(arr, prop.PropertyType.GetElementType());
                
                prop.SetValue(instance, newArray);
                return;
            }

            prop.SetValue(instance, GetValue(value.Value, prop));
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
      
        private object GetDynamicObject(PropertyInfo prop, PacketField field, int maxRank, int rank = 0)
        {
            if (field.Fields == null || maxRank == rank) return GetValue(field.Value, prop);

            var list = new List<object>();

            foreach (var f in field.Fields)
            {
                list.Add(GetDynamicObject(prop, f, maxRank, rank + 1));
            }

            return list.ToArray();
        }

        private object GetValue(byte[] value, PropertyInfo prop)
        {
            if (prop.PropertyType == typeof(string) || prop.PropertyType == typeof(string[])) return Encoding.UTF8.GetString(value);
            else if (prop.PropertyType.IsValueType)
                return ByteArrayToFixedObject(value, prop.PropertyType);
            else if ((prop.PropertyType.IsArray 
                      && (GetElementType(prop.PropertyType))?.GetCustomAttribute<ByteSerializableAttribute>() != null) || 
                prop.PropertyType.GetCustomAttribute<ByteSerializableAttribute>() != null)
                return new Deserializer(value, prop.PropertyType.IsArray ? GetElementType(prop.PropertyType): prop.PropertyType).Deserialize();
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