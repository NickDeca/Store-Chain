using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Store_chain.DataLayer;
using Store_chain.Enums;

namespace Store_chain.Data.Managers
{
    public class TransactionManager
    {
        private StoreChainContext _context;
        public TransactionManager(StoreChainContext context)
        {
            _context = context;
        }

        public async Task<Transactions> AddTransaction(Transactions transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return GetTransaction(transaction.RecipientKey, transaction.ProviderKey, transaction.ProductKey, transaction.DateOfTransaction);
        }

        public Transactions GetTransaction(int recipient, int provider, int? product, DateTime dateTransaction)
        {
            var value = _context.Transactions
                .FirstOrDefault(x => x.RecipientKey == recipient &&
                                     x.ProviderKey == provider &&
                                     x.ProductKey == product &&
                                     x.DateOfTransaction == dateTransaction);

            return value;
        }

        public Transactions GetTransaction(Transactions transaction)
        {
            return _context.Transactions
                .FirstOrDefault(x => x.RecipientKey == transaction.RecipientKey &&
                                     x.ProviderKey == transaction.ProviderKey &&
                                     x.ProductKey == transaction.ProductKey &&
                                     x.DateOfTransaction == transaction.DateOfTransaction);
        }

        public async Task AddTransactionRangeAsync(List<Transactions> transactions)
        {
            _context.Transactions.AddRange(transactions);
            await _context.SaveChangesAsync();
        }

        public IEnumerable<Transactions> GeTransactionsByCustomer(int recipient)
        {
            return _context.Transactions
                .Where(x => x.RecipientKey == recipient &&
                            x.State != (int)StateEnum.ErrorState);
        }

        public IEnumerable<Transactions> GeTransactionsByProvider(int provider)
        {
            return _context.Transactions
                .Where(x => x.ProviderKey == provider &&
                            x.State != (int)StateEnum.ErrorState);
        }

        public IEnumerable<Transactions> GeTransactionsByProduct(int product)
        {
            return _context.Transactions
                .Where(x => x.ProductKey == product &&
                            x.State != (int)StateEnum.ErrorState);
        }

        public Transactions GeTransactionsById(int transactionKey)
        {
            return _context.Transactions
                .FirstOrDefault(x => x.Id == transactionKey);
        }
    }
}
