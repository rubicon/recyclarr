using System.IO.Abstractions;
using Recyclarr.Common;
using Recyclarr.Json.Loading;
using Recyclarr.Repo;

namespace Recyclarr.TrashGuide.MediaNaming;

public class MediaNamingGuideService : IMediaNamingGuideService
{
    private readonly IRepoMetadataBuilder _metadataBuilder;
    private readonly GuideJsonLoader _jsonLoader;

    public MediaNamingGuideService(IRepoMetadataBuilder metadataBuilder, GuideJsonLoader jsonLoader)
    {
        _metadataBuilder = metadataBuilder;
        _jsonLoader = jsonLoader;
    }

    private IReadOnlyList<IDirectoryInfo> CreatePaths(SupportedServices serviceType)
    {
        var metadata = _metadataBuilder.GetMetadata();
        return serviceType switch
        {
            SupportedServices.Radarr => _metadataBuilder.ToDirectoryInfoList(metadata.JsonPaths.Radarr.Naming),
            SupportedServices.Sonarr => _metadataBuilder.ToDirectoryInfoList(metadata.JsonPaths.Sonarr.Naming),
            _ => throw new ArgumentOutOfRangeException(nameof(serviceType), serviceType, null)
        };
    }

    private static IReadOnlyDictionary<string, string> JoinDictionaries(
        IEnumerable<IReadOnlyDictionary<string, string>> dictionaries)
    {
        return dictionaries
            .SelectMany(x => x.Select(y => (y.Key, y.Value)))
            .ToDictionary(x => x.Key.ToLowerInvariant(), x => x.Value);
    }

    public RadarrMediaNamingData GetRadarrNamingData()
    {
        var paths = CreatePaths(SupportedServices.Radarr);
        var data = _jsonLoader.LoadAllFilesAtPaths<RadarrMediaNamingData>(paths);
        return new RadarrMediaNamingData
        {
            File = JoinDictionaries(data.Select(x => x.File)),
            Folder = JoinDictionaries(data.Select(x => x.Folder))
        };
    }

    public SonarrMediaNamingData GetSonarrNamingData()
    {
        var paths = CreatePaths(SupportedServices.Sonarr);
        var data = _jsonLoader.LoadAllFilesAtPaths<SonarrMediaNamingData>(paths);
        return new SonarrMediaNamingData
        {
            Season = JoinDictionaries(data.Select(x => x.Season)),
            Series = JoinDictionaries(data.Select(x => x.Series)),
            Episodes = new SonarrEpisodeNamingData
            {
                Anime = JoinDictionaries(data.Select(x => x.Episodes.Anime)),
                Daily = JoinDictionaries(data.Select(x => x.Episodes.Daily)),
                Standard = JoinDictionaries(data.Select(x => x.Episodes.Standard))
            }
        };
    }
}