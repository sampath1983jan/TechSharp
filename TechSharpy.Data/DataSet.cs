using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Data;
using System.Linq.Dynamic;
using System.Reflection;
using System.Collections;

public static class Extentions
{
    public enum AggregateFunctionE
    {
        Count = 1,
        Sum = 2,
        First = 3,
        Last = 4,
        Average = 5,
        Max = 6,
        Min = 7,
        Exists = 8,
        NONE = 0
    }
     
    /// <summary>
    /// Pivots the DataTable based on provided RowField, DataField, Aggregate Function and ColumnFields.//
    /// </summary>
    /// <param name="rowField">The column name of the Source Table which you want to spread into rows</param>
    /// <param name="dataField">The column name of the Source Table which you want to spread into Data Part</param>
    /// <param name="aggregate">The Aggregate function which you want to apply in case matching data found more than once</param>
    /// <param name="columnFields">The List of column names which you want to spread as columns</param>
    /// <returns>A DataTable containing the Pivoted Data</returns>
    public static DataTable PivotData(this DataTable SourceTable, string rowField, string dataField, AggregateFunctionE aggregate, params string[] columnFields)
    {
           IEnumerable<DataRow> _Source = new List<DataRow>();
        _Source = SourceTable.Rows.Cast<DataRow>();
        DataTable dt = new DataTable();
        string Separator = ".";
        List<string> rowList = _Source.Select(x => x[rowField].ToString()).Distinct().ToList();
        // Gets the list of columns .(dot) separated.
        var colList = _Source.Select(x => (columnFields.Select(n => x[n]).Aggregate((a, b) => a += Separator + b.ToString())).ToString()).Distinct().OrderBy(m => m);

        dt.Columns.Add(rowField);
        foreach (var colName in colList)
            dt.Columns.Add(colName);  // Cretes the result columns.//

        foreach (string rowName in rowList)
        {
            DataRow row = dt.NewRow();
            row[rowField] = rowName;
            foreach (string colName in colList)
            {
                string strFilter = rowField + " = '" + rowName + "'";
                string[] strColValues = colName.Split(Separator.ToCharArray(), StringSplitOptions.None);
                for (int i = 0; i < columnFields.Length; i++)
                    strFilter += " and " + columnFields[i] + " = '" + strColValues[i] + "'";
                row[colName] = SourceTable.Computing(strFilter, dataField, aggregate);
            }
            dt.Rows.Add(row);
        }
        return dt;
    }

    public static DataTable PivotData(this DataTable SourceTable, string rowField, string dataField, AggregateFunctionE aggregate, bool showSubTotal, params string[] columnFields)
    {
        IEnumerable<DataRow> _Source = new List<DataRow>();
        _Source = SourceTable.Rows.Cast<DataRow>();
        DataTable dt = new DataTable();
        string Separator = ".";
        List<string> rowList = _Source.Select(x => x[rowField].ToString()).Distinct().ToList();
        // Gets the list of columns .(dot) separated.
        List<string> colList = _Source.Select(x => columnFields.Aggregate((a, b) => x[a].ToString() + Separator + x[b].ToString())).Distinct().OrderBy(m => m).ToList();

        if (showSubTotal && columnFields.Length > 1)
        {
            string totalField = string.Empty;
            for (int i = 0; i < columnFields.Length - 1; i++)
                totalField += columnFields[i] + "(Total)" + Separator;
            List<string> totalList = _Source.Select(x => totalField + x[columnFields.Last()].ToString()).Distinct().OrderBy(m => m).ToList();
            colList.InsertRange(0, totalList);
        }

        dt.Columns.Add(rowField);
        colList.ForEach(x => dt.Columns.Add(x));

        foreach (string rowName in rowList)
        {
            DataRow row = dt.NewRow();
            row[rowField] = rowName;
            foreach (string colName in colList)
            {
                string filter = rowField + " = '" + rowName + "'";
                string[] colValues = colName.Split(Separator.ToCharArray(), StringSplitOptions.None);
                for (int i = 0; i < columnFields.Length; i++)
                    if (!colValues[i].Contains("(Total)"))
                        filter += " and " + columnFields[i] + " = '" + colValues[i] + "'";
                row[colName] = SourceTable.Computing(filter, dataField, colName.Contains("(Total)") ? AggregateFunctionE.Sum : aggregate);
            }
            dt.Rows.Add(row);
        }
        return dt;
    }

