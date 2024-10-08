﻿using System.Text.Json;
using System.Reflection;
using MetadataExtractor;
using System.Text.RegularExpressions;

class Program
{
    private static readonly JsonSerializerOptions jsonSerializerOptions = new() { WriteIndented = true };

    public class ImageMetadata
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public List<MetadataTag> Tags { get; set; } = [];
    }

    public class MetadataTag
    {
        public string DirectoryName { get; set; } = string.Empty;
        public string TagName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    static async Task Main(string[] args)
    {
        if (args.Length == 0 || args.Contains("-h") || args.Contains("--help"))
        {
            DisplayHelp();
            return;
        }

        if (args.Contains("-v") || args.Contains("--version"))
        {
            var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
            var semVer = assemblyVersion!.ToString(3);
            Console.WriteLine($"Extractor v{semVer}");
            return;
        }

        string? outputPath = null;
        bool isDirectory = false;
        bool includeSubdirectories = false;
        string filePath = args[0];
        string? filterString = null;
        string? regexPattern = null;

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-d")
            {
                isDirectory = true;
                if (i + 1 < args.Length)
                {
                    filePath = args[++i];
                }
                else
                {
                    Console.WriteLine("Error: -d option requires a directory path.");
                    return;
                }
            }
            else if (args[i] == "-o")
            {
                if (i + 1 < args.Length)
                {
                    outputPath = args[++i];
                }
                else
                {
                    Console.WriteLine("Error: -o option requires an output file path.");
                    return;
                }
            }
            else if (args[i] == "-r")
            {
                includeSubdirectories = true;
            }
            else if (args[i] == "-t")
            {
                if (i + 1 < args.Length)
                {
                    filterString = args[++i];
                }
                else
                {
                    Console.WriteLine("Error: -t option requires a filter string.");
                    return;
                }
            }
            else if (args[i] == "-s")
            {
                if (i + 1 < args.Length)
                {
                    regexPattern = args[++i];
                }
                else
                {
                    Console.WriteLine("Error: -s option requires a search string.");
                    return;
                }
            }
            else if (i == 0)
            {
                // If option is not provided, assume it is a file path
                filePath = args[i];
            }
        }

        List<ImageMetadata> allMetadata = [];

        if (isDirectory)
        {
            filePath = System.IO.Path.GetFullPath(filePath);
            if (!System.IO.Directory.Exists(filePath))
            {
                Console.WriteLine($"Directory does not exist: {filePath}");
                return;
            }

            var searchOption = includeSubdirectories ? System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly;
            var imageFiles = System.IO.Directory.GetFiles(filePath, "*.*", searchOption)
                .Where(file => IsValidFile(file))
                .ToArray();

            var tasks = imageFiles.Select(imagePath => Task.Run(() => ExtractMetadata(imagePath, filterString, regexPattern)));
            var filteredMetadata = await Task.WhenAll(tasks);
            allMetadata.AddRange(filteredMetadata.Where(metadata => metadata != null)!);
        }
        else
        {
            if (!System.IO.File.Exists(filePath))
            {
                Console.WriteLine($"File does not exist: {filePath}");
                return;
            }

            if (IsValidFile(filePath))
            {
                var metadata = ExtractMetadata(filePath, filterString, regexPattern);
                if (metadata != null)
                {
                    allMetadata.Add(metadata);
                }
            }
            else
            {
                Console.WriteLine($"Invalid file type: {filePath}");
                return;
            }
        }

        string jsonOutput = JsonSerializer.Serialize(allMetadata, jsonSerializerOptions);

        if (outputPath != null)
        {
            await File.WriteAllTextAsync(outputPath, jsonOutput);
            Console.WriteLine($"Metadata saved to {outputPath}");
        }
        else
        {
            Console.WriteLine(jsonOutput);
        }
    }

    private static bool IsValidFile(string filePath)
    {
        string extension = System.IO.Path.GetExtension(filePath).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" or ".tif" or ".tiff" or ".webp" or
            ".png" or ".bmp" or ".gif" or ".ico" or ".pcx" or
            ".heif" or ".heic" or ".avif" or ".psd" or
            ".nef" or ".cr2" or ".orf" or ".arw" or
            ".rw2" or ".rwl" or ".srw" or
            ".wav" or ".avi" or ".mov" or ".mp4" => true,
            _ => false,
        };
    }

    private static ImageMetadata? ExtractMetadata(string imagePath, string? filterString = null, string? regexPattern = null)
    {
        try
        {
            var directories = ImageMetadataReader.ReadMetadata(imagePath);
            var metadataTags = new List<MetadataTag>();

            foreach (var directory in directories)
            {
                foreach (var tag in directory.Tags)
                {
                    // Filter by TagName
                    if (filterString != null && !tag.Name.ToLowerInvariant().Contains(filterString.ToLowerInvariant()))
                        continue;

                    // Filter by regex in Description
                    if (regexPattern != null)
                    {
                        var regex = new Regex(regexPattern);
                        if (!regex.IsMatch(tag.Description ?? string.Empty))
                            continue;
                    }

                    metadataTags.Add(new MetadataTag
                    {
                        DirectoryName = directory.Name,
                        TagName = tag.Name,
                        Description = tag.Description ?? string.Empty
                    });
                }
            }

            // Only return ImageMetadata if there are tags matching the filters
            if (metadataTags.Count == 0)
            {
                return null;
            }

            return new ImageMetadata
            {
                FilePath = imagePath,
                FileName = System.IO.Path.GetFileName(imagePath),
                Tags = metadataTags
            };
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static void DisplayHelp()
    {
        Console.WriteLine("Usage: extractor <file-path> [-d <directory-path>] [-o <output-file>] [-r]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  <file-path>           Path to a single image file.");
        Console.WriteLine("  -d <directory-path>   Path to a directory containing image files.");
        Console.WriteLine("  -o <output-file>      Path to the output file for saving the extracted metadata.");
        Console.WriteLine("  -r                    Include subdirectories when processing a directory.");
        Console.WriteLine("  -t <filter-string>    Filter metadata by TagName.");
        Console.WriteLine("  -s <search-string>    Filter metadata by Description using string search.");
        Console.WriteLine("  -v, --version         Display version.");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  extractor image.jpg");
        Console.WriteLine("  extractor image.jpg -o output.json");
        Console.WriteLine("  extractor -d images_directory");
        Console.WriteLine("  extractor -d images_directory -o output.json");
        Console.WriteLine("  extractor -d images_directory -r");
        Console.WriteLine("  extractor -d images_directory -t \"textual tata\"");
        Console.WriteLine("  extractor -d images_directory -s \"word\"");
    }
}
