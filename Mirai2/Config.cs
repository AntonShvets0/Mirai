using System.Collections.Generic;

namespace Mirai
{
    public class Config
    {
        private Dictionary<string, object> _config;

        public Config(Dictionary<string, object> config)
        {
            _config = config;
        }

        public object this[string key]
        {
            get => _config.ContainsKey(key) ? _config[key] : null;
            set
            {
                if (_config.ContainsKey(key))
                {
                    _config[key] = value;
                }
                else
                {
                    _config.Add(key, value);
                }
            }
        }
    }
}