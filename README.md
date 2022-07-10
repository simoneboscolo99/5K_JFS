<img align="right" width="250" src="https://github.com/simoneboscolo99/5K_JFS/blob/master/FinalLogo.png"/>

# 5K JFS

[![license](https://img.shields.io/github/license/simoneboscolo99/5K_JFS?color=red)](./LICENSE)
![release](https://img.shields.io/github/v/release/simoneboscolo99/5K_JFS?color=brightgreen)
![Top Language](https://img.shields.io/github/languages/top/simoneboscolo99/5K_JFS)
![OS](https://img.shields.io/badge/OS-Linux%20%7C%20MacOS%20%7C%20Windows-yellow)
![Shield](https://img.shields.io/badge/build-passing-brightgreen.svg)


Welcome to 5K JFS, the future of pictures. This is a simple raytracer, written in C# and developed for the course _Numerical Methods for Photorealistic Image Generation_ held by professor [Maurizio Tomasi][1] at the University of Milan, Physics Department (Academic Year 2021-2022).

The contibutors to the project are [Simone Boscolo][2], [Gabriele Crespi][3] and [Matteo Macchini][4].

## Table of Contents

- [Features](#features)
- [Requirements](#requirements)
- [Usage](#usage)
    - [Render mode](#render-mode)
    - [Demo mode](#demo-mode)
    - [Convert mode](#convert-mode)
- [Examples](#examples)
- [Gallery](#gallery)
- [Contributing](#contributing)
- [History](#history)
- [License](#license)

## Features

## Requirements

5K JFS requires [.NET 6.0.x](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) to run. the library does not work with lower versions.

[ImageSharp](https://www.nuget.org/packages/SixLabors.ImageSharp/2.1.1), [CommandLineUtils](https://www.nuget.org/packages/Microsoft.Extensions.CommandLineUtils), [NetCore.Platforms](https://www.nuget.org/packages/Microsoft.NETCore.Platforms/7.0.0-preview.3.22175.4) and [ShellProgressBar](https://www.nuget.org/packages/ShellProgressBar/). Don't worry, they will be installed automatically when the repository is downloaded

is available for Windows, Linux and MacOS.

## Usage

In order to use the library you can clone this repository through the command

    git clone git@github.com:simoneboscolo99/5K_JFS.git
    
Alternatively, you can download the latest version from the [releases page](https://github.com/simoneboscolo99/5K_JFS/releases).

Describe somewhere the syntax used in scene files

To get command-line help, simply run

    dotnet run

### Render mode

    dotnet run -- render [arguments] [options]
    
some words
    
    dotnet run -- render -?
    
    
Available shapes are:
- Spheres
- Planes
- Boxes
- Disks
- Open cylinders
- Constructive Solid Geometry (CSG): union, difference and intersection of any pair of shapes mentioned above

    

### Demo mode

    dotnet run -- demo [options]
    
some words
    
    dotnet run -- demo -?

### Convert mode 🔄

    dotnet run -- convert [options]
    
some words
    
    dotnet run -- convert -?
    
   
## Examples

## Gallery

## Contributing

## History
See the file [CHANGELOG.md](./CHANGELOG.md).

## License
The code is released under GNU General Public License. See the file [LICENSE](./LICENSE) for further informations.

[1]: https://github.com/ziotom78
[2]: https://github.com/simoneboscolo99
[3]: https://github.com/GabrieleCrespi
[4]: https://github.com/MatteoMacchini
