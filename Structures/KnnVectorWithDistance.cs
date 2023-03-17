namespace NAI.Projekt.KNN_ConsoleApp_s24759.Structures;

public class KnnVectorWithDistance<T>
{
    public KnnVector<T> Vector { get; set; }
    public double Distance { get; set; }

    public KnnVectorWithDistance(KnnVector<T> vector, double distance)
    {
        Vector = vector;
        Distance = distance;
    }
    
    public override string ToString()
    {
        return $"Wektor: {Vector}:\nDystans: {Math.Round(Distance, 2)}";
    }
    
}