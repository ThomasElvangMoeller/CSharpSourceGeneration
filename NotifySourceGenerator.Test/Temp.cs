using System.Collections.Generic;
namespace Noticeme.TestPlatform
{
    //[GenerateNotify]
    public partial class TestClassOriginalName
    {
        private int _field, _anotherField, thirdField;
        private Dictionary<int, string>? _dictionary;
        public string? Prop { get; set; }

        public int Field
        {
            get => _field;
            set
            {
                _field = value;
            }
        }
        public int GetValue()
        {
            return _field;
        }
    }
}

