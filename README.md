### Kjøreinstruksjoner

Åpne løsningen i Visual Studio 2022 eller nyere og start applikasjonen med F5 eller Ctrl+F5. Swagger åpnes automatisk ved oppstart.
For endepunkter som krever autentisering må det først hentes et JWT-token via /auth/token, som deretter legges inn ved å klikke Authorize i Swagger.

# Testbrukere

Admin:
admin / admin123

Operator:
operator / operator123

Guest:
guest / guest123


### Arkitekturvalg

Oppgavebeskrivelsen foreslo et Infrastructure-lag, og det ville vært naturlig å ha dette hvis jeg hadde implementert en database. Men siden jeg valgte in-memory persistens av tidshensyn, finnes ikke noe Infrastructure-lag i denne løsningen. I et oppdrag for en reell kunde ville persistensansvaret blitt flyttet til et eget Infrastructure-lag der databaseintegrasjonen hadde blitt implementert, mens Application-laget kun ville kjent til nødvendige kontrakter for å hente og lagre data. Dette ville gitt en tydeligere separasjon mellom applikasjonslogikk og tekniske detaljer knyttet til persistens. Men for denne case-oppgaven vurderte jeg at den ekstra kompleksiteten ikke ga tilstrekkelig verdi sammenlignet med en enklere in-memory løsning.

Oppgavebeskrivelsen foreslo også at Application-laget skulle inneholde use cases og DTO-er, men jeg valgte å holde implementasjonen enklere fordi jeg hadde begrenset med tid. Use casene er i praksis implementert som metoder i ElevatorService, og DTO'ene ligger i Models-mappen i Api-laget fordi de nå kun brukes som modeller for HTTP-endepunktene. I et reelt produksjonsscenario ville det vært naturlig å ha applikasjonskontraker i Application-laget og mappe mellom Application-DTO'er og HTTP-modeller i Api-laget.

I denne case-oppgaven er det ikke strengt nødvendig med interface hverken for ElevatorDispatcher eller ElevatorService. Jeg valgte likevel interface på dispatcheren fordi algoritmen er oppgavens mest sentrale og mest naturlig utskiftbare komponent. ElevatorService har foreløbig bare én konkret rolle som applikasjonstjeneste, så jeg lot være å abstrahere den for å unngå unødvendig kompleksitet.


### Dispatchalgoritme

Når systemet mottar en heisforespørsel, filtreres først heiser som er i tilstanden Maintenance eller OutOfService bort. For de tilgjengelige heisene beregnes deretter en kostnad basert på følgende enkle prinsipper:
* Avstanden mellom heisen og forespørselsetasjen gir en kostnad på 1 per etasje. 
* Hvis heisen beveger seg i motsatt retning av forespørselen, legges det til 10 i kostnad. 
* En Idle heis får ingen ekstra kostnad.
* En heis i tilstanden DoorsOpen får 1 i ekstra kostnad.

Med mer tid ville det vært opplagt å fokusere på forbedring av algoritmen. Forbedringer kunne vært:
* Skille mellom heiser som er på vei for å betjene en aktiv forespørsel og heiser som kun flyttes automatisk tilbake til lobbyen.
* Ta hensyn til antall planlagte stopp på veien til forespørselen.
* Analysere trafikkmønstre og bruke dette til å plassere heiser proaktivt der etterspørselen forventes å oppstå.


# Lobbypreferanse

For å oppfylle kravet om at minimum 4 heiser skal stå klare i lobbyen ved lav aktivitet, teller systemet hvor mange Idle heiser som befinner seg i lobbyetasjen. 
* Hvis 4 eller flere heiser i tilstanden Idle allerede står i lobbyen, gjøres ingenting. 
* Hvis færre enn 4 Idle heiser står i lobbyen, velges maks 4 av de nærmeste Idle heisene som står i andre etasjer og sendes tilbake til lobbyen. 

Med mer tid kunne også lobbylogikken blitt forbedret. Slik den er implementert nå, kan det i perioder med svært lav aktivitet oppstå situasjoner der alle heisene ender opp som Idle i lobbyen. Mulige forbedringer:
* Sikre at hvis 8 heiser er Idle, står minst to av dem i andre etasjer enn lobbyen.
* Ta høyde for at heisbehovet varierer gjennom døgnet. I visse tidsrom om morgenen bør heisene for eksempel stå klare til å frakte gjestene fra rommene og ned til lobbyen eller etasjen der frokosten serveres. Her også kunne trafikkmønstre blitt analysert og brukt til å plassere heisene mer proaktivt der etterspørselen forventes å oppstå.
