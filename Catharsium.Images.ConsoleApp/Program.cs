﻿using Catharsium.Images.Watermarking.Interfaces;
using Catharsium.Util.IO.Console.Menu.Interfaces;
using Catharsium.Watermarker._Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catharsium.Watermarker;

public class Program
{
    public static async Task Main(string[] args) {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile($"appsettings.json");
        var configuration = builder.Build();

        var serviceProvider = new ServiceCollection()
            .AddWatermarking(configuration)
            .BuildServiceProvider();

        var files = args;
        files = new[] {
            "D:\\Onedrive\\Portfolio\\EPBF\\2024-06-23 10-01-58,  [EPBF 2024].jpg",
            "D:\\Onedrive\\Portfolio\\EPBF\\2024-06-23 10-06-12,  [EPBF 2024].jpg"
        };

        if(files.Any()) {
            var actionHandler = serviceProvider.GetService<IWatermarkApplicator>();
            actionHandler.Apply(files);
        }
        else {
            var mainMenuActionHandler = serviceProvider.GetService<IMainMenuActionHandler>();
            await mainMenuActionHandler.Run();
        }
    }
}