using BookRight.Facade.Command;

namespace BookRight.Facade.Interfaces.UseCase;

public interface IDownloadReportUseCase
{
    Task Execute(DownloadReportRequest request);
}