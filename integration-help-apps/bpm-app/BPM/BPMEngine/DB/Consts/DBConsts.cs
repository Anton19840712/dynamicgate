namespace BPMEngine.DB.Consts;

public static class DBConsts
{
    public static class DBConnections
    {
        /// <summary>
        /// BPMEngine_dev
        /// </summary>
        public static string ConnectionString_BPM = string.Empty;
        /// <summary>
        /// CardSystem_dev
        /// </summary>
        public static string ConnectionString_BLL = string.Empty;
    }
    public static class SQL
    {
        /// <summary>
        /// "SELECT"
        /// </summary>
        public static readonly string SelectSql = "SELECT";
        /// <summary>
        /// "FROM"
        /// </summary>
        public static readonly string FromSql = "FROM";
        /// <summary>
        /// "WHERE"
        /// </summary>
        public static readonly string WhereSql = "WHERE";
        /// <summary>
        /// "="
        /// </summary>
        public static readonly string EqualsSql = "=";
        /// <summary>
        /// "!="
        /// </summary>
        public static readonly string NotEqualsSql = "!=";
        /// <summary>
        /// "AND"
        /// </summary>
        public static readonly string AndSql = "AND";
        /// <summary>
        /// "OR"
        /// </summary>
        public static readonly string OrSql = "OR";
        /// <summary>
        /// "IN"
        /// </summary>
        public static readonly string InSql = "IN";
        /// <summary>
        /// "INSERT"
        /// </summary>
        public static readonly string InsertSql = "INSERT";
        /// <summary>
        /// "INTO"
        /// </summary>
        public static readonly string IntoSql = "INTO";
        /// <summary>
        /// "VALUES"
        /// </summary>
        public static readonly string ValuesSql = "VALUES";
        /// <summary>
        /// "DEFAULT"
        /// </summary>
        public static readonly string DefaultSql = "DEFAULT";
        /// <summary>
        /// "UPDATE"
        /// </summary>
        public static readonly string UpdateSql = "UPDATE";
        /// <summary>
        /// "SET"
        /// </summary>
        public static readonly string SetSql = "SET";
        /// <summary>
        /// "DELETE"
        /// </summary>
        public static readonly string DeleteSql = "DELETE";
    }
    public static class ORMEnums
    {
        public enum QueryFilterType
        {
            EQUALS = 0,
            NOT_EQUALS = 1,
            IN = 2,
        }
        public enum QueryGroupFilterType
        {
            AND = 0,
            OR = 1
        }
    }
}