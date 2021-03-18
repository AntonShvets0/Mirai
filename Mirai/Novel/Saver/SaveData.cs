using Mirai.ByteSaver.Attributes;

namespace Mirai.Novel.Saver
{
    [ByteSerializable]
    public class SaveData
    {
        public string Scene { get; set; }
        
        public object Args { get; set; }
        
        public int Index { get; set; }
        
        public int TextIndex { get; set; }
    }
}