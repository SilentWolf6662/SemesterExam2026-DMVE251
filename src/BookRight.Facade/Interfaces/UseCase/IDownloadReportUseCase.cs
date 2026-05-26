using BookRight.Facade.Command;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookRight.Facade.Interfaces.UseCase
{
    public interface IDownloadReportUseCase
    {
        Task Execute(DownloadReportRequest request);
    }
}
