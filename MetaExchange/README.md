# MetaExchange – Test

Projekt je zasnovan po načelih **Clean Architecture** in uporablja **.NET 9.0** ter je razdeljen na konzolno aplikacijo in spletni API (Web API).

---

## Tehnologije in arhitektura

- **.NET 9.0**
- **Clean Architecture**
- **Docker Compose** za lokalno orkestracijo
- **E2E testi** z `WebApplicationFactory` in `HttpClient`
- **TestData** folder za prednastavljene JSON podatke

---

## Zagon aplikacije

### 1. Console aplikacija (lokalni CLI)

Zaženi iz root mape:

```bash
dotnet run --project MetaExchange.ConsoleApp
```

✅ Po zagonu boš v terminalu pozvan k vnosu:
- Tip naročila: `Buy` ali `Sell`
- Količina BTC

Rezultati bodo prikazani v terminalu.

---

### 2. Web API

#### Zagon preko Docker Compose

```bash
docker-compose up --build
```

🌐 API bo dostopen na:

```
http://localhost:5021/api/exchange/match
```

#### Primer POST zahteve (npr. v Postmanu)

```http
POST http://localhost:5021/api/exchange/match
Content-Type: application/json
```

```json
{
  "type": "Buy",
  "amount": 1.0
}
```
