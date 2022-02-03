namespace WebApiAutores.Middelwares
{
    public static class LoggearRespuestaHttpMiddelwareExtensions
    {
        public static IApplicationBuilder UseLoggearRespuestaHTTP(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LoggearRespuestaHttpMiddelware>();
        }
    }
    public class LoggearRespuestaHttpMiddelware
    {
        private readonly RequestDelegate siguiente;
        private readonly ILogger<LoggearRespuestaHttpMiddelware> logger;

        public LoggearRespuestaHttpMiddelware(RequestDelegate siguiente,
            ILogger<LoggearRespuestaHttpMiddelware> logger)
        {
            this.siguiente = siguiente;
            this.logger = logger;
        }
        // invoke o invokeasync

        public async Task InvokeAsync (HttpContext contexto)
        {
            using (var ms = new MemoryStream())
            {
                var CuerpoOriginalRespuesta = contexto.Response.Body;
                contexto.Response.Body = ms;
                await siguiente(contexto);
                string respuesta = new StreamReader(ms).ReadToEnd();
                ms.Seek(0, SeekOrigin.Begin);
                await ms.CopyToAsync(CuerpoOriginalRespuesta);
                contexto.Response.Body = CuerpoOriginalRespuesta;
                logger.LogInformation(respuesta);
            }
        }
    }
}
