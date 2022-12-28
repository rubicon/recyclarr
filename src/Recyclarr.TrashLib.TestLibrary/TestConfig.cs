using JetBrains.Annotations;
using Recyclarr.TrashLib.Config.Services;

namespace Recyclarr.TrashLib.TestLibrary;

[UsedImplicitly]
public class TestConfig : ServiceConfiguration
{
    public TestConfig()
    {
        Name = "Test";
    }
}