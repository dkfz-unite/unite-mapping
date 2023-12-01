using Unite.Data.Entities.Genome.Analysis;
using Unite.Essentials.Extensions;
using Unite.Indices.Entities.Basic.Analysis;

namespace Unite.Mapping;

public class AnalysisIndexMapper
{
    /// <summary>
    /// Creates an index from the entity. Returns null if entity is null.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <param name="diagnosisDate">Diagnosis date (anchor date for calculation of relative days).</param>
    /// <typeparam name="T">Type of the index.</typeparam>
    /// <returns>Index created from the entity.</returns>
    public static T CreateFrom<T>(in AnalysedSample entity, DateOnly? diagnosisDate) where T : AnalysisIndex, new()
    {
        if (entity == null)
        {
            return null;
        }

        var index = new T();

        Map(entity, index, diagnosisDate);

        return index;
    }

    /// <summary>
    /// Maps entity to index. Does nothing if either entity or index is null.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <param name="index">Index.</param>
    /// <param name="diagnosisDate">Diagnosis date (anchor date for calculation of relative days).</param>
    public static void Map(in AnalysedSample entity, AnalysisIndex index, DateOnly? diagnosisDate)
    {
        if (entity == null || index == null)
        {
            return;
        }

        index.Id = entity.Analysis.Id;
        index.ReferenceId = entity.Analysis.ReferenceId;
        index.Type = entity.Analysis.TypeId.ToDefinitionString();
        index.Day = entity.Analysis.Day ?? entity.Analysis.Date?.RelativeFrom(diagnosisDate);
        index.Purity = entity.Purity;
        index.Ploidy = entity.Ploidy;
    }
}
