using FluentAssertions;
using IRISA.CommunicationCenter.Core.Model;
using IRISA.Loggers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace IRISA.CommunicationCenter.Core.Tests
{
    [TestClass]
    public class IccCoreTests
    {
        [TestMethod]
        public void GroupTelegramsByDestination_Always_ShouldGroup()
        {
            //Arrange
            var iccCore = new IccCore(new InProcessTelegrams(), new InMemoryLogger());
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
            groupedTelegrams.First().All(x => x.DESTINATION == "1").Should().BeTrue();

            groupedTelegrams[1].Count.Should().Be(2);
            groupedTelegrams[1].All(x => x.DESTINATION == "2").Should().BeTrue();

            groupedTelegrams[2].Count.Should().Be(2);
            groupedTelegrams[2].All(x => x.DESTINATION == "3").Should().BeTrue();

        }
    }
}
