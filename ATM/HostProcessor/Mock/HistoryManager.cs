using ATM.Exceptions;
using ATM.HostProcessor.Struct;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ATM.HostProcessor.Mock
{    
    public class HistoryManager : IHistoryManager
    {
        public Dictionary<string, Dictionary<Guid, AtmOperation>> ATOperationHistory { get; } = new Dictionary<string, Dictionary<Guid, AtmOperation>>();

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
        public Guid AddAtOperation(string cardNumber, decimal withdrawalAmount, decimal withdrawalFeeAmount)
        {
            if (!ATOperationHistory.TryGetValue(cardNumber, out var operationHistory))
            {
                operationHistory = new Dictionary<Guid, AtmOperation>();
                ATOperationHistory.Add(cardNumber, operationHistory);
            }

            var operationId = Guid.NewGuid();

            var atOperation = new AtmOperation(
                cardNumber,
                operationId,
                withdrawalAmount,
                withdrawalFeeAmount,
                DateTime.UtcNow);

            operationHistory.Add(operationId, atOperation);

            return operationId;
        }

        /// <summary>
        /// Marks an operation as completed
        /// </summary>
        /// <param name="cardNumber">Card nulmer</param>
        /// <param name="operationId">Operation</param>
        public void CompleteOperation(string cardNumber, Guid operationId)
        {
            var operation = GetOperation(cardNumber, operationId);

            if (operation.OperationCompleted) return;

            operation.SetComplete(DateTime.UtcNow);            
        }

        /// <summary>
        /// Gets operation from history
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <param name="operationId">Operation Id</param>
        /// <returns>Operation</returns>
        public AtmOperation GetOperation(string cardNumber, Guid operationId)
        {
            if (!ATOperationHistory.TryGetValue(cardNumber, out var operationHistory) ||
                !operationHistory.TryGetValue(operationId, out var operation))
                throw new HistoryNotFoundException();
            return operation;
        }

        /// <summary>
        /// Retrieves the history of fees
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <returns>Fees collection</returns>
        public List<Fee> GetFeeHistory(string cardNumber)
        {
            ATOperationHistory.TryGetValue(cardNumber, out var cardHistory);

            return cardHistory?
                .Where(ch => ch.Value.OperationCompleted)
                .Select(ch => new Fee()
                {
                    CardNumber = ch.Value.CardNumber,
                    WithdrawalDate = ch.Value.OperationDate,
                    WithdrawalFeeAmount = ch.Value.Fee
                }).ToList();
        }        
    }
}
