using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI;

namespace WebApplication1
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Write($"<p>");

            // 使用 ASP.NET 4.5 所提供的 RegisterAsyncTask 註冊非同步方法
            // 這行不會等到非同步執行完畢，而會直接往下執行！
            //RegisterAsyncTask(new PageAsyncTask(DoWorkAsync));

            // 千萬不要用 Fire and Forget 的方式執行非同步方法
            //DoWorkAsync();

            Response.Write($"</p>");
        }

        private Task DoWorkAsync()
        {
            List<Task> tasks = new List<Task>();

            //Thread.Sleep(5000);
            var sc = SynchronizationContext.Current;

            for (int i = 0; i < 100; i++)
            {
                var j = i;
                tasks.Add(Task.Run(() =>
                {
                    sc.Send((d) =>
                    {
                        Response.Write($"<pre>{j} {Thread.CurrentThread.ManagedThreadId}</pre>");
                    }, null);
                }));
            }

            return Task.WhenAll(tasks);
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // 非同步工作 (僅回傳 Task 而已) (一定不會 Deadlock)
            // 非同步方法 (標示為 async 的方法) (因為編譯器產生的結果會導致 SynchorizationContext 而產生 Deadlock)

            // 完整使用 async/await 是最沒有問題的非同步呼叫方法
            // 只要不混用 async/await 與 .Result/.Wait()/.WaitAll() 方法，就不會有 Deadlock 問題發生！
            //RegisterAsyncTask(new PageAsyncTask(FillTextBox1Async));

            // 由於 http.GetStringAsync() 並不是一個用 async 宣告的非同步方法，而是一個回傳 Task 的方法，這種寫法並不會有 Deadlock 問題發生！
            // 因為 async/await 透過編譯器所產生的程式碼，會保存目前執行緒正在使用的 SynchorizationContext 物件，並在等待後繼續執行時還原，這個過程可能會產生 Deadlock 問題！
            // 因此，只要你所呼叫的方法沒有使用 async/await 的話，就不會有 Deadlock 的問題。
            //RegisterAsyncTask(new PageAsyncTask(FillTextBox2Async));

            // 由於 http.GetStringAsync() 並不是一個用 async 宣告的非同步方法，當呼叫 task.Result 的時候，是以「同步」的方式執行，因此不會有 Deadlock 的問題！
            // 當你沒有使用 async/await 語法，也意味著不會用到 SynchorizationContext 物件，因此也不可能會產生 Deadlock 問題！
            //RegisterAsyncTask(new PageAsyncTask(FillTextBox3Async));

            // 在任何非同步的執行環境下(無論該方法是否用 async 宣告)，呼叫一個使用 async 宣告的「非同步方法」，並使用「同步」的 API 進行呼叫，就可能產生 Deadlock 問題！
            //RegisterAsyncTask(new PageAsyncTask(FillTextBox4Async));

            // 在任何非同步的執行環境下(無論該方法是否用 async 宣告)，呼叫一個使用 async 宣告的「非同步方法」，並使用「同步」的 API 進行呼叫，就可能產生 Deadlock 問題！
            //RegisterAsyncTask(new PageAsyncTask(FillTextBox5Async));

            // 在任何非同步的執行環境下(無論該方法是否用 async 宣告)，呼叫一個使用 async 宣告的「非同步方法」，並使用「同步」的 API 進行呼叫，就可能產生 Deadlock 問題！
            //RegisterAsyncTask(new PageAsyncTask(FillTextBox6Async));
        }

        private async Task FillTextBox1Async()
        {
            using (var http = new HttpClient())
            {
                var result = await http.GetStringAsync("https://docs.microsoft.com/");

                TextBox1.Text = result;
            }
        }

        private Task FillTextBox2Async()
        {
            var tcs = new TaskCompletionSource<string>();

            using (var http = new HttpClient())
            {
                var result = http.GetStringAsync("https://docs.microsoft.com/");

                TextBox1.Text = result.Result;

                tcs.TrySetResult(result.Result);
            }

            return tcs.Task;
        }

        private async Task<string> FillTextBox3Async()
        {
            await Task.Delay(1000);

            using (var http = new HttpClient())
            {
                var result = http.GetStringAsync("https://docs.microsoft.com/");

                TextBox1.Text = result.Result;
         
                return TextBox1.Text;
            }
        }

        private Task FillTextBox4Async()
        {
            DoSomethingAsync().Wait();

            var tcs = new TaskCompletionSource<string>();
            tcs.TrySetResult("");
            return tcs.Task;
        }

        private Task FillTextBox5Async()
        {
            DoSomethingAsync().Wait();

            return Task.Delay(1000);
        }

        #pragma warning disable AsyncFixer01 // Unnecessary async/await usage
        private async Task FillTextBox6Async()
        {
            #pragma warning disable AsyncFixer02 // Long running or blocking operations under an async method
            DoSomethingAsync().Wait();
            #pragma warning restore AsyncFixer02 // Long running or blocking operations under an async method

            await Task.Delay(1000);
        }
        #pragma warning restore AsyncFixer01 // Unnecessary async/await usage

        #pragma warning disable AsyncFixer01 // Unnecessary async/await usage
        private static async Task DoSomethingAsync()
        {
            await Task.Delay(1000);
        }
        #pragma warning restore AsyncFixer01 // Unnecessary async/await usage
    }
}