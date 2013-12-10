using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skowronski.Artur.Thesis
{
    public class ConstraintWrapper
    {
        AssemblyConstraint nativeAssemblyConstraint;
        public ConstraintWrapper(AssemblyConstraint oConstr,int oType)
        {
            nativeAssemblyConstraint = oConstr;
        }
        public AssemblyConstraint getNative()
        {
            return nativeAssemblyConstraint;
        }
        public ObjectTypeEnum getConstraintType() {
            return nativeAssemblyConstraint.Type;
        }

    }
}
