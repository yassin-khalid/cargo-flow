using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CargoFlow.Helpers;

public class ExceptionsHelpers
{
    public static bool IsNationalIdUniqueConstraintViolation(DbUpdateException ex)
    {
        if (ex.InnerException is not SqlException sqlEx)
        {
            return false;
        }

        if (sqlEx.Number is not (2601 or 2627))
        {
            return false;
        }

        return sqlEx.Message
        .Contains("IX_Customers_NationalId", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsForeignKeyViolation(DbUpdateException ex, string constraintName)
    {
        if (ex.InnerException is not SqlException sqlEx)
        {
            return false;
        }

        // 547 is the SQL Server error number for foreign key conflicts
        if (sqlEx.Number is not 547)
        {
            return false;
        }

        return sqlEx.Message.Contains(constraintName, StringComparison.OrdinalIgnoreCase);
    }
}
