<!DOCTYPE html>
<html>
<body>
<h3>Words of Wisdom:</h3>

<%
	Dim oShell
  Set oShell = Server.CreateObject("Wscript.Shell");
  oShell.Run /bin/Debug/WebCrawler.exe
  Set oShell = nothing
  %>
</body>
</html>
