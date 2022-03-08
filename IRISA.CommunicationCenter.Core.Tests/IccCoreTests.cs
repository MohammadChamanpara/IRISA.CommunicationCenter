using FluentAssertions;
using IRISA.CommunicationCenter.Library.Adapters;
using IRISA.CommunicationCenter.Library.Core;
using IRISA.CommunicationCenter.Library.Definitions;
using IRISA.CommunicationCenter.Library.Logging;
using IRISA.CommunicationCenter.Library.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace IRISA.CommunicationCenter.Core.Tests
{
    [TestClass]
    public class IccCoreTests
    {
        [TestMethod]
        public void Start_WhenThereAreReadyTelegramsInHistory_ShouldSendThem()
        {
            string adapterName = "TheAdapter";
            
            IccTelegram telegram = new IccTelegram()
            {
                Destination = adapterName,
            };

            var logger = new Mock<ILogger>();

            var telegramDefinitions = Mock.Of<ITelegramDefinitions>();
            
            var transferHistory = new Mock<ITransferHistory>();
            transferHistory.Setup(x => x.GetTelegramsToSend()).Returns(new List<IccTelegram> { telegram,telegram });

            var adapter = new Mock<IIccAdapter>();
            adapter.Setup(x => x.Name).Returns(adapterName);

            var adapterRepository = new Mock<IAdapterRepository>();
            adapterRepository
                .Setup(repo => repo.GetAll())
                .Returns(new List<IIccAdapter>() { adapter.Object });

            var iccCore = new IccCore(logger.Object, transferHistory.Object, telegramDefinitions, adapterRepository.Object);

            iccCore.Start();

            iccCore.Started.Should().BeTrue();
            adapter.Verify(x=>x.Send(telegram), Times.Exactly(2));
            logger.Verify(x=>x.LogException(It.IsAny<Exception>(),It.IsAny<string>()), Times.Never());
            logger.Verify(x=>x.LogError(It.IsAny<string>()), Times.Never());
            logger.Verify(x=>x.LogWarning(It.IsAny<string>()), Times.Never());
        }
    }
}
