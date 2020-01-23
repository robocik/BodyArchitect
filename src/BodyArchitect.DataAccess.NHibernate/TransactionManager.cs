using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BodyArchitect.Logger;
using NHibernate;

namespace BodyArchitect.DataAccess.NHibernate
{
    public class TransactionManager : IDisposable
    {
        private ITransaction transaction;
        private bool disposed;
        private bool transactionStarted = false;

        public TransactionManager()
            : this(false)
        {

        }

        public TransactionManager(bool startTransaction)
            : this(startTransaction, IsolationLevel.ReadCommitted)
        {
        }

        public TransactionManager(bool startTransaction, IsolationLevel isolationLevel)
        {
            if (startTransaction)
            {
                BeginTransaction(isolationLevel);
            }
        }


        /// <summary>
        /// Starts new transaction. The session is now running in the transaction with
        /// isolation level seted to ReadCommitted
        /// </summary>
        public void BeginTransaction()
        {
            BeginTransaction(IsolationLevel.ReadCommitted);
        }

        /// <summary>
        /// Starts new transaction. The session is now running in the transaction
        /// </summary>
        /// <param name="isolationLevel">The isolation level of the transaction.</param>
        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            verifyObjectState();

            try
            {
                transactionStarted = true;
                if (!NHibernateContext.Current().Session.Transaction.IsActive)
                {
                    transaction = NHibernateContext.Current().Session.BeginTransaction(isolationLevel);
                }
            }
            catch (Exception ex)
            {
                transactionStarted = false;
                ExceptionHandler.Default.Process(ex);
                throw;
            }
        }

        /// <summary>
        /// Commits current transaction
        /// </summary>
        public void CommitTransaction()
        {
            verifyObjectState();
            try
            {
                if (transaction != null)
                {
                    transaction.Commit();
                    transaction = null;
                }
            }
            finally
            {
                transactionStarted = false;
            }

        }

        /// <summary>
        /// Rollbacks current transaction
        /// </summary>
        public void RollbackTransaction()
        {
            verifyObjectState();
            try
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                    transaction = null;
                }
            }
            finally
            {
                transactionStarted = false;
            }
        }

        /// <summary>
        /// Gets informations indicating whether the transaction is running
        /// </summary>
        public bool IsTransactionRunning
        {
            get
            {
                verifyObjectState();
                return transaction != null && transaction.IsActive;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (!disposed && transactionStarted)
            {
                RollbackTransaction();
            }
            disposed = true;
        }

        #endregion

        private void verifyObjectState()
        {
            if (disposed)
            {
                throw new InvalidOperationException("Cannot use disposed object");
            }
        }
    }
}
