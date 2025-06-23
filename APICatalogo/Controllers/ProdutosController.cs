using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Repositories.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers;

[Route("[controller]")]
[ApiController]

public class ProdutosController(IUnitOfWork unitOfWork, IMapper mapper) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    [HttpGet("produtos/{id}")]
    public ActionResult<IEnumerable<ProdutoDTO>> GetProdutosCategoria(int id)
    {
        var produtos = _unitOfWork.ProdutoRepository.GetProdutosPorCategoria(id);

        if (produtos is null)
            return NotFound();
        var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);
        return Ok(produtosDto);
    }

    [HttpGet]
    public ActionResult<IEnumerable<ProdutoDTO>> Get()
    {
        var produtos = _unitOfWork.ProdutoRepository.GetAll();
        if (produtos is null)
        {
            return NotFound();
        }
        var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);
        return Ok(produtosDto);
    }

    [HttpGet("{id}", Name = "ObterProduto")]
    public ActionResult<ProdutoDTO> Get(int id)
    {
        var produto = _unitOfWork.ProdutoRepository.Get(c => c.ProdutoId == id);
        if (produto is null)
        {
            return NotFound("Produto não encontrado...");
        }
        var produtoDto = _mapper.Map<ProdutoDTO>(produto);
        return Ok(produtoDto);
    }

    [HttpPost]
    public ActionResult<ProdutoDTO> Post(ProdutoDTO produtoDto)
    {
        if (produtoDto is null)
            return BadRequest();

        var produto = _mapper.Map<Produto>(produtoDto);
        var novoProduto = _unitOfWork.ProdutoRepository.Create(produto);
        _unitOfWork.Commit();

        return new CreatedAtRouteResult("ObterProduto",
            new { id = novoProduto.ProdutoId }, novoProduto);
    }

    [HttpPut("{id:int}")]
    public ActionResult<ProdutoDTO> Put(int id, ProdutoDTO produtoDto)
    {
        if (id != produtoDto.ProdutoId)
        {
            return BadRequest();
        }
        var produto = _mapper.Map<Produto>(produtoDto);
        var produtoAtualizado = _unitOfWork.ProdutoRepository.Update(produto);
        var produtoAtualizadoDto = _mapper.Map<ProdutoDTO>(produtoAtualizado);
        _unitOfWork.Commit();

        return Ok(produtoAtualizadoDto);
    }

    [HttpDelete("{id:int}")]
    public ActionResult<ProdutoDTO> Delete(int id)
    {
        var produto = _unitOfWork.ProdutoRepository.Get(p => p.ProdutoId == id);
        if (produto is null)
        {
            return NotFound("Produto não encontrado...");
        }

        var produtoDeletado = _unitOfWork.ProdutoRepository.Delete(produto);
        _unitOfWork.Commit();

        var produtoDeletadoDto = _mapper.Map<ProdutoDTO>(produtoDeletado);

        return Ok(produtoDeletadoDto);


    }
}
