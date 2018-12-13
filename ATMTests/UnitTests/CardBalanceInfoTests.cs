
using ATM.Exceptions;
using ATM.HostProcessor.Mock;
using Xunit;

namespace ATMTests.UnitTests
{
    public class CardBalanceInfoTests
    {
        [Fact]
        public void TestCreate()
        {
            //Arrange
            const decimal balance = 100.33M;
            const decimal blocked = 100M;
            
            //Act
            var cardBalanceInfo = new CardBalanceInfo(balance, blocked);

            //Assert
            Assert.Equal(balance, cardBalanceInfo.Balance);
            Assert.Equal(blocked, cardBalanceInfo.Blocked);
        }

        [Fact]
        public void TestDecreaseBalanceAndBlocked()
        {
            //Arrange
            const decimal balance = 100.33M;
            const decimal blocked = 100M;
            const decimal amount = 100M;

            var cardBalanceInfo = new CardBalanceInfo(balance, blocked);

            //Act
            cardBalanceInfo.DecreaseBalanceAndBlocked(amount);

            //Assert
            Assert.Equal(0.33M, cardBalanceInfo.Balance);
            Assert.Equal(0M, cardBalanceInfo.Blocked);
        }

        [Theory]
        [InlineData(-100)]
        [InlineData(100.01)]
        public void TestDecreaseBalanceAndBlockedInvalidAmountThrows(decimal amount)
        {
            //Arrange
            const decimal balance = 100.33M;
            const decimal blocked = 100M;

            var cardBalanceInfo = new CardBalanceInfo(balance, blocked);

            //Act
            Assert.Throws<InvalidAmountException>(() => cardBalanceInfo.DecreaseBalanceAndBlocked(amount));

            //Assert
        }

        [Fact]
        public void TestBlockFunds()
        {
            //Arrange
            const decimal balance = 100.33M;
            const decimal blocked = 100M;
            const decimal amount = 100M;

            var cardBalanceInfo = new CardBalanceInfo(balance, blocked);

            //Act
            cardBalanceInfo.BlockFunds(amount);

            //Assert
            Assert.Equal(balance, cardBalanceInfo.Balance);
            Assert.Equal(blocked + amount, cardBalanceInfo.Blocked);
        }
    }
}
