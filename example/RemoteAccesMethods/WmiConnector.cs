using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteAccesMethods
{
    public class WmiConnector
    {
        /*
         * /Make a connection to a remote computer using these options
		ManagementScope scope = new ManagementScope("\\\\198.31.1.1\\root\\cimv2", options);
		scope.Connect();


		// Build a query for enumeration of Win32_Environment instances
		SelectQuery query = new SelectQuery("Win32_Process");

		// Instantiate an object searcher with this query
		ManagementObjectSearcher searcher = new ManagementObjectSearcher(query); 

		// Call Get() to retrieve the collection of objects and loop through it
		foreach (ManagementBaseObject envVar in searcher.Get())
			Console.WriteLine("Variable : {0}, Value = {1}", 
				envVar["Name"],envVar["ExecutablePath"]);


        #######################
        HKLM = 2147483650 ;HKEY_LOCAL_MACHINE
sRegPath = "SYSTEM\CurrentControlSet\Services\Eventlog\System";

oLoc = CreateObject("WbemScripting.SWbemLocator")
oSvc = oLoc.ConnectServer(computername, "root/default",user,password)
oReg = oSvc.Get("StdRegProv")
oMethod = oReg.Methods_.Item("EnumKey");
oInParam = oMethod.InParameters.SpawnInstance_();
oInParam.hDefKey = HKLM;
oInParam.sSubKeyName = sRegPath;
oOutParam = oReg.ExecMethod_(oMethod.Name, oInParam);    
aNames = oOutParam.sNames;

subkeylist = ""

for i = 0 to ArrInfo(aNames,6)-1
	 name = aNames[i]
    ;Message("Subkeys", StrCat("KeyName: ", name));
    subkeylist = StrCat(subkeylist,@tab,name)
Next
subkeylist = StrTrim(subkeylist)
AskItemList(sRegPath,subkeylist,@tab,@unsorted,@single)

        #######################
                */
    }
}
