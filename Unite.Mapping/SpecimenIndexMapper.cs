using Unite.Data.Entities.Specimens;
using Unite.Essentials.Extensions;
using Unite.Indices.Entities.Basic.Specimens;

using OrganoidIntervention = Unite.Data.Entities.Specimens.Organoids.Intervention;
using XenograftIntervention = Unite.Data.Entities.Specimens.Xenografts.Intervention;

namespace Unite.Mapping;


public class SpecimenIndexMapper
{
    /// <summary>
    /// Creates an index from the entity. Returns null if entity is null.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <param name="diagnosisDate">Diagnosis date (anchor date for calculation of relative days).</param>
    /// <typeparam name="T">Type of the index.</typeparam>
    /// <returns>Index created from the entity.</returns>
    public static T CreateFrom<T>(in Specimen entity, DateOnly? diagnosisDate) where T : SpecimenIndex, new()
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
    public static void Map(in Specimen entity, SpecimenIndex index, DateOnly? diagnosisDate)
    {
        if (entity == null || index == null)
        {
            return;
        }

        index.Id = entity.Id;
        index.ReferenceId = entity.ReferenceId;
        index.Type = entity.TypeId.ToDefinitionString();

        index.Tissue = CreateFromTissue(entity, diagnosisDate);
        index.Cell = CreateFromCellLine(entity, diagnosisDate);
        index.Organoid = CreateFromOrganoid(entity, diagnosisDate);
        index.Xenograft = CreateFromXenograft(entity, diagnosisDate);
    }


    private static TissueIndex CreateFromTissue(in Specimen entity, DateOnly? diagnosisDate)
    {
        if (entity.Tissue == null)
        {
            return null;
        }

        return new TissueIndex
        {
            Id = entity.Id,
            ReferenceId = entity.Tissue.ReferenceId,
            CreationDay = entity.CreationDay ?? entity.CreationDate?.RelativeFrom(diagnosisDate),

            Type = entity.Tissue.TypeId?.ToDefinitionString(),
            TumorType = entity.Tissue.TumorTypeId?.ToDefinitionString(),
            Source = entity.Tissue.Source?.Value,

            MolecularData = CreateFrom(entity.MolecularData)
        };
    }

    private static CellLineIndex CreateFromCellLine(in Specimen entity, DateOnly? diagnosisDate)
    {
        if (entity.CellLine == null)
        {
            return null;
        }

        return new CellLineIndex
        {
            Id = entity.Id,
            ReferenceId = entity.CellLine.ReferenceId,
            CreationDay = entity.CreationDay ?? entity.CreationDate?.RelativeFrom(diagnosisDate),

            Species = entity.CellLine.SpeciesId?.ToDefinitionString(),
            Type = entity.CellLine.TypeId?.ToDefinitionString(),
            CultureType = entity.CellLine.CultureTypeId?.ToDefinitionString(),

            Name = entity.CellLine.Info?.Name,
            DepositorName = entity.CellLine.Info?.DepositorName,
            DepositorEstablishment = entity.CellLine.Info?.DepositorEstablishment,
            EstablishmentDate = entity.CellLine.Info?.EstablishmentDate,

            PubMedLink = entity.CellLine.Info?.PubMedLink,
            AtccLink = entity.CellLine.Info?.AtccLink,
            ExPasyLink = entity.CellLine.Info?.ExPasyLink,

            MolecularData = CreateFrom(entity.MolecularData),
            DrugScreenings = CreateFrom(entity.DrugScreenings)
        };
    }

    private static OrganoidIndex CreateFromOrganoid(in Specimen entity, DateOnly? diagnosisDate)
    {
        if (entity.Organoid == null)
        {
            return null;
        }

        return new OrganoidIndex
        {
            Id = entity.Id,
            ReferenceId = entity.Organoid.ReferenceId,
            CreationDay = entity.CreationDay ?? entity.CreationDate?.RelativeFrom(diagnosisDate),

            ImplantedCellsNumber = entity.Organoid.ImplantedCellsNumber,
            Tumorigenicity = entity.Organoid.Tumorigenicity,
            Medium = entity.Organoid.Medium,

            MolecularData = CreateFrom(entity.MolecularData),
            DrugScreenings = CreateFrom(entity.DrugScreenings),
            Interventions = CreateFrom(entity.Organoid.Interventions, diagnosisDate)
        };
    }

