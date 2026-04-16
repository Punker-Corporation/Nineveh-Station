shared-solution-container-component-on-examine-main-text = Ele contém [color={ $color }]{ $desc }[/color] { $chemCount -> 
    [1] вещество.
   *[other] смесь веществ.
 }

examinable-solution-has-recognizable-chemicals = Você reconhecerá a composição: { $recognizedString }.
examinable-solution-recognized = [color={ $color }]{ $chemical }[/color]

examinable-solution-on-examine-volume = Contêiner { $fillLevel -> 
    [exact] содержит [color=branco]{ $current }/{ $max }unidade[/color].
   *[other] [bold]{ -solution-vague-fill-level(fillLevel: $fillLevel) }[/bold].
 }

examinable-solution-on-examine-volume-no-max = No contêiner { $fillLevel -> 
    [exact] содержится [color=branco]{ $current }unidade[/color].
   *[other] [bold]{ -solution-vague-fill-level(fillLevel: $fillLevel) }[/bold].
 }

examinable-solution-on-examine-volume-puddle = Poça { $fillLevel -> 
    [exact] [color=branco]{ $current }u[/color].
    [full] огромная и переливается!
    [mostlyfull] огромная и переливается!
    [halffull] глубокая и растекается.
    [halfempty] очень глубокая.
   *[mostlyempty] собирается в пятна.
    [empty] распалась на мелкие капли.
 }

-solution-vague-fill-level = 
    { $fillLevel -> 
    [full] [color=branco]completo[/color]
    [mostlyfull] [color=#DFDFDF]почти полный[/color]
    [halffull] [color=#C8C8C8]наполовину полный[/color]
    [halfempty] [color=#C8C8C8]наполовину пустой[/color]
    [mostlyempty] [color=#A4A4A4]почти пустой[/color]
   *[empty] [color=cinza]vazio[/color]
 } 
