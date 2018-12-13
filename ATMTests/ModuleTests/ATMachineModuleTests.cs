using System;
using System.Collections.Generic;
using ATM;
using ATM.Cash.Enum;
using ATM.Cash.Struct;
using ATM.Exceptions;
using Xunit;

namespace ATMTests.ModuleTests
{
    public class ATMachineModuleTests
    {
        private readonly IATMachine _atMachine;

        private static DiRegistrator DiRegistrator => DiRegistrator.Register();

        public ATMachineModuleTests()
        {
            _atMachine = DiRegistrator.Resolve<IATMachine>();
        }

        [Fact]
        public void TestAtMachineFlow()
        {
            const string operatorCardNumber = "5378919127447013";
            const string cardNumber = "5372100906627713";

            const decimal initUserCardBalance = 45000M;

            const int withdrawAmount = 44000;            
            const int withdrawAmount2 = 200;
            const decimal fee = 440;
            const decimal fee2 = 2;
            const decimal expectedBalance = 45000 - withdrawAmount - withdrawAmount2 - fee - fee2;

            var incorrectMoney = new Money
            {
                Amount = 245001,
                Notes = new Dictionary<PaperNote, int>
                {
                    { PaperNote.Five, 1000 },
                    { PaperNote.Twenty, 2000 },
                    { PaperNote.Fifty, 4000 }
                }
            };

            var money = new Money
            {
                Amount = 245000,
                Notes = new Dictionary<PaperNote, int>
                {
                    { PaperNote.Five, 1000 },
                    { PaperNote.Twenty, 2000 },
                    { PaperNote.Fifty, 4000 }
                }
            };

            var money2 = new Money
            {
                Amount = 5,
                Notes = new Dictionary<PaperNote, int>
                {
                    { PaperNote.Five, 1 }
                }
            };

            // Trying to load money without card
            Assert.Throws<NotAuthorizedOperatorException>(() => _atMachine.LoadMoney(money));

            // Inserting operator card
            _atMachine.InsertCard(operatorCardNumber);

            // Trying to insert card twice :)
            Assert.Throws<CardIsInsertedException>(() => _atMachine.InsertCard(cardNumber));

            // Trying to load incorrect amount of money.
            Assert.Throws<InvalidDeclaredAmountException>(() => _atMachine.LoadMoney(incorrectMoney));

            // Loading money
            _atMachine.LoadMoney(money);

            // Getting operator balance
            var operatorBalance = _atMachine.GetCardBalance();

            Assert.Equal(0, operatorBalance);

            // Trying to wuthdraw money by operator
            Assert.Throws<InsufficientFundsException>(() => _atMachine.WithdrawMoney(5));

            // Returning card
            _atMachine.ReturnCard();

            // Trying to return card twice
            Assert.Throws<CannotAccessCardException>(() => _atMachine.ReturnCard());

            // Trying to get balance without card
            Assert.Throws<CannotAccessCardException>(() => _atMachine.GetCardBalance());

            // Inserting client card
            _atMachine.InsertCard(cardNumber);

            // Trying to load money with client card
            Assert.Throws<NotAuthorizedOperatorException>(() => _atMachine.LoadMoney(money));

            // Trying to get balance
            var balance = _atMachine.GetCardBalance();

            Assert.Equal(initUserCardBalance, balance);

            // Trying to get charged fees
            var fees = _atMachine.RetrieveChargedFees();

            Assert.Null(fees);

            // Trying to get our money
            _atMachine.WithdrawMoney(withdrawAmount);

            // More money
            Assert.Throws<InsufficientFundsException>(() => _atMachine.WithdrawMoney(withdrawAmount));

            // A bit more money
            _atMachine.WithdrawMoney(withdrawAmount2);

            // What was the cost?
            fees = _atMachine.RetrieveChargedFees();

            Assert.Collection(
                fees,
                f =>
                {
                    Assert.Equal(cardNumber, f.CardNumber);
                    Assert.Equal(fee, f.WithdrawalFeeAmount);
                    Assert.True((DateTime.UtcNow - f.WithdrawalDate).Milliseconds < 5000);
                },
                f =>
                {
                    Assert.Equal(cardNumber, f.CardNumber);
                    Assert.Equal(fee2, f.WithdrawalFeeAmount);
                    Assert.True((DateTime.UtcNow - f.WithdrawalDate).Milliseconds < 5000);
                });

            // How much we still have?
            balance = _atMachine.GetCardBalance();

            // =(
            Assert.Equal(expectedBalance, balance);

            // Returning card
            _atMachine.ReturnCard();

            // Reloading ATM
            _atMachine.InsertCard(operatorCardNumber);
            _atMachine.LoadMoney(money2);
            _atMachine.ReturnCard();

            // Trying to get even more money
            _atMachine.InsertCard(cardNumber);
            // ATM has not contain so much money as we want
            Assert.Throws<AmountIsTooBigException>(() => _atMachine.WithdrawMoney(withdrawAmount2));
            _atMachine.ReturnCard();
        }
    }
}
