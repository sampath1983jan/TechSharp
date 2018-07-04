using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MySql.Data.MySqlClient;
 namespace TechSharpy.Data
{

    public enum QueryType
    {
        _Select,
        _Insert,
        _Update,
        _Delete
    }
    public enum FieldType
    {
        _Number,
        _Decimal,
        _Currency,
        _Double,
        _String,
        _Lookup,
        _Date,
        _DateTime,
        _Question,
        _Text,
        _Time,
        _File,
        _Image,
        _Entity
    }
    public enum Operator
    {
        _Equal,
        _NotEqual,
        _In,
        _NotIn,
        _Between,
        _Greater,
        _Greaterthan,
        _Less,
        _Lessthan,
        _Contain,
        _StartWidth,
        _EndWidth,

    }
    public enum JoinType{
        _InnerJoin,
        _LeftJoin,
        _RightJoin,
        
    }
    public enum Condition { 
        _None,
        _And,
        _Or
    }
    public enum SortType{
        _Asc,
        _Desc
    }
    public enum Aggregate { 
        _None,
        _Sum,
        _Average,
        _Min,
        _Max,
        _Count

    }
     public static class QueryResource{
         public static string _delete_table_more_exist = "Delete function cannot perform due to more then one table exist in the query.";

     }

   public class Query
    {     
        public QueryType Type;
        public List<Field> QueryFields;
        public List<Table> Tables;
        public List<Join> Joins;
        public List<WhereGroup> WhereGroups;
        public List<SortOrder> Ordersby;
        private const string Select = "Select {0} from {1} {2}";
        private const string Delete = "Delete from {0}";
        private const string Update = "update {0} set {1} {2}";
        private const string Insert = "Insert Into {0} ({1}) values({2}) ";
        public char FieldSelector = '`';
       /// <summary>
       /// 
       /// </summary>
       /// <param name="queryType"></param>
        public Query(QueryType queryType) {
            this.Type = queryType;
            QueryFields = new List<Field>();
            Tables = new List<Table>();
            WhereGroups = new List<WhereGroup>();
            Ordersby = new List<SortOrder>();
            Joins =new  List<Join>();

        }
       /// <summary>
       /// 
       /// </summary>
       /// <param name="pTableName"></param>
       /// <param name="pAliasName"></param>
       /// <returns></returns>
        public Query AddTable(string pTableName,string pAliasName = "") {
            if (!isTableExist(pTableName) && ! isTableExist(pAliasName))
                this.Tables.Add(new Table(pTableName,pAliasName));
            return this;
        }
       /// <summary>
       /// 
       /// </summary>
       /// <param name="pFieldName"></param>
       /// <param name="TableName"></param>
       /// <param name="pType"></param>
       /// <param name="AliasName"></param>
       /// <param name="Value"></param>
       /// <param name="pOperator"></param>
       /// <returns></returns>
        public Query AddField(string pFieldName, string TableName, FieldType pType,
            string AliasName="",string Value="",Operator pOperator=Operator._Equal,Aggregate Agg=Aggregate._None)
        {
            QueryFields.Add(new Field(pFieldName, TableName, AliasName, Value, pOperator, pType, Agg));
            if (isTableExist(TableName ))
                    this.Tables.Add(new Table(TableName));
            return this;
        }

        public Query AddField(string pFieldNames, string TableName="", string AliasName="")
        {

            string[] fields;
            string[] separators={","};
            fields = pFieldNames.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < fields.Length; i++) {
                QueryFields.Add(new Field(fields[i].ToString(), TableName, AliasName,"",Operator._Equal,FieldType._String));
            }
            if (TableName != "") {
                if (! isTableExist(TableName))
                    this.Tables.Add(new Table(TableName));
            }           

            return this;

        }

