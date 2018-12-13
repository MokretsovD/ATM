using ATM;
using ATM.Card;
using ATM.Cash;
using ATM.Cash.Enum;
using ATM.Cash.Struct;
using ATM.Exceptions;
using NSubstitute;
using System;
using System.Collections.Generic;
using Xunit;

namespace ATMTests.UnitTests
{       
    public class ATMachineTests
    {
        private readonly ICardProcessor _cardProcessor;
        private readonly ICashProcessor _cashProcessor;
        private readonly ATMachine _atMachine;
        
        public ATMachineTests()
        {

            _cardProcessor = Substitute.For<ICardProcessor>();
            _cashProcessor = Substitute.For<ICashProcessor>();

            _atMachine = new ATMachine(_cardProcessor, _cashProcessor);
        }

        [Fact]
        public void TestManufacturer()
        {
            //Arrange
            
            //Act
            var result = _atMachine.Manufacturer;

            //Assert
            Assert.Equal(UnitInfo.Manufacturer, result);
        }

        [Fact]
        public void TestSerialNumber()
        {
            //Arrange

            //Act
            var result = _atMachine.SerialNumber;

            //Assert
            Assert.Equal(UnitInfo.SerialNumber, result);
        }

        [Fact]
        public void TestInsertCard()
        {
            //Arrange
            const string cardNumber = "3543734";

            //Act
            _atMachine.InsertCard(cardNumber);

            //Assert
            _cardProcessor.Received(1).InsertCard(Arg.Is(cardNumber));
        }

        [Fact]
        public void TestGetCardBalance()
        {
            //Arrange
            const decimal balance = 100500;

            _cardProcessor.GetCardBalance().Returns(balance);
            //Act
            var result = _atMachine.GetCardBalance();

            //Assert
            _cardProcessor.Received(1).GetCardBalance();

            Assert.Equal(balance, result);
        }

