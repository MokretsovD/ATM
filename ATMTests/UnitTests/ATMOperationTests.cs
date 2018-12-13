
using ATM.HostProcessor.Mock;
using System;
using Xunit;

namespace ATMTests.UnitTests
{
    public class AtmOperationTests
    {
        public AtmOperationTests()
        {
            
        }

        [Fact]
        public void TestCreate()
        {
            // Arrange
            const string cardNumber = "132123";
            const decimal amount = 65354;
            const decimal fee = 3434;

            var operationId = Guid.NewGuid();
            var operationDate = new DateTime(2018, 05, 17, 13, 05, 57);
                       
            // Act
            var atmOperation = new AtmOperation(
                cardNumber,
                operationId,
                amount,
                fee,
                operationDate);

            // Assert
            Assert.False(atmOperation.OperationCompleted);
            Assert.Equal(operationDate, atmOperation.OperationDate);
            Assert.Equal(cardNumber, atmOperation.CardNumber);
            Assert.Equal(amount, atmOperation.Amount);
            Assert.Equal(fee, atmOperation.Fee);
            Assert.Equal(operationId, atmOperation.OperationId);
        }

        [Fact]
        public void TestSetComplete()
        {
            // Arrange
            const string cardNumber = "132123";
            const decimal amount = 65354;
            const decimal fee = 3434;
            
            var operationId = Guid.NewGuid();
            var operationDate = new DateTime(2018, 05, 17, 13, 05, 57);
            var completionDate = new DateTime(2018, 05, 17, 13, 05, 59);

            var atmOperation = new AtmOperation(
                cardNumber,
                operationId,
                amount,
                fee,
                operationDate);

            // Act
            atmOperation.SetComplete(completionDate);

            // Assert
            Assert.True(atmOperation.OperationCompleted);
            Assert.Equal(completionDate, atmOperation.OperationDate);
            Assert.Equal(cardNumber, atmOperation.CardNumber);
            Assert.Equal(amount, atmOperation.Amount);
            Assert.Equal(fee, atmOperation.Fee);
            Assert.Equal(operationId, atmOperation.OperationId);
        }
    }
}
