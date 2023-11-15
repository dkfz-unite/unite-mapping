using Unite.Data.Entities.Specimens;
using Unite.Data.Extensions;
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
    /// <param name="diagnosisDate">Diagnosis date. Used to calculate relative creation and intervention days if they are not set.</param> 
    /// <typeparam name="T">Type of the index.</typeparam>
    /// <returns>Index created from the entity.</returns>
    public static T CreateFrom<T>(in Specimen specimen, DateOnly? diagnosisDate) where T : SpecimenIndex, new()
    {
        if (specimen == null)
        {
            return null;
        }

        var index = new T();

        Map(specimen, index, diagnosisDate);

        return index;
    }

    /// <summary>
    /// Maps entity to index. Does nothing if either entity or index is null.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <param name="index">Index.</param>
    /// <param name="diagnosisDate">Diagnosis date. Used to calculate relative creation and intervention days if they are not set.</param>
    public static void Map(in Specimen specimen, SpecimenIndex index, DateOnly? diagnosisDate)
    {
        if (specimen == null)
        {
            return;
        }

        index.Id = specimen.Id;
        index.ReferenceId = specimen.ReferenceId;
        index.Type = specimen.TypeId.ToDefinitionString();
        index.CreationDay = specimen.CreationDate.RelativeFrom(diagnosisDate) ?? specimen.CreationDay;

        index.Tissue = CreateFromTissue(specimen);
        index.Cell = CreateFromCellLine(specimen);
        index.Organoid = CreateFromOrganoid(specimen, specimen.CreationDate);
        index.Xenograft = CreateFromXenograft(specimen, specimen.CreationDate);
    }


    private static TissueIndex CreateFromTissue(in Specimen specimen)
    {
        if (specimen.Tissue == null)
        {
            return null;
        }

        return new TissueIndex
        {
            ReferenceId = specimen.Tissue.ReferenceId,

            Type = specimen.Tissue.TypeId?.ToDefinitionString(),
            TumorType = specimen.Tissue.TumorTypeId?.ToDefinitionString(),
            Source = specimen.Tissue.Source?.Value,

            MolecularData = CreateFrom(specimen.MolecularData)
        };
    }

    private static CellLineIndex CreateFromCellLine(in Specimen specimen)
    {
        if (specimen.CellLine == null)
        {
            return null;
        }

        return new CellLineIndex
        {
            ReferenceId = specimen.CellLine.ReferenceId,

            Species = specimen.CellLine.SpeciesId?.ToDefinitionString(),
            Type = specimen.CellLine.TypeId?.ToDefinitionString(),
            CultureType = specimen.CellLine.CultureTypeId?.ToDefinitionString(),

            Name = specimen.CellLine.Info?.Name,
            DepositorName = specimen.CellLine.Info?.DepositorName,
            DepositorEstablishment = specimen.CellLine.Info?.DepositorEstablishment,
            EstablishmentDate = specimen.CellLine.Info?.EstablishmentDate,

            PubMedLink = specimen.CellLine.Info?.PubMedLink,
            AtccLink = specimen.CellLine.Info?.AtccLink,
            ExPasyLink = specimen.CellLine.Info?.ExPasyLink,

            MolecularData = CreateFrom(specimen.MolecularData),
            DrugScreenings = CreateFrom(specimen.DrugScreenings)
        };
    }

    private static OrganoidIndex CreateFromOrganoid(in Specimen specimen, DateOnly? specimenCreationDate)
    {
        if (specimen.Organoid == null)
        {
            return null;
        }

        return new OrganoidIndex
        {
            ReferenceId = specimen.Organoid.ReferenceId,
            ImplantedCellsNumber = specimen.Organoid.ImplantedCellsNumber,
            Tumorigenicity = specimen.Organoid.Tumorigenicity,
            Medium = specimen.Organoid.Medium,

            MolecularData = CreateFrom(specimen.MolecularData),
            DrugScreenings = CreateFrom(specimen.DrugScreenings),
            Interventions = CreateFrom(specimen.Organoid.Interventions, specimenCreationDate)
        };
    }

    private static OrganoidInterventionIndex[] CreateFrom(in IEnumerable<OrganoidIntervention> interventions, DateOnly? specimenCreationDate)
    {
        if (interventions?.Any() != true)
        {
            return null;
        }

        return interventions.Select(intervention =>
        {
            return new OrganoidInterventionIndex
            {
                Type = intervention.Type.Name,
                Details = intervention.Details,
                StartDay = intervention.StartDate.RelativeFrom(specimenCreationDate) ?? intervention.StartDay,
                DurationDays = intervention.EndDate.RelativeFrom(intervention.StartDate) ?? intervention.DurationDays,
                Results = intervention.Results
            };

        }).ToArray();
    }

    private static XenograftIndex CreateFromXenograft(in Specimen specimen, DateOnly? specimenCreationDate)
    {
        if (specimen.Xenograft == null)
        {
            return null;
        }

        return new XenograftIndex
        {
            ReferenceId = specimen.Xenograft.ReferenceId,

            MouseStrain = specimen.Xenograft.MouseStrain,
            GroupSize = specimen.Xenograft.GroupSize,
            ImplantType = specimen.Xenograft.ImplantTypeId?.ToDefinitionString(),
            TissueLocation = specimen.Xenograft.TissueLocationId?.ToDefinitionString(),
            ImplantedCellsNumber = specimen.Xenograft.ImplantedCellsNumber,
            Tumorigenicity = specimen.Xenograft.Tumorigenicity,
            TumorGrowthForm = specimen.Xenograft.TumorGrowthFormId?.ToDefinitionString(),
            SurvivalDaysFrom = specimen.Xenograft.SurvivalDaysFrom,
            SurvivalDaysTo = specimen.Xenograft.SurvivalDaysTo,

            MolecularData = CreateFrom(specimen.MolecularData),
            DrugScreenings = CreateFrom(specimen.DrugScreenings),
            Interventions = CreateFrom(specimen.Xenograft.Interventions, specimenCreationDate)
        };
    }

    private static XenograftInterventionIndex[] CreateFrom(in IEnumerable<XenograftIntervention> interventions, DateOnly? specimenCreationDate)
    {
        if (interventions?.Any() != true)
        {
            return null;
        }

        return interventions.Select(intervention =>
        {
            return new XenograftInterventionIndex
            {
                Type = intervention.Type.Name,
                Details = intervention.Details,
                StartDay = intervention.StartDate.RelativeFrom(specimenCreationDate) ?? intervention.StartDay,
                DurationDays = intervention.EndDate.RelativeFrom(intervention.StartDate) ?? intervention.DurationDays,
                Results = intervention.Results
            };

        }).ToArray();
    }

    private static MolecularDataIndex CreateFrom(in MolecularData molecularData)
    {
        if (molecularData == null)
        {
            return null;
        }

        return new MolecularDataIndex
        {
            MgmtStatus = molecularData.MgmtStatusId?.ToDefinitionString(),
            IdhStatus = molecularData.IdhStatusId?.ToDefinitionString(),
            IdhMutation = molecularData.IdhMutationId?.ToDefinitionString(),
            GeneExpressionSubtype = molecularData.GeneExpressionSubtypeId?.ToDefinitionString(),
            MethylationSubtype = molecularData.MethylationSubtypeId?.ToDefinitionString(),
            GcimpMethylation = molecularData.GcimpMethylation
        };
    }

    private static DrugScreeningIndex[] CreateFrom(in IEnumerable<DrugScreening> screenings)
    {
        if (screenings?.Any() != true)
        {
            return null;
        }

        return screenings.Select(screening =>
        {
            return new DrugScreeningIndex
            {
                Dss = screening.Dss,
                DssSelective = screening.DssSelective,
                Gof = screening.Gof,
                Drug = screening.Feature.Name,
                MinConcentration = screening.MinConcentration,
                MaxConcentration = screening.MaxConcentration,
                AbsIC25 = screening.AbsIC25,
                AbsIC50 = screening.AbsIC50,
                AbsIC75 = screening.AbsIC75
            };

        }).ToArray();
    }
}
