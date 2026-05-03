-create-3rd-person =
    { $chance ->
        [1] Cria
        *[other] criar
    }

-cause-3rd-person =
    { $chance ->
        [1] Causas
        *[other] causa
    }

-satiate-3rd-person =
    { $chance ->
        [1] Satiatos
        *[other] satiato
    }

entity-effect-guidebook-spawn-entity =
    { $chance ->
        [1] Cria
        *[other] criar
    } { $amount ->
        [1] {INDEFINITE($entname)}
        *[other] {$amount} {MAKEPLURAL($entname)}
    }

entity-effect-guidebook-destroy =
    { $chance ->
        [1] Destrui
        *[other] destruir
    } o objeto

entity-effect-guidebook-break =
    { $chance ->
        [1] Quebras
        *[other] pausa
    } o objeto

entity-effect-guidebook-explosion =
    { $chance ->
        [1] Causas
        *[other] causa
    } uma explosão

entity-effect-guidebook-emp =
    { $chance ->
        [1] Causas
        *[other] causa
    } um impulso electromagnético

entity-effect-guidebook-flash =
    { $chance ->
        [1] Causas
        *[other] causa
    } um flash ofuscante

entity-effect-guidebook-foam-area =
    { $chance ->
        [1] Cria
        *[other] criar
    } grandes quantidades de espuma

entity-effect-guidebook-smoke-area =
    { $chance ->
        [1] Cria
        *[other] criar
    } grandes quantidades de fumo

entity-effect-guidebook-satiate-thirst =
    { $chance ->
        [1] Satiatos
        *[other] satiato
    } { $relative ->
        [1] sede em média
        *[other] sede em {NATURALFIXED($relative, 3)}x a taxa média
    }

entity-effect-guidebook-satiate-hunger =
    { $chance ->
        [1] Satiatos
        *[other] satiato
    } { $relative ->
        [1] fome média
        *[other] fome em {NATURALFIXED($relative, 3)}x a taxa média
    }

entity-effect-guidebook-health-change =
    { $chance ->
        [1] { $healsordeals ->
                [heals] Curas
                [deals] Ofertas
                *[both] Modifica a saúde
             }
        *[other] { $healsordeals ->
                    [heals] curar
                    [deals] negócio
                    *[both] alterar a saúde por
                 }
    } { $changes }

entity-effect-guidebook-even-health-change =
    { $chance ->
        [1] { $healsordeals ->
            [heals] Cura uniformemente
            [deals] Acordos uniformes
            *[both] Modifica uniformemente a saúde por
        }
        *[other] { $healsordeals ->
            [heals] curar uniformemente
            [deals] de acordo uniforme
            *[both] modificar uniformemente a saúde
        }
    } { $changes }

entity-effect-guidebook-status-effect-old =
    { $type ->
        [update]{ $chance ->
                    [1] Causas
                     *[other] causa
                 } {LOC($key)} pelo menos {NATURALFIXED($time, 3)} {MANY("second", $time)} sem acumulação
        [add]   { $chance ->
                    [1] Causas
                    *[other] causa
                } {LOC($key)} pelo menos {NATURALFIXED($time, 3)} {MANY("second", $time)} com acumulação
        [set]  { $chance ->
                    [1] Causas
                    *[other] causa
                } {LOC($key)} em vez de {NATURALFIXED($time, 3)} {MANY("second", $time)} sem acumulação
        *[remove]{ $chance ->
                    [1] Remove
                    *[other] remover
                } {NATURALFIXED($time, 3)} {MANY("second", $time)} de {LOC($key)}
    }

entity-effect-guidebook-status-effect =
    { $type ->
        [update]{ $chance ->
                    [1] Causas
                    *[other] causa
                 } {LOC($key)} pelo menos {NATURALFIXED($time, 3)} {MANY("second", $time)} sem acumulação
        [add]   { $chance ->
                    [1] Causas
                    *[other] causa
                } {LOC($key)} pelo menos {NATURALFIXED($time, 3)} {MANY("second", $time)} com acumulação
        [set]  { $chance ->
                    [1] Causas
                    *[other] causa
                } {LOC($key)} pelo menos {NATURALFIXED($time, 3)} {MANY("second", $time)} sem acumulação
        *[remove]{ $chance ->
                    [1] Remove
                    *[other] remover
                } {NATURALFIXED($time, 3)} {MANY("second", $time)} de {LOC($key)}
    } { $delay ->
        [0] imediatamente
        *[other] após uma {NATURALFIXED($delay, 3)} segundo atraso
    }

entity-effect-guidebook-status-effect-indef =
    { $type ->
        [update]{ $chance ->
                    [1] Causas
                    *[other] causa
                 } permanente {LOC($key)}
        [add]   { $chance ->
                    [1] Causas
                    *[other] causa
                } permanente {LOC($key)}
        [set]  { $chance ->
                    [1] Causas
                    *[other] causa
                } permanente {LOC($key)}
        *[remove]{ $chance ->
                    [1] Remove
                    *[other] remover
                } {LOC($key)}
    } { $delay ->
        [0] imediatamente
        *[other] após uma {NATURALFIXED($delay, 3)} segundo atraso
    }

