using System.Collections;
using System.Collections.Generic;
using SFML.Graphics;

namespace Mirai.Styles
{
    public class Style
    {
        private Dictionary<StyleType, object> _values;

        public Style(Dictionary<StyleType, object> data)
        {
            _values = data;
        }

        public Style()
        {
            _values = new Dictionary<StyleType, object>();
        }

        public object Get(StyleType type)
        {
            if (_values.ContainsKey(type)) return _values[type];
            return null;
        }

        public int GetInt(StyleType type) => (int) (Get(type) ?? 0);
        public float GetFloat(StyleType type) => (float) (Get(type) ?? 0);

        public int? GetIntNullable(StyleType type) => (int?) Get(type);
        public float? GetFloatNullable(StyleType type) => (float?) Get(type);

        public Color? GetColor(StyleType type) => (Color?) Get(type);
        
        public void Add(StyleType type, object value)
        {
            if (_values.ContainsKey(type)) _values[type] = value;
            else _values.Add(type, value);
        }
    }
}