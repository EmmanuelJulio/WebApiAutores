namespace WebApiAutores.Dtos
{
    public class DatoHATEOAS
    {
        public string Enlace { get; private set; }
        public string Descripcion { get; private set; }
        public string Metodo { get; private set; }

        public DatoHATEOAS(string enlasce, string descripcion ,string metodo)
        {
            Enlace = enlasce;
            Descripcion = descripcion; 
            Metodo = metodo;    
        }
    }
}
