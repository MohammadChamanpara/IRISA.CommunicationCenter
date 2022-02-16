using FluentAssertions;
using IRISA.CommunicationCenter.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

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

        [TestMethod]
        public void AddTelegramsToProcessingList_WhenProcessingIsEmpty_ShouldAddAll()
        {
            //Arrange
            var iccCore = new IccCore();
            var telegrams = new List<IccTransfer>()
            {
                new IccTransfer() { ID=1 },
                new IccTransfer() { ID=2 },
                new IccTransfer() { ID=3 }
            };

            var inProcess = new HashSet<long>();

            //Act
            var newInprocess = iccCore.AddTelegramsToProcessingList(telegrams, inProcess);

            //Assert
            newInprocess.Count.Should().Be(3);
        }

        [TestMethod]
        public void AddTelegramsToProcessingList_WhenAlreayInProcessing_ShouldNotAdd()
        {
            //Arrange
            var iccCore = new IccCore();
            var telegrams = new List<IccTransfer>()
            {
                new IccTransfer() { ID=1 },
                new IccTransfer() { ID=2 },
                new IccTransfer() { ID=3 }
            };

            var inProcess = new HashSet<long>() { 1, 2 };

            //Act
            var newInprocess = iccCore.AddTelegramsToProcessingList(telegrams, inProcess);

            //Assert
            newInprocess.Count.Should().Be(3);
            newInprocess.Should().BeEquivalentTo(new long[] { 1, 2, 3 });
        }

        [TestMethod]
        public void GroupTelegramsByDestination_Always_ShouldGroup()
        {
            //Arrange
            var iccCore = new IccCore();
            var telegrams = new List<IccTransfer>()
            {
                new IccTransfer() { ID=1, DESTINATION="1" },
                new IccTransfer() { ID=2, DESTINATION="2" },
                new IccTransfer() { ID=3, DESTINATION="3" },
                new IccTransfer() { ID=3, DESTINATION="1" },
                new IccTransfer() { ID=3, DESTINATION="2" },
                new IccTransfer() { ID=3, DESTINATION="3" },
                new IccTransfer() { ID=3, DESTINATION="1" }
            };

            //Act
            var groupedTelegrams = iccCore.GroupTelegramsByDestination(telegrams);

            //Assert
            groupedTelegrams.Count.Should().Be(3);

            groupedTelegrams.First().Count.Should().Be(3);
            groupedTelegrams.First().All(x=>x.DESTINATION=="1").Should().BeTrue();

            groupedTelegrams[1].Count.Should().Be(2);
            groupedTelegrams[1].All(x => x.DESTINATION == "2").Should().BeTrue();

            groupedTelegrams[2].Count.Should().Be(2);
            groupedTelegrams[2].All(x => x.DESTINATION == "3").Should().BeTrue();

        }
    }
}
