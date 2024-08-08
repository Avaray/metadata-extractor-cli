# Image Metadata Extractor CLI

[![Build and Release](https://github.com/Avaray/metadata-extractor-cli/actions/workflows/build-and-release.yml/badge.svg)](https://github.com/Avaray/metadata-extractor-cli/actions/workflows/build-and-release.yml)

A command-line interface (CLI) application that extracts metadata from image files. This tool is designed for developers who need to quickly access metadata from images in various formats.

## Features

- Extracts metadata from single image or all images in a specified directory.
- Outputs the extracted metadata to console as JSON or saves it to a JSON file.
- Supports [various image formats](https://github.com/drewnoakes/metadata-extractor-dotnet?tab=readme-ov-file#features).

## Technologies and Libraries Used

- **Programming Language**: [C#](https://dotnet.microsoft.com/en-us/languages/csharp)
- **Framework**: [.NET 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- **Metadata Extraction Library**: [MetadataExtractor](https://github.com/drewnoakes/metadata-extractor-dotnet) (2.8.1)
- **JSON Serialization**: [System.Text.Json](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/how-to) (part of .NET)

## Usage

> **RELEASES ARE NOT AVAILABLE YET**  
> **I NEED TO CREATE WORKFLOWS FOR THEM**

1. Download the latest release from the [Releases](https://github.com/Avaray/metadata-extractor-cli/releases) page.
2. Extract the contents of the ZIP file to a directory of your choice.
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

- **Directory**: Extract metadata from all images in a specified directory (not recursive).

```
extractor -d <directory-path>
```

<!-- - **Recursive Directory**: Extract metadata from all images in a specified directory and its subdirectories.

```
extractor -r <directory-path>
``` -->

- **Output to File**: Save the extracted metadata to a JSON file.

```
extractor <file-path> -o <output-file>
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

Creating a working CLI took me a little over an hour. However, messing around with configuring GitHub Actions for Releases continues as you read this.

I might add more features in the future. I'm considering adding these features:

- Extract metadata from images in a recursive manner (as default). This feature will break current JSON output format (it's array of objects now).
- Add checks for invalid file types. To print message rather than throwing an exception.
<!-- - Possibility to extract only specific tags. For example `PNG-tEXt`. -->
