﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.NugetNinja.Core;
using Microsoft.NugetNinja.Framework;

namespace Microsoft.NugetNinja.PossiblePackageUpgradePlugin;

public class PackageUpgradeHandler : DetectorBasedCommandHandler<PackageReferenceUpgradeDetector, StartUp>
{
    public override string Name => "upgrade-pkg";

    public override string Description => "The command to upgrade all package references to possible latest and avoid conflicts.";

    public readonly Option<bool> AllowPreviewOption =
        new Option<bool>(
            aliases: new[] { "--allow-preview" },
            description: "Allow using preview versions of packages from Nuget.");

    public readonly Option<string> CustomNugetServer =
        new Option<string>(
            aliases: new[] { "--use-server" },
            description: "If you want to use a customized nuget server instead of the official nuget.org, you can set it with a value like: https://nuget.myserver/v3/index.json");

    public override Option[] GetOptions()
    {
        return new Option[]
        {
            AllowPreviewOption,
            CustomNugetServer
        };
    }

    public override void OnCommandBuilt(Command command)
    {
        var globalOptions = OptionsProvider.GetGlobalOptions();
        command.SetHandler(
            ExecutePackageUpgradeHandler,
            OptionsProvider.PathOptions,
            OptionsProvider.DryRunOption,
            OptionsProvider.VerboseOption,
            AllowPreviewOption,
            CustomNugetServer);
    }

    public Task ExecutePackageUpgradeHandler(string path, bool dryRun, bool verbose, bool allowPreviewOption, string customNugetServer)
    {
        var services = base.BuildServices(verbose);
        services.AddSingleton(new PackageUpgradeHandlerOptions(allowPreviewOption, customNugetServer));
        return base.RunFromServices(services, path, dryRun);
    }
}
