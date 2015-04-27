#!/bin/bash

# (c) 2015 Jannis Limperg
# This software is licensed under the terms of the GNU General Public License
# version 3, a copy of which can be found in the LICENSE file.

# Usage: Generate binary packages by building the Packages project in
# Monodevelop, then execute this script in the Git top directory. Packages for
# the current version will be generated in Packages/. Make sure that the commit
# you have currently checked out corresponds to the version you want to
# generate.
#
# The version number is automatically determined by looking at the
# current commit's annotated tag. The tag must be "va.b.c", where
# a.b.c is the version number.

VERSION_PATTERN='^v([0-9]+.[0-9]+.[0-9]+)$'
PACKAGES_PROJECT_FILE='Packages.mdproj'
PACKAGEDIR='Packages'
LINUX_PREFIX='opdtab_bin_linux-x86'
LINUX_PACKAGE="${LINUX_PREFIX}.tar"
WINDOWS_PREFIX='opdtab_bin_windows-x86'
WINDOWS_PACKAGE="${WINDOWS_PREFIX}.zip"
LAUNCHSCRIPT='opdtab'

function die() {
  echo $1
  exit 1
}

# Get version string from Git. As a safety measure, we only allow a release
# to be created if the commit currently checked out is tagged with an
# annotated tag.

describe_string=$(git describe --exact-match)

echo "$describe_string" | grep -Eq "$VERSION_PATTERN"
if [ $? -ne 0 ]; then
  die "Could not detect version. Do you have a tagged commit checked out?"
fi

version_string=$(echo "$describe_string" | sed -re "s/${VERSION_PATTERN}/\\1/")

# Generate the binary packages.

mdtool build "${PACKAGES_PROJECT_FILE}" \
  || die "Build failure. See previous output for detailed information."

# Add launch script to linux package and compress.

tar -rC "${PACKAGEDIR}" -f "${PACKAGEDIR}/${LINUX_PACKAGE}" "${LAUNCHSCRIPT}" \
  --transform "s,${LAUNCHSCRIPT},${LINUX_PREFIX}/${LAUNCHSCRIPT}," \
  || die "Unable to add launchscript to linux tarball."
bzip2 -9 "${PACKAGEDIR}/${LINUX_PACKAGE}" \
  || die "Unable to compress linux package using bzip2."

# Name tarballs properly.

mv "${PACKAGEDIR}/${LINUX_PACKAGE}.bz2" \
   "${PACKAGEDIR}/${LINUX_PREFIX}_${version_string}.tar.bz2" \
   || die "Unable to rename linux package."

mv "${PACKAGEDIR}/${WINDOWS_PACKAGE}" \
   "${PACKAGEDIR}/${WINDOWS_PREFIX}_${version_string}.zip" \
   || die "Unable to rename windows package."
