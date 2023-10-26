# ap-fileuploader-be
Das Backend-Projekt zum File Uploader im Rahmen meiner betrieblichen Projektarbeit für die IHK im Herbst 2023.

Das Projekt enthält die Geschäftslogik sowie die Logik für den Zugriff auf das Datenschicht. 

Die Geschäftslogik basiert auf der .NET Core 6 Version des ASP.NET Web API Projekttemplates. Hier wurden die REST API-Schnittstellen zur Kommunikation mit dem Frontend sowie Services zur Verarbeitung der zu versendenden oder empfangenen Daten.

Der Zugriff auf die Geschäftslogik erfolgt mithilfe von ersetzbaren EntityFramework Core-Projekten, die als Package dem Web API Projekt hinzuzufügen sind. Das in diesem Repository bestehende EF Core-Projekt setzt voraus, dass in der DEV-Umgebung eine lokale Instanz der SQL Server 2019 Datenbank eingerichtet ist und dort eine Tabelle DC_Metadata hinterlegt ist.