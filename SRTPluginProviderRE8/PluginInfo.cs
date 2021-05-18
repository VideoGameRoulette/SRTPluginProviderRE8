using SRTPluginBase;
using System;

namespace SRTPluginProviderRE8
{
    internal class PluginInfo : IPluginInfo
    {
        public string Name => "Game Memory Provider (Resident Evil 8: Village (2021))";

        public string Description => "A game memory provider plugin for Resident Evil 8: Village (2021).";

        public string Author => "Squirrelies & VideoGameRoulette";

        public Uri MoreInfoURL => new Uri("https://github.com/Squirrelies/SRTPluginProviderRE8");

        public int VersionMajor => assemblyFileVersion.ProductMajorPart;

        public int VersionMinor => assemblyFileVersion.ProductMinorPart;

        public int VersionBuild => assemblyFileVersion.ProductBuildPart;

        public int VersionRevision => assemblyFileVersion.ProductPrivatePart;

        private System.Diagnostics.FileVersionInfo assemblyFileVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
    }
}
