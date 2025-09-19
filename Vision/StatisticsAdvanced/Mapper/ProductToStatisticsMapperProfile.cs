using AutoMapper;
using VisionNet.Vision.Core;

namespace VisionNet.DataBases
{
    public class ProductToStatisticsMapperProfile : Profile
    {
        public ProductToStatisticsMapperProfile()
        {
            // Mapeo de ProductResult a StatisticsRecord
            CreateMap<ProductResult, StatisticsRecord>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Id autonumérico, no se mapea directamente
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Index)) // Asignar el Index como ProductId
                .ForMember(dest => dest.DateTime, opt => opt.MapFrom(src => src.Info.DateTime)) // Mapear la fecha del producto
                .ForMember(dest => dest.InspectionName, opt => opt.MapFrom(src => "Global")) // Mapear nombre de inspección si existe
                .ForMember(dest => dest.MatchingCriteria, opt => opt.MapFrom(src => src.Info.Result)) // Resultado como criterio de coincidencia
                .ForAllOtherMembers(opt => opt.Ignore()); // Ignorar propiedades no mapeadas explícitamente
        }
    }
}
