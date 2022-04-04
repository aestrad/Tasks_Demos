using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace Tasks_Demos
{
    class Program
    {
        static void Main(string[] args)
        {
            int failed = 0;
            var tasks = new List<Task>();
            String[] urls = { "www.google.com", "www.jw.org",
                        "www.docs.microsoft.com", "www.github.com",
                        "www.contoso.com" };

            foreach (var value in urls)
            {
                var url = value;
                tasks.Add(Task.Run(() => {
                    var png = new Ping();
                    try
                    {
                        var reply = png.Send(url);
                        if (!(reply.Status == IPStatus.Success))
                        {
                            Interlocked.Increment(ref failed);

                            throw new TimeoutException("Unable to reach " + url + ".");
                        }
                    }
                    catch (PingException)
                    {

                        Interlocked.Increment(ref failed);
                        throw;
                    }
                }));
            }
            Task t = Task.WhenAll(tasks);

        
            try
            {
                t.Wait();
            }
            catch (Exception e){
                Console.WriteLine(e.Message);
            }

            /// check status for faulted tasks
            if (t.Status == TaskStatus.RanToCompletion)
                Console.WriteLine("All ping attempts succeeded.");
            else if (t.Status == TaskStatus.Faulted)
                Console.WriteLine("{0} ping attempts failed, Message {1}", failed, t.Exception.Message);
        }
    }
}
