using APICatalogo.Controllers;
using APICatalogo.DTOs;
using APICatalogo.Mappings;
using APICatalogo.Models;
using APICatalogo.Repositories.Interfaces;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Linq.Expressions;


namespace ApiCatalogoxUnitTests.UnitTest
{
    public class ProdutosControllerTests
    {
        private readonly Mock<IProdutoRepository> _produtoRepoMock;
        private readonly Mock<IUnitOfWork> _uofMock;
        private readonly IMapper _mapper;
        private readonly ProdutosController _controller;

        public ProdutosControllerTests()
        {
            _produtoRepoMock = new Mock<IProdutoRepository>();
            _uofMock = new Mock<IUnitOfWork>();

            _uofMock.Setup(u => u.ProdutoRepository).Returns(_produtoRepoMock.Object);

            var config = new MapperConfiguration(cfg => cfg.AddProfile<DTOMappingProfile>());
            _mapper = config.CreateMapper();

            _controller = new ProdutosController(_uofMock.Object, _mapper);
        }

        [Fact]
        public async Task Get_DeveRetornarOk_QuandoExistiremProdutos()
        {
            // Arrange
            var produtos = new List<Produto> { new() { ProdutoId = 1, Nome = "Produto 1", CategoriaId = 1 } };
            _produtoRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(produtos);

            // Act
            var result = await _controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var retorno = Assert.IsAssignableFrom<IEnumerable<ProdutoDTO>>(okResult.Value);
            retorno.Should().HaveCount(1);
        }

        [Fact]
        public async Task Get_DeveRetornarNotFound_QuandoNaoExistiremProdutos()
        {
            // Arrange
            _produtoRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

            // Act
            var result = await _controller.Get();

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetPorId_DeveRetornarProduto_QuandoIdForValido()
        {
            var produto = new Produto { ProdutoId = 1, Nome = "Produto Teste", CategoriaId = 1 };
            _produtoRepoMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Produto, bool>>>()))
                            .ReturnsAsync(produto);

            var result = await _controller.Get(1);

            result.Result.Should().BeOfType<OkObjectResult>();
        }
        
        [Fact]
        public async Task Post_DeveRetornarCreatedAtRoute_QuandoProdutoForValido()
        {
            var dto = new ProdutoDTO { Nome = "Produto", CategoriaId = 1, Descricao = "desc", ImagemUrl = "img.jpg" };
            var produtoCriado = new Produto { ProdutoId = 99, Nome = dto.Nome };

            _produtoRepoMock.Setup(r => r.Create(It.IsAny<Produto>())).Returns(produtoCriado);
            _uofMock.Setup(r => r.CommitAsync()).Returns(Task.CompletedTask);

            var result = await _controller.Post(dto);

            result.Result.Should().BeOfType<CreatedAtRouteResult>();
        }

        [Fact]
        public async Task Put_DeveRetornarBadRequest_QuandoIdDoCorpoDiferirDoIdDaRota()
        {
            var dto = new ProdutoDTO { ProdutoId = 2, Nome = "Produto", CategoriaId = 1 };

            var result = await _controller.Put(1, dto);

            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Delete_DeveRetornarNotFound_QuandoProdutoNaoExistir()
        {
            var produto = new Produto { ProdutoId = 1000 };

            _produtoRepoMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Produto, bool>>>()))
                            .ReturnsAsync(produto);

            var result = await _controller.Delete(1000);

            result.Should().BeOfType<NotFoundObjectResult>();
        }
    }

}
