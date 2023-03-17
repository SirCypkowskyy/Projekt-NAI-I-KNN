using NAI.Projekt.KNN_ConsoleApp_s24759.Structures;
using ScottPlot;
using Spectre.Console;
using Style = Spectre.Console.Style;

namespace NAI.Projekt.KNN_ConsoleApp_s24759.Algorithms;

public class KNN
{
    public int Kvalue { get; private set; }
    public List<KnnVector<double>> TrainSet { get; private set; }
    public List<KnnVector<double>> TestSet { get; private set; }
    
    public bool IsModelReady => TrainSet.Count > 0;
    
    public static Style[] DecisiveAttributeAdditionalStyles = new []
    {
        Style.Parse("green"),
        Style.Parse("blue"),
        Style.Parse("magenta"),
        Style.Parse("cyan"),
        Style.Parse("magenta3_1")
    };
                
    public static Style[] ColumnStyles = new []
    {
        Style.Parse("aqua"),
        Style.Parse("hotpink3"),
        Style.Parse("teal"),
        Style.Parse("mediumpurple4"),
        Style.Parse("darkorange"),
    };

    public static string[] ColumnStylesNames = new[]
    {
        "aqua",
        "hotpink3",
        "teal",
        "mediumpurple4",
        "darkorange",
    }; 

    public KNN(int kValue, List<KnnVector<double>> trainSet, List<KnnVector<double>> testSet)
    {
        Kvalue = kValue;
        TrainSet = trainSet;
        TestSet = testSet;
        CheckForInputDataValidity();
    }

    private void CheckForInputDataValidity()
    {
        // Sprawdzamy poprawność danych wejściowych

        // Sprawdzenie dla K
        if (Kvalue < 1)
            throw new Exception("K musi być większe od 0");
        if (Kvalue > TrainSet.Count)
            throw new Exception("K nie może być większe od liczby wektorów uczących");

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
            
        return distances.OrderBy(element => element.Distance).Take(kValue);
    }
    
