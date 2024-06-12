using Unite.Data.Entities.Genome;
using Unite.Data.Entities.Genome.Analysis.Dna;
using Unite.Data.Entities.Genome.Analysis.Dna.Enums;
using Unite.Data.Helpers.Genome.Dna.Ssm;
using Unite.Essentials.Extensions;
using Unite.Indices.Entities.Basic.Genome;
using Unite.Indices.Entities.Basic.Genome.Dna;

using SSM = Unite.Data.Entities.Genome.Analysis.Dna.Ssm;
using CNV = Unite.Data.Entities.Genome.Analysis.Dna.Cnv;
using SV = Unite.Data.Entities.Genome.Analysis.Dna.Sv;

namespace Unite.Mapping;

public class VariantIndexMapper
{
    /// <summary>
    /// Creates an index from the entity. Returns null if entity is null.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <typeparam name="T">Type of the index.</typeparam>
    /// <returns>Index created from the entity.</returns>
    public static T CreateFrom<T>(in Variant entity) where T : VariantIndex, new()
    {
        if (entity == null)
        {
            return null;
        }

        var index = new T();

        Map(entity, index);

        return index;
    }

    /// <summary>
    /// Maps entity to index. Does nothing if either entity or index is null.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <param name="index">Index.</param>
    public static void Map(in Variant entity, VariantIndex index)
    {
        if (entity == null)
        {
            return;
        }

        if (entity is SSM.Variant ssm)
        {
            Map(ssm, index);
        }
        else if (entity is CNV.Variant cnv)
        {
            Map(cnv, index);
        }
        else if (entity is SV.Variant sv)
        {
            Map(sv, index);
        }
    }

    /// <summary>
    /// Maps entity to index. Does nothing if either entity or index is null.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <param name="index">Index.</param>
    public static void Map(in SSM.Variant entity, VariantIndex index)
    {
        if (entity == null)
        {
            return;
        }

        index.Id = $"{VariantType.SSM.ToDefinitionString()}{entity.Id}";
        index.Type = VariantType.SSM.ToDefinitionString();
        index.Ssm = CreateFrom(entity);
    }

    /// <summary>
    /// Maps entity to index. Does nothing if either entity or index is null.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <param name="index">Index.</param>
    public static void Map(in CNV.Variant entity, VariantIndex index)
    {
        if (entity == null)
        {
            return;
        }

        index.Id = $"{VariantType.CNV.ToDefinitionString()}{entity.Id}";
        index.Type = VariantType.CNV.ToDefinitionString();
        index.Cnv = CreateFrom(entity);
    }

    /// <summary>
    /// Maps entity to index. Does nothing if either entity or index is null.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <param name="index">Index.</param>
    public static void Map(in SV.Variant entity, VariantIndex index)
    {
        if (entity == null)
        {
            return;
        }

        index.Id = $"{VariantType.SV.ToDefinitionString()}{entity.Id}";
        index.Type = VariantType.SV.ToDefinitionString();
        index.Sv = CreateFrom(entity);
    }


    private static SsmIndex CreateFrom(in SSM.Variant entity)
    {
        if (entity == null)
        {
            return null;
        }

        return new SsmIndex
        {
            Id = $"{VariantType.SSM.ToDefinitionString()}{entity.Id}",
            Chromosome = entity.ChromosomeId.ToDefinitionString(),
            Start = entity.Start,
            End = entity.End,
            Length = entity.Length.Value,
            Type = entity.TypeId.ToDefinitionString(),
            Ref = entity.Ref,
            Alt = entity.Alt,

            AffectedFeatures = CreateFrom(entity.AffectedTranscripts)
        };
    }

    private static CnvIndex CreateFrom(in CNV.Variant entity)
    {
        if (entity == null)
        {
            return null;
        }

        return new CnvIndex
        {
            Id = $"{VariantType.CNV.ToDefinitionString()}{entity.Id}",
            Chromosome = entity.ChromosomeId.ToDefinitionString(),
            Start = entity.Start,
            End = entity.End,
            Length = entity.Length.Value,
            Type = entity.TypeId.ToDefinitionString(),
            Loh = entity.Loh,
            Del = entity.Del,
            C1Mean = entity.C1Mean,
            C2Mean = entity.C2Mean,
            TcnMean = entity.TcnMean,
            C1 = entity.C1,
            C2 = entity.C2,
            Tcn = entity.Tcn,

            AffectedFeatures = CreateFrom(entity.AffectedTranscripts)
        };
    }

