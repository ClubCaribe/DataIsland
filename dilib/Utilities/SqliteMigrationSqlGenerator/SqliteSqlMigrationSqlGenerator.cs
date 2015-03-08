using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Migrations.History;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Migrations.Utilities;
using System.Data.Entity.Spatial;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Data.SQLite;
using System.Data.SQLite.EF6;
using System.Data.SQLite.Linq;
using System.Data.Sqlite;

namespace System.Data.Entity.Migrations.Sql
{
    public class SqliteSqlMigrationSqlGenerator : MigrationSqlGenerator
    {
        private const string BatchTerminator = "GO";
        internal const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        private IList<MigrationStatement> _statements;
        private HashSet<string> _generatedSchemas;
        private string _providerManifestToken;
        private List<MigrationOperation> _migrationsOperations = new List<MigrationOperation>();

        public override IEnumerable<MigrationStatement> Generate(IEnumerable<MigrationOperation> migrationOperations, string providerManifestToken)
        {

            Check.NotNull(migrationOperations, "migrationOperations");
            Check.NotNull(providerManifestToken, "providerManifestToken");

            _statements = new List<MigrationStatement>();
            _generatedSchemas = new HashSet<string>();

            InitializeProviderServices(providerManifestToken);
            GenerateStatements(migrationOperations);

            return _statements;

        }

        private void GenerateStatements(IEnumerable<MigrationOperation> migrationOperations)
        {

            Check.NotNull(migrationOperations, "migrationOperations");

            _migrationsOperations.Clear();
            foreach(var o in DetectHistoryRebuild(migrationOperations))
            {
                _migrationsOperations.Add(o);
            }

            for (int i = 0; i < _migrationsOperations.Count; i++)
            {
                GenerateSql(i);
            }

        }

        private void InitializeProviderServices(string providerManifestToken)
        {

            Check.NotEmpty(providerManifestToken, "providerManifestToken");

            _providerManifestToken = providerManifestToken;

        }

        private static IEnumerable<MigrationOperation> DetectHistoryRebuild(
            IEnumerable<MigrationOperation> operations)
        {
            DebugCheck.NotNull(operations);

            var enumerator = operations.GetEnumerator();

            while (enumerator.MoveNext())
            {
                var sequence = HistoryRebuildOperationSequence.Detect(enumerator);

                yield return sequence ?? enumerator.Current;
            }
        }


        private void GenerateHistory(HistoryOperation migration)
        {
            //migration

            Check.NotNull(migration, "historyOperation");

            using (var writer = Writer())
            {
                foreach (var commandTree in migration.CommandTrees)
                {
                    switch (commandTree.CommandTreeKind)
                    {
                        case DbCommandTreeKind.Insert:

                            writer.Write(GetInsertHistorySql((DbInsertCommandTree)commandTree));

                            break;
                    }
                }

                Statement(writer);
            }

        }


        string GetInsertHistorySql(DbInsertCommandTree tree)
        {


            var commandText = new StringBuilder();

            var visitor = new SqliteSqlVisitor(commandText);

            commandText.Append("insert into ");

            tree.Target.Expression.Accept(visitor);
            commandText.Append(" ");

            if (0 < tree.SetClauses.Count)
            {
                // (c1, c2, c3, ...)
                commandText.Append("(");
                var first = true;
                foreach (DbSetClause setClause in tree.SetClauses)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        commandText.Append(", ");
                    }
                    setClause.Property.Accept(visitor);
                }
                commandText.AppendLine(")");
            }


