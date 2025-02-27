using System.Data;
using MediatR;
using XMailService.Application.Interfaces;
using static XMailService.Application.MailSignatures.Queries.GetMailSignatureQuery;

namespace XMailService.Application.MailSignatures.Queries;

public sealed record GetMailSignatureQuery : IRequest<Response?>
{
    public required string Name { get; init; }

    internal sealed class Handler(IDbConnectionFactory dbConnection) : IRequestHandler<GetMailSignatureQuery, Response?>
    {
        public async Task<Response?> Handle(GetMailSignatureQuery request, CancellationToken cancellationToken)
        {
            const string sql = """
            select t."Id", t."Name", t."Description", t."Version"
            from public."MailSignatures" t
            where t."Name" = @name and t."SoftDeleted" = false
            order by t."Version" desc
            limit 1
            """;
            object param = new
            {
                name = request.Name
            };

            using IDbConnection connection = dbConnection.CreateConnection();
            Response? response = await dbConnection.QueryFirstAsync<Response>(sql, param);

            return response;
        }
    }

    public sealed record Response
    {
        public required Ulid Id { get; init; }
        public required string Name { get; init; }
        public required string Description { get; init; }
        public required int Version { get; set; }
    }
}
