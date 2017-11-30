![Icon](https://i.imgur.com/tiDW0wD.png?1)
# DependencyTracker 
[![Build status](https://ci.appveyor.com/api/projects/status/hwk6g88wm7orvcog?svg=true)](https://ci.appveyor.com/project/lvermeulen/dependencytracker) [![license](https://img.shields.io/github/license/lvermeulen/DependencyTracker.svg?maxAge=2592000)](https://github.com/lvermeulen/DependencyTracker/blob/master/LICENSE) [![NuGet](https://img.shields.io/nuget/vpre/DependencyTracker.svg?maxAge=2592000)](https://www.nuget.org/packages/DependencyTracker/) [![Coverage Status](https://coveralls.io/repos/github/lvermeulen/DependencyTracker/badge.svg?branch=master)](https://coveralls.io/github/lvermeulen/DependencyTracker?branch=master) [![codecov](https://codecov.io/gh/lvermeulen/DependencyTracker/branch/master/graph/badge.svg)](https://codecov.io/gh/lvermeulen/DependencyTracker)
 ![](https://img.shields.io/badge/.net-4.5.1-yellowgreen.svg) ![](https://img.shields.io/badge/netstandard-1.4-yellowgreen.svg)
DependencyTracker is a library for tracking dependencies between projects. Which project uses which other project (e.g. via NuGet packages)? Which project is used by which other project?

## Features:
* Load dependencies by location or git repo.
* Read dependencies from NuGet **packages.config** files
* Write dependencies to a database

## Usage:

~~~~
// load
var loader = new LocationLoader(somePath);
string path = loader.Load();

// read
var reader = new NuGetReader(path);
var dependencies = reader.Read();

// write
var writer = new MssqlWriter(someConnectionString);
Write(writer, dependencies);
~~~~


## Thanks
* [Dependency](https://thenounproject.com/term/dependency/1340837) icon by [Knut M. Synstad](https://thenounproject.com/knutsynstad) from [The Noun Project](https://thenounproject.com)
