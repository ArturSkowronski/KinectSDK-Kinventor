using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skowronski.Artur.Thesis
{
    public class OccurenceWrapper
    {
        ComponentOccurrence nativeCompOccurence;
        public OccurenceWrapper(ComponentOccurrence oCompOc)
        {
            nativeCompOccurence = oCompOc;
        }

        public ComponentOccurrence getNative()
        {
            return nativeCompOccurence;
        }

        public List<AssemblyConstraint> getConstraintListByType(ConstraintsEnum.Item item)
        {
            var constraintList=new List<AssemblyConstraint>();
            foreach (AssemblyConstraint constraint in nativeCompOccurence.Constraints)
            {
                if (constraint.Type == ConstraintsEnum.getClassType(item)) constraintList.Add(constraint);
            }
            return constraintList;
        }
    }  
}
