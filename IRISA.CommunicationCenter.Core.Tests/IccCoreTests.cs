using FluentAssertions;
using IRISA.CommunicationCenter.Library.Adapters;
using IRISA.CommunicationCenter.Library.Core;
using IRISA.CommunicationCenter.Library.Definitions;
using IRISA.CommunicationCenter.Library.Logging;
using IRISA.CommunicationCenter.Library.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;

namespace IRISA.CommunicationCenter.Core.Tests
{
    [TestClass]
    public class IccCoreTests
    {
        [TestMethod]
        public void Start_WhenThereAreReadyTelegramsInHistory_ShouldSendThem()
        {
            // Arrange
            string adapterName = "TheAdapter";

            IccTelegram telegram = new IccTelegram()
            {
                Destination = adapterName
            };

            var logger = new Mock<ILogger>();

            var telegramDefinitions = Mock.Of<ITelegramDefinitions>();

            var transferHistory = new Mock<ITransferHistory>();
            transferHistory.Setup(x => x.GetTelegramsToSend()).Returns(new List<IccTelegram> { telegram, telegram });

            var adapter = new Mock<IIccAdapter>();
            adapter.Setup(x => x.Name).Returns(adapterName);

            var adapterRepository = MockOfAdapterRepository(adapter.Object);

            var iccCore = new IccCore(logger.Object, transferHistory.Object, telegramDefinitions, adapterRepository.Object);

            // Act
            iccCore.Start();

            // Assert
            iccCore.Started.Should().BeTrue();
            adapter.Verify(x => x.Send(telegram), Times.Exactly(2));

            transferHistory.Verify(x => x.GetTelegramsToSend(), Times.Once);
            transferHistory.GetTelegramsShouldNotBeCalled();

            adapterRepository.Verify(x => x.GetAll(), Times.Once);

            logger.ShouldHaveNoErrors();
        }



        [TestMethod]
        public void Start_WhenAnAdapterSendsATelegram_ShouldSendItToDestination()
        {
            // Arrange
            string sourceAdapterName = "source";
            string destinationAdapterName = "destination";

            IccTelegram telegram = new IccTelegram()
            {
                Destination = destinationAdapterName
            };

            var logger = new Mock<ILogger>();

            var telegramDefinitions = Mock.Of<ITelegramDefinitions>();

            var transferHistory = new Mock<ITransferHistory>();

            var sourceAdapter = new UnitTestAdapter(sourceAdapterName);
            var destinationAdapter = new UnitTestAdapter(destinationAdapterName);

            var adapterRepository = MockOfAdapterRepository(sourceAdapter, destinationAdapter);

            var iccCore = new IccCore(logger.Object, transferHistory.Object, telegramDefinitions, adapterRepository.Object);

            // Act
            iccCore.Start();
            sourceAdapter.SendTelegramToIcc(telegram);

            //Assert
            iccCore.Started.Should().BeTrue();

            sourceAdapter.SentTelegrams.Count.Should().Be(0);

            destinationAdapter.SentTelegrams.Count.Should().Be(1);
            destinationAdapter.SentTelegrams.Single().Sent.Should().BeTrue();
            destinationAdapter.SentTelegrams.Single().Destination.Should().Be(destinationAdapterName);

            transferHistory.Verify(x => x.GetTelegramsToSend(), Times.Once());
            transferHistory.GetTelegramsShouldNotBeCalled();

            logger.ShouldHaveNoErrors();
        }

        [TestMethod]
        public void Start_WhenTelegramHasMultipleDestinations_ShouldSendItToAllDestination()
        {
            // Arrange
            string sourceAdapterName = "source";
            string destination1AdapterName = "destination1";
            string destination2AdapterName = "destination2";

            IccTelegram telegram = new IccTelegram()
            {
                Destination = $",{destination1AdapterName},,{destination2AdapterName},,,"
            };

            var logger = new Mock<ILogger>();

            var telegramDefinitions = Mock.Of<ITelegramDefinitions>();

            var transferHistory = new Mock<ITransferHistory>();

            var sourceAdapter = new UnitTestAdapter(sourceAdapterName);
            var destination1Adapter = new UnitTestAdapter(destination1AdapterName);
            var destination2Adapter = new UnitTestAdapter(destination2AdapterName);

            var adapterRepository = MockOfAdapterRepository(sourceAdapter, destination1Adapter, destination2Adapter);

            var iccCore = new IccCore(logger.Object, transferHistory.Object, telegramDefinitions, adapterRepository.Object);

            // Act
            iccCore.Start();
            sourceAdapter.SendTelegramToIcc(telegram);

            //Assert
            iccCore.Started.Should().BeTrue();

            sourceAdapter.SentTelegrams.Count.Should().Be(0);

            destination1Adapter.SentTelegrams.Count.Should().Be(1);
            destination1Adapter.SentTelegrams.Single().Sent.Should().BeTrue();
            destination1Adapter.SentTelegrams.Single().Destination.Should().Be(destination1AdapterName);

            destination2Adapter.SentTelegrams.Count.Should().Be(1);
            destination2Adapter.SentTelegrams.Single().Sent.Should().BeTrue();
            destination2Adapter.SentTelegrams.Single().Destination.Should().Be(destination2AdapterName);

            transferHistory.Verify(x => x.GetTelegramsToSend(), Times.Once());
            transferHistory.GetTelegramsShouldNotBeCalled();

            logger.ShouldHaveNoErrors();
        }

        private static Mock<IAdapterRepository> MockOfAdapterRepository(params IIccAdapter[] adapters)
        {
            var adapterRepository = new Mock<IAdapterRepository>();

            adapterRepository
                .Setup(x => x.GetAll())
                .Returns(new List<IIccAdapter>(adapters));

            return adapterRepository;
        }
    }
}
