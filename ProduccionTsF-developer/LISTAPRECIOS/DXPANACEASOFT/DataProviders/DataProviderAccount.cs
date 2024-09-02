using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public static class DataProviderAccount
    {
        private static DBContext db = null;

        public static Account Account(int id)
        {
            db = new DBContext();
            return db.Account.FirstOrDefault(w => w.id == id);
        }

        public static IEnumerable AllAccountMoves()
        {
            db = new DBContext();
            return db.Account.Where(w => w.isMovement).ToList();
        }

        public static IEnumerable AllAccountingAssistantDetailTypes()
        {
            db = new DBContext();
            return db.AccountingAssistantDetailType.ToList();
        }

        public static Account AccountById(int? id)
        {
            db = new DBContext();
            return db.Account.FirstOrDefault(w => w.id == id);
        }

        public static IEnumerable Accounts(int id_account_plan)
        {
            db = new DBContext();
            return db.Account.Where(w => w.id_account_plan == id_account_plan && w.isActive).ToList();
        }

        public static IEnumerable AccountTypeGeneralWithCurrent(int? id_current)
        {
            db = new DBContext();
            return db.AccountTypeGeneral.Where(g => (g.isActive) || g.id == id_current).ToList();
        }

        public static IEnumerable DiscountToDetailApplyToWithCurrent(int? id_current)
        {
            db = new DBContext();
            return db.DiscountToDetailApplyTo.Where(g => (g.isActive) || g.id == id_current).ToList();
        }

        public static IEnumerable BasisForGeneralDiscountsWithCurrent(int? id_current)
        {
            db = new DBContext();
            return db.BasisForGeneralDiscounts.Where(g => (g.isActive) || g.id == id_current).ToList();
        }

        public static AccountType AccountTypeById(int? id)
        {
            db = new DBContext();
            return db.AccountType.FirstOrDefault(w => w.id == id);
        }

        public static IEnumerable AccountTypes()
        {
            db = new DBContext();
            return db.AccountType.Where(w => w.isActive).ToList();
        }

        public static IEnumerable AllAccountTypes()
        {
            db = new DBContext();
            return db.AccountType.ToList();
        }

        public static AccountFor AccountForById(int? id)
        {
            db = new DBContext();
            return db.AccountFor.FirstOrDefault(w => w.id == id);
        }

        public static IEnumerable AccountFors()
        {
            db = new DBContext();
            return db.AccountFor.Where(w => w.isActive).ToList();
        }

        public static IEnumerable AllAccountFors()
        {
            db = new DBContext();
            return db.AccountFor.ToList();
        }

        public static AccountPlan AccountPlanById(int? id)
        {
            db = new DBContext();
            return db.AccountPlan.FirstOrDefault(w => w.id == id);
        }

        public static IEnumerable AccountPlans()
        {
            db = new DBContext();
            return db.AccountPlan.Where(w => w.isActive).ToList();
        }

        public static IEnumerable AllAccountPlans()
        {
            db = new DBContext();
            return db.AccountPlan.ToList();
        }

        public static AccountingAssistantDetailType AccountingAssistantDetailTypeById(int? id)
        {
            db = new DBContext();
            return db.AccountingAssistantDetailType.FirstOrDefault(w => w.id == id);
        }
    }
}