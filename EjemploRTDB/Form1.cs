using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Google.Cloud.Firestore.V1;
//using HTTPupt;

namespace CiberController
{
    public partial class CiberController : Form
    {

        [DllImport("Powrprof.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);

        RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        [DllImport("user32")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);
        const int MF_BYCOMMAND = 0;
        const int MF_DISABLED = 2;
        const int SC_CLOSE = 0xF060;

        FirestoreDb db;
        Epoch epoch = new Epoch();
        //Registro registro;
        //IFirebaseConfig config = new FirebaseConfig()
        //{
        //    AuthSecret = "BWf21sX7rDLb21055H48DGJHlgLI6yyBUYeYUf96",
        //    BasePath = "https://cybercafe-b671a-default-rtdb.firebaseio.com/"
        //};
        public int modoInvitado = 0;
        public int estatusBloqueo;
        public int estadoPC;
        //public int entra = 0;
        public int FechaInicial0;
        public int FechaFinal0;

        public String correo;
        public String numPC;

        public String imagepath = Path.Combine(Application.StartupPath, @"img.jpg");
        public String credencialespath = Path.Combine(Application.StartupPath, @"Credenciales.txt");
        //public int datoTimer = 0;
        //public String tiempoInicial;
        //private String CalcularTiempo(Int32 tsegundos)
        //{
        //    Int32 horas = (tsegundos / 3600);
        //    Int32 minutos = ((tsegundos - horas * 3600) / 60);
        //    Int32 segundos = tsegundos - (horas * 3600 + minutos * 60);
        //    return horas.ToString() + ":" + minutos.ToString() + ":" + segundos.ToString();
        //}

        static void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode == PowerModes.Suspend)
            {
                Console.WriteLine("La computadora se está suspendiendo.");
                RestartApplication();
            }
        }

        static void RestartApplication()
        {
            string applicationName = Process.GetCurrentProcess().MainModule.FileName;
            Process.Start(applicationName);
            Environment.Exit(0);
        }

        public CiberController()
        {
            InitializeComponent();
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
            Boolean conexion = false;


            do
            {
                string Estado = "";
                System.Uri Url = new System.Uri("https://www.google.com/");

                System.Net.WebRequest WebRequest;
                WebRequest = System.Net.WebRequest.Create(Url);
                System.Net.WebResponse objetoResp;

                try
                {
                    objetoResp = WebRequest.GetResponse();
                    objetoResp.Close();
                    conexion = true;
                }
                catch (Exception e)
                {
                    Estado = "Necesita estar conectado a internet para ejecutar CiberController";
                    MessageBox.Show(Estado);
                }
                WebRequest = null;


            } while (conexion == false);

            rkApp.SetValue("MyApp", Application.ExecutablePath);
            //CheckForIllegalCrossThreadCalls = false;
            //GoogleCredential credencial = GoogleCredential.FromFile("credenciales.json");
            
            String path = AppDomain.CurrentDomain.BaseDirectory + @"fireStore.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
            db = FirestoreDb.Create("internet-77e6f");
            guardaDatos();
            //registro = new Registro()
            //{
            //    NombrePC = "PC"+numPC,
            //    TiempoTotal = 0
            //};
            //agregar(registro);
            escuchar();

        }


