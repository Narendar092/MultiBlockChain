using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace XICore
{
    public class XIExampleUsage
    {
        public void Main()
        {
            XIIXI oXII = new XIIXI();
            XIDXI oXID = new XIDXI();
            XIIQS oQSI;
            XIIQS oQSCopy;
            DateTime dDOB;
            XIIStructure oStructNodeI;
            string s1Click;
            string sValue;
            long iLen;
            string sLayout;

            oXII.BOI("Driver_T", "1222");
            //oXII.BOD("Claim_T"); // loads the defintion of a BO

            //Useful
            //oQSI = oXII.CreateQSI("Motorhome");

            //Useful
            //oQSCopy = (CIQS)oQSI.CopyMe();
            //Useful
           // oQSCopy.Save();

            // oXIAPI.Application('X').QS('123').Steps('Step Y').Section('Z').XIValue('A').dValue
            // Note: It is assumed that since the script is running from within the application that it knows which application we are looking at. But this needs to be coded (within the methods)
            dDOB = oXII.QSI("1227").StepI("Step 4").SectionI("1").XIValueI("dDOB").dValue;

            // Definition:
            //Useful
            //sValue = oXID.QSD("Motorhome").StepD("Step 4").SectionD("1").XIValueD("dDOB").sDefaultValue;
            // OR - if we don't have an explicit property 'sDefaultValue' then:

            //Useful
            //sValue = oXID.QSD("Motorhome").StepD("Step 4").SectionD("1").XIValueD("dDOB").BOI.AttributeI("sDefaultValue").sValue;


            dDOB = oXII.BOI("Policy_T", "1227").AttributeI("dDOB").dValue;

            oStructNodeI = oXII.BOI("Policy_T", "1227").StructureI("").StructureI("base");

            oStructNodeI = oXII.BOI("Policy_T", "1227").StructureI("").StructureI("base").StructureI("client");

            // s1Click = oXID.D1Click.BOD.AttributeD   '("726").BOD.AttributeD("sDisplayAs").sValue


            // instance - someone ran the 1-click?? I don't think 1click has an instance
            // s1Click = oXII.I1Click("726").BOI.AttributeI("sDisplayAs").sValue

            // Definition
            s1Click = ((XID1Click)oXID.Get_1ClickDefinition(null,726.ToString()).oResult).BOI.AttributeI("sDisplayAs").sValue;
            // OR
            s1Click = ((XID1Click)oXID.Get_1ClickDefinition(null,726.ToString()).oResult).sDisplayAs;


            // WRONG!! dDOB = oXII.BOI("Policy_T", "1227").BOUII.BOI.AttributeI("defaulttemplate").sValue

            // we want to know the configured template of a BOUI, so must cycle through the INSTANCE om. even though this is definition

            // these do the same thing:
            //Useful
            //iLen = oXID.BOD_Get("Policy_T").AttributeD("dDOB").iLength;
            //Useful
            //iLen = oXID.BOD_Get("Policy_T").AttributeD("dDOB").BOI.AttributeI("iLength").iValue;

            // these do the same thing
            //Useful
            //sLayout = oXID.BOD_Get("Policy_T").BOUID.sDefaultLayout;
            //Useful
            //sLayout = oXID.BOD_Get("Policy_T").BOUID.BOI.AttributeI("sdefaultlayout").sValue;
        }
    }
}