Srk.BetaseriesApi, the .NET wrapper to the [betaseries.com](http://www.betaseries.com/) API v2!
========================

This project started on [CodePlex](https://betaseries.codeplex.com/). The codebase has a new home here.

WORK IN PROGRESS
============

EN: In this branch: **implement API v2.2 instead of 1.0**.  
FR: Dans cette branche: **implémenter l'API v2.2 à la place de la v1.0**.

* **Goal: make new apps for betaseries!**  
Objectif : construire des apps cool pour betaseries !
* **Fun constraint: Generate code instead of writing it!**  
Contrainte fun : générer les codes source au lieu de les écrire directement !

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

 `[x]` Microsoft .NET 3.5 assembly 
 `[ ]` Silverlight 4     
 `[ ]` Windows 8 Apps    
 `[ ]` Windows Phone 8   
 `[ ]` Windows Phone 7   
 `[ ]` Windows Mobile 6  

Code generation 
============

Rules for the win
--------------

This library is written in a special way.  
Cette librairie est écrite d'une façon spéciale.

1. Source code related to API requests MUST be generated based on the official documentation.  
   Les codes sources d'appel des méthodes API DOIVENT être générés sur la base de la documentation officielle.  
1. Source code related to API requests MAY be enhanced using a rules file (Api2.xml)  
   Les codes sources d'appel des méthodes API PEUVENT être modifiés par l'application de règles personnalisées.

Rule 1 allows updating code in a automated way. Just run a template file and everything is up-to-date.  
La règle 1 permet la mise à jour automatisée de la librairie. Executer un template et c'est à jour.

Rule 2 covers stuff code generation cannot cover (custom method names, special cases...)  
La règle 2 permet de contourner ce que le génération automatique ne peut pas bien gérer (noms des méthodes, traitements spéciaux...)

The project `Srk.BetaseriesApiFactory` does the big job. The Api2.tt file uses a `ApiFactory` object to generate API calls and data structures in C#.

	class ApiFactory
	{
	    public void Run(TextWriter text)
	    {
	        var context = new ApiFactoryContext();
	        this.FetchDocumentation(context);       // from http://www.betaseries.com/api/methodes/ 
	        this.ReviewDocumentation(context);      // cleanup method names and stuff
		
	        this.LoadTransforms(context, text);     // load & apply custom rules
	        this.ApplyTransforms(context, text, methods: true, argumentEnums: true);
	        this.ApplyTransforms(context, text, responseFormats: true);
		
	        this.WriteEntities(context, text);      // generate DTOs 
	        this.WriteArgumentEnums(context, text); // generate DTOs
	        this.WriteService(context, text);       // generate calls and main classes
	    }
	}

Big issues
--------------

* The documentation is not easy to read from C#
* The documentation does not always include result structures
* The documentation may not specify argument types
* Result structures are hard to handle in C#
 * The main result object should contain static property names (errors and data).
 * The main result object's main data entry does not have a predictable name

Progress
--------------

`[x]` Setup .tt file anf code factory for .NET 3.5
`[x]` Fetch documentation pages from website (normalize html to xml, parse content)  
`[x]` Translate documentation into description objects (methods, arguments, result structures)  
`[x]` Generate data structures and method calls in C#  
`[x]` Create a custom rules file and apply those rules  
`[-]` Verify main API methods (authentication, lists, posts)  
`[ ]` Verify all API methods  
`[ ]` The library is "ready", update http://www.betaseries.com/wiki/Srk.BetaseriesApi  
`[ ]` Implement async methods based on callbacks (.NET 3.5)    
`[ ]` Implement async methods based on callbacks (Silverlight 4)  
`[ ]` Implement async methods based on callbacks (Windows Phone 7)  
`[ ]` Implement async methods based on async/await (.NET 4.5)  
`[ ]` Implement async methods based on async/await (Windows Phone 8)  

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



