using System.Collections.Generic;
using TrashLib.Radarr.CustomFormat.Models;

namespace TrashLib.Radarr.CustomFormat.Processors.GuideSteps
{
    public interface IQualityProfileStep
    {
        Dictionary<string, QualityProfileCustomFormatScoreMapping> ProfileScores { get; }
        List<(string name, string trashId, string profileName)> CustomFormatsWithoutScore { get; }
        void Process(IEnumerable<ProcessedConfigData> configData);
    }
}