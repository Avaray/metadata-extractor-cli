# 🧬 Image Metadata Extractor CLI

[![Build and Release](https://github.com/Avaray/metadata-extractor-cli/actions/workflows/build-and-release.yml/badge.svg)](https://github.com/Avaray/metadata-extractor-cli/actions/workflows/build-and-release.yml)

A command-line interface (CLI) application that extracts metadata from image files.  
This tool is designed for developers who need to quickly access metadata from images in various formats.

## Features

- Extracts metadata from single image or all images in a specified directory (also recursively).
- Outputs the extracted metadata to console as [JSON](https://en.wikipedia.org/wiki/JSON) or saves it to a JSON file.
- Supports [various image formats](https://github.com/drewnoakes/metadata-extractor-dotnet?tab=readme-ov-file#features).

## Technologies and Libraries Used

- **Programming Language**: [C#](https://dotnet.microsoft.com/en-us/languages/csharp)
- **Framework**: [.NET 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- **Metadata Extraction Library**: [MetadataExtractor](https://github.com/drewnoakes/metadata-extractor-dotnet)
- **JSON Serialization**: [System.Text.Json](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/how-to) (part of .NET)

## Usage

1. Download the latest release from the [Releases](https://github.com/Avaray/metadata-extractor-cli/releases) page.  
   You need to have [.NET 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) installed on your machine if you want to use `no-runtime` version.
2. Extract executable file from the downloaded archive.
3. Navigate to the directory where you extracted the files.
4. Run the following command to get list of available commands:

```
extractor -h
```

## Available Commands

- **Single Image**: Extract metadata from a single image file.

```
extractor <file-path>
```

- **Directory**: Extract metadata from all images in a specified directory.

```
extractor -d <directory-path>
```

- **Directory and Subdirectories**: Extract metadata from all images recursively in a specified directory.

```
extractor -d <directory-path> -r
```

- **Output to File**: Save the extracted metadata to a JSON file.

```
extractor <file-path> -o <output-file>
```

### Experimental Options

> These options are experimental and might not work as expected. Pretty sure they will be changed or removed in future versions. Combining them together might not work.

- **Filter Tag Name**: Return metadata with specific tag name. Ignore all other tags.

```
extractor -d <directory-path> -t <tag-name>
```

- **Search in Tag Description**: Filter images by searching in tag description. Using [Regex](https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference) is possible, but might not work as expected. Something like `-s '[\d\w]{10}'` should work.

```
extractor -d <directory-path> -s <search-string>
```

## Development

1. **Prerequisites**: Ensure you have the [.NET SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) (version 8.0 or higher) installed on your machine. You can download it from the official [.NET website](https://dotnet.microsoft.com/download). You can check your current version by running the following command:

```bash
dotnet --version
```

2. **Clone the Repository**:

```bash
git clone https://github.com/Avaray/image-metadata-extractor-cli.git
cd image-metadata-extractor-cli
```

3. **Run the Application**:

```bash
dotnet run
```

4. **Build the Application**:

```bash
dotnet build
```

## About this Project

I created this project because I needed a tool like this. I will use it in my other project where I need to extract metadata from images. Normally I don't code in C# but with help of [Claude](https://claude.ai/) and [Perplexity](https://www.perplexity.ai/) it was pretty easy 👌

Creating a working CLI took me a little over an hour. However, messing around with configuring GitHub Actions for Releases drove me crazy. But finally, I got it working 🎉

I might add more features in the future. I'm considering following changes:

- [x] Extract recursive metadata extraction `-r` option.
- [x] Add checks for invalid file types. To print message rather than throwing an exception.
- [ ] Lower the required .NET version.
- [x] Possibility to filter images by "Tag Name".
- [x] Possibility to filter images with [Regex](https://en.wikipedia.org/wiki/Regular_expression).
