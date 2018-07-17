using System;
using System.Collections.Generic;
using System.Data;
 
using System.Linq;

namespace TechSharpy.Entitifier
{
    public enum EntityType
    {
        _None=0,
        _Master=1,
        _MasterAttribute=2,
        _RelatedMaster=3,
        _Transaction=4,        
        _Sudo=5,
    }
    public enum EntityFieldType {
        _Number=1,
        _Float=2,
        _Text=3,
        _LongText=4,
        _Date=5,
        _DateTime=6,
        _Time=7,
        _Bool=8,
        _Entity=9,
        _Lookup=10,
        _MultiLookup=11,
        _Picture=12,
        _File=13,
        _Auto=14,
    }

    public class EntitySchema
    {
        public string Name;
        public TechSharpy.Entitifier.EntityType EntityType;
        public Int32 EntityKey;
        public string TableName;
        public List<string> PrimaryKeys;
        public string Description;
        public bool IsShow;
        public List<EntityInstance> EntityInstance;
        private Data.EntitySchema dataEntity;
        public EntitySchema() {
            dataEntity = new Data.EntitySchema();
        }
        public EntitySchema(string name,string description, EntityType entityType, int entityKey, string tableName, List<string> primaryKeys)
        {
            Description = description;
            Name = name;
            EntityType = entityType;
            EntityKey = entityKey;
            TableName = tableName;
            PrimaryKeys = primaryKeys;
            IsShow = true;
            dataEntity = new Data.EntitySchema();
        }

        public EntitySchema(int entityKey)
        {
            EntityKey = entityKey;
            PrimaryKeys = new List<string>();
            dataEntity = new Data.EntitySchema();
            Init();
        }

        public EntitySchema(EntityType entityType)
        {
            EntityType = entityType;
            EntityKey = -1;
            dataEntity = new Data.EntitySchema();
            PrimaryKeys = new List<string>();
        }
        /// <summary>
        /// Save Entity 
        /// </summary>
        /// <returns></returns>
        public bool Save() {
            return true;
        }
        /// <summary>
        /// remove Entity 
        /// </summary>
        /// <returns></returns>
        public bool Remove() {
            return true;
        }
        /// <summary>
        /// hide entityschema
        /// </summary>
        /// <returns></returns>
        public bool Hide() {
            return true;
        }
        /// <summary>
        /// Load EntitySchema
        /// </summary>
        public void Init() {
            DataTable dt = new DataTable();
            dt = dataEntity.GetEntity(-1, this.EntityKey);
            
            var e = dt.AsEnumerable().Select(g => new EntitySchema
            {
                EntityKey = g.IsNull("EntityID") ? 0 : g.Field<int>("EntityID"),
                Name = g.IsNull("Name") ? "" : g.Field<string>("Name"),
                TableName = g.IsNull("TableName") ? "" : g.Field<string>("TableName"),
                PrimaryKeys = g.IsNull("Keys") ? "" : g.Field<string>("Keys"),
                Description = g.IsNull("Description") ? "" : g.Field<string>("Description"),
                EntityType = g.IsNull("Type") ? EntityType._Master  : g.Field<EntityType>("Type")
            }).First();
            this.EntityKey = e.EntityID;
            this.Name = e.Name;
            this.TableName = e.TableName;
            this.EntityType = e.Type;
            this.Description = e.Description;
            this.PrimaryKeys = e.Keys;
            InitField();
        }
        /// <summary>
        /// Load FieldInstance 
        /// </summary>
        private void InitField() {

        }
    }
}
