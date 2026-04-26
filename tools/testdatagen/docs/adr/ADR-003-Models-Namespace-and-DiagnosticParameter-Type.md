# ADR-003 – Models-Namespace und DiagnosticDescriptor.Parameters-Typ

**Status:** Akzeptiert  
**Datum:** 2026-04-25  
**Kontext:** WL-03 – Datenmodelle & I/O Layer

---

## Kontext

Bei der Implementierung der Datenmodelle aus WL-03 mussten zwei Designentscheidungen getroffen werden:

1. **Namespace-Platzierung der Records** – Root-Namespace `TestDataGen` vs. Unter-Namespace `TestDataGen.Models`.
2. **Typ für `DiagnosticDescriptor.Parameters`** – Separater Record `DiagnosticParameter` vs. Value-Tuple `(string Type, string Value)`.

---

## Entscheidung 1: Root-Namespace für Models

**Gewählt:** `namespace TestDataGen;` (kein Unter-Namespace)

**Begründung:**
- Die Records `DemoVariant`, `DiagnosticDescriptor`, `TestMethodSpec`, `TestClassSpec` sind projektweite DTOs, die von mehreren Schichten (`Demo/`, `Test/`, `IO/`) verwendet werden.
- Ein Root-Namespace vermeidet `using TestDataGen.Models;`-Direktiven in allen Dateien.
- Das Projekt ist klein genug, dass ein Unter-Namespace keinen Mehrwert bietet.

**Verworfene Alternative:** `namespace TestDataGen.Models;`

---

## Entscheidung 2: Value-Tuple für `DiagnosticDescriptor.Parameters`

**Gewählt:** `IReadOnlyList<(string Type, string Value)>`

**Begründung:**
- Die Parameter-Paare (Typ + Wert) sind einfach und werden nur an einer Stelle (im `TestMethodBuilder`) ausgewertet.
- Ein separater Record `DiagnosticParameter` wäre Overengineering für zwei Felder.
- Value-Tuples sind in C# 7+ idiomatisch für kurzlebige, kleine Datenpakete.

**Verworfene Alternative:** `record DiagnosticParameter(string Type, string Value);`

---

## Konsequenzen

- Alle Schichten können `DemoVariant` etc. ohne zusätzliche `using`-Direktiven nutzen.
- `Parameters`-Zugriff erfolgt via `.Type` und `.Value` (benannte Tuple-Elemente).
