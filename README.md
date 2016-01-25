Srk.BetaseriesApi, the .NET wrapper to the [betaseries.com](http://www.betaseries.com/) API v2!
========================

This project started on [CodePlex](https://betaseries.codeplex.com/). The codebase has a new home here.

WORK IN PROGRESS
============

EN: In this branch: **implement API v2.2 instead of 1.0**.  
FR: Dans cette branche: **implémenter l'API v2.2 à la place de la v1.0**.

* **Goal: make new apps for betaseries!**  
Objectif : construire des apps cool pour betaseries !
* --Fun constraint: Generate code instead of writing it!--  That's not fun.  
--Contrainte fun : générer les codes source au lieu de les écrire directement !-- Nop, don't do it.



What's this?
============

This is a Microsoft .NET library that permits you to use the betaseries.com API with minimum code.  
C'est une librairie .NET permettant l'utilisation de l'API de betaseries.com avec peu de code.

    using Srk.BetaseriesApi2;    
	
    // Then anywhere you can get a client like this
    string apiKey = "xxxxxxxxxxxxxx";
	var client = new Srk.BetaseriesApi2.BetaseriesClient(apiKey, "MySuperApp/1.0");
	
	// authenticate user?
	var authToken = client.Authenticate("username", "password");
    client.SetUser(authToken);
	
	// searching for shows?
	var shows = client.SearchShows("stargate");
	
	// you also have async methods
	client.GetShowAsync(
		"dexter");
		(sender, args) => {
		    if (args.Success) {
		        var myShow = args.Data;
		    } else {
		        Trace.TraceError(args.ErrorMessage);
		    }
		});

This library is available for:  
Cette librairie est disponible pour :

- [ ] Microsoft .NET 3.5 assembly
- [ ] Silverlight 4
- [ ] Windows 8 Apps
- [ ] Windows Phone 8
- [ ] Windows Phone 7
- [ ] Windows Mobile 6

Code generation 
============

Previous commits were going in the wrong direction.

Roadmap to success:

- [x] Format code, stylecop pass
- [ ] Drop .NET < 4.5 support
- [ ] Switch to Microsoft.Net.Http
- [ ] Mark all methods obsolete
- [ ] Convert major methods
- [ ] Clean wrong code
- [ ] Merge to main
- [ ] NUGET




License
=======

This work is licensed under the LGPL v2.1.

Primarily used for software libraries, LGPL requires that derived works be licensed under the same license, but works that only link to it do not fall under this restriction. 

Usage
========

Use: there is a [use guide](UseGuide.md).

Develop: you may fork to add/update api calls...  

Notes / contributing
========

This is a ongoing feature branch. It helps moving from API v1 to v2.

.NET Framework versions
--------------

Everything is built for .NET 3.5 because many projects still aren't using .NET 4+. Linked projects allow more code features by duplicating base code and build symbols handle what syntaxes are used in each build target.

HTTP stack 
--------------

Uses old-school WebHttpRequest and WebHttpResponse. It works quite well with the instrumentation in place.

Style
--------------

I'm transitioning codes from a custom style to the standard StyleCop style. You may see various formatings, target is StyleCop compliance.

References
============

[API 1 reference documentation](http://www.betaseries.com/wiki/Documentation), [wiki page for this wrapper](http://www.betaseries.com/wiki/Srk.BetaseriesApi)

[API 2 reference documentation](http://www.betaseries.com/api/docs)



