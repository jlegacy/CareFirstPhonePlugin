using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CSPluginForm.OrganizationService;
using CSPluginForm.CallLogService;
using System.Configuration;
using System.ServiceModel;

namespace CallLoginForm
{
    public partial class PluginForm : Form
    {

        string pluginUser = "george";
        string pluginPassword = "pants#1";



        OrganizationService.svcOrganizationSoapClient organizationClient;
        CallLogService.svcCallLogsSoapClient callLogsClient;
        Dictionary<string, int> dictGroup = new Dictionary<string, int>();
        Dictionary<string, int> dictAgent = new Dictionary<string, int>();
        Dictionary<int, string> dictUser = new Dictionary<int, string>();



        EndpointAddress organizationEndpoint = new EndpointAddress("http://support.cybersharks.net/services2/svcOrganization.asmx");
        EndpointAddress callLogsEndpoint = new EndpointAddress("http://support.cybersharks.net/services2/svcCallLogs.asmx");


        public string phone;
        public DateTime fromDate;
        public DateTime fromTime;
        public DateTime toDate;
        public DateTime toTime;
        public bool callAnswered = false;

        public int groupId;
        public int userId;
        public string userName;

        string defaultGroup;
        string defaultAgent;


        string pfxSubject = "Subject=";
        string pfxPhoneNumber = "PhoneNumber=";
        string pfxCustomerName = "CustomerName=";
        string pfxDescription = "Description=";
        string pfxDateStart = "DateStart=";
        string pfxDateEnd = "DateEnd=";
        string pfxGroupID = "GroupId=";
        string pfxUserID = "UserId=";
        string pfxEmailAddress = "EmailAddress=";
        string pfxAgentID = "AgentID=";



