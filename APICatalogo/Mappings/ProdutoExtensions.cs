using APICatalogo.DTOs;
using APICatalogo.Models;

namespace APICatalogo.Mappings;
public static class ProdutoExtensions
{
    public static ProdutoDTO ToDTO(this Produto produto)
    {
        return new ProdutoDTO
        {
            ProdutoId = produto.ProdutoId,
            Nome = produto.Nome ?? "",
            Descricao = produto.Descricao ?? "",
            Preco = produto.Preco,
            ImagemUrl = produto.ImagemUrl ?? "",            
            CategoriaId = produto.CategoriaId,            
        };
    }

    public static Produto ToEntity(this ProdutoDTO dto)
    {
        return new Produto
        {
            ProdutoId = dto.ProdutoId,
            Nome = dto.Nome,
            Descricao = dto.Descricao,
            Preco = dto.Preco,
            ImagemUrl = dto.ImagemUrl,            
            CategoriaId = dto.CategoriaId
        };
    }
}
