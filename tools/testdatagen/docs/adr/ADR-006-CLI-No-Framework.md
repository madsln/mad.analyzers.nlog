# ADR-006 – CLI-Architektur: kein Framework, statisches Routing

**Datum:** 2026-04-25  
**Status:** Akzeptiert  
**Kontext:** WL-06 – CLI Layer & Entry Point

---

## Kontext

Für die CLI-Schicht standen zwei Optionen zur Wahl:
1. Ein CLI-Framework (z. B. `System.CommandLine` oder `Spectre.Console.Cli`)
2. Manuelles Argument-Parsing mit statischem Routing

Das Tool hat genau drei Subkommandos (`demo`, `test`, `all`) mit je zwei bis drei
klar definierten Parametern und keinerlei Subkommando-Verschachtelung.

---

## Entscheidung

**Manuelles Argument-Parsing** – kein externes CLI-Framework.

---

## Begründung

- Das Argument-Schema ist trivial und vollständig bekannt (keine dynamischen Erweiterungen geplant).
- Ein Framework würde eine zusätzliche NuGet-Abhängigkeit einführen, die der restlichen
  Codebasis (zero external deps) widerspricht.
- `ParseArg(args, "--key")` ist in jeder Command-Klasse identisch und vollständig lesbar.
- Die gesamte CLI-Logik ist in < 150 Zeilen ausgedrückt.

---

## Konsequenzen

- Kein NuGet-Paket `System.CommandLine` o. ä. erforderlich.
- Erweiterung um weitere Subkommandos erfordert eine neue Klasse + einen `switch`-Eintrag in `CliRouter`.
- Automatische Shell-Completion und strukturierte Fehler-Typen (wie bei `System.CommandLine`) stehen nicht zur Verfügung – für ein internes Build-Tool akzeptabel.
