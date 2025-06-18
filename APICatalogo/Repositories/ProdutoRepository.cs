using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Repositories.Interfaces;

namespace APICatalogo.Repositories;

public class ProdutoRepository(AppDbContext context) : Repository<Produto>(context), IProdutoRepository
{
    public IEnumerable<Produto> GetProdutosPorCategoria(int id)
    {
        return GetAll().Where(c => c.CategoriaId == id);
    }
}