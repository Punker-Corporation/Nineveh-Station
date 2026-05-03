ore-silo-ui-title = Armazenamento de materiais
ore-silo-ui-label-clients = Carros
ore-silo-ui-label-mats = Materiais
ore-silo-ui-itemlist-entry = {$linked ->
    [true] {"[Linked] "}
    *[False] {""}
} {$name} ({$beacon}) {$inRange ->
    [true] {""}
    *[false] (Fora de alcance)
}
