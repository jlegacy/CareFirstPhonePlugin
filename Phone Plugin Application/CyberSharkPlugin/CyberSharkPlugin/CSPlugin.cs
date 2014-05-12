using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Diagnostics;
using TCX.CRM;

namespace CyberSharkPlugin
{
    public class CSPlugin : IPlugin
    {

        public bool storeCallInfo = true;

        public void CreateContactRecord(Contact contact)
        {
        }

        public string GetAdditionalWorkingStatusInformation()
        {
            return "";
        }

        public PluginConfigurationControl GetConfigurationControl()
        {
            return new CyberSharkConfigurationControl();
        }

        public Contact GetContactInformation(string contactNumber)
        {


            Contact contact = new Contact();
            try
            {     
              
                string callerNumber = stripNonDigits(contactNumber);
                contact.Phone = new Phone(callerNumber);


                //Open the configuration file using the dll location
                Configuration myDllConfig =
                  System.Configuration.ConfigurationManager.OpenExeConfiguration(this.GetType().Assembly.Location);
                // Get the appSettings section
                AppSettingsSection myDllConfigAppSettings =
                       (AppSettingsSection)myDllConfig.GetSection("appSettings");
                // return the desired field 
                string CallLogFileName = myDllConfigAppSettings.Settings["CallLogFileName"].Value;
                




                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = CallLogFileName ;
                startInfo.Arguments = callerNumber;
                Process exeProcess = Process.Start(startInfo);
                
      
               

             }

              catch (Exception ex)
            {
                MessageBox.Show( ex.ToString () ,
                              "CyberShark Plugin Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            return contact;
        }


        private string stripNonDigits(string input)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in input) if (Char.IsDigit(c)) sb.Append(c);
            return sb.ToString();
        }

        public System.Drawing.Icon GetIcon()
        {

            return Properties.Resources._3cx;
             
        }

        public string GetName()
        {
            return "CyberSharkPlugin";
        }

        public bool HasToStoreCallInformation
        {
            get
            {
                return storeCallInfo;
            }
            set
            {
                storeCallInfo = value;
            }
        }

        public event OutboundCallRequestHandler OnOutboundCallRequest;

        public void ReloadConfiguration()
        {
             
        }

        public void ShowContactRecord(Contact contact)
        {
           
           
        }

        public void Start()
        {
            
            
        }

        public void Stop()
        {
         
        }

        public void StoreCallInformation(CallInformation callInformation)
        {
            try
            {
            
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        public void VerifyPluginState()
        {
             
        }
    }
}
