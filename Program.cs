using System.Text.Json;
using System.Reflection;
using MetadataExtractor;

class Program
{
    public class ImageMetadata
    {
        public string FileName { get; set; } = string.Empty;
        public List<MetadataTag> Tags { get; set; } = new List<MetadataTag>();
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
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            Console.WriteLine($"Extractor v{version}");
            return;
        }

        string? outputPath = null;
        bool isDirectory = false;
        string filePath = args[0];

        // Check for additional parameters
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
            else if (i == 0)
            {
                // If it's not a -d or -o option and it's the first argument, treat it as the file path
                filePath = args[i];
            }
        }

        List<ImageMetadata> allMetadata = new List<ImageMetadata>();

        if (isDirectory)
        {
            filePath = System.IO.Path.GetFullPath(filePath);
            if (!System.IO.Directory.Exists(filePath))
            {
                Console.WriteLine($"Directory does not exist: {filePath}");
                return;
            }

            var imageFiles = System.IO.Directory.GetFiles(filePath, "*.*", System.IO.SearchOption.TopDirectoryOnly)
                .Where(file => file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".webp", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".tiff", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            var tasks = imageFiles.Select(imagePath => Task.Run(() => ExtractMetadata(imagePath)));
            allMetadata.AddRange(await Task.WhenAll(tasks));
        }
        else
        {
            if (!System.IO.File.Exists(filePath))
            {
                Console.WriteLine($"File does not exist: {filePath}");
                return;
            }

            allMetadata.Add(ExtractMetadata(filePath));
        }

        string jsonOutput = JsonSerializer.Serialize(allMetadata, new JsonSerializerOptions { WriteIndented = true });

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

    private static ImageMetadata ExtractMetadata(string imagePath)
    {
        var directories = ImageMetadataReader.ReadMetadata(imagePath);
        var metadataTags = new List<MetadataTag>();

        foreach (var directory in directories)
        {
            foreach (var tag in directory.Tags)
            {
                metadataTags.Add(new MetadataTag
                {
                    DirectoryName = directory.Name,
                    TagName = tag.Name,
                    Description = tag.Description!
                });
            }
        }

        return new ImageMetadata
        {
            FileName = System.IO.Path.GetFileName(imagePath),
            Tags = metadataTags
        };
    }

    private static void DisplayHelp()
    {
        Console.WriteLine("Usage: extractor <file-path> [-d <directory-path>] [-o <output-file>]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  <file-path>           Path to a single image file.");
        Console.WriteLine("  -d <directory-path>   Path to a directory containing image files.");
        Console.WriteLine("  -o <output-file>      Path to the output file for saving the extracted metadata.");
        Console.WriteLine("  -v, --version         Display version.");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  extractor image.jpg");
        Console.WriteLine("  extractor -d images_directory");
        Console.WriteLine("  extractor image.jpg -o output.json");
        Console.WriteLine("  extractor -d images_directory -o output.json");
    }
}
