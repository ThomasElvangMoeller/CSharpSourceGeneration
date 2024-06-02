using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NotifySourceGenerator
{

    [Generator]
    public class NotifySourceGen : IIncrementalGenerator
    {
        const string _notifyAttribute = """
        using System;
        namespace NotifySourceGenerator
        {
            [AttributeUsage(AttributeTargets.Class)]
            public sealed class GenerateNotifyAttribute : Attribute { }
        }

        """;

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // add static code
            context.RegisterPostInitializationOutput(e => e.AddSource("NotifySourceGenAttribute.g.cs", SourceText.From(_notifyAttribute, Encoding.UTF8)));

            var classes = context.SyntaxProvider.ForAttributeWithMetadataName("NotifySourceGenerator.GenerateNotifyAttribute", GenPredicate, GenAction);

            context.RegisterSourceOutput(classes, GenerateSourceCode);
        }

        public static void GenerateSourceCode(SourceProductionContext context, GenModel model)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"namespace {model.Namespace}");
            sb.AppendLine("{");
            sb.AppendLine($"   public partial class {model.ClassName}");
            sb.AppendLine("   {");
            sb.AppendLine("      public void Notify(string parameter)");
            sb.AppendLine("      {");
            sb.AppendLine("         Console.WriteLine(\"Parameter '\" + parameter + \"' has changed!\");");
            sb.AppendLine("      }");
            foreach (var item in model.FieldNames)
            {
                sb.AppendLine($"      public {item.Item1} {item.Item2.ToUpperInvariant()}");
                sb.AppendLine("      {");
                sb.AppendLine($"         get => {item.Item2};");
                sb.AppendLine("         set");
                sb.AppendLine("         {");
                sb.AppendLine($"            {item.Item2} = value;");
                sb.AppendLine($"            Notify(\"{item.Item2.ToUpperInvariant()}\");");
                sb.AppendLine("         }");
                sb.AppendLine("      }");
                sb.AppendLine();
            }
            sb.AppendLine("   }");
            sb.AppendLine("}");


            context.AddSource(model.ClassName + ".g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
        }
        public static bool GenPredicate(SyntaxNode node, CancellationToken token)
        {
            if (node is ClassDeclarationSyntax classDeclarationSyntax)
            {
                var debug = classDeclarationSyntax.Modifiers.Any(e => e.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PartialKeyword));
                return debug;
            }
            return false;
        }

        public static GenModel GenAction(GeneratorAttributeSyntaxContext context, CancellationToken token)
        {
            if (context.TargetNode is ClassDeclarationSyntax cds)
            {
                string nmspc = cds.GetNamespace();
                string clssnm = cds.Identifier.Value as string ?? "CLASS MISSING";
                var fields2 = cds.Members.Where(e => e is FieldDeclarationSyntax).Cast<FieldDeclarationSyntax>().Select(e => e.Declaration);

                List<(string, string)> items = new List<(string, string)>();

                foreach (var field2 in fields2)
                {
                    items.AddRange(field2.Variables.Select(e => (field2.Type.ToString(), e.Identifier.Text)));
                }

                return new GenModel(nmspc, clssnm, items.ToArray());
            }
            return new GenModel();
        }

    }

    public static class Util
    {

        // determine the namespace the class/enum/struct is declared in, if any
        public static string GetNamespace(this BaseTypeDeclarationSyntax syntax)
        {
            // If we don't have a namespace at all we'll return an empty string
            // This accounts for the "default namespace" case
            string nameSpace = string.Empty;

            // Get the containing syntax node for the type declaration
            // (could be a nested type, for example)
            SyntaxNode? potentialNamespaceParent = syntax.Parent;

            // Keep moving "out" of nested classes etc until we get to a namespace
            // or until we run out of parents
            while (potentialNamespaceParent != null &&
                    potentialNamespaceParent is not NamespaceDeclarationSyntax
                    && potentialNamespaceParent is not FileScopedNamespaceDeclarationSyntax)
            {
                potentialNamespaceParent = potentialNamespaceParent.Parent;
            }

            // Build up the final namespace by looping until we no longer have a namespace declaration
            if (potentialNamespaceParent is BaseNamespaceDeclarationSyntax namespaceParent)
            {
                // We have a namespace. Use that as the type
                nameSpace = namespaceParent.Name.ToString();

                // Keep moving "out" of the namespace declarations until we 
                // run out of nested namespace declarations
                while (true)
                {
                    if (namespaceParent.Parent is not NamespaceDeclarationSyntax parent)
                    {
                        break;
                    }

                    // Add the outer namespace as a prefix to the final namespace
                    nameSpace = $"{namespaceParent.Name}.{nameSpace}";
                    namespaceParent = parent;
                }
            }

            // return the final namespace
            return nameSpace;
        }

    }

    public readonly record struct GenModel
    {
        public readonly string Namespace;
        public readonly string ClassName;
        public readonly (string, string)[] FieldNames;

        public GenModel(string @namespace, string className, (string, string)[] fieldNames)
        {
            Namespace = @namespace;
            ClassName = className;
            FieldNames = fieldNames;
        }
    }

}