        public PluginForm()
        {
            InitializeComponent();

        }



        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PluginForm_Load(object sender, EventArgs e)
        {
            try
            {

                //Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                //config.AppSettings.Settings["IP"].Value = "10.0.0.2";
                //config.Save(ConfigurationSaveMode.Modified);
                //ConfigurationManager.RefreshSection("appSettings"); 


                //Open the configuration file using the dll location
                Configuration myDllConfig =
                  System.Configuration.ConfigurationManager.OpenExeConfiguration(this.GetType().Assembly.Location);
                // Get the appSettings section
                AppSettingsSection myDllConfigAppSettings =
                       (AppSettingsSection)myDllConfig.GetSection("appSettings");
                // return the desired field 
                string defaultGroup = myDllConfigAppSettings.Settings["DefaultGroup"].Value;
                string defaultAgent = myDllConfigAppSettings.Settings["DefaultAgent"].Value;

                BasicHttpBinding binding = new BasicHttpBinding();
                binding.Name = "svcOrganizationSoap";

                // load the group combo box
                // create a new Organization client 
                organizationClient = new svcOrganizationSoapClient(binding, organizationEndpoint);
                //  fill the group dictionary
                GroupInfoArrayResult giResult = new GroupInfoArrayResult();
                giResult = organizationClient.GetAllGroups(pluginUser, pluginPassword);
                for (int i = 0; i < giResult.Groups.Length; i++)
                {
                    GroupInfo info = giResult.Groups[i];
                    dictGroup.Add(info.Name, info.ID);
                }
                // fill the box

                foreach (KeyValuePair<string, int> pair in dictGroup)
                {
                    cboGroupName.Items.Add(pair.Key);

                }

                // set the default group
                cboGroupName.SelectedItem = defaultGroup;
                // get the slelected group id from the dictionary
                if (dictGroup.ContainsKey(cboGroupName.SelectedItem.ToString()))
                {
                    groupId = dictGroup[cboGroupName.SelectedItem.ToString()];
                }
                // get a list of agents for the group
                AgentInfoArrayResult agentResult = organizationClient.GetAgentsForGroup(pluginUser, pluginPassword, groupId);
                // clear any previous dictionary items
                dictAgent = new Dictionary<string, int>();
                dictUser = new Dictionary<int, string>();
                // load the list to the agents dictionary
                for (int i = 0; i < agentResult.Agents.Length; i++)
                {

                    AgentInfo agent = agentResult.Agents[i];
                    dictAgent.Add(agent.DisplayName, agent.ID);
                    dictUser.Add(agent.ID, agent.UserName);

                }
                // load the agents combo box
                cboAgent.Items.Clear();
                foreach (KeyValuePair<string, int> pair in dictAgent)
                {
                    cboAgent.Items.Add(pair.Key);
                }
                // set the default agent
                cboAgent.SelectedItem = defaultAgent;




                //  fill the call boxes
                this.txtPhone.Text = phone;
                this.dtpFromDate.Value = fromDate;
                this.dtpFromTime.Value = fromDate;
                this.dtpDateTo.Value = toDate;
                this.dtpTimeTo.Value = toDate;


            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred loading the CyberSharksPlugin CallLog form..." + Environment.NewLine +
                                    ex.ToString(), "CyberShark Plugin Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cboGroupName_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {
                // get the slelected group id from the dictionary
                if (dictGroup.ContainsKey(cboGroupName.SelectedItem.ToString()))
                {
                    groupId = dictGroup[cboGroupName.SelectedItem.ToString()];
                }
                // get a list of agents for the group
                AgentInfoArrayResult agentResult = organizationClient.GetAgentsForGroup(pluginUser, pluginPassword, groupId);
                // clear any previous dictionary items
                dictAgent = new Dictionary<string, int>();
                dictUser = new Dictionary<int, string>();
                // load the list to the agents dictionary
                for (int i = 0; i < agentResult.Agents.Length; i++)
                {

                    AgentInfo agent = agentResult.Agents[i];
                    dictAgent.Add(agent.DisplayName, agent.ID);
                    dictUser.Add(agent.ID, agent.UserName);

                }
                // load the agents combo box
                cboAgent.Items.Clear();
                foreach (KeyValuePair<string, int> pair in dictAgent)
                {
                    cboAgent.Items.Add(pair.Key);
                }
                // set the default agent
                cboAgent.SelectedItem = defaultAgent;

            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred selecting a group name in  the CyberSharksPlugin CallLog form..." + Environment.NewLine +
                                   ex.ToString(), "CyberShark Plugin Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {

            try
            {

                // make a binding
                BasicHttpBinding binding = new BasicHttpBinding();
                binding.Name = "svcCallLogsSoap";
                // create a clall logs client
                callLogsClient = new svcCallLogsSoapClient(binding, callLogsEndpoint);
                // set parameters from the form
                var parameters = new CallLogService.ArrayOfString 
                {
                  pfxSubject + this.txtSubject .Text ,
                  pfxPhoneNumber + this.txtPhone.Text,
                  pfxCustomerName + this.txtName.Text ,
                  pfxDateStart + this.dtpFromDate.ToString (),
                  pfxDateEnd + this.dtpDateTo .ToString (),
                  pfxGroupID + groupId.ToString (),
                  pfxUserID +  userId.ToString (),
                  pfxEmailAddress + txtEmail .Text,
                  pfxDescription + this.txtText.Text

                };
                // create a call log
                CallLogIdResult result = callLogsClient.CreateCallLog2(pluginUser, pluginPassword, parameters);

                if (result.Result)
                {
                    ssStatus.Text = "The Call Log was saved...";
                    MessageBox.Show(result.Message + Environment.NewLine + "The Call Log was saved",
                                 "CyberShark Plugin Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                }
                else
                {
                    ssStatus.Text = "The Call Log was NOT saved...";
                    MessageBox.Show(result.Message + Environment.NewLine + "The Call Log was NOT saved",
                                 "CyberShark Plugin Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("An error occurred adding a call log in   the CyberSharksPlugin CallLog form..." + Environment.NewLine +
                                 ex.ToString(), "CyberShark Plugin Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            this.Close();
        }

        private void cboAgent_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // set the agent id
                if (dictAgent.ContainsKey(cboAgent.SelectedItem.ToString()))
                {
                    userId = dictAgent[cboAgent.SelectedItem.ToString()];
                }
                if (dictUser.ContainsKey(userId))
                {
                    userName = dictUser[userId];
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred selecting an agent   the CyberSharksPlugin CallLog form..." + Environment.NewLine +
                                ex.ToString(), "CyberShark Plugin Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
