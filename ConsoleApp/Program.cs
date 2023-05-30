using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

using var db = new DomainContext();
db.Database.EnsureCreated();

var request = new Request
{
    Info = new RequestInfo { Text = "Any" }
};

db.Add(request);
db.SaveChanges();

var config = new MapperConfiguration(cfg =>
{
    cfg.CreateMap<Request, RequestDto>();
    cfg.CreateMap<RequestInfo, RequestInfoDto>();
});

var result = db.Requests
    .ProjectTo<RequestDto>(config)
    .Single(x => x.Id == request.Id);

Console.WriteLine(result.Info?.Text != null);
// Write FALSE, expected TRUE


Console.WriteLine(db.Requests.ProjectTo<RequestDto>(config).Expression);
/*		
db.Requests.ProjectTo<RequestDto>(config).Expression	
{[Microsoft.EntityFrameworkCore.Query.EntityQueryRootExpression].
Select(dtoRequest => new RequestDto() 
{
 	Id = dtoRequest.Id, 
 	Info = IIF((dtoRequest.Info == default(Object)), null, new RequestInfoDto() 
 	{
 		Text = dtoRequest.Info.Text, 
 		Number = dtoRequest.Info.Number
 	})})}	System.Linq.Expressions.Expression {System.Linq.Expressions.MethodCallExpression2}
*/

Console.WriteLine(db.Requests.ProjectTo<RequestDto>(config).ToQueryString());
/* 
SELECT [r].[Id], CASE
WHEN([r].[Info_Number] IS NULL) OR ([r].[Info_Text] IS NULL) THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END, [r].[Info_Text], [r].[Info_Number]
FROM[Requests] AS[r]

Why operator is "OR"?
*/

Console.ReadKey();


public class DomainContext : DbContext
{
    public DbSet<Request> Requests { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=DomainContext;Trusted_Connection=True");
    }
}

public class Request
{
    public Guid Id { get; set; }
    public RequestInfo Info { get; set; }
}

[Owned]
public class RequestInfo
{
    public string Text { get; set; }
    public string Number { get; set; }
}

public class RequestDto
{
    public Guid Id { get; set; }
    public RequestInfoDto Info { get; set; }
}

public class RequestInfoDto
{
    public string Text { get; set; }
    public string Number { get; set; }
}