using APICatalogo.DTOs;
using APICatalogo.Models;

namespace APICatalogo.Mappings
{
    public static class CategoriaExtensions
    {
        public static CategoriaDTO? ToCategoriaDTO(this Categoria categoria)
        {
            if (categoria is null)
                return null;

            return new CategoriaDTO
            {
                CategoriaId = categoria.CategoriaId,
                Nome = categoria.Nome,
                ImagemUrl = categoria.ImagemUrl
            };
        }

        public static Categoria? ToCategoria(this CategoriaDTO categoriaDto)
        {
            if (categoriaDto is null) return null;

            return new Categoria
            {
                CategoriaId = categoriaDto.CategoriaId,
                Nome = categoriaDto.Nome,
                ImagemUrl = categoriaDto.ImagemUrl
            };
        }

        public static IEnumerable<CategoriaDTO> ToCategoriaDTOList(this IEnumerable<Categoria> categorias)
        {
            return categorias is null || !categorias.Any()
                ? []
                : (IEnumerable<CategoriaDTO>)[.. categorias.Select(categoria => new CategoriaDTO
            {
                CategoriaId = categoria.CategoriaId,
                Nome = categoria.Nome,
                ImagemUrl = categoria.ImagemUrl
            })];
        }
    }
}
