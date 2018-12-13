using ATM.HostProcessor.Struct;
using System;
using System.Collections.Generic;

namespace ATM.HostProcessor.Mock
{
    public interface IHistoryManager
    {
        Dictionary<string, Dictionary<Guid, AtmOperation>> ATOperationHistory { get; }

        /// <summary>
        /// Retrieves the history of fees
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <returns>Fees collection</returns>
        List<Fee> GetFeeHistory(string cardNumber);

        /// <summary>
        /// Adds a withdrawal operation
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <param name="withdrawalAmount">Withdrawal amount</param>
        /// <param name="withdrawalFeeAmount">Fee amount</param>
        /// <returns>Operation identifier</returns>
        /// <remarks>
        /// Where is operation type? Naturally it should be here, but I omitted it to finish the task faster. :)
        /// Also data structure inside was affected by this decision
        /// </remarks>
        Guid AddAtOperation(string cardNumber, decimal withdrawalAmount, decimal withdrawalFeeAmount);

        /// <summary>
        /// Marks an operation as completed
        /// </summary>
        /// <param name="cardNumber">Card nulmer</param>
        /// <param name="operationId">Operation</param>
        void CompleteOperation(string cardNumber, Guid operationId);

        /// <summary>
        /// Gets operation from history
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <param name="operationId">Operation Id</param>
        /// <returns>Operation</returns>
        AtmOperation GetOperation(string cardNumber, Guid operationId);
    }
}
