Srk.BetaseriesApi, the .NET wrapper to the [betaseries.com](http://www.betaseries.com/) API!
========================

This project started on [CodePlex](https://betaseries.codeplex.com/). The codebase has a new home here.

What's this?
============

This is a Microsoft .NET library that permits you to use the betaseries.com API with minimum code.

It uses the betaseries v1 API which will close on 30/04/2014.

There is a nice amount of work necessary to implement the v2 API.

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

License
=======

This work is licensed under the LGPL v2.1.

Primarily used for software libraries, LGPL requires that derived works be licensed under the same license, but works that only link to it do not fall under this restriction. 

Usage
========

Use: there is a [use guide](UseGuide.md).

Develop: you may fork to add/update api calls...  

References
============

[API 1 reference documentation](http://www.betaseries.com/wiki/Documentation), [wiki page for this wrapper](http://www.betaseries.com/wiki/Srk.BetaseriesApi)

[API 2 reference documentation](http://www.betaseries.com/api/docs)



