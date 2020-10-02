using System;
using System.Collections.Generic;

public class Dism
{
    const int Success = 0;
    const int Success_And_Reboot_Required = 3010;

    public class Feature
    {
        public string Name { get; set; }
        public FeatureState State { get; set; }
        string displayName;

        public string DisplayName
        {
            get
            {
                GetDisplayName();
                return displayName;
            }
        }

        public bool EnableFeature()
        {
            var runner = new ProcessRunner();

            // The DISM command line options here are OS specific
            var dismParams = GetDismParams();
            var t = runner.RunProcess("Dism.exe", dismParams, null, s => { }, null)
                .ConfigureAwait(false);
            var exitcode = t.GetAwaiter().GetResult();
            return exitcode == Success || exitcode == Success_And_Reboot_Required;
        }

        string GetDismParams()
        {
            var operatingSystem = WindowsVersion.GetOperatingSystem();
            if (operatingSystem == Windows.Unsupported)
            {
                throw new Exception($"Unsupported operating system - cannot enable Feature {Name}");
            }
            if (operatingSystem == Windows.Workstation7 || operatingSystem == Windows.Server2008R2)
            {
                return $"/online /norestart /english /enable-feature /featurename:{Name}";
            }
            return $"/online /norestart /english /enable-feature /all /featurename:{Name}";
        }

        void GetDisplayName()
        {
            if (!string.IsNullOrEmpty(displayName))
            {
                return;
            }

            var runner = new ProcessRunner();
            var task = runner.RunProcess("Dism.exe", $"/online /english /get-featureinfo /featurename:{Name}", null, FeatureInfoParser, null).ConfigureAwait(false);
            if (task.GetAwaiter().GetResult() != Success)
            {
                displayName = Name; //fall back to name
            }
        }

        void FeatureInfoParser(string s)
        {
            const string displayNameLabel = "Display Name : ";

            if (!string.IsNullOrEmpty(displayName))
                return;

            if (s.StartsWith(displayNameLabel))
            {
                displayName = s.Remove(0, displayNameLabel.Length);
            }
        }
    }

    public enum FeatureState
    {
        Unknown,
        Disabled,
        Enabled,
        EnablePending,
        DisablePending
    }

    public static bool TryGetFeatures(out List<Feature> features)
    {
        var dismFeatures = new Dism();
        features = dismFeatures.features;
        var runner = new ProcessRunner();
        dismFeatures.features.Clear();
        var task = runner.RunProcess("Dism.exe", "/online /english /get-features", null, dismFeatures.FeatureParser, null).ConfigureAwait(false);
        return task.GetAwaiter().GetResult() == Success;
    }

    List<Feature> features = new List<Feature>();

    Feature currentFeature;

    void FeatureParser(string s)
    {
        const string featureNameLabel = "Feature Name : ";
        const string stateLabel = "State : ";

        if (s.StartsWith(featureNameLabel))
        {
            currentFeature = new Feature
            {
                Name = s.Remove(0, featureNameLabel.Length)
            };
            return;
        }

        if (!s.StartsWith(stateLabel)) return;

        FeatureState state;
        if (!Enum.TryParse(s.Remove(0, stateLabel.Length).Replace(" ", ""), out state))
        {
            state = FeatureState.Unknown;
        }
        currentFeature.State = state;
        features.Add(currentFeature);
    }
}
