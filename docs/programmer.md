# Programátorská dokumentace

## Bytekódy

### Otisky

Každý bytekód má vlastní otisk, aby bylo možné z pole bytů rozeznat, jak se má interpretovat, tento otisk jsou první
4 byty v bytekódu
- otisk Simple bytekódu je 0
- otisk First level bytekódu je 1
- otisk Second level bytekódu je 2

### Simple bytekód
Simple bytekód má 8 instrukcí
- 0x00 je přičtení 1 k aktuální buňce
- 0x01 je odečtení 1 od aktuální buňky
- 0x02 je posunutí data pointeru o jedno doleva
- 0x03 je posunutí data pointeru o jedno doprava
- 0x04 je vypsání hodnoty aktuální buňky
- 0x05 je přečtění jednoho bytu ze vstupu a uložení jeho hodnoty do aktivní buňky
- 0x06 is levá závorka, má 4 bytový parametr, který je adresa kam se má skočit, pokud skončí iterace skrz cyklus, parametr je LSB first
- 0x07 is pravá závorka, má 4 bytový parametr, který je adresa kam se má skočit, pokud se má pokračovat v iterovaní cyklu, parametr je LSB first

### First level bytekód
First level bytekód má 8 instrukcí, ale spojuje více stejých instrukcí, které přicházejí hned za sebou do jedné instrukce
- parametr těchto instrukcí specifikuje, kolik elementárních instrukcí jdoucích za sebou nahrazuje
  - 0x00 je přičtení, má 1 bytový parametr
  - 0x01 je odečtení, má 1 bytový parametr
  - 0x02 is posunutí data pointeru doleva, má 4 bytový parametr
  - 0x03 is posunutí data pointeru doprava, má 4 bytový parametr
- 0x04 je vypsání hodnoty aktuální buňky
- 0x05 je přečtění jednoho bytu ze vstupu a uložení jeho hodnoty do aktivní buňky
- 0x06 is levá závorka, má 4 bytový parametr, který je adresa kam se má skočit, pokud skončí iterace skrz cyklus, parametr je LSB first
- 0x07 is pravá závorka, má 4 bytový parametr, který je adresa kam se má skočit, pokud se má pokračovat v iterovaní cyklu, parametr je LSB first

### Second level bytekód
Tento bytekód je stejný jako First level bytekód, ale je rozšířen o dvě další instrukce:
- 0x08 - Set memory zero, tato instrukce nahrazuje tento cyklus v kódu Brainfucku: `[-]` nebo `[+]`
  - tato instrukce nastaví hodnotu aktivní paměťové buňky na 0
  - tato instrukce nemá žádné parametry
- 0x09 - Move data, tato instrukce nahrazuje tento cyklus v kódu Brainfucku: `[->+<]`, `[-<+>]`, `[<+>-]` nebo `[>+<-]`,
který se stará o přesun hodnoty z jedné buňky do buňky další
  - počet instrukcí pro pohyb v paměti může být více, ale musí být stejný pro pohyb do obou směrů a tyto instrukce musí být
  bezprostředně za sebou
  - počet instrukcí pro přičtení může být větší než jedna (to lze využít k vynásobení hodnoty v paměti), ale všechny musí
  být bezprostředně za sebou
  - tato instrukce ma 5 bytový parametr
    - první byte je koeficient násobení (kolikrát je ve zdrojovém kódu instrukce pro přičtení)
    - zbylé čtyři byty jsou signed integer, který reprezentuje kam se mají data přesunout (je to relativní posun vzhledem
    k data pointeru)

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

#### Soubor `SyntaxTreeGenerationClasses.cs`
Tento soubor obsahuje třídy, které reprezentují kompilovaný program a jeho části v průběhu kompilace, pomocí těchto tříd 
se staví syntax tree celého programu, tyto třídy využívá pouze `SecondLevelBytecodeGenerator` a to konkrétně k tomu aby 
bylo jednodušší rozeznat instrukce Move data a Set memory zero při překladu programu. Soubor obsahuje tyto třídy:
- `Operation` tato třída rerezentuje jednu instrukci byte kódu a to i s parametrem
  - třída má pouze virtuální funkci `AddBytes(List<byte> bytes)`, která danou instrukci přidá do `bytes` argumentu
