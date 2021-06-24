using SRTPluginBase;
using System;

namespace SRTPluginProviderRE8
{
    internal class PluginInfo : IPluginInfo
    {
        public string Name => "Game Memory Provider (Resident Evil 8: Village (2021))";

        public string Description => "A game memory provider plugin for Resident Evil 8: Village (2021).";

        public string Author => "Squirrelies & VideoGameRoulette";

        public Uri MoreInfoURL => new Uri("https://github.com/VideoGameRoulette/SRTPluginProviderRE8");

        public int VersionMajor => assemblyVersion.Major;

        public int VersionMinor => assemblyVersion.Minor;

        public int VersionBuild => assemblyVersion.Build;

        public int VersionRevision => assemblyVersion.Revision;

        private Version assemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
    }
}
