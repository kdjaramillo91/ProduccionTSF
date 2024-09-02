using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.DataProviders
{
    public static class DataProviderPrintControl
    {
        private static DBContext db = null;

        public static printcontrol SaveControlPrint(printcontrol vprincontrol)
        {
            db = new DBContext();

            Boolean isnew = false;
            printcontrol printcontrol = db.printcontrol
                                            .Where(x => x.namereport == vprincontrol.namereport 
                                                    && x.id_referencia == vprincontrol.id_referencia 
                                                    && x.optiondescrip == vprincontrol.optiondescrip).FirstOrDefault();

            if (printcontrol == null)
            {
             
                    vprincontrol.printnumber = 1;
                
              
                isnew = true;
            }
            else
            {
                printcontrol.printnumber += 1;
                vprincontrol = printcontrol;

            }




            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {

                try
                {
                    if (isnew)
                    {
                        db.printcontrol.Add(vprincontrol);

                    }
                    else
                    {
                        db.printcontrol.Attach(vprincontrol);
                        db.Entry(vprincontrol).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                    trans.Commit();
                }
                catch (Exception)
                {

                    trans.Rollback();


                }
                trans.Dispose();

            }

            return db.printcontrol.Where(x => x.namereport == vprincontrol.namereport && x.id_referencia == vprincontrol.id_referencia && x.optiondescrip == vprincontrol.optiondescrip).FirstOrDefault();
        }

    }

}