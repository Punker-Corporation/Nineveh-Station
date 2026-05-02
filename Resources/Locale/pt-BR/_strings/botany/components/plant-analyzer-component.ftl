plant-analyzer-component-no-seed = planta não encontrada
plant-analyzer-component-health = Saúde:
plant-analyzer-component-age = Idade:
plant-analyzer-component-water = Água:
plant-analyzer-component-nutrition = Itens alimentares:
plant-analyzer-component-toxins = Toxinas:
plant-analyzer-component-pests = Pragas:
plant-analyzer-component-weeds = Ervas daninhas:
plant-analyzer-component-alive = [color=green]AO VIVO[color]
plant-analyzer-component-dead = [color=red]MORTO[color]
plant-analyzer-component-unviable = [color=red]GENE DA MORTE[color]
plant-analyzer-component-mutating = [color=#00ff5f] MUTAR [color]
plant-analyzer-component-kudzu = ZXQ0QZ KUDZU [color]
plant-analyzer-soil = Existem produtos químicos não absorvidos neste { $holder }: [color=white]{ $chemicals }[/color].
plant-analyzer-soil-empty = Não há produtos químicos não absorvidos neste { $holder }.
plant-analyzer-component-environemt = Este [color=green]{ $seedName }[/color] requer uma atmosfera a um nível de pressão de [color=lightblue]{ $kpa }kPa ± { $kpaTolerance }kPa[/color], uma temperatura de [color=lightsalmon]{ $temp }°k ± { $tempTolerance }°k[/color] e um nível de luz de [color=white]{ $lightLevel } ± { $lightTolerance }[/color].
plant-analyzer-component-environemt-void = Este [color=green]{ $seedName }[/color] deve ser cultivado [bolditalic]no vácuo do espaço[/bolditalic] a um nível de luz de [color=white]{ $lightLevel } ± { $lightTolerance }[/color].
plant-analyzer-component-environemt-gas = Este [color=green]{ $seedName }[/color] requer uma atmosfera contendo [bold]{ $gases }[/bold] a um nível de pressão de [color=lightblue]{ $kpa }kPa ± { $kpaTolerance }kPa[/color], uma temperatura de [color=lightsalmon]{ $temp }°k ± { $tempTolerance }°k[/color] e um nível de luz de [color=white]{ $lightLevel } ± { $lightTolerance }[/color].
plant-analyzer-produce-plural = { $thing }
plant-analyzer-output = {$yield ->
    [0]{$gasCount ->
        [0]A única coisa que parece fazer é consumir água e nutrientes.
        *[other]A única coisa que parece fazer é transformar água e nutrientes em [bold] {$gases} [/bold].
    }
    *[other]Tem [color=lightgreen] {$yield} {$potency} [/color] {$seedless ->
        [true]{" "} mas [color=red] sem sementes [/color]
        *[false]{$nothing}
    }{" "}{$yield ->
        [one]flor
        *[other]flores
    }{" "}that{$gasCount ->
        [0]{$nothing}
        *[other]{$yield ->
            [one]{" "} emite
            *[other]{" "} emite
        } {" "} [bold] ZXQ2QZ [/bold] e
    }{" "}will turn into{$yield ->
        [one]{" "} {INDEFINITE($firstProduce)} [color=#a4885c] {$produce} [/color]
        *[other]{" "} ZXQ1QZ {$producePlural} [/color]
    }.{$chemCount ->
        [0]{$nothing}
        *[other]{" "} Existem vestígios de [color=white] {$chemicals} [/color] no seu caule.
    }
}
plant-analyzer-potency-tiny = microscópico
plant-analyzer-potency-small = pequeno
plant-analyzer-potency-below-average = tamanho abaixo da média
plant-analyzer-potency-average = tamanho médio
plant-analyzer-potency-above-average = tamanho acima da média
plant-analyzer-potency-large = bem grande
plant-analyzer-potency-huge = enorme
plant-analyzer-potency-gigantic = gigantesco
plant-analyzer-potency-ludicrous = ridiculamente grande
plant-analyzer-potency-immeasurable = imensamente grande
plant-analyzer-print = Selo
plant-analyzer-printout-missing = N / D
plant-analyzer-printout =
    {"[color=#9FED58][head=2]Plant Analyzer Report[/head][/color]"}
    ──────────────────────────────
    Espécie {"[bullet/]"}: {$seedName}
    {"    "}[bullet/] Viable: {$viable ->
        [no][color=red] Sem [/color]
        [yes][color=green] Sim [/color]
        *[other]{LOC("plant-analyzer-printout-missing")}
    }
    ZXQ0QZ [bullet/] Endurance: {$endurance}
    {" "} [bullet/] Lifespan: {$lifespan}
    {" "} [bullet/] Produto: [color=#a4885c] {$produce} [/color]
    {"    "}[bullet/] Kudzu: {$kudzu ->
        [no][color=green] Sem [/color]
        [yes][color=red] Sim [/color]
        *[other]{LOC("plant-analyzer-printout-missing")}
    }
    {"[bullet/]"} Perfil de crescimento:
    {" "} [bullet/] Água: [color=cyan] {$water} [/color]
    {" "} [bullet/] Nutrição: [color=orange] {$nutrients} [/color]
    {" "} [bullet/] Toxinas: [color=yellowgreen] ZXQ3QZ [/color]
    {" "} [bullet/] Pestes: [color=magenta] {$pests} [/color]
    {" "} [bullet/] Ervas daninhas: [color=red] {$weeds} [/color]
    {"[bullet/]"} Perfil ambiental:
    {" "} [bullet/] Composição: [bold] {$gasesIn} [/bold]
    {" "} [bullet/] Pressão: [color=lightblue] {$kpa} kPa ± {$kpaTolerance} kPa [/color]
    {" "} [bullet/] Temperatura: [color=lightsalmon] {$temp} K ± {$tempTolerance} K [/color]
    {" "} [bullet/] Luz: [color=gray] [bold] {$lightLevel} ± {$lightTolerance} [/bold] [/color]
    {"[bullet/]"} Flowers: {$yield ->
        [-1]{LOC("plant-analyzer-printout-missing")}
        [0][color=red] 0 [/color]
        *[other][color=lightgreen] ZXQ1QZ {$potency} [/color]
    }
    {"[bullet/]"} Seeds: {$seeds ->
        [no][color=red] Sem [/color]
        [yes][color=green] Sim [/color]
        *[other]{LOC("plant-analyzer-printout-missing")}
    }
    {"[bullet/]"} Produtos químicos: [color=gray] [bold] {$chemicals} [/bold] [/color]
    {"[bullet/]"} Emissões: [bold] {$gasesOut} [/bold]
