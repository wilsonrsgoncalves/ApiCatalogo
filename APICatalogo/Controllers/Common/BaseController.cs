using APICatalogo.Pagination;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace APICatalogo.Controllers.Common;

[ApiController]
[Route("[controller]")]
public abstract class BaseController<TDto, TEntity>(ILogger logger) : ControllerBase
    where TEntity : class
{
    
    private readonly ILogger<CategoriasController> _logger = (ILogger<CategoriasController>)logger;
    protected ActionResult<IEnumerable<TDto>> Paginar<PagedEntity>(
        PagedList<PagedEntity> entidades,
        Func<IEnumerable<PagedEntity>, IEnumerable<TDto>> conversor)
        where PagedEntity : class 
    {
        if (!entidades.Any())
        {
            _logger.LogInformation("Nenhum resultado encontrado.");
            return NotFound("Nenhum item encontrado.");
        }

        var metadata = new
        {
            entidades.TotalCount,
            entidades.PageSize,
            entidades.CurrentPage,
            entidades.TotalPages,
            entidades.HasNext,
            entidades.HasPrevious
        };

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

        return Ok(conversor(entidades));
    }
}
