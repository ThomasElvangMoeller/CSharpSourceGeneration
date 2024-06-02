//HintName: TestClassOriginalName.g.cs
namespace NoticeMe.TestPlatform
{
   public partial class TestClassOriginalName
   {
      public void Notify(string parameter)
      {
         Console.WriteLine("Parameter '" + parameter + "' has changed!");
      }
      public int _FIELD
      {
         get => _field;
         set
         {
            _field = value;
            Notify("_FIELD");
         }
      }

      public int _ANOTHERFIELD
      {
         get => _anotherField;
         set
         {
            _anotherField = value;
            Notify("_ANOTHERFIELD");
         }
      }

      public int THIRDFIELD
      {
         get => thirdField;
         set
         {
            thirdField = value;
            Notify("THIRDFIELD");
         }
      }

      public Dictionary<int, string>? _DICTIONARY
      {
         get => _dictionary;
         set
         {
            _dictionary = value;
            Notify("_DICTIONARY");
         }
      }

   }
}
