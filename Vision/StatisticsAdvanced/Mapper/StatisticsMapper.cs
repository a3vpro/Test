using AutoMapper;
using System.Collections.Generic;
using VisionNet.Core.Types;
using VisionNet.Vision.Core;

namespace VisionNet.DataBases
{
    public class StatisticsMapper
    {
        private readonly IMapper _mapper;

        public StatisticsMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductToStatisticsMapperProfile>();
                cfg.AddProfile<InspectionToStatisticsMapperProfile>();
            });

            _mapper = config.CreateMapper();
        }

        public List<StatisticsRecord> MapToStatistics(ProductResult productResult)
        {
            var statisticsRecords = new List<StatisticsRecord>();

            // Mapeo de ProductResult a StatisticsRecord
            var productStatistics = _mapper.Map<StatisticsRecord>(productResult);
            statisticsRecords.Add(productStatistics);

            // Mapeo de cada InspectionResult a StatisticsRecord
            foreach (var inspectionResult in productResult.Inspections)
            {
                var context = new InspectionMappingContext
                {
                    Product = productResult,
                    Inspection = inspectionResult
                };

                var inspectionStatistics = _mapper.Map<StatisticsRecord>(context);
                statisticsRecords.Add(inspectionStatistics);
            }

            return statisticsRecords;
        }
    }
}
