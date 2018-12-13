using System;
using System.Collections.Generic;
using ATM.Exceptions;
using ATM.HostProcessor.Struct;

namespace ATM.HostProcessor.Mock
{
    /// <summary>
    /// This class mocks requests to the host processor service
    /// It most likely belongs to the module tests but I decided to leave it here
    /// </summary>
    public class HostProcessorServiceMock : IHostProcessorService
    {
        /// <summary>
        /// % of the withdrawal fee
        /// </summary>
        private const int WithdrawalFee = 1;

        /// <summary>
        /// This is operators card :)
        /// </summary>
        private const string OperatorCardNumber = "5378919127447013";
        private readonly IHistoryManager _historyManager;
        
        public HostProcessorServiceMock(IHistoryManager historyManager)
        {
            _historyManager = historyManager;
        }

        /// <summary>
        /// Here is the list of available cards. Key is a card number, value is a balance
        /// Why to not use Money pattern? Because this doesn't make any difference
        /// in this test task. ATM works with one currency only.
        /// </summary>
        private Dictionary<string, CardBalanceInfo> availableCards = new Dictionary<string, CardBalanceInfo>
        {
           {"5474497414986400", new CardBalanceInfo(1000, 0) },
           {"5460213706709186", new CardBalanceInfo(5000, 5000) },
           {"5372100906627713", new CardBalanceInfo(100000, 55000) },
           {"5157430603601427", new CardBalanceInfo(0, 0) },
           {"5249281006972497", new CardBalanceInfo(10, 0) }
        };

        /// <summary>
        /// Tries to block provided amount plus withdrawal fee. Operator cards cannot be used for this operation
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <param name="amount">Amount to block</param>
        /// <returns>Operation Id</returns>
        public Guid BlockAmount(string cardNumber, int amount)
        {
            if (IsOperatorCard(cardNumber)) throw new NotAuthorizedOperatorException();

            var balanceInfo = GetCardBalanceInfo(cardNumber);

            var fee = CalculateWithdrawalFee(amount);

            var totalAmount = amount + fee;

            if (balanceInfo.EffectiveBalance < totalAmount)
                throw new InsufficientFundsException();

            balanceInfo.BlockFunds(totalAmount);            

            return _historyManager.AddAtOperation(cardNumber, amount, fee);
        }

        /// <summary>
        /// Retrieves card balance
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <returns>Current balance</returns>
        public decimal GetCardBalance(string cardNumber)
        {
            if (IsOperatorCard(cardNumber)) return 0;

            var balanceInfo = GetCardBalanceInfo(cardNumber);

            return balanceInfo.EffectiveBalance;            
        }

        /// <summary>
        /// Validates card and returns the card information
        /// </summary>
        /// <param name="cardNumber">Card Number</param>
        /// <returns>Card information</returns>
        public CardInfo RetrieveCardInfo(string cardNumber)
        {
            ValidateCardNumber(cardNumber);

            return new CardInfo
            {
                CardNumber = cardNumber,
                IsOperator = IsOperatorCard(cardNumber)
            };
        }

        /// <summary>
        /// Reduces card balance from the blocked amount
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <param name="operationId">Operation id</param>
        public void WithdrawFromBlocked(string cardNumber, Guid operationId)
        {
            var balanceInfo = GetCardBalanceInfo(cardNumber);

            var operation = _historyManager.GetOperation(cardNumber, operationId);                       

            var totalAmount = operation.Amount + operation.Fee;

            balanceInfo.DecreaseBalanceAndBlocked(totalAmount);

            _historyManager.CompleteOperation(cardNumber, operationId);
        }

        /// <summary>
        /// Returns a history of charged fees
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <returns>History of the charged fees</returns>
        public List<Fee> RetrieveChargedFees(string cardNumber)
        {
            return _historyManager.GetFeeHistory(cardNumber);
        }
                       
        private CardBalanceInfo GetCardBalanceInfo(string cardNumber)
        {
            if (!availableCards.TryGetValue(cardNumber, out var balanceInfo))
                throw new CardIsNotExistsException(cardNumber);

            return balanceInfo;
        }
               
        private decimal CalculateWithdrawalFee(int amount)
        {
            return Math.Round((decimal)(amount * WithdrawalFee) / 100, 2, MidpointRounding.ToEven);
        }

        private bool IsOperatorCard(string cardNumber)
        {
            return cardNumber == OperatorCardNumber;
        }

        /// <summary>
        /// Validate card number.
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <remarks>
        /// I see no point in implementing Luhn or any other validation here 
        /// as well as "CardValidator" class. Let's just assume that every 
        /// non null/empty string is a good one :) 
        /// </remarks>
        private void ValidateCardNumber(string cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber)) throw new InvalidCardNumberException(cardNumber);

            if (cardNumber == OperatorCardNumber) return;

            if (!availableCards.ContainsKey(cardNumber)) throw new CardIsNotExistsException(cardNumber);
        }        
    }
}
