This experiment starts with the default MAUI Blazor project file.  Started just to see if it compiles out of the box.
This one did.  I then started experiementing with adding pages, the objective is to add a page that has an embedded 
web browser that works over the Internet.

1. Added Serilog logging impacted files:
	Serilog packages added	
	MauiProgram.cs updated (initialize)
	DefMauiBlazorPrj.csproj updated (create directory for log files)
	Counter.razor updated (testing)