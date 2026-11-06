# Vereinsmeisterschaften Changelog

## [v2.0.0]

- Neue Funktion zur Anzeige der bisherigen Gewinnerhistorie hinzugefügt (neue Seite, neue Analysen und neue Workspace-Datei) (#9)
- Metadaten für CSV-Dateien unterstützt, zuletzt verwendete Rudolph-Tabellenparameter in der Wettkampfdatei speichern und in der Benutzeroberfläche anzeigen
- Verarbeitung der Rudolph-Tabelle angepasst (Interpolation von kubischen Splines auf Polynome umgestellt und offene Altersklassen berücksichtigt) (#29)
- Erstellung neuer Workspaces durch einen gemeinsamen Dialog zur Eingabe aller erforderlichen Angaben verbessert
- Enter-Taste im `TimeSpanControl` zum Verlassen des Fokusmodus unterstützt und das `TimeSpanControl` auf der Wettkampfseite grundsätzlich verwendet (nicht mehr nur bei der Zellenbearbeitung)
- Hinweis zur Unterstützung der mehrspaltigen Sortierung im DataGrid ergänzt
- Wettkampfzeiten aus der Rudolph-Tabelle für nicht unterstützte Altersklassen interpolieren (#29);
- Diagramm zum Vergleich von Wettkampfzeiten und Alter hinzugefügt (#28)
- Seiten nach dem Laden eines Workspaces über die Seitenleiste aktualisieren (#30)
- Native `OpenFolderDialog`- und `OpenFileDialog`-Dialoge anstelle der Varianten aus Windows Forms verwenden (#32)
- Symbole und Texte für leere Listen ergänzt (#31)
- Validierung der Wettkampfstreckenregeln ergänzt und den Stil „Alle“ für Wettkampfstreckenregeln entfernt (#10)
- Prüfung auf doppelte Wettkämpfe ergänzt
- Wettkampfstrecken automatisch anhand der Streckenregeln zuweisen und Drag & Drop zum Sortieren auf der Wettkampfseite ermöglichen (#10)
- Neuen Dienst zur Verwaltung von Wettkampfstreckenregeln hinzugefügt (#10)
- Parser und Datenmodelle für die Rudolph-Tabelle ergänzt und auf der Wettkampfseite eingebunden (#10)
- Kleinere optische Anpassungen vorgenommen (Farbe des Workspace-Manager-Flyouts und verschiedene Textfarben)
- Dokumentation zur Verwendung von Vorlagen ergänzt (#12)
- Weitere Details zu Platzhaltern und Dokumenten ergänzt (#27)
- PDF-Konvertierung über Microsoft Word ergänzt (die verwendete Konvertierungsanwendung wird dynamisch ausgewählt) (#26)
- Möglichkeit zur Einrichtung eines neuen Workspaces einschließlich Standardvorlagen ergänzt (#21)
- Dokumenttyp für Analysen und entsprechende Platzhalter ergänzt (#25)
- Kennzeichnung für vollständig neue Workspaces ergänzt (werden als ungespeichert betrachtet); einige Analysen sind erst verfügbar, wenn Personen vorhanden sind; Tooltips angepasst
- Workspace-Manager zum Speichern und Laden von Workspaces über die Titelleiste ergänzt und zuletzt verwendete Workspace-Pfade gespeichert (#24)
- Dokumentation zur Entwicklungsumgebung und Hinweis zum .NET-Runtime-Installer in der README ergänzt (#11)
- Dokumentation zu Analysemodulen und Ansichten ergänzt (#22)
- Start- und Personenzähler von der Workspace-Seite entfernt und Screenshots der Analyseseite aktualisiert (#8)
- Analyseseite und Analysedienste erstellt (#8, #13, #14, #15, #16, #17, #18, #19, #23)
- Größe der Build-Ausgabe durch Entfernen von `Microsoft.Windows.SDK.NET.dll` reduziert (#20)
- Zielframework auf .NET 10 (Long-Term Support) und Solution-Datei auf `.slnx` aktualisiert; hierfür wird Visual Studio 2026 benötigt
- Fehlerhafte Dialogverwaltung korrigiert (#4)
- Geburtsjahr von Personen auf der Seite zur Wettkampfvorbereitung über einen Tooltip anzeigen (#7)
- Tooltip des `TimeSpanControl` korrigiert (zeigt nun die korrekte Anzahl an Nachkommastellen an) (#5)
- Zeitformat für die Dokumentenerstellung korrigiert (#6)

## [v1.0.0]

- Erste Version
