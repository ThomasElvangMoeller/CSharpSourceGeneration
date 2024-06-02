using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace NotifySourceGenerator.Test;

public class NotifyGenTest
{
    [Fact]
    public Task Test1()
    {
        var source = @"
using System.Collections.Generic;
using NotifySourceGenerator;
namespace NoticeMe.TestPlatform
{
    [GenerateNotify]
    public partial class TestClassOriginalName
    {
        private int _field, _anotherField, thirdField;
        private Dictionary<int, string>? _dictionary;
        public string? Prop { get; set; }
        public int GetValue()
        {
            return _field;
        }
    }
}";
        return TestHelper.Verify<NotifySourceGen>(source);
    }


}



public static class TestHelper
{
    public static Task Verify<T>(string source) where T : IIncrementalGenerator, new()
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

        IEnumerable<PortableExecutableReference> references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
        };

        CSharpCompilation cSharpCompilation = CSharpCompilation.Create("Tests", [syntaxTree], references);

        T generator = new T();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver = driver.RunGenerators(cSharpCompilation);

        return Verifier.Verify(driver).UseDirectory("Snapshots");
    }
}