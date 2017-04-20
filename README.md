# OPDtab

OPDtab is the premier (and, as far as I'm aware, only) tabbing software for
debating competitions in the format Offene Parlamentarische Debatte (OPD), used
primarily in German-speaking countries.

## User Documentation

This document contains technical documentation only. A user's guide (in German)
consisting mostly of video tutorials is available at the project's old [Google
Code website](https://code.google.com/archive/p/opdtab/wikis). Beware that the
guide is for an older version, so some details will differ; however, most of
the content is still relevant to current OPDtab.

## Installation

1. Download a [binary archive](https://github.com/JLimperg/opdtab/releases)
   or build OPDtab yourself, as described below.
2. Unpack the archive in a directory of your choice.
3. Install the dependencies listed below.
4. On Windows, execute `OPDtab.exe`. On Linux, execute `opdtab`.

### Dependencies

1. .NET Framework 4.0/Mono
   1. Linux: Usually a package named 'mono' or something similar. Usually
      preinstalled. Should also be pulled in as a dependency of gtk-sharp
      below. [Direct
      download](http://www.mono-project.com/download/#download-lin) available
      from the Mono project
   2. Windows:
      [Installer](http://www.microsoft.com/en-us/download/details.aspx?id=17851).
      Usually preinstalled.
2. [Gtk# for .NET](http://www.mono-project.com/docs/gui/gtksharp/), version 2
   1. Linux: Usually a package named 'gtk-sharp' or similar. [Direct
      download](http://www.mono-project.com/download/#download-lin) available
      from the Mono project.
   2. Windows: [Installer](http://www.mono-project.com/download/#download-win).
3. LaTeX
   1. Linux: [TeXLive](https://www.tug.org/texlive/), should be provided by
      your distribution's package manager. The TeXLive distribution is usually
      split into multiple packages; install them all to make sure that the PDF
      export works.
   2. Windows: [MiKTeX](http://miktex.org/). Either choose the full
      installation or make sure that MiKTeX can download missing packages as
      required.

## Building

Before anything else, install the dependencies listed above. If you are
on Linux and your distribution provides separate 'developer' packages,
install those for gtk-sharp.

### Linux

Building requires the Mono IDE Monodevelop or its command line frontend
`mdtool`. From the command line, use
```
mdtool build OPDtab.sln
```
to build the project and
```
mdtool build Packages.mdproj
```
to generate binary archives in the `Packages` directory.

To generate release binary packages, use the `mkrelease.sh` script. See
the usage notes at the top of that file for details.

### Other Operating Systems

Unfortunately, I'm not familiar with the C# toolchains on other OSs. Help in
extending this documentation would be much appreciated.

## License

This software is made available under the terms and conditions of the General
Public License version 3, the full text of which can be found in the LICENSE
file.

## Contact

OPDtab was written primarily by Andreas Neiser and is currently maintained by
Jannis Limperg. Please use the [issue
tracker](https://github.com/JLimperg/opdtab/issues) for bug reports or
questions. Contributions of any nature are always most welcome and may be
submitted in any form you find convenient.