    private static SvIndex CreateFrom(in SV.Variant entity)
    {
        if (entity == null)
        {
            return null;
        }

        return new SvIndex
        {
            Id = $"{VariantType.SV.ToDefinitionString()}{entity.Id}",
            Chromosome = entity.ChromosomeId.ToDefinitionString(),
            Start = entity.Start,
            End = entity.End,
            OtherChromosome = entity.OtherChromosomeId.ToDefinitionString(),
            OtherStart = entity.OtherStart,
            OtherEnd = entity.OtherEnd,
            Length = entity.Length,
            Type = entity.TypeId.ToDefinitionString(),
            Inverted = entity.Inverted,

            AffectedFeatures = CreateFrom(entity.AffectedTranscripts)
        };
    }

    private static AffectedFeatureIndex[] CreateFrom(in IEnumerable<SSM.AffectedTranscript> entities)
    {
        if (entities?.Any() != true)
        {
            return null;
        }

        return entities.Select(entity =>
        {
            return new AffectedFeatureIndex
            {
                Gene = CreateFrom(entity.Feature?.Gene),
                Transcript = CreateFrom(entity),
                Effects = CreateFrom(entity.Effects)
            };

        }).ToArray();
    }

    private static AffectedFeatureIndex[] CreateFrom(in IEnumerable<CNV.AffectedTranscript> entities)
    {
        if (entities?.Any() != true)
        {
            return null;
        }

        return entities.Select(entity =>
        {
            return new AffectedFeatureIndex
            {
                Gene = CreateFrom(entity.Feature?.Gene),
                Transcript = CreateFrom(entity),
                Effects = CreateFrom(entity.Effects)
            };

        }).ToArray();
    }

    private static AffectedFeatureIndex[] CreateFrom(in IEnumerable<SV.AffectedTranscript> entities)
    {
        if (entities?.Any() != true)
        {
            return null;
        }

        return entities.Select(entity =>
        {
            return new AffectedFeatureIndex
            {
                Gene = CreateFrom(entity.Feature?.Gene),
                Transcript = CreateFrom(entity),
                Effects = CreateFrom(entity.Effects)
            };

        }).ToArray();
    }

    private static AffectedTranscriptIndex CreateFrom(in SSM.AffectedTranscript entity)
    {
        if (entity == null)
        {
            return null;
        }

        return new AffectedTranscriptIndex
        {
            Feature = CreateFrom(entity.Feature),
            Distance = entity.Distance,
            ProteinChange = ProteinChangeCodeGenerator.Generate(entity.ProteinStart, entity.ProteinEnd, entity.ProteinChange),
            CodonChange = CodonChangeCodeGenerator.Generate(entity.CDSStart, entity.CDSEnd, entity.CodonChange)
        };
    }

    private static AffectedTranscriptIndex CreateFrom(in CNV.AffectedTranscript entity)
    {
        if (entity == null)
        {
            return null;
        }

        return new AffectedTranscriptIndex
        {
            Feature = CreateFrom(entity.Feature),
            OverlapBpNumber = entity.OverlapBpNumber,
            OverlapPercentage = entity.OverlapPercentage,
            Distance = entity.Distance
        };
    }

    private static AffectedTranscriptIndex CreateFrom(in SV.AffectedTranscript entity)
    {
        if (entity == null)
        {
            return null;
        }

        return new AffectedTranscriptIndex
        {
            Feature = CreateFrom(entity.Feature),
            OverlapBpNumber = entity.OverlapBpNumber,
            OverlapPercentage = entity.OverlapPercentage,
            Distance = entity.Distance
        };
    }

    private static EffectIndex[] CreateFrom(in IEnumerable<Effect> entities)
    {
        if (entities?.Any() != true)
        {
            return null;
        }

        return entities.Select(entity =>
        {
            return new EffectIndex
            {
                Type = entity.Type,
                Impact = entity.Impact,
                Severity = entity.Severity
            };

        }).ToArray();
    }

    private static GeneIndex CreateFrom(in Gene entity)
    {
        return GeneIndexMapper.CreateFrom<GeneIndex>(entity);
    }

    private static TranscriptIndex CreateFrom(in Transcript entity)
    {
        if (entity == null)
        {
            return null;
        }

        return new TranscriptIndex
        {
            Id = entity.Id,
            StableId = entity.StableId,
            Symbol = entity.Symbol,
            Description = entity.Description,
            Biotype = entity.Biotype,
            IsCanonical = entity.IsCanonical,
            Chromosome = entity.ChromosomeId.ToDefinitionString(),
            Start = entity.Start,
            End = entity.End,
            Strand = entity.Strand,
            ExonicLength = entity.ExonicLength,

            Protein = CreateFrom(entity.Protein)
        };
    }

    private static ProteinIndex CreateFrom(in Protein entity)
    {
        if (entity == null)
        {
            return null;
        }

        return new ProteinIndex
        {
            Id = entity.Id,
            StableId = entity.StableId,
            IsCanonical = entity.IsCanonical,
            Start = entity.Start,
            End = entity.End,
            Length = entity.Length
        };
    }
}
