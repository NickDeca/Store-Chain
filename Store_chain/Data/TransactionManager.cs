using System;
using System.Collections.Generic;
using System.Linq;
using Store_chain.Enums;
using Store_chain.Model;

namespace Store_chain.Data
{
    public class TransactionManager
    {
        private StoreChainContext _context;
        public TransactionManager(StoreChainContext context)
        {
            _context = context;
        }

        public Transactions AddTransaction(Transactions transaction)
        {
            _context.Add(transaction);
            _context.SaveChanges();
            return GetTransaction(transaction.RecipientKey, transaction.ProviderKey, transaction.ProductKey, transaction.DateOfTransaction, transaction.Major);
        }

        public Transactions GetTransaction(int recipient, int provider, int? product, DateTime dateTransaction, int major)
        {
            return _context.transactionTable
                .FirstOrDefault(x => x.RecipientKey == recipient &&
                                     x.ProviderKey == provider &&
                                     x.ProductKey == product &&
                                     x.DateOfTransaction == dateTransaction &&
                                     x.State != (int)StateEnum.ErrorState &&
                                     x.Major == major);
        }

        public Transactions GetTransaction(Transactions transaction)
        {
            return _context.transactionTable
                .FirstOrDefault(x => x.RecipientKey == transaction.RecipientKey &&
                                     x.ProviderKey == transaction.ProviderKey &&
                                     x.ProductKey == transaction.ProductKey &&
                                     x.DateOfTransaction == transaction.DateOfTransaction &&
                                     x.State != (int)StateEnum.ErrorState &&
                                     x.Major == transaction.Major);
        }

        public void AddTransactionRange(List<Transactions> transactions)
        {
            _context.transactionTable.AddRange(transactions);
            _context.SaveChanges();
        }

        public IEnumerable<Transactions> GeTransactionsByCustomer(int recipient)
        {
            return _context.transactionTable
                .Where(x => x.RecipientKey == recipient &&
                            x.State != (int)StateEnum.ErrorState);
        }

        public IEnumerable<Transactions> GeTransactionsByProvider(int provider)
        {
            return _context.transactionTable
                .Where(x => x.ProviderKey == provider &&
                            x.State != (int)StateEnum.ErrorState);
        }

        public IEnumerable<Transactions> GeTransactionsByProduct(int product)
        {
            return _context.transactionTable
                .Where(x => x.ProductKey == product &&
                            x.State != (int)StateEnum.ErrorState);
        }

        public Transactions GeTransactionsById(int transactionKey)
        {
            return _context.transactionTable
                .FirstOrDefault(x => x.Id == transactionKey);
        }
    }
}
