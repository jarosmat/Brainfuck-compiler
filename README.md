# Zápočtový program: Brainfuck virtual machine

## Specifikace

Tento program má za úkol kompilovat programovací jazyk Brainfuck do mnou definovaného bytekódu, a následně tento bytekód
interpretovat. Definoval jsem více verzí bytekódu, různé verze mají za úkol optimalizovat výpočet, toho dosáhnu přidáním
parametrů pro instrukce nebo rozšířením bytekódu a nahrazením některých konstrukcí v jazyce Brainfuck jednou instrukcí v
mnou definovaném bytekódu.

## Instalace a spuštění

Před spuětěním programu je potřeba nainstalovat knihovnu `System.CommandLine`, pro instalaci prejděte do složky `Brainfuck`
a spusťte příkaz `dotnet add package System.CommandLine --prerelease` (je nutné specifikovat `--prerelease`, jelikož knihovna je
stále v beta verzi)

Pro spuštění zůstaňte ve složce `Brainfuck` a spusťte příkaz `dotnet run -- -h`, nebo spusťte ve stejné složce `dotnet build`
a přejděte do složky `Brainfuck/Brainfuck/bin/Debug/net9.0`, kde spusťte již vytvořený executable soubor jako konzolovou aplikaci.
Pro referenci k použití použijte option `-h` při spouštění nebo přejděte na [tento odkaz](docs/user.md) pro rozsáhlejší dokumentaci

## Dokumentace

* [Uživatelská dokumentace](docs/user.md)
* [Ukázky použití](docs/examples.md)
* [Programátorská dokumentace](docs/programmer.md)
