betaseries library usage
========================


Choosing the right assembly
---------------------------

There are 3 different assemblies. You have to choose the right one for your project.

 * `Srk.!BetaSeriesApi.dll` is for Microsoft .NET 3.5 (or higher) projects (we use LINQ to XML, so 3.5 minimum)
 * `SrkSL.!BetaSeriesApi.dll` is for Silverlight 4 projects (we are thinking about making it a silverlight 3 assembly)
 * `SrkWP7.!BetaSeriesApi.dll` is for Windows Phone 7 projects
 * `SrkWM.!BetaSeriesApi.dll` is for Windows Mobile 6 projects

Actually, each assembly implements the same methods because most code files are linked from the .NET projects to the others. 

Instantiating API clients
-------------------------

### Theory

To use the API with this library, you have to instantiate a client class. For a better architecture, we use interfaces to abstract the way our client classes are made.

In the .NET assembly, there are 3 interfaces:

 * `IBetaseriescApi`  contains synchronous and asynchronous methods. This is the one you will frequently use.
 * `IBetaseriesSyncApi` contains synchronous methods
 * `IBetaseriesAsyncApi`  contains asynchronous methods

In Silverlight and WP7 assemblies, there is just one interface because Silverlight does not support synchronous HTTP calls:

 * `IBetaseriesApi` contains asynchronous methods

Now, the API supports XML and JSON for data transfer. We do only support XML with our `BetaseriesXmlClient` class (which implements `IBetaseriesSyncAsyncApi`). If you want, you can create a `BetaseriesJsonClient` inheriting from `BetaseriesBaseHttpClient` (but there is no real advantage of doing this). 

As you understood, to use the API, you need instances of `BetaseriesXmlClient`. There are 2 ways of instantiating this class.

 * If you use a `BetaseriesClientFactory` object, advantages are:
   * 1 line of code to set your API key
   * session token sharing between all instances of `BetaseriesXmlClient` (in fact, all instances of `BetaseriesBaseHttpClient`)
   * 1 line client instanciation with no configuration/parameters
   * you can create the factory from your configuration file with one line instead of setting configuration by code
 * If you instantiate directly a `BetaseriesXmlClient`:
   * you have to set your API key on each instance
   * you have to set your userAgent on each instance
   * you have to set the session token on each instance at login/logout
   * if your application only needs one instance, it's okay

Now, I recommend to use the factory class which is pretty small and has no consequences on your application. Only 2 lines of code are necessary.


	// at application startup, you have to create 1 factory
	BetaseriesClientFactory.Default = new BetaseriesClientFactory("betaseriesapikey", "MyBetaApplication/1.0.0.0", true);
	
	// Then anywhere in your application you can get a new client object
	IBetaseriesApi client = BetaseriesClientFactory.Default.CreateDefaultClient();
	
	// you can also create your factory from configuration 
	BetaseriesClientFactory.Default = BetaseriesClientFactory.CreateFromConfiguration(true);
	
	// if you want an instance of another client type, you can use
	IBetaseriesSyncAsyncApi client = BetaseriesClientFactory.Default.CreateClient<BetaseriesXmlClient>();
	IBetaseriesSyncAsyncApi client = BetaseriesClientFactory.Default.CreateClient<BetaseriesJsonClient>(); // does not exist yet

If your application is very small, you can also create `BetaseriesXmlClient` instances directly:

	// Simple instanciation is also available
	IBetaseriesSyncAsyncApi client = new BetaseriesXmlClient("betaseriesapikey", "MyBetaApplication/1.0.0.0");
	
	// For silverlight/wp7
	IBetaseriesApi client = new BetaseriesXmlClient("betaseriesapikey", "MyBetaApplication/1.0.0.0");

### Practice: Console Application (.NET 3.5)

Create a Console application for .NET 3.5 or higher. Add a reference to Srk.!BetaSeriesApi.dll.

