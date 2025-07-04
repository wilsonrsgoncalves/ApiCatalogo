using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace APICatalogo.Controllers;

[Route("[controller]")]
[ApiController]
[ApiConventionType(typeof(DefaultApiConventions))]
public class ProdutosController(IUnitOfWork uof, IMapper mapper) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = uof;
    private readonly IMapper _mapper = mapper;

    // ========================================
    // GET: /produtos
    // ========================================
    [Authorize(Policy = "UserOnly")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    ///Este metodo lfdkdsfpkdsfksdpdfssdfpo
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get()
    {
        var produtos = await _unitOfWork.ProdutoRepository.GetAllAsync();

        if (produtos == null || !produtos.Any())
            return NotFound("Nenhum produto encontrado.");

        var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);
        return Ok(produtosDto);
    }

    // ========================================
    // GET: /produtos/{id}
    // ========================================
    [Authorize(Policy = "UserOnly")]
    [HttpGet("{id:int}", Name = "ObterProduto")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProdutoDTO>> Get(int id)
    {
        if (id <= 0)
            return BadRequest("ID inválido.");

        var produto = await _unitOfWork.ProdutoRepository.GetAsync(p => p.ProdutoId == id);

        if (produto == null)
            return NotFound("Produto não encontrado.");

        var produtoDto = _mapper.Map<ProdutoDTO>(produto);
        return Ok(produtoDto);
    }

    // ========================================
    // GET: /produtos/categoria/{id}
    // ========================================
    [HttpGet("categoria/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetPorCategoria(int id)
    {
        var produtos = await _unitOfWork.ProdutoRepository.GetProdutosPorCategoriaAsync(id);

        if (produtos == null || !produtos.Any())
            return NotFound("Nenhum produto encontrado para esta categoria.");

        var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);
        return Ok(produtosDto);
    }

    // ========================================
    // GET: /produtos/pagination
    // ========================================
    [HttpGet("pagination")]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetComPaginacao([FromQuery] ProdutosParameters parametros)
    {
        var produtos = await _unitOfWork.ProdutoRepository.GetProdutosAsync(parametros);
        return ObterProdutosComPaginacao(produtos);
    }

    // ========================================
    // GET: /produtos/filter/preco/pagination
    // ========================================
    [HttpGet("filter/preco/pagination")]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetFiltroPorPreco([FromQuery] ProdutosFiltroPreco parametros)
    {
        var produtos = await _unitOfWork.ProdutoRepository.GetProdutosFiltroPrecoAsync(parametros);
        return ObterProdutosComPaginacao(produtos);
    }

    private ActionResult<IEnumerable<ProdutoDTO>> ObterProdutosComPaginacao(PagedList<Produto> produtos)
    {
        var metadata = new
        {
            produtos.TotalCount,
            produtos.PageSize,
            produtos.CurrentPage,
            produtos.TotalPages,
            produtos.HasNext,
            produtos.HasPrevious
        };

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

        var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);
        return Ok(produtosDto);
    }

    // ========================================
    // POST: /produtos
    // ========================================
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProdutoDTO>> Post([FromBody] ProdutoDTO produtoDto)
    {
        if (produtoDto == null)
            return BadRequest("Dados inválidos.");

        var produto = _mapper.Map<Produto>(produtoDto);
        var novoProduto = _unitOfWork.ProdutoRepository.Create(produto);
        await _unitOfWork.CommitAsync();

        var novoProdutoDto = _mapper.Map<ProdutoDTO>(novoProduto);

        return CreatedAtRoute("ObterProduto", new { id = novoProdutoDto.ProdutoId }, novoProdutoDto);
    }

    // ========================================
    // PUT: /produtos/{id}
    // ========================================
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProdutoDTO>> Put(int id, [FromBody] ProdutoDTO produtoDto)
    {
        if (id != produtoDto.ProdutoId)
            return BadRequest("ID do parâmetro difere do corpo da requisição.");

        var produto = _mapper.Map<Produto>(produtoDto);
        var atualizado = _unitOfWork.ProdutoRepository.Update(produto);
        await _unitOfWork.CommitAsync();

        var atualizadoDto = _mapper.Map<ProdutoDTO>(atualizado);
        return Ok(atualizadoDto);
    }

    // ========================================
    // PATCH: /produtos/{id}/updatepartial
    // ========================================
    [HttpPatch("{id:int}/updatepartial")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProdutoDTOUpdateResponse>> Patch(int id,
        [FromBody] JsonPatchDocument<ProdutoDTOUpdateRequest> patchDto)
    {
        if (patchDto == null || id <= 0)
            return BadRequest("Requisição inválida.");

        var produto = await _unitOfWork.ProdutoRepository.GetAsync(p => p.ProdutoId == id);
        if (produto == null)
            return NotFound("Produto não encontrado.");

        var produtoUpdateRequest = _mapper.Map<ProdutoDTOUpdateRequest>(produto);

        patchDto.ApplyTo(produtoUpdateRequest, ModelState);

        if (!ModelState.IsValid || !TryValidateModel(produtoUpdateRequest))
            return BadRequest(ModelState);

        _mapper.Map(produtoUpdateRequest, produto);
        _unitOfWork.ProdutoRepository.Update(produto);
        await _unitOfWork.CommitAsync();

        var responseDto = _mapper.Map<ProdutoDTOUpdateResponse>(produto);
        return Ok(responseDto);
    }

    // ========================================
    // DELETE: /produtos/{id}
    // ========================================
    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProdutoDTO>> Delete(int id)
    {
        var produto = await _unitOfWork.ProdutoRepository.GetAsync(p => p.ProdutoId == id);

        if (produto == null)
            return NotFound("Produto não encontrado.");

        var deletado = _unitOfWork.ProdutoRepository.Delete(produto);
        await _unitOfWork.CommitAsync();

        var deletadoDto = _mapper.Map<ProdutoDTO>(deletado);
        return Ok(deletadoDto);
    }
}
