<div align="center">

# Projekt k-NN (*k-Nearest Neighbors*) - *"k-Najbliższych-Sąsiadów"*

<img src="https://sklep.pja.edu.pl/wp-content/uploads/2017/03/PJATK_shop-1.png" width="50%"></img>
## Narzędzia Sztucznej Inteligencji (NAI) PJATK
### Cyprian Gburek<br>s24759, 17c
</div>
<br>

## Spis treści
1. [Opis projektu](#opis-projektu)
2. [Założenia projektu](#założenia-projektu)
3. [Wykorzystane biblioteki](#wykorzystane-biblioteki)
4. [Przykładowe wyniki](#przykładowe-wyniki)
5. [Uruchamianie projektu](#uruchamianie-projektu)

## Opis projektu

Projekt k-NN *"k-Najbliższych-Sąsiadów"* to implementacja algorytmu k-NN w języku C#. Projekt został stworzony w celu zaliczenia przedmiotu Narzędzia Sztucznej Inteligencji na Polsko-Japońskiej Akademii Technik Komputerowych (PJATK).
<br><br>
Autorem projektu jest Cyprian Gburek, w chwili oddania projektu student 2 roku Informatyki na PJATK.

## Założenia projektu

Projekt ma na celu implementację algorytmu k-NN w języku C#. W tym celu użytkownik ma za zadanie dostarczenie programowi trzech argumentów wejściowych:
- ścieżki absolutnej do pliku z danymi uczącymi
- ścieżki absolutnej do folderu do zapisu wyników klasyfikacji
- liczby k (ilość najbliższych sąsiadów)
- (**opcjonalnie**) ścieżki absolutnej do pliku z danymi testowymi
Użytkownik może podać wskazane argumenty poprzez:
- wpisanie ich w konsoli przy inicjalizacji modelu k-NN w trakcie działania programu
- podanie ich jako argumenty wejściowe do programu, w podanej kolejności

## Wykorzystane biblioteki
Projekt wykorzystuje następujące biblioteki:
- [Spectre.Console](https://spectreconsole.net/) - biblioteka do tworzenia interfejsu użytkownika w konsoli
- [ScottPlot](https://scottplot.net/) - biblioteka do tworzenia wykresów i grafów w oddzielnym oknie Windows Forms, oraz do zapisu wykresów do oddzielnych plików

## Uruchamianie projektu
Projekt można uruchomić poprzez:
- uruchomienie projektu w dowolnym środowisku programistycznym (np. Visual Studio)
- komendę `dotnet run` w konsoli w folderze z projektem (wymagane zainstalowanie .NET Core SDK)
- kompilację projektu do pliku wykonywalnego i uruchomienie go
- 