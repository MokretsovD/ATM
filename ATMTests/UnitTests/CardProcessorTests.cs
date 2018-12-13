
using ATM.Card;
using ATM.Exceptions;
using ATM.HostProcessor;
using ATM.HostProcessor.Struct;
using NSubstitute;
using System;
using System.Collections.Generic;
using Xunit;

namespace ATMTests.UnitTests
{
    public class CardProcessorTests
    {
        private readonly IHostProcessorService _hostProcessorService;
        private readonly CardProcessor _cardProcessor;

        public CardProcessorTests()
        {
            _hostProcessorService = Substitute.For<IHostProcessorService>();

            _cardProcessor = new CardProcessor(_hostProcessorService);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestInsertCard(bool isOperator)
        {
            //Arrange
            const string cardNumber = "35434";

            var cardInfo = new CardInfo()
            {
                CardNumber = cardNumber,
                IsOperator = isOperator
            };

            _hostProcessorService.RetrieveCardInfo(cardNumber).Returns(cardInfo);

            //Act
            _cardProcessor.InsertCard(cardNumber);

            //Assert
            _hostProcessorService.Received(1).RetrieveCardInfo(Arg.Is(cardNumber));

            Assert.Equal(cardInfo, _cardProcessor.CardInformation);
            Assert.True(_cardProcessor.CardIsAccessible);
            Assert.Equal(isOperator, _cardProcessor.AuthorizedOperator);
        }


        [Fact]
        public void TestInsertCardInsertedThrows()
        {
            //Arrange
            const string cardNumber = "35434";

            var cardInfo = new CardInfo()
            {
                CardNumber = cardNumber,
                IsOperator = false
            };

            _hostProcessorService.RetrieveCardInfo(cardNumber).Returns(cardInfo);
            _cardProcessor.InsertCard(cardNumber);
            _hostProcessorService.ClearReceivedCalls();

            //Act
            Assert.Throws<CardIsInsertedException>(() => _cardProcessor.InsertCard(cardNumber));

            //Assert
            _hostProcessorService.DidNotReceiveWithAnyArgs().RetrieveCardInfo(Arg.Any<string>());

        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestReturnCard(bool isOperator)
        {
            //Arrange
            const string cardNumber = "35434";

            var cardInfo = new CardInfo()
            {
                CardNumber = cardNumber,
                IsOperator = isOperator
            };

            _hostProcessorService.RetrieveCardInfo(cardNumber).Returns(cardInfo);
            _cardProcessor.InsertCard(cardNumber);

            //Act
            _cardProcessor.ReturnCard();

            //Assert
            Assert.Equal(cardInfo, _cardProcessor.CardInformation);
            Assert.False(_cardProcessor.CardIsAccessible);
            Assert.False(_cardProcessor.AuthorizedOperator);
        }

        [Fact]
        public void TestReturnCardTwiceThrows()
        {
            //Arrange
            const string cardNumber = "35434";

            var cardInfo = new CardInfo()
            {
                CardNumber = cardNumber,
                IsOperator = false
            };

            _hostProcessorService.RetrieveCardInfo(cardNumber).Returns(cardInfo);
            _cardProcessor.InsertCard(cardNumber);
            _cardProcessor.ReturnCard();

            //Act
            Assert.Throws<CannotAccessCardException>(() => _cardProcessor.ReturnCard());

            //Assert
            Assert.False(_cardProcessor.CardIsAccessible);
        }

        [Fact]
        public void TestGetCardBalance()
        {
            //Arrange
            const string cardNumber = "35434";
            const decimal balance = 10052246;

            var cardInfo = new CardInfo()
            {
                CardNumber = cardNumber,
                IsOperator = false
            };

            _hostProcessorService.RetrieveCardInfo(cardNumber).Returns(cardInfo);
            _cardProcessor.InsertCard(cardNumber);

            _hostProcessorService.GetCardBalance(cardNumber).Returns(balance);

            //Act
            var result = _cardProcessor.GetCardBalance();

            //Assert
            _hostProcessorService.Received(1).GetCardBalance(Arg.Is(cardNumber));

            Assert.Equal(balance, result);
        }

        [Fact]
        public void TestGetCardBalanceNotAccesibleThrows()
        {
            //Arrange

            //Act
            Assert.Throws<CannotAccessCardException>(() => _cardProcessor.GetCardBalance());

            //Assert
            _hostProcessorService.DidNotReceiveWithAnyArgs().GetCardBalance(Arg.Any<string>());
        }

        [Fact]
        public void TestBlockAmount()
        {
            //Arrange
            const int amount = 437;

            const string cardNumber = "35434";
            var operationId = Guid.NewGuid();

            var cardInfo = new CardInfo()
            {
                CardNumber = cardNumber,
                IsOperator = false
            };

            _hostProcessorService.RetrieveCardInfo(cardNumber).Returns(cardInfo);
            _cardProcessor.InsertCard(cardNumber);

            _hostProcessorService.BlockAmount(cardNumber, amount).Returns(operationId);

            //Act
            var result = _cardProcessor.BlockAmount(amount);

            //Assert
            _hostProcessorService.Received(1).BlockAmount(Arg.Is(cardNumber), Arg.Is(amount));

            Assert.Equal(operationId, result);
        }

        [Fact]
        public void TestBlockAmountNotAccesibleThrows()
        {
            //Arrange
            const int amount = 437;

            //Act
            Assert.Throws<CannotAccessCardException>(() => _cardProcessor.BlockAmount(amount));

            //Assert
            _hostProcessorService.DidNotReceiveWithAnyArgs().BlockAmount(Arg.Any<string>(), Arg.Any<int>());
        }

        [Fact]
        public void TestWithdrawFromBlocked()
        {
            //Arrange
            const string cardNumber = "35434";
            var operationId = Guid.NewGuid();

            var cardInfo = new CardInfo()
            {
                CardNumber = cardNumber,
                IsOperator = false
            };

            _hostProcessorService.RetrieveCardInfo(cardNumber).Returns(cardInfo);
            _cardProcessor.InsertCard(cardNumber);

            //Act
            _cardProcessor.WithdrawFromBlocked(operationId);

            //Assert
            _hostProcessorService.Received(1).WithdrawFromBlocked(Arg.Is(cardNumber), Arg.Is(operationId));
        }

        [Fact]
        public void TestWithdrawFromBlockedNotAccesibleThrows()
        {
            //Arrange
            var operationId = Guid.NewGuid();

            //Act
            Assert.Throws<CannotAccessCardException>(() => _cardProcessor.WithdrawFromBlocked(operationId));

            //Assert
            _hostProcessorService.DidNotReceiveWithAnyArgs().WithdrawFromBlocked(Arg.Any<string>(), Arg.Any<Guid>());
        }

        [Fact]
        public void TestRetrieveChargedFees()
        {
            //Arrange
            const string cardNumber = "35434";
            var operationId = Guid.NewGuid();

            var cardInfo = new CardInfo()
            {
                CardNumber = cardNumber,
                IsOperator = false
            };

            _hostProcessorService.RetrieveCardInfo(cardNumber).Returns(cardInfo);
            _cardProcessor.InsertCard(cardNumber);

            var fees = new List<Fee>
            {
                new Fee()
                {
                    CardNumber = cardNumber,
                    WithdrawalDate = DateTime.UtcNow,
                    WithdrawalFeeAmount = 1000
                }
            };

            _hostProcessorService.RetrieveChargedFees(cardNumber).Returns(fees);

            //Act
            var result =_cardProcessor.RetrieveChargedFees();

            //Assert
            _hostProcessorService.Received(1).RetrieveChargedFees(Arg.Is(cardNumber));
        }

        [Fact]
        public void TestRetrieveChargedFeesNotAccesibleThrows()
        {
            //Arrange

            //Act
            Assert.Throws<CannotAccessCardException>(() => _cardProcessor.RetrieveChargedFees());

            //Assert
            _hostProcessorService.DidNotReceiveWithAnyArgs().RetrieveChargedFees(Arg.Any<string>());
        }
    }
}
