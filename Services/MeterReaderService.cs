using Grpc.Core;

namespace GrpcDemo.Services;

public class MeterReaderService: MeterReadingService.MeterReadingServiceBase
{
   private readonly ILogger<MeterReaderService> _logger;
   private List<ReadingMessage> _repository;
   public MeterReaderService(ILogger<MeterReaderService> logger)
   {
      _logger = logger;
      _repository = new List<ReadingMessage>();
   }
   
   public override  Task<StatusMessage> AddReading(ReadingPacket request, ServerCallContext context)
   {
      StatusMessage result = new ()
      {
         Success = ReadingStatus.Failure
      };
      
      if (request.Successful == ReadingStatus.Success)
      {
         try
         {
            foreach (var reading in request.Readings)
            {
               // Save to the database
               _repository.Add(reading);
               result.Success = ReadingStatus.Success;
            }
         }
         catch (Exception e)
         {
            result.Message = "Exception thrown during processing";
            _logger.LogError("Exception thrown during savings of readings {E}", e);
         }
      }

      return Task.FromResult(result);
   }
}