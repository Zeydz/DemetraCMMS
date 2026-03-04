# Demetra CMMS

![CI/CD Status](https://github.com/Zeydz/dotnet-projektuppgift/actions/workflows/ci-cd.yml/badge.svg)

Ett webbaserat CMMS (Computerized Maintenance Management System) för hantering av underhållsärenden, utrustning och tekniker.

## Om Projektet

Detta projekt är ett underhålls och servicesystem utvecklat med ASP.NET Core MVC som gör det möjligt för användare att:

- Hantera felanmälningar och serviceärenden 
- Tilldela ärenden till tekniker baserat på kompetens
- Spåra utrustning
- Följa upp ärenden med prioritering och statushantering
- Publik åtkomst för att skapa felanmälningar

---

## Funktioner

- **Ärendehantering (Tickets)**
    - Fullständig CRUD för underhållsärenden
    - Prioritetsnivåer: Critical (4h), High (24h), Medium (3d), Low (7d)
    - Statusflöde: New - Assigned - In Progress - Resolved - Closed
    - Automatisk beräkning av förfallodatum
    - Översyn över försenade ärenden

- **Utrustningshantering (Equipment)**
    - Registrera och hantera utrustning
    - Koppla till platser
    - Spåra status: Operational, Under Maintenance, Out of Service

- **Teknikerhantering**
    - Hantera tekniker med kompetenser (skills)
    - Tilldela ärenden baserat på kompetens
    - Spåra aktiva/inaktiva tekniker
    - Visa tickets per tekniker

- **Platshantering (Locations)**
    - Organisera utrustning per fysisk plats
    - Hantera adresser och beskrivningar

- **Kompetenshantering (Skills)**
    - Skapa kompetensområden
    - Tilldela kompetenser till tekniker

- **Användarhantering**
    - Rollbaserad åtkomstkontroll (Admin, Manager, Technician, User)
    - Automatisk synkronisering mellan användare och tekniker

- **Publik ticketregistrering**
    - Användare utan konto kan rapportera fel via hemsidan
    - Bekräftelsesida med ärendenummer
    - Statusspårning via ärendenummer

- **Dashboard**
    - Statistik över ärenden
    - Uppdelning efter status och prioritet
    - Utrustnings- och teknikerstatistik
    - Senaste tickets

---

## Teknisk Stack

### Backend
- **Framework:** ASP.NET Core 10.0 MVC
- **Språk:** C#
- **ORM:** Entity Framework Core 10.0
- **Autentisering:** ASP.NET Core Identity
- **Databas:** SQLite

### Frontend
- **View Engine:** Razor Pages (.cshtml)
- **CSS Framework:** Tailwind CSS

### Testing & CI/CD
- **Testing Framework:** xUnit 2.9.3
- **CI/CD:** GitHub Actions

---

## 🚀 Installation

### 1. Klona Repot

```bash
git clone https://github.com/Zeydz/DemetraCMMS.git
```

### 2. Installera NuGet-paket

```bash
dotnet restore
```

### 3. Konfigurera Databasen

Projektet använder SQLite som databas för utveckling. Konfigurationen finns i `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=maintenance.db"
  }
}
```

### 4. Skapa och Seeda Databasen

Databasen skapas automatiskt vid första körningen, men du kan också köra migreringar manuellt:

```bash
# Skapa databasen
dotnet ef database update
```

Seed-data läggs till automatiskt vid första körningen och innehåller:
- **Roller:** Admin, Manager, Technician, User
- **Administratör:** admin@maintenance.local / Admin123!
- **Platser** 
- **Utrustning** 
- **Kompetenser**
- **Tekniker**
- **Användare**
- **Ärenden**

### 5. Kör Applikationen

```bash
dotnet run
```

### 6. Logga In

Använd admin-kontot för att komma åt alla funktioner:

```
Email: admin@maintenance.local
Lösenord: Admin123!
```

---

## Databashantering

### Databasstruktur

Projektet använder **Code-First** approach med Entity Framework Core. Databasstrukturen definieras i C#-klasser under `Models/Entities/`.

#### Huvudentiteter

| Entitet | Beskrivning                                |
|---------|--------------------------------------------|
| **ApplicationUser** | Användare med autentisering                |
| **Location** | Fysiska platser                            |
| **Equipment** | Utrustning som kräver underhåll            |
| **Skill** | Kompetensområden                           |
| **Technician** | Tekniker                                   |
| **Ticket** | Underhållsärenden                          |
| **TechnicianSkill** | Kopplingar mellan tekniker och kompetenser |
| **MaintenanceSchedule** | Planerade underhåll                        |

### Migreringar

#### Skapa en migration

```bash
dotnet ef migrations add <beskrivning>
```

#### Applicera migrationer

```bash
dotnet ef database update
```

---

## Användning

### Navigering

Applikationen har två huvudlayouter:

1. **Publik Layout** (`_LayoutPublic.cshtml`)
    - Tillgänglig för alla
    - Felanmälans formulär
    - Statusspårning

2. **Admin Layout** (`_LayoutAdmin.cshtml`)
    - Kräver inloggning
    - Sidomenyn med tillgång till alla moduler
    - Dashboard

## Användarroller

### Admin
- **Behörighet:** Full åtkomst till alla funktioner
- **Kan:**
    - Hantera alla användare
    - Skapa, redigera, ta bort alla tickets
    - Hantera utrustning, platser, kompetenser, tekniker
    - Tilldela roller

### Manager
- **Behörighet:** Hantering av tickets och resurser
- **Kan:**
    - Hantera ärenden, utrustning, platser, kompetenser, tekniker
    - Tilldela ärenden till tekniker
- **Kan inte:**
    - Hantera användarkonton

### Technician
- **Behörighet:** Arbetsfokuserad
- **Kan:**
    - Se tilldelade ärenden
    - Uppdatera ärendestatus
    - Se utrustning
- **Kan inte:**
    - Skapa ärenden
    - Hantera andra entiteter

### User
- **Behörighet:** Grundläggande
- **Kan:**
    - Skapa egna ärenden
    - Se sina ärenden
- **Kan inte:**
    - Se andras ärenden
    - Hantera system

---

## CI/CD Pipeline

Projektet använder **GitHub Actions** som CI/CD-pipeline.

### Workflow

Pipeline körs automatiskt vid:
- Push till `main`
- Pull requests

### Jobb

#### 1. Build and Test
- Installerar .NET 10.0
- Laddar ned beroenden
- Bygger projektet
- Kör unit tests (xUnit)
- Validerar migreringar
- Publicerar applikation
- Laddar upp artifacts

#### 2. Code Quality
- Kontrollerar kodformatering
- Skannar sårbarheter i packages

### Köra tests lokalt

```bash
# Kör alla tester
dotnet test
```
---

## Säkerhet

### Implementerade säkerhetsåtgärder

- **Autentisering:** ASP.NET Core Identity med hashade lösenord
- **Auktorisering:** Rollbaserad åtkomstkontroll via `[Authorize]`
- **CSRF-skydd:** `[ValidateAntiForgeryToken]` på alla POST-formulär
- **SQL Injection:** EF Core sköter detta automatiskt
- **Input-validering:** Data Annotations och ModelState
-
---

Skapat av Joakim Möller