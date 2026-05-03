### UI

# Shown when a stack is examined in details range
comp-stack-examine-detail-count = {$contagem ->
    [one] Há [color={$markupCountColor}] {$count} [/color]
    *[other] Há [color={$markupCountColor}] {$count} [/color] coisas
} in the stack.

# Controle de status da pilha
comp-stack-status = Quantidade: [color=white]{ $count }[/color]

### Interaction Messages

# Shown when attempting to add to a stack that is full
comp-stack-already-full = A pilha já está cheia.
# Shown when a stack becomes full
comp-stack-becomes-full = A pilha agora está cheia.
# Text related to splitting a stack
comp-stack-split = Você divide a pilha.
comp-stack-split-halve = Divida ao meio
comp-stack-split-too-small = A pilha é muito pequena para ser dividida.
