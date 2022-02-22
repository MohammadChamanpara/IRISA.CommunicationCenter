using FluentAssertions;
using IRISA.CommunicationCenter.Core.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace IRISA.CommunicationCenter.Core.Tests
{
    [TestClass]
    public class InProcessTelegramsTests
    {
        [TestMethod]
        public void RemoveFrom_Always_ShouldRemoveInProcessTelegrams()
        {
            //Arrange
            var inProcessTelegrams = new InProcessTelegrams(1, 2);
            var telegrams = new List<IccTelegram>()
            {
                new IccTelegram() { TransferId = 1 },
                new IccTelegram() { TransferId = 2 },
                new IccTelegram() { TransferId = 3 },
                new IccTelegram() { TransferId = 4 },
                new IccTelegram() { TransferId = 5 }
            };

            //Act
            var sendingTelegrams = inProcessTelegrams.RemoveFrom(telegrams);

            //Assert
            sendingTelegrams.Count.Should().Be(3);
            sendingTelegrams.TrueForAll(x => x.TransferId > 2);
        }

        [TestMethod]
        public void RemoveFrom_WhenProcessingListIsEmpty_ShouldNotRemoveAnyTelegrams()
        {
            //Arrange
            var inProcessTelegrams = new InProcessTelegrams();

            var telegrams = new List<IccTelegram>()
            {
                new IccTelegram() { TransferId = 1 },
                new IccTelegram() { TransferId = 2 },
                new IccTelegram() { TransferId = 3 },
                new IccTelegram() { TransferId = 4 },
                new IccTelegram() { TransferId = 5 }
            };

            //Act
            var sendingTelegrams = inProcessTelegrams.RemoveFrom(telegrams);

            //Assert
            sendingTelegrams.Count.Should().Be(5);
        }

        [TestMethod]
        public void AddRange_WhenProcessingIsEmpty_ShouldAddAll()
        {
            //Arrange
            var inProcessTelegrams = new InProcessTelegrams();

            var telegrams = new List<IccTelegram>()
            {
                new IccTelegram() { TransferId = 1 },
                new IccTelegram() { TransferId = 2 },
                new IccTelegram() { TransferId = 3 }
            };

            //Act
            inProcessTelegrams.AddRange(telegrams);

            //Assert
            inProcessTelegrams.GetAllIds().Count.Should().Be(3);
        }

        [TestMethod]
        public void AddRange_WhenAlreayInProcessing_ShouldNotAdd()
        {
            //Arrange
            var inProcessTelegrams = new InProcessTelegrams(1, 2);

            var telegrams = new List<IccTelegram>()
            {
                new IccTelegram() { TransferId = 1 },
                new IccTelegram() { TransferId = 2 },
                new IccTelegram() { TransferId = 3 }
            };

            //Act
            inProcessTelegrams.AddRange(telegrams);

            //Assert
            inProcessTelegrams.GetAllIds().Count.Should().Be(3);
            inProcessTelegrams.GetAllIds().Should().BeEquivalentTo(new long[] { 1, 2, 3 });
        }


    }
}
