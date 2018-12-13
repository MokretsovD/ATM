using ATM.Exceptions;

namespace ATM.HostProcessor.Mock
{
    public class CardBalanceInfo
    {
        public decimal Balance { get; private set; }
        public decimal Blocked { get; private set; }
        public decimal EffectiveBalance => Balance - Blocked;

        public CardBalanceInfo(decimal balance, decimal blocked)
        {
            Balance = balance;
            Blocked = blocked;
        }

        public void DecreaseBalanceAndBlocked(decimal amount)
        {
            if (amount < 0 || Blocked < amount) throw new InvalidAmountException();
            Balance -= amount;
            Blocked -= amount;
        }

        public void BlockFunds(decimal amount)
        {
            Blocked += amount;
        }
    }
}
