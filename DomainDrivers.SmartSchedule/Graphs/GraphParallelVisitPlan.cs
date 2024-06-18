namespace DomainDrivers.SmartSchedule.Graphs;

public class GraphParallelVisitPlan
{
    private readonly Dictionary<int, List<int>> _verticesByStageNumber = new();

    public void AddParallelStage(int vertex, int stageNumber)
    {
        if (!_verticesByStageNumber.TryGetValue(stageNumber, out var vertices))
        {
            vertices = new List<int>();
            _verticesByStageNumber[stageNumber] = vertices;
        }

        vertices.Add(vertex);
    }

    public List<List<int>> GetVerticesVisitedInParallel()
    {
        var orderedByStageNumber = _verticesByStageNumber.OrderBy(x => x.Key);

        return orderedByStageNumber
            .Select(x => x.Value)
            .ToList();
    }
}
