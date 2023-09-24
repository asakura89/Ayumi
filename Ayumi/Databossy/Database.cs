using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Text.RegularExpressions;
using Exy;
using Microsoft.Extensions.Configuration;
using ConfigurationManager = AppSea.ConfigurationManager;

namespace Databossy;

public interface IDatabaseFactory {
    IDatabase CreateSession(DbProviderFactory provider);

    IDatabase CreateSession(DbProviderFactory provider, String contextName);
}

public class DatabaseFactory : IDatabaseFactory {
    public IDatabase CreateSession(DbProviderFactory provider) => CreateSession(provider, ConfigurationManager.ConnectionStrings[0].Key);

    public IDatabase CreateSession(DbProviderFactory provider, String contextName) => new Database(provider, contextName);
}

public interface IDatabase : IDisposable {
    String ContextName { get; }
    String ConnectionString { get; }
    DbProviderFactory Provider { get; }

    DataTable QueryDataTable(String queryString);

    DataTable QueryDataTable(String queryString, params Object[] queryParams);

    DataTable NQueryDataTable(String queryString, Object paramObj);

    DataSet QueryDataSet(String queryString);

    DataSet QueryDataSet(String queryString, params Object[] queryParams);

    DataSet NQueryDataSet(String queryString, Object paramObj);

    IEnumerable<T> Query<T>(String queryString);

    IEnumerable<T> Query<T>(String queryString, params Object[] queryParams);

    IEnumerable<T> NQuery<T>(String queryString, Object paramObj);

    T QuerySingle<T>(String queryString);

    T QuerySingle<T>(String queryString, params Object[] queryParams);

    T NQuerySingle<T>(String queryString, Object paramObj);

    T QueryScalar<T>(String queryString);

    T QueryScalar<T>(String queryString, params Object[] queryParams);

    T NQueryScalar<T>(String queryString, Object paramObj);

    Int32 Execute(String queryString);

    Int32 Execute(String queryString, params Object[] queryParams);

    Int32 NExecute(String queryString, Object paramObj);
}

public class Database : IDatabase {
    DbConnection connection;

    public String ContextName { get; set; }
    public String ConnectionString { get; set; }
    public DbProviderFactory Provider { get; set; }

    public Database(DbProviderFactory provider, String contextName) {
        Provider = provider;
        ContextName = contextName;

        IConfigurationSection config = ConfigurationManager.GetConnectionString(contextName);
        ConnectionString = config?.Value;
    }

    void Open() {
        connection = Provider.CreateConnection();
        if (connection == null)
            throw new UnintendedBehaviorException("Connection creation from factory failed.");

        connection.ConnectionString = ConnectionString;
        connection.Open();
    }

    DbCommand BuildSqlCommand(String queryString) {
        DbCommand builtSqlCommand = connection.CreateCommand();
        builtSqlCommand.CommandText = queryString;
        builtSqlCommand.CommandType = CommandType.Text;

        return builtSqlCommand;
    }

    DbCommand BuildSqlCommand(String queryString, Object[] queryParams) {
        DbCommand builtSqlCommand = connection.CreateCommand();
        builtSqlCommand.CommandText = queryString;
        builtSqlCommand.CommandType = CommandType.Text;
        BuildSqlCommandParameter(ref builtSqlCommand, queryParams);

        return builtSqlCommand;
    }

    DbCommand NBuildSqlCommand<TParam>(String queryString, TParam paramObj) {
        DbCommand builtSqlCommand = connection.CreateCommand();
        builtSqlCommand.CommandText = queryString;
        builtSqlCommand.CommandType = CommandType.Text;
        NBuildSqlCommandParameter(ref builtSqlCommand, paramObj);

        return builtSqlCommand;
    }

    void BuildSqlCommandParameter(ref DbCommand builtSqlCommand, Object[] queryParams) {
        builtSqlCommand.Parameters.Clear();
        for (Int32 paramIdx = 0; paramIdx < queryParams.Length; paramIdx++) {
            Object currentqueryParams = queryParams[paramIdx] ?? DBNull.Value;
            DbParameter param = builtSqlCommand.CreateParameter();
            param.ParameterName = paramIdx.ToString();
            param.Value = currentqueryParams;
            builtSqlCommand.Parameters.Add(param);
        }
    }

