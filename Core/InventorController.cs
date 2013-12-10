using Inventor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Skowronski.Artur.Thesis
{
    public class InventorController : AbstractController
    {
        Inventor.Application inventorApplication = null;
        AssemblyDocument assemblyDoc;
        AssemblyComponentDefinition assemblyComp;
        int getComponent;
        int getComponent2;
        ComponentOccurrence oOccurrence;
        Object oEntity1;
        Object oEntity2;
        public bool isConstructed{
        get;set;
        }
        AngleConstraint a;
        Dictionary<string, ParameterWrapper> parameterList;
        public InventorController()
        {
            isConstructed=false;
            try
            {
                inventorApplication = (Inventor.Application)
                System.Runtime.InteropServices.Marshal.
                GetActiveObject("Inventor.Application");
                assemblyDoc = (AssemblyDocument)inventorApplication.ActiveDocument;
                assemblyComp = assemblyDoc.ComponentDefinition;
                parameterList = getParameterList();
                isConstructed = true;
                createConstraintList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex.Message); 
                return;
            }
            isConstructed = true;
        }
        public void updateAngleByConstraints(string p1, int angle)
        {
            a = (AngleConstraint)constraintList[p1];
            oEntity2 = a.EntityTwo;
            oEntity1 = a.EntityOne;
            String sVal = "" + angle + " deg";
            a.Delete();
            constraintList.Remove(p1);
            AngleConstraint d = assemblyComp.Constraints.AddAngleConstraint(oEntity1, oEntity2, sVal);
            d.Name = p1;
            constraintList.Add(p1, (AssemblyConstraint)d);
        }


        private Dictionary<string, ParameterWrapper> getParameterList()
        {
            Dictionary<string, ParameterWrapper> parameterList = new Dictionary<string, ParameterWrapper>();
            foreach (Parameter parametre in assemblyDoc.ComponentDefinition.Parameters)
            {
                parameterList.Add(parametre.Name, new ParameterWrapper(parametre));
            }
            return parameterList;
        }

        private Dictionary<string, OccurenceWrapper> getOccurenceList()
        {
            Dictionary<string, OccurenceWrapper> occurenceList = new Dictionary<string, OccurenceWrapper>();
            for (int i = 0; i < assemblyComp.Occurrences.Count;i++ )
               {
                   occurenceList.Add(assemblyComp.Occurrences[i].Name, new OccurenceWrapper(assemblyComp.Occurrences[i]));
                   foreach (AssemblyConstraint occurence in assemblyComp.Occurrences[i].Constraints)
                   {

                   }
               }
            return occurenceList;
        }

       Dictionary<string,AssemblyConstraint> constraintList = new Dictionary<string,AssemblyConstraint>();



       public void createConstraintList()
       {
            foreach (ComponentOccurrence occurrence in assemblyComp.Occurrences)
            {
                foreach (AssemblyConstraint constraint in occurrence.Constraints)
                {
                    constraintList.Add(constraint.Name, constraint);
                }
            }
          
       }
        public void updateAngleByParameter(string name,  int angle)
        {
            parameterList[name].updateAngleByParameter(angle);
            inventorApplication.ActiveDocument.Update();
        }
        public List<TreeViewItem> createTreeViewByOccurences()
        {
            List<TreeViewItem> TreeViewConstraints = new List<TreeViewItem>();
            try
            {
                foreach (ComponentOccurrence occurrence in assemblyComp.Occurrences)
                {
                    TreeViewItem TVI = new TreeViewItem();
                    TVI.FontSize = 22;
                    TVI.Header = occurrence.Name;
                    foreach (AssemblyConstraint constraint in occurrence.Constraints)
                    {
                        TreeViewItem TVI2 = new TreeViewItem();
                        TVI2.Header = constraint.Name;
                        TVI.Items.Add(TVI2);
                    }
                    TreeViewConstraints.Add(TVI);
                }
            }
            catch { }
            return TreeViewConstraints;
        }
       

        public override string ToString()
        {
            String toString = "";
            foreach (ComponentOccurrence occurrence in assemblyComp.Occurrences)
            {
                toString += "" + occurrence.Name + "\n";
                foreach (AssemblyConstraint constraint in occurrence.Constraints)
                {
                    toString += "\t: " + constraint.Name + "\n";
                }
            }
            return toString;
        }

    }
}
