namespace DXPANACEASOFT.WORKERS.Services
{
    public interface IServiceValidationProcessExecution
    {
        bool Execute();
    }
    public class ServiceValidationProcessExecution : IServiceValidationProcessExecution
    {
        private readonly IConfiguration _configuration;
        public ServiceValidationProcessExecution(IConfiguration configuration) 
        {
            _configuration = configuration;
        }

        public bool Execute() 
        {
            bool canContinue = false;
            BackgroundProcess? bp = null;
            try 
            {
                using (ProductionContext context = new ProductionContext(_configuration))
                {
                    bp = context
                            .BackgroundProcesses
                            .FirstOrDefault(fod => fod.Code == "BALCALPRO");

                    if (bp == null)
                    {
                        context.BackgroundProcesses.Add(new BackgroundProcess
                        {
                            Code = "BALCALPRO",
                            State = "INPROCESS",
                            DateCreation = DateTime.Now,
                        });
                        context.SaveChanges();
                        canContinue = true;
                    }
                    else
                    {
                        if (bp.State == "AVAILABLE")
                        {
                            bp.State = "INPROCESS";
                            bp.DateModification = DateTime.Now;
                            context.SaveChanges();
                            canContinue = true;
                        }
                        else if (bp.State == "INPROCESS")
                            canContinue = false;
                        else
                            canContinue = false;
                    }

                }
            }
            catch(Exception ex)
            {
                canContinue = false;
            }
            return canContinue;
        }
    }
}
