using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace Tasks_Demos
{
    class Program
    {
        static void Main2(string[] args)
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


        /// <summary>
        /// Both exceptions are thrown, but Debugger.Break() is hit only once.
        /// What is more, the exception value is not AggregateException, but InvalidTimeZoneException.
        /// This is because of new async/await which does the unwrapping into the actual exception.
        /// If you want to read other Exceptions (not only the first one), you would have to read them from the Task returned from WhenAll method call.
        /// 
        /// https://github.com/dotnet/runtime/issues/31494#issuecomment-553941135
        /// https://stackoverflow.com/questions/49403432/does-task-whenall-wait-for-all-the-tasks-in-case-of-exceptions
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static async Task Main(string[] args)
        {
           await TestTaskWhenAll();
        }

        private static async Task TestTaskWhenAll()
        {
            try
            {
                await Task.WhenAll(
                    ShortOperationAsync(),
                    LongOperationAsync()
                );
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message); // Short operation exception
                Debugger.Break();
            }
        }

        private static async Task ShortOperationAsync()
        {
            await Task.Delay(1000);
            throw new InvalidTimeZoneException("Short operation exception");

        }

        private static async Task LongOperationAsync()
        {
            await Task.Delay(5000);
            throw new ArgumentException("Long operation exception");
        }
    }
}
