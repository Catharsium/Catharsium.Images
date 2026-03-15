using Catharsium.Images.Core.Metadata.Interfaces;
using Catharsium.Images.Watermarking._Configuration;
using Catharsium.Images.Watermarking.Interfaces;
using Catharsium.Images.Watermarking.Models;
using Catharsium.Util.IO.Console.Interfaces;
using Catharsium.Util.IO.Files.Interfaces;

namespace Catharsium.Images.Watermarking.Services;

public class WatermarkApplicator(
    IFileFactory fileFactory,
    IWatermarkingService<IFile> imageWatermarkingService,
    IMetadataRetriever metadataRetriever,
    IFileNameService fileNameService,
    IConsole console,
    WatermarkingSettings settings)
    : IWatermarkApplicator
{
    public void Apply(string[] files) {
        IDirectory mainFolder = null,
            publicColorFolder = null, publicColorLogoFolder = null, publicGrayscaleFolder = null, publicGrayscaleLogoFolder = null,
            privateColorFolder = null, privateColorLogoFolder = null, privateGrayscaleFolder = null, privateGrayscaleLogoFolder = null;
        foreach(var file in files) {
            var sourceFile = fileFactory.CreateFile(file);
            var outputFile = sourceFile;

            var metadata = metadataRetriever.Get(sourceFile);
            var watermarkSet = this.GetWatermarkSet(metadata.Keywords);

            if(mainFolder == null) {
                (mainFolder, publicColorFolder, publicColorLogoFolder, publicGrayscaleFolder, publicGrayscaleLogoFolder, privateColorFolder, privateColorLogoFolder, privateGrayscaleFolder, privateGrayscaleLogoFolder) =
                    CreateSubfolders(fileFactory, fileNameService, sourceFile, $"{metadata.Series} ({metadata.Sublocation})", metadata.Timestamp);
            }

            outputFile = metadata.IsPrivate
                ? metadata.IsGrayScale
                    ? fileFactory.CreateFile(Path.Combine(privateGrayscaleLogoFolder.FullName, sourceFile.Name))
                    : fileFactory.CreateFile(Path.Combine(privateColorLogoFolder.FullName, sourceFile.Name))
                : metadata.IsGrayScale
                    ? fileFactory.CreateFile(Path.Combine(publicGrayscaleLogoFolder.FullName, sourceFile.Name))
                    : fileFactory.CreateFile(Path.Combine(publicColorLogoFolder.FullName, sourceFile.Name));

            var watermarksPortrait = watermarkSet.WatermarksPortrait?.Select(this.MapToImageWatermark);
            console.WriteLine($"Handling image {sourceFile.Name}");
            console.WriteLine($"\tSet: {watermarkSet.Name}");
            console.WriteLine($"\tOutput: {outputFile.FullName}");
            imageWatermarkingService.ApplyTo(sourceFile, outputFile, metadata.IsGrayScale, watermarkSet.Watermarks.Select(this.MapToImageWatermark), watermarksPortrait);

            MoveSourceFile(publicColorFolder, publicGrayscaleFolder, privateColorFolder, privateGrayscaleFolder, sourceFile, metadata.IsPrivate, metadata.IsGrayScale);
        }
    }


    public WatermarkSet GetWatermarkSet(string[] keywords) {
        var set = settings.DefaultSet;
        if(keywords != null) {
            foreach(var keyword in keywords) {
                var result = settings.Sets.FirstOrDefault(s => s.Name == keyword);
                if(result != null) {
                    set = result;
                    break;
                }
            }
        }

        return set;
    }


    private WatermarkRequest<IFile> MapToImageWatermark(Watermark w) {
        return new WatermarkRequest<IFile> {
            Image = fileFactory.CreateFile(w.Image),
            Scale = w.Scale,
            Anchor = w.Anchor,
            Flow = w.Flow,
            OffsetX = w.OffsetX,
            OffsetY = w.OffsetY,
            BackgroundMarginX = w.BackgroundMarginX,
            BackgroundMarginY = w.BackgroundMarginY,
        };
    }


    private static (IDirectory, IDirectory, IDirectory, IDirectory, IDirectory, IDirectory, IDirectory, IDirectory, IDirectory) CreateSubfolders(IFileFactory fileFactory, IFileNameService fileNameService, IFile sourceFile, string title, DateTimeOffset? date) {
        var mainFolder = fileFactory.CreateDirectory(Path.Combine(sourceFile.Directory.FullName, $"{date.Value:yyyy-MM-dd}, {fileNameService.SuggestValidFileName(title)}"));
        if(!mainFolder.Exists) {
            mainFolder.Create();
        }

        var publicFolder = fileFactory.CreateDirectory(Path.Combine(mainFolder.FullName, "Public"));
        if(!publicFolder.Exists) {
            publicFolder.Create();
        }

        var publicColorFolder = fileFactory.CreateDirectory(Path.Combine(publicFolder.FullName, "Color"));
        if(!publicColorFolder.Exists) {
            publicColorFolder.Create();
        }

        var publicColorLogoFolder = fileFactory.CreateDirectory(Path.Combine(publicFolder.FullName, "Color [Logo]"));
        if(!publicColorLogoFolder.Exists) {
            publicColorLogoFolder.Create();
        }

        var publicGrayscaleFolder = fileFactory.CreateDirectory(Path.Combine(publicFolder.FullName, "B&W"));
        if(!publicGrayscaleFolder.Exists) {
            publicGrayscaleFolder.Create();
        }

        var publicGrayscaleLogoFolder = fileFactory.CreateDirectory(Path.Combine(publicFolder.FullName, "B&W [Logo]"));
        if(!publicGrayscaleLogoFolder.Exists) {
            publicGrayscaleLogoFolder.Create();
        }

        var privateFolder = fileFactory.CreateDirectory(Path.Combine(mainFolder.FullName, "Private"));
        if(!privateFolder.Exists) {
            privateFolder.Create();
        }

        var privateColorFolder = fileFactory.CreateDirectory(Path.Combine(privateFolder.FullName, "Color"));
        if(!privateColorFolder.Exists) {
            privateColorFolder.Create();
        }

        var privateColorLogoFolder = fileFactory.CreateDirectory(Path.Combine(privateFolder.FullName, "Color [Logo]"));
        if(!privateColorLogoFolder.Exists) {
            privateColorLogoFolder.Create();
        }

        var privateGrayscaleFolder = fileFactory.CreateDirectory(Path.Combine(privateFolder.FullName, "B&W"));
        if(!privateGrayscaleFolder.Exists) {
            privateGrayscaleFolder.Create();
        }

        var privateGrayscaleLogoFolder = fileFactory.CreateDirectory(Path.Combine(privateFolder.FullName, "B&W [Logo]"));
        if(!privateGrayscaleLogoFolder.Exists) {
            privateGrayscaleLogoFolder.Create();
        }

        return (mainFolder, publicColorFolder, publicColorLogoFolder, publicGrayscaleFolder, publicGrayscaleLogoFolder, privateColorFolder, privateColorLogoFolder, privateGrayscaleFolder, privateGrayscaleLogoFolder);
    }


    private static void MoveSourceFile(IDirectory publicColorFolder, IDirectory publicGrayscaleFolder, IDirectory privateColorFolder, IDirectory privateGrayscaleFolder, IFile sourceFile, bool isPrivate, bool isGrayScale) {
        if(isPrivate) {
            if(isGrayScale) {
                sourceFile.MoveTo(Path.Combine(privateGrayscaleFolder.FullName, sourceFile.Name));
            }
            else {
                sourceFile.MoveTo(Path.Combine(privateColorFolder.FullName, sourceFile.Name));
            }
        }
        else {
            if(isGrayScale) {
                sourceFile.MoveTo(Path.Combine(publicGrayscaleFolder.FullName, sourceFile.Name));
            }
            else {
                sourceFile.MoveTo(Path.Combine(publicColorFolder.FullName, sourceFile.Name));
            }
        }
    }
}