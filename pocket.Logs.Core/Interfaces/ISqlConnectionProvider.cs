using System.Data.SqlClient;

namespace pocket.Logs.Core.Interfaces
{
    public interface ISqlConnectionProvider
    {
        string? GetConnectionString(string name = "Default");
        SqlConnection GetConnection(string name = "Default");
    }
}