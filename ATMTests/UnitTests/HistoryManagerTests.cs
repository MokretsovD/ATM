
using ATM.Exceptions;
using ATM.HostProcessor.Mock;
using System;
using Xunit;

namespace ATMTests.UnitTests
{
    public class HistoryManagerTests
    {
        private readonly HistoryManager _historyManager;

        public HistoryManagerTests()
        {
            _historyManager = new HistoryManager();
        }

        [Fact]
        public void TestAddAtOperation()
        {
            //Arrange
            const string cardNumber = "4343";
            const string cardNumber2 = "12312";
            const decimal withdrawalAmount = 53735;
            const decimal withdrawalFeeAmount = 343;

            const decimal withdrawalAmount2 = 12312;
            const decimal withdrawalFeeAmount2 = 234;

            //Act
            var result1 = _historyManager.AddAtOperation(cardNumber, withdrawalAmount, withdrawalFeeAmount);
            var result2 = _historyManager.AddAtOperation(cardNumber, withdrawalAmount2, withdrawalFeeAmount2);
            var result3 = _historyManager.AddAtOperation(cardNumber2, 0, 0);

            //Assert
            var historyCard1 = _historyManager.ATOperationHistory[cardNumber];
            Assert.Equal(cardNumber, historyCard1[result1].CardNumber);
            Assert.Equal(withdrawalAmount, historyCard1[result1].Amount);
            Assert.Equal(withdrawalFeeAmount, historyCard1[result1].Fee);
            Assert.False(historyCard1[result1].OperationCompleted);
            Assert.Equal(result1, historyCard1[result1].OperationId);
            Assert.True((DateTime.UtcNow - historyCard1[result1].OperationDate).Milliseconds < 2000);

            Assert.Equal(cardNumber, historyCard1[result2].CardNumber);
            Assert.Equal(withdrawalAmount2, historyCard1[result2].Amount);
            Assert.Equal(withdrawalFeeAmount2, historyCard1[result2].Fee);
            Assert.False(historyCard1[result2].OperationCompleted);
            Assert.Equal(result2, historyCard1[result2].OperationId);
            Assert.True((DateTime.UtcNow - historyCard1[result2].OperationDate).Milliseconds < 2000);

            var historyCard2 = _historyManager.ATOperationHistory[cardNumber2];

            Assert.Equal(cardNumber2, historyCard2[result3].CardNumber);
            Assert.Equal(0, historyCard2[result3].Amount);
            Assert.Equal(0, historyCard2[result3].Fee);
            Assert.False(historyCard2[result3].OperationCompleted);
            Assert.Equal(result3, historyCard2[result3].OperationId);
            Assert.True((DateTime.UtcNow - historyCard2[result3].OperationDate).Milliseconds < 2000);
        }
               
        [Fact]
        public void TestGetOperation()
        {
            //Arrange
            const string cardNumber = "4343";
            const string cardNumber2 = "12312";
            const decimal withdrawalAmount = 53735;
            const decimal withdrawalFeeAmount = 343;

            const decimal withdrawalAmount2 = 12312;
            const decimal withdrawalFeeAmount2 = 234;

            _historyManager.AddAtOperation(cardNumber, withdrawalAmount, withdrawalFeeAmount);
            var operationId = _historyManager.AddAtOperation(cardNumber, withdrawalAmount2, withdrawalFeeAmount2);
            _historyManager.AddAtOperation(cardNumber2, 0, 0);

            var expectedOperation = _historyManager.ATOperationHistory[cardNumber][operationId];

            //Act
            var result = _historyManager.GetOperation(cardNumber, operationId);

            //Assert
            Assert.Equal(expectedOperation, result);
        }

        [Fact]
        public void TestGetOperationNonExistentThrows()
        {
            //Arrange
            const string cardNumber = "4343";
            const string cardNumber2 = "12312";
            const decimal withdrawalAmount = 53735;
            const decimal withdrawalFeeAmount = 343;

            const decimal withdrawalAmount2 = 12312;
            const decimal withdrawalFeeAmount2 = 234;

            _historyManager.AddAtOperation(cardNumber, withdrawalAmount, withdrawalFeeAmount);
            var operationId = _historyManager.AddAtOperation(cardNumber, withdrawalAmount2, withdrawalFeeAmount2);
            _historyManager.AddAtOperation(cardNumber2, 0, 0);

            var expectedOperation = _historyManager.ATOperationHistory[cardNumber][operationId];

            //Act
            Assert.Throws<HistoryNotFoundException>(() => _historyManager.GetOperation(cardNumber, Guid.NewGuid()));

            //Assert
        }


        [Fact]
        public void TestCompleteOperation()
        {
            //Arrange
            const string cardNumber = "4343";
            const string cardNumber2 = "12312";
            const decimal withdrawalAmount = 53735;
            const decimal withdrawalFeeAmount = 343;

            const decimal withdrawalAmount2 = 12312;
            const decimal withdrawalFeeAmount2 = 234;

            var operationId1 = _historyManager.AddAtOperation(cardNumber, withdrawalAmount, withdrawalFeeAmount);
            var operationId2 = _historyManager.AddAtOperation(cardNumber, withdrawalAmount2, withdrawalFeeAmount2);
            var operationId3 = _historyManager.AddAtOperation(cardNumber2, 0, 0);

            //Act
            _historyManager.CompleteOperation(cardNumber, operationId2);

            //Assert
            Assert.False(_historyManager.ATOperationHistory[cardNumber][operationId1].OperationCompleted);
            Assert.True(_historyManager.ATOperationHistory[cardNumber][operationId2].OperationCompleted);
            Assert.False(_historyManager.ATOperationHistory[cardNumber2][operationId3].OperationCompleted);
        }


        [Fact]
        public void TestGetFeeHistory()
        {
            //Arrange
            const string cardNumber = "4343";
            const string cardNumber2 = "12312";
            const decimal withdrawalAmount = 53735;
            const decimal withdrawalFeeAmount = 343;

            const decimal withdrawalAmount2 = 12312;
            const decimal withdrawalFeeAmount2 = 234;           

            var operationId1 = _historyManager.AddAtOperation(cardNumber, withdrawalAmount, withdrawalFeeAmount);
            var operationId2 = _historyManager.AddAtOperation(cardNumber, withdrawalAmount2, withdrawalFeeAmount2);
            _historyManager.AddAtOperation(cardNumber, 0, 0);
            _historyManager.AddAtOperation(cardNumber2, 0, 0);

            _historyManager.CompleteOperation(cardNumber, operationId1);
            _historyManager.CompleteOperation(cardNumber, operationId2);

            var WithdrawalDate1 = _historyManager.ATOperationHistory[cardNumber][operationId1].OperationDate;
            var WithdrawalDate2 = _historyManager.ATOperationHistory[cardNumber][operationId2].OperationDate;

            //Act
            var result = _historyManager.GetFeeHistory(cardNumber);

            //Assert
            Assert.Collection(
                result,
                r =>
                {
                    Assert.Equal(cardNumber, r.CardNumber);
                    Assert.Equal(withdrawalFeeAmount, r.WithdrawalFeeAmount);
                    Assert.Equal(WithdrawalDate1, r.WithdrawalDate);
                },
                r =>
                {
                    Assert.Equal(cardNumber, r.CardNumber);
                    Assert.Equal(withdrawalFeeAmount2, r.WithdrawalFeeAmount);
                    Assert.Equal(WithdrawalDate2, r.WithdrawalDate);
                });
        }  
    }
}
