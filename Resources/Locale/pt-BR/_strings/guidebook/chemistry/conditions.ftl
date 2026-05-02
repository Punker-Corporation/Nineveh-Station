reagent-effect-condition-guidebook-total-damage =
    { $max ->
    [2147483648] o corpo tem pelo menos { NATURALFIXED($min, 2) } danos
   *[other]
            { $min ->
    [0] não mais { NATURALFIXED($max, 2) } danos
   *[other] entre { NATURALFIXED($min, 2) } e { NATURALFIXED($max, 2) } danos
 }
 }
reagent-effect-condition-guidebook-total-hunger =
    { $max ->
    [2147483648] ter pelo menos { NATURALFIXED($min, 2) } fome
   *[other]
            { $min ->
    [0] objetivo não é mais { NATURALFIXED($max, 2) } fome
   *[other] objetivo  { NATURALFIXED($min, 2) } e { NATURALFIXED($max, 2) } fome
 }
 }
reagent-effect-condition-guidebook-reagent-threshold =
    { $max ->
    [2147483648] O sistema circulatório tem pelo menos { NATURALFIXED($min, 2) }Ed. { $reagent }
   *[other]
            { $min ->
    [0] não mais { NATURALFIXED($max, 2) }Ed. { $reagent }
   *[other] entre { NATURALFIXED($min, 2) }unidade { NATURALFIXED($max, 2) }Ed. { $reagent }
 }
 }
reagent-effect-condition-guidebook-mob-state-condition = paciente em { $state }
reagent-effect-condition-guidebook-job-condition = posição alvo - { $job }
reagent-effect-condition-guidebook-solution-temperature =
    temperatura da solução { $max ->
    [2147483648] menos { NATURALFIXED($min, 2) }k
   *[other]
            { $min ->
    [0] mais { NATURALFIXED($max, 2) }k
   *[other] entre { NATURALFIXED($min, 2) }k { NATURALFIXED($max, 2) }k
 }
 }
reagent-effect-condition-guidebook-body-temperature =
    temperatura { $max ->
    [2147483648] menos { NATURALFIXED($min, 2) }k
   *[other]
            { $min ->
    [0] mais { NATURALFIXED($max, 2) }k
   *[other] entre { NATURALFIXED($min, 2) }k { NATURALFIXED($max, 2) }k
 }
 }
reagent-effect-condition-guidebook-organ-type =
    metabolizador { $shouldhave ->
    [true] ele
   *[false] Não é.
 } { $name } órgão
reagent-effect-condition-guidebook-has-tag =
    objetivo { $invert ->
    [true] nenhum
   *[false] tem
 } marcador { $tag }
reagent-effect-condition-guidebook-this-reagent = este reagente
