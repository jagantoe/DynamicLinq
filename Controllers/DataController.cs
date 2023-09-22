using Microsoft.AspNetCore.Mvc;

namespace DynamicLinq.Controllers;

[ApiController]
[Route("[controller]")]
public class DataController : ControllerBase
{
    private readonly UserContext _context;

    public DataController(DataWrapper wrapper)
    {
        _context = wrapper.Context;
    }

    [HttpGet(Name = "Users")]
    public IEnumerable<User> Get(string query)
    {
        var conditions = QueryParser.Parse(query);
        IQueryable<User> users = _context.Users;
        foreach (var condition in conditions)
        {
            users = users.DynamicWhere(condition);
        }

        return users.ToList();
    }
}
