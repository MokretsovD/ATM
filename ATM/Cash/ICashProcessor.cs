using ATM.Cash.Enum;
using ATM.Cash.Struct;

namespace ATM.Cash
{
    /// <summary>
    /// Encapsulates operations with cash and store
    /// the current cash dispenser device state
    /// </summary>
    public interface ICashProcessor
    {
        /// <summary>
        /// Cash in the cash dispenser device
        /// </summary>
        Money Cash { get; }

        /// <summary>
        /// Loads money to the ATM
        /// </summary>
        /// <param name="money">Money to load</param>
        void LoadMoney(Money money);

        /// <summary>
        /// Retrieves the smallest banknote in the cash dispenser
        /// </summary>
        /// <returns>Minimum available banknote nominal</returns>
        PaperNote MinDenomination();

        /// <summary>
        /// Withdraws money from the cash dispenser device
        /// </summary>
        /// <param name="amount">Amount to withdraw</param>
        /// <returns>Withdrawed money info</returns>
        Money WithdrawMoney(int amount);
    }
}
