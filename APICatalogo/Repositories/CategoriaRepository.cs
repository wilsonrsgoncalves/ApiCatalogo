using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Repositories.Interfaces;

namespace APICatalogo.Repositories;

public class CategoriaRepository(AppDbContext context) : Repository<Categoria>(context), ICategoriaRepository
{
}
