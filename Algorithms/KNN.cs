﻿using System.Windows.Forms;
using NAI.Projekt.KNN_ConsoleApp_s24759.Forms;
using NAI.Projekt.KNN_ConsoleApp_s24759.Structures;
using ScottPlot;
using Spectre.Console;
using Style = Spectre.Console.Style;

namespace NAI.Projekt.KNN_ConsoleApp_s24759.Algorithms;

public class KNN
{
    public int Kvalue { get; private set; }
    public List<KnnVector<double>> TrainSet { get; private set; }

    public static readonly string[] ColumnStylesNames = {
        "aqua",
        "hotpink3",
        "teal",
        "mediumpurple4",
        "darkorange",
    }; 

    public KNN(int kValue, List<KnnVector<double>> trainSet)
    {
        Kvalue = kValue;
        TrainSet = trainSet;
        CheckForInputDataValidity();
    }

    private void CheckForInputDataValidity()
    {
        // Sprawdzamy poprawność danych wejściowych

        // Sprawdzenie dla K

        while (Kvalue > TrainSet.Count || Kvalue < 1)
        {
            AnsiConsole.WriteLine("[red]K musi być większe od 0 i mniejsze od liczby wektorów uczących[/]");
            Kvalue = AnsiConsole.Ask<int>("K musi być mniejsze od liczby wektorów uczących. Podaj nową wartość");
            
        }

        // Sprawdzenie dla danych uczących
        if (TrainSet.Count < 1)
            throw new Exception("Liczba wektorów uczących musi być większa od 0");

        var numberOfPointsInFirstVector = TrainSet[0].NumberOfPoints;
        if (TrainSet.Any(vector => vector.NumberOfPoints != numberOfPointsInFirstVector))
            throw new Exception("Liczba punktów we wszystkich wektorach musi być taka sama");

    }
    
    private static IEnumerable<KnnVectorWithDistance<double>> FindKNearest(in KnnVector<double> vector, in IEnumerable<KnnVector<double>> trainingSet, int kValue)
    { 
        var distances = new List<KnnVectorWithDistance<double>>();
        
        foreach (var trainingVector in trainingSet)
        {
            var distance = CalculateEuclideanDistance(vector, trainingVector);
            distances.Add(new KnnVectorWithDistance<double>(trainingVector, distance));
        }
            
        return distances
            .OrderBy(element => element.Distance)
            .Take(kValue);
    }
    
