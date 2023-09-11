using lab2_WebRequest;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Lab
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /*
            // 쓰레드
            Lab1_thread.Room1 r1 = new Lab1_thread.Room1();
            r1.TestStart();*/

            /*
            //웹 이미지 다운로드
            //다운받을 대상 이미지
            string url1 = "https://www.google.com/images/branding/googlelogo/1x/googlelogo_color_272x92dp.png";
            //바탕화면 경로에 test.png로 다운받은 이미지 생성
            string fname = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)+"/test.png";

            HttpWebRead hw = new HttpWebRead();
            //hw.WebDownload(url1, fname);
            Task t = hw.webreading2(url1, fname);
            t.Wait();*/

            /*
            //task test
            lab3_task.Room1 case1 = new lab3_task.Room1();
            case1.start();
            */

            //서버
            //clinet2 정해진 데이터 한번 보내기
            //client1 원하는 데이터 한번 보내기
            //서버 하나에 클라이언트 2개 열었을 때 서버가 정상적으로 받는지 테스트
            //시작 프로젝트 구성을 Lab다음에 Client1과 Client2가 실행되도록 바꿔줘야함.
            /*lab4_server.server_micro server = new lab4_server.server_micro();
            server.main();*/

            //c# operator test
            //Lab7_Base.Operator.TestStart();
        }
    }
}
/// <summary>
/// 쓰레드
/// </summary>
namespace Lab1_thread
{


    class Room1
    {
        public void TestStart()
        {
            Thread thread1 = new Thread(Work);
            thread1.Name = "1";
            Thread thread2 = new Thread(Work);
            thread2.Name = "2";
            Thread thread3 = new Thread(Work);
            thread3.Name = "3";

            Console.WriteLine(thread1.ThreadState == ThreadState.Running);
            thread1.Start();
            thread1.Join();
            thread2.Start();
            thread2.Interrupt();
            thread2.Join();
            Console.WriteLine((int)thread1.ThreadState + " " + (int)thread2.ThreadState);
            Console.WriteLine((thread1.ThreadState & ThreadState.Stopped));
            /*
            thread2.Join();
            thread3.Start();*/

        }
        public void Work()
        {
            try
            {
                int i = 0;
                for (; i < 100; i++)
                {
                    Thread.Sleep(1);
                    Console.WriteLine("Work ID : " + Thread.CurrentThread.Name + "\tWorking " + i);

                }
            }
            catch (ThreadInterruptedException e)
            {
                Console.WriteLine(e.Message);
            }


        }
    }


}
namespace lab2_WebRequest
{

