using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace SRTPluginProviderRE8.Structs
{
    [DebuggerDisplay("{_DebuggerDisplay,nq}")]
    public struct InventoryEntry // : IEquatable<InventoryEntry>, IEqualityComparer<InventoryEntry>
    {
        /// <summary>
        /// Debugger display message.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string _DebuggerDisplay
        {
            get
            {
                if (IsItem && IsAmmoClip) 
                    return string.Format("Weapon Clip {0} Quantity {1}", (ItemEnumeration)ItemID, Quantity);
                else if (IsItem && !IsAmmoClip)
                    return string.Format("[#{0}] Item {1} Quantity {2}", SlotPosition, (ItemEnumeration)ItemID, Quantity);
                else if (IsWeapon)
                    return string.Format("[#{0}] Weapon {1} Ammo {2}", SlotPosition, (WeaponEnumeration)ItemID, Ammo);
                else if (IsKeyItem)
                    return string.Format("Key Item {0}", (KeyItemEnumeration)ItemID);
                else if (IsTreasure)
                    return string.Format("Treasure {0} Quantity {1}", (TreasureEnumeration)ItemID, Quantity);
                else if (IsCraftable)
                    return string.Format("Craftable {0} Quantity {1}", (CraftableEnumeration)ItemID, Quantity);
                else if (IsMap)
                    return string.Format("Map {0}", (CraftableEnumeration)ItemID);
                else if (ItemID > 0)
                    return string.Format("Uknown Item 0x{0}", ItemID.ToString("X4"));
                else
                    return string.Format("Empty Slot");
            }
        }

        public string ItemName 
        { 
            get
            {
                if (IsItem)
                    return string.Format("{0}", (ItemEnumeration)ItemID);
                else if (IsWeapon)
                    return string.Format("{0}", (WeaponEnumeration)ItemID);
                else if (IsKeyItem)
                    return string.Format("{0}", (KeyItemEnumeration)ItemID);
                else if (IsTreasure)
                    return string.Format("{0}", (TreasureEnumeration)ItemID);
                else if (IsCraftable)
                    return string.Format("{0}", (CraftableEnumeration)ItemID);
                else if (IsMap)
                    return string.Format("{0}", (MapEnumeration)ItemID);
                else
                    return "None";
            }
        }

        //public static readonly int[] EMPTY_INVENTORY_ITEM = new int[5] { 0x00000000, unchecked((int)0xFFFFFFFF), 0x00000000, 0x00000000, 0x01000000 };

        // Storage variable.
        // Accessor properties.
        public uint ItemID { get => _itemid; set => _itemid = value; }
        internal uint _itemid;
        public byte HasAttachment { get => _hasAttachment; set => _hasAttachment = value; }
        internal byte _hasAttachment;
        public uint SlotPosition { get => _slotPosition; set => _slotPosition = value; }
        internal uint _slotPosition;
        //public AttachmentsFlag Attachments => (AttachmentsFlag)_data[2];
        public bool IsItem => Enum.IsDefined(typeof(ItemEnumeration), _itemid);
        public bool IsWeapon => Enum.IsDefined(typeof(WeaponEnumeration), _itemid);
        public bool IsKeyItem => Enum.IsDefined(typeof(KeyItemEnumeration), _itemid);
        public bool IsTreasure => Enum.IsDefined(typeof(TreasureEnumeration), _itemid);
        public bool IsCraftable => Enum.IsDefined(typeof(CraftableEnumeration), _itemid);
        public bool IsMap => Enum.IsDefined(typeof(MapEnumeration), _itemid);
        public bool IsAmmoClip => SlotPosition == 0xFFFFFFFF;
        public bool IsItemSlot => IsItem && !IsAmmoClip || IsWeapon;
        public int Quantity { get => _quantity; set => _quantity = value; }
        internal int _quantity;
        public byte IsHorizontal { get => _ishorizontal; set => _ishorizontal = value; }
        internal byte _ishorizontal;
        public int Ammo { get => _ammo; set => _ammo = value; }
        internal int _ammo;

        //public bool Equals(InventoryEntry other) => this == other;
        //public bool Equals(InventoryEntry x, InventoryEntry y) => x == y;
        //public override bool Equals(object obj)
        //{
        //    if (obj is InventoryEntry)
        //        return this == (InventoryEntry)obj;
        //    else
        //        return base.Equals(obj);
        //}
        //public static bool operator ==(InventoryEntry obj1, InventoryEntry obj2)
        //{
        //    if (ReferenceEquals(obj1, obj2))
        //        return true;
        //
        //    if (ReferenceEquals(obj1, null) || ReferenceEquals(obj1._data, null))
        //        return false;
        //
        //    if (ReferenceEquals(obj2, null) || ReferenceEquals(obj2._data, null))
        //        return false;
        //
        //    return obj1.SlotPosition == obj2.SlotPosition && obj1._data.SequenceEqual(obj2._data);
        //}
        //public static bool operator !=(InventoryEntry obj1, InventoryEntry obj2) => !(obj1 == obj2);
        //
        //public override int GetHashCode() => SlotPosition ^ _data.Aggregate((int p, int c) => p ^ c);
        //public int GetHashCode(InventoryEntry obj) => obj.GetHashCode();
        //
        //public override string ToString() => _DebuggerDisplay;

        //public InventoryEntry Clone()
        //{
        //    InventoryEntry clone = new InventoryEntry() { _data = new int[this._data.Length] };
        //    clone._slotPosition = this._slotPosition;
        //    for (int i = 0; i < this._data.Length; ++i)
        //        clone._data[i] = this._data[i];
        //    clone._invDataOffset = this._invDataOffset;
        //    return clone;
        //}

        //public static InventoryEntry Clone(InventoryEntry subject) => subject.Clone();
    }
}