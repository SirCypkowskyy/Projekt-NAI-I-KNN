using System.Globalization;
using System.Text.RegularExpressions;
using NAI.Projekt.KNN_ConsoleApp_s24759.Algorithms;
using NAI.Projekt.KNN_ConsoleApp_s24759.Structures;
using NAI.Projekt.KNN_ConsoleApp_s24759.Views;
using Spectre.Console;
using Color = Spectre.Console.Color;

namespace NAI.Projekt.KNN_ConsoleApp_s24759;

static class Program   
{
    public static KnnExecutor KnnExecutor { get; set; }
    
    static void Main(string[] args)
    {
        var visualizer = new ViewsVisualizer(args);
    }


    public static void InitKnnExecutor(string inputFilePath, string outputFolderPath, int k)
    {
        var txtIrisData = File.ReadAllText(inputFilePath);
        var irisDataRows = txtIrisData.Split('\n');


        var irisData = (from row in irisDataRows
            select row.Split(',')
            into rowValues
            let rowPointValuesDouble = rowValues.Take(rowValues.Length - 1)
                .Select(element => double.Parse(element, NumberStyles.Any, CultureInfo.InvariantCulture))
            select new KnnVector<double>(rowPointValuesDouble, rowValues.Last())).ToList();


        KnnExecutor = new KnnExecutor(
            new KNN(k, irisData, new List<KnnVector<double>>()),
            outputFolderPath
        );
        
        KnnExecutor.ShowInputData();
    }

    public static void InitKnnTesting(string testedDataFilePath)
    {
        var txtIrisData = File.ReadAllText(testedDataFilePath);
        var irisDataRows = txtIrisData.Split('\n');

        var irisDataToTest = (from row in irisDataRows
                select row.Split(',')
                into rowValues
                let rowPointValuesDouble = rowValues.Take(rowValues.Length - 1)
                    .Select(element => double.Parse(element, NumberStyles.Any, CultureInfo.InvariantCulture))
                let decisiveAttribute = rowValues.Last() != rowPointValuesDouble.Last().ToString()
                    ? rowValues.Last()
                    : null
                select (decisiveAttribute is null
                        ? new KnnVector<double>(rowPointValuesDouble, isTrainingVector: true)
                        : new KnnVector<double>(rowPointValuesDouble, decisiveAttribute, isTrainingVector: true)
                    )
            );

        KnnExecutor.TestData(in irisDataToTest);
    }
    
    
    public static bool IsKnnModelCreated()
    {
        return KnnExecutor is {Knn.TrainSet.Count: > 0};
    }
    
}