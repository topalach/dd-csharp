using DomainDrivers.SmartSchedule.Planning.Parallelization;

namespace DomainDrivers.SmartSchedule.Graphs;

public class VerticesMap
{
    private readonly Dictionary<int, Vertex> _vertices = new();

    private int GetNextIndex() => _vertices.Count;

    public void AddStage(Stage stage)
    {
        var vertex = new Vertex(Index: GetNextIndex(), stage);
        _vertices[vertex.Index] = vertex;
    }

    public Stage GetStage(int index) => _vertices[index].Stage;

    public int GetStageIndex(Stage stage) => _vertices.First(x => x.Value.Stage == stage).Key;
}