    public static DataTable PivotData(this DataTable _SourceTable, string DataField, AggregateFunctionE Aggregate, string[] RowFields, string[] ColumnFields, Boolean isFindSummary = false)
    {
        DataTable dt = new DataTable();
        string Separator = ".";
        var RowList = _SourceTable.DefaultView.ToTable(true, RowFields).AsEnumerable().ToList();
        for (int index = RowFields.Count() - 1; index >= 0; index--)
            RowList = RowList.OrderBy(x => x.Field<object>(RowFields[index])).ToList();
        // Gets the list of columns .(dot) separated.

        //dt.Columns.Add(RowFields);
        foreach (string s in RowFields)
        {
            //Get dataType of the column

            if (_SourceTable.Columns[s].DataType.Name.ToLower().IndexOf("datetime") >= 0 || _SourceTable.Columns[s].DataType.Name.ToLower().IndexOf("date") >= 0)
            {
                dt.Columns.Add(s, typeof(System.DateTime));
            }
            else if (_SourceTable.Columns[s].DataType.Name.ToLower().IndexOf("int") >= 0 || _SourceTable.Columns[s].DataType.Name.ToLower().IndexOf("double") >= 0 || _SourceTable.Columns[s].DataType.Name.ToLower().IndexOf("float") >= 0 || _SourceTable.Columns[s].DataType.Name.ToLower().IndexOf("decimal") >= 0)
            {
                dt.Columns.Add(s, typeof(double));
            }
            else
            {
                dt.Columns.Add(s);
            }
        }


        if (ColumnFields != null)
        {
            var ColList = (from x in _SourceTable.AsEnumerable()
                           select new
                           {
                               Name = ColumnFields.Select(n => x.Field<object>(n))
                                   .Aggregate((a, b) => a += Separator + b.ToString())
                           })
                                  .Distinct();
            //.OrderBy(m => m.Name);
            foreach (var col in ColList)
            {
                if (col.Name != null)
                {
                    dt.Columns.Add(col.Name.ToString(), typeof(double));  // Cretes the result columns.//
                }
            }



            foreach (var RowName in RowList)
            {
                DataRow row = dt.NewRow();
                string strFilter = string.Empty;

                foreach (string Field in RowFields)
                {
                    row[Field] = RowName[Field];
                    strFilter += " and " + Field + " = '" + RowName[Field].ToString() + "'";
                }
                strFilter = strFilter.Substring(5);

                foreach (var col in ColList)
                {
                    string filter = strFilter;
                    if (col.Name != null)
                    {
                        string[] strColValues = col.Name.ToString().Split(Separator.ToCharArray(), StringSplitOptions.None);
                        for (int i = 0; i < ColumnFields.Length; i++)
                            if (strColValues[i] != null) { filter += " and " + ColumnFields[i] + " = '" + strColValues[i] + "'"; }
                        object val = _SourceTable.Computing(filter, DataField, Aggregate);
                        
                        if (val == null)
                        {
                            val = 0;
                        }
                        row[col.Name.ToString()] = val;
                    }
                }
                dt.Rows.Add(row);
            }
        }
        else
        {
            dt.Columns.Add(DataField, typeof(double));

            foreach (var RowName in RowList)
            {
                DataRow row = dt.NewRow();
                string strFilter = string.Empty;

                foreach (string Field in RowFields)
                {
                    row[Field] = RowName[Field];
                    strFilter += " and " + Field + " = '" + RowName[Field].ToString() + "'";
                }
                strFilter = strFilter.Substring(5);

                row[DataField] = _SourceTable.Computing(strFilter, DataField, Aggregate);
               
                dt.Rows.Add(row);
            }

            if (isFindSummary)
            {
                for (int index = 0; index < RowFields.Count() - 1; index++)
                {
                    DataTable _temp = new DataTable();
                    _temp = dt.Clone();

                    List<string> rFields = new List<string>();
                    DataTable _dt = new DataTable();
                    List<string> _disRow = new List<string>();


                    for (int rix = 0; rix < RowFields.Count() - 1; rix++)
                    {
                        if (index >= rix)
                        {
                            {
                                rFields.Add(RowFields[rix]);

                            }
                        }
                    }


                    foreach (string i in rFields)
                    {
                        _disRow.Add(i);

                    }
                    _dt = _SourceTable.DefaultView.ToTable(true, _disRow.ToArray());



                    foreach (DataRow _dr in _dt.Rows)
                    {
                        DataRow dr = _temp.NewRow();
                        string strFilter = "";
                        foreach (string fn in _disRow)
                        {
                            dr[fn] = _dr[fn];
                            strFilter += " and " + fn + " = '" + _dr[fn].ToString() + "'";
                        }
                        strFilter = strFilter.Substring(5);
                        dr[DataField] = dt.Computing(strFilter, DataField, Aggregate);
                        _temp.Rows.Add(dr);
                    }
                    dt.Merge(_temp);
                }
            }



        }


        for (int i = 0; i < dt.Columns.Count; i++)
        {
            dt.Columns[i].ColumnName = dt.Columns[i].ColumnName.Replace(" ", "_");
        }







        return dt;
    }