    public void TestData(in IEnumerable<KnnVector<double>> testSet, string savePath, bool checkIntegrityWithAssignedClass = false, bool appendToLocalTestSet = false)
    {
        var testSetCount = testSet.Count();

        CheckForTestDataValidity(in testSet);
        
        var integrityWithAssignedClass = 0.0;
        var currentTestedVector = 0;
        
        // linie oddzielające sprawdzane wektory
        var titleRule = new Rule();
        titleRule.RuleStyle("grey37");
        titleRule.Centered();
        var emptyRule = new Rule();
        emptyRule.RuleStyle("grey37");
        emptyRule.Centered();
        
        var testedVectors = new List<(KnnVector<double> vector, string originalClass, string givenClass)>();
        
        testSet.ToList().ForEach(element => {
            
            for (var i = 0; i < 2; i++)
                AnsiConsole.Write(emptyRule);
            AnsiConsole.Write(titleRule);
            for (var i = 0; i < 2; i++)
                AnsiConsole.Write(emptyRule);

            currentTestedVector++;
            Console.WriteLine();
            var decisiveAttributeTable = new Table().Centered();
            
            decisiveAttributeTable.Title = new TableTitle($"Wektor testowy {currentTestedVector}/{testSetCount}");
            decisiveAttributeTable.AddColumn(new TableColumn("Oryginalny atrybut decyzyjny"));
            decisiveAttributeTable.AddColumn(new TableColumn("Przypisany atrybut decyzyjny"));

            AnsiConsole.Write(new Markup($"Wektor [bold yellow]testowy[/]:\n[bold]{element}[/]").Centered());
            var kNearest = FindKNearest(element, TrainSet, Kvalue);

            var kNearestVectorsTable = new Table()
                .Centered()
                .Title(new TableTitle($"{Kvalue} [bold yellow]najbliższych[/] wektorów:"));

            var columnPointsNum = kNearest.First().Vector.NumberOfPoints;
            for (var i = 0; i < columnPointsNum; i++)
            {
                kNearestVectorsTable.AddColumn(
                    new TableColumn($"{((VectorPointsAutoGeneratedNames)i).ToString()}").Centered()
                    );
            }
            kNearestVectorsTable.AddColumn(new TableColumn("Atrybut decyzyjny"));
            
            kNearest.ToList().ForEach(vectorWithDistance => {
                var row = new List<Markup>();
                vectorWithDistance.Vector.InnerPoints.ForEach(point => row.Add(
                    new Markup(point.ToString()).Centered()
                    ));
                row.Add(new Markup(vectorWithDistance.Vector.DecisiveAttributeName));
                kNearestVectorsTable.AddRow(row.ToArray());
            });
            
            AnsiConsole.Write(kNearestVectorsTable);

            if(!checkIntegrityWithAssignedClass) return;
                
            var mostPopularClass = kNearest.GroupBy(element => element.Vector.DecisiveAttributeName)
                .OrderByDescending(group => group.Count())
                .First()
                .Key;

            testedVectors.Add((element, element.DecisiveAttributeName, mostPopularClass));
            
            decisiveAttributeTable.AddRow(new Markup(element.DecisiveAttributeName), new Markup(mostPopularClass));
            
            var decisiveAttributesWithCount = kNearest.GroupBy(element => element.Vector.DecisiveAttributeName)
                .Select(group => (group.Key, group.Count()))
                .OrderByDescending(element => element.Item2);
            
            var decisiveAttributesTable = new Table().Centered().BorderStyle(Style.Parse("grey37"));
            decisiveAttributesTable.Title = new TableTitle("Atrybuty decyzyjne");
            decisiveAttributesTable.AddColumn(new TableColumn("Atrybut decyzyjny"));
            decisiveAttributesTable.AddColumn(new TableColumn("Liczba wystąpień"));

            foreach (var pair in decisiveAttributesWithCount)
            {
                decisiveAttributesTable.AddRow(new Markup(pair.Item1).Centered(), new Markup(pair.Item2.ToString()).Centered());
            }
            
            AnsiConsole.Write(decisiveAttributesTable);

            if (mostPopularClass == element.DecisiveAttributeName)
            {
                integrityWithAssignedClass++;
                AnsiConsole.Write(
                    new Markup($"[bold green]Przewidywana klasyfikacja zgodna z zadaną klasyfikacją![/]")
                        .Centered());
            }
            else
                AnsiConsole.Write(
                    new Markup($"[bold red]Przewidywana klasyfikacja niezgodna z zadaną klasyfikacją![/]")
                        .Centered());
                
            var percent = Math.Round(integrityWithAssignedClass / currentTestedVector * 100, 2);
                
            AnsiConsole.Write(
                new Markup($"[bold yellow]Obecna poprawność klasyfikacji[/]: {integrityWithAssignedClass}/{currentTestedVector} ({percent.ToString()}%)")
                    .Centered()
                );
            AnsiConsole.Write(
                new Markup($"Sprawdzono [bold yellow]{currentTestedVector}[/]/{testSetCount.ToString()} wektorów")
                    .Centered()
                );
            
            AnsiConsole.Write(decisiveAttributeTable);
        });
        
        var finalIntegrity = integrityWithAssignedClass / currentTestedVector;
        
        var finalIntegrityPercent = Math.Round(finalIntegrity * 100, 2);
        
        // zapisujemy nowy zbiór testowy do lokalnego zbioru testowego, jeśli użytkownik tego chce
        if(appendToLocalTestSet)
            TrainSet.AddRange(testSet);

        for (var i = 0; i < 2; i++)
            Console.WriteLine();
        
        var finalTable = new Table()
            .Centered()
            .BorderStyle(Style.Parse("bold yellow"))
            .HeavyBorder()
            .Title(new TableTitle("Wyniki testowania"));

        if (checkIntegrityWithAssignedClass)
        {
            finalTable.AddColumn("Dane końcowe");
            finalTable.AddRow(new Markup($"[bold yellow]Końcowa poprawność klasyfikacji[/]: {integrityWithAssignedClass}/{currentTestedVector} ({finalIntegrityPercent.ToString()}%)")
                .Centered());
            
            AnsiConsole.Write(finalTable);
        }
            
        var trainingVectorsSplitByDecisiveAttribute = TrainSet.GroupBy(vector => vector.DecisiveAttributeName);
        var testVectorsSplitByDecisiveAttribute = testedVectors.GroupBy(vector => vector.givenClass);

        var showGraph = AnsiConsole
            .Confirm("Czy chcesz wyświetlić wykres rozłożenia wektorów testowych i treningowych w dwuwymiarowej przestrzeni dwóch wybranych cech?");
        
        if(!showGraph)
            return;
        
        ShowKnnTestingDataExternalWindow(savePath, trainingVectorsSplitByDecisiveAttribute, testVectorsSplitByDecisiveAttribute);
    }

