using ATM.Cash;
using ATM.Cash.Enum;
using ATM.Cash.Struct;
using ATM.Exceptions;
using System.Collections.Generic;
using Xunit;

namespace ATMTests.UnitTests
{
    public class CashProcessorTests
    {
        private readonly CashProcessor _cashProcessor;

        public CashProcessorTests()
        {
            _cashProcessor = new CashProcessor();
        }

        [Fact]
        public void TestLoadMoney()
        {
            //Arrange
            var money = new Money()
            {
                Amount = 30250,
                Notes = new Dictionary<PaperNote, int>
                {
                    {PaperNote.Five, 50 },
                    {PaperNote.Ten, 100 },
                    {PaperNote.Twenty, 200 },
                    {PaperNote.Fifty, 500 }
                }
            };

            //Act
            _cashProcessor.LoadMoney(money);
            //Assert

            Assert.Equal(money, _cashProcessor.Cash);
        }

        [Theory]
        [InlineData(30251)]
        [InlineData(30249)]
        [InlineData(-30250)]
        [InlineData(0)]
        public void TestLoadMoneyIncorrectAmountThrows(int amount)
        {
            //Arrange
            var money = new Money()
            {
                Amount = amount,
                Notes = new Dictionary<PaperNote, int>
                {
                    {PaperNote.Five, 50 },
                    {PaperNote.Ten, 100 },
                    {PaperNote.Twenty, 200 },
                    {PaperNote.Fifty, 500 }
                }
            };

            //Act
            Assert.Throws<InvalidDeclaredAmountException>(() => _cashProcessor.LoadMoney(money));

            //Assert
        }

        [Theory]
        [InlineData(50, 100, 200, 500, PaperNote.Five)]
        [InlineData(1, 100, 200, 500, PaperNote.Five)]
        [InlineData(0, 1, 200, 500, PaperNote.Ten)]
        [InlineData(0, 0, 1, 500, PaperNote.Twenty)]
        [InlineData(0, 0, 1, 0, PaperNote.Twenty)]
        [InlineData(0, 0, 0, 1, PaperNote.Fifty)]
        public void TestMinDenomination(int fiveAmount, int tenAmount, int twentyAmount, int fiftyAmount, PaperNote expected)
        {
            //Arrange
            var money = new Money()
            {
                Amount = fiveAmount * 5 + tenAmount * 10 + twentyAmount * 20 + fiftyAmount * 50,
                Notes = new Dictionary<PaperNote, int>
                {
                    {PaperNote.Five, fiveAmount },
                    {PaperNote.Ten, tenAmount },
                    {PaperNote.Twenty, twentyAmount },
                    {PaperNote.Fifty, fiftyAmount }
                }
            };
            _cashProcessor.LoadMoney(money);

            //Act
            var result = _cashProcessor.MinDenomination();
            //Assert

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(30005, 1, 100, 200, 500)]
        [InlineData(30245, 49, 100, 200, 500)]
        [InlineData(25000, 0, 0, 0, 500)]
        [InlineData(25005, 1, 0, 0, 500)]
        [InlineData(25025, 1, 0, 1, 500)]
        [InlineData(25035, 1, 1, 1, 500)]
        [InlineData(50, 0, 0, 0, 1)]
        [InlineData(20, 0, 0, 1, 0)]
        [InlineData(10, 0, 1, 0, 0)]
        [InlineData(5, 1, 0, 0, 0)]
        [InlineData(15, 1, 1, 0, 0)]
        [InlineData(25, 1, 0, 1, 0)]
        [InlineData(30, 0, 1, 1, 0)]
        [InlineData(65, 1, 1, 0, 1)]
        public void TestWithdrawMoney(int amount, int fiveAmount, int tenAmount, int twentyAmount, int fiftyAmount)
        {
            //Arrange
            const int fiveInitAmount = 50;
            const int tenInitAmount = 100;
            const int twentyInitAmount = 200;
            const int fiftyInitAmount = 500;
            const int initAmount = 30250;

            var money = new Money()
            {
                Amount = initAmount,
                Notes = new Dictionary<PaperNote, int>
                {
                    {PaperNote.Five, fiveInitAmount },
                    {PaperNote.Ten, tenInitAmount },
                    {PaperNote.Twenty, twentyInitAmount },
                    {PaperNote.Fifty, fiftyInitAmount }
                }
            };

            _cashProcessor.LoadMoney(money);

            //Act
            var result = _cashProcessor.WithdrawMoney(amount);

            //Assert
            Assert.Equal(amount, result.Amount);
            Assert.Equal(fiveAmount, result.Notes[PaperNote.Five]);
            Assert.Equal(tenAmount, result.Notes[PaperNote.Ten]);
            Assert.Equal(twentyAmount, result.Notes[PaperNote.Twenty]);
            Assert.Equal(fiftyAmount, result.Notes[PaperNote.Fifty]);
            Assert.Equal(initAmount - amount, _cashProcessor.Cash.Amount);
            Assert.Equal(fiveInitAmount - fiveAmount, _cashProcessor.Cash.Notes[PaperNote.Five]);
            Assert.Equal(tenInitAmount - tenAmount, _cashProcessor.Cash.Notes[PaperNote.Ten]);
            Assert.Equal(twentyInitAmount - twentyAmount, _cashProcessor.Cash.Notes[PaperNote.Twenty]);
            Assert.Equal(fiftyInitAmount - fiftyAmount, _cashProcessor.Cash.Notes[PaperNote.Fifty]);
        }

    }
}
