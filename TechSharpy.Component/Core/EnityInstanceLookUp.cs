using System;
using System.Collections.Generic;
using System.Data;
using TechSharpy.Entitifier.Data;
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
        private Entitifier.Data.LookUp dataLookup; 
        public EnityInstanceLookUp(int lookUpID, string name, bool isCore, bool haveChild, List<LookUpItem> lookUpItems, LookUpType lookUpType)
        {
            LookUpID = lookUpID;
            Name = name;
            IsCore = isCore;
            HaveChild = haveChild;
            LookUpItems = lookUpItems ?? throw new ArgumentNullException(nameof(lookUpItems));
            LookUpType = lookUpType;
            dataLookup = new LookUp();
        }

        public EnityInstanceLookUp(int lookUpID)
        {
            LookUpID = lookUpID;
            Name = "";
            IsCore = false;
            HaveChild = false;
            LookUpItems = new List<LookUpItem>();
            LookUpType = LookUpType._None;
            dataLookup = new LookUp();
        }

        public void Init() {
            DataTable dt, dtLookUp = new DataTable();
            dt = dataLookup.GetLookUpItems(-1, LookUpID);
            dtLookUp = dt.DefaultView.ToTable(true, "ClientID", "LookUpId", "LookUPName", "IsCore", "HaveChild",  "LookUpType");
            foreach (DataRow dr in dtLookUp.Rows)
            {
                Name = dr["LookUPName"] == DBNull.Value ? "" : dr["LookUPName"].ToString();
                IsCore = dr["IsCore"] == DBNull.Value ? false : Convert.ToBoolean(dr["IsCore"]);
                LookUpType = dr["LookUpType"] == DBNull.Value ?  LookUpType._None :  (LookUpType)dr["LookUpType"];
                HaveChild = dr["HaveChild"] == DBNull.Value ? false : Convert.ToBoolean(dr["HaveChild"]);
            }
            foreach (DataRow dr in dt.Rows)
            {
                LookUpItem item = new LookUpItem{                   
                    ItemID = dr["LookupInstanceID"] == DBNull.Value ? -1 : Convert.ToInt32(dr["LookupInstanceID"]),
                    LookUpID = this.LookUpID,
                    Name = dr["ItemName"] == DBNull.Value ? "" : Convert.ToString(dr["ItemName"]),
                    ShortName = dr["ShortName"] == DBNull.Value ? "" : Convert.ToString(dr["ShortName"]),
                    Order = dr["Order"] == DBNull.Value ? -1 : Convert.ToInt32(dr["Order"]),
                    ParentLookUpID = dr["ParentLookUpID"] == DBNull.Value ? -1 : Convert.ToInt32(dr["ParentLookUpID"]),
                };
                LookUpItems.Add(item);
            }
        }
        public int AddLookUpItem(int lookupID, LookUpItem lookUpItem) {
            int inext;
            if (lookUpItem.ItemID > 0)
            {
                if (dataLookup.SaveItem(-1, lookUpItem.ItemID, lookupID, lookUpItem.Name, lookUpItem.ShortName, lookUpItem.Order, lookUpItem.ParentLookUpID))
                {
                    inext = lookUpItem.ItemID;
                }
                else {
                    inext = -1;
                };
            }
            else {
                inext = dataLookup.SaveItem(-1, lookupID, lookUpItem.Name, lookUpItem.ShortName, lookUpItem.Order, lookUpItem.ParentLookUpID);
            }           
            return inext;
        }
        public bool RemoveLookUpItem(int itemID) {
            if (dataLookup.DeleteLookUpItem(-1, LookUpID, itemID))
            {
                return true;
            }
            else {
                return false;
            }
        }
        public bool Save() {
            int inext;
            if (this.LookUpID > 0)
            {
                dataLookup.Save(-1, this.LookUpID, this.Name, this.IsCore, this.HaveChild, this.LookUpType);
                foreach (LookUpItem item in this.LookUpItems)
                {
                    AddLookUpItem(this.LookUpID, item);
                }
            }
            else {
                inext = dataLookup.Save(-1, this.Name, this.IsCore, this.HaveChild, this.LookUpType);
                if (inext > 0)
                {
                    foreach (LookUpItem item in this.LookUpItems)
                    {
                        AddLookUpItem(inext, item);
                    }
                }
            }           
            return true;
        }

        public bool Remove() {
            if (dataLookup.Delete(-1, LookUpID)) {
                dataLookup.DeleteLookUpItems(-1, LookUpID);
            }
            return true;
        }                      
    }

   public class LookUpItem {
        public int ItemID;
        public int LookUpID;
        public string Name;
        public string ShortName;
        public int Order;
        public int ParentLookUpID;
        public LookUpItem() {

        }
        public LookUpItem(int itemID, int lookUpID, string value, string shortName, int order, int parentLookUpID)
        {
            ItemID = itemID;
            LookUpID = lookUpID;
            Name = value ?? throw new ArgumentNullException(nameof(value));
            ShortName = shortName ?? throw new ArgumentNullException(nameof(shortName));
            Order = order;
            ParentLookUpID = parentLookUpID;
        }

        public LookUpItem(int lookUpID)
        {
            LookUpID = lookUpID;
            ItemID = -1;
            Name = "";
            ShortName = "";
            Order = 0;
            ParentLookUpID = -1;
        }

    }

}