Configure your factory and you're ready to go.

	using Srk.BetaSeriesApi.Common;
	
	namespace SourceEdge.BetaSeriesConsoleDemo {
	    class Program {
	        static void Main(string[] args) {
	            // create your factory 1 time
	            BetaseriesClientFactory.Default = new BetaseriesClientFactory("your api key here", "MyBetaseriesApp/1.0.0.0", true);
	
	            // get as many clients as you want
	            IBetaseriesApi client = BetaseriesClientFactory.Default.CreateDefaultClient();
	        }
	    }
	}

### Practice: WPF Application (.NET 3.5)

Create or use an existing WPF application for .NET 3.5 or higher. Add a reference to Srk.!BetaSeriesApi.Common.dll.

You need to create a method to set your factory. To do this, open your App.xaml file and add an event handler for the Startup event.

	<Application x:Class="WpfApp.App"
	             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
	             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
	             StartupUri="MainWindow.xaml"
	             Startup="Application_Startup">
	</Application>

Then, in App.xaml.cs, create your factory in the appropriate method.

    public partial class App : Application {
        private void Application_Startup(object sender, StartupEventArgs e) {
            BetaseriesClientFactory.Default = new BetaseriesClientFactory("your api key here", "MyBetaseriesApp/1.0.0.0", true);
        }
    }

Finally, in your ViewModels or in code-behind files, you can get an API client.

    public class MyViewModel : ViewModelBase {

        protected IBetaseriesSyncAsyncApi client {
            get {
                if (_client == null)
                    _client = BetaseriesClientFactory.Default.CreateDefaultClient();
                return _client;
            }
        }
        private IBetaseriesSyncAsyncApi _client;

    }


Using synchronous methods (.NET only)
-------------------------------------

Provided clients do have synchronous methods. You have to take care: calling the API can take time depending on your internet connection. If you're calling a synchronous method in the UI thread, your UI will freeze.

Here is a sample console application searching for TV shows.

	static void Main(string[] args) {
	    // create your factory 1 time
	    BetaseriesClientFactory.Default = new BetaseriesClientFactory("your api key here", "ConsoleDemoApplication/1.0.0.0", true);
	
	    // get a configured client
	    IBetaseriesApi client = BetaseriesClientFactory.Default.CreateDefaultClient();
	
	    Stopwatch sw = new Stopwatch();
	
	    // search for shows containing "star"
	    sw.Start();
	    IList<Show> shows = null;
	    try {
	        shows = client.SearchShows("star");
	    } catch (Exception ex) {
	        // something bad happened
	        Console.Error.WriteLine(ex.Message);
	    }
	    sw.Stop();
	    Console.WriteLine(sw.Elapsed.ToString());
	    Console.WriteLine();
	
	    // show a list in the console
	    foreach (Show show in shows) {
	        Console.WriteLine(show.Title);
	    }
	
	    // no need to call Dispose on the client, we did not used event handlers
	    
	    Console.Read();
	}

Here is the result:

