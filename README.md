# Domsjö Pingisklubb - Web App

Detta projekt är en ASP.NET Web App för en påhittad Bordtennisklubb, skapad som en del av en gruppuppgift i kursen Agila-arbetsmetoder. Applikationen hanterar bordtennismatcher och är utvecklad enligt agila principer (Scrum) i en teammiljö.

## Innehållsförteckning
- [Beskrivning av projektet](#beskrivning-av-projektet)
- [Tekniker och arkitektur](#tekniker-och-arkitektur)
- [Design och UX](#design-och-ux)
- [Metoder, patterns och principer](#metoder-patterns-och-principer)
- [Viktiga funktioner](#viktiga-funktioner)
- [Kravspecifikation](#kravspecifikation)
- [Trello Board](#trello-board)
- [Installation](#installation)
- [Team](#team)
- [Kontakt](#kontakt)

---

## Beskrivning av projektet

Denna webapplikation är utvecklad för att hantera bordtennismatcher. Användare kan:
- Starta en match med valda spelare, spela set och registrera poäng.
- Se historik över tidigare spelade matcher med sökfunktion (låst för admin).
- Se statistik för spelare samt jämföra spelare mot varandra (låst för admin).
- Visa topplistor över spelare.
- Administrera spelare och matcher (låst för admin).

Projektet är byggt med Razor Pages och Entity Framework Code First, samt en professionell och användarvänlig design.

---

## Tekniker och arkitektur

- **ASP.NET Core Razor Pages**: För frontend och backend.
- **Entity Framework Core (Code First)**: För databasåtkomst.
- **SQL Server**: Som databas.
- **Identity**: För inloggning och behörighetskontroll.
- **GitHub**: Versionshantering med feature branches och relevanta commit-meddelanden.
- **Trello**: För uppföljning av arbetsuppgifter (Scrum Board).

---

## Design och UX

- Färger, fonter och ikoner är hämtade från Ängby Sportklubbs befintliga hemsida för att skapa en enhetlig känsla.
- Navbar och Footer imiterar Ängby Sportklubbs original med sociala media-länkar och sponsorer.
- LandingPage, Login, LiveMatch, Matchhistorik, Spelarstatistik och Top10 är implementerade enligt designkraven för att ge en enkel och logisk användarupplevelse.

---

## Metoder, patterns och principer

Projektet använder flera professionella metoder och designprinciper:

- **Objektorienterad programmering (OOP)**: Små metoder och väl namngivna klasser och variabler samt DRY-principen.
- **Libraries**: Affärslogik separerad i services för bättre testbarhet och underhåll.
- **Services**: Hanterar affärslogik och datalagring.
- **Dependency Injection (DI)**: För modulärt och testbart beroendehantering.
- **DTOs (Data Transfer Objects)**: För att separera datalager från presentation.
- **ViewModels**: För att hantera data mellan PageModels och Views.
- **ModelState**: För validering av användarinmatning.
- **Scrum**: Sprintar på en vecka, inklusive Sprint Planning, Daily Standups, Sprint Review och Retrospective.

---

## Viktiga funktioner

- CRUD för bordtennismatch (Create, Read, Delete). Viss möjlighet till (Update).
- Registrering av poäng i realtid under match.
- Kontroll av när ett set är över (först till 11 poäng, minst 2 poängs marginal).
- Möjlighet att spela bäst av 3, 5 eller 7 set.
- Automatisk uppdatering av databas efter varje set och när matchen är vunnen.
- Sökning på spelare och visning av deras matchhistorik.
- Topplista för spelare.

---

## Kravspecifikation

Projektet uppfyller krav från uppgiften:
- Funktionalitet för matchadministration, poänghantering och setvinst.
- OOP, Libraries, Services, DI, DTOs, ViewModels, ModelState.
- UX/UI med professionell design och god användarupplevelse.
- GitHub med feature branches och relevanta commit-meddelanden.
- Scrum med sprintar, dagliga standups och sprint reviews.

---

## Trello Board

Länk till vår Trello Board: inaktuell

---

## Installation

1. Klona detta repository från GitHub.
2. Öppna projektet i Visual Studio.
3. Bygg projektet (Build).
4. Starta applikationen med IIS Express.
5. Logga in som admin med:
   - **Användarnamn**: admin@domsjo.net
   - **Lösenord**: *Admin100

---

## Team

Projektet utvecklades av ett team på 5 utvecklare. Vår lärare agerade Product Owner och en av oss agerade Scrum Master.
- Jon Johansson (Scrum Master)
- Emma Lübke
- Tova Dalin
- Alexandra Sundqvist
- Max Berridge

---

## Kontakt

För frågor eller feedback, kontakta projektets Scrum Master på jonjohansson88@gmail.com

---

