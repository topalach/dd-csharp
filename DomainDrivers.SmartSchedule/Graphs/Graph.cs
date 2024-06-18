namespace DomainDrivers.SmartSchedule.Graphs;

public class Graph
{
    private readonly AdjacencyList _adjacencyList = new();

    public void AddEdge(int source, int destination) => _adjacencyList.AddEdge(source, destination);

    public List<List<int>> GetVerticesVisitedInParallel()
    {
        var parallelVisitPlan = new GraphParallelVisitPlan();
        int? nextUnvisitedVertex;

        while ((nextUnvisitedVertex = _adjacencyList.GetNextUnvisitedVertex()) is not null)
        {
            VisitConnectedVertices(parallelVisitPlan, vertex: nextUnvisitedVertex.Value, stageNumber: 1);
        }

        return parallelVisitPlan.GetVerticesVisitedInParallel();
    }

    private void VisitConnectedVertices(GraphParallelVisitPlan parallelVisitPlan, int vertex, int stageNumber)
    {
        _adjacencyList.VisitWithoutCycles(vertex);
        parallelVisitPlan.AddParallelStage(vertex, stageNumber);

        var destinations = _adjacencyList.GetDestinations(vertex);

        foreach (var destination in destinations)
        {
            VisitConnectedVertices(parallelVisitPlan, destination, stageNumber: stageNumber + 1);
        }
    }

    public void AddVertex(int vertex)
    {
        _adjacencyList.AddVertex(vertex);
    }
}