entity-effect-guidebook-knockdown =
    { $type ->
        [update]{ $chance ->
                    [1] Causas
                    *[other] causa
                    } {LOC($key)} pelo menos {NATURALFIXED($time, 3)} {MANY("second", $time)} sem acumulação
        [add]   { $chance ->
                    [1] Causas
                    *[other] causa
                } knockdown para pelo menos {NATURALFIXED($time, 3)} {MANY("second", $time)} com acumulação
        *[set]  { $chance ->
                    [1] Causas
                    *[other] causa
                } knockdown para pelo menos {NATURALFIXED($time, 3)} {MANY("second", $time)} sem acumulação
        [remove]{ $chance ->
                    [1] Remove
                    *[other] remover
                } {NATURALFIXED($time, 3)} {MANY("second", $time)} de nocaute
    }

entity-effect-guidebook-set-solution-temperature-effect =
    { $chance ->
        [1] Conjuntos
        *[other] definido
    } a temperatura da solução para exatamente {NATURALFIXED($temperature, 2)}k

entity-effect-guidebook-adjust-solution-temperature-effect =
    { $chance ->
        [1] { $deltasign ->
                [1] Adiciona
                *[-1] Remove
            }
        *[other]
            { $deltasign ->
                [1] adicionar
                *[-1] remover
            }
    } calor da solução até atingir { $deltasign ->
                [1] no máximo {NATURALFIXED($maxtemp, 2)}k
                *[-1] pelo menos {NATURALFIXED($mintemp, 2)}k
            }

entity-effect-guidebook-adjust-reagent-reagent =
    { $chance ->
        [1] { $deltasign ->
                [1] Adiciona
                *[-1] Remove
            }
        *[other]
            { $deltasign ->
                [1] adicionar
                *[-1] remover
            }
    } {NATURALFIXED($amount, 2)}u de {$reagent} { $deltasign ->
        [1] para
        *[-1] de
    } a solução

entity-effect-guidebook-adjust-reagent-group =
    { $chance ->
        [1] { $deltasign ->
                [1] Adiciona
                *[-1] Remove
            }
        *[other]
            { $deltasign ->
                [1] adicionar
                *[-1] remover
            }
    } {NATURALFIXED($amount, 2)}u de reagentes no grupo {$group} { $deltasign ->
            [1] para
            *[-1] de
        } a solução

entity-effect-guidebook-adjust-temperature =
    { $chance ->
        [1] { $deltasign ->
                [1] Adiciona
                *[-1] Remove
            }
        *[other]
            { $deltasign ->
                [1] adicionar
                *[-1] remover
            }
    } {POWERJOULES($amount)} de calor { $deltasign ->
            [1] para
            *[-1] de
        } O corpo em que está

entity-effect-guidebook-chem-cause-disease =
    { $chance ->
        [1] Causas
        *[other] causa
    } a doença { $disease }

entity-effect-guidebook-chem-cause-random-disease =
    { $chance ->
        [1] Causas
        *[other] causa
    } Doenças { $diseases }

entity-effect-guidebook-jittering =
    { $chance ->
        [1] Causas
        *[other] causa
    } agitação

entity-effect-guidebook-clean-bloodstream =
    { $chance ->
        [1] Limpa
        *[other] limpar
    } a corrente sanguínea de outros produtos químicos

entity-effect-guidebook-cure-disease =
    { $chance ->
        [1] Curas
        *[other] cura
    } doenças

entity-effect-guidebook-eye-damage =
    { $chance ->
        [1] { $deltasign ->
                [1] Ofertas
                *[-1] Curas
            }
        *[other]
            { $deltasign ->
                [1] negócio
                *[-1] curar
            }
    } lesões oculares

entity-effect-guidebook-vomit =
    { $chance ->
        [1] Causas
        *[other] causa
    } vómitos

entity-effect-guidebook-create-gas =
    { $chance ->
        [1] Cria
        *[other] criar
    } { $moles } { $moles ->
        [1] mole
        *[other] moles
    } de { $gas }

entity-effect-guidebook-drunk =
    { $chance ->
        [1] Causas
        *[other] causa
    } bebedeira

entity-effect-guidebook-electrocute =
    { $chance ->
        [1] { $stuns ->
            [true] Eletrocutos
            *[false] Choques
            }
        *[other] { $stuns ->
            [true] eletrocuta
            *[false] choque
            }
    } o metabolizador para {NATURALFIXED($time, 3)} {MANY("second", $time)}

entity-effect-guidebook-emote =
    { $chance ->
        [1] Forçar a vontade
        *[other] força
    } o metabolizador para [bold][color=white]{$emote}[/color][/bold]

entity-effect-guidebook-extinguish-reaction =
    { $chance ->
        [1] Extinções
        *[other] extinguir
    } fogo

entity-effect-guidebook-flammable-reaction =
    { $chance ->
        [1] Aumentos
        *[other] aumento
    } inflamabilidade

