# ATM
Test task - implement automated teller machine in code using TDD approach.

This simple test task has been given to me by one of the companies.

Required features:
* A client can withdraw cash only if card is inserted
* Machine aloows to withdraw: 5, 10, 20, 50 euro paper notes
* Charge the clients card withdrawal fee 1%
* Keep track of all money that was charged to the clients
* Should be possible for client to check his balance
* Fill ATM machine with cash (operator only)
* In case of error throw exception of different type for each situation
* No need for UI
* The solution should be based on the provided ATM interface

ATM Interface:

```csharp

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
IEnumerable<Fee> RetrieveChargedFees()
}

public struct Money
{
public int Amount { get; set; }
public Dictionary<PaperNote, int> Notes { get; set; }
}

public struct Fee
{
public struct CardNumber { get; set; }
public decimal WithdrawalFeeAmount { get; set; }
public DateTime WithdrawalDate { get; set; }
}

```
