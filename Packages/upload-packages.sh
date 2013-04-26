#!/bin/bash
set -e

PKG_WIN=OPDtab-windows-$(git describe).zip
PKG_LIN=OPDtab-linux-$(git describe).tar.gz

cp OPDtab.zip $PKG_WIN
cp OPDtab.tar.gz $PKG_LIN

echo "Uploading $PKG_WIN"
./googlecode_upload.py -s "Windows binary package, rev $(git rev-parse master)" \
    -p opdtab -l Type-Archive,OpSys-Windows $PKG_WIN

echo "Uploading $PKG_LIN"
./googlecode_upload.py -s "Linux binary package, rev $(git rev-parse master)" \
    -p opdtab -l Type-Archive,OpSys-Linux $PKG_LIN

rm OPDtab.zip OPDtab.tar.gz