![console app showing a list of tv series](http://static.projects.sandrock.fr/betaseries/ConsoleDemo00.png)

	00:00:03.0358041
	
	A Million Stars Fall From The Sky
	Banner of the Stars
	Bastard
	Battlestar Galactica
	Battlestar Galactica (1978)
	Crest of the Stars
	Instant Star
	...

As you can see, the method took 3 seconds to return a result. If you don't want to freeze your UI, you might want to use the async pattern or to use your own way to create a secondary thread.

A simple way of creating a thread for your calls can be.

	// ask the threadpool to fullfil a job in another thread
	ThreadPool.QueueUserWorkItem(d => {
	    var result = client.SearchShows("star");
	
	    // use the dispatcher to return to the main thread
	    d.BeginInvoke(new Action(() => {
	        
	        // do something with result
	        
	        
	    }));
	}, System.Windows.Threading.Dispatcher.CurrentDispatcher);

Using asynchronous methods
--------------------------

Using asynchronous methods is an easy way to work with threading. Each service method has an event handler. This event handler will be raised when the service call has returned from the server.

Our `BetaseriesXmlClient` implements both synchronous and asynchronous methods. Synchronous methods are simple and return data. Asynchronous methods always return `void` and ends with `Async`. Each async method also have an event handler associated. Here is a sample from API interfaces.

 * `Member GetMember(string username);`  this is the synchronous method
 * `void GetMemberAsync(string username);`  this is the asynchronous method
 * `event AsyncResponseHandler<Member> GetMemberEnded;`  an event handler raised when `GetMemberAsync` finished his job

This example is in a !ViewModel class (this is equivalent to a code-behind class but for the MVVM pattern). First, in the constructor, you have to register to the appropriate events. Then you need to call a service method. Finally, the response will arrive in the delegate you chose in the constructor.


    public class LoginViewModel : CommonViewModel, IDisposable {

        public Member CurrentMember { get; set; }
    
        private IBetaseriesApi client;
        
        // this little thing is usefull with async methods
        private Dispatcher CurrentDispatcher = Dispatcher.CurrentDispatcher;
        
        
        public Login() {
            // create client from the factory
            client = BetaseriesClientFactory.Default.CreateDefaultClient();
            
            // best place to register to the event
            client.AuthenticateEnded += client_AuthenticateEnded;
            client.GetMemberEnded += client_GetMemberEnded;
        }

        // this method will be called by an ICommand for example
        private void DoLogin(string username, string password) {
            // async methods can be recognized with the Asyc suffix
            client.AuthenticateAsync(username, password);
        }

        private void client_AuthenticateEnded(object sender, AsyncResponseArgs<string> e) {
            // if you're using silverlight, the response come from another thread
            // you have to use the UI dispatcher to avoid errors
            CurrentDispatcher.BeginInvoke(() => {
                if (e.Succeed) {
                    // authentication is successfull
                    // now get user information
                    // calling this method with null return current user's information
                    client.GetMemberAsync(null);
                } else {
                    // authentication is not successfull
                    // more info in e.BetaError or e.Error
                    MessageBox.Show("Cannot login. " + Environment.NewLine + Environment.NewLine + e.Error.Message);
                }
            });
        }

        private void client_GetMemberEnded(object sender, AsyncResponseArgs<Member> e) {
            CurrentDispatcher.BeginInvoke(() => {
                if (e.Succeed) {
                    // here e.Data in a Member object contain information about the user
                    CurrentMember = e.Data;
                    MessageBox.Show("Hello " + CurrentMember.Username + "! ");
                } else {
                    MessageBox.Show("Cannot login. " + Environment.NewLine + Environment.NewLine + e.Error.Message);
                }
            });
        }
        
        protected void Dispose() {
            if (client != null) {
                // we've used event handlers, we have to clean
                factory.Dispose();
            }
        }
    }

Disposing clients
-----------------

When using a factory or using asynchronous methods, API client will hold references. In some cases this can lead to memory leaks. This is why our `BetaseriesXmlClient` implements IDisposable. 

For example, imagine an object A (any application class) creating an object B (API client). A will hold a reference to B, preventing B from being collected by the GC. If object A registers to an object B event, then B will hold a reference to object A. In this case each object holds a reference to each other. This can prevent the garbage collector to destroy those objects, leading to abusive memory usage (and even application crash).

A simple way of preventing this is to use the `IDisposable` interface. When you call `Dispose()` on a betaseries API client, all event handlers are cleared. This way, no cross-reference is held and objects can be destroyed.

### ViewModel case

If you use the MVVM pattern, you might already use a toolkit. If this is the case, you might have a Dispose method overridable in the !ViewModel base class. If you don't, I recommend you to have a base !ViewModel class that implements `IDisposable`. In your VM you will be able to override the `Dispose` method to Dispose the betaseries API client. Then in your view, you will have to call Dispose on your !ViewModel (implementing `IDisposable` does not magically call Dispose, you still have to do it).

### Code-behind case 

If you're using a simple code-behind approach, then you can always override the `Dispose` method to call `client.Dispose();` (don't forget to call the base method).