    void NBuildSqlCommandParameter<TParam>(ref DbCommand builtSqlCommand, TParam paramObj) {
        PropertyInfo[] properties = ValidateAndGetNParam(builtSqlCommand.CommandText, paramObj);

        builtSqlCommand.Parameters.Clear();
        foreach (PropertyInfo currentProperty in properties) {
            Object currentParam = currentProperty.GetValue(paramObj, null) ?? DBNull.Value;
            DbParameter param = builtSqlCommand.CreateParameter();
            param.ParameterName = currentProperty.Name;
            param.Value = currentParam;
            builtSqlCommand.Parameters.Add(param);
        }
    }

    PropertyInfo[] ValidateAndGetNParam(String queryString, Object paramObj) {
        var queryParamRgx = new Regex("(?<!@)@\\w+", RegexOptions.Compiled);
        MatchCollection definedParams = queryParamRgx.Matches(queryString);
        PropertyInfo[] properties = paramObj.GetType().GetProperties();
        foreach (Match param in definedParams) {
            String closureParam = param.Value.Replace("@", "");
            PropertyInfo foundProperty = properties.FirstOrDefault(p => p.Name == closureParam);
            if (foundProperty == null)
                throw new UnintendedBehaviorException($"Sql Param \"{closureParam}\" is defined but value isn't supplied.");
        }

        return properties;
    }

    DbDataAdapter BuildSelectDataAdapter(DbCommand builtSqlCommand) {
        DbDataAdapter builtDataAdapter = Provider.CreateDataAdapter();
        if (builtDataAdapter == null)
            throw new UnintendedBehaviorException("Data Adapter creation from factory failed.");

        builtDataAdapter.SelectCommand = builtSqlCommand;
        return builtDataAdapter;
    }

    public DataTable QueryDataTable(String queryString) {
        Open();
        var dt = new DataTable();
        using (DbCommand cmd = BuildSqlCommand(queryString)) {
            using (DbDataAdapter dataAdapter = BuildSelectDataAdapter(cmd))
                dataAdapter.Fill(dt);
        }

        return dt;
    }

    public DataTable QueryDataTable(String queryString, params Object[] queryParams) {
        Open();
        var dt = new DataTable();
        using (DbCommand cmd = BuildSqlCommand(queryString, queryParams)) {
            using (DbDataAdapter dataAdapter = BuildSelectDataAdapter(cmd))
                dataAdapter.Fill(dt);
        }

        return dt;
    }

    public DataTable NQueryDataTable(String queryString, Object paramObj) {
        Open();
        var dt = new DataTable();
        using (DbCommand cmd = NBuildSqlCommand(queryString, paramObj)) {
            using (DbDataAdapter dataAdapter = BuildSelectDataAdapter(cmd))
                dataAdapter.Fill(dt);
        }

        return dt;
    }

    public DataSet QueryDataSet(String queryString) {
        Open();
        var ds = new DataSet();
        using (DbCommand cmd = BuildSqlCommand(queryString)) {
            using (DbDataAdapter dataAdapter = BuildSelectDataAdapter(cmd))
                dataAdapter.Fill(ds);
        }

        return ds;
    }

    public DataSet QueryDataSet(String queryString, params Object[] queryParams) {
        Open();
        var ds = new DataSet();
        using (DbCommand cmd = BuildSqlCommand(queryString, queryParams)) {
            using (DbDataAdapter dataAdapter = BuildSelectDataAdapter(cmd))
                dataAdapter.Fill(ds);
        }

        return ds;
    }

    public DataSet NQueryDataSet(String queryString, Object paramObj) {
        Open();
        var ds = new DataSet();
        using (DbCommand cmd = NBuildSqlCommand(queryString, paramObj)) {
            using (DbDataAdapter dataAdapter = BuildSelectDataAdapter(cmd))
                dataAdapter.Fill(ds);
        }

        return ds;
    }

