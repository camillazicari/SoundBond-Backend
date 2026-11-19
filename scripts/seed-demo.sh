#!/usr/bin/env bash
# crea qualche utente di prova per testare match e chat
# uso: DEMO_PASSWORD=... ./scripts/seed-demo.sh [http://localhost:5220]
set -euo pipefail
API="${1:-http://localhost:5220}"
: "${DEMO_PASSWORD:?Imposta DEMO_PASSWORD (almeno 8 caratteri)}"

post() { curl -sf -X POST "$API$1" -H 'Content-Type: application/json' ${3:+-H "Authorization: Bearer $3"} -d "$2"; }
deezer_artist() { curl -sf "https://api.deezer.com/search/artist?q=$(python3 -c 'import urllib.parse,sys;print(urllib.parse.quote(sys.argv[1]))' "$1")&limit=1"; }
json() { python3 -c 'import json,sys; print(json.dumps(sys.argv[1]))' "$1"; }

seed_user() {
  local user="$1" nome="$2" cognome="$3" generi="$4" artisti="$5"
  local email="$user@demo.soundbond.test" token
  post /api/Account/register "{\"nome\":\"$nome\",\"cognome\":\"$cognome\",\"email\":\"$email\",\"dataDiNascita\":\"1998-05-10\",\"nomeUtente\":\"$user\",\"password\":$(json "$DEMO_PASSWORD")}" >/dev/null || true
  token=$(post /api/Account/login "{\"email\":\"$email\",\"password\":$(json "$DEMO_PASSWORD")}" | python3 -c 'import json,sys; print(json.load(sys.stdin)["token"])')
  curl -sf -X POST "$API/api/Profilo" -H "Authorization: Bearer $token" >/dev/null || true
  IFS=',' read -ra gs <<< "$generi";  for g in "${gs[@]}"; do post /api/Generi "{\"nome\":$(json "$g")}" "$token" >/dev/null || true; done
  IFS=',' read -ra as <<< "$artisti"
  for a in "${as[@]}"; do
    local info id img
    info=$(deezer_artist "$a")
    id=$(python3 -c 'import json,sys;d=json.loads(sys.argv[1])["data"];print(d[0]["id"] if d else "")' "$info")
    img=$(python3 -c 'import json,sys;d=json.loads(sys.argv[1])["data"];print(d[0]["picture_medium"] if d else "")' "$info")
    post /api/Artisti "{\"nome\":$(json "$a"),\"img\":$(json "$img")}" "$token" >/dev/null || true
    # brano più ascoltato dell'artista
    [ -n "$id" ] && curl -sf "https://api.deezer.com/artist/$id/top?limit=1" | python3 -c '
import json,sys
d=json.load(sys.stdin).get("data") or []
if not d: sys.exit(0)
t=d[0]
print(json.dumps({"titolo":t["title"],"artista":t["artist"]["name"],"img":t["album"]["cover_medium"]}))' \
      | { read -r body || true; [ -n "${body:-}" ] && post /api/Brani "$body" "$token" >/dev/null || true; }
  done
  echo "creato $email"
}

seed_user giulia.demo Giulia Rossi  "Pop,Rock,Indie"       "Arctic Monkeys,Dua Lipa,Måneskin"
seed_user marco.demo  Marco  Bianchi "Rock,Indie,Elettronica" "Arctic Monkeys,Daft Punk,Måneskin"
seed_user sara.demo   Sara   Verdi   "Jazz,Soul,Pop"        "Amy Winehouse,Dua Lipa,Norah Jones"
seed_user luca.demo   Luca   Neri    "Hip Hop,Rap"          "Kendrick Lamar,Marracash,Salmo"
