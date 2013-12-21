This is the homepage for the .NET library for the betaseries API

Sources are under license. Please read NetLibLicenses before use.

Importing from codeplex...
==========================

The project started on [CodePlex](https://betaseries.codeplex.com/). I'm importing it right now...

What's this?
============

This is a Microsoft .NET project that permits you to use the betaseries.com API with minimum code.

	// Because you will need multiple api client instances, you can configure them this way
	BetaseriesClientFactory.Default = 
	  new BetaseriesClientFactory("betaseriesapikey", "MyBetaApplication/1.0.0.0", true);
	
	// Then anywhere you can get a client like this
	IBetaseriesSyncAsyncApi client = 
	  BetaseriesClientFactory.Default.CreateDefaultClient();
	// Simple instanciation is also available
	IBetaseriesSyncAsyncApi client = 
	  new BetaseriesXmlClient("betaseriesapikey", "MyBetaApplication/1.0.0.0");
	
	// authenticate user?
	client.Authenticate("username", "password");
	
	// searching for shows?
	var shows = client.SearchShows("stargate");
	
	// you also have async methods
	client.GetShowEnded += (sender, args) => {
	    if (args.Success) {
	        var myShow = args.Data;
	    } else {
	        MessageBox.Show(args.ErrorMessage);
	    }
	};
	client.GetShowAsync("dexter");

This library is available for:

 * Microsoft .NET 3.5 assembly (we use LINQ to XML, so 3.5)
 * Silverlight 4 
 * Windows Phone 7
 * Windows Mobile 6 

TODO
====

 * Import documentation from trac.
 * Import sources from codeplex.

License
=======

This work is licensed under the LGPL v2.1.

Primarily used for software libraries, LGPL requires that derived works be licensed under the same license, but works that only link to it do not fall under this restriction. 
