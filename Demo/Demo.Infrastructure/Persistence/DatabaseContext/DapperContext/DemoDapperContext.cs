using Microsoft.Data.SqlClient;

namespace Demo.Infrastructure.Persistence.DatabaseContext.DapperContext;

public class DemoDapperContext
{
    private readonly string _connectionString;

    public DemoDapperContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public SqlConnection CreateDemoConnection()
    {
        return new SqlConnection(_connectionString);
    }
}