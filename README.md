# Catharsium.Images

Console tool that watermarks Lightroom exports and sorts them into dated Public/Private × Color/B&W folders.

## Usage

- **With arguments:** pass image files and/or folders. Folders contribute their top-level files.
  ```
  dotnet run --project Catharsium.Images.ConsoleApp -- "/path/to/export"
  ```
- **Without arguments:** the menu opens. *Watermark → Add* asks for a folder; press Enter to use the configured `InputFolder`.

## Configuration

`Catharsium.Images.ConsoleApp/appsettings.json` holds the watermark sets. Their `Image` paths are relative to `LogoFolder`; absolute paths also work.

Machine-specific paths go in `Catharsium.Images.ConsoleApp/appsettings.local.json`, which git ignores and which overrides `appsettings.json`. Copy `appsettings.local.example.json` to start:

| Setting | Section | Meaning |
|---|---|---|
| `InputFolder` | `Catharsium.Images.ConsoleApp` | Default folder for *Watermark → Add* |
| `LogoFolder` | `Catharsium.Images.Watermarking` | Base folder for watermark images |