    /// <summary>
    /// Retrives the data for matching RowField value and ColumnFields values with Aggregate function applied on them.
    /// </summary>
    /// <param name="Filter">DataTable Filter condition as a string</param>
    /// <param name="DataField">The column name which needs to spread out in Data Part of the Pivoted table</param>
    /// <param name="Aggregate">Enumeration to determine which function to apply to aggregate the data</param>
    /// <returns></returns>
    public static object Computing(this DataTable dt, string Filter, string DataField, AggregateFunctionE Aggregate)
    {
        try
        {
           

            //    var rows = dt.AsEnumerable().ToList().AsQueryable().Where(Filter);


            DataRow[] FilteredRows = dt.Select(Filter);
            object[] objList = FilteredRows.Select(x => x.Field<object>(DataField)).ToArray();

            switch (Aggregate)
            {
                case AggregateFunctionE.Average:
                    return GetAverage(objList);
                case AggregateFunctionE.Count:
                    return objList.Count();
                case AggregateFunctionE.Exists:
                    return (objList.Count() == 0) ? "False" : "True";
                case AggregateFunctionE.First:
                    return GetFirst(objList);
                case AggregateFunctionE.Last:
                    return GetLast(objList);
                case AggregateFunctionE.Max:
                    return GetMax(objList);
                case AggregateFunctionE.Min:
                    return GetMin(objList);
                case AggregateFunctionE.Sum:
                    return GetSum(objList);
                case AggregateFunctionE.NONE:
                    return (objList.Count() == 0) ? null : objList[0];
                default:
                    return null;
            }
        }
        catch (Exception ex)
        {
            return "#Error";
        }
    }

    public static object GetAverage(this object[] objList)
    {
        return objList.Count() == 0 ? null : (object)(Convert.ToDouble(GetSum(objList)) / objList.Count());
    }
    public static object GetSum(this object[] objList)
    {
        return objList.Count() == 0 ? null : (object)(objList.Aggregate(new double(), (x, y) => x += (Convert.ToDouble(y == "" ? 0 : y))));
    }
    public static object GetFirst(this object[] objList)
    {
        return (objList.Count() == 0) ? null : objList.First();
    }
    public static object GetLast(this object[] objList)
    {
        return (objList.Count() == 0) ? null : objList.Last();
    }
    public static object GetMax(this object[] objList)
    {
        return (objList.Count() == 0) ? null : objList.Max();
    }
    public static object GetMin(this object[] objList)
    {
        return (objList.Count() == 0) ? null : objList.Min();
    }

    public static DataTable DuplicateColumn(this DataTable dtResult, string ColumnName, string DuplicateColumnName)
    {
        DataColumn dc, newdc;
        dc = dtResult.Columns[ColumnName];
        newdc = new DataColumn();
        newdc.ColumnName = DuplicateColumnName;
        newdc.DataType = dc.DataType;
        dtResult.Columns.Add(newdc);
        foreach (DataRow dr in dtResult.Rows)
        {
            dr[DuplicateColumnName] = dr[ColumnName];
        }
        return dtResult;
    }