    // NOTE: must be materialized to List
    // --> https://softwareengineering.stackexchange.com/questions/300242/will-the-database-connection-be-closed-if-we-yield-the-datareader-row-and-not-re
    public IEnumerable<T> Query<T>(String queryString) {
        Open();
        var result = new List<T>();
        using (DbCommand cmd = BuildSqlCommand(queryString))
        using (DbDataReader reader = cmd.ExecuteReader())
            while (reader.Read())
                result.Add(ToTResult<T>(reader));

        return result;
    }

    public IEnumerable<T> Query<T>(String queryString, params Object[] queryParams) {
        Open();
        var result = new List<T>();
        using (DbCommand cmd = BuildSqlCommand(queryString, queryParams))
        using (DbDataReader reader = cmd.ExecuteReader())
            while (reader.Read())
                result.Add(ToTResult<T>(reader));

        return result;
    }

    public IEnumerable<T> NQuery<T>(String queryString, Object paramObj) {
        Open();
        var result = new List<T>();
        using (DbCommand cmd = NBuildSqlCommand(queryString, paramObj))
        using (DbDataReader reader = cmd.ExecuteReader())
            while (reader.Read())
                result.Add(ToTResult<T>(reader));

        return result;
    }

    public T QuerySingle<T>(String queryString) => Query<T>(queryString).FirstOrDefault();

    public T QuerySingle<T>(String queryString, params Object[] queryParams) => Query<T>(queryString, queryParams).FirstOrDefault();

    public T NQuerySingle<T>(String queryString, Object paramObj) => NQuery<T>(queryString, paramObj).FirstOrDefault();

    public T QueryScalar<T>(String queryString) {
        Open();
        using (DbCommand cmd = BuildSqlCommand(queryString))
            return (T) Convert.ChangeType(cmd.ExecuteScalar(), Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T));
    }

    public T QueryScalar<T>(String queryString, params Object[] queryParams) {
        Open();
        using (DbCommand cmd = BuildSqlCommand(queryString, queryParams))
            return (T) Convert.ChangeType(cmd.ExecuteScalar(), Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T));
    }

    public T NQueryScalar<T>(String queryString, Object paramObj) {
        Open();
        using (DbCommand cmd = NBuildSqlCommand(queryString, paramObj))
            return (T) Convert.ChangeType(cmd.ExecuteScalar(), Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T));
    }

    public Int32 Execute(String queryString) {
        Open();
        Int32 result;
        using (DbCommand cmd = BuildSqlCommand(queryString))
            result = cmd.ExecuteNonQuery();

        return result;
    }

    public Int32 Execute(String queryString, params Object[] queryParams) {
        Open();
        Int32 result;
        using (DbCommand cmd = BuildSqlCommand(queryString, queryParams))
            result = cmd.ExecuteNonQuery();

        return result;
    }

    public Int32 NExecute(String queryString, Object paramObj) {
        Open();
        Int32 result;
        using (DbCommand cmd = NBuildSqlCommand(queryString, paramObj))
            result = cmd.ExecuteNonQuery();

        return result;
    }

    public void Dispose() {
        if (connection != null) {
            if (connection.State != ConnectionState.Closed)
                connection.Close();

            connection.Dispose();
            connection = null;
        }

        GC.SuppressFinalize(this);
    }

    TResult ToTResult<TResult>(IDataRecord record) {
        var t = Activator.CreateInstance<TResult>();
        Type tType = typeof(TResult);
        PropertyInfo[] tProperties = tType.GetProperties();
        FieldInfo[] tFields = tType.GetFields();

        if (tProperties.Length != 0) {
            foreach (PropertyInfo property in tProperties) {
                Object result = record[record.GetOrdinal(property.Name)];
                if (result != DBNull.Value) {
                    Type safeType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                    Object safeResult = Convert.ChangeType(result, safeType);

                    property.SetValue(t, safeResult, null);
                }
            }
        }

        if (tFields.Length != 0) {
            foreach (FieldInfo field in tFields) {
                Object result = record[record.GetOrdinal(field.Name)];
                if (result != DBNull.Value) {
                    Type safeType = Nullable.GetUnderlyingType(field.FieldType) ?? field.FieldType;
                    Object safeResult = Convert.ChangeType(result, safeType);

                    field.SetValue(t, safeResult);
                }
            }
        }

        return t;
    }
}