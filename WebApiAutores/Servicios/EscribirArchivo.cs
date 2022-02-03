namespace WebApiAutores.Servicios
{
    public class EscribirArchivo : IHostedService
    {
        private readonly IWebHostEnvironment env;
        private readonly string nombreArchivo = "Archivo1.txt";
        private Timer Timer;
        public EscribirArchivo(IWebHostEnvironment env)
        {
            this.env = env;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Timer = new Timer(DoWorck, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            Escribir("Proseso iniciado");
            return Task.CompletedTask;
              
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Timer.Dispose();
            Escribir("Proseso Finalizado");
            return Task.CompletedTask;
        }
        private void DoWorck(object state)
        {
            Escribir("Proceso en ejecucion"+ DateTime.Now.ToString("dd/mm/yyyy hh:mm:ss"));
        }
        public void Escribir(string mensaje)
        {
            var ruta = $@"{ env.ContentRootPath}\wwwroot\{nombreArchivo}";
            using (StreamWriter writer = new StreamWriter(ruta, append: true))
            {
                writer.WriteLine(mensaje);
            }
        }
    }
}
