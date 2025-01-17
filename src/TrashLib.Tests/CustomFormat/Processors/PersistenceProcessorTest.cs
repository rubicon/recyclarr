using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;
using TrashLib.Services.CustomFormat.Api;
using TrashLib.Services.CustomFormat.Models;
using TrashLib.Services.CustomFormat.Models.Cache;
using TrashLib.Services.CustomFormat.Processors;
using TrashLib.Services.Radarr.Config;

namespace TrashLib.Tests.CustomFormat.Processors;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class PersistenceProcessorTest
{
    [Test]
    public async Task Custom_formats_are_deleted_if_deletion_option_is_enabled_in_config()
    {
        var steps = Substitute.For<IPersistenceProcessorSteps>();
        var cfApi = Substitute.For<ICustomFormatService>();
        var qpApi = Substitute.For<IQualityProfileService>();

        var config = new RadarrConfiguration {DeleteOldCustomFormats = true};

        var guideCfs = Array.Empty<ProcessedCustomFormatData>();
        var deletedCfsInCache = new Collection<TrashIdMapping>();
        var profileScores = new Dictionary<string, QualityProfileCustomFormatScoreMapping>();

        var processor = new PersistenceProcessor(cfApi, qpApi, config, steps);
        await processor.PersistCustomFormats(guideCfs, deletedCfsInCache, profileScores);

        steps.JsonTransactionStep.Received().RecordDeletions(Arg.Is(deletedCfsInCache), Arg.Any<List<JObject>>());
    }

    [Test]
    public async Task Custom_formats_are_not_deleted_if_deletion_option_is_disabled_in_config()
    {
        var steps = Substitute.For<IPersistenceProcessorSteps>();
        var cfApi = Substitute.For<ICustomFormatService>();
        var qpApi = Substitute.For<IQualityProfileService>();

        var config = new RadarrConfiguration {DeleteOldCustomFormats = false};

        var guideCfs = Array.Empty<ProcessedCustomFormatData>();
        var deletedCfsInCache = Array.Empty<TrashIdMapping>();
        var profileScores = new Dictionary<string, QualityProfileCustomFormatScoreMapping>();

        var processor = new PersistenceProcessor(cfApi, qpApi, config, steps);
        await processor.PersistCustomFormats(guideCfs, deletedCfsInCache, profileScores);

        steps.JsonTransactionStep.DidNotReceive()
            .RecordDeletions(Arg.Any<IEnumerable<TrashIdMapping>>(), Arg.Any<List<JObject>>());
    }
}