        public Query AddJoin(string pTableName, string pFieldName, JoinType pjoin, string pJoinTable,
           string pJoinField, Condition pCondition = Condition._None)
        {
            Joins.Add(new Join(pTableName, pFieldName, pjoin, pJoinTable, pJoinField, pCondition));
            return this;
        }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="pGroupIndex"></param>
       /// <param name="pTableName"></param>
       /// <param name="pFieldName"></param>
       /// <param name="pType"></param>
       /// <param name="pOperator"></param>
       /// <param name="value"></param>
       /// <param name="pCondition"></param>
       /// <returns></returns>
        public Query AddWhere(int pGroupIndex, string pTableName, string pFieldName, FieldType pType, 
            Operator pOperator = Operator._Equal,
            string value ="",Condition pCondition=Condition._And) {
                WhereGroup wg;

                if (this.WhereGroups.Count > pGroupIndex){
                    wg = this.WhereGroups[pGroupIndex];
                    wg.whereCases.Add(new WhereCase(pGroupIndex, pTableName, pFieldName, pType, pOperator, value, pCondition));
                }
                else {
                    wg = new WhereGroup();
                    wg.whereCases.Add(new WhereCase(pGroupIndex, pTableName, pFieldName, pType, pOperator, value, pCondition));
                    this.WhereGroups.Add(wg);
                }               
                return this;
        }
       /// <summary>
       /// 
       /// </summary>
       /// <param name="pGroupIndex"></param>
       /// <param name="pTableName"></param>
       /// <param name="pFieldName"></param>
       /// <param name="pJoinTable"></param>
       /// <param name="pJoinField"></param>
       /// <param name="pJoinType"></param>
       /// <param name="pCondition"></param>
       /// <returns></returns>
        public Query AddWhere(int pGroupIndex, string pTableName, string pFieldName, 
            string pJoinTable, string pJoinField,
           JoinType pJoinType = JoinType._InnerJoin, Condition pCondition = Condition._And)
        {

            WhereGroup wg;
            if (this.WhereGroups.Count > pGroupIndex)
            {
                wg = this.WhereGroups[pGroupIndex];
                wg.whereCases.Add(new WhereCase(pGroupIndex, pTableName, pFieldName, pJoinTable, pJoinField,
                pJoinType, pCondition));
            }
            else
            {
                wg = new WhereGroup();
                wg.whereCases.Add(new WhereCase(pGroupIndex, pTableName, pFieldName, pJoinTable, pJoinField,
                pJoinType, pCondition));
                this.WhereGroups.Add(wg);
            }                          
            return this;
       }

        public Query AddSortOrder(string pTableName,string pFieldName,SortType pOrder) {
            this.Ordersby.Add(new SortOrder(pTableName, pFieldName, pOrder));
            return this;
        }


       /// <summary>
       /// 
       /// </summary>
       /// <param name="tbName"></param>
       /// <returns></returns>
        private bool isTableExist(string tbName) {
        
            for (int iTblCount = 0; iTblCount < Tables.Count; iTblCount ++)
            {
                if (this.Tables[iTblCount].TableName == tbName || this.Tables[iTblCount].AliasName==tbName) {
                    return true;

                }             
              }
            return false;
        }
       /// <summary>
       /// 
       /// </summary>
       /// <returns></returns>
        //public System.Data.DataTable GetDataTable()
        //{
        //    MySql.Data.MySqlClient.MySqlConnection con = new MySql.Data.MySqlClient.MySqlConnection(GetConnectioin());
        //    con.Open();
        //    System.Data.DataTable dt = new System.Data.DataTable();
        //    MySqlCommand cmd = new MySqlCommand();
        //    cmd.CommandText = toString();
        //    cmd.CommandType = System.Data.CommandType.Text;
        //    cmd.Connection = con;
        //    using (MySqlDataAdapter a = new MySqlDataAdapter(cmd))
        //    {
        //        a.Fill(dt);
        //    }

        //    return dt;
            
