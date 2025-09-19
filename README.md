# ![AppIcon](https://github.com/M1S2/Vereinsmeisterschaften/raw/master/Icons/AppIcon.png) Vereinsmeisterschaften

[![GitHub Release Version](https://img.shields.io/github/v/release/M1S2/Vereinsmeisterschaften)](https://github.com/M1S2/Vereinsmeisterschaften/releases/latest)
[![GitHub License](https://img.shields.io/github/license/M1S2/Vereinsmeisterschaften)](https://github.com/M1S2/Vereinsmeisterschaften/blob/master/LICENSE.md)


Programm für die Vereinsmeisterschaften der Schwimmabteilung des TSV Illertissen.

## Arbeitsbereich
Der Arbeitsbereich ist ein Ordner, in dem alle Einstellungen, Wettkämpfe und Meldungen gespeichert werden. Hier bietet es sich an, für jeden Wettkampf (z.B. jedes Jahr) einen neuen Ordner anzulegen. Es ist egal, wo sich der Ordner auf der Festplatte befindet.

Folgende Dateien werden erzeugt:
- WorkspaceSettings.json: JSON Datei mit allen Einstellungen für den Arbeitsbereich.
- Person.csv: Liste mit allen Meldungen und den erschwommenen Zeiten.
- Competitions.csv: Liste mit allen verfügbaren Wettkämpfen.
- BestRace.csv: Das Rennen, das als gute Variante markiert wurde, wird hier gespeichert. Wenn kein Rennen markiert wurde, existiert diese Datei nicht.

Es können hier natürlich weitere Dateien abgelegt werden (z.B. Templates).

## Seiten Beschreibung
Nachfolgend werden kurz die Funktionen der jeweiligen Seite der Anwendung beschrieben. Die Reihenfolge der Seiten spiegelt ungefähr den gesamten Workflow wieder (von der Eingabe der Meldungen, über die Eingabe der Zeiten während des Wettkampfs bis zur Auswertung).

### Start
Startseite, von der auf alle anderen Bereiche zugegriffen werden kann. Gleichwertig mit dem Menü am linken Rand.

<img src="https://github.com/M1S2/Vereinsmeisterschaften/raw/master/Doc/Screenshots/Screenshot_Main.png" width="100%"></img>

### Arbeitsbereich
Hier kann der aktuelle Ordner des Arbeitsbereichs ausgewählt und alle Änderungen gespeichert werden.
Außerdem können alle Einstellungen für verschiedene Aspekte der Software hier getroffen und auch wieder zurückgesetzt werden.

<img src="https://github.com/M1S2/Vereinsmeisterschaften/raw/master/Doc/Screenshots/Screenshot_Workspace.png" width="100%"></img>

### Wettkämpfe
Hier werden alle verfügbaren Wettkämpfe definiert. Ein Wettkampf wird beschrieben über die Lage, die Strecke, das Alter und eine entsprechende Zeit.
Die Zeit wird dann für die Berechnung der Punkte her genommen. Wird exakt die hier eingetragene Zeit erschwommen, gibt es 100 Punkte. Wenn langsamer gibt es weniger Punkte, wenn schneller dann mehr.
Für jeden Start einer Person muss auch ein entsprechender Wettkampf verfügbar sein.

<img src="https://github.com/M1S2/Vereinsmeisterschaften/raw/master/Doc/Screenshots/Screenshot_Competitions.png" width="100%"></img>

### Meldungen
Hier können alle Meldungen eingetragen werden. Für die Meldung ist der Name, der Jahrgang und die gewünschten Lagen nötig. Für jede Lage muss auch ein entsprechender Wettkampf hinterlegt sein! Sonst wird der Start ignoriert (wird hier auch farblich markiert).

<img src="https://github.com/M1S2/Vereinsmeisterschaften/raw/master/Doc/Screenshots/Screenshot_People.png" width="100%"></img>

### Rennen vorbereiten
Hier kann eine Reihenfolge für die Starts erstellt werden. Entweder manuell per Hand (Variante hinzufügen und die Starts per Hand vom rechten Bereich in die Mitte ziehen) oder automatisch (es werden zufällig Varianten erstellt und nach verschiedenen Kriterien bewertet).

Jede Variante eines Rennens wird von 0% (schlecht) bis 100% (perfekt) bewertet. Im Arbeitsbereich kann die Gewichtung oder verschiedene Parameter für die automatische Berechnung eingestellt werden.

Es gibt auch die Möglichkeit, bestimmte Starts hervorzuheben. Z.B. alle Starts einer Person um zu schauen, ob genügend Pausen vorhanden sind.

<img src="https://github.com/M1S2/Vereinsmeisterschaften/raw/master/Doc/Screenshots/Screenshot_Races.png" width="100%"></img>

### Zeiten eingeben
Für jeden Start kann hier die erschwommene Zeit eingetragen werden (in Minute, Sekunden und Zehntelsekunden). Es stehen verschiedene Filtermöglichkeiten zur Verfügung.

<img src="https://github.com/M1S2/Vereinsmeisterschaften/raw/master/Doc/Screenshots/Screenshot_TimeInput.png" width="100%"></img>

### Ergebnisse
Hier wird die Auswertung aus allen Starts angezeigt (wie viele Punkte die jeweils erschwommenen Zeiten ergeben). Es ist möglich, auch die besten Schwimmer in einer bestimmten Lage zu ermitteln.

<img src="https://github.com/M1S2/Vereinsmeisterschaften/raw/master/Doc/Screenshots/Screenshot_Results.png" width="100%"></img>

### Dokumente erstellen
Hier können verschiedene Dokumente erstellt werden, die vor, während oder nach dem Wettkampf nötig sind. Der Filter gilt nur für die Urkunden-Erstellung.

Außerdem ist eine Übersicht über alle verfügbaren Platzhalter vorhanden, die in den Dokumenten verwendet werden können.

<img src="https://github.com/M1S2/Vereinsmeisterschaften/raw/master/Doc/Screenshots/Screenshot_Documents.png" width="100%"></img>

### Einstellungen
Hier kann das Software Design verändert werden. Hell, Dunkel oder Systemeinstellung stehen zur Verfügung.

<img src="https://github.com/M1S2/Vereinsmeisterschaften/raw/master/Doc/Screenshots/Screenshot_Settings.png" width="100%"></img>

## Punktewertung
Die Punkte eines Starts werden folgendermaßen berechnet:
- Folgende Formel wird verwendet: `Punkte = (2 - (ErschwommeneZeit / WettkampfVorgabeZeit)) * 100;`
- Wird exakt die Zeit des hinterlegten Wettkampfs erschwommen (ErschwommeneZeit == WettkampfVorgabeZeit), gibt es 100 Punkte.
- Wenn langsamer dann gibt es weniger Punkte.
- Wenn schneller dann gibt es mehr Punkte.
- Null Punkte, sobald `ErschwommeneZeit >= 2 * WettkampfVorgabeZeit`

Der Start mit der höchsten Punktezahl wird als "Bestes Rennen" gewertet und für die Gesamtwertung herangezogen. Es wird kein Durchschnitt gebildet.

## Entwickler Dokumentation
Detailliertere Entwickler Dokumentation: https://m1s2.github.io/Vereinsmeisterschaften

## Icons
- SwimmingStyles.png: https://www.vecteezy.com/vector-art/349116-swimming-aqua-aquatic-sport-game-icon-symbol-sign-pictogram
- icons8-child-64.png: https://icons8.com/icon/67246/babys-room
- DrawingImage_Female: https://pictogrammers.com/library/mdi/icon/gender-female/
- DrawingImage_Male: https://pictogrammers.com/library/mdi/icon/gender-male/
- DrawingImage_DecimalComma: https://pictogrammers.com/library/mdi/icon/decimal-comma/
- DrawingImage_Counter: https://pictogrammers.com/library/mdi/icon/counter/
- DrawingImage_Weight: https://pictogrammers.com/library/mdi/icon/weight/
- DrawingImage_Priority: https://pictogrammers.com/library/mdi/icon/priority-high/
- DrawingImage_Trophy: https://pictogrammers.com/library/mdi/icon/trophy-outline/_

https://retouchinglabs.com/inkscape-convert-png-to-svg/
https://stackoverflow.com/questions/18624070/convert-svg-to-xaml