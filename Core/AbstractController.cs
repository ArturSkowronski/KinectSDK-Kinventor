using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Skowronski.Artur.Thesis
{
    interface AbstractController
    {
        void updateAngleByParameter(string name, int angle);
        List<TreeViewItem> createTreeViewByOccurences();
        void updateAngleByConstraints(string p1, int p2);

        bool isConstructed { get; set; }
    }
}