            if (0 < tree.SetClauses.Count)
            {
                // values c1, c2, ...
                var first = true;
                commandText.Append(" values (");
                foreach (DbSetClause setClause in tree.SetClauses)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        commandText.Append(", ");
                    }
                    setClause.Value.Accept(visitor);
                }
                commandText.AppendLine(")");
            }

            return commandText.ToString();
        }

        private void GenerateAlterColumn(AlterColumnOperation migration)
        {
            
        }

        private void GenerateRenameTable(RenameTableOperation migration)
        {
            Contract.Requires(migration != null);

            using (var writer = Writer())
            {
                writer.Write("ALTER TABLE ");
                writer.Write(RemoveDboFromName(migration.Name));
                writer.Write(" RENAME TO ");
                writer.Write(RemoveDboFromName(migration.NewName));

                Statement(writer);
            }
        }


        private void GenerateDropColumn(DropColumnOperation dropColumnOperation)
        {
            Contract.Requires(dropColumnOperation != null);

            
        }


        private void Generate(ColumnModel column, TextWriter writer, bool isAlter = false)
        {
            Contract.Requires(column != null);
            Contract.Requires(writer != null);

            writer.Write(column.Name+" ");
            writer.Write(BuildColumnType(column));


            // check if it has a max length
            if (column.MaxLength != null && column.MaxLength > 0)

                writer.Write("({0})", column.MaxLength);


            // check if it's nullable
            if ((column.IsNullable != null)
                && !column.IsNullable.Value
                && isAlter == false)
            {

                writer.Write(" NOT NULL");
            }
            else
            {
                writer.Write(" NULL");
            }


            // check if it has a default value
            if (column.DefaultValue != null)
            {
                writer.Write(" DEFAULT ");
                writer.Write(Generate((dynamic)column.DefaultValue));
            }
            else if (!string.IsNullOrWhiteSpace(column.DefaultValueSql))
            {
                writer.Write(" DEFAULT ");
                writer.Write(column.DefaultValueSql);
            }

        }


        private void GenerateAddColumn(AddColumnOperation addColumnOperation)
        {
            Contract.Requires(addColumnOperation != null);

            using (var writer = Writer())
            {
                writer.Write("ALTER TABLE ");
                writer.Write(RemoveDboFromName(addColumnOperation.Table));
                writer.Write(" ADD COLUMN ");

                var column = addColumnOperation.Column;

                Generate(column, writer);
                
                if ((column.IsNullable != null)
                    && !column.IsNullable.Value
                    && (column.DefaultValue == null)
                    && (string.IsNullOrWhiteSpace(column.DefaultValueSql))
                    && !column.IsIdentity
                    && !column.IsTimestamp)
                {
                    writer.Write(" DEFAULT ");

                    if (column.Type == PrimitiveTypeKind.DateTime)
                    {
                        writer.Write(Generate(DateTime.Parse("1900-01-01 00:00:00", CultureInfo.InvariantCulture)));
                    }
                    else
                    {
                        writer.Write(Generate((dynamic)column.ClrDefaultValue));
                    }
                }

                Statement(writer);
            }
        }


        private void GenerateCreateIndex(CreateIndexOperation createIndexOperation)
        {
            Contract.Requires(createIndexOperation != null);

            using (var writer = Writer())
            {
                writer.Write("CREATE ");

                if (createIndexOperation.IsUnique)
                {
                    writer.Write("UNIQUE ");
                }

                writer.Write("INDEX IF NOT EXISTS ");
                writer.Write(IndexName(createIndexOperation, false));
                writer.Write(" ON ");
                writer.Write(RemoveDboFromName(createIndexOperation.Table));
                writer.Write("(");
                writer.Write(string.Join(",",createIndexOperation.Columns));
                writer.Write(")");

                Statement(writer);
            }
        }


        protected virtual void GenerateSql(SqlOperation sqlOperation)
        {
            Check.NotNull(sqlOperation, "sqlOperation");

            StatementBatch(sqlOperation.Sql, sqlOperation.SuppressTransaction);
        }


        protected void StatementBatch(string sqlBatch, bool suppressTransaction = false)
        {
            Check.NotNull(sqlBatch, "sqlBatch");

            // Handle backslash utility statement (see http://technet.microsoft.com/en-us/library/dd207007.aspx)
            sqlBatch = Regex.Replace(sqlBatch, @"\\(\r\n|\r|\n)", "");

            // Handle batch splitting utility statement (see http://technet.microsoft.com/en-us/library/ms188037.aspx)
            var batches = Regex.Split(sqlBatch,
                String.Format(CultureInfo.InvariantCulture, @"\s+({0}[ \t]+[0-9]+|{0})(?:\s+|$)", BatchTerminator),
                RegexOptions.IgnoreCase);

            for (int i = 0; i < batches.Length; ++i)
            {
                // Skip batches that merely contain the batch terminator
                if (batches[i].StartsWith(BatchTerminator, StringComparison.OrdinalIgnoreCase) ||
                    (i == batches.Length - 1 && string.IsNullOrWhiteSpace(batches[i])))
                {
                    continue;
                }

                // Include batch terminator if the next element is a batch terminator
                if (batches.Length > i + 1 &&
                    batches[i + 1].StartsWith(BatchTerminator, StringComparison.OrdinalIgnoreCase))
                {
                    int repeatCount = 1;

                    // Handle count parameter on the batch splitting utility statement
                    if (!batches[i + 1].Equals(BatchTerminator,StringComparison.OrdinalIgnoreCase))
                    {
                        repeatCount = int.Parse(Regex.Match(batches[i + 1], @"([0-9]+)").Value, CultureInfo.InvariantCulture);
                    }

                    for (int j = 0; j < repeatCount; ++j)
                        Statement(batches[i], suppressTransaction, BatchTerminator);
                }
                else
                {
                    Statement(batches[i], suppressTransaction);
                }
            }
        }


        private void GenerateAddForeignKey(AddForeignKeyOperation addForeignKeyOperation)
        {
            
        }


        private void GeneratePrimaryKey(PrimaryKeyOperation addPrimaryKeyOperation)
        {
            
        }


        private void GenerateCreateTable(CreateTableOperation createTableOperation, int index)
        {
            Contract.Requires(createTableOperation != null);

            using (var writer = Writer())
            {
                WriteCreateTable(createTableOperation, writer,index);

                Statement(writer);
            }
        }


        private void GenerateDropIndex(DropIndexOperation dropIndexOperation)
        {
            Contract.Requires(dropIndexOperation != null);

            using (var writer = Writer())
            {
                writer.Write("DROP INDEX ");
                writer.Write(IndexName(dropIndexOperation, true));

                Statement(writer);
            }
        }


        protected virtual string IndexName(IndexOperation index, bool withSchema)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(index.Table));
            Contract.Requires(!(!index.Columns.Any()));

            var databaseName = RemoveDboFromName(index.Table);


            var name = new List<string>();

            // check if I've to add the schema name before the index name
            // needed during drop operation
            if (withSchema)

                name.Add(databaseName);

            name.Add(string.Format(CultureInfo.InvariantCulture, "index_{0}_{1}",
                                  RemoveDboFromName(index.Table),
                                  string.Join("_",index.Columns.ToArray())));

            return string.Join(".", name.ToArray());

        }


        private void GenerateCreateSchema(string schema)
        {
        }


        private void WriteCreateTable(CreateTableOperation createTableOperation, IndentedTextWriter writer, int index)
        {
            Contract.Requires(createTableOperation != null);
            Contract.Requires(writer != null);

            writer.WriteLine("CREATE TABLE " + RemoveDboFromName(createTableOperation.Name) + " (");
            writer.Indent++;

            var columnCount = createTableOperation.Columns.Count();

            int c = 0;
            foreach (var column in createTableOperation.Columns)
            {
                Generate(column, writer);

                if (c < columnCount - 1)
                {
                    writer.WriteLine(",");
                }

                c++;
            }
            
            if (createTableOperation.PrimaryKey != null)
            {
                writer.WriteLine(",");
                writer.Write("CONSTRAINT ");
                writer.Write(RemoveDboFromName(createTableOperation.PrimaryKey.Name));
                writer.Write(" PRIMARY KEY (");
                writer.Write(string.Join(",",createTableOperation.PrimaryKey.Columns.ToArray()));
                writer.WriteLine(")");
            }
            else
            {
                writer.WriteLine();
            }

            for (int i = index + 1; i < this._migrationsOperations.Count; i++)
            {
                if(this._migrationsOperations[i] is AddForeignKeyOperation)
                {
                    AddForeignKeyOperation operation = (AddForeignKeyOperation)this._migrationsOperations[i];
                    writer.WriteLine(",");
                    writer.WriteLine(String.Format("FOREIGN KEY({0}) REFERENCES {1}({2})", string.Join(",", operation.PrincipalColumns.ToArray()), RemoveDboFromName(operation.DependentTable), string.Join(",", operation.DependentColumns.ToArray())));
                    this._migrationsOperations.RemoveAt(i);
                    i = index + 1;
                }
            }

                writer.Indent--;
            writer.Write(")");
            

        }



        protected virtual string Generate(DbGeometry defaultValue)
        {
            return "'" + defaultValue + "'";
        }

        protected virtual void GenerateSql(int index)
        {
            object operation = this._migrationsOperations[index];
            if (operation is RenameTableOperation)
            {
                GenerateRenameTable((RenameTableOperation)operation);
            }
            if (operation is AlterColumnOperation)
            {
                GenerateAlterColumn((AlterColumnOperation)operation);
            }
            else if (operation is DropIndexOperation)
            {
                GenerateDropIndex((DropIndexOperation)operation);

            }
            else if (operation is CreateTableOperation)
            {
                GenerateCreateTable((CreateTableOperation)operation,index);

            }
            else if (operation is PrimaryKeyOperation)
            {
                GeneratePrimaryKey((PrimaryKeyOperation)operation);

            }
            else if (operation is AddForeignKeyOperation)
            {
                GenerateAddForeignKey((AddForeignKeyOperation)operation);

            }
            else if (operation is SqlOperation)
            {
                GenerateSql((SqlOperation)operation);

            }
            else if (operation is CreateIndexOperation)
            {
                GenerateCreateIndex((CreateIndexOperation)operation);

            }
            else if (operation is AddColumnOperation)
            {
                GenerateAddColumn((AddColumnOperation)operation);

            }
            else if (operation is DropColumnOperation)
            {
                GenerateDropColumn((DropColumnOperation)operation);

            }
            else if (operation is AlterColumnOperation)
            {
                GenerateAlterColumn((AlterColumnOperation)operation);

            }
            else if (operation is HistoryOperation)
            {
                GenerateHistory((HistoryOperation)operation);
            }
            else if (operation is DropTableOperation)
            {
                GenerateDropTable((DropTableOperation)operation);
            }
        }

        protected virtual string Generate(string defaultValue)
        {
            Check.NotNull(defaultValue, "defaultValue");

            return "'" + defaultValue + "'";
        }


        protected virtual string Generate(object defaultValue)
        {
            Check.NotNull(defaultValue, "defaultValue");
            //Debug.Assert(defaultValue.GetType().IsValueType());

            return string.Format(CultureInfo.CurrentCulture, "{0}", defaultValue);
        }


        private string Generate(bool defaultValue)
        {
            return (defaultValue ? "TRUE" : "FALSE");
        }


        private string Generate(IEnumerable<byte> defaultValue)
        {
            Contract.Requires(defaultValue != null);

            return "decode('" + ToHexString(defaultValue) + "', 'hex')";

        }


        protected virtual string Generate(DateTime defaultValue)
        {
            return "'" + defaultValue.ToString(DateTimeFormat, CultureInfo.InvariantCulture) + "'";
        }


        private void GenerateDropTable(DropTableOperation dropTableOperation)
        {
            Contract.Requires(dropTableOperation != null);

            using (var writer = Writer())
            {
                writer.Write("DROP TABLE ");
                writer.Write(RemoveDboFromName(dropTableOperation.Name));

                Statement(writer);
            }
        }


        private void Generate(DropPrimaryKeyOperation dropPrimaryKeyOperation)
        {
            Contract.Requires(dropPrimaryKeyOperation != null);

            
        }


        private static string BuildColumnType(ColumnModel column)
        {
            Contract.Requires(column != null);

            // if the type is already set, I return it
            if (String.IsNullOrWhiteSpace(column.StoreType) == false)

                return column.StoreType;


            // handle the others cases
            switch (column.Type)
            {
                case PrimitiveTypeKind.Binary:
                    return "BLOB";
                case PrimitiveTypeKind.Boolean:
                    return "BOOLEAN";
                case PrimitiveTypeKind.Byte:
                    return "INTEGER";
                case PrimitiveTypeKind.DateTime:
                    return "DATETIME";
                case PrimitiveTypeKind.Decimal:
                    return "DECIMAL";
                case PrimitiveTypeKind.Double:
                    return "FLOAT";
                case PrimitiveTypeKind.Single:
                    return "REAL";
                case PrimitiveTypeKind.Int16:
                    return "SMALLINT";
                case PrimitiveTypeKind.Int32:
                    return "INTEGER";
                case PrimitiveTypeKind.Int64:
                    return "INTEGER";
                case PrimitiveTypeKind.String:
                    return (column.MaxLength != null && column.MaxLength > 0 ?
                        "VARCHAR" :
                        "TEXT");
                case PrimitiveTypeKind.Time:
                    return "DATETIME";
                case PrimitiveTypeKind.Guid:
                    return "BLOB";
                case PrimitiveTypeKind.Geometry:
                    return "FLOATING POINT";
                default:
                    return "";
            }
        }


        /// <summary>
        /// Quotes an identifier for SQL Server.
        /// </summary>
        /// <param name="identifier"> The identifier to be quoted. </param>
        /// <returns> The quoted identifier. </returns>
        protected virtual string Quote(string identifier)
        {
            Check.NotEmpty(identifier, "identifier");

            return "\"" + identifier + "\"";
        }

        /// <summary>
        /// Generates a quoted name. The supplied name may or may not contain the schema.
        /// </summary>
        /// <param name="name"> The name to be quoted. </param>
        /// <returns> The quoted name. </returns>
        [SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId = "0#")]
        protected virtual string Name(string name)
        {
            Check.NotEmpty(name, "name");

            return name;
        }

        protected virtual string RemoveDboFromName(string name)
        {
            Check.NotEmpty(name, "name");

            return name.Replace("dbo.", "");
        }


        /// <summary>
        /// Gets a new <see cref="IndentedTextWriter" /> that can be used to build SQL.
        /// This is just a helper method to create a writer. Writing to the writer will
        /// not cause SQL to be registered for execution. You must pass the generated
        /// SQL to the Statement method.
        /// </summary>
        /// <returns> An empty text writer to use for SQL generation. </returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        protected static IndentedTextWriter Writer()
        {
            return new IndentedTextWriter(new StringWriter(CultureInfo.InvariantCulture));
        }


        /// <summary>
        /// Adds a new Statement to be executed against the database.
        /// </summary>
        /// <param name="writer"> The writer containing the SQL to be executed. </param>
        /// <param name="batchTerminator">The batch terminator for the database provider.</param>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        protected void Statement(IndentedTextWriter writer, string batchTerminator = null)
        {
            Check.NotNull(writer, "writer");

            Statement(writer.InnerWriter.ToString(), batchTerminator: batchTerminator);
        }

        /// <summary>
        /// Adds a new Statement to be executed against the database.
        /// </summary>
        /// <param name="sql"> The statement to be executed. </param>
        /// <param name="suppressTransaction"> Gets or sets a value indicating whether this statement should be performed outside of the transaction scope that is used to make the migration process transactional. If set to true, this operation will not be rolled back if the migration process fails. </param>
        /// <param name="batchTerminator">The batch terminator for the database provider.</param>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        protected void Statement(string sql, bool suppressTransaction = false, string batchTerminator = null)
        {
            Check.NotEmpty(sql, "sql");

            _statements.Add(
                new MigrationStatement
                {
                    Sql = sql,
                    SuppressTransaction = suppressTransaction,
                    BatchTerminator = batchTerminator
                });
        }

        public string ToHexString(IEnumerable<byte> bytes)
        {
            Contract.Requires(bytes != null);
            var stringBuilder = new StringBuilder();
            foreach (var @byte in bytes)
            {
                stringBuilder.Append(@byte.ToString("X2", CultureInfo.InvariantCulture));
            }
            return stringBuilder.ToString();
        }


        private class HistoryRebuildOperationSequence : MigrationOperation
        {
            public readonly AddColumnOperation AddColumnOperation;
            public readonly DropPrimaryKeyOperation DropPrimaryKeyOperation;

            private HistoryRebuildOperationSequence(
                AddColumnOperation addColumnOperation,
                DropPrimaryKeyOperation dropPrimaryKeyOperation)
                : base(null)
            {
                AddColumnOperation = addColumnOperation;
                DropPrimaryKeyOperation = dropPrimaryKeyOperation;
            }

            public override bool IsDestructiveChange
            {
                get { return false; }
            }

            public static HistoryRebuildOperationSequence Detect(IEnumerator<MigrationOperation> enumerator)
            {
                const string historyTableName = HistoryContext.DefaultTableName;

                var addColumnOperation = enumerator.Current as AddColumnOperation;
                if (addColumnOperation == null
                    || addColumnOperation.Table != historyTableName
                    || addColumnOperation.Column.Name != "ContextKey")
                {
                    return null;
                }

                Debug.Assert(addColumnOperation.Column.DefaultValue is string);

                enumerator.MoveNext();
                var dropPrimaryKeyOperation = (DropPrimaryKeyOperation)enumerator.Current;
                Debug.Assert(dropPrimaryKeyOperation.Table == historyTableName);
                DebugCheck.NotNull(dropPrimaryKeyOperation.CreateTableOperation);

                enumerator.MoveNext();
                var alterColumnOperation = (AlterColumnOperation)enumerator.Current;
                Debug.Assert(alterColumnOperation.Table == historyTableName);

                enumerator.MoveNext();
                var addPrimaryKeyOperation = (AddPrimaryKeyOperation)enumerator.Current;
                Debug.Assert(addPrimaryKeyOperation.Table == historyTableName);

                return new HistoryRebuildOperationSequence(
                    addColumnOperation, dropPrimaryKeyOperation);
            }
        }


    }


    internal class DebugCheck
    {
        [Conditional("DEBUG")]
        public static void NotNull<T>(T value) where T : class
        {
            Debug.Assert(value != null);
        }

        [Conditional("DEBUG")]
        public static void NotNull<T>(T? value) where T : struct
        {
            Debug.Assert(value != null);
        }

        [Conditional("DEBUG")]
        public static void NotEmpty(string value)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(value));
        }
    }


    internal class Check
    {
        public static T NotNull<T>(T value, string parameterName) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            return value;
        }

        public static T? NotNull<T>(T? value, string parameterName) where T : struct
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            return value;
        }

        public static string NotEmpty(string value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException();
            }

            return value;
        }
    }
}