        [Fact]
        public void TestWithdrawMoney()
        {
            //Arrange
            const int amount = 100500;
            const decimal balance = 200500;
            var operationId = Guid.NewGuid();


            var allCash = new Money
            {
                Amount = 240000,
                Notes = new Dictionary<PaperNote, int>
                {
                    { PaperNote.Twenty, 2000 },
                    { PaperNote.Fifty, 4000 }
                }
            };

            var resultCash = new Money
            {
                Amount = amount,
                Notes = new Dictionary<PaperNote, int>
                {
                    { PaperNote.Fifty, 2010 }
                }
            };

            _cashProcessor.Cash.Returns(allCash);
            _cashProcessor.MinDenomination().Returns(PaperNote.Twenty);
            _cashProcessor.WithdrawMoney(amount).Returns(resultCash);
            _cardProcessor.GetCardBalance().Returns(balance);
            _cardProcessor.BlockAmount(amount).Returns(operationId);

            //Act
            var result = _atMachine.WithdrawMoney(amount);

            //Assert

            Received.InOrder(() =>
            {
                _cashProcessor.MinDenomination();
                _cardProcessor.GetCardBalance();
                _cardProcessor.BlockAmount(Arg.Is(amount));
                _cashProcessor.WithdrawMoney(Arg.Is(amount));
                _cardProcessor.WithdrawFromBlocked(Arg.Is(operationId));
            });

            Assert.Equal(resultCash, result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(int.MinValue)]
        public void TestWithdrawMoneyZeroOrNegativeAmountThrows(int amount)
        {
            //Arrange

            //Act
            Assert.Throws<InvalidAmountException>(() => _atMachine.WithdrawMoney(amount));

            //Assert
            _cashProcessor.DidNotReceive().MinDenomination();
            _cardProcessor.DidNotReceive().GetCardBalance();
            _cardProcessor.DidNotReceiveWithAnyArgs().BlockAmount(Arg.Any<int>());
            _cashProcessor.DidNotReceiveWithAnyArgs().WithdrawMoney(Arg.Any<int>());
            _cardProcessor.DidNotReceiveWithAnyArgs().WithdrawFromBlocked(Arg.Any<Guid>());
        }

        [Fact]
        public void TestWithdrawMoneyTooHighAmountThrows()
        {
            //Arrange
            const int amount = 240001;

            var allCash = new Money
            {
                Amount = 240000,
                Notes = new Dictionary<PaperNote, int>
                {
                    { PaperNote.Twenty, 2000 },
                    { PaperNote.Fifty, 4000 }
                }
            };

            _cashProcessor.Cash.Returns(allCash);

            //Act
            Assert.Throws<AmountIsTooBigException>(() => _atMachine.WithdrawMoney(amount));

            //Assert
            _cashProcessor.DidNotReceive().MinDenomination();
            _cardProcessor.DidNotReceive().GetCardBalance();
            _cardProcessor.DidNotReceiveWithAnyArgs().BlockAmount(Arg.Any<int>());
            _cashProcessor.DidNotReceiveWithAnyArgs().WithdrawMoney(Arg.Any<int>());
            _cardProcessor.DidNotReceiveWithAnyArgs().WithdrawFromBlocked(Arg.Any<Guid>());
        }

        [Theory]
        [InlineData(239951, PaperNote.Fifty)]
        [InlineData(239949, PaperNote.Fifty)]
        [InlineData(239991, PaperNote.Twenty)]
        [InlineData(239989, PaperNote.Twenty)]
        [InlineData(21, PaperNote.Twenty)]
        [InlineData(19, PaperNote.Twenty)]
        public void TestWithdrawMoneyNotMultipleAmountThrows(int amount, PaperNote minNote)
        {
            //Arrange
            var allCash = new Money
            {
                Amount = 240000,
                Notes = new Dictionary<PaperNote, int>
                {
                    { PaperNote.Twenty, 2000 },
                    { PaperNote.Fifty, 4000 }
                }
            };

            _cashProcessor.Cash.Returns(allCash);
            _cashProcessor.MinDenomination().Returns(minNote);

            //Act
            Assert.Throws<AmountIsNotMultipleException>(() => _atMachine.WithdrawMoney(amount));

            //Assert
            _cardProcessor.DidNotReceive().GetCardBalance();
            _cardProcessor.DidNotReceiveWithAnyArgs().BlockAmount(Arg.Any<int>());
            _cashProcessor.DidNotReceiveWithAnyArgs().WithdrawMoney(Arg.Any<int>());
            _cardProcessor.DidNotReceiveWithAnyArgs().WithdrawFromBlocked(Arg.Any<Guid>());
        }

        [Fact]
        public void TestWithdrawMoneyTooLowBalanceThrows()
        {
            //Arrange
            const int amount = 240000;
            const decimal balance = 239000;

            var allCash = new Money
            {
                Amount = 240000,
                Notes = new Dictionary<PaperNote, int>
                {
                    { PaperNote.Twenty, 2000 },
                    { PaperNote.Fifty, 4000 }
                }
            };

            _cashProcessor.Cash.Returns(allCash);
            _cashProcessor.MinDenomination().Returns(PaperNote.Twenty);
            _cardProcessor.GetCardBalance().Returns(balance);

            //Act
            Assert.Throws<InsufficientFundsException>(() => _atMachine.WithdrawMoney(amount));

            //Assert
            _cardProcessor.DidNotReceiveWithAnyArgs().BlockAmount(Arg.Any<int>());
            _cashProcessor.DidNotReceiveWithAnyArgs().WithdrawMoney(Arg.Any<int>());
            _cardProcessor.DidNotReceiveWithAnyArgs().WithdrawFromBlocked(Arg.Any<Guid>());
        }

        [Fact]
        public void TestReturnCard()
        {
            //Arrange

            //Act
            _atMachine.ReturnCard();

            //Assert
            _cardProcessor.Received(1).ReturnCard();
        }

        [Fact]
        public void TestLoadMoney()
        {
            //Arrange

            var money = new Money
            {
                Amount = 240000,
                Notes = new Dictionary<PaperNote, int>
                {
                    { PaperNote.Twenty, 2000 },
                    { PaperNote.Fifty, 4000 }
                }
            };

            _cardProcessor.AuthorizedOperator.Returns(true);

            //Act
            _atMachine.LoadMoney(money);

            //Assert
            _cashProcessor.Received(1).LoadMoney(Arg.Is(money));
        }


        [Fact]
        public void TestLoadMoneyNotOperatorThrows()
        {
            //Arrange

            var money = new Money
            {
                Amount = 240000,
                Notes = new Dictionary<PaperNote, int>
                {
                    { PaperNote.Twenty, 2000 },
                    { PaperNote.Fifty, 4000 }
                }
            };

            _cardProcessor.AuthorizedOperator.Returns(false);

            //Act
            Assert.Throws<NotAuthorizedOperatorException>(() => _atMachine.LoadMoney(money));

            //Assert
            _cashProcessor.DidNotReceiveWithAnyArgs().LoadMoney(Arg.Any<Money>());
        }
    }
}
