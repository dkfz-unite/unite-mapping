﻿using Unite.Data.Entities.Genome;
using Unite.Data.Entities.Genome.Variants;
using Unite.Data.Extensions;
using Unite.Data.Utilities.Mutations;
using Unite.Indices.Entities.Basic.Genome;
using Unite.Indices.Entities.Basic.Genome.Variants;
using Unite.Mapping;

using CNV = Unite.Data.Entities.Genome.Variants.CNV;
using SSM = Unite.Data.Entities.Genome.Variants.SSM;
using SV = Unite.Data.Entities.Genome.Variants.SV;

namespace Unite.Genome.Indices.Services.Mappers;

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

        if (entity is SSM.Variant mutation)
        {
            Map(mutation, index);
        }
        else if (entity is CNV.Variant copyNumberVariant)
        {
            Map(copyNumberVariant, index);
        }
        else if (entity is SV.Variant structuralVariant)
        {
            Map(structuralVariant, index);
        }

        // index,Id - resolved by property getter
        // index.Type - resolved by property getter
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

        index.Sv = CreateFrom(entity);
    }


    private static MutationIndex CreateFrom(in SSM.Variant entity)
    {
        if (entity == null)
        {
            return null;
        }

        return new MutationIndex
        {
            Id = entity.Id,
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

    private static CopyNumberVariantIndex CreateFrom(in CNV.Variant entity)
    {
        if (entity == null)
        {
            return null;
        }

        return new CopyNumberVariantIndex
        {
            Id = entity.Id,
            Chromosome = entity.ChromosomeId.ToDefinitionString(),
            Start = entity.Start,
            End = entity.End,
            Length = entity.Length.Value,
            Type = entity.TypeId.ToDefinitionString(),
            Loh = entity.Loh,
            HomoDel = entity.HomoDel,
            C1Mean = entity.C1Mean,
            C2Mean = entity.C2Mean,
            TcnMean = entity.TcnMean,
            C1 = entity.C1,
            C2 = entity.C2,
            Tcn = entity.Tcn,

            AffectedFeatures = CreateFrom(entity.AffectedTranscripts)
        };
    }

    private static StructuralVariantIndex CreateFrom(in SV.Variant entity)
    {
        if (entity == null)
        {
            return null;
        }

        return new StructuralVariantIndex
        {
            Id = entity.Id,
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
                Consequences = CreateFrom(entity.Consequences)
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
                Consequences = CreateFrom(entity.Consequences)
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
                Consequences = CreateFrom(entity.Consequences)
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
            AminoAcidChange = AAChangeCodeGenerator.Generate(entity.ProteinStart, entity.ProteinEnd, entity.AminoAcidChange),
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

    private static ConsequenceIndex[] CreateFrom(in IEnumerable<Consequence> entities)
    {
        if (entities?.Any() != true)
        {
            return null;
        }

        return entities.Select(entity =>
        {
            return new ConsequenceIndex
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
