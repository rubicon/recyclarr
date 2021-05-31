﻿using System.Linq;
using Trash.Radarr.CustomFormat.Models;

namespace Trash.TestLibrary
{
    public static class CfTestUtils
    {
        public static QualityProfileCustomFormatScoreMapping NewMapping(params FormatMappingEntry[] entries)
            => new(false) {Mapping = entries.ToList()};

        public static QualityProfileCustomFormatScoreMapping NewMappingWithReset(params FormatMappingEntry[] entries)
            => new(true) {Mapping = entries.ToList()};
    }
}