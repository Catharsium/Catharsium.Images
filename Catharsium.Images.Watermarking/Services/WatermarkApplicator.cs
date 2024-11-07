using Catharsium.Images.Watermarking._Configuration;
using Catharsium.Images.Watermarking.Interfaces;
using Catharsium.Images.Watermarking.Models;
using Catharsium.Util.IO.Files.Interfaces;
using MetadataExtractor;
using MetadataExtractor.Formats.Iptc;

namespace Catharsium.Images.Watermarking.Services;

public class WatermarkApplicator : IWatermarkApplicator
{
    private readonly IFileFactory fileFactory;
    private readonly IWatermarkingService<IFile> imageWatermarkingService;
    private readonly IWatermarkingService<string> textWatermarkingService;
    private readonly WatermarkingSettings settings;


    public WatermarkApplicator(
        IFileFactory fileFactory,
        IWatermarkingService<IFile> imageWatermarkingService,
        IWatermarkingService<string> textWatermarkingService,
        WatermarkingSettings settings) {
        this.fileFactory = fileFactory;
        this.imageWatermarkingService = imageWatermarkingService;
        this.textWatermarkingService = textWatermarkingService;
        this.settings = settings;
    }


    public void Apply(string[] files) {
        foreach(var file in files) {
            var sourceFile = this.fileFactory.CreateFile(file);
            var outputFile = sourceFile;

            var watermarkSet = this.GetWatermarkSet(sourceFile);
            if(!watermarkSet.OverrideInputFiles) {
                var folder = this.fileFactory.CreateDirectory(Path.Combine(sourceFile.Directory.FullName, "[Logo]"));
                if(!folder.Exists) {
                    folder.Create();
                }

                outputFile = this.fileFactory.CreateFile(Path.Combine(folder.FullName, sourceFile.Name));
            }

            if(watermarkSet.ImageWatermarks != null) {
                this.imageWatermarkingService.ApplyTo(sourceFile, outputFile, watermarkSet.ImageWatermarks.Select(this.MapToImageWatermark));
            }

            if(watermarkSet.TextWatermarks != null) {
                this.textWatermarkingService.ApplyTo(outputFile, outputFile, watermarkSet.TextWatermarks.Select(this.MapToTextWatermark));
            }
        }
    }


    private WatermarkSet GetWatermarkSet(IFile file) {
        IEnumerable<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata(file.FullName);
        var iptcDirectory = directories.OfType<IptcDirectory>().FirstOrDefault();
        var keywords = iptcDirectory?.GetDescription(IptcDirectory.TagKeywords)?.Split(';');

        foreach(var keyword in keywords) {
            var result = this.settings.Sets.FirstOrDefault(s => s.Name == keyword);
            if(result != null) {
                return result;
            }
        }

        return this.settings.DefaultSet;
    }


    private WatermarkRequest<IFile> MapToImageWatermark(Watermark w) {
        return new WatermarkRequest<IFile> {
            Mark = this.fileFactory.CreateFile(w.Mark),
            Scale = w.Scale,
            Anchor = w.Anchor,
            Flow = w.Flow,
            OffsetX = w.OffsetX,
            OffsetY = w.OffsetY
        };
    }


    private WatermarkRequest<string> MapToTextWatermark(Watermark w) {
        return new WatermarkRequest<string> {
            Mark = w.Mark,
            Scale = w.Scale,
            Anchor = w.Anchor,
            Flow = w.Flow,
            OffsetX = w.OffsetX,
            OffsetY = w.OffsetY
        };
    }
}