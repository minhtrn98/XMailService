using System.Data;
using MediatR;
using XMailService.Application.Interfaces;
using static XMailService.Application.MailTemplates.Queries.GetMailTemplateQuery;

namespace XMailService.Application.MailTemplates.Queries;

public sealed record GetMailTemplateQuery : IRequest<Response?>
{
    public required string Name { get; init; }

    internal sealed class Handler(IDbConnectionFactory dbConnection) : IRequestHandler<GetMailTemplateQuery, Response?>
    {
        public async Task<Response?> Handle(GetMailTemplateQuery request, CancellationToken cancellationToken)
        {
            const string sql = """
            select t."Id", t."Name", t."Subject", t."Description", t."Version"
            from public."MailTemplates" t
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
        public required string Subject { get; init; }
        public required string Description { get; init; }
        public required int Version { get; set; }
    }
}
