using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Migrations.Sql;
using System.Globalization;
using System.Linq;
using System.Text;

namespace System.Data.Sqlite
{
    public class SqliteSqlVisitor : DbExpressionVisitor
    {

        private readonly StringBuilder _commandText;

        internal SqliteSqlVisitor(
            StringBuilder commandText)
        {
            _commandText = commandText;

        }

        public override void Visit(DbVariableReferenceExpression expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DbUnionAllExpression expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DbTreatExpression expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DbSortExpression expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DbSkipExpression expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DbScanExpression expression)
        {

            Check.NotNull(expression, "expression");

            _commandText.Append(GetTargetTSql(expression.Target));

        }


        // <summary>
        // Gets escaped TSql identifier describing this entity set.
        // </summary>
        internal static string GetTargetTSql(EntitySetBase entitySetBase)
        {
            var definingQuery = entitySetBase.GetMetadataPropertyValue<string>("DefiningQuery");
            if (definingQuery != null)
            {
                return "(" + definingQuery + ")";
            }
            // construct escaped T-SQL referencing entity set
            var builder = new StringBuilder(50);

            var table = entitySetBase.GetMetadataPropertyValue<string>("Table");
            builder.Append(
                string.IsNullOrEmpty(table)
                    ? QuoteIdentifier(entitySetBase.Name)
                    : QuoteIdentifier(table));

            return builder.ToString();
        }

        internal static string QuoteIdentifier(string name)
        {
            DebugCheck.NotEmpty(name);
            // We assume that the names are not quoted to begin with.
            return "\"" + name + "\"";
        }

        public override void Visit(DbRelationshipNavigationExpression expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DbRefExpression expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DbQuantifierExpression expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DbPropertyExpression expression)
        {
            Check.NotNull(expression, "expression");


            _commandText.Append(GenerateMemberTSql(expression.Property));
        }

        internal static string GenerateMemberTSql(EdmMember member)
        {
            return "\"" + member.Name + "\"";
        }

        public override void Visit(DbProjectExpression expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DbParameterReferenceExpression expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DbOrExpression expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DbOfTypeExpression expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DbNullExpression expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DbNotExpression expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DbNewInstanceExpression expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DbLimitExpression expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DbLikeExpression expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DbJoinExpression expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DbIsOfExpression expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DbIsNullExpression expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DbIsEmptyExpression expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DbIntersectExpression expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DbGroupByExpression expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DbRefKeyExpression expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DbEntityRefExpression expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DbFunctionExpression expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DbFilterExpression expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DbExceptExpression expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DbElementExpression expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DbDistinctExpression expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DbDerefExpression expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DbCrossJoinExpression expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DbConstantExpression expression)
        {
            Check.NotNull(expression, "expression");

            string returnValue = "";

            var primitiveTypeKind = ((PrimitiveType)expression.ResultType.EdmType).PrimitiveTypeKind;


            // CONSIDER(CMeek):: add logic for Xml here
            switch (primitiveTypeKind)
            {
                case PrimitiveTypeKind.Binary:
                    returnValue = "X'"+ToHexString((byte[])expression.Value)+"'";
                    break;

                case PrimitiveTypeKind.Boolean:
                    returnValue = (((bool)expression.Value)?"1":"0");
                    break;

                case PrimitiveTypeKind.Byte:
                    returnValue = "X'" + ((byte)expression.Value).ToString("X2", CultureInfo.InvariantCulture) + "'";
                    break;

                case PrimitiveTypeKind.Time:
                    returnValue = "'"+((DateTime)expression.Value).ToString("yyyy-MM-dd HH:mm:ss")+"'";
                    break;

                case PrimitiveTypeKind.DateTimeOffset:
                    returnValue = "'" + ((DateTime)expression.Value).ToString("yyyy-MM-dd HH:mm:ss") + "'";
                    break;

                case PrimitiveTypeKind.DateTime:
                    returnValue = "'" + ((DateTime)expression.Value).ToString("yyyy-MM-dd HH:mm:ss") + "'";
                    break;

                case PrimitiveTypeKind.Decimal:
                    returnValue = ((decimal)expression.Value).ToString();
                    break;

                case PrimitiveTypeKind.Double:
                    returnValue = ((double)expression.Value).ToString();
                    break;

                case PrimitiveTypeKind.Geography:
                    returnValue = ((int)expression.Value).ToString();
                    break;

                case PrimitiveTypeKind.Geometry:
                    returnValue = ((Int32)expression.Value).ToString();
                    break;

                case PrimitiveTypeKind.Guid:
                    returnValue = "X'" + ToHexString(((Guid)expression.Value).ToByteArray()) + "'";
                    break;

                case PrimitiveTypeKind.Int16:
                    returnValue = ((Int16)expression.Value).ToString();
                    break;

                case PrimitiveTypeKind.Int32:
                    returnValue = ((Int32)expression.Value).ToString();
                    break;

                case PrimitiveTypeKind.Int64:
                    returnValue = ((Int64)expression.Value).ToString();
                    break;

                case PrimitiveTypeKind.SByte:
                    returnValue = "X'" + ((sbyte)expression.Value).ToString("X2", CultureInfo.InvariantCulture) + "'";
                    break;

                case PrimitiveTypeKind.Single:
                    returnValue = ((Single)expression.Value).ToString();
                    break;

                case PrimitiveTypeKind.String:
                    returnValue = "'"+(string)expression.Value+"'";
                    break;

            }

            _commandText.Append(returnValue);
        }

        public static string ToHexString(IEnumerable<byte> bytes)
        {
            var stringBuilder = new StringBuilder();
            foreach (var @byte in bytes)
            {
                stringBuilder.Append(@byte.ToString("X2", CultureInfo.InvariantCulture));
            }
            return stringBuilder.ToString();
        }

        public override void Visit(DbComparisonExpression expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DbCastExpression expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DbCaseExpression expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DbArithmeticExpression expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DbApplyExpression expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DbAndExpression expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DbExpression expression)
        {
            throw new NotImplementedException();
        }
    }

    internal static class MetdataItemExtensions
    {
        public static T GetMetadataPropertyValue<T>(this MetadataItem item, string propertyName)
        {
            DebugCheck.NotNull(item);

            var property = item.MetadataProperties.FirstOrDefault(p => p.Name == propertyName);
            return property == null ? default(T) : (T)property.Value;
        }
    }
}