    public static DataTable Split(this DataTable dtResult, string ColumnName, string Splitter, string ColPrefix)
    {
        DataTable newdtResult = new DataTable();
        newdtResult.Columns.Add(new DataColumn("auto", typeof(Int64)));
        newdtResult.Columns["auto"].AutoIncrement = true;
        newdtResult.Columns["auto"].AutoIncrementSeed = 1;
        newdtResult.Columns["auto"].AutoIncrementStep = 1;
        DataTableReader dtr = new DataTableReader(dtResult);
        newdtResult.Load(dtr);
        dtResult.Clear();
        dtResult.Columns.Clear();
        char[] myChar = Splitter.ToCharArray();

        var result = from s in newdtResult.AsEnumerable()
                     select new Col
                     {
                         ColsName = s[ColumnName].ToString().Split(myChar).ToArray(),
                         ColKey = Convert.ToInt64(s["auto"]),
                         MaxLength = s[ColumnName].ToString().Split(myChar).ToArray().Length,
                     };

        var maxObject = result.OrderByDescending(item => item.MaxLength).First();


        List<Col> toUpCol = new List<Col>();
        toUpCol = result.ToList();

        var joinTable = from t1 in toUpCol
                        join t2 in newdtResult.AsEnumerable()
                            on t1.ColKey equals t2["auto"]
                        select new { t2, t1 };

        foreach (DataColumn col in newdtResult.Columns)
            dtResult.Columns.Add(col.ColumnName, col.DataType);

        for (int i = 0; i < maxObject.MaxLength; i++)
        {
            dtResult.Columns.Add(ColPrefix + "_" + (i + 1), typeof(string));
        }


        foreach (var row in joinTable)
        {
            var newRow = dtResult.NewRow();
            var obj = row.t2.ItemArray.ToArray();
            var obj1 = row.t1.ColsName.ToArray();
            var z = new object[obj.Length + obj1.Length];
            obj.CopyTo(z, 0);
            obj1.CopyTo(z, obj.Length);
            newRow.ItemArray = z;
            try
            {
                dtResult.Rows.Add(newRow);
            }
            catch { }
        }
        dtResult.Columns.Remove("auto");
        return dtResult;
    }

    public static DataTable FindReplace(this DataTable dtResult, string ColumnName, List<KeyValuePair<string, string>> pfindReplace, string NewColumnName)
    {
        if (dtResult.Rows.Count == 0) return dtResult;

        DataSet ds = new DataSet();
        //  DataTable dt = new DataTable();
        DataTable newdtResult = new DataTable();
        //dt.Columns.Add(new DataColumn("auto1", typeof(Int64)));
        //dt.Columns["auto1"].AutoIncrement = true;
        //dt.Columns["auto1"].AutoIncrementSeed = 1;
        //dt.Columns["auto1"].AutoIncrementStep = 1;

        newdtResult.Columns.Add(new DataColumn("auto", typeof(Int64)));
        newdtResult.Columns["auto"].AutoIncrement = true;
        newdtResult.Columns["auto"].AutoIncrementSeed = 1;
        newdtResult.Columns["auto"].AutoIncrementStep = 1;

        DataTableReader dtr = new DataTableReader(dtResult);
        newdtResult.Load(dtr);

        //dtr = new DataTableReader(dtResult.DefaultView.ToTable(false,ColumnName ) );
        //dt.Load(dtr);


        //ds.Tables.Add(dt);

        dtResult.Clear();
        dtResult.Columns.Clear();

        //string strNewXmls = "";           
        //foreach (var element in pfindReplace)
        //{
        //    strNewXmls = ds.GetXml().Replace(element.Key, element.Value);

        //}          
        //System.IO.StringReader srXmlStringReader = new System.IO.StringReader(strNewXmls);
        ////ds.Tables[0].Rows.Clear(); 
        //ds = new DataSet();
        //ds.ReadXml(srXmlStringReader  );

        //dt = ds.Tables[0];
        //dt.Columns[ColumnName].ColumnName = NewColumnName;


        var result = from s in newdtResult.AsEnumerable()
                     select new Col
                     {
                         ColName = s[ColumnName].ToString().toReplace(pfindReplace),
                         ColKey = Convert.ToInt64(s["auto"])
                     };

        List<Col> toUpCol = new List<Col>();
        toUpCol = result.ToList();

        //  var dtFinal = new DataTable();
        var joinTable = from t1 in newdtResult.AsEnumerable()
                        join t2 in toUpCol.AsEnumerable()
                            on Convert.ToInt64(t1["auto"]) equals t2.ColKey
                        select new { t1, t2 };

        foreach (DataColumn col in newdtResult.Columns)
            dtResult.Columns.Add(col.ColumnName, col.DataType);

        dtResult.Columns.Add(NewColumnName, typeof(string));






        foreach (var row in joinTable)
        {
            var newRow = dtResult.NewRow();
            var obj = row.t1.ItemArray.ToArray();
            var obj1 = row.t2.ColName;
            Array.Resize(ref obj, obj.Length + 1);
            obj[obj.Length - 1] = obj1;
            newRow.ItemArray = obj;





            dtResult.Rows.Add(newRow);
        }
        dtResult.Columns.Remove("auto");

        return dtResult;
    }

