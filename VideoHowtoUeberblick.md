# Überblick zum Video Howto #

Von dem Autor des Programms wurden einige Screencasts gemacht, um die Bedienung und das Drumherum des OPDtab Programms zu erläutern. Diese sind nicht ganz ausgegoren was Sprache und Didaktik angeht. Daher sollte man immer versuchen, die erwähnten Schritte an einer eigenen Installation nachzuvollziehen. Die Beispieldaten aus den Videos kann man erfragen unter andreas.neiser@gmail.com.

Anregungen und Kommentare sind immer erwünscht. Diese werden dann eingearbeitet für nachfolgende Tabmaster.

Hier die einzelnen Bereiche des Video Howto (am besten in HD und Vollbild schauen). Man findet sie auch links in der Sidebar, die Reihenfolge sollte beim ersten Schauen eingehalten werden.
  1. [Vorbereitungen](VideoHowto01Vorbereitungen.md)
  1. [Datenverwaltung](VideoHowto02Datenverwaltung.md)
  1. [Rundenerstellung](VideoHowto03Rundenerstellung.md)
  1. [Ergebniseingabe](VideoHowto04Ergebniseingabe.md)
  1. [Breakrunden](VideoHowto05Breakrunden.md)

## Änderungen seit [r60](https://code.google.com/p/opdtab/source/detail?r=60) (Stand [r151](https://code.google.com/p/opdtab/source/detail?r=151), 2012-10-21) ##

Das Tutorial an sich ist mit Revision [r60](https://code.google.com/p/opdtab/source/detail?r=60) erstellt worden. Die Änderungen seit dem sind aber lediglich Erweiterungen (sowie kleinere Bugfixes). Hier eine unvollständige Aufzählung:
  * Export der Debaters und des Rankings als CSV (nicht nur als PDF)
  * Das Setzen der Juroren wird nun durch einen CompactView in GenerateRound und durch das Vorgeben einer Position (Chair, FirstJudge, OtherJudge, NotAvailable) besser unterstützt. Die Vorgabe wird durch den entsprechenden Algorithmus unterstützt.
  * Das Setzen der freien Redner kann wahlweise nun nicht zwangsweise zyklisch geschehen, sondern etwas zufälliger. Dies kann aber dazu führen, dass zwei Leute aus dem selben Team im gleichen Raum reden (man kann durch Setzen eines höheren Conflict-Wertes FreeVsFree dagegen wirken). Aber vielleicht sind so bessere Vorrunden-Setzungen möglich?
  * Die durchschnittliche Punktzahl pro Runde und Position wird angezeigt in ShowRanking
  * Layout Verbesserungen in GenerateRound:
    * Das Fenster für die Thema-Eingabe wird automatisch etwas größer
    * Die Toolbox lässt sich verstecken, nützlich vor allem im CompactView
    * Die Buttons für die PDF Erstellung sind besser strukturiert.
    * Die Raumnamen und Beschreibungen kann man nun komfortabler eingeben.
  * Im Ranking werden die Positionen der Redner als hochgestellte Zahlen/Buchstaben angezeigt.
  * Die Blacklist/Whitelist Einträge können nun durch einen Editor einfacher bearbeitet werden.