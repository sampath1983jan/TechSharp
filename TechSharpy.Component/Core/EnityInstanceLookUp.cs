using System;
using System.Collections.Generic;
using System.Text;

namespace TechSharpy.Entitifier
{
    public enum LookUpType {
        _None=0,
        _Year=1,
        _Month=2,
        _Gender=3,
        _Nationality=4,
        _Currency=5,
        _Quarter=6,
        _YesNo=7,
        _Relationship=8,
        _Color=9,
        _BoodGroup=10,
        _Country=11,

    }
    
   public class EnityInstanceLookUp
    {
        public int LookUpID;
        public string Name;
        public bool IsCore;
        public bool HaveChild;
        public List<LookUpItem> LookUpItems;
        public LookUpType LookUpType;

        public EnityInstanceLookUp(int lookUpID, string name, bool isCore, bool haveChild, List<LookUpItem> lookUpItems, LookUpType lookUpType)
        {
            LookUpID = lookUpID;
            Name = name;
            IsCore = isCore;
            HaveChild = haveChild;
            LookUpItems = lookUpItems ?? throw new ArgumentNullException(nameof(lookUpItems));
            LookUpType = lookUpType;
        }

        public EnityInstanceLookUp(int lookUpID)
        {
            LookUpID = lookUpID;
            this.Name = "";
            this.IsCore = false;
            this.HaveChild = false;
            this.LookUpItems = new List<LookUpItem>();
            this.LookUpType = LookUpType._None;
        }
        public bool Save() {
            return true;
        }
        public bool Remove() {
            return true;
        }

        

         
    }

    public class LookUpItem {
        public int ItemID;
        public int LookUpID;
        public string Value;
        public string ShortName;
        public int Order;
        public int ParentLookUpID;

        public LookUpItem(int itemID, int lookUpID, string value, string shortName, int order, int parentLookUpID)
        {
            ItemID = itemID;
            LookUpID = lookUpID;
            Value = value ?? throw new ArgumentNullException(nameof(value));
            ShortName = shortName ?? throw new ArgumentNullException(nameof(shortName));
            Order = order;
            ParentLookUpID = parentLookUpID;
        }

        public LookUpItem(int lookUpID)
        {
            LookUpID = lookUpID;
            ItemID = -1;
            Value = "";
            ShortName = "";
            Order = 0;
            ParentLookUpID = -1;
        }

    }


}
