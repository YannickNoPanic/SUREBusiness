# SUREbusiness - Wagenpark API

Gefeliciteerd met het bereiken van deze opdracht!  
Dit is een volledige backend implementatie voor het bijhouden van een wagenpark, gebouwd met aandacht voor schoonheid, onderhoudbaarheid en schaalbaarheid.

## Architectuur
- **Clean Architecture** met 3 lagen:
  - **API**: Controllers, DTO's, Mapping
  - **Core**: Entities, Use Cases, Business rules
  - **Infrastructure**: EF Core, Repository pattern, Query objects (CQRS-light), RDW service
- Use Case pattern voor duidelijke business logica
- Scheiding tussen commands (mutaties) en queries (read-only)
- Volledig getest op alle lagen

## Features
- Auto toevoegen met **RDW kentekenvalidatie** (bonus)
- Flexibele update: uitlenen, terugbrengen, status wijzigen, opmerkingen toevoegen
- **Gefilterde en gepagineerde lijst** opvragen op huidige bezitter en status (bonus)
- Business rules:
  - Uitlenen alleen als auto beschikbaar is
  - Status van verkochte auto mag niet meer gewijzigd worden
  - Verkocht maken wist automatisch uitgeleend-aan gegevens
- Automatisch **100 dummy auto's** bij eerste start

## Technologieën
- .NET 8
- Entity Framework Core met SQLite (development)
- AutoFixture + NSubstitute + FluentAssertions voor tests
- Clean Architecture + Use Case + Repository + Query pattern

## Hoe te draaien
```bash
dotnet restore
dotnet run --project src/SUREBusiness.API