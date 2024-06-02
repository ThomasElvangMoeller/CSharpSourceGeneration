//HintName: NotifySourceGenAttribute.g.cs
using System;
namespace NotifySourceGenerator
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class GenerateNotifyAttribute : Attribute { }
}
