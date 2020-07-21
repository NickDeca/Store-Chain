using System;
using System.Linq;
using Store_chain.DataLayer;
using Store_chain.Enums;
using Store_chain.Model;

namespace Store_chain.Data
{
    public class StoreManager
    {
        private StoreChainContext _context { get; set; }
        public StoreManager(StoreChainContext context)
        {
            _context = context;
        }

        public void CreateStoreRow(decimal capital, int transactionKey, StoreCalculationEnum operation)
        {
            TransactionManager transactionManager = new TransactionManager(_context);
            try
            {
                // Get last row in the Store table
                var lastStoreCapital = _context.Store.LastOrDefault();

                Transactions transaction = transactionManager.GeTransactionsById(transactionKey);

                if (lastStoreCapital != null)
                {
                    var finalSum = operation == 0 ? lastStoreCapital.Capital - capital : lastStoreCapital.Capital + capital;

                    _context.Store.Add(new CentralStoreCapital
                    {
                        Capital = finalSum,
                        TransactionKey = transactionKey
                    });
                }
                else if (!_context.Store.Any() || transaction.State != (int)StateEnum.OkState)
                {
                    DateTime timeNow = DateTime.Now;

                    var firstTransaction = new Transactions
                    {
                        RecipientKey = 0,
                        ProviderKey = 0,
                        Capital = capital,
                        ProductKey = 0,
                        DateOfTransaction = timeNow,
                        ProductQuantity = 0,
                        State = (int)StateEnum.OkState,
                        Major = 0, //TODO why did i put it here ?
                        ErrorText = string.Empty,
                    };
                    transactionManager.AddTransaction(firstTransaction);

                    _context.Store.Add(new CentralStoreCapital
                    {
                        Capital = capital,
                        TransactionKey = transactionKey
                    });
                }

                _context.SaveChanges();
            }
            catch (Exception err)
            {
                //TODO error transactions 
                throw err;
            }
        }
    }
}
