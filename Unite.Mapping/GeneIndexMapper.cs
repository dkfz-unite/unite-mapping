using Unite.Data.Entities.Genome;
using Unite.Data.Extensions;
using Unite.Indices.Entities.Basic.Genome;

namespace Unite.Mapping;

public class GeneIndexMapper
{
    /// <summary>
    /// Maps entity to index. Does nothing if either entity or index is null.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <param name="index">Index.</param>
    public static void Map(in Gene entity, GeneIndex index)
    {
        if (entity == null || index == null)
        {
            return;
        }
        
        index.Id = entity.Id;
        index.StableId = entity.StableId;
        index.Symbol = entity.Symbol;
        index.Description = entity.Description;
        index.Biotype = entity.Biotype;
        index.Chromosome = entity.ChromosomeId.ToDefinitionString();
        index.Start = entity.Start;
        index.End = entity.End;
        index.Strand = entity.Strand;
        index.ExonicLength = entity.ExonicLength;
    }
}