    private static OrganoidInterventionIndex[] CreateFrom(in IEnumerable<OrganoidIntervention> entities, DateOnly? diagnosisDate)
    {
        if (entities?.Any() != true)
        {
            return null;
        }

        return entities.Select(entity =>
        {
            return new OrganoidInterventionIndex
            {
                Type = entity.Type.Name,
                Details = entity.Details,
                StartDay = entity.StartDay ?? entity.StartDate?.RelativeFrom(diagnosisDate),
                DurationDays = entity.DurationDays ?? entity.EndDate?.RelativeFrom(entity.StartDate),
                Results = entity.Results
            };

        }).ToArray();
    }

    private static XenograftIndex CreateFromXenograft(in Specimen entity, DateOnly? diagnosisDate)
    {
        if (entity.Xenograft == null)
        {
            return null;
        }

        return new XenograftIndex
        {
            Id = entity.Id,
            ReferenceId = entity.Xenograft.ReferenceId,
            CreationDay = entity.CreationDay ?? entity.CreationDate?.RelativeFrom(diagnosisDate),

            MouseStrain = entity.Xenograft.MouseStrain,
            GroupSize = entity.Xenograft.GroupSize,
            ImplantType = entity.Xenograft.ImplantTypeId?.ToDefinitionString(),
            TissueLocation = entity.Xenograft.TissueLocationId?.ToDefinitionString(),
            ImplantedCellsNumber = entity.Xenograft.ImplantedCellsNumber,
            Tumorigenicity = entity.Xenograft.Tumorigenicity,
            TumorGrowthForm = entity.Xenograft.TumorGrowthFormId?.ToDefinitionString(),
            SurvivalDaysFrom = entity.Xenograft.SurvivalDaysFrom,
            SurvivalDaysTo = entity.Xenograft.SurvivalDaysTo,

            MolecularData = CreateFrom(entity.MolecularData),
            DrugScreenings = CreateFrom(entity.DrugScreenings),
            Interventions = CreateFrom(entity.Xenograft.Interventions, diagnosisDate)
        };
    }

    private static XenograftInterventionIndex[] CreateFrom(in IEnumerable<XenograftIntervention> entities, DateOnly? diagnosisDate)
    {
        if (entities?.Any() != true)
        {
            return null;
        }

        return entities.Select(entity =>
        {
            return new XenograftInterventionIndex
            {
                Type = entity.Type.Name,
                Details = entity.Details,
                StartDay = entity.StartDay ?? entity.StartDate?.RelativeFrom(diagnosisDate),
                DurationDays = entity.DurationDays ?? entity.EndDate?.RelativeFrom(entity.StartDate),
                Results = entity.Results
            };

        }).ToArray();
    }

    private static MolecularDataIndex CreateFrom(in MolecularData entity)
    {
        if (entity == null)
        {
            return null;
        }

        return new MolecularDataIndex
        {
            MgmtStatus = entity.MgmtStatusId?.ToDefinitionString(),
            IdhStatus = entity.IdhStatusId?.ToDefinitionString(),
            IdhMutation = entity.IdhMutationId?.ToDefinitionString(),
            GeneExpressionSubtype = entity.GeneExpressionSubtypeId?.ToDefinitionString(),
            MethylationSubtype = entity.MethylationSubtypeId?.ToDefinitionString(),
            GcimpMethylation = entity.GcimpMethylation
        };
    }

    private static DrugScreeningIndex[] CreateFrom(in IEnumerable<DrugScreening> entities)
    {
        if (entities?.Any() != true)
        {
            return null;
        }

        return entities.Select(entity =>
        {
            return new DrugScreeningIndex
            {
                Dss = entity.Dss,
                DssSelective = entity.DssSelective,
                Gof = entity.Gof,
                Drug = entity.Drug.Name,
                MinConcentration = entity.MinConcentration,
                MaxConcentration = entity.MaxConcentration,
                AbsIC25 = entity.AbsIC25,
                AbsIC50 = entity.AbsIC50,
                AbsIC75 = entity.AbsIC75
            };

        }).ToArray();
    }
}