- `LoopStart : Operation`, tato třída reprezentuje cyklus v kódu, dědí od třídy `Operation`
  - třída sice reprezentuje cyklus, ale ukončení cyklu musí být do cyklu přidáno stejným způsobem jako ostatní instrukce
  - třída má funcki `AddOperation(Operation operation)`, která přidá instrukci do cyklu při procházení kódu Brainfucku
  - třída má dále funkce `IsSetMemoryZero()`, `IsMoveDataStartDecrement()` a `IsMoveDataStartShift()`, které rozpoznají, jestli
    cyklus lze nahradit za instrukce Move data nebo Set memory zero
  - třída definuje vlastní implementaci virtuální metody `AddBytes(List<byte> bytes)`
- `Program : LoopStart` tato třída reprezentuje celý překládaný program, dědí od třídy `LoopStart`, udržuje si informaci 
o tom, jaký cyklus právě překládá a má zásobník všech cyklů, jak se postupně při překladaci zanořuje, o toto se třída
stará na pozadí, třída má tyto metody:
  - `AddNewOperation(ExtendedInstructions opCode, byte[]? pars)`, která přidá instrukci do programu při procházení kódu Brainfucku
  - `SwapCurrentScope(ExtendedInstructions newInstructionOpCode, byte[]? pars)`, tato metoda vymění cyklus, který je 
  právě procházan za jednu instrukci
  - `SetLoopStartParams(byte[]? pars)`, která nastaví adresu skoku cyklu, ve chvíli, kdy už je tato adresa známá

Při kompilaci stačí vytvořit jednu instanci třídy `Program` a přidávat do této instance pomocí metody 
`AddNewOperation(ExtendedInstructions opCode, byte[]? pars)` instrukce. Pouze je potřeba ve chvíli, kdy program narazí na
`]` instrukci, tak je potřeba manuálně pomocí metody `SetLoopStartParams(byte[]? pars)` nastavit adresu kam se má skákat ze
začátku cyklu.

### BytecodeInterpreter
Tento projekt se stará o interpretaci definovaných bytekódů,  obsahuje následující třídy:
- `Interpreter`, třída, která se stará o vykonávání instrukcí bytekódu, který dostane na vykonání, pro interpretaci je
je potřeba zavolat metodu `Run(CodeRunner runner)`
  - metoda si od daného `CodeRunner` objektu vyžádá funkce, které reprezentují jednotlivé instrukce a postupně je volá,
  dokud nenarazí na konec kódu, o získání argumentů se starají samotné funkce
- `CodeRunner`, abstraktní třída, která definuje členy, které sdílí všechny třídy, které se starají o vykonavání instrukcí
bytekódů
  - Hlavní je abstraktní metoda `GetDelegates()`, která vrátí pole delegátů typu `RunFunc`, tyto delegáty reprezentují
  jednotlivé instrukce daných bytekódů
  - dále obsahuje metody `PrintValue()` a `ReadValue()`, které jsou sdílené všemi code runner třídami pro načítání do paměti
  a výpis z paměti
  - dále je tady pole, které zastává úlohu Paměti a členy zastávající funkci Instruction pointeru a pointeru do paměti
- `CodeRunnerFactory`, třída, která se stará o vytvoření specifického code runneru, podle zadaného bytekódu
- `SimpleCodeRunner`, `FirstLevelCodeRunner`, `SecondLevelCodeRunner`, třídy, které se starají o vykonávaní instrukcí
jednotlivých bytekódů, všechny dědí od třídy `CodeRunner`

### Utilities
Tento projekt seskupuje třídy, které jsou využívány více projekty nebo nesouvisí přímo s nějakým projektem,
obsahuje následující třídy:
- `AssemblyTranslator`, tato třída do specifické instance třídy `StreamWriter` zapíše přeložený bytekód v human readable
formě (něco jako assembly)
- `ByteNumbersHandler`, tato třída se stará překlad čísel do polí bytů, tato třída existuje, jelikož třída `System.BitConverter` 
ze standartní knihovny C# je machine specific
- `IoHandler`, tato třída se stará o vstup a výstup, především u testů

### IntegrationTests a UnitTests
V těchto projekty jsou testy, k testování využívám knihovnu xUnit.
