using AutoMapper;
using WebApiAutores.Dtos;
using WebApiAutores.Entidades;

namespace WebApiAutores.Utilidades
{
    public class AutomapperProfiles : Profile
    {
        public AutomapperProfiles()
        {
            CreateMap<AutorCreacionDTO, Autor>();
            CreateMap<Autor, AutorDTO>();

            CreateMap<Autor, AutorDTOConLibros>()
                .ForMember(autorDto => autorDto.Libros, opciones => opciones.MapFrom(MapAutorDTOLibros));

            
               

            CreateMap<LibroCreacionDTO, Libro>()
                .ForMember(libro => libro.AutoresLibro, opciones => opciones.MapFrom(MapAutoresLibro));
            CreateMap<Libro, LibroDTOConAutores>()
                .ForMember(libroDTO => libroDTO.Autores, opciones => opciones.MapFrom(MapLibroAutores));
            CreateMap<Libro, LibroDTO>();

            CreateMap<LibroPatchDTO,Libro>().ReverseMap();

            CreateMap<ComentarioCreacionDTO,Comentario>();
            CreateMap<Comentario, ComentarioDTO>();


        }

        private List<LibroDTO> MapAutorDTOLibros(Autor autor, AutorDTO autorDTO)
        {
            var resultado = new List<LibroDTO>();
            if (autor.AutoresLibros == null)
                return resultado;

            foreach (var autoresLibro in autor.AutoresLibros)
            {
                resultado.Add(new LibroDTO()
                {
                    id = autoresLibro.AutorId,
                    Titulo = autoresLibro.libro.Titulo
                });

            }
            return resultado;
        }

        private List<AutorDTO>  MapLibroAutores(Libro libro,LibroDTO libroDTO)
        {
            var resultado = new List<AutorDTO>();
            if (libro.AutoresLibro == null)
                return resultado;
            
            foreach(var autoresLibro in libro.AutoresLibro)
            {
                resultado.Add(new AutorDTO()
                {
                    id = autoresLibro.AutorId,
                    Nombre = autoresLibro.autor.nombre
                });

            }
            return resultado;
        }

        private List<AutorLibro> MapAutoresLibro(LibroCreacionDTO libroCreacionDTO,Libro libro)
        {
            var resultado= new List<AutorLibro>();
            if(libroCreacionDTO.Autores==null)
                return resultado;

            foreach(var autorid in libroCreacionDTO.Autores)
            {
                resultado.Add(new AutorLibro() { AutorId = autorid });
            }
            return resultado;

        }
    }
}