    public static DataTable DateParse(this DataTable dtResult, string ColumnName, ParseType DateParseType)
    {

        DataTable newdtResult = new DataTable();
        List<Col> toUpCol = new List<Col>();
        newdtResult.Columns.Add(new DataColumn("auto", typeof(Int64)));
        newdtResult.Columns["auto"].AutoIncrement = true;
        newdtResult.Columns["auto"].AutoIncrementSeed = 1;
        newdtResult.Columns["auto"].AutoIncrementStep = 1;

        DataTableReader dtr = new DataTableReader(dtResult);
        newdtResult.Load(dtr);
        dtResult.Clear();
        dtResult.Columns.Clear();
        // var result=new object();
        if (DateParseType == ParseType._DATE)
        {
            var result = from s in newdtResult.AsEnumerable()
                         select new Col
                         {
                             ColName = s[ColumnName] is DBNull ? "" : Convert.ToDateTime(s[ColumnName]).Date.ToShortDateString(),
                             ColKey = Convert.ToInt64(s["auto"])
                         };
            toUpCol = result.ToList();
        }
        else if (DateParseType == ParseType._DAY)
        {
            var result = from s in newdtResult.AsEnumerable()
                         select new Col
                         {
                             ColName = s[ColumnName] is DBNull ? "" : Convert.ToDateTime(s[ColumnName]).Day.ToString(),
                             ColKey = Convert.ToInt64(s["auto"])
                         };
            toUpCol = result.ToList();
        }
        else if (DateParseType == ParseType._YEAR)
        {
            var result = from s in newdtResult.AsEnumerable()
                         select new Col
                         {
                             ColName = s[ColumnName] is DBNull ? "" : Convert.ToDateTime(s[ColumnName]).Year.ToString(),
                             ColKey = Convert.ToInt64(s["auto"])
                         };
            toUpCol = result.ToList();
        }
        else if (DateParseType == ParseType._MONTH)
        {
            var result = from s in newdtResult.AsEnumerable()
                         select new Col
                         {
                             ColName = s[ColumnName] is DBNull ? "" : Convert.ToDateTime(s[ColumnName]).Month.ToString(),
                             ColKey = Convert.ToInt64(s["auto"])
                         };
            toUpCol = result.ToList();
        }
        else if (DateParseType == ParseType._WEEK)
        {
            CultureInfo cul = CultureInfo.CurrentCulture;
            var result = from s in newdtResult.AsEnumerable()
                         select new Col
                         {
                             ColName = s[ColumnName] is DBNull ? "" : cul.Calendar.GetWeekOfYear(Convert.ToDateTime(s[ColumnName]), CalendarWeekRule.FirstDay, DayOfWeek.Monday).ToString(),
                             ColKey = Convert.ToInt64(s["auto"])
                         };
            toUpCol = result.ToList();
        }
        else if (DateParseType == ParseType._QUARTER)
        {
            CultureInfo cul = CultureInfo.CurrentCulture;
            var result = from s in newdtResult.AsEnumerable()
                         select new Col
                         {
                             ColName = s[ColumnName] is DBNull ? "" : Math.Ceiling(Convert.ToDateTime(s[ColumnName]).Month / 3m).ToString(),
                             ColKey = Convert.ToInt64(s["auto"])
                         };
            toUpCol = result.ToList();
        }
        else if (DateParseType == ParseType._MMMDDYYYY)
        {
            CultureInfo cul = CultureInfo.CurrentCulture;
            var result = from s in newdtResult.AsEnumerable()
                         select new Col
                         {
                             ColName = s[ColumnName] is DBNull ? "" : Convert.ToDateTime(s[ColumnName]).ToString("MMMM dd, yyyy"),
                             ColKey = Convert.ToInt64(s["auto"])
                         };
            toUpCol = result.ToList();
        }
        else if (DateParseType == ParseType._DDMMYYYY)
        {
            CultureInfo cul = CultureInfo.CurrentCulture;
            var result = from s in newdtResult.AsEnumerable()
                         select new Col
                         {
                             ColName = s[ColumnName] is DBNull ? "" : Convert.ToDateTime(s[ColumnName]).ToString("dd/MM/yyyy"),
                             ColKey = Convert.ToInt64(s["auto"])
                         };
            toUpCol = result.ToList();
        }
        else if (DateParseType == ParseType._MMDDYYYY)
        {
            CultureInfo cul = CultureInfo.CurrentCulture;
            var result = from s in newdtResult.AsEnumerable()
                         select new Col
                         {
                             ColName = s[ColumnName] is DBNull ? "" : Convert.ToDateTime(s[ColumnName]).ToString("MM/dd/yyyy"),
                             ColKey = Convert.ToInt64(s["auto"])
                         };
            toUpCol = result.ToList();
        }
        else if (DateParseType == ParseType._DDMMMYYYY)
        {
            CultureInfo cul = CultureInfo.CurrentCulture;
            var result = from s in newdtResult.AsEnumerable()
                         select new Col
                         {
                             ColName = s[ColumnName] is DBNull ? "" : Convert.ToDateTime(s[ColumnName]).ToString("dd MMMM, yyyy"),
                             ColKey = Convert.ToInt64(s["auto"])
                         };
            toUpCol = result.ToList();
        }
        else if (DateParseType == ParseType._HOUR)
        {
            CultureInfo cul = CultureInfo.CurrentCulture;
            var result = from s in newdtResult.AsEnumerable()
                         select new Col
                         {
                             ColName = s[ColumnName] is DBNull ? "" : Convert.ToDateTime(s[ColumnName]).Hour.ToString(),
                             ColKey = Convert.ToInt64(s["auto"])
                         };
            toUpCol = result.ToList();
        }
        else if (DateParseType == ParseType._MIN)
        {
            CultureInfo cul = CultureInfo.CurrentCulture;
            var result = from s in newdtResult.AsEnumerable()
                         select new Col
                         {
                             ColName = s[ColumnName] is DBNull ? "" : Convert.ToDateTime(s[ColumnName]).Minute.ToString(),
                             ColKey = Convert.ToInt64(s["auto"])
                         };
            toUpCol = result.ToList();
        }
        else if (DateParseType == ParseType._SEC)
        {
            CultureInfo cul = CultureInfo.CurrentCulture;
            var result = from s in newdtResult.AsEnumerable()
                         select new Col
                         {
                             ColName = s[ColumnName] is DBNull ? "" : Convert.ToDateTime(s[ColumnName]).Second.ToString(),
                             ColKey = Convert.ToInt64(s["auto"])
                         };
            toUpCol = result.ToList();
        }

        newdtResult.Columns.Remove(ColumnName);

        var joinTable = from t1 in toUpCol
                        join t2 in newdtResult.AsEnumerable()
                            on t1.ColKey equals t2["auto"]
                        select new { t2, t1 };

        foreach (DataColumn col in newdtResult.Columns)
            dtResult.Columns.Add(col.ColumnName, col.DataType);
        dtResult.Columns.Add(ColumnName, typeof(string));
        foreach (var row in joinTable)
        {
            var newRow = dtResult.NewRow();
            var obj = row.t2.ItemArray.ToArray();
            var obj1 = row.t1.ColName;
            Array.Resize(ref obj, obj.Length + 1);
            obj[obj.Length - 1] = obj1;
            newRow.ItemArray = obj;
            dtResult.Rows.Add(newRow);
        }
        dtResult.Columns.Remove("auto");
        return dtResult;

    }
    
