# Fotografix

Fotografix is a modern photo editor for Windows with a non-destructive editing workflow
and a minimalist user interface. It is written in C# and built on the Universal Windows
Platform. It uses [Win2D](https://github.com/Microsoft/Win2D) for hardware-accelerated
rendering.

## How to get Fotografix

The latest release can be [installed from the Microsoft Store](https://www.microsoft.com/store/apps/9NBZQK3WVN38?cid=readme).
Older releases are available in source form on the [GitHub releases page](https://github.com/lmadhavan/fotografix/releases).

## How to build Fotografix

**Prerequisites**
* Visual Studio Community 2019 with UWP and desktop C++ workloads
* Windows 10 SDK build 19041

**Building the source code**
* Open `Fotografix.sln` in Visual Studio.
* Set the solution configuration to `Debug` and platform to `x64`.
* Build the solution.

The initial build will automatically pull down additional dependencies from NuGet. Once
the build completes, you can deploy and launch the application locally.