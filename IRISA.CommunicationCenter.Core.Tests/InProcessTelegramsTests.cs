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
            var inProcessTelegrams = new InProcessTelegrams(1,2);
            var telegrams = new List<IccTransfer>()
            {
                new IccTransfer() { ID=1 },
                new IccTransfer() { ID=2 },
                new IccTransfer() { ID=3 },
                new IccTransfer() { ID=4 },
                new IccTransfer() { ID=5 }
            };

            //Act
            var sendingTelegrams = inProcessTelegrams.RemoveFrom(telegrams);

            //Assert
            sendingTelegrams.Count.Should().Be(3);
            sendingTelegrams.TrueForAll(x => x.ID > 2);
        }

        [TestMethod]
        public void RemoveFrom_WhenProcessingListIsEmpty_ShouldNotRemoveAnyTelegrams()
        {
            //Arrange
            var inProcessTelegrams = new InProcessTelegrams();

            var telegrams = new List<IccTransfer>()
            {
                new IccTransfer() { ID=1 },
                new IccTransfer() { ID=2 },
                new IccTransfer() { ID=3 },
                new IccTransfer() { ID=4 },
                new IccTransfer() { ID=5 }
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

            var telegrams = new List<IccTransfer>()
            {
                new IccTransfer() { ID=1 },
                new IccTransfer() { ID=2 },
                new IccTransfer() { ID=3 }
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
            var inProcessTelegrams = new InProcessTelegrams(1,2);

            var telegrams = new List<IccTransfer>()
            {
                new IccTransfer() { ID=1 },
                new IccTransfer() { ID=2 },
                new IccTransfer() { ID=3 }
            };

            //Act
            inProcessTelegrams.AddRange(telegrams);

            //Assert
            inProcessTelegrams.GetAllIds().Count.Should().Be(3);
            inProcessTelegrams.GetAllIds().Should().BeEquivalentTo(new long[] { 1, 2, 3 });
        }

        
    }
}
