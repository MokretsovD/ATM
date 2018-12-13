
using ATM.Exceptions;
using ATM.HostProcessor.Mock;
using ATM.HostProcessor.Struct;
using NSubstitute;
using System;
using System.Collections.Generic;
using Xunit;

namespace ATMTests.UnitTests
{
    public class HostProcessorServiceMockTests
    {
        private readonly IHistoryManager _historyManager;
        private readonly HostProcessorServiceMock _hostProcessorServiceMock;

        public HostProcessorServiceMockTests()
        {
            _historyManager = Substitute.For<IHistoryManager>();

            _hostProcessorServiceMock = new HostProcessorServiceMock(_historyManager);
        }

        [Theory]
        [InlineData("5378919127447013", true)]
        [InlineData("5372100906627713", false)]
        public void TestRetrieveCardInfo(string cardNumber, bool operatorsCard)
        {
            // Arrange            

            // Act
            var result = _hostProcessorServiceMock.RetrieveCardInfo(cardNumber);

            // Assert
            Assert.Equal(cardNumber, result.CardNumber);
            Assert.Equal(operatorsCard, result.IsOperator);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void TestRetrieveCardInfoEmptyThrows(string cardNumber)
        {
            // Arrange            

            // Act
            Assert.Throws<InvalidCardNumberException>(() => _hostProcessorServiceMock.RetrieveCardInfo(cardNumber));

            // Assert
        }

        [Fact]
        public void TestRetrieveCardInfoNonExistentThrows()
        {
            // Arrange            
            const string cardNumber = "1";

            // Act
            Assert.Throws<CardIsNotExistsException>(() => _hostProcessorServiceMock.RetrieveCardInfo(cardNumber));

            // Assert
        }

        [Theory]
        [InlineData("5378919127447013", 0)]
        [InlineData("5372100906627713", 45000)]
        [InlineData("5249281006972497", 10)]
        [InlineData("5460213706709186", 0)]        
        public void TestGetCardBalance(string cardNumber, decimal balance)
        {
            // Arrange            

            // Act
            var result = _hostProcessorServiceMock.GetCardBalance(cardNumber);

            // Assert
            Assert.Equal(balance, result);
        }

        [Fact]
        public void TestGetCardBalanceNonExistentThrows()
        {
            // Arrange            
            const string cardNumber = "1";

            // Act
            Assert.Throws<CardIsNotExistsException>(() => _hostProcessorServiceMock.GetCardBalance(cardNumber));

            // Assert
        }


        [Fact]
        public void TestBlockAmount()
        {
            // Arrange            
            const string cardNumber = "5372100906627713";
            const int amount = 33;
            const int amount2 = 100;
            const int amount3 = 10000;

            const decimal fee = 0.33M;
            const decimal fee2 = 1M;
            const decimal fee3 = 100M;

            var operationId = Guid.NewGuid();
            var operationId2 = Guid.NewGuid();
            var operationId3 = Guid.NewGuid();
                       
            _historyManager.AddAtOperation(cardNumber, amount, fee).Returns(operationId);
            _historyManager.AddAtOperation(cardNumber, amount2, fee2).Returns(operationId2);
            _historyManager.AddAtOperation(cardNumber, amount3, fee3).Returns(operationId3);

            var initBalance = _hostProcessorServiceMock.GetCardBalance(cardNumber);
            var expectedBalance = initBalance - amount - fee - amount2 - fee2 - amount3 - fee3;

            // Act
            var result = _hostProcessorServiceMock.BlockAmount(cardNumber, amount);
            var result2 = _hostProcessorServiceMock.BlockAmount(cardNumber, amount2);
            var result3 = _hostProcessorServiceMock.BlockAmount(cardNumber, amount3);
            
            // Assert
            Assert.Equal(operationId, result);
            Assert.Equal(operationId2, result2);
            Assert.Equal(operationId3, result3);

            Received.InOrder(() =>
            {
                _historyManager.AddAtOperation(Arg.Is(cardNumber), Arg.Is((decimal)amount), Arg.Is(fee));
                _historyManager.AddAtOperation(Arg.Is(cardNumber), Arg.Is((decimal)amount2), Arg.Is(fee2));
                _historyManager.AddAtOperation(Arg.Is(cardNumber), Arg.Is((decimal)amount3), Arg.Is(fee3));
            });

            var resultBalance = _hostProcessorServiceMock.GetCardBalance(cardNumber);

            Assert.Equal(expectedBalance, resultBalance);
        }


        [Fact]
        public void TestBlockAmountInsufficientFundsThrows()
        {
            // Arrange            
            const string cardNumber = "5372100906627713";
            const int amount = 44554;
            const int amount2 = 1;

            // Act
             _hostProcessorServiceMock.BlockAmount(cardNumber, amount);
            Assert.Throws<InsufficientFundsException>(() => _hostProcessorServiceMock.BlockAmount(cardNumber, amount2));

            // Assert           
        }

        [Fact]
        public void TestBlockAmountOperatorCardThrows()
        {
            // Arrange            
            const string cardNumber = "5378919127447013";
            const int amount = 44554;

            // Act
            Assert.Throws<NotAuthorizedOperatorException>(() => _hostProcessorServiceMock.BlockAmount(cardNumber, amount));

            // Assert     
            _historyManager.DidNotReceiveWithAnyArgs().AddAtOperation(Arg.Any<string>(), Arg.Any<decimal>(), Arg.Any<decimal>());
        }

        [Fact]
        public void TestBlockAmountNonExistentCardThrows()
        {
            // Arrange            
            const string cardNumber = "1";
            const int amount = 44554;

            // Act
            Assert.Throws<CardIsNotExistsException>(() => _hostProcessorServiceMock.BlockAmount(cardNumber, amount));

            // Assert     
            _historyManager.DidNotReceiveWithAnyArgs().AddAtOperation(Arg.Any<string>(), Arg.Any<decimal>(), Arg.Any<decimal>());
        }

        [Fact]
        public void TestWithdrawFromBlocked()
        {
            // Arrange            
            const string cardNumber = "5372100906627713";
            const int amount = 100;
            const decimal fee = 1M;

            var operationDate = new DateTime(2018, 09, 12);

            var operationId = Guid.NewGuid();

            var operation = new AtmOperation(cardNumber, operationId, amount, fee, operationDate);

            _historyManager.GetOperation(cardNumber, operationId).Returns(operation);

            var initialBalance = _hostProcessorServiceMock.GetCardBalance(cardNumber);
            var expectedBalance = initialBalance - amount - fee;

            _hostProcessorServiceMock.BlockAmount(cardNumber, amount);

            // Act

            _hostProcessorServiceMock.WithdrawFromBlocked(cardNumber, operationId);

            // Assert
            Received.InOrder(() =>
            {
                _historyManager.GetOperation(Arg.Is(cardNumber), Arg.Is(operationId));
                _historyManager.CompleteOperation(Arg.Is(cardNumber), Arg.Is(operationId));
            });

            var resultBalance = _hostProcessorServiceMock.GetCardBalance(cardNumber);

            Assert.Equal(expectedBalance, resultBalance);
        }

        [Fact]
        public void TestWithdrawFromBlockedNonExistentCardThrows()
        {
            // Arrange            
            const string cardNumber = "1";    
            var operationId = Guid.NewGuid();

            // Act
            Assert.Throws<CardIsNotExistsException>(() => _hostProcessorServiceMock.WithdrawFromBlocked(cardNumber, operationId));

            // Assert
            _historyManager.DidNotReceiveWithAnyArgs().CompleteOperation(Arg.Any<string>(), Arg.Any<Guid>());
        }

        [Fact]
        public void TestRetrieveChargedFees()
        {
            //Arrange
            const string cardNumber = "5372100906627713";

            var feeHistory = new List<Fee>
            {
                new Fee
                {
                    CardNumber = cardNumber,
                    WithdrawalDate = DateTime.UtcNow,
                    WithdrawalFeeAmount = 100500
                }
            };

            _historyManager.GetFeeHistory(cardNumber).Returns(feeHistory);

            //Act
            var result = _hostProcessorServiceMock.RetrieveChargedFees(cardNumber);

            //Assert
            _historyManager.Received(1).GetFeeHistory(Arg.Is(cardNumber));

            Assert.Equal(feeHistory, result);
        }       
    }
}
