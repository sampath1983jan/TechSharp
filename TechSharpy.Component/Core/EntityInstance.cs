using System;
using System.Collections.Generic;
using System.Text;

namespace TechSharpy.Entitifier
{
    public class EntityInstance
    {
        public string Name;
        public Int32 InstanceID;
        public EntityFieldType FieldType;
        public bool IsKey;
        public bool IsRequired;
        public bool IsUnique;
        public Int32 LookUpID;
        public bool IsCore;
        public Int32 EntityKey;
        public string Value;
        public bool IsReadOnly;
        public string DefaultValue;
        public int DisplayOrder;
        public List<string> LookUpArray;
        public string Min;
        public string Max;
        public int MaxLength;
        public string Note;
        public bool AutoIncrement;
        public Int64 Incrementfrom;
        public Int64 Incrementby;
        public bool IsShow;

        public EntityInstance(string name, int instanceID, EntityFieldType fieldType, bool isKey, bool isRequired, bool isUnique, int lookUpID, bool isCore, int entityKey, string value, bool isReadOnly, string defaultValue, int displayOrder, List<string> lookUpArray, string min, string max, int maxLength, string note, bool autoIncrement, long incrementfrom, long incrementby)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            InstanceID = instanceID;
            FieldType = fieldType;
            IsKey = isKey;
            IsRequired = isRequired;
            IsUnique = isUnique;
            LookUpID = lookUpID;
            IsCore = isCore;
            EntityKey = entityKey;
            Value = value ?? throw new ArgumentNullException(nameof(value));
            IsReadOnly = isReadOnly;
            DefaultValue = defaultValue ?? throw new ArgumentNullException(nameof(defaultValue));
            DisplayOrder = displayOrder;
            LookUpArray = lookUpArray ?? throw new ArgumentNullException(nameof(lookUpArray));
            Min = min ?? throw new ArgumentNullException(nameof(min));
            Max = max ?? throw new ArgumentNullException(nameof(max));
            MaxLength = maxLength;
            Note = note ?? throw new ArgumentNullException(nameof(note));
            AutoIncrement = autoIncrement;
            Incrementfrom = incrementfrom;
            Incrementby = incrementby;
            IsShow = true;
        }

        public EntityInstance(int pinstanceID)
        {
            InstanceID = pinstanceID;
            Name = "";
            InstanceID = -1;
            FieldType = EntityFieldType._Text;
            IsKey = false;
            IsRequired = false;
            IsUnique = false;
            LookUpID = -1;
            IsCore = false;
            EntityKey = -1;
            Value = "";
            IsReadOnly = false;
            DefaultValue = "";
            DisplayOrder = -1;
            LookUpArray =  new List<string>();
            Min = "0";
            Max = "0";
            MaxLength = 0;
            Note = "";
            AutoIncrement = false ;
            Incrementfrom = 0;
            Incrementby = 0;
        }

        public bool Save() {
            return true;
        }

        public bool Remove() {
            return true;
        }

        public bool Hide() {
            return true;
        }

    }
}
