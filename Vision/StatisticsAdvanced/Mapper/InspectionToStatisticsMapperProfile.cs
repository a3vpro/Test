using AutoMapper;
using VisionNet.DataBases;
using VisionNet.Vision.Core;

namespace VisionNet.DataBases
{
    public class InspectionToStatisticsMapperProfile : Profile
    {
        public InspectionToStatisticsMapperProfile()
        {
            // Mapeo de InspectionMappingContext a StatisticsRecord
            CreateMap<InspectionMappingContext, StatisticsRecord>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Id autonumérico, no se mapea directamente
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product.Info.Index)) // Mapear ProductId desde ProductResult
                .ForMember(dest => dest.DateTime, opt => opt.MapFrom(src => src.Product.Info.DateTime)) // Mapear DateTime desde ProductResult
                .ForMember(dest => dest.InspectionName, opt => opt.MapFrom(src => src.Inspection.Name)) // Mapear nombre de la inspección
                .ForMember(dest => dest.MatchingCriteria, opt => opt.MapFrom(src => src.Inspection.Result)) // Mapear el resultado como criterio de coincidencia
                .ForAllOtherMembers(opt => opt.Ignore()); // Ignorar propiedades no mapeadas explícitamente
        }
    }
}
