using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebWhatsappBotCore;

namespace DanielExample
{
    class Program
    {
        IWebWhatsappDriver _driver;
        static void Main(string[] args)
        {

            Program x = new Program();
            x.MainS(null);
        }
     
        void MainS(string[] args)
        {
            Console.WriteLine("1. Iniciar");
            Console.WriteLine("2. Configure");
            string x = Console.ReadLine();
          
            switch (x)
            {
                case "Iniciar":
                case "1":
                    Start(new WebWhatsappBotCore.Chrome.ChromeWApp());

                    break;
                case "Configure":
                case "2":
                   
                    break;
                default:
                    Main(null);
                    break;

            }
            Console.WriteLine("Pronto");
            Console.ReadKey();
        }
        //API DO DESKFres
        static void SendTicket(string msg)
        {
            string fdDomain = "jadson0102"; // your freshdesk domain
            string apiKey = "Hk5sKTE9nT3yF8GqMcWL";
            string apiPath = "/api/v2/tickets"; // API path
            string json = "{\"status\": 2, \"priority\": 1, \"email\":\"jadson0102@live.com\",\"subject\":\" ASSUNTO DO CHAMADO \",\"description\":\" "+ msg +" \"}";
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://" + fdDomain + ".freshdesk.com" + apiPath);
            Console.WriteLine("ok");
            //HttpWebRequest class is used to Make a request to a Uniform Resource Identifier (URI).  
            request.ContentType = "application/json";
            // Set the ContentType property of the WebRequest. 
            request.Method = "POST";

            byte[] byteArray = Encoding.UTF8.GetBytes(json);
            // Set the ContentLength property of the WebRequest. 
            request.ContentLength = byteArray.Length;
            string authInfo = apiKey + ":X"; // It could be your username:password also.
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            request.Headers["Authorization"] = "Basic " + authInfo;
            request.UserAgent = "Mozilla";
            request.KeepAlive = true;
            request.ProtocolVersion = HttpVersion.Version10;
       
            //Get the stream that holds request data by calling the GetRequestStream method. 
            Stream dataStream1 = request.GetRequestStream();

            // Write the data to the request stream. 
            dataStream1.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object. 
            dataStream1.Close();

        
                try
                {
                Console.WriteLine("Enviando Request");
                WebResponse response = request.GetResponse();
                // Get the stream containing content returned by the server.
                //Send the request to the server by calling GetResponse. 
                dataStream1 = response.GetResponseStream();
                // dataStream1 the stream using a StreamReader for easy access. 
                StreamReader reader = new StreamReader(dataStream1);
                // Read the content. 
                string Response = reader.ReadToEnd();
                //return status code
                Console.WriteLine("Status Code: {1} {0}", ((HttpWebResponse)response).StatusCode, (int)((HttpWebResponse)response).StatusCode);
                //return location header
                Console.WriteLine("Location: {0}", response.Headers["Location"]);
                //return the response 
                Console.Out.WriteLine(Response);
            }
            catch (WebException ex)
            {
                Console.WriteLine("API Error: Your request is not successful. If you are not able to debug this error properly, mail us at support@freshdesk.com with the follwing X-Request-Id");
                Console.WriteLine("X-Request-Id: {0}", ex.Response.Headers["X-Request-Id"]);
                Console.WriteLine("Error Status Code : {1} {0}", ((HttpWebResponse)ex.Response).StatusCode, (int)((HttpWebResponse)ex.Response).StatusCode);
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    Console.Write("Error Response: ");
                    Console.WriteLine(reader.ReadToEnd());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR");
                Console.WriteLine(ex.Message);
            }

        }
      
        void Start(IWebWhatsappDriver driver)
        {
            _driver = driver;
            driver.StartDriver();

            Console.WriteLine("Press enter after scan QRCode");
            Console.ReadKey();

            driver.OnMsgRecieved += OnMsgRec;
            Task.Run(() => driver.MessageScanner());

            Console.WriteLine("Use CTRL+C to exit");

        }
      
        private void OnMsgRec(IWebWhatsappDriver.MsgArgs arg)
        {
            /// MSG A SER ENVIADA
            Console.WriteLine(arg.Sender + " Escreveu: " + arg.Msg + " at " + arg.TimeStamp);
            string Suporte = @"*Olá, escolha uma das opções abaixo:*
1 - Sobre a Empresa.
2 - Criar um Ticket.
3 - Meus Tickets.";

            /// SE O CLIENTE DIGITAR OI, SERÁ ENVIADA A MSG PREDEFINIDA
            if (arg.Msg.ToLower().Contains("oi") && arg.Sender.ToLower().Contains("@c.us"))
            {
                Thread.Sleep(4000);
                _driver.SendMessageToNumber(arg.Sender, Suporte);         

            }
            /// SE DIGITOU 2 ENVIA A MSG QUAL O SEU NOME
            if (arg.Msg.ToLower().Contains("2") && arg.Sender.ToLower().Contains("@c.us"))
            {
                Thread.Sleep(4000);

                _driver.SendMessageToNumber(arg.Sender, "*Qual o seu nome?*");

               
            }

            if (arg.Msg == "2")
            {
                if (arg.Msg != "2")
                {
                    string texto = arg.Msg;
                    _driver.SendMessageToNumber(arg.Sender, "Seu nome é: " + texto);
                }

            }




            //if (arg.Msg != null && arg.Sender.ToLower().Contains("@c.us"))
            //{                
            //        _driver.SendMessageToNumber(arg.Sender, "Seu nome é: " + arg.Msg);
            //        _driver.SendMessageToNumber(arg.Sender, "Digite a Mensagem: ");                        

            //}

            //if (arg.Msg != null && arg.Sender.ToLower().Contains("@c.us"))
            //{
            //    _driver.SendMessageToNumber(arg.Sender, "Seu nome é: " + arg.Msg);
            //    _driver.SendMessageToNumber(arg.Sender, "Digite a Mensagem: ");

            //}


            //if (arg.Msg != null)
            //    {
            //        SendTicket(arg.Msg.ToString());
            //    }




            //if (arg.Msg.ToLower().StartsWith("/group"))
            //{
            //    string[] argr = arg.Msg.Split('-');

            //    String nombreGrupo = argr[1].Trim();
            //    string[] numerosGrupo = argr[2].Split(' ');
            //    numerosGrupo = numerosGrupo.Where(x => x != "").ToArray();

            //    _driver.createGroupFast(nombreGrupo, numerosGrupo);
            //    Thread.Sleep(500);
            //    _driver.SendMessageToName(nombreGrupo, nombreGrupo);
            //}

            if (arg.Msg.ToLower().StartsWith("/number"))
            {
                _driver.SendMessageToNumber(arg.Msg.ToLower().Split('-')[1], arg.Msg.ToLower().Split('-')[2], arg.Msg.Split('-')[3]);
            }


        }
    }
}
