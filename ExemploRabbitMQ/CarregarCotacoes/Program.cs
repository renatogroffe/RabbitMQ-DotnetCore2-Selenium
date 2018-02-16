using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CarregarCotacoes
{
    class Program
    {
        private static IConfiguration _configuration;
        private static SeleniumConfigurations _seleniumConfigurations;

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json");
            _configuration = builder.Build();

            // Para este exemplo foi criado um container Docker baseado
            // em uma imagem do RabbitMQ. Segue o comando para geração
            // desta estrutura:
            // docker run -d --hostname rabbit-local --name testes-rabbitmq -p 6672:5672 -p 15672:15672 -e RABBITMQ_DEFAULT_USER=testes -e RABBITMQ_DEFAULT_PASS=Testes2018! rabbitmq:3-management-alpine
            var rabbitMQConfigurations = new RabbitMQConfigurations();
            new ConfigureFromConfigurationOptions<RabbitMQConfigurations>(
                _configuration.GetSection("RabbitMQConfigurations"))
                    .Configure(rabbitMQConfigurations);

            _seleniumConfigurations = new SeleniumConfigurations();
            new ConfigureFromConfigurationOptions<SeleniumConfigurations>(
                _configuration.GetSection("SeleniumConfigurations"))
                    .Configure(_seleniumConfigurations);

            var factory = new ConnectionFactory()
            {
                HostName = rabbitMQConfigurations.HostName,
                Port = rabbitMQConfigurations.Port,
                UserName = rabbitMQConfigurations.UserName,
                Password = rabbitMQConfigurations.Password
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "CarregarCotacoes",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += Consumer_Received;
                channel.BasicConsume(queue: "CarregarCotacoes",
                     autoAck: true,
                     consumer: consumer);

                Console.WriteLine("Aguardando mensagens para processamento");
                Console.WriteLine("Pressione uma tecla para encerrar...");
                Console.ReadKey();
            }
        }

        private static void Consumer_Received(
            object sender, BasicDeliverEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Body);
            Console.WriteLine(Environment.NewLine +
                "[Nova mensagem recebida] " + message);

            List<Cotacao> cotacoes;
            PaginaCotacoes pagina =
                new PaginaCotacoes(_seleniumConfigurations);
            try
            {
                Console.WriteLine("Iniciando extração dos dados...");
                pagina.CarregarPagina();
                cotacoes = pagina.ObterCotacoes();
                Console.WriteLine("Dados extraídos com sucesso!");

                new CotacoesDAO(_configuration.GetConnectionString("TestesRabbitMQ"))
                    .CarregarDados(cotacoes);
                Console.WriteLine("Carga dos dados efetuada com sucesso!");
            }
            finally
            {
                pagina.Fechar();
            }
        }
    }
}