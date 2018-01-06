using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntegralSystem
{
    class VipInfo
    {
        public int vipId = 0;
        public string vipName;
        public string tel;
        public float bonus = 0;
        public float maxBonus = 0;

        public VipInfo(int vipId, string vipName, string tel, float bonus, float maxBonus)
        {
            this.vipId = vipId;
            this.vipName = vipName;
            this.tel = tel;
            this.bonus = bonus;
            this.maxBonus = maxBonus;
        }

        public override string ToString()
        {
            return string.Format("{0}-{1} {2}", vipId, vipName, tel);
        }
    }
}
