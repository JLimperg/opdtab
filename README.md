# OPDtab

OPDtab is the premier (and, as far as I'm aware, only) tabbing software for
debating competitions in the format Offene Parlamentarische Debatte (OPD), used
primarily in German-speaking countries. This document contains technical
documentation only; for a user's guide see
[https://code.google.com/p/opdtab/w/list](https://code.google.com/p/opdtab/w/list).

## Installation

1. Obtain a binary archive from
   [https://github.com/JLimperg/opdtab/releases](https://github.com/JLimperg/opdtab/releases)
   or build it yourself, as described below.
2. Unpack the archive in a directory of your choice.
3. On Linux, install the Mono C# runtime if necessary. The corresponding
   package is usually called `mono` and comes preinstalled with most major
   distributions.
4. On Windows, execute `OPDtab.exe`. On Linux, execute `opdtab`.

## Building

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
tracker](https://github.com/neiser/opdtab/issues) for bug reports or
questions. Contributions of any nature are always most welcome and may be
submitted in any form you find convenient.
