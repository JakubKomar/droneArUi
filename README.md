# Vizualizační nástroj pro pilota dronu v brýlích HoloLens 2

Tento nástroj si klade za cíl usnadnit plánovací rutinu obvyklých misí a pomoci při jejím bezpečném plnění. Nástroj byl vytvořen také s ohledem na legislativní omezení provozu dronů a přispívá k jejich korektnímu dodržování. Problematika je řešena za pomocí klient-server aplikace v prostředí rozšířené reality v brýlích HoloLens 2. Pro pomoc při pilotáži autor implementoval průhledové rozhraní/HUD, z angl. head-up display, který se pohybuje spolu s dronem a okolo něho zobrazuje užitečná data - výškoměr, rychloměr a záznam z kamery.Snadnější plánování misí přináší implementovaná 3D mini-mapa, která je kopií a zmenšeninou reálného světa. V této mapě pak pilot plánuje mise, vidí pozici drona v reálném čase a veškeré informace z mapy se navíc zobrazují i v okolí pilota v reálném světě pomocí rozšířené reality.

#### Demo video:
[![Demo video](https://img.youtube.com/vi/PgQNG-16zcc/0.jpg)](https://www.youtube.com/watch?v=PgQNG-16zcc)

#### Popis aplikace:
Aplikace je typu klient-server a je napsána v herním enginu Unity s využitím knihoven VLC knihovna - https://github.com/videolan/vlc-unity}{github.com/videolan/vlc-unity, Mapbox - https://www.mapbox.com/unity}{mapbox.com/unity a ProBuilder - https://unity.com/features/probuilder. Veškerá logika je psána v jazyce C\#. Program potřebuje ke korektnímu fungování internetové připojení pro stažení aktuálních geografických dat z OpenStreet map (Mapbox), dle aktuální pozice operátora. Aplikace má globální pole působnosti a lze ji tedy plně užít kdekoliv. 

Aplikace dále vyžaduje zdroj telemetrických dat dronu a záznam kamery dronu. Ty zajišťuje telemetrický sever\footnote{Telemetrický server a RTMP video server. Díky serverovému řešení je možné připojit do tohoto ekosystému další kompatibilní aplikace, jako je například monitorovací aplikace v notebooku. Zdrojem dat pro tyto servery je upravená aplikace DJI SDK poskytující letová data, která periodicky zasílá požadovaná data na příslušné služby. Jednotlivé komponenty systému jsou propojeny Wi-Fi Hotspotem. 

Rozhraní implementované aplikace se skládá ze dvou hlavních částí. První je HUD widget, který zobrazuje letové a systémové veličiny spolu se záznamem z kamery. Tato část má za úkol snížit psychickou námahu pilota.  Druhou částí programu jsou objekty mise rozmístěné v prostoru okolo operátora, se kterými je manipulováno skrze 3D mini-mapu. Tato část má usnadnit programování a inspekci mise.

#### Návod na zprovoznění:
1. Stáhněte a nainstalujte telemetrický server a RTMP video server
 - https://github.com/robofit/drone\_server}{github.com/robofit/drone\_server
 - https://www.monaserver.ovh/
2. zkontrolujete firewallová pravidla
3. do telefonu si nainstalujte aplikaci, která zasílá data na jednotlivé servery
 - [github.com/robofit/drone\_dji\_streamer](https://github.com/robofit/drone_dji_streamer)
 - pokud nebude fungovat (velmi pravděpodobně), použijte mnou zkompilovanou aplikaci v releases
 - aplikace má tendenci padat pokud nemá spojení se servrem, zadejte ip adresu ještě před připojením vysílače
 - aplikace padá i v případě že se zakne gymbal, zkrátka vše musí perfektně fungovat jinak aplikace spadne
5. Spojení lze ověřit přes aplikaci DroCo
 - https://github.com/robofit/drone_vstool

#### Navázání spojení v Hololens:
1. V hololens 2 řekněte povel "connection", měl by se vám zobrazit panel s ipadresamy... Zadejteje a zmáčkněte tlačítko connect. Konzole lze vyvolat povelem "debug", pro ladící výpisy.
2. Úspěšné spojení by mělo být oznámeno hláškou "Connection established" a pokud je dron připojen icona přeškrtlého drona by měla zmizet.
3. Dronu je dobré zkalibrovat kompas a před spojením s ním chvilinku povílítnout a udělat jedno dvě kolečka pro ustálení GPS.
4. Proveďte kalibraci tak, že si nad drona stoupnete a koukáte se ve stejném směru jako dron. Řeknete povel (calibrate/calibration). Úspěšná kalibrace bude oznámena.
5. Vše by mělo být připraveno, případné ověření/dokalibrování lze provést povelem "soft calibration".

#### Hlasové povely:
 - "Calibration/calibrate" - dle GPS dat dronu a kompasu je proveda word-scale calibrace hráče
 - "Soft calibration" - zobrazí se budovy okolo hráče a šipky pro manuální korekci, ukončete opakováním povelu.
 - "Toggle camera" - zapnutí/vypnutí fpv módu
 - "Map" přivolá mapu
 - "Toggle map" zapne vypne mapu
 - "Debug" přivolá debugovací konzoly
 - "Connection" přivolá panel připojení
 - "Skip" přeskoč waypoint
 - "Reset" zresetuj trasu
 - "Toggle profiler" ukaž analýzu výkonosti

#### Použité zdroje v programu:
 - upravená knihovna mapbox pro hololens
https://github.com/mertusta1996/Mapbox-Hololens-2-Unity-UWP-
 - model dronu
https://sketchfab.com/3d-models/dji-mavic-3-c5a5abae1dea468ab73b1bdc7d616fa6#download
- zdroj 2D svg ikon
https://www.svgrepo.com/

#### Pužité knihovny
 - VLC knihovna - https://github.com/videolan/vlc-unity
 - Mapbox - https://www.mapbox.com/unity}
 - ProBuilder - https://unity.com/features/probuilder
 - SVG unity

