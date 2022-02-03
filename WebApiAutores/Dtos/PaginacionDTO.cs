namespace WebApiAutores.Dtos
{
    public class PaginacionDTO
    {
        public int Pagina { get; set; } = 1;
        private int recorsPorPagina = 10;
        private readonly int candidadMaximaPorPagina = 50;
        public int RecorsPorPagina
        {
            get { 
                return recorsPorPagina;
                }
            set { 
                recorsPorPagina = (value > candidadMaximaPorPagina) ? candidadMaximaPorPagina : value; 
                }
        }
    }
}
