namespace WebApiAutores.Services
{
    public class WriteInFile : IHostedService
    {
        private readonly IWebHostEnvironment env;
        private readonly string fileName = "File-Net.txt";
        private Timer timer;

        public WriteInFile(IWebHostEnvironment env)
        {
            this.env = env;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Write("Process initialized");
            timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            return Task.CompletedTask;


        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Write("Process finalized");
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            Write("Excecution process " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
        }

        private void Write(string message)
        {
            var path = $@"{env.ContentRootPath}\wwwroot\{fileName}";

            using (StreamWriter writer = new StreamWriter(path, append: true)) 
            {
                writer.WriteLine(message);
            }
        }

    }
}