        //}
       /// <summary>
       /// 
       /// </summary>
       /// <returns></returns>
        private string GetConnectioin() {
            string Connection = ConfigurationManager.AppSettings["ConnectionString"];
            SMRHRT.Services.Security.CryptoProvider cryp = new SMRHRT.Services.Security.CryptoProvider("check your connection");
         Connection=   cryp.DecryptString(Connection);
         return Connection;

        }
       /// <summary>
       /// 
       /// </summary>
       /// <returns></returns>
        internal string toString() { 

           System.Text.StringBuilder   sb= new System.Text.StringBuilder();
        if( this.Type ==QueryType._Select) {
            System.Text.StringBuilder sbField = new System.Text.StringBuilder();
            System.Text.StringBuilder sbJoin = new System.Text.StringBuilder();
            System.Text.StringBuilder sbWhereGroup = new System.Text.StringBuilder();
            for (int ifld = 0; ifld < this.QueryFields.Count; ifld++)
            {
                Field fld = this.QueryFields[ifld];
                sbField.Append("," + FieldSelector + fld.TableName + FieldSelector + "." + fld.Name);                
            }
            string fd="";
            fd = sbField.ToString();
            if (fd.StartsWith(","))
            {
                fd = fd.Substring(1);
            }
            for (int ijoin = 0; ijoin < this.Joins.Count; ijoin++) { 
                Join jn=this.Joins[ijoin];
                string joinType = "";
                if (jn.Type == JoinType._InnerJoin) {
                    joinType = " Inner Join ";

                }
                else if (jn.Type == JoinType._LeftJoin)
                {
                    joinType = " Left Join ";
                }
                else if (jn.Type==JoinType. _RightJoin)
                { joinType = " Right Join "; 
                }

                if (ijoin == 0)
                {
                    sbJoin.Append(FieldSelector + jn.TableName + FieldSelector + " " + getTableAlias(jn.TableName) +  joinType
                        + FieldSelector + getTableAlias(jn.JoinTable) + FieldSelector + " on " + getTableAlias(jn.TableName) + "." + jn.FieldName + " = "
                        + FieldSelector + getTableAlias(jn.JoinTable) + FieldSelector + "." + jn.JoinField);
                }
                else {
                    sbJoin.Append( joinType
                           + FieldSelector + getTableAlias(jn.JoinTable) + FieldSelector + " on " + getTableAlias(jn.TableName) + "." + jn.FieldName + " = "
                           + FieldSelector + getTableAlias(jn.JoinTable) + FieldSelector + "." + jn.JoinField);
                }
                
            }
            if (this.Joins.Count == 0) {
                if (this.Tables.Count > 1)
                {
                    throw new Exception("More than one table to select");
                }
                else {
                    sbJoin.Append(" " + this.Tables[0].TableName + " ");
                }
            }
           
            for (int iwhereg = 0; iwhereg < this.WhereGroups.Count; iwhereg++)
            {
                if (iwhereg == 0)
                {
                    sbWhereGroup.Append(" Where (");
                }
                else
                {
                    if (this.WhereGroups[iwhereg].condition == Condition._And)
                    {
                        sbWhereGroup.Append(" AND (");
                    }
                    else if (this.WhereGroups[iwhereg].condition == Condition._Or)
                    {
                        sbWhereGroup.Append(" OR (");
                    }
                    else
                    {
                        sbWhereGroup.Append(" ");
                    }
                }
                System.Text.StringBuilder sbWherecase = new System.Text.StringBuilder();
                for (int iwhere = 0; iwhere < this.WhereGroups[iwhereg].whereCases.Count; iwhere++)
                {
                    WhereCase ws = this.WhereGroups[iwhereg].whereCases[iwhere];
                   
                    if (iwhere != 0)
                    {
                        if (ws.condition == Condition._And)
                        {
                            sbWherecase.Append(" AND ");
                        }
                        else if (ws.condition == Condition._Or)
                        {
                            sbWherecase.Append(" OR ");
                        }
                        else
                        {
                            sbWherecase.Append(" ");
                        }
                    }
                    sbWherecase.Append(FieldSelector + getTableAlias(ws.TableName) + FieldSelector + "." + FieldSelector + ws.FieldName + FieldSelector + getOperator(ws.Operation,
                    MakeValidateData(ws.Type, ws.ConditionValue, ws.FieldName)));
                  
                }

                sbWhereGroup.Append(sbWherecase + " ) ");
               
            }
            sb.AppendFormat(Select, fd, sbJoin, sbWhereGroup);
        }
        else if (this.Type == QueryType._Insert) {
            System.Text.StringBuilder sbField = new System.Text.StringBuilder();
            System.Text.StringBuilder sbFieldvalue = new System.Text.StringBuilder();
            for (int ifld = 0; ifld < this.QueryFields.Count; ifld++) {               
                Field fld = this.QueryFields[ifld];
                sbField.Append( ","+ FieldSelector + fld.TableName + FieldSelector + "." + fld.Name );
                sbFieldvalue.Append("," + MakeValidateData(fld.Type, fld.Value, fld.Name));
            }
            sb.AppendFormat(Insert, FieldSelector + this.Tables[0].TableName + FieldSelector, sbField.ToString().Substring(1), sbFieldvalue.ToString().Substring(1));
        }

        else if (this.Type == QueryType._Delete) {
            if (this.Tables.Count > 1) {
                throw new Exception(QueryResource._delete_table_more_exist);
            }
            sb.AppendFormat(Delete, this.Tables[0].TableName);           
        }

        else if (this.Type == QueryType._Update) {
            System.Text.StringBuilder sbField = new System.Text.StringBuilder();
                    System.Text.StringBuilder sbWhereGroup = new System.Text.StringBuilder();

            for (int ifld = 0; ifld < this.QueryFields.Count; ifld++)
            {
                if (ifld != 0) {
                    sbField.Append(",");                    
                }                 
                Field fd;
                fd = this.QueryFields[ifld];
                sbField.Append(FieldSelector + getTableAlias( fd.TableName) + FieldSelector + "." + FieldSelector + fd.Name + FieldSelector + " = ");
                sbField.Append(MakeValidateData(fd.Type,fd.Value,fd.Name));
            }

            for (int iwhereg = 0; iwhereg < this.WhereGroups.Count; iwhereg++)
            {
                if (iwhereg == 0)
                {
                    sbWhereGroup.Append(" Where (");
                }
                else
                {
                    if (this.WhereGroups[iwhereg].condition == Condition._And)
                    {
                        sbWhereGroup.Append(" AND (");
                    }
                    else if (this.WhereGroups[iwhereg].condition == Condition._Or)
                    {
                        sbWhereGroup.Append(" OR (");
                    }
                    else
                    {
                        sbWhereGroup.Append(" ");
                    }
                }
                System.Text.StringBuilder sbWherecase = new System.Text.StringBuilder();
                for (int iwhere = 0; iwhere < this.WhereGroups[iwhereg].whereCases.Count; iwhere++)                {
                    WhereCase ws = this.WhereGroups[iwhereg].whereCases[iwhere];
                  
                    if (iwhere != 0)
                    {
                        if (ws.condition == Condition._And)
                        {
                            sbWherecase.Append(" AND ");
                        }
                        else if (ws.condition == Condition._Or)
                        {
                            sbWherecase.Append(" OR ");
                        }
                        else
                        {
                            sbWherecase.Append(" ");
                        }
                    }
                    sbWherecase.Append(FieldSelector + getTableAlias(ws.TableName) + FieldSelector + "." + FieldSelector + ws.FieldName + FieldSelector + getOperator(ws.Operation,
                    MakeValidateData(ws.Type,ws.ConditionValue,ws.FieldName)));
                  
                }
                sbWhereGroup.Append(sbWherecase + " ) ");
              //  sbWhereGroup.Append(")");
            }
            sb.AppendFormat(Update, this.Tables[0].TableName + " as " + getTableAlias(this.Tables[0].TableName), sbField.ToString(), sbWhereGroup.ToString());
        }            
        return sb.ToString();
        }
        private string MakeValidateData(FieldType fldType,string Value,string FieldName) {
            if (fldType == FieldType._Number || fldType == FieldType._Double || fldType == FieldType._Decimal
                || fldType == FieldType._Currency || fldType == FieldType._Lookup)
            {
                if (!Value.IsNumeric())
                {
                    throw new Exception(string.Format("Invalid Data in {0}", FieldName));
                }
                else {
                    return Value;
                }
            }
            else if (fldType == FieldType._Date || fldType == FieldType._DateTime || fldType == FieldType._Time) {
                if (!Value.IsDate())
                {
                    throw new Exception(string.Format("Invalid Data in {0}", FieldName));
                }
                else {
                    return "'" +  Convert.ToDateTime(Value).ToString("yyyy/MM/dd HH:mm:ss") + "'";
                }
            }
            else if (fldType == FieldType._String || fldType == FieldType._Text)
            {
                return "'" + Value.ToString() + "'";
            }
            else if (fldType == FieldType._Question) {
                if (!Value.IsBool())
                {
                    throw new Exception(string.Format("Invalid Data in {0}", FieldName));
                }
                else {
                    return Boolean.Parse(Value).ToString();
                }
            }
            else if (fldType == FieldType._File || fldType == FieldType._Image) {
                return Value;
            }

            return "";
        }

