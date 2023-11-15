using Unite.Data.Entities.Images;
using Unite.Data.Extensions;
using Unite.Indices.Entities.Basic.Images;

namespace Unite.Mapping;

public class ImageIndexMapper
{
    /// <summary>
    /// Creates an index from the entity. Returns null if entity is null.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <param name="diagnosisDate">Diagnosis date. Used to calculate relative scanning day if it's not set.</param>
    /// <typeparam name="T">Type of the index.</typeparam>
    /// <returns>Index created from the entity.</returns>
    public static T CreateFrom<T>(in Image entity, DateOnly? diagnosisDate) where T : ImageIndex, new()
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
    /// <param name="diagnosisDate">Diagnosis date. Used to calculate relative scanning day if it's not set.</param>
    public static void Map(in Image entity, ImageIndex index, DateOnly? diagnosisDate)
    {
        if (entity == null || index == null)
        {
            return;
        }

        index.Id = entity.Id;
        index.ReferenceId = entity.ReferenceId;
        index.Type = entity.TypeId.ToDefinitionString();
        index.ScanningDay = entity.CreationDay ?? entity.CreationDate.RelativeFrom(diagnosisDate);

        index.Mri = CreateFrom(entity.MriImage);
    }


    private static MriImageIndex CreateFrom(in MriImage entity)
    {
        if (entity == null)
        {   
            return null;
        }

        return new MriImageIndex
        {
            ReferenceId = entity.ReferenceId,

            WholeTumor = entity.WholeTumor,
            ContrastEnhancing = entity.ContrastEnhancing,
            NonContrastEnhancing = entity.NonContrastEnhancing,

            MedianAdcTumor = entity.MedianAdcTumor,
            MedianAdcCe = entity.MedianAdcCe,
            MedianAdcEdema = entity.MedianAdcEdema,

            MedianCbfTumor = entity.MedianCbfTumor,
            MedianCbfCe = entity.MedianCbfCe,
            MedianCbfEdema = entity.MedianCbfEdema,

            MedianCbvTumor = entity.MedianCbvTumor,
            MedianCbvCe = entity.MedianCbvCe,
            MedianCbvEdema = entity.MedianCbvEdema,

            MedianMttTumor = entity.MedianMttTumor,
            MedianMttCe = entity.MedianMttCe,
            MedianMttEdema = entity.MedianMttEdema
        };
    }
}
