﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.NugetNinja.Core;

public sealed class NugetVersion : ICloneable, IComparable<NugetVersion?>, IEquatable<NugetVersion?>
{
    public NugetVersion(string versionString)
    {
        this.SourceString = versionString;
        if (versionString.Contains("-"))
        {
            this.PrimaryVersion = Version.Parse(versionString.Split("-")[0]);
            this.AdditionalText = versionString.Split("-")[1].ToLower().Trim();
        }
        else
        {
            this.PrimaryVersion = Version.Parse(versionString);
        }
    }

    public string SourceString { get; set; }
    public Version PrimaryVersion { get; }
    public string AdditionalText { get; } = string.Empty;

    public static bool operator ==(NugetVersion? lvs, NugetVersion? rvs) => lvs?.Equals(rvs) ?? ReferenceEquals(lvs, rvs);

    public static bool operator !=(NugetVersion? lvs, NugetVersion? rvs) => !(lvs == rvs);

    public static bool operator <(NugetVersion? lvs, NugetVersion? rvs) => lvs?.CompareTo(rvs) < 0;

    public static bool operator >(NugetVersion? lvs, NugetVersion? rvs) => lvs?.CompareTo(rvs) > 0;

    public object Clone() => new NugetVersion(this.SourceString);

    public bool IsPreviewVersion() => !string.IsNullOrWhiteSpace(this.AdditionalText);

    public int CompareTo(NugetVersion? otherNugetVersion)
    {
        if (ReferenceEquals(otherNugetVersion, null))
        {
            throw new ArgumentNullException(paramName: nameof(otherNugetVersion));
        }

        if (!this.PrimaryVersion.Equals(otherNugetVersion.PrimaryVersion))
        {
            return this.PrimaryVersion.CompareTo(otherNugetVersion.PrimaryVersion);
        }

        if (string.IsNullOrWhiteSpace(this.AdditionalText) || string.IsNullOrWhiteSpace(otherNugetVersion.AdditionalText))
        {
            if (!string.IsNullOrWhiteSpace(this.AdditionalText))
            {
                return -1;
            }
            if (!string.IsNullOrWhiteSpace(otherNugetVersion.AdditionalText))
            {
                return 1;
            }
            return 0;
        }

        return string.CompareOrdinal(this.AdditionalText, otherNugetVersion.AdditionalText);
    }

    public bool Equals(NugetVersion? otherNugetVersion)
    {
        if (ReferenceEquals(otherNugetVersion, null))
        {
            return false;
        }
        return
            this.PrimaryVersion.Equals(otherNugetVersion.PrimaryVersion) &&
            this.AdditionalText.Equals(otherNugetVersion.AdditionalText);
    }

    public override string ToString()
    {
        return $"{PrimaryVersion}-{AdditionalText}".TrimEnd('-');
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
            return true;
        if (ReferenceEquals(this, null))
            return false;
        if (ReferenceEquals(obj, null))
            return false;
        if (obj is NugetVersion nuVersion)
            return this.Equals(nuVersion);
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.PrimaryVersion, this.AdditionalText);
    }
}
