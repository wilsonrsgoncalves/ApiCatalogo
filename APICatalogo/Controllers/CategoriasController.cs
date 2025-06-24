using APICatalogo.DTOs;
using APICatalogo.Mappings;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace APICatalogo.Controllers;

[Route("[controller]")]
[ApiController]
public class CategoriasController(IUnitOfWork unitOfWork, ILogger<CategoriasController> logger) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<CategoriasController> _logger = logger;

    [HttpGet]
    public ActionResult<IEnumerable<CategoriaDTO>> GetAll()
    {
        var categorias = _unitOfWork.CategoriaRepository.GetAll();

        if (!categorias.Any())
        {
            _logger.LogWarning("Nenhuma categoria encontrada.");
            return NotFound("Não existem categorias...");
        }

        return Ok(categorias.ToCategoriaDTOList());
    }

    [HttpGet("pagination")]
    public ActionResult<IEnumerable<CategoriaDTO>> GetPaginated([FromQuery] CategoriasParameters parametros)
    {
        var categorias = _unitOfWork.CategoriaRepository.GetCategorias(parametros);
        return PaginacaoComMetadata(categorias);
    }

    [HttpGet("filter/nome/pagination")]
    public ActionResult<IEnumerable<CategoriaDTO>> GetByNomeComPaginacao([FromQuery] CategoriasFiltroNome filtro)
    {
        var categoriasFiltradas = _unitOfWork.CategoriaRepository.GetCategoriasFiltroNome(filtro);
        return PaginacaoComMetadata(categoriasFiltradas);
    }

    [HttpGet("{id:int}", Name = "ObterCategoria")]
    public ActionResult<CategoriaDTO> GetById(int id)
    {
        var categoria = _unitOfWork.CategoriaRepository.Get(c => c.CategoriaId == id);

        if (categoria is null)
        {
            _logger.LogWarning("Categoria com id={CategoriaId} não encontrada.", id);
            return NotFound($"Categoria com id={id} não encontrada.");
        }

        return Ok(categoria.ToCategoriaDTO());
    }

    [HttpPost]
    public ActionResult<CategoriaDTO> Create([FromBody] CategoriaDTO categoriaDto)
    {
        if (categoriaDto is null)
        {
            _logger.LogWarning("Dados inválidos no corpo da requisição.");
            return BadRequest("Dados inválidos.");
        }

        var categoria = categoriaDto.ToCategoria();
        if (categoria is null)
        {
            _logger.LogWarning("Falha ao converter CategoriaDTO para Categoria.");
            return BadRequest("Dados inválidos.");
        }

        var createdCategoria = _unitOfWork.CategoriaRepository.Create(categoria);
        _unitOfWork.Commit();

        return CreatedAtRoute("ObterCategoria", new { id = createdCategoria.CategoriaId }, createdCategoria.ToCategoriaDTO());
    }

    [HttpPut("{id:int}")]
    public ActionResult<CategoriaDTO> Update(int id, [FromBody] CategoriaDTO categoriaDto)
    {
        if (categoriaDto is null || id != categoriaDto.CategoriaId)
        {
            _logger.LogWarning("Dados inválidos para atualização da categoria.");
            return BadRequest("Dados inválidos.");
        }

        var existingCategoria = _unitOfWork.CategoriaRepository.Get(c => c.CategoriaId == id);

        if (existingCategoria is null)
        {
            _logger.LogWarning("Categoria com id={CategoriaId} não encontrada para atualização.", id);
            return NotFound($"Categoria com id={id} não encontrada.");
        }

        var categoria = categoriaDto.ToCategoria();
        if (categoria is null)
        {
            _logger.LogWarning("Falha ao converter CategoriaDTO para Categoria.");
            return BadRequest("Dados inválidos.");
        }

        var updatedCategoria = _unitOfWork.CategoriaRepository.Update(categoria);
        _unitOfWork.Commit();

        return Ok(updatedCategoria.ToCategoriaDTO());
    }

    [HttpDelete("{id:int}")]
    public ActionResult<CategoriaDTO> Delete(int id)
    {
        var categoria = _unitOfWork.CategoriaRepository.Get(c => c.CategoriaId == id);

        if (categoria is null)
        {
            _logger.LogWarning("Categoria com id={CategoriaId} não encontrada para exclusão.", id);
            return NotFound($"Categoria com id={id} não encontrada.");
        }

        var categoriaExcluida = _unitOfWork.CategoriaRepository.Delete(categoria);
        _unitOfWork.Commit();

        return Ok(categoriaExcluida.ToCategoriaDTO());
    }

    private ActionResult<IEnumerable<CategoriaDTO>> PaginacaoComMetadata(PagedList<Categoria> categorias)
    {
        var metadata = new
        {
            categorias.TotalCount,
            categorias.PageSize,
            categorias.CurrentPage,
            categorias.TotalPages,
            categorias.HasNext,
            categorias.HasPrevious
        };

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
        return Ok(categorias.ToCategoriaDTOList());
    }
}
