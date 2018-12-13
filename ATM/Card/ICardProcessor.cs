using ATM.HostProcessor.Struct;
using System;
using System.Collections.Generic;

namespace ATM.Card
{

    /// <summary>
    /// Encapsulates operations with a bank card and stores
    /// the current card state
    /// </summary>
    public interface ICardProcessor
    {
        /// <summary>
        /// Information about the last inserted card
        /// </summary>
        CardInfo CardInformation { get; }

        /// <summary>
        /// True if card was not extracted
        /// </summary>
        bool CardIsAccessible { get; }

        /// <summary>
        /// True if current user is an operator
        /// </summary>
        bool AuthorizedOperator { get; }

        /// <summary>
        /// Inserts card
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        void InsertCard(string cardNumber);

        /// <summary>
        /// Extracts card
        /// </summary>
        void ReturnCard();

        /// <summary>
        /// Gets current balance of the inserted card
        /// </summary>
        /// <returns>Balance</returns>
        decimal GetCardBalance();

        /// <summary>
        /// Blocks requested amount on the inserted card
        /// </summary>
        /// <param name="amount">Requested amount</param>
        /// <returns>Operation identifier</returns>
        Guid BlockAmount(int amount);

        /// <summary>
        /// Decreases balance of the inserted card
        /// </summary>
        /// <param name="operationId">Operation identifier</param>
        void WithdrawFromBlocked(Guid operationId);

        /// <summary>
        /// Retrieves the history of charged fees
        /// </summary>
        /// <returns>Fee history</returns>
        List<Fee> RetrieveChargedFees();
    }
}
