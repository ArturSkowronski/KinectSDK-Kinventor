using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Skowronski.Artur.Thesis
{
    class SkeletonAnalyzerContainer
    {
        Dictionary<string, SkeletonAnalyzer> skeletonList;
        Dictionary<string, SkeletonAnalyzer> skeletonListCon;

        public SkeletonAnalyzerContainer() {
            skeletonList = new Dictionary<string, SkeletonAnalyzer>();
            skeletonListCon = new Dictionary<string, SkeletonAnalyzer>();

        }

        public void AddParameter(string name, SkeletonAnalyzer skeleton)
        {
            skeletonList.Add(name, skeleton);
        }

        public Dictionary<string, SkeletonAnalyzer> getParameterList()
        {
            return skeletonList;
        }
        public Dictionary<string, SkeletonAnalyzer> getConstraintList()
        {
            return skeletonListCon;
        }
        public SkeletonAnalyzer GetParameter(string name)
        {
            return skeletonList[name];
        }
        public new String ToString()
        {
            string toString = "";
            foreach (KeyValuePair<string, SkeletonAnalyzer> skeleton in getParameterList())
            {
                List<JointType> bodySegments=skeleton.Value.GetBodySegments();
                toString += skeleton.Key + " :: " + bodySegments[0] + " : " + bodySegments[1] + " : " + bodySegments[2]+"\n";
            }
            return toString;
        }

        internal void AddConstraints(string name, SkeletonAnalyzer analyzer)
        {
            skeletonListCon.Add(name, analyzer);
        }
    }
}
