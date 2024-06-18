using DomainDrivers.SmartSchedule.Exceptions;

namespace DomainDrivers.SmartSchedule.Graphs;

public class AdjacencyList
{
    private readonly Dictionary<int, List<int>> _adjacencyList = new();
    private readonly HashSet<int> _visitedVertices = [];

    public void AddVertex(int vertex)
    {
        if (!_adjacencyList.ContainsKey(vertex))
        {
            _adjacencyList[vertex] = [];
        }
    }

    public void AddEdge(int source, int destination)
    {
        if (!_adjacencyList.ContainsKey(source))
        {
            _adjacencyList[source] = [];
        }

        _adjacencyList[source].Add(destination);
    }

    public int[] GetDestinations(int source) =>
        !_adjacencyList.TryGetValue(source, out var value)
            ? []
            : value.ToArray();

    public void VisitWithoutCycles(params int[] vertices)
    {
        foreach (var vertex in vertices)
        {
            if (!_visitedVertices.Add(vertex))
            {
                throw new GraphCycleDetectedException($"Vertex {vertex} was already visited.");
            }
        }
    }

    private bool IsVisited(int vertex) => _visitedVertices.Contains(vertex);

    public int? GetNextUnvisitedVertex()
    {
        var unvisitedVertices = _adjacencyList
            .Where(x => !IsVisited(x.Key))
            .Select(x => x.Key)
            .ToList();

        if (unvisitedVertices.Count == 0)
        {
            return null;
        }

        return unvisitedVertices.First();
    }
}