    public void CheckForTestDataValidity(in IEnumerable<KnnVector<double>> knnVectors)
    {
        if (!knnVectors.Any())
            throw new Exception("Zbiór testowy nie może być pusty");

        var numOfPoints = knnVectors.First().NumberOfPoints;
        if (knnVectors.Any(vector => vector.NumberOfPoints != numOfPoints))
            throw new Exception("Liczba punktów we wszystkich wektorach musi być taka sama");
        
        if (knnVectors.Any(vector => vector.NumberOfPoints != TrainSet[0].NumberOfPoints))
            throw new Exception("Liczba punktów we wszystkich wektorach testowych musi być taka sama jak w zbiorze treningowym");

        if (knnVectors.Any(vector => vector.UnderlyingType != TrainSet[0].UnderlyingType))
            throw new Exception("Typy danych we wszystkich wektorach muszą być takie same");
    }
    
    public void ShowKnnTrainingData()
    {
        var columnNames = new List<string>();

        var inputData = TrainSet;
        var innerPoints = inputData.First().InnerPoints;

        for (var i = 0; i < innerPoints.Count; i++)
        {
            var columnName = ((VectorPointsAutoGeneratedNames)i).ToString();
            columnNames.Add(columnName);
        }
        columnNames.Add("Atrybut decyzyjny");
        
        var rows = inputData.Select(vector => vector.InnerPoints.Select(point => Math.Round(point, 2).ToString())
                .ToList()
                .Append(vector.DecisiveAttributeName))
            .Select(vectorRow => vectorRow.ToList())
            .ToList();

        new TableForm(columnNames, rows).ShowDialog();
    }