    public static DataTable UpperCase(this DataTable dtResult, string ColumnName)
    {
        DataTable newdtResult = new DataTable();
        newdtResult.Columns.Add(new DataColumn("auto", typeof(Int64)));
        newdtResult.Columns["auto"].AutoIncrement = true;
        newdtResult.Columns["auto"].AutoIncrementSeed = 1;
        newdtResult.Columns["auto"].AutoIncrementStep = 1;

        DataTableReader dtr = new DataTableReader(dtResult);
        newdtResult.Load(dtr);
        dtResult.Clear();
        dtResult.Columns.Clear();

        var result = from s in newdtResult.AsEnumerable()
                     select new Col
                     {
                         ColName = s[ColumnName].ToString().ToUpper(),
                         ColKey = Convert.ToInt64(s["auto"])
                     };
        List<Col> toUpCol = new List<Col>();
        toUpCol = result.ToList();
        newdtResult.Columns.Remove(ColumnName);

        var joinTable = from t1 in toUpCol
                        join t2 in newdtResult.AsEnumerable()
                            on t1.ColKey equals t2["auto"]
                        select new { t2, t1 };

        foreach (DataColumn col in newdtResult.Columns)
            dtResult.Columns.Add(col.ColumnName, col.DataType);
        dtResult.Columns.Add(ColumnName, typeof(string));
        foreach (var row in joinTable)
        {
            var newRow = dtResult.NewRow();
            var obj = row.t2.ItemArray.ToArray();
            var obj1 = row.t1.ColName;
            Array.Resize(ref obj, obj.Length + 1);
            obj[obj.Length - 1] = obj1;
            newRow.ItemArray = obj;
            dtResult.Rows.Add(newRow);
        }
        dtResult.Columns.Remove("auto");
        return dtResult;
    }

