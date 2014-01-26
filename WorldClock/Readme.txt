Debug Build and Deploy:
C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\MSBuild.exe "C:\Documents and Settings\leonard.erwine\My Documents\Visual Studio 2005\Projects\WebParts\WorldClock\WorldClock.csproj" /p:TargetFX1_1=true /p:Configuration=Debug /t:Rebuild
C:\Windows\System32\MakeCAB.exe -f "C:\Documents and Settings\leonard.erwine\My Documents\Visual Studio 2005\Projects\WebParts\WorldClock\WorldClock.ddf"
"C:\Program Files\Common Files\Microsoft Shared\web server extensions\60\bin\STSADM.EXE" -o addwppack -filename "C:\Documents and Settings\leonard.erwine\My Documents\Visual Studio 2005\Projects\WebParts\WorldClock\WorldClock.cab" -globalinstall -force
COPY /Y "C:\Documents and Settings\leonard.erwine\My Documents\Visual Studio 2005\Projects\WebParts\WorldClock\bin\FX1_1\Debug\WorldClock.pdb" C:\Inetpub\wwwroot\bin\
C:\Windows\System32\iisreset.exe
"C:\Program Files\Internet Explorer\iexplore.exe" "http://spsdev/sites/wptest/default.aspx"

Debug Quick Build and Deploy:
C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\MSBuild.exe "C:\Documents and Settings\leonard.erwine\My Documents\Visual Studio 2005\Projects\WebParts\WorldClock\WorldClock.csproj" /p:TargetFX1_1=true /p:Configuration=Debug
C:\Windows\System32\MakeCAB.exe -f "C:\Documents and Settings\leonard.erwine\My Documents\Visual Studio 2005\Projects\WebParts\WorldClock\WorldClock.ddf"
"C:\Program Files\Common Files\Microsoft Shared\web server extensions\60\bin\STSADM.EXE" -o addwppack -filename "C:\Documents and Settings\leonard.erwine\My Documents\Visual Studio 2005\Projects\WebParts\WorldClock\WorldClock.cab" -globalinstall -force
COPY /Y "C:\Documents and Settings\leonard.erwine\My Documents\Visual Studio 2005\Projects\WebParts\WorldClock\bin\FX1_1\Debug\WorldClock.pdb" C:\Inetpub\wwwroot\bin\
C:\Windows\System32\iisreset.exe
"C:\Program Files\Internet Explorer\iexplore.exe" "http://spsdev/sites/wptest/default.aspx"

Debug Build direct:
C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\MSBuild.exe "C:\Documents and Settings\leonard.erwine\My Documents\Visual Studio 2005\Projects\WebParts\WorldClock\WorldClock.csproj" /p:TargetFX1_1=true /p:Configuration=Debug /t:Rebuild /p:OutputPath=C:\Inetpub\wwwroot\bin\ & C:\Windows\System32\iisreset.exe & "c:\Program Files\Internet Explorer\iexplore.exe" http://spsdev/sites/wptest/default.aspx

Debug Quick build direct:
C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\MSBuild.exe "C:\Documents and Settings\leonard.erwine\My Documents\Visual Studio 2005\Projects\WebParts\WorldClock\WorldClock.csproj" /p:TargetFX1_1=true /p:Configuration=Debug /p:OutputPath=C:\Inetpub\wwwroot\bin\ & C:\Windows\System32\iisreset.exe & "c:\Program Files\Internet Explorer\iexplore.exe" http://spsdev/sites/wptest/default.aspx

Release Build and Deploy:
C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\MSBuild.exe "C:\Documents and Settings\leonard.erwine\My Documents\Visual Studio 2005\Projects\WebParts\WorldClock\WorldClock.csproj" /p:TargetFX1_1=true /p:Configuration=Release /t:Rebuild
C:\Windows\System32\MakeCAB.exe -f "C:\Documents and Settings\leonard.erwine\My Documents\Visual Studio 2005\Projects\WebParts\WorldClock\WorldClockR.ddf"
"C:\Program Files\Common Files\Microsoft Shared\web server extensions\60\bin\STSADM.EXE" -o addwppack -filename "C:\Documents and Settings\leonard.erwine\My Documents\Visual Studio 2005\Projects\WebParts\WorldClock\WorldClock.cab" -globalinstall -force

Release build direct:
C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\MSBuild.exe "C:\Documents and Settings\leonard.erwine\My Documents\Visual Studio 2005\Projects\WebParts\WorldClock\WorldClock.csproj" /p:TargetFX1_1=true /p:Configuration=Release /t:Rebuild /p:OutputPath=C:\Inetpub\wwwroot\bin\

Delete Web Part
"C:\Program Files\Common Files\Microsoft Shared\web server extensions\60\bin\STSADM.EXE" -o deletewppack -name WorldClock.cab
del C:\Inetpub\wwwroot\bin\WorldClock.dll
del C:\Inetpub\wwwroot\bin\WorldClock.pdb
C:\Windows\System32\iisreset.exe

Calling Order, normal:
WebPart.Constructor
WebPart.Properties get/set
WebPart.CreateWebPartMenu
WebPart.CreateChildControls
WebPart.SaveViewState
WebPart.RenderWebPart

Calling Order, Child Control Event:
WebPart.Constructor
WebPart.Properties get/set
WebPart.CreateChildControls
WebPart.CreateWebPartMenu
WebPart.ChildControl_EventHandlers
WebPart.SaveViewState
WebPart.RenderWebPart

Calling Order, Edit initial:
WebPart.Constructor
WebPart.LoadViewState
WebPart.CreateWebPartMenu
WebPart.GetToolParts
ToolPart.Constructor
ToolPart.ToolPart_Init_Handler
WebPart.CreateChildControls
ToolPart.CreateChildControls
WebPart.Properties get/set
WebPart.SaveViewState
ToolPart.SaveViewState
WebPart.RenderWebPart
ToolPart.RenderToolPart

Calling Order, Edit, Web Part Child Control Event:
WebPart.Constructor
WebPart.LoadViewState
WebPart.CreateChildControls
WebPart.CreateWebPartMenu
WebPart.GetToolParts
ToolPart.Constructor
ToolPart.ToolPart_Init_Handler
ToolPart.LoadViewState
WebPart.Properties get/set
WebPart.ChildControl_Event_Handler
ToolPart.CreateChildControls
WebPart.SaveViewState
ToolPart.SaveViewState
WebPart.RenderWebPart
ToolPart.RenderToolPart

Calling Order, Edit, Tool Part Child Control Event:
WebPart.Constructor
WebPart.CreateWebPartMenu
WebPart.GetToolParts
ToolPart.Constructor
ToolPart.ToolPart_Init_Handler
ToolPart.LoadViewState
WebPart.Properties get/set
ToolPart.CreateChildControls
ToolPart.ChildControl_Event_Handler
WebPart.CreateChildControls
WebPart.SaveViewState
ToolPart.SaveViewState
WebPart.RenderWebPart
ToolPart.RenderToolPart

Calling Order, Edit, Cancel:
WebPart.Constructor
WebPart.CreateWebPartMenu
WebPart.GetToolParts
ToolPart.Constructor
ToolPart.ToolPart_Init_Handler
ToolPart.LoadViewState
WebPart.Properties get/set
WebPart.CreateChildControls
WebPart.SaveViewState
WebPart.RenderWebPart
