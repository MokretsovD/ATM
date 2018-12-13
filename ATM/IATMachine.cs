using ATM.Cash.Struct;
using ATM.HostProcessor.Struct;
using System.Collections.Generic;

namespace ATM
{
    public interface IATMachine
    {
        ///<summary>ATM manufacturer.</summary>
        string Manufacturer { get; }
        ///<summary>ATM serial number.</summary>
        string SerialNumber { get; }
        /// <summary>Insert bank card into ATM machine</summary>
        /// <param name="cardNumber">Card number</param>        
        void InsertCard(string cardNumber);
        /// <summary>Retrieves the balance available on the card</summary>
        decimal GetCardBalance();
        ///<summary>Withdraw money from ATM.</summary>
        ///<paramname="amount">Amount of money to withdraw.</param>
        Money WithdrawMoney(int amount);
        ///<summary>Returns card back to client.</summary>
        void ReturnCard();
        /// <summary>Load the money into ATM machine</summary>
        /// <param name="money">Money loaded into ATM machine</param>
        void LoadMoney(Money money);
        /// <summary>Retrieves charged fees</summary>
        /// <returns>List of charged fees</returns>
        IEnumerable<Fee> RetrieveChargedFees();
    }
}
