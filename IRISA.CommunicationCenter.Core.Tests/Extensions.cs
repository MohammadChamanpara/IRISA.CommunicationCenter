using IRISA.CommunicationCenter.Library.Core;
using IRISA.CommunicationCenter.Library.Logging;
using IRISA.CommunicationCenter.Library.Models;
using Moq;
using System;

namespace IRISA.CommunicationCenter.Core.Tests
{
    static class Extensions
    {
        public static void ShouldHaveNoErrors(this Mock<ILogger> logger)
        {
            logger.Verify(x => x.LogException(It.IsAny<Exception>(), It.IsAny<string>()), Times.Never());
            logger.Verify(x => x.LogError(It.IsAny<string>()), Times.Never());
            logger.Verify(x => x.LogWarning(It.IsAny<string>()), Times.Never());
        }

        public static void GetTelegramsShouldNotBeCalled(this Mock<ITransferHistory> transferHistory)
        {
            int resultCount;
            transferHistory
                .Verify
                (
                    x => x.GetTelegrams
                    (
                        It.IsAny<IccTelegramSearchModel>(),
                        It.IsAny<int>(), out resultCount
                    ),
                    Times.Never()
                );
        }
    }
}
