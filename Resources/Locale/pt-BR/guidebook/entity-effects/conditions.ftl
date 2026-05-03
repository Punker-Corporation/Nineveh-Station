entity-condition-guidebook-total-damage =
    { $max ->
        [2147483648] tem pelo menos {NATURALFIXED($min, 2)} dano total
        *[other] { $min ->
                    [0] tem no máximo {NATURALFIXED($max, 2)} dano total
                    *[other] tem entre {NATURALFIXED($min, 2)} e {NATURALFIXED($max, 2)} dano total
                 }
    }

entity-condition-guidebook-type-damage =
    { $max ->
        [2147483648] tem pelo menos {NATURALFIXED($min, 2)} de dano {$type}
        *[other] { $min ->
                    [0] tem no máximo {NATURALFIXED($max, 2)} de dano {$type}
                    *[other] tem entre {NATURALFIXED($min, 2)} e {NATURALFIXED($max, 2)} de danos {$type}
                 }
    }

entity-condition-guidebook-group-damage =
    { $max ->
        [2147483648] tem pelo menos {NATURALFIXED($min, 2)} de dano {$type}.
        *[other] { $min ->
                    [0] tem no máximo {NATURALFIXED($max, 2)} de dano {$type}.
                    *[other] tem entre {NATURALFIXED($min, 2)} e {NATURALFIXED($max, 2)} de danos {$type}
                 }
    }

entity-condition-guidebook-total-hunger =
    { $max ->
        [2147483648] o alvo tem pelo menos {NATURALFIXED($min, 2)} fome total
        *[other] { $min ->
                    [0] o alvo tem no máximo {NATURALFIXED($max, 2)} fome total
                    *[other] o alvo tem entre {NATURALFIXED($min, 2)} e {NATURALFIXED($max, 2)} fome total
                 }
    }

entity-condition-guidebook-reagent-threshold =
    { $max ->
        [2147483648] há pelo menos {NATURALFIXED($min, 2)} u de {$reagent}
        *[other] { $min ->
                    [0] há no máximo {NATURALFIXED($max, 2)} u de {$reagent}
                    *[other] há entre {NATURALFIXED($min, 2)} u e {NATURALFIXED($max, 2)} u de {$reagent}
                 }
    }

entity-condition-guidebook-mob-state-condition =
    a multidão é { $state }

entity-condition-guidebook-job-condition =
    o trabalho do alvo é { $job }

entity-condition-guidebook-solution-temperature =
    the solution's temperature is { $max ->
            [2147483648] pelo menos {NATURALFIXED($min, 2)} k
            *[other] { $min ->
                        [0] no máximo {NATURALFIXED($max, 2)} k
                        *[other] entre {NATURALFIXED($min, 2)} k e {NATURALFIXED($max, 2)} k
                     }
    }

entity-condition-guidebook-body-temperature =
    the body's temperature is { $max ->
            [2147483648] pelo menos {NATURALFIXED($min, 2)} k
            *[other] { $min ->
                        [0] no máximo {NATURALFIXED($max, 2)} k
                        *[other] entre {NATURALFIXED($min, 2)} k e {NATURALFIXED($max, 2)} k
                     }
    }

entity-condition-guidebook-organ-type =
    the metabolizing organ { $shouldhave ->
                                [true] é
                                *[false] não está
                           } Órgão {INDEFINITE($name)} {$name}

entity-condition-guidebook-has-tag =
    the target { $invert ->
                 [true] não tem
                 *[false] tem
                } a etiqueta {$tag}

entity-condition-guidebook-this-reagent = este reagente

entity-condition-guidebook-breathing =
    the metabolizer is { $isBreathing ->
                [true] respiração normal
                *[false] sufocante
               }

entity-condition-guidebook-internals =
    the metabolizer is { $usingInternals ->
                [true] usando internos
                *[false] ar atmosférico respiratório
               }
