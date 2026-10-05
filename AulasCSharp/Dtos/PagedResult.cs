namespace AulasCSharp.Dtos;

// Envelope genérico imutável contendo a lista de dados e os metadados da paginação
public record PagedResult<T>(
    IEnumerable<T> Itens,
    long TotalRegistros,
    int PaginaAtual,
    int TamanhoPagina
)
{
    // Propriedade calculada automaticamente com base no total de registros e tamanho da página
    public int TotalPaginas => (int)Math.Ceiling((double)TotalRegistros / TamanhoPagina);
}