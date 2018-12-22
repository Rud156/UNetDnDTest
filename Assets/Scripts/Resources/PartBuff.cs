using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UNetUI.Resources
{
    public class PartBuff : MonoBehaviour
    {
        private Item _buffItem;

        public Item GetItem() => _buffItem;

        public Item SetItem(Item buffItem) => _buffItem = buffItem;
    }
}