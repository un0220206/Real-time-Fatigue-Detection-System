using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports; //必須引用System.IO.Ports才可以直接找到SerialPort。
using Tobii.Gaze.Core;
using System.Threading;
using System.IO;
using System.Numerics;
using Microsoft.Win32;
using System.Media;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        //protected SerialPort serialPort = new SerialPort();//定義一個序列埠元件。
        //public static Uri url;
        public static double[] L_array = new double[4];
        public static double[] O_array = new double[2];
        public static double[] O1_array = new double[2] { 0, 0 };
        public static double[] H_PressureAndCurve = new double[16];
        public static double O_PressureAndCurve = 0;
        public static double O1_PressureAndCurve = 0;
        public static String MO7 { get; set; }
        public static String C_Datetime { get; set; }
        public static String L_Datetime { get; set; }
        public static String H_Datetime { get; set; }
        public static int C_times = 0;
        public static int L_times = 0;
        public static int H_times = 0;
        public static SoundPlayer player = new SoundPlayer();
        public static SoundPlayer player1 = new SoundPlayer();
        public static SoundPlayer player2 = new SoundPlayer();
        public static Boolean Shortornot = true;
        public static String Status = "安全";
        public static Boolean LookSameOrNot = false;
        public static Boolean OpenOrNot = false;
        public static Boolean HandHaveVoice = false;
        public static Boolean CloseEyeHaveVoice = false;
        public static Boolean LookSameHaveVoice = false;
        public static Boolean IsFirstFIve = true;
        public static Boolean InFiveMinutes = false;
        public static Boolean IsFirstTwenty = false;
        public static Boolean IsSecondTwenty = false;
        public static Boolean HaveTwentyVoice = false;
        public static Boolean CantStop = false;
        public static int FiveMinutes_times = 0;
        public static String FiveMinutes { get; set; }
        public static String F_TwentyMinutes { get; set; }
        public static String S_TwentyMinutes { get; set; }
        public static String Voice1 = @"http://billor.chsh.chc.edu.tw/sound/bb.wav";
        public static String Voice2 = @"D:\\改版後完整_new\\longBB.wav";
        public static String Voice3 = @"D:\\改版後完整_new\\Parking.wav";
        public Form1()
        {
            InitializeComponent();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //callPort("COM13"); //我
            //callPort("COM3"); //郁萱
            timer.Interval = 1;
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //判斷序列埠連線狀態是否為連線中
            if (serialPort1.IsOpen)
            {
                //關閉連線
                serialPort1.Close();
            }
            //釋放序列埠元件
            serialPort1.Dispose();
        }

        //計時器元件的觸發事件
        private void timer_Tick(object sender, EventArgs e)
        {
            Uri url = null;  //判斷是否有輸入指令
            url = new EyeTrackerCoreLibrary().GetConnectedEyeTracker();
            if (Status == "安全")
            {
                label5.Text = Status;
                label5.Visible = false;
                label3.Visible = true;
            }
            else if (Status == "請專心開車-閉眼超過1秒")
            {
                label5.Text = Status;
                label5.Visible = true;
                label3.Visible = false;
            }
            else if (Status == "請專心開車-閉眼超過2秒(請按暫停)")
            {
                label5.Text = Status;
                label5.Visible = true;
                label3.Visible = false;
            }
            else if (Status == "請專心開車-凝視同一點超過2秒")
            {
                label5.Text = Status;
                label5.Visible = true;
                label3.Visible = false;
            }
            else if (Status == "請專心開車-手部微張及壓力值均為0")
            {
                label5.Text = Status;
                label5.Visible = true;
                label3.Visible = false;
            }
            else if (Status == "請專心開車-閉眼超過2秒3次(請按暫停)")
            {
                label5.Text = Status;
                label5.Visible = true;
                label3.Visible = false;
            }
            else if (Status == "請靠邊停車休息")
            {
                label5.Text = Status;
                label5.Visible = true;
                label3.Visible = false;
            }
            if (url == null)
            {
                txt_Tobii.Text = "No eye tracker found\r\n";
                return;
            }
            StreamWriter wr = File.AppendText("D:\\改版後完整_new\\眼部數據9_雨天早上.txt");
            wr.WriteLine(HaveTwentyVoice);
            wr.Close();
            IEyeTracker tracker = null;
            Thread thread = null;
            try
            {
                //---------------------------------內建功能--------------------------------------
                tracker = new EyeTracker(url);
                tracker.EyeTrackerError += EyeTrackerError;
                tracker.GazeData += EyeTrackerGazeData;
                thread = CreateAndRunEventLoopThread(tracker);
                tracker.Connect();
                // Good habit to start by retrieving device info to check that communication works.
                PrintDeviceInfo(tracker);
                //開始偵測
                tracker.StartTracking();
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                Thread.Sleep(5);
                sw.Start();
                txt_Tobii.Text += String.Format("Left eye: X: " + L_array[0].ToString() + ", Y: {0,-20} Right eye: X: " + L_array[2].ToString() + ", Y: " + L_array[3].ToString() + "\r\n", L_array[1].ToString());
                txt_Tobii.ScrollBars = ScrollBars.Vertical;
                txt_Tobii.SelectionStart = txt_Tobii.Text.Length;
                txt_Tobii.ScrollToCaret();
                sw.Stop();
                /*if (Global.zero >= 10)
                {
                    Global.zero = 0;//眨眼
                    //MaxPoint();
                    SimpleButtonMaxPoint();
                    Global.a = 0; Global.b = 0; Global.c = 0; Global.d = 0; Global.e1 = 0; Global.f = 0; Global.g = 0; Global.h = 0; Global.i = 0; Global.j = 0; Global.k = 0; Global.l = 0; Global.m = 0; Global.n = 0; Global.o = 0; Global.p = 0; Global.zero = 0;
                    SimpleButtonDirect();
                    //direct();
                    //merger();
                }*/


                //break;

                //停止偵測
                tracker.StopTracking();
                tracker.Disconnect();
            }
            catch (EyeTrackerException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (thread != null)
                {
                    tracker.BreakEventLoop();
                    thread.Join();
                }

                if (tracker != null)
                {
                    tracker.Dispose();
                }
            }
        }
        private static void EyeTrackerError(object sender, EyeTrackerErrorEventArgs e)
        {
            Console.WriteLine("ERROR: " + e.Message);
        }

        
        private static void EyeTrackerGazeData(object sender, GazeDataEventArgs e)
        {
            var gazeData = e.GazeData;
            L_array[0] = Math.Round((gazeData.Left.GazePointOnDisplayNormalized.X * 1920), 3);
            L_array[1] = Math.Round((gazeData.Left.GazePointOnDisplayNormalized.Y * 1080), 3);
            L_array[2] = Math.Round((gazeData.Right.GazePointOnDisplayNormalized.X * 1920), 3);
            L_array[3] = Math.Round((gazeData.Right.GazePointOnDisplayNormalized.Y * 1080), 3);
            DateTime myDate = DateTime.Now;
            String myDateString = myDate.ToString("yyyy-MM-dd HH:mm:ss.ff");
            //label7.Text = myDateString;
            OverTwentyMinutes(myDateString);
            if (!HaveTwentyVoice)
            {
                CloseEyes(L_array, myDateString);
                LookSame(L_array, myDateString);
            }
            /*StreamWriter wr = File.AppendText("D:\\專題測試\\眼部數據9_雨天早上.txt");
            wr.WriteLine(L_array[0] + ", " + L_array[1] + ", " + L_array[2] + ", " + L_array[3] + "," + Status + ", " + myDateString);
            wr.Close();*/
            O1_array[0] = L_array[2];
            O1_array[1] = L_array[3];
        }
        private static void OverTwentyMinutes(String myDateString)
        {
            if (IsFirstTwenty)
            {
                if ((DateTime.Parse(myDateString) - DateTime.Parse(F_TwentyMinutes)).TotalMinutes >= 0.5)
                {
                    Status = "請靠邊停車休息";
                    IsFirstTwenty = false;
                    IsSecondTwenty = true;
                    player.Stop();
                    player.SoundLocation = Voice1;
                    if (!player1.IsLoadCompleted)
                    {
                        player1.SoundLocation = Voice3;
                        player1.Load(); //同步載入聲音
                        player1.PlayLooping(); //UI執行緒同步播放
                    }
                    S_TwentyMinutes = myDateString;
                    HaveTwentyVoice = true;
                }
            }
            else if (IsSecondTwenty && !HaveTwentyVoice)
            {
                if ((DateTime.Parse(myDateString) - DateTime.Parse(S_TwentyMinutes)).TotalMinutes >= 0.5)
                {
                    Status = "請靠邊停車休息";
                    CantStop = true;
                    player.Stop();
                    player.SoundLocation = Voice1;
                    if (!player1.IsLoadCompleted || player1.SoundLocation == Voice1)
                    {
                        player1.SoundLocation = Voice3;
                        player1.Load(); //同步載入聲音
                        player1.PlayLooping(); //UI執行緒同步播放
                    }
                    HaveTwentyVoice = true;
                }
            }
        }
        private static void LookSame(double[] L_array, String myDateString)
        {
            if ((((Math.Abs(O1_array[0] - L_array[2]) <= 35.0 && Math.Abs(O1_array[1] - L_array[3]) <= 35.0) || LookSameOrNot) && !(L_array[0] == 0 && L_array[1] == 0 && L_array[2] == 0 && L_array[3] == 0)) && !CloseEyeHaveVoice)
            {
                if (L_times == 0)
                {
                    L_Datetime = myDateString;
                    O_array[0] = O1_array[0];
                    O_array[1] = O1_array[1];
                    L_times += 1;
                    LookSameOrNot = true;
                }
                else if (Math.Abs(O_array[0] - L_array[2]) <= 35.0 && Math.Abs(O_array[1] - L_array[3]) <= 35.0)
                {
                    if ((DateTime.Parse(myDateString) - DateTime.Parse(L_Datetime)).TotalSeconds >= 2.0)
                    {
                        Status = "請專心開車-凝視同一點超過2秒";
                        LookSameHaveVoice = true;
                        if (!player.IsLoadCompleted || player.SoundLocation == Voice2)
                        {
                            player.SoundLocation = Voice1;
                            player.Load(); //同步載入聲音
                            player.Play(); //UI執行緒同步播放
                        }
                    }
                }
                else if (Math.Abs(O_array[0] - L_array[2]) > 35.0 && Math.Abs(O_array[1] - L_array[3]) > 35.0 && !CloseEyeHaveVoice && LookSameHaveVoice)
                {
                    Status = "安全";
                    LookSameHaveVoice = false;
                    LookSameOrNot = false;
                    L_times = 0;
                    //player.Stop();
                    player.SoundLocation = Voice2;
                }
            }
        }
        private static void CloseEyes(double[] L_array, String myDateString)
        {
            //label7.Text = myDateString;
            //label6.Text = times.ToString();
            if (L_array[0] == 0 && L_array[1] == 0 && L_array[2] == 0 && L_array[3] == 0 && !CloseEyeHaveVoice)
            {
                if (C_times == 0)
                {
                    C_Datetime = myDateString;
                    C_times += 1;
                }
                else if ((DateTime.Parse(myDateString) - DateTime.Parse(C_Datetime)).TotalSeconds >= 2.5)
                {
                    Shortornot = false;
                    if (FiveMinutes_times == 3)
                    {
                        Status = "請專心開車-閉眼超過2秒3次(請按暫停)";
                    }
                    else
                    {
                         Status = "請專心開車-閉眼超過2秒(請按暫停)";
                    }
                    CloseEyeHaveVoice = true;
                    if (IsFirstFIve && !(IsFirstTwenty || IsSecondTwenty))
                    {
                        IsFirstFIve = false;
                        FiveMinutes = myDateString;
                        FiveMinutes_times += 1;
                        InFiveMinutes = true;
                    }
                    else if ((DateTime.Parse(myDateString) - DateTime.Parse(FiveMinutes)).TotalMinutes > 5.0)
                    {
                        IsFirstFIve = true;
                        InFiveMinutes = false;
                        FiveMinutes_times = 0;
                    }
                    //label5.Text = ((DateTime.Parse(myDateString) - DateTime.Parse(C_Datetime)).TotalSeconds).ToString();
                    if (!player.IsLoadCompleted || player.SoundLocation == Voice1)
                    {
                        if (FiveMinutes_times == 3)
                        {
                            player.SoundLocation = Voice3;
                            player.Load(); //同步載入聲音
                            player.PlayLooping(); //UI執行緒同步播放
                            F_TwentyMinutes = myDateString;
                            IsFirstTwenty = true;
                            InFiveMinutes = false;
                        }
                        else
                        {
                            player.SoundLocation = Voice2;
                            player.Load(); //同步載入聲音
                            player.PlayLooping(); //UI執行緒同步播放
                        }
                    }
                }
                else if ((DateTime.Parse(myDateString) - DateTime.Parse(C_Datetime)).TotalSeconds >= 0.8)
                {
                    Status = "請專心開車-閉眼超過1秒";
                    Shortornot = true;
                    //label5.Text = ((DateTime.Parse(myDateString) - DateTime.Parse(C_Datetime)).TotalSeconds).ToString();
                    if (!player.IsLoadCompleted || player.SoundLocation == Voice2)
                    {
                        player.SoundLocation = Voice1; //shortBB
                        player.Load(); //同步載入聲音
                        player.Play(); //UI執行緒同步播放
                    }
                }
            }
            else if ((Shortornot == true && !CloseEyeHaveVoice) && !LookSameHaveVoice)
            {
                Status = "安全";
                C_times = 0;
                player.Stop();
                player.SoundLocation = Voice2;
            }
            else
            {
                C_times = 0;
            }
            //label3.Text = C_Datetime;
        }
        private static void PrintDeviceInfo(IEyeTracker tracker)
        {
            var info = tracker.GetDeviceInfo();
            Console.WriteLine("Serial number: {0}", info.SerialNumber);
            var trackBox = tracker.GetTrackBox();
        }

        private static Thread CreateAndRunEventLoopThread(IEyeTracker tracker)
        {
            var thread = new Thread(() =>
            {
                try
                {
                    tracker.RunEventLoop();                                       //執行迴圈
                }
                catch (EyeTrackerException ex)
                {
                    Console.WriteLine("An error occurred in the eye tracker event loop: " + ex.Message);
                }
                Console.WriteLine("Leaving the event loop.");
            });

            thread.Start();
            return thread;
        }
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                DateTime myDate = DateTime.Now;
                String myDateString = myDate.ToString("yyyy-MM-dd HH:mm:ss.ff");
                
                //StreamWriter wr = File.AppendText("D:\\2.txt"); //郁萱
                String s = serialPort1.ReadLine();
                MO7 = s;
                //H_PressureAndCurve = s.Split(',');
                int count = 0;
                if (s.Split(',').Length == 16)
                {
                    foreach (String ss in s.Split(','))
                    {
                        H_PressureAndCurve[count] = Double.Parse(ss);
                        count++;
                    }
                }
                
                double x = O1_PressureAndCurve - H_PressureAndCurve[4];
                if (!HaveTwentyVoice)
                {
                    if (((x < 10 && x >= 3) || OpenOrNot) && !CloseEyeHaveVoice)
                    {
                        if (H_times == 0)
                        {
                            H_Datetime = myDateString;
                            O_PressureAndCurve = O1_PressureAndCurve;
                            H_times += 1;
                            OpenOrNot = true;
                        }
                        else if ((Math.Abs(O_PressureAndCurve - H_PressureAndCurve[4]) >= 25.0 || O_PressureAndCurve < H_PressureAndCurve[7]) && HandHaveVoice)
                        {
                            Status = "安全";
                            H_times = 0;
                            label5.Visible = false;
                            label3.Visible = true;
                            OpenOrNot = false;
                            HandHaveVoice = false;
                            //player.Stop();
                            player.SoundLocation = Voice2;
                        }
                        else if ((DateTime.Parse(myDateString) - DateTime.Parse(H_Datetime)).TotalSeconds <= 2.0)
                        {
                            if (O_PressureAndCurve - H_PressureAndCurve[4] >= 12.0 && IsZero(H_PressureAndCurve))
                            {
                                Status = "請專心開車-手部微張及壓力值均為0";
                                HandHaveVoice = true;
                                label5.Visible = true;
                                label3.Visible = false;
                                if (!player.IsLoadCompleted || player.SoundLocation == Voice2)
                                {
                                    player.SoundLocation = Voice1;
                                    player.Load(); //同步載入聲音
                                    player.Play(); //UI執行緒同步播放
                                }
                            }
                            else if ((DateTime.Parse(myDateString) - DateTime.Parse(H_Datetime)).TotalSeconds > 2.0 && !IsZero(H_PressureAndCurve) && HandHaveVoice)
                            {
                                Status = "安全";
                                H_times = 0;
                                label5.Visible = false;
                                label3.Visible = true;
                                OpenOrNot = false;
                                HandHaveVoice = false;
                                //player.Stop();
                                player.SoundLocation = Voice2;
                            }
                        }
                    }
                    O1_PressureAndCurve = Convert.ToDouble(H_PressureAndCurve[4]);
                    /*StreamWriter wr = File.AppendText("D:\\專題測試\\手部壓力9_雨天早上.txt"); //我
                    //wr.WriteLine(MO7 + H_times + ", " + H_Datetime + ", " + O_PressureAndCurve + ", " + Status + ", " + myDateString);
                    foreach (double aa in H_PressureAndCurve)
                    {
                        wr.Write(aa + ",");
                    }
                    wr.WriteLine(Status + ", " + myDateString);
                    wr.Close();*/
                }
            }
            catch { }
        }
        private void HandVoice(String[] H_PressureAndCurve, String myDateString)
        {
            
        }
        private static Boolean IsZero(double[] H_PressureAndCurve)
        {
            if (Convert.ToDouble(H_PressureAndCurve[1]) == 0 && Convert.ToDouble(H_PressureAndCurve[2]) == 0 && Convert.ToDouble(H_PressureAndCurve[3]) == 0 && 
                Convert.ToDouble(H_PressureAndCurve[5]) == 0 && Convert.ToDouble(H_PressureAndCurve[6]) == 0 && Convert.ToDouble(H_PressureAndCurve[8]) == 0 && 
                Convert.ToDouble(H_PressureAndCurve[9]) == 0 && Convert.ToDouble(H_PressureAndCurve[11]) == 0 && Convert.ToDouble(H_PressureAndCurve[12]) == 0 &&
                Convert.ToDouble(H_PressureAndCurve[14]) == 0 && Convert.ToDouble(H_PressureAndCurve[15]) == 0)
            {
                return true;
            } else
            {
                return false;
            }
            /*if (H_PressureAndCurve.Sum() == 0)
            {
                return true;
            }
            else
            {
                return false;
            }*/
        }
        private void callPort(String x)
        {
            serialPort1.Close();
            serialPort1.PortName = x;
            serialPort1.BaudRate = 9600;
            serialPort1.Open();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                //將序列埠元件所讀取到的資料行再加上一個換行符號，然後交給txt_monitor顯示。
                txt_monitor2.Text += MO7 + System.Environment.NewLine;
                txt_monitor2.ScrollBars = ScrollBars.Vertical;
                txt_monitor2.SelectionStart = txt_monitor2.Text.Length;
                txt_monitor2.ScrollToCaret();
            }
            catch { }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (((Shortornot == false && CloseEyeHaveVoice) || HaveTwentyVoice) && !CantStop)
            {
                Status = "安全";
                CloseEyeHaveVoice = false;
                C_times = 0;
                player.Stop();
                player.SoundLocation = Voice1;
                if (InFiveMinutes)
                {
                    FiveMinutes_times += 1;
                } else
                {
                    FiveMinutes_times = 0;
                }
                if (!CantStop)
                {
                    HaveTwentyVoice = false;
                    player1.Stop();
                    player1.SoundLocation = Voice1;
                }
            }
        }
    }
}
