# ![AppIcon](https://github.com/M1S2/Vereinsmeisterschaften/raw/master/Icons/AppIcon.png) Vereinsmeisterschaften

[![GitHub Release Version](https://img.shields.io/github/v/release/M1S2/Vereinsmeisterschaften)](https://github.com/M1S2/Vereinsmeisterschaften/releases/latest)
[![GitHub License](https://img.shields.io/github/license/M1S2/Vereinsmeisterschaften)](https://github.com/M1S2/Vereinsmeisterschaften/blob/master/LICENSE.md)


Programm für die Vereinsmeisterschaften der Schwimmabteilung des TSV Illertissen. Folgende Funktionen sind enthalten:
- Verwaltung von Meldungen
- Verwaltung von Wettkämpfen
- Automatische oder manuelle Erstellung von Rennabläufen
- Eingabe der erschwommenen Zeiten
- Auswertung der Ergebnisse
- Erstellung von Dokumenten (Startlisten, Urkunden, Ergebnislisten, ...)

## Erste Inbetriebnahme ("Installation")

Die Software benötigt keine Installation im klassischen Sinne. Es muss lediglich die .exe Datei entpackt werden.

1. [Aktuellstes Release herunterladen](https://github.com/M1S2/Vereinsmeisterschaften/releases/latest).
2. Die heruntergeladene .zip Datei an einen beliebigen Ort entpacken.
3. Die Datei `Vereinsmeisterschaften.exe` im entpackten Ordner ausführen.
4. Nach dem ersten Start einen Arbeitsbereich auswählen oder einen neuen Ordner anlegen und dann auswählen.

Sollte die Software nicht starten, könnte eine Runtime Version fehlen.
Dann folgenden Runtime Installer herunterladen und installieren: [.NET Desktop Runtime 10.0.1 Installer](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-desktop-10.0.1-windows-x64-installer)
Anschließend sollte die Software starten.

## Seiten Beschreibung
Nachfolgend werden kurz die Funktionen der jeweiligen Seite der Anwendung beschrieben. Die Reihenfolge der Seiten spiegelt ungefähr den gesamten Workflow wieder (von der Eingabe der Meldungen, über die Eingabe der Zeiten während des Wettkampfs bis zur Auswertung).

### Start
Startseite, von der auf alle anderen Bereiche zugegriffen werden kann. Gleichwertig mit dem Menü am linken Rand.

<img src="https://github.com/M1S2/Vereinsmeisterschaften/raw/master/Doc/Screenshots/Screenshot_Main.png" width="100%"></img>

### Arbeitsbereich
Hier kann der aktuelle Ordner des Arbeitsbereichs (mehr Details siehe unten) ausgewählt und alle Änderungen gespeichert werden (standardmäßig eingeklappt, sobald ein Arbeitsbereich offen ist).
Außerdem können alle Einstellungen für verschiedene Aspekte der Software hier getroffen und auch wieder zurückgesetzt werden.

<img src="https://github.com/M1S2/Vereinsmeisterschaften/raw/master/Doc/Screenshots/Screenshot_Workspace.png" width="100%"></img>

Es gibt außerdem einen alternative Ansicht, mit der von überall aus der aktuelle Arbeitsbereich gespeichert oder gewechselt werden kann (diese ist über die Titelleiste neben dem Zurück-Knopf zugänglich).

<img src="https://github.com/M1S2/Vereinsmeisterschaften/raw/master/Doc/Screenshots/Screenshot_WorkspaceManager.png" width="100%"></img>

### Wettkämpfe
Hier werden alle verfügbaren Wettkämpfe und Regel für die zugeordneten Wettkampf Strecken definiert. Ein Wettkampf wird beschrieben über die Lage, die Strecke, das Alter und eine entsprechende Zeit.
Die Zeit wird dann für die Berechnung der Punkte her genommen. Wird exakt die hier eingetragene Zeit erschwommen, gibt es 100 Punkte. Wenn langsamer gibt es weniger Punkte, wenn schneller dann mehr.
Die Strecke eines Wettkampfs wird über die die Wettkampf Distanz Regeln festgelegt. Die erste passende Regel wird verwendet.
Für jeden Start einer Person muss auch ein entsprechender Wettkampf verfügbar sein.

Außerdem können hier auch die Wettkämpfe automatisch aus der Rudolph Tabelle erstellt werden (enthält für verschiedene Alter, Stile und Strecken entsprechende Vorgabezeiten).
Es werden nur Wettkämpfe erzeugt, für die auch Wettkampf Distanz Regeln vorhanden sind.

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

### Ehemalige Vereinsmeister

Hier werden alle ehemaligen und die aktuellen Vereinsmeister angezeigt. Dargestellt werden alle Personen, die im jeweiligen Jahr auf dem Gold, Silber oder Bronze Platz gelandet sind (Gesamtwertung).
Die aktuellen Vereinsmeister werden von der Ergebnisseite genommen.

Alle Informationen über die ehemaligen Vereinsmeister werden in einer Datei abgespeichert (nur die Namen und ein Zeitstempel pro Jahr).
Neue Einträge können nur hinzugefügt werden, indem die aktuellen Vereinsmeister über den Button auf dieser Seite hinzugefügt werden. Alle anderen Änderungen müssen direkt in der Datei gemacht werden.

<img src="https://github.com/M1S2/Vereinsmeisterschaften/raw/master/Doc/Screenshots/Screenshot_WinnerHistory.png" width="100%"></img>

### Dokumente erstellen
Hier können verschiedene Dokumente erstellt werden, die vor, während oder nach dem Wettkampf nötig sind.

Außerdem ist eine Übersicht über alle verfügbaren Platzhalter vorhanden, die in den Dokumenten verwendet werden können.

Mehr Details sind im Kapitel [Dokumenten Templates](#dokumenten-templates) zu finden.

<img src="https://github.com/M1S2/Vereinsmeisterschaften/raw/master/Doc/Screenshots/Screenshot_Documents.png" width="100%"></img>

### Analysen
Auf dieser Seite werden verschiedene Analysen dargestellt. Diese Analysen hängen von verschiedenen Daten ab, die auf den anderen Seiten eingegeben wurden.
Infos zu den einzelnen Analysen:
- **Anzahl Personen:** Analysieren, wie viele Personen gemeldet sind (aktiv und inaktiv).
- **Geschlechterverteilung der aktiven Personen:** Analysieren, wie viel Prozent der Personen männlich bzw. weiblich sind. Nur aktive Personen werden gezählt.
- **Anzahl Starts:** Analysieren, wie viele Starts gemeldet sind (aktiv und inaktiv).
- **Geschlechterverteilung der gültigen Starts:** Analysieren, wie viel Prozent der Starts männlich bzw. weiblich sind. Nur gültige Starts werden gezählt (aktiv und mit zugewiesenem Wettkampf).
- **Anzahl von gültigen Starts pro Stil:** Analysieren, wie viele Starts pro Schwimmstil gemeldet sind. Nur gültige Starts werden gezählt (aktiv und mit zugewiesenem Wettkampf).
- **Anzahl von gültigen Starts pro Distanz:** Analysieren, wie viele Starts pro Distanz gemeldet sind. Nur gültige Starts werden gezählt (aktiv und mit zugewiesenem Wettkampf).
- **Anzahl Personen pro Geburtsjahr:** Analysieren, wie viele Personen pro Geburtsjahr gestartet sind. Nur aktive Personen werden gezählt.
- **Anzahl an gültigen Starts pro Person:** Analysieren, wie viele Starts jede Person hat. Nur gültige Starts werden gezählt (aktiv und mit zugewiesenem Wettkampf).
- **Abstände zwischen den Starts einer Person:** Analysieren, wie viel Pause zwischen den Starts einer Person ist. Nur aktive Starts werden gezählt.
- **Geburtsjahr je Platz im Gesamtergebnis:** Analysieren, welches Geburtsjahr für jeden Platz im Gesamtergebnis zugeordnet ist.
- **Geburtsjahre je Rennen:** Analysieren, welche Geburtsjahre je Rennen antreten.
- **Wettkampf Zeiten:** Analysieren, welche Wettkampfzeiten pro Alter und Stil geschwommen werden müssen.
- **Bisherige Podiumsplätze pro Person: Analysieren, wie viele Podiumsplätze jede Person bisher hatte. Die hier dargestellten Personen sind die Vereinsmeister der vergangenen Jahre. Die momentanen (potentiellen) Vereinsmeister werden nicht mit angezeigt.**

<img src="https://github.com/M1S2/Vereinsmeisterschaften/raw/master/Doc/Screenshots/Screenshot_Analytics.png" width="100%"></img>

### Einstellungen
Hier kann das Software Design verändert werden. Hell, Dunkel oder Systemeinstellung stehen zur Verfügung.

<img src="https://github.com/M1S2/Vereinsmeisterschaften/raw/master/Doc/Screenshots/Screenshot_Settings.png" width="100%"></img>

## Arbeitsbereich
Der Arbeitsbereich ist ein Ordner, in dem alle Einstellungen, Wettkämpfe und Meldungen gespeichert werden. Hier bietet es sich an, für jeden Wettkampf (z.B. jedes Jahr) einen neuen Ordner anzulegen. Es ist egal, wo sich der Ordner auf der Festplatte befindet.

Folgende Dateien werden erzeugt:
- WorkspaceSettings.json: JSON Datei mit allen Einstellungen für den Arbeitsbereich. Dieser Dateiname ist fix.
- Personen.csv / Person.csv: Liste mit allen Meldungen und den erschwommenen Zeiten.
- Wettkaempfe.csv / Competitions.csv: Liste mit allen verfügbaren Wettkämpfen.
- WettkampfDistanzRegeln.csv / CompetitionDistanceRules.csv: Liste mit allen verfügbaren Regeln für die Wettkampf Distanzen.
- BestesRennen.csv / BestRace.csv: Das Rennen, das als gute Variante markiert wurde, wird hier gespeichert. Wenn kein Rennen markiert wurde, existiert diese Datei nicht.
- LetzteVereinsmeister.csv / WinnersHistory.csv: Diese Datei enthält alle bisherigen Vereinsmeister. Pro Zeile wird für ein Jahr gespeichert, wer auf dem Gold, Silber und Bronze Platz gelandet ist. Diese Datei wird immer mit kopiert, wenn ein neuer Arbeitsbereich aus einem Alten erstellt wurde, damit die Historie fortgeführt werden kann.

Die Dateinamen werden folgendermaßen ermittelt:
- Beim Laden des Arbeitsbereichs wird die Datei der aktuellen Systemsprache verwendet (wenn verfügbar). Wenn nicht verfügbar, wird die neueste Datei aus allen verfügbaren Sprachen (momentan Deutsch und Englisch) verwendet.
- Beim Speichern des Arbeitsbereichs wird immer die aktuelle Systemsprache verwendet. Sollte der Arbeitsbereich mit einer Datei einer anderen Sprache geladen worden sein, wird die alte Datei gelöscht und die neue Datei in Systemsprache abgespeichert.

Bei den Datei-Inhalten gilt die gleiche Logik wie beim Dateinamen. Beim Laden wird z.B. "Brust" und "Breaststroke" beim Stil unterstützt. Beim Abspeichern wird z.B. immer "Brust" geschrieben, wenn die Systemsprache Deutsch ist.

Es können hier natürlich weitere Dateien abgelegt werden (z.B. Templates).

## Punktewertung
Die Punkte eines Starts werden folgendermaßen berechnet:
- Folgende Formel wird verwendet: `Punkte = (2 - (ErschwommeneZeit / WettkampfVorgabeZeit)) * 100;`
- Wird exakt die Zeit des hinterlegten Wettkampfs erschwommen (ErschwommeneZeit == WettkampfVorgabeZeit), gibt es 100 Punkte.
- Wenn langsamer dann gibt es weniger Punkte.
- Wenn schneller dann gibt es mehr Punkte.
- Null Punkte, sobald `ErschwommeneZeit >= 2 * WettkampfVorgabeZeit`

Der Start mit der höchsten Punktezahl wird als "Bestes Rennen" gewertet und für die Gesamtwertung herangezogen. Es wird kein Durchschnitt gebildet.

## Dokumenten Templates
Bei der Erstellung von Dokumenten werden Template Dateien verwendet, die als Vorlage für das spätere Dokument verwendet werden.
Bei den Templates handelt es sich um gewöhnliche .docx Dateien, die z.B. in Microsoft Word oder Libre Office erstellt werden können. Diese Dateien legen die Struktur und das Aussehen für die späteren Ausgabedokumente fest. Daten aus der Vereinsmeisterschaften Software können über Platzhalter eingefügt werden.
Platzhalter werden dabei über bestimmte Zeichen (über die Arbeitsbereich Einstellungen konfigurierbar) gekennzeichnet, die vor und nach dem Platzhalternamen stehen (z.B. "%Name%", wobei "%" der Platzhalter Marker ist).

### Verfügbare Platzhalter
Auf der Seite [Dokumente erstellen](#dokumente-erstellen) ist eine Tabelle verfügbar, die alle verfügbaren Platzhalter auflistet. Nicht jeder Platzhalter kann in jedem Dokumententyp verwendet werden. Das wird auch in der Tabelle aufgeschlüsselt.

### Textplatzhalter / Tabellenplatzhalter
Die Platzhalter können entweder direkt im Text oder in Tabellen verwendet werden:
- **Text:** Textplatzhalter werden an ihrer jeweiligen Stelle im Dokument einfach ersetzt. Die Ersetzungsdaten werden anhand des Platzhalternamens bestimmt.
- **Tabelle:** Tabellenplatzhalter müssen sich innerhalb einer Tabelle des Dokuments befinden. Alle Tabellen im Dokument, die mindestens einen unterstützten Platzhalter enthalten, werden verarbeitet. Die erste Zeile mit einem Platzhalter dient als Vorlage für die neuen Zeilen. Für jedes Datenelement wird eine neue Zeile erstellt (z.B. eine neue Zeile je Person). Die Ersetzungsdaten werden durch den Platzhalternamen bestimmt.

Je nach Dokumententyp werden entweder Text- oder Tabellenplatzhalter unterstützt. Das wird auf der Seite [Dokumente erstellen](#dokumente-erstellen) unter dem jeweiligen Button zum Erstellen des Dokuments mit Icons dargestellt.

### Eine Seite vs. Mehrere Seiten
Je nach Dokumententyp wird entweder eine Seite oder mehrere Seiten erstellt. Das wird auf der Seite [Dokumente erstellen](#dokumente-erstellen) unter dem jeweiligen Button zum Erstellen des Dokuments mit Icons dargestellt.
- **Eine Seite:** Es wird nur eine Seite für die Daten dieses Dokumententyps erstellt (bzw. falls der Inhalt zu lang wird, gibt es mehrere Seiten). Z.B. wird aus einer Liste an Personen eine Seite mit einer Tabelle mit allen Personen erzeugt ("Übersichtsliste erstellen").
- **Mehrere Seiten:** Es werden mehrere Seite für die Daten dieses Dokumententyps erstellt (eine Seite pro Datenelement, z.B. eine Seite pro Person). Z.B. wird aus einer Liste an Personen je eine Seite pro Person erzeugt ("Urkunden erstellen").

### PDF Erstellung
In den Arbeitsbereich Einstellungen kann konfiguriert werden, dass neben dem .docx Ausgabedokumente auch ein .pdf erstellt werden soll. Für die Umwandlung gibt es momentan folgende unterstützte Programme:
- Word (Microsoft Office Word): muss auf dem PC installiert sein.
- Libre Office: muss auf dem PC installiert sein.

Welches Programm zur Umwandlung verwendet wird, wird folgendermaßen bestimmt:
1. Es wird bestimmt, mit welchem Programm das Template .docx erstellt wurde. Ist das ermittelte Programm installiert, wird es auch zur Umwandlung verwendet (z.B. Template mit Word erstellt und Word installiert -> Word verwenden).
2. Ist das ermittelte Programm nicht installiert, wird das nächste installierte Programm verwendet (z.B. Template mit Word erstellt, Word NICHT installiert, Libre Office installiert -> Libre Office verwenden).
3. Gibt es einen Fehler bei der Umwandlung, wird auch das nächste installierte Programm probiert.
4. Wurde kein passendes Programm gefunden oder gab es einen Fehler bei der Umwandlung, wird eine Fehlermeldung angezeigt.

## Wettkampf Zuordnung
Jeder Start einer Person muss einem Wettkampf zugeordnet werden. Dies geschieht abhängig vom gewählten Modus. Folgendes Diagramm zeigt exemplarisch die Zuordnung für Brust Starts (Wasserflöhe werden immer dem ersten gefundenen Wasserfloh-Wettkampf zugeordnet):

```mermaid
---
config:
    layout: elk
---
flowchart LR
    subgraph Wettkämpfe
        C18f["♀ Brust 18J."]
        C18m["♂ Brust 18J."]
        C17f["♀ Brust 17J."]
        C17m["♂ Brust 17J."]
        C16f["♀ Brust 16J."]
        C16m["♂ Brust 16J."]
        C15f["♀ Brust 15J."]
        C15m["♂ Brust 15J."]
        C14f["♀ Brust 14J."]
        C14m["♂ Brust 14J."]
        C13f["♀ Brust 13J."]
        C13m["♂ Brust 13J."]
        Gap["Fehlende Wettkämpfe"]
        C9f["♀ Brust 9J."]
        C9m["♂ Brust 9J."]
        C8f["♀ Brust 8J."]
        C8m["♂ Brust 8J."]
        C7f["♀ Brust 7J."]
        C7m["♂ Brust 7J."]
        C6f["♀ Wasserfloh 6J."]
        C6m["♂ Wasserfloh 6J."]
    end

    subgraph Legende
        direction TB
        L1["🟡① Nur genaues Alter"]
        L2["🔴② Genaues oder nächst niedrigeres Alter"]
        L3["🟢③ Genaues oder nächst höheres Alter"]
        L4["🔵④ Genaues Alter oder max. Alter wenn die Person älter ist"]
        L5["⚫⑤ Genaues Alter oder nächstgelegenes Alter (niedrigeres Alter bevorzugen bei gleichem Abstand)"]
        L6["⚪⑥ Genaues Alter oder nächstgelegenes Alter (höheres Alter bevorzugen bei gleichem Abstand)"]
    end

    P19f["👤 ♀ 19J."]
    P18m["👤 ♂ 18J."]
    P15f["👤 ♀ 15J."]
    P12m["👤 ♂ 12J."]
    P11m["👤 ♂ 11J."]
    P8f["👤 ♀ 8J."]
    P5m["👤 ♂ 5J."]
    
    P19f -- "🔴② 🔵④ ⚫⑤ ⚪⑥" --> C18f
    P18m -- "🟡① 🔴② 🟢③ 🔵④ ⚫⑤ ⚪⑥" --> C18m
    P15f -- "🟡① 🔴② 🟢③ 🔵④ ⚫⑤ ⚪⑥" --> C15f
    P12m -- "🟢③ ⚫⑤ ⚪⑥" --> C13f
    P12m -- "🔴②" --> C9f    
    P11m -- "🟢③ ⚪⑥" --> C13f
    P11m -- "🔴② ⚫⑤" --> C9f
    P8f  -- "🟡① 🔴② 🟢③ 🔵④ ⚫⑤ ⚪⑥" --> C8m
    P5m  -- "🟡① 🔴② 🟢③ 🔵④" --> C6m
```

## Entwickler Dokumentation
Detailliertere Entwickler Dokumentation: https://m1s2.github.io/Vereinsmeisterschaften

## Icons
- SwimmingStyles.png: https://www.vecteezy.com/vector-art/349116-swimming-aqua-aquatic-sport-game-icon-symbol-sign-pictogram
- icons8-child-64.png: https://icons8.com/icon/67246/babys-room
- Geometry_Female: https://pictogrammers.com/library/mdi/icon/gender-female/
- Geometry_Male: https://pictogrammers.com/library/mdi/icon/gender-male/
- Geometry_DecimalComma: https://pictogrammers.com/library/mdi/icon/decimal-comma/
- Geometry_Counter: https://pictogrammers.com/library/mdi/icon/counter/
- Geometry_Weight: https://pictogrammers.com/library/mdi/icon/weight/
- Geometry_Priority: https://pictogrammers.com/library/mdi/icon/priority-high/
- Geometry_Trophy: https://pictogrammers.com/library/mdi/icon/trophy-outline/_
- Geometry_AccountTie: https://pictogrammers.com/library/mdi/icon/account-tie/

https://retouchinglabs.com/inkscape-convert-png-to-svg/
https://stackoverflow.com/questions/18624070/convert-svg-to-xaml