    class HttpWebRead
    {
        private static readonly HttpClient client = new HttpClient();
        static HttpWebRead()
        {
            client = new HttpClient();
        }
        ~HttpWebRead()
        {
            Console.WriteLine("dd");
            client.Dispose();
        }
        /// <summary>
        /// 웹 이미지 다운로드
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fname"></param>
        /// <returns></returns>
        public async Task webreading2(string url, string fname)
        {
            int isready = 9999;
            byte[] buffer = new byte[1000];

            using (HttpResponseMessage response = await client.GetAsync(url))
            {


                try
                {
                    response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode)
                    {
                        //응답객체에서 스트림 받아오기
                        using (Stream strreader = await response.Content.ReadAsStreamAsync())
                        {
                            //Console.WriteLine(strreader.Length);
                            //출력파일 생성

                            using (FileStream fs = new FileStream(fname, FileMode.OpenOrCreate))
                            {

                                while (isready != 0)
                                {
                                    await Task.Delay(1000);
                                    //스트림에서 버퍼 크기만큼 처음부터 읽어오기
                                    await strreader.ReadAsync(buffer, 0, buffer.Length);
                                    //출력 파일에 버퍼만큼 쓰기
                                    await fs.WriteAsync(buffer, 0, buffer.Length);
                                    isready = (int)(strreader.Length - strreader.Position);
                                    Console.WriteLine(strreader.Position);
                                }
                                //Console.WriteLine(fs.Length);
                            }

                        }
                    }
                    Console.WriteLine("파일 다운로드 완료");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
        }
        /// <summary>
        /// 공유기 안 로컬 ip
        /// </summary>
        public static void GetIP()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            for (int i = 0; i < host.AddressList.Length; i++)
            {
                Console.WriteLine(host.AddressList[i].ToString());
                if (host.AddressList[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    //Console.WriteLine(host.AddressList[i].ToString());
                    //return host.AddressList[i].ToString();
                }
            }
        }


        public void WebDownload(string url, string fname)
        {
            //버퍼
            byte[] buffer = new byte[4096];
            //스트림에서 읽어 들여야할 데이터 크기. 0이되면 다 읽음
            int isready = 9999;
            //홈페이지 주소 설정.
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.110 Safari/537.36";

            //홈페이지로부터 응답객체 받아오기
            using (HttpWebResponse getresponse = (HttpWebResponse)request.GetResponse())
            {
                //서버에서 응답 성공시
                if (getresponse.StatusCode == HttpStatusCode.OK)
                {
                    //응답객체에서 스트림 받아오기
                    using (Stream strreader = getresponse.GetResponseStream())
                    {
                        //출력파일 생성
                        using (Stream output = File.OpenWrite(fname))
                        {
                            //읽어들여야할 데이터가 있는가
                            while (isready != 0)
                            {
                                //스트림에서 버퍼 크기만큼 처음부터 읽어오기
                                isready = strreader.Read(buffer, 0, buffer.Length);
                                //출력 파일에 버퍼만큼 쓰기
                                output.Write(buffer, 0, isready);
                            }
                            Console.WriteLine("파일 다운로드 완료");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("서버 응답 실패. 사유: " + getresponse.StatusCode);
                }

            }

        }
    }
    class WebRead
    {
        /// <summary>
        /// html 가져오기
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string WebClinetReading(string url)
        {
            WebClient web = new WebClient();
            string str = "";
            using (Stream stream = web.OpenRead(url))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    str = reader.ReadToEnd();
                }
            }
            return str;
        }
        public string WebClinetDownload(string url)
        {
            WebClient web = new WebClient();
            string str = web.DownloadString(url);
            return str;
        }
        public void WebClinetDownload(string url, string fname)
        {
            WebClient web = new WebClient();
            web.DownloadFile(url, fname);
        }
        /// <summary>
        /// 인터넷에서 가져온 것
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool HttpWebRequestDownload(string uri, string fileName)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            bool bImage = response.ContentType.StartsWith("image",
                StringComparison.OrdinalIgnoreCase);
            if ((response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Moved ||
                response.StatusCode == HttpStatusCode.Redirect) &&
                bImage)
            {
                using (Stream inputStream = response.GetResponseStream())
                using (Stream outputStream = File.OpenWrite(fileName))
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    do
                    {
                        bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                        outputStream.Write(buffer, 0, bytesRead);
                    } while (bytesRead != 0);
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }

    class base64
    {
        public static string encoding(string text, System.Text.Encoding mode = null)
        {
            if (mode == null)
            {
                mode = System.Text.Encoding.UTF8;
            }
            byte[] data = mode.GetBytes(text);
            return Convert.ToBase64String(data);
        }
        public static string decodeing(string text, System.Text.Encoding mode = null)
        {
            if (mode == null)
            {
                mode = System.Text.Encoding.UTF8;
            }
            byte[] data = Convert.FromBase64String(text);
            return mode.GetString(data);
        }
    }
}
/// <summary>
/// 태스크
/// </summary>
namespace lab3_task
{
    class Room1
    {
        /// <summary>
        /// task 실험 시작
        /// </summary>
        public void start()
        {
            /*
            //태스크 생성 및 비동기 작업 시작
            Task t=taskmain1();
            //태스크가 끝날 때까지 동기적으로 기다림.
            t.Wait();

            //두번째 실험 시작.
            taskmain2();
            */

            /*
            Task t= taskmain3();
            t.Wait();
            */
            taskmain3();
            Task.Delay(6000).Wait();
        }
        /// <summary>
        /// 함수 자체를 비동기 키워드 사용 가능
        /// </summary>
        /// <returns></returns>
        public static async Task taskmain1()
        {
            //태스크 리스트로 태스크 관리
            List<Task> tasklist = new List<Task>();
            tasklist.Add(work1());
            tasklist.Add(work2());
            tasklist.Add(work3());

            //태스크를 하나 끝날 때까지 기다린 다음 제거하는 것을 리스트가 모두 비워질 때 까지 반복
            while (tasklist.Count > 0)
            {
                //태스크 중 하나라도 작업이 끝나면 할당
                Task finish = await Task.WhenAny(tasklist);
                //끝난 작업 리스트에서 제거
                tasklist.Remove(finish);
            }
            Console.WriteLine("all done");
        }
        //비동기 키워드 없음
        public void taskmain2()
        {
            //태스크 리스트로 태스크 관리
            List<Task> tasklist = new List<Task>();
            tasklist.Add(work1());
            tasklist.Add(work2());
            tasklist.Add(work3());
            //태스크가 모두 끝나면 반환
            Task working = Task.WhenAll(tasklist.ToArray());
            //작업이 끝날 때까지 대기
            working.Wait();

            Console.WriteLine("all done");
        }

