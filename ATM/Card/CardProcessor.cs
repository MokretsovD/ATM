using ATM.Exceptions;
using ATM.HostProcessor;
using ATM.HostProcessor.Struct;
using System;
using System.Collections.Generic;

namespace ATM.Card
{
    /// <summary>
    /// Encapsulates operations with a bank card and stores
    /// the current card state
    /// </summary>
    public class CardProcessor : ICardProcessor
    {
        private readonly IHostProcessorService _hostProcessorService;

        public CardProcessor(IHostProcessorService hostProcessorService)
        {
            _hostProcessorService = hostProcessorService;
        }

        /// <summary>
        /// Information about the last inserted card
        /// </summary>
        public CardInfo CardInformation { get; private set; }

        /// <summary>
        /// True if card was not extracted
        /// </summary>
        public bool CardIsAccessible { get; private set; }

        /// <summary>
        /// Trie if current user is an operator
        /// </summary>
        public bool AuthorizedOperator => CardIsAccessible && CardInformation.IsOperator;

        /// <summary>
        /// Blocks requested amount on the inserted card
        /// </summary>
        /// <param name="amount">Requested amount</param>
        /// <returns>Operation identifier</returns>
        public Guid BlockAmount(int amount)
        {
            if (!CardIsAccessible) throw new CannotAccessCardException();
            return _hostProcessorService.BlockAmount(CardInformation.CardNumber, amount);
        }
               
        /// <summary>
        /// Gets current balance for the inserted card
        /// </summary>
        /// <returns>balance</returns>
        public decimal GetCardBalance()
        {
            if (!CardIsAccessible) throw new CannotAccessCardException();
            return _hostProcessorService.GetCardBalance(CardInformation.CardNumber);
        }

        /// <summary>
        /// Inserts card in card reader
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        public void InsertCard(string cardNumber)
        {
            if (CardIsAccessible) throw new CardIsInsertedException();
            CardInformation = _hostProcessorService.RetrieveCardInfo(cardNumber);
            CardIsAccessible = true;
        }

        /// <summary>
        /// Retrieves the history of charged fees
        /// </summary>
        /// <returns>Fee history</returns>
        public List<Fee> RetrieveChargedFees()
        {
            if (!CardIsAccessible) throw new CannotAccessCardException();
            return _hostProcessorService.RetrieveChargedFees(CardInformation.CardNumber);
        }

        /// <summary>
        /// Removes card from the card reader
        /// </summary>
        public void ReturnCard()
        {
            if (!CardIsAccessible) throw new CannotAccessCardException();
            //Somewhere here we are issuing a command to the card reader device to exract card and waiting for the success/fail state
            CardIsAccessible = false;
        }

        /// <summary>
        /// Decreases balance of the inserted card
        /// </summary>
        /// <param name="operationId">Operation identifier</param>
        public void WithdrawFromBlocked(Guid operationId)
        {
            if (!CardIsAccessible) throw new CannotAccessCardException();
            _hostProcessorService.WithdrawFromBlocked(CardInformation.CardNumber, operationId);
        }
    }
}
