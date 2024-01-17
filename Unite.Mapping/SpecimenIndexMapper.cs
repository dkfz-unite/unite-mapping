using Unite.Data.Entities.Specimens;
using Unite.Essentials.Extensions;
using Unite.Indices.Entities.Basic.Specimens;

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

        index.Material = CreateFromMaterial(entity, diagnosisDate);
        index.Line = CreateFromLine(entity, diagnosisDate);
        index.Organoid = CreateFromOrganoid(entity, diagnosisDate);
        index.Xenograft = CreateFromXenograft(entity, diagnosisDate);
    }


    private static MaterialIndex CreateFromMaterial(in Specimen entity, DateOnly? diagnosisDate)
    {
        if (entity.Material == null)
        {
            return null;
        }

        return new MaterialIndex
        {
            Id = entity.Id,
            ReferenceId = entity.Material.ReferenceId,
            CreationDay = entity.CreationDay ?? entity.CreationDate?.RelativeFrom(diagnosisDate),

            Type = entity.Material.TypeId?.ToDefinitionString(),
            TumorType = entity.Material.TumorTypeId?.ToDefinitionString(),
            Source = entity.Material.Source?.Value,

            MolecularData = CreateFrom(entity.MolecularData)
        };
    }

    private static LineIndex CreateFromLine(in Specimen entity, DateOnly? diagnosisDate)
    {
        if (entity.Line == null)
        {
            return null;
        }

        return new LineIndex
        {
            Id = entity.Id,
            ReferenceId = entity.Line.ReferenceId,
            CreationDay = entity.CreationDay ?? entity.CreationDate?.RelativeFrom(diagnosisDate),

            CellsSpecies = entity.Line.CellsSpeciesId?.ToDefinitionString(),
            CellsType = entity.Line.CellsTypeId?.ToDefinitionString(),
            CellsCultureType = entity.Line.CellsCultureTypeId?.ToDefinitionString(),

            Name = entity.Line.Info?.Name,
            DepositorName = entity.Line.Info?.DepositorName,
            DepositorEstablishment = entity.Line.Info?.DepositorEstablishment,
            EstablishmentDate = entity.Line.Info?.EstablishmentDate,

            PubMedLink = entity.Line.Info?.PubMedLink,
            AtccLink = entity.Line.Info?.AtccLink,
            ExPasyLink = entity.Line.Info?.ExPasyLink,

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
            Interventions = CreateFrom(entity.Interventions, diagnosisDate),
            DrugScreenings = CreateFrom(entity.DrugScreenings)
        };
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
            ImplantLocation = entity.Xenograft.ImplantLocationId?.ToDefinitionString(),
            ImplantedCellsNumber = entity.Xenograft.ImplantedCellsNumber,
            Tumorigenicity = entity.Xenograft.Tumorigenicity,
            TumorGrowthForm = entity.Xenograft.TumorGrowthFormId?.ToDefinitionString(),
            SurvivalDaysFrom = entity.Xenograft.SurvivalDaysFrom,
            SurvivalDaysTo = entity.Xenograft.SurvivalDaysTo,

            MolecularData = CreateFrom(entity.MolecularData),
            Interventions = CreateFrom(entity.Interventions, diagnosisDate),
            DrugScreenings = CreateFrom(entity.DrugScreenings)
        };
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

    private static InterventionIndex[] CreateFrom(in IEnumerable<Intervention> entities, DateOnly? diagnosisDate)
    {
        if (entities?.Any() != true)
        {
            return null;
        }

        return entities.Select(entity =>
        {
            return new InterventionIndex
            {
                Type = entity.Type.Name,
                Details = entity.Details,
                StartDay = entity.StartDay ?? entity.StartDate?.RelativeFrom(diagnosisDate),
                DurationDays = entity.DurationDays ?? entity.EndDate?.RelativeFrom(entity.StartDate),
                Results = entity.Results
            };

        }).ToArray();
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
                AbsIC25 = entity.AbsIC25,
                AbsIC50 = entity.AbsIC50,
                AbsIC75 = entity.AbsIC75,
                MinConcentration = entity.MinConcentration,
                MaxConcentration = entity.MaxConcentration
            };

        }).ToArray();
    }
}
