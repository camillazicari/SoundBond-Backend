<p align="center">
  <img src="frontend/src/assets/SoundBond/SoundBondViola.png" alt="Logo" width="180"/>
</p>

<h2 align="center">La tua musica, le tue connessioni.</h2>

**SoundBond** è un social network musicale che abbina utenti in base ai loro gusti. Ascolta brani, esplora nuove playlist, invia richieste e chatta con chi condivide la tua passione.

### 🚀 Funzionalità principali

- 🔒 **Proteggi la tua identità musicale** <br/>
  Registrati in modo sicuro con il sistema di autenticazione ASP.NET Identity, che mantiene al sicuro i tuoi dati mentre esplori il mondo SoundBond.

- 🎵 **Playlist che parlano di te** <br/>
  Non più playlist generiche! SoundBond crea raccolte musicali giornaliere su misura analizzando i tuoi generi preferiti, perché ogni brano rifletta davvero il tuo gusto unico.

- 🎧 **Ascolta prima, ama dopo** <br/>
  Grazie all'integrazione con l'API di Deezer, puoi scoprire anteprime dei brani direttamente nella piattaforma, senza dover saltare tra app diverse.

- 🤝 **Anima gemella musicale? Ora la trovi** <br/>
  Un algoritmo intelligente ti abbina con altri utenti che condividono la tua passione per artisti, generi o brani specifici. Chi sa che amicizie nasceranno da un riff condiviso?

- 📝 **Il tuo biglietto da visita sonoro** <br/>
  Crea un profilo che racconti chi sei attraverso la musica: aggiungi i tuoi artisti del cuore, i generi che ti fanno vibrare e i brani che non smetteresti mai di ascoltare.

- 💬 **Chatta al ritmo giusto** <br/>
  Scambia messaggi in tempo reale con i tuoi match grazie alla tecnologia SignalR, perfetta per condividere impressioni su quel nuovo album o organizzare il prossimo concerto insieme.

- 🌟 **La tua opinione modella SoundBond** <br/>
  Ogni feedback che lasci aiuta a migliorare la piattaforma, rendendola sempre più adatta alle esigenze della community musicale.

<hr/>

### 🛠️ Tech Stack

**Frontend**

- [React](https://reactjs.org/)
- [Vite](https://vitejs.dev/)
- [TailwindCSS](https://tailwindcss.com/)
- [Redux Toolkit](https://redux-toolkit.js.org/)
- [React Router DOM](https://reactrouter.com/)

**Backend**

- [ASP.NET Core Web API](https://docs.microsoft.com/en-us/aspnet/core/web-api/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/)
- [ASP.NET Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity)
- [SignalR](https://dotnet.microsoft.com/apps/aspnet/signalr)

**Servizi esterni**

- [Deezer API](https://developers.deezer.com/)

<hr/>

### 💻 Installazione e setup

**Prerequisiti**

- Node.js

- .NET 8 SDK

- SQL Server

- Visual Studio 2022 o superiore

**Frontend**

1. Clona il repository frontend:

```bash
git clone https://github.com/camillazicari/SoundBond-Frontend.git
```

2. Accedi alla cartella del progetto

```bash
cd CAPSTONE
```

3. Installa le dipendenze

```bash
npm install
```

4. Avvia il server di sviluppo

```bash
node server.js & npm start
```

**Backend**

1. Clona il repository del backend

```bash
git clone https://github.com/camillazicari/SoundBond-Backend.git
```

2. Apri la soluzione SoundBond.API in Visual Studio

3. Configura appsettings.json con la tua stringa di connessione per SQL Server

4. Applica le migration (se necessario)

```bash
$ dotnet ef database update
```

5. Avvia il progetto (assicurati che usi HTTP)

<hr/>

### 🔮 Funzionalità future

Modalità ascolto condiviso

Integrazione Spotify

<hr/>

### 👤 Autore

Sviluppato da Camilla Zicari
🔗 GitHub — camillazicari

<hr />

### 🔗 Link Utili

Frontend: https://github.com/camillazicari/SoundBond-Frontend.git
