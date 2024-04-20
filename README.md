# Vizualizační nástroj pro pilota dronu v brýlích HoloLens 2

Tento nástroj si klade za cíl usnadnit plánovací rutinu obvyklých misí a pomoci při jejím bezpečném plnění. Nástroj byl vytvořen také s ohledem na legislativní omezení provozu dronů a přispívá k jejich korektnímu dodržování. Problematika je řešena za pomocí klient-server aplikace v prostředí rozšířené reality v brýlích HoloLens 2. Pro pomoc při pilotáži autor implementoval průhledové rozhraní/HUD, z angl. head-up display, který se pohybuje spolu s dronem a okolo něho zobrazuje užitečná data - výškoměr, rychloměr a záznam z kamery.Snadnější plánování misí přináší implementovaná 3D mini-mapa, která je kopií a zmenšeninou reálného světa. V této mapě pak pilot plánuje mise, vidí pozici drona v reálném čase a veškeré informace z mapy se navíc zobrazují i v okolí pilota v reálném světě pomocí rozšířené reality.

#### Demo video:
[![Demo video](https://img.youtube.com/vi/PgQNG-16zcc/0.jpg)](https://www.youtube.com/watch?v=PgQNG-16zcc)

#### Popis aplikace:
Aplikace je typu klient-server a je napsána v herním enginu Unity s využitím knihoven VLC knihovna - https://github.com/videolan/vlc-unity}{github.com/videolan/vlc-unity, Mapbox - https://www.mapbox.com/unity}{mapbox.com/unity a ProBuilder - https://unity.com/features/probuilder. Veškerá logika je psána v jazyce C\#. Program potřebuje ke korektnímu fungování internetové připojení pro stažení aktuálních geografických dat z OpenStreet map (Mapbox), dle aktuální pozice operátora. Aplikace má globální pole působnosti a lze ji tedy plně užít kdekoliv. 

Aplikace dále vyžaduje zdroj telemetrických dat dronu a záznam kamery dronu. Ty zajišťuje telemetrický sever\footnote{Telemetrický server - https://github.com/robofit/drone\_server}{github.com/robofit/drone\_server a RTMP video server - https://www.monaserver.ovh/. Díky serverovému řešení je možné připojit do tohoto ekosystému další kompatibilní aplikace, jako je například monitorovací aplikace v notebooku. Zdrojem dat pro tyto servery je upravená aplikace DJI SDK poskytující letová data - https://github.com/robofit/drone\_dji\_streamer}{github.com/robofit/drone\_dji\_streamer, která periodicky zasílá požadovaná data na příslušné služby. Jednotlivé komponenty systému jsou propojeny Wi-Fi Hotspotem. 

Rozhraní implementované aplikace se skládá ze dvou hlavních částí. První je HUD widget, který zobrazuje letové a systémové veličiny spolu se záznamem z kamery. Tato část má za úkol snížit psychickou námahu pilota.  Druhou částí programu jsou objekty mise rozmístěné v prostoru okolo operátora, se kterými je manipulováno skrze 3D mini-mapu. Tato část má usnadnit programování a inspekci mise.

#### Návod na zprovoznění:

