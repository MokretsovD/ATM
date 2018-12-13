using ATM.HostProcessor.Struct;
using System;
using System.Collections.Generic;

namespace ATM.HostProcessor
{
    /// <summary>
    /// This is interface of the HostProcessor service.
    /// As far as I know ATM doesn't work by itself, it is a mere client 
    /// which constantly communicates with external service/services.
    /// </summary>
    public interface IHostProcessorService
    {
        /// <summary>
        /// Validates card and returns card information
        /// </summary>
        /// <param name="cardNumber">Card Number</param>
        /// <returns>Card information</returns>
        CardInfo RetrieveCardInfo(string cardNumber);
        
        /// <summary>
        /// Retrieves card balance
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <returns>Current balance</returns>
        decimal GetCardBalance(string cardNumber);

        /// <summary>
        /// Tries to block provided amount plus withdrawal fee. Operator cards cannot be used for this operation
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <param name="amount">Amount to block</param>
        /// <returns>Operation Id</returns>
        Guid BlockAmount(string cardNumber, int amount);

        /// <summary>
        /// Reduces card balance from the blocked amount
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <param name="operationId">Operation id</param>
        void WithdrawFromBlocked(string cardNumber, Guid operationId);

        /// <summary>
        /// Returns a history of charged fees
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <returns>History of the charged fees</returns>
        List<Fee> RetrieveChargedFees(string cardNumber);
    }
}
