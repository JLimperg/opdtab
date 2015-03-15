Im Voraus: Diese Anleitung ist weder pädagogisch noch vollständig. Daher nutze man die Kommentarfunktion für Fragen. Das wird dann hier eingebaut. Man beachte auch TipsTricks.

# Voraussetzungen #

## Windows ##

  * Prinzipiell sollte man diese Dateien herunterladen & installieren:
    1. [.Net Framework 4.0](http://www.microsoft.com/download/en/details.aspx?id=17851) (oft schon installiert)
    1. [Gtk# for .Net](http://download.mono-project.com/gtk-sharp/gtk-sharp-2.12.10.win32.msi)
  * Ein Neustart ist manchmal notwendig, damit ein Doppelklick auf OPDtab.exe das Programm auch startet
  * Falls man die vielfältigen **PDF Exporte** nutzen möchte (das ist ratsam), sollte man eine vollständige **Miktex Installation** durchführen, siehe hier: http://miktex.org/ (man kann auch eine "normale" Installation ausführen, aber dann sollte man Internet zur Verfügung haben, um eventuell fehlende Pakete nachladen zu können beim eigentlichen Erstellen der PDFs. Außerdem sollte man dann auswählen, dass **Pakete automatisch installiert werden** ohne dass eine Nachfrage stattfindet.)

## Linux ##
  * Man muss mono und gtk-sharp (Version 2) installieren (oft gibt es ein entsprechendes Paket). Meistens sind diese Pakete aber schon installiert.
  * Eine aktuelle TexLive Installation sollte installiert sein, damit man PDF Exporte nutzen kann. Unter Debian/Ubuntu heißt ein gutes Package texlive-full, das installiert einfach alles. Sollte die PDF Erstellung später nicht funktionieren, liegt es höchstwahrscheinlich an einer nicht kompletten oder nicht aktuellen Tex Installation.


# Installation #
  * Nach dem Herunterladen und Entpacken Archivs unter Downloads findet man die Datei OPDtab.exe, ein paar DLLs und verschiedene Unterordner. Wichtig davon ist hauptsächlich "pdfs", dort landen die erzeugten PDFs wie Rundensetzung, Übersicht, Tab, Juroren-Konflikt-Matrizen, etc. Unter "data" finden sich XML-Dateien, die aktuelle ist immer "data/tournament.dat", andere sind automatische Backups.

# Start des Programms #

## Windows ##

  * Man sollte das Programm durch einen Doppelklick auf OPDtab.exe starten.
  * Es gibt die **Alternative**, das Programm mit Mono zu starten (falls es nicht richtig funktioniert. Bitte genaue Angaben was nicht funktionert an mich melden!). Dazu folgende Schritte:
    1. [Mono](http://download.mono-project.com/archive/2.10.6/windows-installer/1/mono-2.10.6-gtksharp-2.12.11-win32-1.exe) installieren
    1. OPDtab.exe mit dem Befehl "mono OPDtab.exe" in der Kommandozeile starten. Als kleines Helferlein dazu gibt es auch ein Batch-File zum [Download](http://code.google.com/p/opdtab/downloads/detail?name=StartOPDtabWithMono.bat). Dieses wurde aber nur unter Windows 7 getestet und man muss wohl den Pfad anpassen.

## Linux ##
  * Unter Linux macht man eine Shell auf und startet das Programm mit "mono OPDtab.exe", sodass auch das aktuelle Verzeichnis (pwd) dasselbe ist, in dem die eben entpackte OPDtab.exe liegt.

# Weiter geht's #
Es sollte sich ein recht kleiner Hauptbildschirm nach dem Start öffnen. Man kann dann unter VideoHowtoUeberblick erlernen, wie das Programm zu bedienen ist.