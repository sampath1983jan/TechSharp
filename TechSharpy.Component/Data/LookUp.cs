using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TechSharpy.Data;

namespace TechSharpy.Entitifier.Data
{
    class LookUp : DataAccess

    {
        DataTable dtResult;
        public LookUp()
        {
            try
            {
                this.Init();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetLookUpItems(int pClientID, int pLookUpID)
        {
            dtResult = new DataTable();
            Query selectQ = new Query(QueryType._Select).AddTable("vw_lookupitem").AddField("*", "vw_lookupitem").
                AddWhere(0, "vw_lookupitem", "ClientID", FieldType._Number, Operator._Equal, pClientID.ToString()).
                AddWhere(0, "vw_lookupitem", "LookUpID", FieldType._Number, Operator._Equal, pLookUpID.ToString());
            dtResult = this.GetData(selectQ);
            return dtResult;
        }

        public int Save(int pClientID, string pName, bool pIsCore,bool haveChild, Entitifier.LookUpType lookupType)
        {
            int NextID = this.getNextID("LookUp");
            Query iQuery = new Query(QueryType._Insert
                ).AddTable("s_entitylookup")
                .AddField("LookUpID", "s_entitylookup", FieldType._Number, "", NextID.ToString())
                .AddField("Name", "s_entitylookup", FieldType._String, "", pName.ToString())
                .AddField("IsCore", "s_entitylookup", FieldType._Question, "", pIsCore.ToString())
                .AddField("HaveChild", "s_entitylookup", FieldType._Question, "", pIsCore.ToString())
                .AddField("ClientID", "s_entitylookup", FieldType._Number, "", pClientID.ToString())
                .AddField("LookUpType", "s_entitylookup", FieldType._Number, "", lookupType.ToString())
             .AddField("LastUPD", "s_entitylookup", FieldType._DateTime, "", DateTime.Now.ToString());
            if (this.ExecuteQuery(iQuery) > 0)
            {
                return NextID;
            }
            else
            {
                return -1;
            }
        }

        public bool Save(int pClientID, int pLookUpId, string pName, bool pIsCore, bool haveChild, Entitifier.LookUpType lookupType)
        {
            Query iQuery = new Query(QueryType._Update
         ).AddTable("s_entitylookup")
         .AddField("LookUpName", "s_entitylookup", FieldType._String, "", pName.ToString())
         .AddField("IsCore", "s_entitylookup", FieldType._String, "", pIsCore.ToString())
         .AddField("HaveChild", "s_entitylookup", FieldType._String, "", haveChild.ToString())
         .AddField("LookUpType", "s_entitylookup", FieldType._Number, "", lookupType.ToString())
            .AddField("lastUpD", "s_entitylookup", FieldType._DateTime, "", DateTime.Now.ToString())
        .AddWhere(0, "s_entitylookup", "ClientID", FieldType._Number, Operator._Equal, pClientID.ToString()).
        AddWhere(0, "s_entitylookup", "LookUpId", FieldType._Number, Operator._Equal, pLookUpId.ToString());
            if (this.ExecuteQuery(iQuery) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool Delete(int pClientID, int pLookUpID)
        {
            Query DeleteQ = new Query(QueryType._Delete).AddTable("s_entitylookup")
                .AddWhere(0, "s_entitylookup", "ClientID", FieldType._Number, Operator._Equal, pClientID.ToString()).
      AddWhere(0, "s_entitylookup", "LookUpID", FieldType._Number, Operator._Equal, pLookUpID.ToString());
            int iResult;
            iResult = this.ExecuteQuery(DeleteQ);
            if (iResult >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeleteLookUpItems(int pClientID, int pLookUpID)
        {
            Query DeleteQ = new Query(QueryType._Delete).AddTable("s_entitylookupitem")
                .AddWhere(0, "s_entitylookupitem", "ClientID", FieldType._Number, Operator._Equal, pClientID.ToString()).
      AddWhere(0, "s_entitylookupitem", "LookUpID", FieldType._Number, Operator._Equal, pLookUpID.ToString());
            int iResult;
            iResult = this.ExecuteQuery(DeleteQ);
            if (iResult >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool DeleteLookUpItem(int pClientID, int pLookUpID, int pLookUpItemID)
        {
            Query DeleteQ = new Query(QueryType._Delete).AddTable("s_entitylookupitem")
                .AddWhere(0, "s_entitylookupitem", "ClientID", FieldType._Number, Operator._Equal, pClientID.ToString())
                 .AddWhere(0, "s_entitylookupitem", "LookUpItemID", FieldType._Number, Operator._Equal, pLookUpItemID.ToString()).
      AddWhere(0, "s_entitylookupitem", "LookUpID", FieldType._Number, Operator._Equal, pLookUpID.ToString());
            int iResult;
            iResult = this.ExecuteQuery(DeleteQ);
            if (iResult >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int SaveItem(int pClientID, int pLookUpID, string pName, string pShortName, int order,int parentLookUpID)
        {
            int NextID = this.getNextID("LookUpItem");
            Query iQuery = new Query(QueryType._Insert
                ).AddTable("s_entitylookupitem")
                .AddField("LookUpID", "s_entitylookupitem", FieldType._Number, "", pLookUpID.ToString())
                   .AddField("LookUpInstanceID", "s_entitylookupitem", FieldType._Number, "", NextID.ToString())
                .AddField("Name", "s_entitylookupitem", FieldType._String, "", pName.ToString())
                .AddField("ShortName", "s_entitylookupitem", FieldType._String, "", pShortName.ToString())
                .AddField("order", "s_entitylookupitem", FieldType._Number, "", order.ToString())
                .AddField("parentLookUpID", "s_entitylookupitem", FieldType._Number, "", parentLookUpID.ToString())
                .AddField("ClientID", "s_entitylookupitem", FieldType._Number, "", pClientID.ToString())
             .AddField("LastUPD", "s_entitylookupitem", FieldType._DateTime, "", DateTime.Now.ToString());
            if (this.ExecuteQuery(iQuery) > 0)
            {
                return NextID;
            }
            else
            {
                return -1;
            }
        }

        public bool SaveItem(int pClientID, int pLookupinstanceid, int pLookUpID, string pName, string pShortName,int order,int parentLookUpID)
        {
            Query iQuery = new Query(QueryType._Update
         ).AddTable("s_entitylookup")
         .AddField("LookUpName", "s_entitylookup", FieldType._String, "", pName.ToString())
         .AddField("ShortName", "s_entitylookup", FieldType._String, "", pShortName.ToString())
         //.AddField("IsEnabled", "s_entitylookup", FieldType._Question, "", IsEnabled.ToString())
     .AddField("lastUpD", "s_entitylookup", FieldType._DateTime, "", DateTime.Now.ToString())
      .AddField("order", "s_entitylookupitem", FieldType._Number, "", order.ToString())
                .AddField("parentLookUpID", "s_entitylookupitem", FieldType._Number, "", parentLookUpID.ToString())
    .AddWhere(0, "s_entitylookup", "ClientID", FieldType._Number, Operator._Equal, pClientID.ToString()).
        AddWhere(0, "s_entitylookup", "LookUpId", FieldType._Number, Operator._Equal, pLookUpID.ToString()).
         AddWhere(0, "s_entitylookup", "Lookupinstanceid", FieldType._Number, Operator._Equal, pLookupinstanceid.ToString());
            if (this.ExecuteQuery(iQuery) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
