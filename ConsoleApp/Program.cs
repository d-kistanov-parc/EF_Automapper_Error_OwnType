using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

using var db = new DomainContext();
db.Database.EnsureCreated();

var request = new Request
{
    Info = new RequestInfo { Text = "AnyText" }
};

db.Add(request);
db.SaveChanges();

var resultByEf = db.Requests.Single(x => x.Id == request.Id);

Console.WriteLine($"By original request: {resultByEf.Info?.Text}");
// By original request: AnyText

var config = new MapperConfiguration(cfg => cfg.CreateMap<Request, RequestDto>());

var resultByAutoMapper = db.Requests
    .AsNoTracking()
    .ProjectTo<RequestDto>(config)
    .Single(x => x.Id == request.Id);

Console.WriteLine($"By automapper request: {resultByAutoMapper.Info?.Text}");
// By automapper request: AnyText ??????

Console.ReadKey();


public class DomainContext : DbContext
{
    public DbSet<Request> Requests { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(@"Server=localhost;Port=5432;Database=DomainContext;User ID=postgres;Password=password;");
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
    public RequestInfo Info { get; set; }
}

public class RequestInfoDto
{
    public string Text { get; set; }
    public string Number { get; set; }
}