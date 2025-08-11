# Uživatelská dokumentace

Program slouží jako virtuální stroj pro spouštění programů napsaných v programovacím jazyce Brainfuck

## Popis jazyka Brainfuck

Kód brainfucku se provádí nad polem buněk, buněk je v poli 30 000 a každá buňka má kapacitu 8 bitů,
při přetečení nebo podtečení začne buňka opět od 0 nebo 255,
V jednu chvíli se pracuje pouze nad jednou(aktivní) buňkou. Kód se skládá z jednoznakových příkazů,
všechny ostatní znaky jsou ignorovány. V Brainfucku je 8 jednoznakových příkazů:
- "+" přičtení 1 k aktivní buňce
- "-" odečtení 1 od aktivní buňky
- "<" posunutí ukazatele na buňky o 1 doleva
- ">" posunutí ukazatele na buňky o 1 doprava
- "," uložení 8 bitů ze vstupu do aktivní buňky
- "." vypsání hodnoty aktivní buňky na výstup
- "[" začátek while loopu - pokud je hodnota aktivní buňky 0 začne interpret vykonávat příkazy za odpovídajícím znakem "\]"
- "]" konec while loopu - interpret začne vykonávat odpovídající znak "["

Soubory se zdrojovým kódem Brainfucku obyčejně končí příponou .bf

## Build

Před spuětěním programu je potřeba nainstalovat knihovnu `System.CommandLine`, pro instalaci prejděte do složky `Brainfuck`
a spusťte příkaz `dotnet add package System.CommandLine --prerelease`

Pro sestavení zůstaňte ve složce `Brainfuck` a spusťte příkaz `dotnet build` a přejděte do složky `/Brainfuck/bin/Debug/net9.0`,
kde spusťte již vytvořený executable soubor jako konzolovou aplikaci.

## Ovládání

použití:\
`Brainfuck <soubor ke spuštění> [options]`

Argumenty:

`<soubor ke spuštění>`

Options:
- `-?`, `-h`, `--help`  Vypíše tento návod v angličtine na příkazovou řádku
- `--version`       Ukáže informace o verzi
- `-v`              Tímto optionem lze specifikovat, jak verzi bytekódu má program použít:
  1. pro použití Simple bytekódu: 0, 
  2. pro použití First level bytekódu: 1
  3. pro požití Second level bytekódu: 2, 
  - pokud nebude specifikováno jinak, bude použit Second level bytekód
- `-b`              Tímto optionem lze specifikovat, do jakého souboru má program vypsat Assembly zkompilovaného programu






