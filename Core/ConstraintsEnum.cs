using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skowronski.Artur.Thesis
{
    public class ConstraintsEnum
    {
        public enum Item : int { Angle = 1, Position=4 };
        public static ObjectTypeEnum getClassType(Item enumItem)
        {
            switch (enumItem) { 
            case Item.Angle:
                return ObjectTypeEnum.kAngleConstraintObject;
            default:
                return ObjectTypeEnum.kAngleConstraintObject;
            }
        }
    }
}
