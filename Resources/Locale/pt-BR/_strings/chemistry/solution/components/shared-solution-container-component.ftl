shared-solution-container-component-on-examine-main-text = Contém {INDEFINITE($desc)} [color={$color}]{$desc}[/color] { $chemCount ->
    [1] química.
   *[other] mistura de produtos químicos.
    }

examinable-solution-has-recognizable-chemicals = Você pode reconhecer {$recognizedString} na solução.
examinable-solution-recognized = [color={$color}]{$chemical}[/color]

examinable-solution-on-examine-volume = A solução contida é { $fillLevel ->
    [exact] Exploração [color=white]{$current}/{$max}u[/color].
   *[other] [bold]{ -solution-vague-fill-level(fillLevel: $fillLevel) }[/bold].
}

examinable-solution-on-examine-volume-no-max = A solução contida é { $fillLevel ->
    [exact] Exploração [color=white]{$current}u[/color].
   *[other] [bold]{ -solution-vague-fill-level(fillLevel: $fillLevel) }[/bold].
}

examinable-solution-on-examine-volume-puddle = A poça é { $fillLevel ->
    [exact] [color=white]{$current}u[/color].
    [full] enorme e transbordante!
    [mostlyfull] enorme e transbordante!
    [halffull] profundo e fluindo.
    [halfempty] Muito profundo.
   *[mostlyempty] A juntar-se.
    [empty] formando várias pequenas piscinas.
}

-solution-vague-fill-level =
    { $fillLevel ->
        [full] [color=white]Completo[/color]
        [mostlyfull] [color=#DFDFDF]Principalmente Cheio[/color]
        [halffull] [color=#C8C8C8]Metade Completo[/color]
        [halfempty] [color=#C8C8C8]Meio Vazio[/color]
        [mostlyempty] [color=#A4A4A4]Vazio principalmente[/color]
       *[empty] [color=gray]Vazio[/color]
    }
