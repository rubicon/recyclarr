namespace Recyclarr.Cli.Tests.Reusable;

internal sealed class TrashRepoFileMapper : RemoteRepoFileMapper
{
    private const string RepoUrlPrefix =
        "https://raw.githubusercontent.com/TRaSH-Guides/Guides/refs/heads/master";

    public async Task DownloadFiles(params string[] repoFilePaths)
    {
        await base.DownloadFiles(RepoUrlPrefix, repoFilePaths);
    }
}
