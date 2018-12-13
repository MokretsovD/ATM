using System;

namespace ATM.HostProcessor.Mock
{
    /// <summary>
    /// Represents ATM operation. In this case only a withdrawal operation :)
    /// </summary>
    public class AtmOperation
    {
        /// <summary>
        /// Operation identifier
        /// </summary>
        public Guid OperationId { get; }
        /// <summary>
        /// Number of card
        /// </summary>
        public string CardNumber { get; }
        /// <summary>
        /// Charged fee
        /// </summary>
        public decimal Fee { get; }
        /// <summary>
        /// Operation Date
        /// </summary>
        public DateTime OperationDate { get; private set; }
        /// <summary>
        /// Withdrawed amount
        /// </summary>
        public decimal Amount { get; }
        /// <summary>
        /// Indicates that operation has been completed succefully        
        /// </summary>
        /// <remarks>This could be OperationStatus instead. But for the simplicity sake I made it like this</remarks>
        public bool OperationCompleted { get; private set; }
        
        public AtmOperation(string cardNumber, Guid operationId, decimal amount, decimal fee, DateTime operationDate)
        {
            OperationDate = operationDate;
            CardNumber = cardNumber;
            OperationId = operationId;
            Fee = fee;
            Amount = amount;
            OperationCompleted = false;
        }

        /// <summary>
        /// Completes the current operation
        /// </summary>
        /// <param name="CompletionDate">Date and time.</param>
        public void SetComplete(DateTime CompletionDate)
        {
            // We can see that current OperationDate will be rewritten by 
            // CompletionDate. Why? The reason is the same - to avoid unnecessary 
            // complexity in the test task where it is possible.
            // Good solution would be to have status history for the operation
            // with date and time for each status change
            OperationDate = CompletionDate;
            OperationCompleted = true;
        }
    }
}