        public async void taskmain3()
        {
            CancellationTokenSource cancelsource = new CancellationTokenSource();
            CancellationToken t = cancelsource.Token;
            //시작 후 바로 기다리는 형식
            //Task<int> te = await Task.Factory.StartNew( (object s)=> { return  work4(s); },t);
            //람다식 없이 사용
            //Task<int> te = Task.Factory.StartNew(work5, t);
            //await te;
            Task<int> te = await Task.Factory.StartNew((object s) => { return work4(s); }, t, t);

            await Task.Delay(2000);

            cancelsource.Cancel();
            while (te.IsCompleted)
            {
                te.Wait();
            }

            Console.WriteLine(te.IsCompleted);
            int i2 = te.Result;

            Console.WriteLine(i2);
        }
        public async void taskmain4()
        {
            CancellationTokenSource cancelsource = new CancellationTokenSource();
            CancellationToken t = cancelsource.Token;
            Task<int> te = Task.Factory.StartNew(work5, t);
            Task.Delay(3000).Wait();


            cancelsource.Cancel();
            Task.Delay(2000).Wait();
            int i2 = await te;
            Console.WriteLine(i2);
            /*
            canceltokensouce = new CancellationTokenSource();
            CancellationToken token = canceltokensouce.Token;
            Task t = Task.Factory.StartNew(async ()=> { await work4(token); }, token);
            canceltokensouce.Cancel();
            await Task.Delay(10000);
            */
        }
        public static async Task work1()
        {
            await Task.Delay(1000);
            Console.WriteLine("work1 done");
        }
        public static async Task work2()
        {
            await Task.Delay(3000);
            Console.WriteLine("work2 done");
        }
        public static async Task work3()
        {
            await Task.Delay(2000);
            Console.WriteLine("work3 done");
        }
        public static async Task<int> work4(object state)
        {
            CancellationToken t = (CancellationToken)state;
            int re = 0;
            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(500);
                Console.WriteLine("work4 working");
                /*
                //work cancle?
                if (t.IsCancellationRequested)
                {
                    Console.WriteLine("work4 cancle");
                    re = 0;
                    break;
                }
                else
                {
                    Console.WriteLine("working");
                    re = 1;
                }*/
            }
            Console.WriteLine("work4 end");
            return re;
        }
        public static int work5(object state)
        {
            CancellationToken t = (CancellationToken)state;
            int re = 0;
            for (int i = 1; i < 10; i++)
            {
                Thread.Sleep(500);
                //work cancle?
                if (t.IsCancellationRequested)
                {
                    Console.WriteLine("work5 cancle");
                    re = 0;
                    break;
                }
                else
                {
                    Console.WriteLine("working");
                    re = 1;
                }
            }
            Console.WriteLine("work5 end");
            return re;
        }
        public void workend()
        {
            Console.WriteLine("task end");
        }
    }
}

namespace lab4_server
{
    public class Stateobj
    {
        public Socket sk = null;
        public int id;
    }
    class server
    {
        static async Task server1()
        {
            int maxsize = 1024;
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 11000);
            sock.Bind(ep);
            sock.Listen(10);
            while (true)
            {
                Socket clientsock = await Task.Factory.FromAsync(sock.BeginAccept, sock.EndAccept, null);

                byte[] buff = new byte[maxsize];
                int ncount = await Task.Factory.FromAsync<int>(clientsock.BeginReceive(buff, 0, buff.Length, SocketFlags.None, null, clientsock), clientsock.EndReceive);
                if (ncount > 0)
                {
                    string msg = Encoding.ASCII.GetString(buff, 0, ncount);
                    Console.WriteLine(msg);
                    await Task.Factory.FromAsync(clientsock.BeginSend(buff, 0, buff.Length, SocketFlags.None, null, clientsock), clientsock.EndSend);

                }
                clientsock.Close();
            }
        }
    }