        private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Resume:
                    // Se ejecuta cuando la computadora regresa de la suspensión o hibernación
                    EjecutarComando();
                    break;
            }
        }

        private void EjecutarComando()
        {
            //MessageBox.Show("La computadora ha regresado de la suspensión o hibernación");
            Boolean conexion = false;

            do
            {
                string Estado = "";
                System.Uri Url = new System.Uri("https://www.google.com/");

                System.Net.WebRequest WebRequest;
                WebRequest = System.Net.WebRequest.Create(Url);
                System.Net.WebResponse objetoResp;

                try
                {
                    objetoResp = WebRequest.GetResponse();
                    objetoResp.Close();
                    conexion = true;
                }
                catch (Exception e)
                {
                    Estado = "Necesita estar conectado a internet para ejecutar CiberController";
                    MessageBox.Show(Estado);
                }
                WebRequest = null;


            } while (conexion == false);

            rkApp.SetValue("MyApp", Application.ExecutablePath);
            //CheckForIllegalCrossThreadCalls = false;
            //GoogleCredential credencial = GoogleCredential.FromFile("credenciales.json");

            String path = AppDomain.CurrentDomain.BaseDirectory + @"fireStore.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
            db = FirestoreDb.Create("internet-77e6f");
            guardaDatos();
            //registro = new Registro()
            //{
            //    NombrePC = "PC"+numPC,
            //    TiempoTotal = 0
            //};
            //agregar(registro);
            escuchar();
        }

        //private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        //{
        //    if (e.Mode == PowerModes.Resume)
        //    {
        //        MessageBox.Show("La computadora ha regresado de la hibernación o suspensión.");
        //    }
        //}
        public async void agregar(Registro registro)
        {
            Boolean conexion;
            do
            {
                conexion = true;
                try
                {
                    DocumentReference documento = db.Collection(correo).Document("PC" + numPC);
                    await documento.SetAsync(registro, SetOptions.Overwrite);

                }
                catch (Exception e)
                {
                    conexion = false;
                    MessageBox.Show(@"No se pudo conectar a Internet \" + e.Message);
                }
            } while (conexion == false);
        }
        public void escuchar()
        {
            Boolean conexion;
            do
            {
                conexion = true;
                try
                {
                    DocumentReference documento = db.Collection(correo).Document("PC" + numPC);
                    FirestoreChangeListener escuchar = documento.Listen(snapshot =>
                    {
                        if (snapshot.Exists)
                        {
                            Invoke((MethodInvoker)(() =>
                            {

                                Registro registro = snapshot.ConvertTo<Registro>();
                                FechaInicial0 = registro.FechaInicial;
                                estatusBloqueo = registro.Estatus;
                                estadoPC = registro.PcEstado;

                                DateTime datotime = epoch.convertirFecha(registro.FechaInicial);
                                String hora = datotime.ToString("hh:mm:ss tt");
                                inicioTime_lbl.Text = Convert.ToString(hora);
                                finalizacionTime_lbl.Text = Convert.ToString(registro.FechaFinal);
                                impresiones_lbl.Text = Convert.ToString(registro.ImpresionesNegro);
                                escaneos_lbl.Text = Convert.ToString(registro.Escaneos);
                                costoAdicional_lbl.Text = Convert.ToString("$ " + registro.CostoAdicional + " Pesos");
                                totalPagar_lbl.Text = Convert.ToString("$ " + registro.TotalPagar + " Pesos");
                                FechaInicial0 = registro.FechaInicial;
                                FechaFinal0 = registro.FechaFinal;

                                
                                bloqueo();

                                //tiempoTotal_lbl.Text = Convert.ToString(CalcularTiempo(registro.TiempoTotal));
                                //registro.ImpresionesNegro = registro.ImpresionesNegro;
                                //Console.WriteLine("Pase");
                                //Console.WriteLine(Status);

                            }));
                        }
                    });
                }
                catch (Exception e)
                {
                    conexion = false;
                    MessageBox.Show(@"No se pudo conectar a Internet \" + e.Message);
                }
            } while (conexion == false);


        }
        public void OnChangeGetAsync()
        {
            escuchar();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //registro.FechaInicial = epoch.convertirEpoch(DateTime.Now);
            //registro.Estatus = 1;
            //cliente.SetAsync("Cosmos/PC1", registro);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            //registro.FechaFinal = epoch.convertirEpoch(DateTime.Now);
            //registro.Estatus = 0;
            //int tiempoTotal = registro.FechaFinal - registro.FechaInicial;
            //TimeSpan tiempo = TimeSpan.FromSeconds(tiempoTotal);
            //registro.TiempoTotal = tiempo.ToString();
            //cliente.SetAsync("Cosmos/PC1", registro);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            //registro.ImpresionesNegro += 1;
            //cliente.SetAsync("Cosmos/PC1", registro);
        }
        private void button4_Click(object sender, EventArgs e)
        {
            //registro.Escaneos += 1;
            //cliente.SetAsync("Cosmos/PC1", registro);
        }
        private void button5_Click(object sender, EventArgs e)
        {
            //FirebaseResponse consulta = cliente.GetAsync("Cosmos/PC1").Result;
            //Registro registro = consulta.ResultAs<Registro>();
            //int tiempoTotal = registro.FechaFinal - registro.FechaInicial;
            //TimeSpan tiempo = TimeSpan.FromSeconds(tiempoTotal);
            //registro.TiempoTotal = tiempo.ToString();
            //cliente.SetAsync("Cosmos/PC1", registro);
        }
        private void button6_Click(object sender, EventArgs e)
        {
            //cliente.PushAsync("Cosmos/Registros", registro);
        }
        public void bloqueo()
        {

            if (estatusBloqueo == 1
                //&& entra == 1
                )
            {
                //registro.Estatus = 1;
                //registro.TiempoTotal = "0";
                //registro.FechaFinal = 0;
                //registro.FechaInicial = epoch.convertirEpoch(DateTime.Now);
                //agregar(registro);
                //datoTimer = 0
                //timerCalcula.Start();
                //cliente.SetAsync("Cosmos/PC1", registro);
                modoInvitado = 0;
                estatusBloqueo = 4;
                //entra = 0;
                BackgroundImage = null;
                this.panel1.Visible = true;
                WindowState = FormWindowState.Normal;
                FormBorderStyle = FormBorderStyle.Sizable;
                TopMost = false;
                timerCalcula.Enabled = true;
                tiempoTotal_lbl.Text = "00:00:00";
            }
            else if (estatusBloqueo == 2
                //&& entra == 0
                )
            {
                //registro.Estatus = 1;
                //registro.FechaFinal = epoch.convertirEpoch(DateTime.Now);
                //int tiempoTotal = registro.FechaFinal - registro.FechaInicial;
                //TimeSpan tiempo = TimeSpan.FromSeconds(tiempoTotal);
                //registro.TiempoTotal = tiempo.ToString();
                //agregar(registro);
                //datoTimer = 1;
                //timerCalcula.Stop();
                //timerCalcula.Enabled = true;
                //timerCalcula.Dispose();
                if(modoInvitado == 1){
                    tiempoTotal_lbl.Text = "0";

                    estatusBloqueo = 4;
                    inicioTime_lbl.Text = "0";
                    finalizacionTime_lbl.Text = "0";
                    BackgroundImage = null;
                    this.panel1.Visible = true;
                    WindowState = FormWindowState.Maximized;
                    FormBorderStyle = FormBorderStyle.None;
                    TopMost = true;
                    timerCalcula.Enabled = false;
                    
                }
                else if(modoInvitado == 0)
                {
                    int epochh = FechaFinal0;
                    int dato = epochh - FechaInicial0;
                    TimeSpan time = TimeSpan.FromSeconds(dato);
                    tiempoTotal_lbl.Text = time.ToString();

                    estatusBloqueo = 4;
                    DateTime datotime = epoch.convertirFecha(FechaFinal0);
                    String hora = datotime.ToString("hh:mm:ss tt");
                    finalizacionTime_lbl.Text = hora;
                    //entra = 0;
                    BackgroundImage = null;
                    this.panel1.Visible = true;
                    WindowState = FormWindowState.Maximized;
                    FormBorderStyle = FormBorderStyle.None;
                    TopMost = true;
                    timerCalcula.Enabled = false;
                }


            }
            else if (estatusBloqueo == 0
                //&& entra == 0
                )
            {
                //registro.Estatus = 0;
                //registro.TiempoTotal = 0;
                //registro.FechaFinal = 0;
                //registro.FechaInicial = 0;
                //registro.Escaneos = 0;
                //registro.ImpresionesColor = 0;
                //registro.ImpresionesNegro = 0;
                //registro.NombrePC = "";
                //registro.CostoAdicional = 0;
                //registro.TotalPagar = 0;
                //agregar(registro);
                //datoTimer = 0;
                //timerCalcula.Enabled = true;
                //timerCalcula.Dispose();
                //FechaInicial0 = 0;
                //cliente.SetAsync("Cosmos/PC1", registro);
                estatusBloqueo = 4;
                //entra = 1;
                this.panel1.Visible = false;
                WindowState = FormWindowState.Maximized;
                FormBorderStyle = FormBorderStyle.None;
                if (File.Exists(imagepath))
                {
                    BackgroundImage = Image.FromFile(imagepath);
                }
                TopMost = true;
            }
            else if (estatusBloqueo == 5)
            {
                modoInvitado = 1;
                timerCalcula.Enabled = false;
                estatusBloqueo = 4;
                BackgroundImage = null;
                this.panel1.Visible = true;
                WindowState = FormWindowState.Normal;
                FormBorderStyle = FormBorderStyle.Sizable;
                TopMost = false;
                inicioTime_lbl.Text = "0";
                finalizacionTime_lbl.Text = "0";
                tiempoTotal_lbl.Text = "00:00:00";
            }

            if (estadoPC == 1)
            {
                SystemEvents.PowerModeChanged += OnPowerModeChanged;
                Application.SetSuspendState(PowerState.Suspend, true, true);
                estadoPC= 0;
            }
            else if (estadoPC == 2)
            {
                //Process.Start("shutdown", "/s /t 0");
                Process.Start("shutdown", "/s /f /t 0");
            }


        }
        private void timerCalcula_Tick(object sender, EventArgs e)
        {
            int epochh = epoch.convertirEpoch(DateTime.Now);
            int dato = epochh - FechaInicial0;
            TimeSpan time = TimeSpan.FromSeconds(dato);
            tiempoTotal_lbl.Text = time.ToString();
        }

        public void guardaDatos()
        {


            String var;
            String var2;
            //Escribre.WriteLine(textBox1.Text);
            //Escribre.WriteLine();
            //Escribre.WriteLine(numericUpDown1.Value);
            //Escribre.Close();
            do
            {
                if (File.Exists(credencialespath))
                {
                    //MessageBox.Show("Exito", "Si existe txt");
                    TextReader Leer = new StreamReader(credencialespath);
                    correo = Leer.ReadLine();
                    Leer.ReadLine();
                    numPC = Leer.ReadLine();
                    Leer.Close();

                    if (correo == null | numPC == null | correo == "" | numPC == "")
                    {
                        TextWriter Escribre = new StreamWriter(credencialespath);
                        MessageBox.Show("Debes ingresar credenciales");
                        var = Interaction.InputBox("Ingresa el correo electrónico con el que vinculaste tu Skill de Alexa", "CORREO ELECTRONICO", "");

                        Escribre.WriteLine(var);
                        Escribre.WriteLine();
                        var2 = Interaction.InputBox("Ingresa el numero de la PC a controlar", "Numero de la computadora", "");
                        Escribre.WriteLine(var2);
                        Escribre.Close();
                    }
                }
                else
                {
                    //MessageBox.Show("No existe");
                    TextWriter Escribre = new StreamWriter(credencialespath);
                    MessageBox.Show("Debes ingresar credenciales");
                    var = Interaction.InputBox("Ingresa el correo electrónico con el que vinculaste tu Skill de Alexa", "CORREO ELECTRONICO", "");

                    Escribre.WriteLine(var);
                    Escribre.WriteLine();
                    var2 = Interaction.InputBox("Ingresa el numero de la PC a controlar", "Numero de la computadora", "");
                    Escribre.WriteLine(var2);
                    Escribre.Close();

                }
            } while (correo == null | numPC == null | correo == "" | numPC == "");


            //MessageBox.Show("Tu correo vinculado es: "+correo + "\n\nEl numero de la computadora es:" + numPC);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var sm = GetSystemMenu(Handle, false);
            EnableMenuItem(sm, SC_CLOSE, MF_BYCOMMAND | MF_DISABLED);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            var sm = GetSystemMenu(Handle, false);
            EnableMenuItem(sm, SC_CLOSE, MF_BYCOMMAND | MF_DISABLED);

            int formWidth = this.ClientSize.Width;
            int formHeight = this.ClientSize.Height;

            // Ajustar la posición de los controles (ejemplo con un Panel)
            int panelWidth = panel1.Width;
            int panelHeight = panel1.Height;

            int x = (formWidth - panelWidth) / 2;
            int y = (formHeight - panelHeight) / 2;
            panel1.Location = new System.Drawing.Point(x, y);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
