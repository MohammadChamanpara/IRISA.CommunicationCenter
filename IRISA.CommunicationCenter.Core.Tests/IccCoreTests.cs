using FluentAssertions;
using IRISA.CommunicationCenter.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace IRISA.CommunicationCenter.Core.Tests
{
    [TestClass]
    public class IccCoreTests
    {
        [TestMethod]
        public void RemoveInProcessTelegrams_Always_ShouldRemoveInProcessTelegrams()
        {
            //Arrange
            var iccCore = new IccCore();
            var telegrams = new List<IccTransfer>()
            {
                new IccTransfer() { ID=1 },
                new IccTransfer() { ID=2 },
                new IccTransfer() { ID=3 },
                new IccTransfer() { ID=4 },
                new IccTransfer() { ID=5 }
            };

            var inProcess = new HashSet<long>() { 1, 2 };

            //Act
            var sendingTelegrams = iccCore.RemoveInProcessTelegrams(telegrams, inProcess);

            //Assert
            sendingTelegrams.Count.Should().Be(3);
            sendingTelegrams.TrueForAll(x => x.ID > 2);
        }

        [TestMethod]
        public void RemoveInProcessTelegrams_WhenProcessingListIsEmpty_ShouldNotRemoveAnyTelegrams()
        {
            //Arrange
            var iccCore = new IccCore();
            var telegrams = new List<IccTransfer>()
            {
                new IccTransfer() { ID=1 },
                new IccTransfer() { ID=2 },
                new IccTransfer() { ID=3 },
                new IccTransfer() { ID=4 },
                new IccTransfer() { ID=5 }
            };

            var inProcess = new HashSet<long>();

            //Act
            var sendingTelegrams = iccCore.RemoveInProcessTelegrams(telegrams, inProcess);

            //Assert
            sendingTelegrams.Count.Should().Be(5);
        }
    }
}
