using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Weather.WeatherService;
using Weather.CyberSharkService;
using Weather.CyberSharkService2;

namespace Weather
{
    public partial class Form1 : Form
    {


        
        string pfxSubject = "Subject=";
        string pfxPhoneNumber= "PhoneNumber=";
        string pfxCustomerName= "CustomerName=";
        string pfxDescription = "Description=";
        string pfxDateStart = "DateStart=";
        string pfxDateEnd = "DateEnd=";
        string pfxGroupID = "GroupId=";
        string pfxUserID = "UserId=";



        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        void getWeatherByZip(string zipCode)
        {
           
            try
            {

                WeatherService.WeatherSoapClient client = new WeatherService.WeatherSoapClient();
                client.Open();

                WeatherReturn  ret = new WeatherReturn ();
              
                ret  =  client.GetCityWeatherByZIP(zipCode);

                rtxtResults.Text =  "City:        " + ret.City + Environment.NewLine
                                 + "State:       " + ret.State  + Environment.NewLine
                                 +  "Skies:       "  + ret.Description + Environment.NewLine
                                 +  "Pressure:    " + ret.Pressure + Environment.NewLine
                                 +  "Humidity:    " + ret.RelativeHumidity + Environment.NewLine
                                 +  "Temperature: " + ret.Temperature + Environment.NewLine
                                 +  "Wind:        " + ret.Wind + Environment.NewLine;




            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private string createCallLog ()
        {
            string w = "";

            string user = "George";
            string password = "pants#1";

            try
            {
                /*
                // create a test entry
                var parameters = new CyberSharkService.ArrayOfString 
                {
                  pfxSubject + this.txtSubject .Text ,
                  pfxPhoneNumber + this.txtPhoneNumber.Text,
                  pfxCustomerName + this.txtCustomerName .Text,
                  pfxDateStart + this.txtStart .Text,
                  pfxDateEnd + this.txtEnd.Text ,
                  pfxGroupID + this.txtGroupId .Text,
                  pfxUserID + this.txtUserId .Text
                };

                MessageBox.Show(parameters.ToString());

                // create a service client
                CyberSharkService.svcCallLogsSoapClient client = new CyberSharkService.svcCallLogsSoapClient();
                // open the client
                client.Open();
               
                // invoke create call log
                CyberSharkService.CallLogIdResult result = client.CreateCallLog2(user, password, parameters);
                // display the result in a message box
                MessageBox.Show(result.CallLogId.ToString() + "|"
                                    +  result.CallNumber + "|"
                                        + result.Message.ToString() + "|"
                                                + result.Result.ToString() + "|"
                                                    + result.ResultCode.ToString() + "|" );
                // close the client
                client.Close
                 */


                CyberSharkService2.svcOrganizationSoapClient orgClient = new CyberSharkService2.svcOrganizationSoapClient();

                CyberSharkService2.GroupInfoArrayResult giresult = new GroupInfoArrayResult();

                giresult =  orgClient.GetAllGroups("George", "pants#1");

                for (int i = 0; i < giresult.Groups.Length; i++)
                {
                    GroupInfo info = giresult.Groups[i];
                    MessageBox.Show (info.Name);
                }

              
                
                
            }
            
            catch (Exception e)
            {
                // display the exception
                MessageBox.Show(e.ToString(), "Exception...");
            }
            

            return w;
        }

        private void button1_Click(object sender, EventArgs e)
        {






            createCallLog();
        }

        private void btnGetWeather_Click(object sender, EventArgs e)
        {
            getWeatherByZip(this.txtZip.Text);
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
