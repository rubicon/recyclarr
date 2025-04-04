using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

namespace Recyclarr.TestLibrary;

public static class MockFileSystemExtensions
{
    public static void AddFileFromEmbeddedResource(
        this MockFileSystem fs,
        IFileInfo path,
        Type typeInAssembly,
        string embeddedResourcePath
    )
    {
        fs.AddFileFromEmbeddedResource(path.FullName, typeInAssembly, embeddedResourcePath);
    }

    public static void AddFileFromEmbeddedResource(
        this MockFileSystem fs,
        string path,
        Type typeInAssembly,
        string embeddedResourcePath
    )
    {
        embeddedResourcePath = embeddedResourcePath.Replace("/", ".", StringComparison.Ordinal);
        var resourcePath = $"{typeInAssembly.Namespace}.{embeddedResourcePath}";
        fs.AddFileFromEmbeddedResource(path, typeInAssembly.Assembly, resourcePath);
    }

    public static void AddSameFileFromEmbeddedResource(
        this MockFileSystem fs,
        IFileInfo path,
        Type typeInAssembly,
        string resourceSubPath = "Data"
    )
    {
        fs.AddFileFromEmbeddedResource(path, typeInAssembly, $"{resourceSubPath}.{path.Name}");
    }

    public static void AddFilesFromEmbeddedNamespace(
        this MockFileSystem fs,
        IDirectoryInfo path,
        Type typeInAssembly,
        string embeddedResourcePath
    )
    {
        var replace = embeddedResourcePath.Replace("/", ".", StringComparison.Ordinal);
        embeddedResourcePath = $"{typeInAssembly.Namespace}.{replace}";
        fs.AddFilesFromEmbeddedNamespace(
            path.FullName,
            typeInAssembly.Assembly,
            embeddedResourcePath
        );
    }
}
