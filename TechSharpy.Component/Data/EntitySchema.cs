using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TechSharpy.Data;

namespace TechSharpy.Entitifier.Data
{
    public class EntitySchema:DataAccess

    {
        DataTable dtResult;
        public EntitySchema()
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

        public DataTable GetEntityList(int ClientID)
        {
            dtResult = new DataTable();
            //dtResult.Pivot("","");
            Query selectQ = new Query(QueryType._Select).AddTable("s_entity").AddField("*", "s_entity").
                AddWhere(0, "s_entity", "ClientID", FieldType._Number, Operator._Equal, ClientID.ToString());
            dtResult = this.GetData(selectQ);
            return dtResult;
        }

        public DataTable GetEntity(int ClientID, Int64 EntityID)
        {
            dtResult = new DataTable();
            Query selectQ = new Query(QueryType._Select).AddTable("s_entity").AddField("*", "s_entity").
                AddWhere(0, "s_entity", "ClientID", FieldType._Number, Operator._Equal, ClientID.ToString()).
                AddWhere(0, "s_entity", "EntityID", FieldType._Number, Operator._Equal, EntityID.ToString());
            dtResult = this.GetData(selectQ);
            return dtResult;
        }


        public Boolean Delete(int pClientID, int pEntityID)
        {
            Query DeleteQ = new Query(QueryType._Delete).AddTable("s_entity").AddWhere(0, "s_entity", "ClientID", FieldType._Number, Operator._Equal, pClientID.ToString()).
               AddWhere(0, "s_entity", "EntityID", FieldType._Number, Operator._Equal, pEntityID.ToString());
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

        public int Save(int pClientID, string pTableName, string Name, string pDescription, string keys, TechSharpy.Entitifier.EntityType pType)
        {
            int NextID = this.getNextID("Entity");
            Query iQuery = new Query(QueryType._Insert
                ).AddTable("s_entity")
                .AddField("EntityID", "s_entity", FieldType._Number, "", NextID.ToString())
                .AddField("Name", "s_entity", FieldType._String, "", Name.ToString())
                .AddField("Keys", "s_entity", FieldType._String, "", keys.ToString())
                .AddField("TableName", "s_entity", FieldType._String, "", pTableName.ToString())
                .AddField("Description", "s_entity", FieldType._String, "", pDescription.ToString())
                .AddField("Type", "s_entity", FieldType._Number, "", Convert.ToInt32(pType).ToString())
                .AddField("ClientID", "s_entity", FieldType._Number, "", pClientID.ToString())
          .AddField("LastUPD", "s_entity", FieldType._DateTime, "", DateTime.Now.ToString());
            if (this.ExecuteQuery(iQuery) > 0)
            {
                return NextID;
            }
            else
            {
                return -1;
            }
        }

        public bool Update(int pClientID, Int64 pEntityID, string pTableName, string Name, string pDescription, string keys,
           TechSharpy.Entitifier.EntityType pType)
        {
            Query iQuery = new Query(QueryType._Update
                ).AddTable("s_entity")
                .AddField("Description", "s_entity", FieldType._String, "", pDescription.ToString())
                .AddField("Name", "s_entity", FieldType._String, "", Name.ToString())
                .AddField("Keys", "s_entity", FieldType._String, "", keys.ToString())
                .AddField("TableName", "s_entity", FieldType._String, "", pTableName.ToString())
                .AddField("Description", "s_entity", FieldType._String, "", pDescription.ToString())
                .AddField("Type", "s_entity", FieldType._Number, "", Convert.ToInt32(pType).ToString())
            // .AddField("ClientID", "s_entity", FieldType._Number, "", pClientID.ToString())
            .AddField("lastUpD", "s_entity", FieldType._DateTime, "", DateTime.Now.ToString())
           .AddWhere(0, "s_entity", "ClientID", FieldType._Number, Operator._Equal, pClientID.ToString()).
               AddWhere(0, "s_entity", "EntityID", FieldType._Number, Operator._Equal, pEntityID.ToString());
            if (this.ExecuteQuery(iQuery) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckEntityExist(string EntityName)
        {
            dtResult = new DataTable();
            Query selectQ = new Query(QueryType._Select).AddTable("s_entity").AddField("*", "s_entity").
               //    AddWhere(0, "s_entity", "ClientID", FieldType._Number, Operator._Equal, ClientID.ToString()).
               AddWhere(0, "s_entity", "TableName", FieldType._String, Operator._Equal, EntityName.ToString());
            dtResult = this.GetData(selectQ);
            if (dtResult.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public DataTable GetEntityFields(int ClientID, Int64 EntityID)
        {
            dtResult = new DataTable();
            Query selectQ = new Query(QueryType._Select).AddTable("s_entityfields").AddField("*", "s_entityfields").
               AddWhere(0, "s_entityfields", "ClientID", FieldType._Number, Operator._Equal, ClientID.ToString()).
               AddWhere(0, "s_entityfields", "EntityID", FieldType._Number, Operator._Equal, EntityID.ToString());
            dtResult = this.GetData(selectQ);
            return dtResult;
        }

        public bool ExecuteNonQuery(TQuery tq)
        {
            try
            {
                if (this.ExecuteTQuery(tq))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to Create Entity", ex.InnerException);
            }

        }
    }
}
