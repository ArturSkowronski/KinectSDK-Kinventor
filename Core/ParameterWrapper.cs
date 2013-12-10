using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skowronski.Artur.Thesis
{
    class ParameterWrapper
    {
        
        public Parameter nativeParameter{
        get;set;
        }

        public ParameterWrapper(Parameter oParam)
        {
            nativeParameter = oParam;
        }


        public void updateAngleByParameter(int angle)
        {
            Inventor.Parameter invParam = nativeParameter;
            invParam.Value = Math.PI * angle / 180;
 
        }
    }
}
