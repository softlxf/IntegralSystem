using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntegralSystem
{
    class GoodsInfo : IComparable<GoodsInfo>  
    {
        public int id = 0;
        public string name;
        public float price = 0;
        public int count = 0;

        public GoodsInfo(int id, string name, float price, int count)
        {
            this.id = id;
            this.name = name;
            this.price = price;
            this.count = count;
        }

        public override string ToString()
        {
            if (count == 1)
                return string.Format("{0}({1})", name, price);
            return string.Format("{0}({1}*{2})", name, price, count);
        }

        public int CompareTo(GoodsInfo other)
        {
            return id.CompareTo(other.id);
        }
    }
}
