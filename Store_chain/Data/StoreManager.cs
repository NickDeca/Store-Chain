using System;
using System.Linq;
using Store_chain.DataLayer;
using Store_chain.Enums;

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
                var lastStoreCapital = _context.CentralStoreCapital.LastOrDefault();

                Transactions transaction = transactionManager.GeTransactionsById(transactionKey);

                // If not the first ever made entity in Store
                if (lastStoreCapital != null)
                {
                    // the last row in StoreCapital is the Final sum in the Store's capital and the transactionKey is the last responsible transaction that changed it
                    var finalSum = operation == 0 ? lastStoreCapital.Capital - capital : lastStoreCapital.Capital + capital;

                    _context.CentralStoreCapital.Add(new CentralStoreCapital
                    {
                        Capital = finalSum,
                        TransactionKey = transactionKey
                    });
                }
                // First Transaction to the Store 
                else if (!_context.CentralStoreCapital.Any() || transaction.State != (int)StateEnum.OkState)
                {
                    if(operation == StoreCalculationEnum.Subtraction)
                        throw new Exception("First ever transaction should be an addition");

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
                        ErrorText = string.Empty,
                    };
                    transactionManager.AddTransaction(firstTransaction);

                    _context.CentralStoreCapital.Add(new CentralStoreCapital
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