    public void TestData(in IEnumerable<KnnVector<double>> testSet, bool testTheWholeSet = false, bool checkIntegrityWithAssignedClass = false, bool appendToTestSet = false)
    {
        var testSetCount = testSet.Count();
        
        if(appendToTestSet)
            TestSet.AddRange(testSet);
        
        CheckForTestDataValidity();
        
        var integrityWithAssignedClass = 0.0;
        var currentTestedVector = 0;
        
        var testedVectors = new List<(KnnVector<double> vector, string originalClass, string givenClass)>();
        
        // jeśli testujemy cały zbiór, to sprawdzamy każdy wektor w TestSet wewnątrz klasy
        if (testTheWholeSet)
        {
            TestSet.ForEach(element =>
            {
                currentTestedVector++;
                AnsiConsole.MarkupLine($"Wektor [bold yellow]testowy[/]:\n[bold]{element}[/]");
                var kNearest = FindKNearest(element, TrainSet, Kvalue);
                AnsiConsole.MarkupLine($"{Kvalue} [bold yellow]najbliższych[/] wektorów:");
                kNearest.ToList().ForEach(knnVector => AnsiConsole.MarkupLine($"[bold]{knnVector}[/]"));
                
                if(!checkIntegrityWithAssignedClass) return;
                
                
                var mostPopularClass = kNearest.GroupBy(element => element.Vector.DecisiveAttributeName)
                    .OrderByDescending(group => group.Count())
                    .First()
                    .Key;
                
                testedVectors.Add((element, element.DecisiveAttributeName, mostPopularClass));

                
                if (mostPopularClass == element.DecisiveAttributeName)
                {
                    integrityWithAssignedClass++;
                    AnsiConsole.MarkupLine($"[bold green]Przewidywana klasyfikacja zgodna z zadaną klasyfikacją![/]");
                }
                else
                    AnsiConsole.MarkupLine($"[bold red]Przewidywana klasyfikacja niezgodna z zadaną klasyfikacją![/]");
                AnsiConsole.MarkupLine($"[bold yellow]Obecna poprawność klasyfikacji[/]: {integrityWithAssignedClass}/{currentTestedVector} ({Math.Round(integrityWithAssignedClass / currentTestedVector * 100, 2)}%)");
                AnsiConsole.MarkupLine($"Sprawdzono [bold yellow]{currentTestedVector}[/]/{TestSet.Count.ToString()} wektorów");
            });
        }
        else // jeśli nie, to sprawdzamy tylko testSet podany w argumencie
        {
            testSet.ToList().ForEach(element =>
            {
                currentTestedVector++;
                AnsiConsole.MarkupLine($"Wektor [bold yellow]testowy[/]:\n[bold]{element}[/]");
                var kNearest = FindKNearest(element, TrainSet, Kvalue);
                AnsiConsole.MarkupLine($"{Kvalue} [bold yellow]najbliższych[/] wektorów:");
                kNearest.ToList().ForEach(knnVector => AnsiConsole.MarkupLine($"[bold]{knnVector}[/]"));
                
                
                if(!checkIntegrityWithAssignedClass) return;
                
                
                var mostPopularClass = kNearest.GroupBy(element => element.Vector.DecisiveAttributeName)
                    .OrderByDescending(group => group.Count())
                    .First()
                    .Key;
                
                testedVectors.Add((element, element.DecisiveAttributeName, mostPopularClass));

                
                if (mostPopularClass == element.DecisiveAttributeName)
                {
                    integrityWithAssignedClass++;
                    AnsiConsole.MarkupLine($"[bold green]Przewidywana klasyfikacja zgodna z zadaną klasyfikacją![/]");
                }
                else
                    AnsiConsole.MarkupLine($"[bold red]Przewidywana klasyfikacja niezgodna z zadaną klasyfikacją![/]");
                AnsiConsole.MarkupLine($"[bold yellow]Obecna poprawność klasyfikacji[/]: {integrityWithAssignedClass}/{currentTestedVector} ({Math.Round(integrityWithAssignedClass / currentTestedVector * 100, 2)}%)");
                AnsiConsole.MarkupLine($"Sprawdzono [bold yellow]{currentTestedVector}[/]/{TestSet.Count.ToString()} wektorów");
            });
        }
        
        var finalIntegrity = integrityWithAssignedClass / TestSet.Count;
        
        if(checkIntegrityWithAssignedClass)
            AnsiConsole.MarkupLine($"[bold yellow]Końcowa poprawność klasyfikacji[/]: {integrityWithAssignedClass}/{TestSet.Count} ({Math.Round(finalIntegrity * 100, 2)}%)");
        
        var plt = new Plot(600, 400);
        plt.Title("Rozłożenie wektorów testowych w przestrzeni pierwszych dwóch cech");
        
        var colorBasedOnDecisiveAttribute = delegate(KnnVector<double> vector)
        {
            return vector.DecisiveAttributeName switch
            {
                "Iris-setosa" => System.Drawing.Color.Red,
                "Iris-versicolor" => System.Drawing.Color.Green,
                "Iris-virginica" => System.Drawing.Color.Blue,
                _ => System.Drawing.Color.Black
            };
        };
        
        var trainingVectorsSplitByDecisiveAttribute = TrainSet.GroupBy(vector => vector.DecisiveAttributeName);
        var testVectorsSplitByDecisiveAttribute = testedVectors.GroupBy(vector => vector.givenClass);
        
        trainingVectorsSplitByDecisiveAttribute.ToList().ForEach(group =>
        {
            plt.AddScatter(group.Select(vector => vector[0]).ToArray(), 
                group.Select(vector => vector[1]).ToArray(), label: 
                $"Wektory treningowe ({group.First().DecisiveAttributeName})", 
                markerSize: 20,
                lineWidth: 0);
        });
        
        testVectorsSplitByDecisiveAttribute.ToList().ForEach(group =>
        {
            plt.AddScatter(group.Select(vector => vector.vector[0]).ToArray(), 
                group.Select(vector => vector.vector[1]).ToArray(), 
                label: $"Sprawdzone wektory testowe ({group.First().givenClass})", 
                markerSize: 20,
                lineWidth: 0);
        });
        
        // plt.PlotScatter(testSet.Select(vector => vector[0]).ToArray(), 
        //     testSet.Select(vector => vector[1]).ToArray(), label: 
        //     "Wektory testowe", 
        //     markerSize: 1,
        //     lineWidth: 0,
        //     color: colorBasedOnDecisiveAttribute(),
        //     markerShape: MarkerShape.filledCircle);
        //
        // plt.AddScatter(TrainSet.Select(vector => vector[0]).ToArray(), 
        //     TrainSet.Select(vector => vector[1]).ToArray(), label: 
        //     "Wektory treningowe", 
        //     markerSize: 1,
        //     lineWidth: 0,
        //     color: System.Drawing.Color.Crimson,
        //     markerShape: MarkerShape.filledCircle);
        plt.Legend();
        var win = new ScottPlot.FormsPlotViewer(plt);
        win.ShowDialog();
    }

    public void CheckForTestDataValidity()
    {
        // Sprawdzenie dla danych testowych
        if (TestSet.Count == 0) return;
        {
            if (TestSet.Any(vector => vector.NumberOfPoints != TestSet[0].NumberOfPoints))
                throw new Exception("Liczba punktów we wszystkich wektorach musi być taka sama");

            if (TestSet.Any(vector => vector.UnderlyingType != TrainSet[0].UnderlyingType))
                throw new Exception("Typy danych we wszystkich wektorach muszą być takie same");
        }
    }
    
    public void ShowKnnModelLive()
    {
        var table = new Table().Centered()
            .BorderStyle(Style.Parse("red"))
            .Title(new TableTitle("Dane treningowe", new Style(Color.Yellow)));

        AnsiConsole.Live(table)
            .AutoClear(false)
            .Start(ctx =>
            {
                var inputData = TrainSet;
                var innerPoints = inputData.First().InnerPoints;

                for (var i = 0; i < innerPoints.Count; i++)
                {
                    var columnNameMarkup = new Markup(((VectorPointsAutoGeneratedNames) i).ToString(), ColumnStyles[i]);
                    table.AddColumn(new TableColumn(columnNameMarkup).Centered());
                    ctx.Refresh();
                    Thread.Sleep(500);
                }

                table.AddColumn("Atrybut decyzyjny");
                ctx.Refresh();
                Thread.Sleep(600);


                foreach (var vector in inputData)
                {
                    table.AddRow(
                        vector.InnerPoints
                        .Select(point => Math.Round(point, 2).ToString())
                        .ToList()
                        .Append(vector.DecisiveAttributeName)
                        .ToArray()
                        );
     
                    ctx.Refresh();
                    Thread.Sleep(150);
                }
            });
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

