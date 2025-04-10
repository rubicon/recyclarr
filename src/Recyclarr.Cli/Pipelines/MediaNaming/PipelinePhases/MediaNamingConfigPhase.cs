using Autofac.Features.Indexed;
using Recyclarr.Cli.Pipelines.MediaNaming.PipelinePhases.Config;
using Recyclarr.Config.Models;
using Recyclarr.ServarrApi.MediaNaming;
using Recyclarr.TrashGuide;
using Recyclarr.TrashGuide.MediaNaming;

namespace Recyclarr.Cli.Pipelines.MediaNaming.PipelinePhases;

internal record InvalidNamingConfig(string Type, string ConfigValue);

internal record ProcessedNamingConfig
{
    public required MediaNamingDto Dto { get; init; }
    public IReadOnlyCollection<InvalidNamingConfig> InvalidNaming { get; init; } = [];
}

internal class MediaNamingConfigPhase(
    ILogger log,
    IMediaNamingGuideService guide,
    IIndex<SupportedServices, IServiceBasedMediaNamingConfigPhase> configPhaseStrategyFactory,
    IServiceConfiguration config
) : IPipelinePhase<MediaNamingPipelineContext>
{
    public async Task<PipelineFlow> Execute(
        MediaNamingPipelineContext context,
        CancellationToken ct
    )
    {
        var lookup = new NamingFormatLookup();
        var strategy = configPhaseStrategyFactory[config.ServiceType];
        var dto = await strategy.ProcessNaming(guide, lookup);

        context.ConfigOutput = new ProcessedNamingConfig
        {
            Dto = dto,
            InvalidNaming = lookup.Errors,
        };

        return LogAndExitIfNeeded(context.ConfigOutput);
    }

    private PipelineFlow LogAndExitIfNeeded(ProcessedNamingConfig configOutput)
    {
        if (configOutput.InvalidNaming.Count != 0)
        {
            foreach (var (topic, invalidValue) in configOutput.InvalidNaming)
            {
                log.Error(
                    "An invalid media naming format is specified for {Topic}: {Value}",
                    topic,
                    invalidValue
                );
            }

            return PipelineFlow.Terminate;
        }

        var differences = configOutput.Dto switch
        {
            RadarrMediaNamingDto x => x.GetDifferences(new RadarrMediaNamingDto()),
            SonarrMediaNamingDto x => x.GetDifferences(new SonarrMediaNamingDto()),
            _ => throw new ArgumentException("Unsupported configuration type"),
        };

        if (differences.Count == 0)
        {
            log.Debug("No media naming changes to process");
            return PipelineFlow.Terminate;
        }

        return PipelineFlow.Continue;
    }
}
