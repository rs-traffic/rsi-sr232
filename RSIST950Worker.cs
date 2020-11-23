using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace RSIST950
{
    public class RSIST950Worker :  IHostedService, IDisposable
    {
        private readonly ILogger<RSIST950Worker> _logger;
        private Timer _timer;

        public RSIST950Worker(ILogger<RSIST950Worker> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(10));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            //_logger.LogInformation("Timed Background Service is working.");

             string str = Utils.GetDataFromControler().Result;
            //     var str = "bb 3c 00 05 00 00 00 07 10 00 00 00 00 00 00 00 00 00 00 18 63 e2 03 3a 00 00 00 80 00 00 00 c5 00"
              //       + " 00 00 c5 00 00 00 00 01 00 00 00 00 00 00 00 02 00 00 00 00 00 00 00 02 09 00 00 02 00 09 d9 03";

            string xml = Utils.GetDataXML(str);
            Utils.SendData(str + "<EOF>");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
