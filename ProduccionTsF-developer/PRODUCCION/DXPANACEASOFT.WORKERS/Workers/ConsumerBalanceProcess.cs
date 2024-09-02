using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using DXPANACEASOFT.WORKERS.Services;
using Newtonsoft.Json;
using DXPANACEASOFT.WORKERS.Models;

namespace DXPANACEASOFT.WORKERS
{
    public class ConsumerBalanceProcess : BackgroundService
    {
        private readonly ILogger _logger;
        private IConnection _connection;
        private IModel _channel;
        private readonly IConfiguration _configuration;

        public ConsumerBalanceProcess(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            this._logger = loggerFactory.CreateLogger<ConsumerBalanceProcess>();
            _configuration = configuration;
            InitRabbitMQ();
            
        }

        private void InitRabbitMQ()
        {
            var factory = new ConnectionFactory { HostName = _configuration.GetValue<string>("hostnamequeue") };

            // create connection
            _connection = factory.CreateConnection();

            // create channel
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare("balanceprocess.exchange", ExchangeType.Topic);
            _channel.QueueDeclare("balanceprocess.queue", true, false, false, null);
            _channel.QueueBind("balanceprocess.queue", "balanceprocess.exchange", "balanceprocess.queue.bind", null);
            _channel.BasicQos(0, 1, false);

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                // received message
                var content = System.Text.Encoding.UTF8.GetString(ea.Body.Span);

                // handle the received message
                var objMessage = JsonConvert.DeserializeObject<MonthlyBalanceProcessMessageDto>(content);

                //HandleMessage(content);
                bool canContinue = false;
                ValidationProcessExecution(out canContinue);

                if (!canContinue)
                {
                    _channel.BasicAck(ea.DeliveryTag, false);
                    return;
                }
                _channel.BasicAck(ea.DeliveryTag, false);

                ProcessExection(objMessage);
            };

            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            _channel.BasicConsume("balanceprocess.queue", false, consumer);
            return Task.CompletedTask;
        }

        private void ValidationProcessExecution(out bool canContinue)
        {
            canContinue = false;
            IServiceValidationProcessExecution valProces = new ServiceValidationProcessExecution(_configuration);
            canContinue = valProces.Execute();
            // we just print this message 
            //_logger.LogInformation($"consumer received {content}");
        }
        private void ProcessExection(MonthlyBalanceProcessMessageDto monthlyBalanceMessage) 
        {
            if (monthlyBalanceMessage == null)
                return;
            IServiceProcessExecution servProcess = new ServiceProcessExecution(_configuration);
            servProcess.Execute(monthlyBalanceMessage);
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.LogInformation($"connection shut down {e.ReplyText}");
        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e)
        {
            _logger.LogInformation($"consumer cancelled {e.ConsumerTags}");
        }

        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e)
        {
            _logger.LogInformation($"consumer unregistered {e.ConsumerTags}");
        }

        private void OnConsumerRegistered(object sender, ConsumerEventArgs e)
        {
            _logger.LogInformation($"consumer registered {e.ConsumerTags}");
        }

        private void OnConsumerShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.LogInformation($"consumer shutdown {e.ReplyText}");
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