    #region micro
    public class serverstate
    {
        public const int receivebuffersize = 10;
        public byte[] receivebuffer = new byte[receivebuffersize];
        public StringBuilder receivedsb = new StringBuilder();
        public Socket clientworksk = null;
        public int id;
    }
    class server_micro
    {
        bool serveron = true;
        // 방화벽 인바운드 규칙 - lab1 
        //대기중인 쓰레드 모두 통과시키고 추가로 오는 쓰레드는 대기상태로 둠.
        //프로그래머가 직접 대기상태로 두는 시점을 설정해야함.
        public static ManualResetEvent alldone = new ManualResetEvent(false);
        public void main()
        {
            //내 아이피 정보를 dns에서 가져온다.
            IPHostEntry iphostinfo = Dns.GetHostEntry(Dns.GetHostName());
            //ip정보에서 메인 ip를 가져온다.(공인 아이피, 공유기 이기 떄문에 로컬 아이피를 가져온다.)
            IPAddress ipadress = iphostinfo.AddressList[0];
            //IPAddress.Any
            //ip에 11000포트
            IPEndPoint ep = new IPEndPoint(ipadress, 80);
            //ip에 tcp연결형으로 소켓 생성
            Socket sk1 = new Socket(ipadress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                //소켓에 포트 바인딩
                sk1.Bind(ep);
                //수신 가능 상태
                sk1.Listen(100);
                waitasecond();
                while (serveron)
                {
                    //목록에서 가져오기 차단
                    alldone.Reset();
                    serverstate s = new serverstate();
                    s.clientworksk = sk1;
                    //통신 시작.
                    sk1.BeginAccept(new AsyncCallback(AcceptCallback), sk1);
                    //다음 신호까지 대기
                    alldone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();

            sk1.Close();
        }
        public async void waitasecond()
        {
            await Task.Delay(15000);
            serveron = false;
            alldone.Set();
        }
        public static void AcceptCallback(IAsyncResult ar)
        {
            //하나 가져오기
            alldone.Set();

            //소켓 가져오기
            Socket sk = (Socket)ar.AsyncState;
            Socket skhandler = sk.EndAccept(ar);
            serverstate state = new serverstate();
            state.clientworksk = skhandler;
            //1. 버퍼 2. 버퍼배열에 쓰기 시작할 위치 3. 버퍼에 쓸 크기 4. 소켓 옵션. 5. 콜백 메서드 6. 추가 정보
            //따로 포지션 필요없이 자기가 알아서 저번에 받은 위치부터 계속 받아 온다.
            skhandler.BeginReceive(state.receivebuffer, 0, serverstate.receivebuffersize, 0, new AsyncCallback(readcallback), state);
        }

        public static void readcallback(IAsyncResult ar)
        {
            string content = string.Empty;
            serverstate state = (serverstate)ar.AsyncState;
            Socket handler = state.clientworksk;
            int byteread = handler.EndReceive(ar);
            if (byteread != 0)
            {
                Console.WriteLine(handler.LocalEndPoint);
            }
            if (byteread > 0)
            {

                //Console.WriteLine(byteread);
                //Console.WriteLine(Encoding.ASCII.GetString(state.receivebuffer, 0, byteread));
                //받은 데이터 추가.
                state.receivedsb.Append(Encoding.ASCII.GetString(state.receivebuffer, 0, byteread));

                content = state.receivedsb.ToString();
            }
            //끝이라면. 종료조건
            //<EOF>가 string 배열 0보다 더 전에 위치 하지 않는다면
            if (content.IndexOf("<EOF>") > -1)
            {
                // 클라이언트에게 읽은 데이터를 콘솔에 띄운다.
                Console.WriteLine("소켓으로부터 {0} 바이트 읽음. \n 받은 데이터 : {1}", content.Length, content);
                // 클라이언트에게 같은 데이터를 보낸다.

                send(handler, content);
            }
            else
            {
                //Console.WriteLine("기다려! 더 읽어올깨~");
                // 모든 데이터를 수신받지 못하면 추가로 받는다.
                // 종료조건이 만족하지 못했으므로 계속 받는다.
                handler.BeginReceive(state.receivebuffer, 0, serverstate.receivebuffersize, 0, new AsyncCallback(readcallback), state);
            }
        }
        public static void send(Socket handler, string data)
        {
            byte[] bytedata = Encoding.ASCII.GetBytes(data);
            Console.WriteLine(data + "send");
            handler.BeginSend(bytedata, 0, bytedata.Length, 0, new AsyncCallback(sendcallback), handler);
        }
        private static void sendcallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;
                int bytesent = handler.EndSend(ar);
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
    #endregion micro
}

namespace Lab7_Base
{
    public class Indexer
    {
        public static void TestStart()
        {
            Indexer i1 = new Indexer();
            for (int i = 0; i < i1.Length; i++)
            {
                i1[i] = i;
                Console.WriteLine(i1[i]);
            }

        }
        int[] data = new int[10];
        public int Length
        {
            get { return data.Length; }
        }
        public int this[int index]
        {
            get
            {
                return data[index];
            }
            set
            {
                data[index] = value;
            }
        }
    }

    public class GeterSeterInit
    {
        public static void TestStart()
        {
            GeterSeterInit t1 = new GeterSeterInit();
            t1.AutoPropertiesValue = "AutoPropertiesValue";
            Console.WriteLine(t1.AutoPropertiesValue);
            Console.WriteLine(t1.FinalProperties);
            Console.WriteLine(t1.FinalProperties2);
            Console.WriteLine($"{t1.FinalProperties3} 이 속성은 null이 찍히며 init문 안에서만 초기화한건 속성을 호출 전까지는 초기화하지 않는다.");

        }
        private string FinalValue;
        private string FinalValue2;

        public string AutoPropertiesValue
        {
            get; set;
        }
        public string FinalProperties
        {
            get; init;
        }
        public string FinalProperties2
        {
            get { return FinalValue; }
            init { FinalValue = value; }
        }
        public string FinalProperties3
        {
            get { return FinalValue2; }
            init { FinalValue2 = "Final Properties 2"; }
        }
        public GeterSeterInit()
        {
            FinalProperties = "Final Properties 1";
            FinalProperties2 = "Final Properties 2";
        }
        /* 이미 getter가 있기 때문에 오류남.
         * 즉 getter는 컴파일러가 get_함수이름의 함수를 자동으로 생성하는 것과 같음
        public string get_FinalProperties2()
        {
            return FinalValue;
        }*/
    }
    public class Operator
    {
        public static void TestStart()
        {
            Operator o1 = new Operator(1, 1);
            o1++;
            Operator o2 = o1;
            o2++;
            Console.WriteLine($"o1 :{o1} \t  o2 : {o2}");
            Console.WriteLine($"o1+o2 : {o1 + o2}");
        }
        int x;
        int y;
        public Operator(int i1, int i2)
        {
            x = i1;
            y = i2;
        }
        public static Operator operator +(Operator o1, Operator o2)
        {
            return new Operator(o1.x + o2.x, o1.y + o2.y);
        }
        public static Operator operator ++(Operator o1)
        {
            return new Operator(o1.x + 1, o1.y + 1);
        }
        public override string ToString()
        {
            return $"x: {x} y:{y}";
        }
    }
}