        private string getOperator(Operator opt,string fval) {
            if (opt == Operator._Equal) {
                return " = " + fval ;
            }
            else if (opt == Operator._Contain)
            {
                return string.Format( "like '%{0}%' ",fval);
            }
            else if (opt == Operator._EndWidth)
            {
                return string.Format("like '%{0}' ", fval);
            }
            else if (opt == Operator._StartWidth)
            {
                return string.Format("like '{0}%' ", fval);
            }
            else if (opt == Operator._Greater)
            {
                return " > " + fval;
            }
            else if (opt == Operator._Less)
            {
                return " < " + fval;
            }
            else if (opt == Operator._Greaterthan)
            {
                return " >= " + fval;
            }
            else if (opt == Operator._Lessthan)
            {
                return "<=" + fval;
            }
            else if (opt == Operator._In)
            {
                return " in (" + fval + ")";
            }
            else if (opt == Operator._NotEqual)
            {
                return " != " + fval                     ;
            }
            else if (opt == Operator._NotIn)
            {
                return " not in (" + fval + ")";
            }
            return " = " + fval;
        }

       private string getTableAlias(string tbName){
           for (int itbl = 0; itbl < this.Tables.Count; itbl++) {
               if (this.Tables[itbl].TableName == tbName)
               {
                   //if (this.Tables[itbl].AliasName != "")
                   //{
                   //    return this.Tables[itbl].AliasName;
                   //}
                   //else {
                   //    return this.Tables[itbl].TableName;
                   //}
                   return this.Tables[itbl].AliasName != "" ? this.Tables[itbl].AliasName : this.Tables[itbl].TableName;
               }               
           }
           return tbName;
       }
        //public bool ValidateSchema()
        //{
        //   var BoolResult = true;
        //    //SELECT\s.*FROM\s.*WHERE\s.*
        //    //regex = New Regex("^select") 'This expression will check whether select statement 
        //   var Schema = GenerateQuery().Trim();
        //    //Me.Schema = Me.Schema.ToLower()
        //   string qry = Schema.ToLower();
        //    try
        //    {
        //        if (qry.StartsWith("select") == false)
        //        {
        //            BoolResult = false;
        //        }
        //        else if (qry.IndexOf("delete ") > 0 | qry.IndexOf("insert ") > 0 | qry.IndexOf("update ") > 0 | qry.IndexOf("drop ") > 0)
        //        {
        //            BoolResult = false;
        //        }
        //        else if (qry.IndexOf("from") < 1)
        //        {
        //            BoolResult = false;
        //        }
        //        else if (qry.IndexOf("#") > 0 | qry.IndexOf("--") > 0 | qry.IndexOf("*/") > 0 | qry.IndexOf("/*") > 0)
        //        {
        //            BoolResult = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return BoolResult;
        //}
    }

    /// <summary>
    /// 
    /// </summary>
   public class Table
   {
       public string TableName;
       public string  AliasName;
       public Table(string pTableName, string pAliasName="")
       {
           this.TableName = pTableName;
           this.AliasName = pAliasName;
       }
   }
    /// <summary>
    /// 
    /// </summary>
   public class Field
   {
       public string Name;
       public string TableName;
       public string Alias;
       public string Value;
       public Operator Operation;
       public FieldType Type;
       public Aggregate FieldAggregate;
       public int Size;
       public bool IsUnique;
       public bool IsKeyField;
       public bool IsRequired;
       public string ReName = "";
       public Field(string pfieldName, string pTableName, string pAliasName,
           string pValue, Operator pOperator, FieldType pType, Aggregate pAgg = Aggregate._None)
       {
           this.Name = pfieldName;
           this.TableName = pTableName;
           this.Alias = pAliasName;
           this.Value = pValue;
           this.Operation = pOperator;
           this.Type=pType;
           this.FieldAggregate = pAgg;
       }
       public string GetNullType() {
           if (this.IsKeyField) {
               return "NOT NULL";
           }
           else if (this.IsRequired)
           {
               return "NOT NULL";
           }
           else {
               return "NULL";
           }
       }
       public string GetBaseDataType() {
           if (this.Type == FieldType._Number || this.Type == FieldType._Lookup
               || this.Type == FieldType._Entity)
           {
               return "INT";
           }
           else if (this.Type == FieldType._String) {
               if (this.Size == 0) {
                   this.Size = 255;
               }
               return "VARCHAR(" + this.Size + ")";
           }
           else if (this.Type == FieldType._Text) {
               return "TEXT";
           }
           else if (this.Type == FieldType._Currency || this.Type == FieldType._Decimal || this.Type == FieldType._Double)
           {
               return "DOUBLE(18,3)";
           }
           else if (this.Type == FieldType._File || this.Type == FieldType._Image)
           {
               return "LONGTEXT";
           }
           else if (this.Type == FieldType._Date || this.Type == FieldType._DateTime || this.Type == FieldType._Time) {
               return "DATETIME";
           }
           else if (this.Type == FieldType._Question)
           {
               return "BIT";
           }
           else {
               return "TEXT";
           }
       }
   }
    /// <summary>
    /// 
    /// </summary>
   public class WhereGroup
   {
       public List<WhereCase> whereCases;
       public Int16 GroupIndex;
       public Condition condition;
       public WhereGroup() {
           this.whereCases = new List<WhereCase>();
           this.condition = new Condition();
       }
   }
    /// <summary>
    /// 
    /// </summary>
   public class WhereCase
   {
       public int GroupIndex;
       public string TableName;
       public string FieldName;
       public FieldType Type;
       public JoinType Join;
       public Condition condition;
       public string JoinTable;
       public string JoinField;
       public string ConditionValue;
       public Operator Operation;
      

       public WhereCase(int pGroupIndex, string pTableName, string pFieldName,  string pJoinTable, string pJoinField,
           JoinType pJoinType = JoinType._InnerJoin, Condition pCondition = Condition._And)
       {
           this.GroupIndex = pGroupIndex;
           this.TableName = pTableName;
           this.FieldName = pFieldName;
           this.Join = pJoinType;
           this.condition = pCondition;
           this.JoinField = pJoinField;
           this.JoinTable = pJoinTable;
         
       }
       public WhereCase(int pGroupIndex, string pTableName, string pFieldName, FieldType pType, Operator pOperator = Operator._Equal,
            string value = "", Condition pCondition = Condition._And)
       {
           this.GroupIndex = pGroupIndex;
           this.TableName = pTableName;
           this.FieldName = pFieldName;
           this.Join = JoinType._InnerJoin;
           this.condition = pCondition;
            this.Type = pType;
            this.JoinTable = "";
            this.JoinField = "";
            this.ConditionValue = value;
            this.Operation = pOperator;
            
       }

   }

   public class Join {
       public string TableName;
       public string FieldName;     
       public JoinType Type;
       public Condition condition;
       public string JoinTable;
       public string JoinField;    
       //public Operator Operation;
       public Join(string pTableName,string pFieldName,JoinType pjoin,string pJoinTable,
           string pJoinField,Condition pCondition =Condition._None) {
           this.TableName = pTableName;
           this.FieldName = pFieldName;
           this.Type = pjoin;
           this.JoinTable = pJoinTable;
           this.JoinField = pJoinField;
           this.condition = pCondition;
       }
   }
   public class SortOrder {

       string TableName;
       string FieldName;
       SortType Type;
       public SortOrder(string pTableName,string pFieldName,SortType pType) {
           this.TableName = pTableName;
           this.FieldName = pFieldName;
           this.Type = pType;

       }
   }

   
}
