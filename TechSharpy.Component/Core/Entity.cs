using System;
using System.Collections.Generic;

namespace TechSharpy.Entitifier
{
    public enum EntityType
    {
        _Master=1,
        _MasterAttribute=2,
        _RelatedMaster=3,
        _Transaction=4,        
        _Sudo=5,
    }
    public enum FieldType {
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
        public bool IsShow;
        public List<EntityInstance> EntityInstance;
        public EntitySchema(string name, EntityType entityType, int entityKey, string tableName, List<string> primaryKeys)
        {
            Name = name;
            EntityType = entityType;
            EntityKey = entityKey;
            TableName = tableName;
            PrimaryKeys = primaryKeys;
            IsShow = true;
        }

        public EntitySchema(int entityKey)
        {
            EntityKey = entityKey;
            PrimaryKeys = new List<string>();
            Init();
        }

        public EntitySchema(EntityType entityType)
        {
            EntityType = entityType;
            EntityKey = -1;
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
            InitField();
        }
        /// <summary>
        /// Load FieldInstance 
        /// </summary>
        private void InitField() {

        }
    }
}
