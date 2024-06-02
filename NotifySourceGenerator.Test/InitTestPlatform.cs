using System.Runtime.CompilerServices;

namespace NotifySourceGenerator.Test
{
    public static class InitTestPlatform
    {
        [ModuleInitializer]
        public static void Init()
        {
            VerifySourceGenerators.Initialize();
        }
    }
}
