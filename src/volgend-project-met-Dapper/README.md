# Volgend project met Dapper?
Bij het bouwen van .NET applicaties spelen ORM’s een grote rol voor het ophalen van data. Twee populaire opties zijn Dapper, 
een mico-ORM, en Entity Framework, een full-featured ORM. Beide ORM’s hebben beide hun eigen aanpak voor het ophalen van data. 
Entity Framework zorgt ervoor dat developers zich geen druk hoeven te maken over het schrijven van SQL of het zelf uitvoeren van migraties 
op een database. Dapper daarentegen dwingt de developer hierbij om zelf zijn 
verantwoordelijkheid te nemen. Het zelf schrijven van SQL en het zelf uitvoeren van migraties op de database zijn taken 
die vallen onder de verantwoordelijkheid van de gebruiker. In deze blogpost ga ik door middel van een hands-on karakter 
onderzoek doen naar hoe Dapper werkt en waarom jouw volgend project misschien wel Dapper moet gebruiken.

<img src="plaatjes/dapper.jpg" width="250" align="right" alt="dapper logo" title="dapper">

*[Jochem Kalsbeek, oktober 2024.](https://github.com/hanaim-devops/blog-student-naam)*
<hr/>

De hoofdzaak van deze blogpost is het leren begrijpen van hoe data kan worden beheerd aan de hand van Dapper en hoe dit zich 
verhoudt met Entity Framework op het gebied van gebruiksvriendelijkheid, performance en learnability. Daarbij beschouw 
ik hoe Entity Framework precies werkt als bijzaak. Hierover worden in deze hands-on geen concrete voorbeelden gegeven.

<img src="plaatjes/logo.png">

## Cheapest Flight Tickets

### 1. Opzetten Applicatie
In deze hands-on tutorial richten we ons op het opzetten van een solution voor een fictieve applicatie genaamd ‘Cheapest Flight Tickets’ waarop gebruikers
fictief de goedkoopste vliegtickets kunnen bekijken. 
Het doel van deze applicatie is om de performance van Dapper te vergelijken met die van Entity Framework in een benchmark. 
Deze solution bevat de volgende twee projecten:
- CheapestFlightTickets.Benchmark: Dit is een console project, hierin worden de benchmarks uitgevoerd om de performance te vergelijken. 
Dit project bevat de benchmark-logica en meet hoe snel Dapper en Entity Framework operaties kunnen uitvoeren.
- CheapestFlightTickets.Data: Dit project bevat de logica voor het beheren van de data, inclusief de database models voor zowel Dapper als Entity Framework. 
Dit project wordt gebruikt door het console project.

Maak beide projecten aan in de solution en installeer Dapper samen met Entity Framework in het data project. 
Om Dapper te installeren in het data project dien je alleen een package te installeren met het volgende commando:

```bash
    dotnet add CheapestFlightTickets.Data package Dapper
```

Als je dit commando hebt uitgevoerd is Dapper geïnstalleerd en is het klaar voor gebruik. 
De volgende stap is het installeren en configureren van Entity Framework. Hoe dit moet laat ik in deze hands-on achterwege maar is <a href="https://github.com/hanaim-devops/devops-blog-kriebslang/tree/main/CheapestPlaneTickets">hier</a> terug te zien. 
De volgende stap is het schrijven van Dapper query’s.

### 2. Query's schrijven met Dapper
Gebruikers van Cheapest Flight Tickets willen graag een lijst van vluchten kunnen inzien met een prijs erbij. 
Hiervoor moet er een Dapper query worden geschreven die deze lijst met vluchten kan ophalen uit de database. 
Hieronder staat een voorbeeld van hoe een query kan worden opgesteld met Dapper:

```csharp
    public Flight? GetFlight(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        var sql = $"""
                   
                           SELECT 
                               f.Id, 
                               f.DepartureTime, 
                               f.ArrivalTime, 
                               f.Price, 
                               f.PlaneId, 
                               p.Id, 
                               p.Model 
                           FROM Flights f
                           WHERE f.Id = @Id
                   """;

        return connection.Query<Flight>(sql,
                param: new { Id = id })
            .FirstOrDefault();
    }
```

Hierbij is duidelijk te zien dat Dapper eigenlijk niet veel meer is dan een verzameling extension methods op een 
SQLConnection die we kennen uit ADO.NET. Een van de verschillen met bijvoorbeeld Entity Framework is dat je bij Dapper 
zelf SQL moet schrijven terwijl bij Entity Framework dit voor jou geregeld wordt doordat je LINQ expressies kan gebruiken. 
Dapper biedt echter wel de mogelijkheid tot het gebruik van geparametriseerde query’s zoals je in het voorbeeld ziet.

#### Multimapping
Vorig voorbeeld mist nog iets. Model Flight kent namelijk een one-to-one-relation met Plane. Dapper laadt niet automatisch gerelateerde entiteiten. 
Gebruikers moeten zelf joins schrijven om data uit andere tabellen erbij op te halen. Dapper biedt echter wel de mogelijkheid om gerelateerde data te mappen. 
Als we de vorige query er weer bij pakken en we breiden deze verder uit met een join om het bijbehorende vliegtuig op te halen dan krijg je het volgende resultaat:

```csharp
    public Flight? GetFlight(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        var sql = $"""
                   
                           SELECT 
                               f.Id, 
                               f.DepartureTime, 
                               f.ArrivalTime, 
                               f.Price, 
                               f.PlaneId, 
                               p.Id, 
                               p.Model 
                           FROM Flights f
                           INNER JOIN Planes p ON f.PlaneId = p.Id
                           WHERE f.Id = @Id
                   """;

        return connection.Query<Flight>(sql,
                param: new { Id = id })
            .FirstOrDefault();
    }
```

Nu haalt Dapper ook de bijbehorende data op. Echter zal hij deze nog niet goed mappen,
de volgende stap is om Dapper uit te leggen hoe wij de data graag gemapt willen hebben. Hieronder staat een voorbeeld hoe dit in zijn werking gaat.

```csharp
    public Flight? GetFlight(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        var sql = $"""
                   
                           SELECT 
                               f.Id, 
                               f.DepartureTime, 
                               f.ArrivalTime, 
                               f.Price, 
                               f.PlaneId, 
                               p.Id, 
                               p.Model 
                           FROM Flights f
                           INNER JOIN Planes p ON f.PlaneId = p.Id
                           WHERE f.Id = @Id
                   """;

        return connection.Query<Flight, Plane, Flight>(sql, (flight, plane) =>
                {
                    flight.Plane = plane;
                    return flight;
                },
                splitOn: "PlaneId",
                param: new { Id = id })
            .FirstOrDefault();
    }
```

Door middel van een lambda-functie met daarin parameters ontvang je de gemapde data van zowel de data van de huidige 
entiteit als de gerelateerde entiteit. En geef je aan Dapper aan hoe hij gerelateerde data moet mappen. 
Door de splitOn property toe te voegen vertel je Dapper wanneer hij moet gaan beginnen met het mappen van de volgende entiteit gerelateerde entiteit.

### 3. Statements schrijven met Dapper
Naast het ophalen van alle vluchten willen gebruikers zich ook aanmelden bij de applicatie. 
Er moeten hiervoor ook statements worden geschreven met Dapper. Dit gaat eigenlijk op dezelfde manier als een query. 
Hieronder staat een voorbeeld van een insert statement met Dapper:

Hierbij komt weer de extension method terug die een geparametriseerde statement probeert uit te voeren over een SQLconnection. 
Het object dat vervolgens geïnjecteerd moet worden, geef je via een parameter mee aan deze methode.

### 4. Installeren benchmark tool

Om de performance te vergelijken tussen entity framework en Dapper maken we gebruik van benchmark dotnet. 
Volgens de <a href="https://benchmarkdotnet.org/articles/overview.html">documentatie van benchmark dotnet</a> moeten we deze als eerst installeren via nuget. 
Open de terminal in het benchmark project en voor het volgende commando uit:

Hierna staat benchmark dotnet geïnstalleerd en hoeft deze alleen geconfigureerd te worden. Waarna de benchmark geschreven kan worden. 
Hoe dit moet laat ik in deze tutorial achterwege maar het uiteindelijke resultaat vind je <a href="https://github.com/hanaim-devops/devops-blog-kriebslang/tree/main/CheapestPlaneTickets">hier</a> terug.

### 5. Benchmarking

De benchmark die geschreven is in de vorige stap, gaan we nu uitvoeren zodat we kunnen vergelijken hoe Dapper presteert ten 
opzichte van Entity Framework op het gebied van query prestaties. De benchmark kun je uitvoeren door het volgende commando 
in de terminal van het benchmark project te typen:

Deze commando zal de benchmark starten. Als ik de benchmark uitvoer krijg ik onderstaande resultaten terug. 
Hierin is duidelijk te zien dat Dapper sneller is dan Entity Framework. Dit heeft te maken met het feit dat Entity Framework 
eerst een LINQ query moet vertalen naar SQL.

<img src="plaatjes/benchmark-small.png" />

Volgens de <a href="https://learn.microsoft.com/en-us/ef/core/querying/sql-queries?tabs=sqlserver">entity framework docs</a> is er ook functionaliteit binnen Entity Framework waarmee je
query’s kan uitvoeren zonder de vertaalslag van SQL naar LINQ. Hieronder staat nog een benchmark waarbij ook deze functionaliteit gebruikt wordt in de benchmark:

<img src="plaatjes/benchmark-with-raw.png" />

Dit geeft niet het resultaat waar we gehoopt op hadden. Microsoft beschrijft in dit <a href="https://learn.microsoft.com/en-us/ef/core/querying/sql-queries?tabs=sqlserver">artikel</a> dat deze functionaliteit voor een andere intentie ontwikkeld is. 
Het moet namelijk gebruikt worden om SQL te combineren met LINQ. 
Nu zal hij tweemaal de compilestap doorlopen. Eenmaal om de SQL om te zetten naar LINQ waarna hij de LINQ weer omzet 
naar SQL die hij vervolgens gaat uitvoeren op de database. Dit verklaart de langere uitvoeringsduur. Ook Cantekin deed een 
<a href="https://salihcantekin.medium.com/the-big-fight-dapper-vs-entity-framework-detailed-benchmark-2345af933382">benchmark</a> 
waarbij hij tot dezelfde conclusie kwam.

#### Grotere dataset
Het vorige voorbeeld is uitgevoerd op een kleine dataset lees ongeveer honderd records. Wat gebeurt er als we de dataset 
vergroten naar ongeveer honderdduizend records? Is het resultaat dan hetzelfde? Dit heb ik uitgevoerd met het onderstaande resultaat:

<img src="plaatjes/benchmark-large.png" />

Hierbij valt op dat het verschil tussen Dapper en Entity Framework groter wordt naarmate de dataset groeit. 
Bij de kleinere dataset was het gemiddelde verschil tussen Dapper en Entity Framework kleiner dan bij de grote.

## Conclusie
Beide ORM’s bieden eigen voordelen voor het gebruik van de een boven de ander. Entity Framework zorgt ervoor dat developers 
zich bijna geen zorgen hoeven te maken over het schrijven van SQL query’s. Echter blijkt uit dit onderzoek dat Entity Framework minder 
goed scoort op het gebied van query prestaties. Bij grotere datasets wordt het prestatieverschil tussen Dapper en Entity Framework alleen maar groter. 
Daarom is de keuze tussen beide frameworks afhankelijk van de specifieke projectvereisten: kies Dapper voor optimale prestaties en controle, 
en Entity Framework voor eenvoud en productiviteit.

## Bronnen

1. Mahajan, H. (2024, 9 oktober). *Dapper vs EF Core: Which ORM framework should you choose for your .NET application?* Level Up Coding. Geraadpleegd van [https://levelup.gitconnected.com/dapper-vs-ef-core-which-orm-framework-should-you-choose-for-your-net-application-54f2723b176a](https://levelup.gitconnected.com/dapper-vs-ef-core-which-orm-framework-should-you-choose-for-your-net-application-54f2723b176a)

2. Learn Dapper. (z.d.). *Dapper vs Entity Framework*. Geraadpleegd op 9 oktober 2024 van [https://www.learndapper.com/dapper-vs-entity-framework](https://www.learndapper.com/dapper-vs-entity-framework)

3. Cantekin, S. (z.d.). *The big fight: Dapper vs Entity Framework detailed benchmark*. Medium. Geraadpleegd op 9 oktober 2024 van [https://salihcantekin.medium.com/the-big-fight-dapper-vs-entity-framework-detailed-benchmark-2345af933382](https://salihcantekin.medium.com/the-big-fight-dapper-vs-entity-framework-detailed-benchmark-2345af933382)

4. Learn Dapper. (z.d.). *Learn Dapper*. Geraadpleegd op 9 oktober 2024 van [https://www.learndapper.com/](https://www.learndapper.com/)

5. Microsoft. (z.d.). *SQL queries*. Learn Microsoft. Geraadpleegd op 9 oktober 2024 van [https://learn.microsoft.com/en-us/ef/core/querying/sql-queries?tabs=sqlserver](https://learn.microsoft.com/en-us/ef/core/querying/sql-queries?tabs=sqlserver)





