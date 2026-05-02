## UI

cargo-console-menu-title = Console de pedidos de carga
cargo-console-menu-account-name-label = Nome da conta:{ " " }
cargo-console-menu-account-name-none-text = Não
cargo-console-menu-shuttle-name-label = Nome do ônibus:{ " " }
cargo-console-menu-shuttle-name-none-text = Não
cargo-console-menu-points-label = Créditos:{ " " }
cargo-console-menu-points-amount = ${ $amount }
cargo-console-menu-shuttle-status-label = Status do transporte:{ " " }
cargo-console-menu-shuttle-status-away-text = Partiu
cargo-console-menu-order-capacity-label = Volume do pedido:{ " " }
cargo-console-menu-call-shuttle-button = Ativar telepad
cargo-console-menu-permissions-button = Acessos
cargo-console-menu-categories-label = Categorias:{ " " }
cargo-console-menu-search-bar-placeholder = Procurar
cargo-console-menu-requests-label = Solicitações
cargo-console-menu-orders-label = Pedidos
cargo-console-menu-order-reason-description = Motivo: { $reason }
cargo-console-menu-populate-categories-all-text = Todos
cargo-console-menu-populate-orders-cargo-order-row-product-name-text = { $productName } (x{ $orderAmount }) de { $orderRequester }
cargo-console-menu-cargo-order-row-approve-button = Aprovar
cargo-console-menu-cargo-order-row-cancel-button = Cancelar
# Orders
cargo-console-order-not-allowed = Acesso negado
cargo-console-station-not-found = Nenhum complexo disponível
cargo-console-invalid-product = ID do produto inválido
cargo-console-too-many = Muitos pedidos aprovados
cargo-console-snip-snip = Pedido reduzido à capacidade
cargo-console-insufficient-funds = Fundos insuficientes ({ $cost } necessário)
cargo-console-unfulfilled = Não há espaço para atender o pedido
cargo-console-trade-station = Enviar para { $destination }
cargo-console-unlock-approved-order-broadcast = [bold]O pedido de { $productName } x{ $orderAmount }[/bold], custando [bold]{ $cost }[/bold], foi aprovado por [bold]{ $approver }[/bold]
cargo-console-paper-print-name = Ordem #{ $orderNumber }
cargo-console-paper-print-text = [head=2] Ordem # {$orderNumber} [/head]
    {"[bold]Item:[/bold]"} {$itemName} (x {$orderQuantity} )
    ZXQ0QZ {$requester}

    {"[head=3]Order Information[/head]"}
    {"[bold]Payer[/bold]:"} {$account} [font="Monospace"] \[ {$accountcode} \] [/font]
    ZXQ0QZ {$approver}
    ZXQ0QZ {$reason}

# Consola de transporte de carga
cargo-shuttle-console-menu-title = Console de chamada de transporte de carga
cargo-shuttle-console-station-unknown = Desconhecido
cargo-shuttle-console-shuttle-not-found = Não encontrado
cargo-no-shuttle = Transporte de carga não encontrado!
cargo-shuttle-console-organics = Formas de vida orgânicas descobertas em ônibus espacial

# Chaves adicionadas para impedir qualquer fallback de localiza??o.

cargo-console-menu-account-name-format = [bold] [color={$color}] {$name} [/color] [/bold] [font="Monospace"] \[ {$code} \] [/font]

cargo-console-menu-tab-title-orders = Ordens

cargo-console-menu-tab-title-funds = Transferências

cargo-console-menu-account-action-transfer-limit = Limite de Transferência [bold]: [/bold] $ {$limit}

cargo-console-menu-account-action-transfer-limit-unlimited-notifier = [color=gold] (Ilimitado) [/color]

cargo-console-menu-account-action-select = Ação da conta [bold]: [/bold]

cargo-console-menu-account-action-amount = [bold] Quantidade: [/bold] $

cargo-console-menu-account-action-button = Transferência

cargo-console-menu-toggle-account-lock-button = Alternar o Limite de Transferência

cargo-console-menu-account-action-option-withdraw = Retirar o Dinheiro

cargo-console-menu-account-action-option-transfer = Fundos de transferência para {$code}

# Ordens

cargo-console-fund-withdraw-broadcast = [bold] {$name} retirou {$amount} spesos do {$name1} \[ {$code1} \]

cargo-console-fund-transfer-broadcast = [bold] {$name} transferido {$amount} spesos de {$name1} \[ {$code1} \] para {$name2} \[ {$code2} \] [/bold]

cargo-console-fund-transfer-user-unknown = Desconhecido


cargo-console-paper-reason-default = Nenhum

cargo-console-paper-approver-default = Auto

cargo-funding-alloc-console-menu-title = Console de Alocação de Financiamento

cargo-funding-alloc-console-label-account = [bold] Conta [/bold]

cargo-funding-alloc-console-label-code = Código [bold] [/bold]

cargo-funding-alloc-console-label-balance = [bold] Saldo [/bold]

cargo-funding-alloc-console-label-cut = Divisão de Receitas [bold] (%)


cargo-funding-alloc-console-label-primary-cut = Corte de fundos de carga de fontes não-lockbox (%):

cargo-funding-alloc-console-label-lockbox-cut = Repartição dos fundos das vendas de cadeados (%):


cargo-funding-alloc-console-label-help-non-adjustible = A carga recebe {$percent} % dos lucros de vendas não-lockbox. O resto é dividido como especificado abaixo:

cargo-funding-alloc-console-label-help-adjustible = Os restantes fundos de fontes não-locadoras são distribuídos como especificado abaixo:

cargo-funding-alloc-console-button-save = Salvar alterações

cargo-funding-alloc-console-label-save-fail = Divisão de Receitas [bold] Inválido! [/bold] [color=red] ({$pos ->
    [1] +
    *[-1] -
}{$val}%)[/color]

# Modelo de deslizamento

cargo-acquisition-slip-body = Detalhe do Activo [head=3] [/head]
    ZXQ0QZ {$product}
    ZXQ0QZ {$description}
    {"[bold]Unit cost:[/bold"} ] $ {$unit}
    ZXQ0QZ {$amount}
    {"[bold]Cost:[/bold]"} $ {$cost}

    {"[head=3]Purchase Detail[/head]"}
    ZXQ0QZ {$orderer}
    ZXQ0QZ {$reason}
