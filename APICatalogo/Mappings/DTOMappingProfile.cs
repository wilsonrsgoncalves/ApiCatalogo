using APICatalogo.DTOs;
using APICatalogo.Models;
using AutoMapper;

namespace APICatalogo.Mappings
{
    public class DTOMappingProfile : Profile
    {
        public DTOMappingProfile()
        {
            CreateMap<Produto,ProdutoDTO>().ReverseMap();
            CreateMap<Categoria,CategoriaDTO>().ReverseMap();   
            CreateMap<Produto,ProdutoDTOUpdateRequest>().ReverseMap();
            CreateMap<Produto,ProdutoDTOUpdateResponse>().ReverseMap();

        }
      
    }
}
