# Programátorská dokumentace

*Tady vysvětlete hlavní koncepty a strukturu vašeho programu. Pokud je váš program rozdělen do několika souborů, napište, k čemu každý soubor slouží a jakou funkcionalitu (třídy nebo funkce) v něm najdu. Pokud v programu používáte třídy, stručně popište význam těch nejdůležitějších (co mají za úkol).*

## Struktura programu

Program má 6 projektů:
### Brainfuck
Tento projekt se stará o vstup, jedná se o konzolovou aplikaci a k parsování vstupu z příkazové řádky
používám knihovnu System.CommandLine 

### BytecodeGenerator
Tento projekt se stará o překlad zdrojového kódu do předem definovaného bytekódu, obsahuje následující třídy:
- `BytecodeGenerator`, abstraktní třída, která definuje členy, které sdílí všechny třídy, které generují bytekód
  - Hlavní je abstraktní metoda `CreateByteCode`, která vytváří daný bytekód a vrací ho v podobě pole bytů
  - dále obsahuje pole jako jsou například list, pro kontrulu správnosti závorek nebo stream reader, ze kterého se načítá
  zdrojový kód
- `BytecodeGeneratorFactory`, třída, která se stará o vytvoření specifického generátoru bytekódu, podle zadaného otisku
- `SimpleByteCodeGenerator`, `FirstLevelBytecodeGenerator`, `SecondLevelBytecodeGenerator`, třídy které se starají o
překlad zdrojového kódu do nějakého určitého bytekódu, všechny z těchto tříd dědí od abstraktní třídy `BytecodeGenerator`

### BytecodeInterpreter
Tento projekt se stará o interpretaci definovaných bytekódů,  obsahuje následující třídy:
- `Interpreter`, třída, která se stará o vykonávání instrukcí bytekódu, který dostane na vykonání, pro interpretaci je
je potřeba zavolat metodu `Run(CodeRunner runner)`
  - metoda si od daného `CodeRunner` objektu vyžádá funkce, které reprezentují jednotlivé instrukce a postupně je volá,
  dokud nenarazí na konec kódu
- `CodeRunner`, abstraktní třída, která definuje členy, které sdílí všechny třídy, které se starají o vykonavání instrukcí
bytekódů
  - Hlavní je abstraktí metoda `GetDelegates()`, která vrátí pole delegátů typu `RunFunc`, tyto delegáty reprezentují
  jednotlivé instrukce daných bytekódů
  - dále obsahuje metody `PrintValue()` a `ReadValue()`, které jsou sdílené všemi code runner třídami pro načítání do paměti
  a výpis z paměti
  - dále jsou tady pole, které zastávají úlohu Paměti, Instruction pointeru a pointeru do paměti
- `CodeRunnerFactory`, třída, která se stará o vytvoření specifického code runneru, podle zadaného bytekódu
- `SimpleCodeRunner`, `FirstLevelCodeRunner`, `SecondLevelCodeRunner`, třídy, které se starají o vykonávaní instrukcí
jednotlivých bytekódů, všechny dědí od třídy `CodeRunner`

### Utilities
Tento projekt seskupuje třídy, které jsou většinou využívány více projekty nebo nesouvisí přímo s nějakým jiným projektem,
obsahuje následující třídy:
- `AssemblyTranslator`, tato třída do specifické instance třídy `StreamWriter` zapíše přeložený bytekód v human readable
formě (něco jako assembly)
- `ByteNumbersHandler`, tato třída se stará překlad čísel do polí bytů, tato třída existuje, jelikož třída `System.BitConverter` 
ze standartní knihovny C# je machine specific
- `IoHandler`, tato třída se stará o vstup a výstup, především u testů

### IntegrationTests a UnitTests
V těchto projekty jsou testy, k testování využívám knihovnu xUnit.