entity-effect-guidebook-ignite =
    { $chance ->
        [1] Ignições
        *[other] inflamar
    } o metabolizador

entity-effect-guidebook-make-sentient =
    { $chance ->
        [1] Marcas
        *[other] make
    } o senciente do metabolizador

entity-effect-guidebook-make-polymorph =
    { $chance ->
        [1] Polimorfos
        *[other] polimorfo
    } o metabolizador em uma { $entityname }

entity-effect-guidebook-modify-bleed-amount =
    { $chance ->
        [1] { $deltasign ->
                [1] Induz
                *[-1] Reduz
            }
        *[other] { $deltasign ->
                    [1] induzir
                    *[-1] reduzir
                 }
    } hemorragia

entity-effect-guidebook-modify-blood-level =
    { $chance ->
        [1] { $deltasign ->
                [1] Aumentos
                *[-1] Diminuições
            }
        *[other] { $deltasign ->
                    [1] aumentos
                    *[-1] diminuições
                 }
    } nível sanguíneo

entity-effect-guidebook-paralyze =
    { $chance ->
        [1] Paralisação
        *[other] paralisação
    } o metabolizador para pelo menos {NATURALFIXED($time, 3)} {MANY("second", $time)}

entity-effect-guidebook-movespeed-modifier =
    { $chance ->
        [1] Modifica
        *[other] modificar
    } velocidade de movimento por {NATURALFIXED($sprintspeed, 3)}x para pelo menos {NATURALFIXED($time, 3)} {MANY("second", $time)}

entity-effect-guidebook-reset-narcolepsy =
    { $chance ->
        [1] Caracteres temporários
        *[other] temporariamente stave
    } de narcolepsia

entity-effect-guidebook-wash-cream-pie-reaction =
    { $chance ->
        [1] Lavagens
        *[other] lavar
    } torta de creme fora do rosto

entity-effect-guidebook-cure-zombie-infection =
    { $chance ->
        [1] Curas
        *[other] cura
    } uma infecção zumbi em curso

entity-effect-guidebook-cause-zombie-infection =
    { $chance ->
        [1] Dá
        *[other] dar
    } um indivíduo a infecção zumbi

entity-effect-guidebook-innoculate-zombie-infection =
    { $chance ->
        [1] Curas
        *[other] cura
    } uma infecção zumbi em curso, e fornece imunidade para futuras infecções

entity-effect-guidebook-reduce-rotting =
    { $chance ->
        [1] Regenera
        *[other] regenerar
    } {NATURALFIXED($time, 3)} {MANY("second", $time)} de apodrecimento

entity-effect-guidebook-area-reaction =
    { $chance ->
        [1] Causas
        *[other] causa
    } uma reacção de fumo ou espuma para {NATURALFIXED($duration, 3)} {MANY("second", $duration)}

entity-effect-guidebook-add-to-solution-reaction =
    { $chance ->
        [1] Causas
        *[other] causa
    } {$reagent} a adicionar ao seu recipiente de solução interna

entity-effect-guidebook-artifact-unlock =
    { $chance ->
        [1] Ajuda
        *[other] ajuda
        } Desbloquear um artefacto alienígena.

entity-effect-guidebook-artifact-durability-restore =
    Restaurações {$restored} durabilidade em nós de artefato alienígenas ativos.

entity-effect-guidebook-plant-attribute =
    { $chance ->
        [1] Ajustes
        *[other] ajustar
    } {$attribute} por {$positive ->
    [true] [color=red]{$amount}[/color]
    *[false] [color=green]{$amount}[/color]
    }

entity-effect-guidebook-plant-cryoxadone =
    { $chance ->
        [1] Tempos para trás
        *[other] idade anterior
    } a planta, dependendo da idade e tempo da planta para crescer

entity-effect-guidebook-plant-phalanximine =
    { $chance ->
        [1] Restaurações
        *[other] restaurar
    } viabilidade para uma planta tornada inviável por uma mutação

entity-effect-guidebook-plant-diethylamine =
    { $chance ->
        [1] Aumentos
        *[other] aumento
    } tempo de vida da planta e/ou saúde de base com 10% de chance para cada

entity-effect-guidebook-plant-robust-harvest =
    { $chance ->
        [1] Aumentos
        *[other] aumento
    } a potência da planta por {$increase} até um máximo de {$limit}. Faz a planta perder suas sementes uma vez que a potência atinge {$seedlesstreshold}. Tentando adicionar potência sobre {$limit} pode causar diminuição na produtividade com uma chance de 10%

entity-effect-guidebook-plant-seeds-add =
    { $chance ->
        [1] Restaura o
        *[other] restaurar a
    } sementes da planta

entity-effect-guidebook-plant-seeds-remove =
    { $chance ->
        [1] Remove o
        *[other] remover a
    } sementes da planta

entity-effect-guidebook-plant-mutate-chemicals =
    { $chance ->
        [1] Mutatos
        *[other] mutado
    } uma planta para produzir {$name}
