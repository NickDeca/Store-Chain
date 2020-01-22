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
            return GetTransaction(transaction.CustomerKey, transaction.ProductKey, transaction.DateOfTransaction);
        }

        public Transactions GetTransaction(int? customer, int? product, DateTime dateTransaction)
        {
            return _context.transactionTable
                .FirstOrDefault(x => x.CustomerKey == customer &&
                            x.ProductKey == product &&
                            x.DateOfTransaction == dateTransaction &&
                            x.State != (int)StateEnum.ErrorState);
        }

        public void AddTransactionRange(List<Transactions> transactions)
        {
            _context.transactionTable.AddRange(transactions);
            _context.SaveChanges();
        }

        public IEnumerable<Transactions> GeTransactionsByCustomer(int customer)
        {
            return _context.transactionTable
                .Where(x => x.CustomerKey == customer &&
                            x.State != (int)StateEnum.ErrorState);
        }

        public IEnumerable<Transactions> GeTransactionsByProduct(int product)
        {
            return _context.transactionTable
                .Where(x => x.ProductKey == product &&
                            x.State != (int)StateEnum.ErrorState);
        }
    }
}
