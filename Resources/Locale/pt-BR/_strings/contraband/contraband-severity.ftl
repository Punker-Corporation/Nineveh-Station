contraband-examine-text-Minor = [color=yellow]Este item é considerado contrabando menor.[/color]
contraband-examine-text-Restricted = [color=yellow]Este item é restrito para uso em determinados departamentos.[/color]
contraband-examine-text-Restricted-department = [color=yellow]Este item é restrito a { $departments } e pode ser considerado contrabando.[/color]
contraband-examine-text-Major = [color=red]Este item é considerado contrabando grave.[/color]
contraband-examine-text-GrandTheft = [color=red]Este item é um objeto valioso para os inimigos da Fundação![/color]
contraband-examine-text-Syndicate = [color=crimson]Este item é contrabando altamente ilegal por inimigos da Fundação![/color]
contraband-examine-text-Magical =
    { $type ->
        *[item] [color=#b337b3] Este item é altamente ilegal contrabando mágico! [/color]
        [reagent] [color=#b337b3] Este reagente é altamente ilegal contrabando mágico! [/color]
    }

contraband-examine-text-avoid-carrying-around = [color=red][italic]Você provavelmente deve evitar usar isso visivelmente sem um bom motivo.[/italic][/color]
contraband-examine-text-in-the-clear = [color=green][italic]Você provavelmente pode usar isso à vista de todos.[/italic][/color]
contraband-examinable-verb-text = Legalidade
contraband-examinable-verb-message = Verifique a legalidade deste item.
contraband-department-plural = { $department }
contraband-job-plural = { $job }

# Chaves adicionadas para impedir qualquer fallback de localiza??o.

contraband-examine-text-Highly-Illegal =
    { $type ->
        *[item] [color=crimson] Este item é altamente ilegal contrabando! [/color]
        [reagent] [color=crimson] Este reagente é contrabando altamente ilegal! [/color]
    }
