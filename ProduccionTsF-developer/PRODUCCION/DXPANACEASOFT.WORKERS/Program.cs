
using DXPANACEASOFT.WORKERS;



IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        //services.AddDbContext<ProductionContext>(options =>
         //   options.UseSqlServer(hostContext.Configuration.GetaConnectionString("ProductionDatabase")));
        services.AddHostedService<ConsumerBalanceProcess>();
    })
    .UseWindowsService()
    .Build();

await host.RunAsync();
