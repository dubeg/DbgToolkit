using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace DbgToolkit.Collection;

public static class DataTableExtensions {
    public static DataTable ToDataTable<T>(this IEnumerable<T> data) {
        var dataTable = new DataTable();
        if (data == null || !data.Any())
            return dataTable;

        var properties = typeof(T).GetProperties()
            .Where(p => IsSimpleType(p.PropertyType))
            .ToList();
        
        foreach (var prop in properties) {
            dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        }

        foreach (var item in data) {
            var row = dataTable.NewRow();
            foreach (var prop in properties) {
                row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
            }
            dataTable.Rows.Add(row);
        }

        return dataTable;
    }

    private static bool IsSimpleType(Type type) {
        var simpleTypes = new[] {
            typeof(string),
            typeof(bool),
            typeof(DateTime),
            typeof(int),
            typeof(decimal),
            typeof(double),
            typeof(float),
            typeof(Enum)
        };
        type = Nullable.GetUnderlyingType(type) ?? type;
        return simpleTypes.Contains(type) || type.IsEnum;
    }

    public static async Task ExecuteBulkInsertAsync<T>(
        this SqlConnection connection,
        IEnumerable<T> data,
        string storedProcedureName,
        string sqlTypeName,
        string tableValuedParameterName = "Data",
        SqlTransaction? transaction = null) {
        if (connection == null)
            throw new ArgumentNullException(nameof(connection));

        if (string.IsNullOrEmpty(storedProcedureName))
            throw new ArgumentException("Stored procedure name cannot be null or empty", nameof(storedProcedureName));

        if (string.IsNullOrEmpty(sqlTypeName))
            throw new ArgumentException("SQL type name cannot be null or empty", nameof(sqlTypeName));

        var dataTable = data.ToDataTable();
        if (dataTable.Rows.Count == 0)
            return;

        using var command = new SqlCommand(storedProcedureName, connection) {
            CommandType = CommandType.StoredProcedure,
            Transaction = transaction
        };

        var parameter = new SqlParameter {
            ParameterName = tableValuedParameterName,
            SqlDbType = SqlDbType.Structured,
            TypeName = sqlTypeName,
            Value = dataTable
        };

        command.Parameters.Add(parameter);

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        await command.ExecuteNonQueryAsync();
    }

    public static void ExecuteBulkInsert<T>(
        this SqlConnection connection,
        IEnumerable<T> data,
        string storedProcedureName,
        string sqlTypeName,
        string tableValuedParameterName = "Data",
        SqlTransaction? transaction = null) {
        if (connection == null)
            throw new ArgumentNullException(nameof(connection));

        if (string.IsNullOrEmpty(storedProcedureName))
            throw new ArgumentException("Stored procedure name cannot be null or empty", nameof(storedProcedureName));

        if (string.IsNullOrEmpty(sqlTypeName))
            throw new ArgumentException("SQL type name cannot be null or empty", nameof(sqlTypeName));

        var dataTable = data.ToDataTable();
        if (dataTable.Rows.Count == 0)
            return;

        using var command = new SqlCommand(storedProcedureName, connection) {
            CommandType = CommandType.StoredProcedure,
            Transaction = transaction
        };

        var parameter = new SqlParameter {
            ParameterName = tableValuedParameterName,
            SqlDbType = SqlDbType.Structured,
            TypeName = sqlTypeName,
            Value = dataTable
        };

        command.Parameters.Add(parameter);

        if (connection.State != ConnectionState.Open)
            connection.Open();

        command.ExecuteNonQuery();
    }
}
