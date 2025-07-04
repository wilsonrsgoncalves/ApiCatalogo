using APICatalogo.Controllers;
using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Repositories.Interfaces;
using AutoMapper;
using Moq;


namespace ApiCatalogoxUnitTests.UnitTest
{
    public class ProdutosUnitTestController
    {

        public readonly Mock<IUnitOfWork> _uofMock;
        public readonly Mock<IProdutoRepository> _produtoRepoMock;
        private readonly IMapper _mapper;
        private readonly ProdutosController _controller;
        public ProdutosUnitTestController()
        {     
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Produto, ProdutoDTO>().ReverseMap();
            });
            _mapper = config.CreateMapper();            
            _produtoRepoMock = new Mock<IProdutoRepository>();
            _uofMock = new Mock<IUnitOfWork>();
            _uofMock.Setup(u => u.ProdutoRepository).Returns(_produtoRepoMock.Object);            
            _controller = new ProdutosController(_uofMock.Object, _mapper);
        }
    }
}