    public static DataTable LowerCase(this DataTable dtResult, string ColumnName)
    {
        DataTable newdtResult = new DataTable();
        newdtResult.Columns.Add(new DataColumn("auto", typeof(Int64)));
        newdtResult.Columns["auto"].AutoIncrement = true;
        newdtResult.Columns["auto"].AutoIncrementSeed = 1;
        newdtResult.Columns["auto"].AutoIncrementStep = 1;
        DataTableReader dtr = new DataTableReader(dtResult);
        newdtResult.Load(dtr);
        dtResult.Clear();
        dtResult.Columns.Clear();
        var result = from s in newdtResult.AsEnumerable()
                     select new Col
                     {
                         ColName = s[ColumnName].ToString().ToLower(),
                         ColKey = Convert.ToInt64(s["auto"])
                     };
        List<Col> toUpCol = new List<Col>();
        toUpCol = result.ToList();
        newdtResult.Columns.Remove(ColumnName);
        var joinTable = from t1 in toUpCol
                        join t2 in newdtResult.AsEnumerable()
                            on t1.ColKey equals t2["auto"]
                        select new { t2, t1 };

        foreach (DataColumn col in newdtResult.Columns)
            dtResult.Columns.Add(col.ColumnName, col.DataType);
        dtResult.Columns.Add(ColumnName, typeof(string));
        foreach (var row in joinTable)
        {
            var newRow = dtResult.NewRow();
            var obj = row.t2.ItemArray.ToArray();
            var obj1 = row.t1.ColName;
            Array.Resize(ref obj, obj.Length + 1);
            obj[obj.Length - 1] = obj1;
            newRow.ItemArray = obj;
            dtResult.Rows.Add(newRow);
        }
        dtResult.Columns.Remove("auto");
        return dtResult;
    }

    public static DataTable TitleCase(this DataTable dtResult, string ColumnName)
    {
        DataTable newdtResult = new DataTable();
        newdtResult.Columns.Add(new DataColumn("auto", typeof(Int64)));
        newdtResult.Columns["auto"].AutoIncrement = true;
        newdtResult.Columns["auto"].AutoIncrementSeed = 1;
        newdtResult.Columns["auto"].AutoIncrementStep = 1;
        DataTableReader dtr = new DataTableReader(dtResult);
        newdtResult.Load(dtr);
        dtResult.Clear();
        dtResult.Columns.Clear();
        var result = from s in newdtResult.AsEnumerable()
                     select new Col
                     {
                         ColName = s[ColumnName].ToString().toTitleCase(titleCase.All),
                         ColKey = Convert.ToInt64(s["auto"])
                     };
        List<Col> toUpCol = new List<Col>();
        toUpCol = result.ToList();
        newdtResult.Columns.Remove(ColumnName);
        var joinTable = from t1 in toUpCol
                        join t2 in newdtResult.AsEnumerable()
                            on t1.ColKey equals t2["auto"]
                        select new { t2, t1 };

        foreach (DataColumn col in newdtResult.Columns)
            dtResult.Columns.Add(col.ColumnName, col.DataType);
        dtResult.Columns.Add(ColumnName, typeof(string));
        foreach (var row in joinTable)
        {
            var newRow = dtResult.NewRow();
            var obj = row.t2.ItemArray.ToArray();
            var obj1 = row.t1.ColName;
            Array.Resize(ref obj, obj.Length + 1);
            obj[obj.Length - 1] = obj1;
            newRow.ItemArray = obj;
            try
            {
                dtResult.Rows.Add(newRow);
            }
            catch { }

        }
        dtResult.Columns.Remove("auto");
        return dtResult;
    }

    public static DataTable CapitalCase(this DataTable dtResult, string ColumnName)
    {
        DataTable newdtResult = new DataTable();
        newdtResult.Columns.Add(new DataColumn("auto", typeof(Int64)));
        newdtResult.Columns["auto"].AutoIncrement = true;
        newdtResult.Columns["auto"].AutoIncrementSeed = 1;
        newdtResult.Columns["auto"].AutoIncrementStep = 1;
        DataTableReader dtr = new DataTableReader(dtResult);
        newdtResult.Load(dtr);
        dtResult.Clear();
        dtResult.Columns.Clear();
        var result = from s in newdtResult.AsEnumerable()
                     select new Col
                     {
                         ColName = s[ColumnName].ToString().toTitleCase(titleCase.First),
                         ColKey = Convert.ToInt64(s["auto"])
                     };
        List<Col> toUpCol = new List<Col>();
        toUpCol = result.ToList();
        newdtResult.Columns.Remove(ColumnName);
        var joinTable = from t1 in toUpCol
                        join t2 in newdtResult.AsEnumerable()
                            on t1.ColKey equals t2["auto"]
                        select new { t2, t1 };

        foreach (DataColumn col in newdtResult.Columns)
            dtResult.Columns.Add(col.ColumnName, col.DataType);
        dtResult.Columns.Add(ColumnName, typeof(string));
        foreach (var row in joinTable)
        {
            var newRow = dtResult.NewRow();
            var obj = row.t2.ItemArray.ToArray();
            var obj1 = row.t1.ColName;
            Array.Resize(ref obj, obj.Length + 1);
            obj[obj.Length - 1] = obj1;
            newRow.ItemArray = obj;
            //   dtResult.Rows.Add(newRow);
        }
        dtResult.Columns.Remove("auto");
        return dtResult;
    }

