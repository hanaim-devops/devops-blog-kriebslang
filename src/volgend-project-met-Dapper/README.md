# Volgend project met Dapper?
Dapper is een lichtgewicht open-source object-relational mapping (ORM) library, ontworpen om efficiënt SQL queries uit 
te voeren binnen .NET of .NET Core applicaties. Het mapt het resultaat van deze queries naar de objecten die in je 
applicatie worden gebruikt. In deze blogpost zal Dapper vergeleken worden met Entity Framework (EF) en wordt er 
toegelicht of Dapper wellicht een betere keuze is voor een volgend project.

<img src="plaatjes/dapper.jpg" width="250" align="right" alt="dapper logo" title="dapper">

*[Jochem Kalsbeek, oktober 2024.](https://github.com/hanaim-devops/blog-student-naam)*
<hr/>

Hoewel beide libraries ORM-functionaliteiten bieden, verschillen ze in mogelijkheden. Volgens de officiële website van 
Dapper (www.learndapper.com) is dapper gericht op eenvoud en snelheid, terwijl Entity Framework uitgebreidere 
functionaliteit biedt zoals change tracking, SQL-generatie en database-migraties. 

<img src="plaatjes/functionality_dapper.png">

Uit de bovenstaande tabel blijkt duidelijk dat Dapper minder functionaliteit biedt dan Entity Framework, 
maar deze eenvoud kan ook een voordeel zijn. Voor sommige ontwikkelaars is de beperkte functionaliteit juist 
aantrekkelijk omdat het meer controle biedt over de uitgevoerde SQL en de manier waarop migraties worden uitgevoerd. Voor 
andere developers is dit juist een hindernis omdat ze zelf verantwoordelijk zijn voor het schrijven van SQL code of het 
uit laten voeren van migrations.

Volgens de officiële website van Dapper (www.learndapper.com) is de kracht van Dapper dat het simpel wordt gehouden. 
Ontwikkelaars schrijven zelf de SQL-statements, en Dapper voert ze uit terwijl de resultaten worden gemapt naar objecten. 
Dit geeft volledige controle over de queries. Wat voor sommige developers juist een voordeel is.
Een voorbeeld van hoe Dapper gebruikt kan worden om gegevens op te halen:

<img src="plaatjes/select.png">


In tegenstelling tot Entity Framework, waar LINQ wordt gebruikt om SQL te genereren, vereist Dapper dat 
ontwikkelaars zelf SQL schrijven. Dit betekent dat er kennis van SQL wordt vereist om met Dapper te werken. Voor ontwikkelaars 
zonder SQL-ervaring kan dit een hindernis zijn. Entity Framework daarentegen werkt met LINQ, wat de drempel voor 
developers kan verlagen.

<img src="plaatjes/insert.png">

Zoals je kunt zien, moet de ontwikkelaar het SQL-statement zelf opbouwen, wat kennis van SQL vereist. 
In tegenstelling tot Entity Framework, waar deze kennis niet nodig is.

Naast dat er onderzoek is verricht naar de gebruiksgemak en leercurve van Dapper tegenover 
Entity Framework is er ook onderzoek gedaan naar de prestaties tegenover Entity Framework. 
Om hier onderzoek naar te doen heb is er gebruik gemaakt van BenchmarkDotNet. Deze package helpt developers om voor functies benchmarks te 
maken waarin de performance van de functies wordt getest. Naderhand geeft BenchmarkDotNet een uitgebreid resultaat 
terug van hoe de verschillende benchmarks gescoord hebben tegenover elkaar. Om Dapper te vergelijken met Entity Framework 
heb zijn er drie verschillende benchmarks geschreven waarvan hieronder het resultaat staat. Elke benchmark wordt twintig keer 
uitgevoerd waarna voor elke benchmark het gemiddelde uitvoertijd wordt berekend.

<img src="plaatjes/benchmark_select.png">

Uit de resultaten blijkt dat Dapper aanzienlijk sneller is dan Entity Framework, met name omdat Dapper geen extra stappen 
zoals SQL-generatie hoeft uit te voeren. Bij Entity Framework moeten LINQ-expressies eerst worden omgezet naar SQL, wat extra tijd kost.
Opvallend genoeg bleek dat zelfs de "FromSQL"-functionaliteit van Entity Framework, waarbij directe SQL wordt uitgevoerd, trager is dan de versie met LINQ-expressies.
In de documentatie over queries binnen Entity Framework staat de volgende afbeelding:

<img src="plaatjes/raw_sql.png">

In deze afbeelding wordt getoond hoe de FromSQL methode gebruikt wordt om een eigen SQL query te gebruiken. 
Na de query wordt de query afgemaakt met LINQ expressies. Hier zit de overhead. De ingevoerde SQL code moet worden 
omgezet in LINQ expressies om zo gebruikt te worden in de query. Nadat alles in LINQ expressies is gezet zal het 
weer worden omgezet in SQL code waarna het uitgevoerd wordt. Dit betekent dat er twee vertaal stappen moeten worden 
uitgevoerd voordat de query kan worden uitgevoerd. Hierdoor is de uitvoeringstijd trager als bij een query waar alleen 
gewerkt werd met LINQ expressies.

Uit dit onderzoek blijkt dat Dapper een lagere uitvoeringstijd heeft bij het ophalen van entiteiten uit een database 
maar heeft meer technische kennis nodig van SQL. Entity Framework biedt daarentegen meer functionaliteit en een lagere leercurve 
dankzij LINQ. Het kiezen tussen Dapper en Entity Framework hangt af van de eisen van de opdrachtgevers of de developers. 
Projecten waarbij snelle ophaaltijden nodig zijn, kan beter gekozen worden voor Dapper doordat de uitvoeringstijden lager zijn dan bij Entity Framework. 
In gevallen waarbij developers zich niet willen druk maken over het schrijven van SQL code dan kan er beter gekozen worden 
voor Entity Framework waarbij er gebruikgemaakt kan worden van LINQ expressies.

Bronnen
<ul>
    <li>https://www.c-sharpcorner.com/article/dapper-vs-entity-framework-core/ - Geraadpleegd op 9 oktober 2024</li>
    <li>https://levelup.gitconnected.com/dapper-vs-ef-core-which-orm-framework-should-you-choose-for-your-net-application-54f2723b176a - Geraadpleegd op 9 oktober 2024</li>
    <li>https://www.learndapper.com/dapper-vs-entity-framework - Geraadpleegd op 9 oktober 2024</li>
    <li>https://salihcantekin.medium.com/the-big-fight-dapper-vs-entity-framework-detailed-benchmark-2345af933382 - Geraadpleegd op 9 oktober 2024</li>
    <li>https://www.learndapper.com/</li>
    <li>https://learn.microsoft.com/en-us/ef/core/querying/sql-queries?tabs=sqlserver - Geraadpleegd op 9 oktober 2024</li>
    <li>https://salihcantekin.medium.com/the-big-fight-dapper-vs-entity-framework-detailed-benchmark-2345af933382 - Geraadpleegd op 9 oktober 2024</li>`
</ul>




