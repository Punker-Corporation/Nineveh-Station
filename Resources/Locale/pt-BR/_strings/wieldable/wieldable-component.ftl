### Localidade para empunhar itens; ou seja, em duas mãos

wieldable-verb-text-wield = Wield
wieldable-verb-text-unwield = Descascados

wieldable-component-successful-wield = Você tem { THE($item) }.
wieldable-component-failed-wield = Você desencadeou o { THE($item) }.
wieldable-component-successful-wield-other = { CAPITALIZE(THE($user)) } empunha { THE($item) }.
wieldable-component-failed-wield-other = { CAPITALIZE(THE($user)) } desencadeia { THE($item) }.
wieldable-component-blocked-wield = { CAPITALIZE(THE($blocker)) } bloqueia-o de empunhar { THE($item) }.

wieldable-component-no-hands = Não tens mãos suficientes!
wieldable-component-not-enough-free-hands = {$number ->
    [one] Você precisa de uma mão livre para empunhar { THE($item) }.
    *[other] Você precisa de mãos livres { $number } para empunhar { THE($item) }.
}
wieldable-component-not-in-hands = O { CAPITALIZE(THE($item)) } não está nas tuas mãos!

wieldable-component-requires = { CAPITALIZE(THE($item))} deve ser empunhada!

gunwieldbonus-component-examine = Esta arma melhorou a precisão quando empunhada.

gunrequireswield-component-examine = Esta arma só pode ser disparada quando empunhada.