    public static DataTable Trancate(this DataTable dtResult, string ColumnName, int TrancateIndex)
    {
        DataTable newdtResult = new DataTable();
        newdtResult.Columns.Add(new DataColumn("auto", typeof(Int64)));
        newdtResult.Columns["auto"].AutoIncrement = true;
        newdtResult.Columns["auto"].AutoIncrementSeed = 1;
        newdtResult.Columns["auto"].AutoIncrementStep = 1;
        DataTableReader dtr = new DataTableReader(dtResult);
        newdtResult.Load(dtr);
        dtResult.Clear();
        dtResult.Columns.Clear();
        var result = from s in newdtResult.AsEnumerable()
                     select new Col
                     {
                         ColName = s[ColumnName].ToString().Substring(0, s[ColumnName].ToString().Length < TrancateIndex ? s[ColumnName].ToString().Length : TrancateIndex),
                         ColKey = Convert.ToInt64(s["auto"])
                     };
        List<Col> toUpCol = new List<Col>();
        toUpCol = result.ToList();
        newdtResult.Columns.Remove(ColumnName);
        var joinTable = from t1 in toUpCol
                        join t2 in newdtResult.AsEnumerable()
                            on t1.ColKey equals t2["auto"]
                        select new { t2, t1 };

        foreach (DataColumn col in newdtResult.Columns)
            dtResult.Columns.Add(col.ColumnName, col.DataType);
        dtResult.Columns.Add(ColumnName, typeof(string));
        foreach (var row in joinTable)
        {
            var newRow = dtResult.NewRow();
            var obj = row.t2.ItemArray.ToArray();
            var obj1 = row.t1.ColName;
            Array.Resize(ref obj, obj.Length + 1);
            obj[obj.Length - 1] = obj1;
            newRow.ItemArray = obj;
            //      dtResult.Rows.Add(newRow);
        }
        dtResult.Columns.Remove("auto");
        return dtResult;
    }

    public static string toTitleCase(this string str, titleCase tcase = titleCase.First)
    {
        CultureInfo ci = new CultureInfo("en-US");

        str = str.ToLower();
        switch (tcase)
        {
            case titleCase.First:
                var strArray = str.Split(' ');
                if (strArray.Length > 1)
                {
                    strArray[0] = ci.TextInfo.ToTitleCase(strArray[0]);
                    return string.Join(" ", strArray);
                }
                break;
            case titleCase.All:
                return ci.TextInfo.ToTitleCase(str);
            default:
                break;
        }
        return ci.TextInfo.ToTitleCase(str);
    }
    
    private class Col
    {
        public string ColName = "";
        public Int64 ColKey = 0;
        public string[] ColsName;
        public Int64 MaxLength;
    }

    private static string toReplace(this string str, List<KeyValuePair<string, string>> findreplace)
    {
        foreach (var element in findreplace)
        {
            str = str.Replace(element.Key, element.Value);
        }
        return str;
    }

    public enum titleCase
    {
        First,
        All
    }

    public enum ParseType
    {
        _NONE,
        _DATE,
        _HOUR,
        _MIN,
        _SEC,
        _DAY,
        _MONTH,
        _QUARTER,
        _YEAR,
        _WEEK,
        _DDMMYYYY,
        _MMDDYYYY,
        _MMMDDYYYY,
        _DDMMMYYYY,
    }

    public static bool IsNumeric(this string text)
    {
        double test;
        return double.TryParse(text, out test);
    }

    public static bool IsDate(this string text) {
        try
        {
            DateTime dt = DateTime.Parse(text);
            if (dt != DateTime.MinValue && dt != DateTime.MaxValue)
                return true;
            return false;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsBool(this string value) {
        bool flag; 
        try
        {
          return  Boolean.TryParse(value, out flag);
        }
        catch {
            return false;
        }
    }

}