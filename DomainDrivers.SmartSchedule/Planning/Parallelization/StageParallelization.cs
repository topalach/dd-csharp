using DomainDrivers.SmartSchedule.Exceptions;
using DomainDrivers.SmartSchedule.Graphs;

namespace DomainDrivers.SmartSchedule.Planning.Parallelization;

public class StageParallelization
{
    public ParallelStagesList Of(ISet<Stage> stages)
    {
        var verticesMap = new VerticesMap();
        var graph = new Graph();

        foreach (var stage in stages)
        {
            verticesMap.AddStage(stage);
            graph.AddVertex(verticesMap.GetStageIndex(stage));
        }

        foreach (var stage in stages)
        {
            var dependencies = stage.Dependencies;

            foreach (var dependency in dependencies)
            {
                graph.AddEdge(
                    source: verticesMap.GetStageIndex(dependency),
                    destination: verticesMap.GetStageIndex(stage));
            }
        }

        var parallelStagesList = new List<ParallelStages>();

        List<List<int>> parallelStagesInGraph;

        try
        {
            parallelStagesInGraph = graph.GetVerticesVisitedInParallel();
        }
        catch (GraphCycleDetectedException)
        {
            return ParallelStagesList.Empty();
        }

        foreach (var parallelVertices in parallelStagesInGraph)
        {
            var stagesInParallelStep = parallelVertices.Select(x => verticesMap.GetStage(x)).ToHashSet();

            var parallelStages = new ParallelStages(stagesInParallelStep);
            parallelStagesList.Add(parallelStages);
        }

        return new ParallelStagesList(parallelStagesList);
    }
}
