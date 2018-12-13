using ATM.Cash.Enum;
using ATM.Cash.Struct;
using ATM.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace ATM.Cash
{
    /// <summary>
    /// Encapsulates operations with cash and store
    /// the current cash dispenser device state
    /// </summary>
    public class CashProcessor : ICashProcessor
    {
        private Money _cash;

        /// <summary>
        /// Cash in the cash dispenser device
        /// </summary>
        public Money Cash => _cash;

        /// <summary>
        /// Loads money to the ATM
        /// </summary>
        /// <param name="money">Money to load</param>
        public void LoadMoney(Money money)
        {
            Validate(money);

            // Here we assume that ATM cash dispenser is the cassette type 
            // dispenser and that operator changes all cash cassettes at once.
            // That means he extracts all cassetes before he inserts a new ones :)

            // And yes, somewhere here we are most probably communicating with the device
            // Honestly, I have no the slightest idea about the real ATM encashment process >.<

            _cash = money;
        }

        /// <summary>
        /// Retrieves the smallest banknote in the cash dispenser
        /// </summary>
        /// <returns>Minimum available banknote nominal</returns>
        public PaperNote MinDenomination()
        {
            return Cash.Notes.Where(n => n.Value > 0).Min(pn => pn.Key);
        }

        /// <summary>
        /// Withdraws money from the cash dispenser device. The biggest nominals will be withdrawed first
        /// </summary>
        /// <param name="amount">Amount to withdraw</param>
        /// <returns>Withdrawed money info</returns>
        public Money WithdrawMoney(int amount)
        {
            var money = new Money()
            {
                Notes = new Dictionary<PaperNote, int>()
            };

            var remainingAmount = amount;

            var notes = _cash.Notes.OrderByDescending(n => n.Key).ToList();

            //Go through the list of available nominales from the largest to the smallest
            //to form a result money bundle
            notes.ForEach(note =>
            {
                var notesCount = remainingAmount / (int)note.Key;

                if (notesCount > note.Value) notesCount = note.Value;

                money.Notes.Add(note.Key, notesCount);

                _cash.Notes[note.Key] = note.Value - notesCount;

                remainingAmount -= ((int)note.Key * notesCount);
            });

            _cash.Amount -= amount;
            money.Amount = amount;

            //In the real life maybe it would be a good idea here to check that remainingAmount == 0.            

            return money;
        }
                      

        /// <summary>
        /// Validate the correctness of the input data
        /// </summary>
        /// <param name="money">Money</param>
        private void Validate(Money money)
        {
            // Let's check that declared amount is correct
            var realAmount = money.Notes.Sum(n => (int)n.Key * n.Value);

            if (realAmount != money.Amount) throw new InvalidDeclaredAmountException();
        }
    }
}
