namespace NotifySourceGenerator.ConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
    }

    [GenerateNotify]
    public partial class TestClass
    {
        private int _field, _anotherField, thirdField;
        private Dictionary<int, string>? _dictionary;
        public string? Prop { get; set; }
        public int GetValue()
        {
            return _field;
        }
    }
}
