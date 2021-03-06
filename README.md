![Icon](https://i.imgur.com/tiDW0wD.png?1)
# DependencyTracker 
[![Build status](https://ci.appveyor.com/api/projects/status/2pnf07tt5u29f00i?svg=true)](https://ci.appveyor.com/project/lvermeulen/dependencytracker) [![license](https://img.shields.io/github/license/lvermeulen/DependencyTracker.svg?maxAge=2592000)](https://github.com/lvermeulen/DependencyTracker/blob/master/LICENSE) [![NuGet](https://img.shields.io/nuget/vpre/DependencyTracker.Core.svg?maxAge=2592000)](https://www.nuget.org/packages/DependencyTracker.Core/) 
 ![](https://img.shields.io/badge/.net-4.5.2-yellowgreen.svg) ![](https://img.shields.io/badge/netstandard-1.4-yellowgreen.svg)

DependencyTracker is a library for tracking NuGet and npm dependencies between projects and tells you which project uses which other project.

## Features
* Load dependencies by location or git repo.
* Read dependencies from **NuGet** packages.config, **npm** package.json, **LibMan** libman.json and Gemfile files
* Write dependencies to a database or to a Graphviz .dot file
* Extensible: write your own loader, reader and writer

## Usage

```C#
using (var loader = new LocationLoader(@"C:\MyProjects"))
{
    // read
    var reader = new NuGetReader(loader.Location);
    var dependencies = reader.Read();

    // write
    var writer = new MssqlWriter(myConnectionString);
    writer.Write(dependencies);

    Console.WriteLine($"{reader.Count} files read");
}

```

## Thanks
* [Dependency](https://thenounproject.com/term/dependency/1340837) icon by [Knut M. Synstad](https://thenounproject.com/knutsynstad) from [The Noun Project](https://thenounproject.com)
