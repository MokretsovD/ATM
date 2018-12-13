using System.Collections.Generic;
using ATM.Card;
using ATM.Cash;
using ATM.Cash.Struct;
using ATM.Exceptions;
using ATM.HostProcessor.Struct;

namespace ATM
{
    public class ATMachine : IATMachine
    {
        private readonly ICardProcessor _cardProcessor;
        private readonly ICashProcessor _cashProcessor;

        public ATMachine(
            ICardProcessor cardProcessor,
            ICashProcessor cashProcessor)
        {
            _cardProcessor = cardProcessor;
            _cashProcessor = cashProcessor;
        }

        ///<summary>ATM manufacturer.</summary>
        public string Manufacturer => UnitInfo.Manufacturer;
        ///<summary>ATM serial number.</summary>
        public string SerialNumber => UnitInfo.SerialNumber;

        /// <summary>Retrieves the balance available on the card</summary>
        public decimal GetCardBalance()
        {
            return _cardProcessor.GetCardBalance();
        }

        /// <summary>
        /// Insert bank card into ATM machine
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <remarks> In the real ATM, this method would be called most likely after bank card 
        /// is inserted in the card reader device and the device was able to read it successfully
        /// (to retrieve the card number at the least :) ).</remarks>
        public void InsertCard(string cardNumber)
        {
            _cardProcessor.InsertCard(cardNumber);
        }

        /// <summary>Load the money into ATM machine</summary>
        /// <param name="money">Money loaded into ATM machine</param>
        public void LoadMoney(Money money)
        {
            if (!_cardProcessor.AuthorizedOperator)
                throw new NotAuthorizedOperatorException();

            _cashProcessor.LoadMoney(money);
        }

        /// <summary>Retrieves charged fees</summary>
        /// <returns>List of charged fees</returns>
        public IEnumerable<Fee> RetrieveChargedFees()
        {
            return _cardProcessor.RetrieveChargedFees();
        }

        ///<summary>
        ///Returns card back to client.
        ///</summary>
        public void ReturnCard()
        {
            _cardProcessor.ReturnCard();
        }

        ///<summary>Withdraw money from ATM.</summary>
        ///<paramname="amount">Amount of money to withdraw.</param>
        public Money WithdrawMoney(int amount)
        {
            WithdrawalValidation(amount);

            var operationId = _cardProcessor.BlockAmount(amount);
            var money = _cashProcessor.WithdrawMoney(amount);
            _cardProcessor.WithdrawFromBlocked(operationId);

            return money;
        }

        /// <summary>
        /// Preliminary checks the possiblity of withdrawal
        /// </summary>
        /// <param name="amount">Requested amount</param>
        private void WithdrawalValidation(int amount)
        {
            if (amount <= 0) throw new InvalidAmountException();

            if (amount > _cashProcessor.Cash.Amount) throw new AmountIsTooBigException();

            var minBankNote = (int)_cashProcessor.MinDenomination();

            if (amount % minBankNote != 0) throw new AmountIsNotMultipleException(minBankNote);

            if (amount > _cardProcessor.GetCardBalance()) throw new InsufficientFundsException();            
        }
    }
}
