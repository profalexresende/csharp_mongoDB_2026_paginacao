using AulasCSharp.Dominio;
using AulasCSharp.Dtos;
using AulasCSharp.Infraestrutura;

var builder = WebApplication.CreateBuilder(args);

// Configuração do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Injeção de Dependência do Repositório NoSQL
builder.Services.AddScoped<IProdutoRepositorio, MongoProdutoRepositorio>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ----------------------------------------------------------------------------------
// ENDPOINTS DA MINIMAL API
// ----------------------------------------------------------------------------------

// Rota GET /api/produtos: Lê os parâmetros da Query String via [AsParameters]
app.MapGet("/api/produtos", async ([AsParameters] ProdutoFiltroRequest filtro, IProdutoRepositorio repo) =>
{
    var resultado = await repo.ObterPaginadoAsync(filtro);
    return Results.Ok(resultado);
});

// Rota GET /api/produtos/{id}
app.MapGet("/api/produtos/{id:guid}", async (Guid id, IProdutoRepositorio repo) =>
{
    var produto = await repo.ObterPorIdAsync(id);
    return produto is not null ? Results.Ok(produto) : Results.NotFound(new { mensagem = "Produto não encontrado." });
});

// Rota POST /api/produtos
app.MapPost("/api/produtos", async (CriarProdutoRequest request, IProdutoRepositorio repo) =>
{
    try
    {
        var produto = new Produto(request.Nome, request.Preco, request.Estoque);
        await repo.AdicionarAsync(produto);
        return Results.Created($"/api/produtos/{produto.Id}", produto);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { erro = ex.Message });
    }
});

app.Run();

// DTO para recepção da requisição POST
record CriarProdutoRequest(string Nome, decimal Preco, int Estoque);