    public void ShowKnnTestingDataExternalWindow(string savePath, IEnumerable<IGrouping<string, KnnVector<double>>> trainingVectorsSplitByDecisiveAttribute,
        IEnumerable<IGrouping<string, (KnnVector<double> vector, string originalClass, string givenClass)>> testVectorsSplitByDecisiveAttribute)
    {
        AnsiConsole.Clear();
        Thread.Sleep(1000);
        
        var possibleAxes = new List<VectorPointsAutoGeneratedNames>();
        var numberOfPoints = TrainSet.First().NumberOfPoints;
        
        
        for (var i = 0; i < numberOfPoints; i++)
            possibleAxes.Add((VectorPointsAutoGeneratedNames)i);

        var xAxisEnumChoice = AnsiConsole.Prompt(
            new SelectionPrompt<VectorPointsAutoGeneratedNames>()
                .Title("Wybierz atrybut dla osi X")
                .PageSize(10)
                .AddChoices(possibleAxes)
        );
        
        AnsiConsole.Clear();
        Thread.Sleep(1000);
        
        var yAxisEnumChoice = AnsiConsole.Prompt(
            new SelectionPrompt<VectorPointsAutoGeneratedNames>()
                .Title("Wybierz atrybut dla osi Y")
                .PageSize(10)
                .AddChoices(possibleAxes)
        );

        while (xAxisEnumChoice == yAxisEnumChoice)
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold red]Wybrano ten sam atrybut dla osi X i Y![/]");
            yAxisEnumChoice = AnsiConsole.Prompt(
                new SelectionPrompt<VectorPointsAutoGeneratedNames>()
                    .Title($"Wybierz atrybuty dla osi Y (atrybut [bold yellow]{xAxisEnumChoice}[/] został już wybrany do osi X)")
                    .PageSize(10)
                    .AddChoices(possibleAxes)
            );
        }
        
        var xAxisInt = (int)xAxisEnumChoice;
        var yAxisInt = (int)yAxisEnumChoice;
        
        AnsiConsole.Clear();
        Thread.Sleep(1000);
        AnsiConsole.MarkupLine($"Wybrano atrybuty [bold yellow]{xAxisEnumChoice}[/] i [bold yellow]{yAxisEnumChoice}[/] dla osi X i Y");
        
        var mergeTrainingAndTestVectors = AnsiConsole.Confirm("Czy chcesz wyświetlić wektory treningowe i testowe o wspólnym atrybucie decyzyjnym razem [blue](y)[/] czy oddzielnie [red](n)[/]?");
        AnsiConsole.MarkupLine("Wyświetlanie wykresu...");
        
        var plt = new Plot(600, 400);
        plt.Title("Rozłożenie wektorów testowych w przestrzeni pierwszych dwóch cech (kolor - atrybut decyzyjny)");
        plt.Legend();
        plt.Style(ScottPlot.Style.Black);
        
        var markerSize = 15;
        
        if (mergeTrainingAndTestVectors)
        {
            // grupujemy wektory treningowe i testowe po atrybucie decyzyjnym
            // używając trainingVectorsSplitByDecisiveAttribute i testVectorsSplitByDecisiveAttribute
            
            var groupsHashMap = new Dictionary<string, List<KnnVector<double>>>();
            
            trainingVectorsSplitByDecisiveAttribute.ToList().ForEach(group =>
            {
                if(groupsHashMap.TryGetValue(group.Key, out List<KnnVector<double>> value))
                    value.AddRange(group.Select(vector => vector));
                else
                    groupsHashMap.Add(group.Key, group.Select(vector => vector).ToList());
            });
            
            testVectorsSplitByDecisiveAttribute.ToList().ForEach(group =>
            {
                if(groupsHashMap.TryGetValue(group.Key, out List<KnnVector<double>> value))
                    value.AddRange(group.Select(vector => vector.vector));
                else
                    groupsHashMap.Add(group.Key, group.Select(vector => vector.vector).ToList());
            });
            
            groupsHashMap.ToList().ForEach(group =>
            {
                plt.AddScatter(group.Value.Select(vector => vector[xAxisInt]).ToArray(), 
                    group.Value.Select(vector => vector[yAxisInt]).ToArray(), label: 
                    $"Wektory atrybutu decyzyjnego ({group.Key})", 
                    markerSize: markerSize,
                    lineWidth: 0);
            });
        }
        else
        {
            // grupujemy wektory treningowe po atrybucie decyzyjnym
            trainingVectorsSplitByDecisiveAttribute.ToList().ForEach(group =>
            {
                plt.AddScatter(group.Select(vector => vector[xAxisInt]).ToArray(), 
                    group.Select(vector => vector[yAxisInt]).ToArray(), label: 
                    $"Wektory treningowe atrybutu decyzyjnego ({group.First().DecisiveAttributeName})", 
                    markerSize: markerSize,
                    lineWidth: 0);
            });
        
            // grupujemy wektory testowe po przydzielony atrybucie decyzyjnym
            testVectorsSplitByDecisiveAttribute.ToList().ForEach(group =>
            {
                plt.AddScatter(group.Select(vector => vector.vector[xAxisInt]).ToArray(), 
                    group.Select(vector => vector.vector[yAxisInt]).ToArray(), 
                    label: $"Wektory testowe atrybutu decyzyjnego ({group.First().givenClass})", 
                    markerSize: markerSize,
                    lineWidth: 0);
            });

        }
        
        // wyświetlamy wykres
        
        var win = new FormsPlotViewer(plt);
        var exitButton = new Button
        {
            Text = "Zamknij",
            Dock = DockStyle.Bottom
        };
        
        var saveButton = new Button
        {
            Text = "Zapisz wykres (na razie nie działa)",
            Dock = DockStyle.Bottom
        };

        saveButton.Click += (_, _) => {
            var saveFilePath = Path.Combine(savePath, $"knn_plot_{DateTime.UtcNow.Date}.png");
            AnsiConsole.WriteLine($"Zapisywanie wykresu do pliku {saveFilePath}...");
            var savedPath = plt.SaveFig($"knnPlot.png", 1280, 720, false, 96);
            AnsiConsole.WriteLine("[green]Zapisano![/]");
            AnsiConsole.WriteLine($"Ścieżka do pliku: {savedPath}");
        };
        exitButton.Click += (_, _) => win.Close();
        win.Controls.Add(exitButton);
        win.Controls.Add(saveButton);
        win.ShowDialog();
    }
    
    public static double CalculateEuclideanDistance(KnnVector<double> vectorA, KnnVector<double> vectorB)
    {
        if (vectorA.NumberOfPoints != vectorB.NumberOfPoints)
            throw new ArgumentException("Wektory muszą mieć tę samą liczbę punktów!");
        
        var suma = 0.0;
        
        for (var i = 0; i < vectorA.NumberOfPoints; i++)
        {
            var roznica = vectorB[i] - vectorA[i];
            suma += roznica * roznica;
        }

        return Math.Sqrt(suma);
    }
}

