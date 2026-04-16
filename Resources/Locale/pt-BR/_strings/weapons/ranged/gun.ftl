gun-selected-mode-examine = O modo de disparo é [color={ $color }]{ $mode }[/color].
gun-fire-rate-examine = Taxa de tiro [color={ $color }]{ $fireRate }[/color] por segundo.
gun-selector-verb = Alterar para { $mode }
gun-selected-mode = { $mode } selecionado
gun-disabled = Você não pode usar armas!
gun-clumsy = A arma explode na sua cara!
gun-set-fire-mode = Modo { $mode } selecionado
gun-alert-level-condition = O código da estação é muito baixo para este modo de disparo!
gun-magazine-whitelist-fail = Não cabe em uma arma!
gun-magazine-fired-empty = Não há mais cartuchos!
gun-Insulated-gloves = Seus dedos são grossos demais para puxar o gatilho!
# SelectiveFire
gun-SemiAuto = semi-automático
gun-Burst = fila
gun-FullAuto = auto
# BallisticAmmoProvider
gun-ballistic-cycle = Distorcer
gun-ballistic-cycled = Distorcido
gun-ballistic-cycled-empty = Alta
gun-ballistic-transfer-invalid = { CAPITALIZE($ammoEntity) } não pode ser colocado em { $targetEntity }!
gun-ballistic-transfer-empty = { CAPITALIZE($entity) } está vazio.
gun-ballistic-transfer-target-full = { CAPITALIZE($entity) } já está totalmente carregado.
# CartridgeAmmo
gun-cartridge-spent = Está [color=red]usado[/color].
gun-cartridge-unspent = É [color=lime]não usado[/color].
# BatteryAmmoProvider
gun-battery-examine = 
    Заряда хватит на [color={ $color }]{ $count }[/color] { $contagem -> 
    [one] выстрел
    [few] выстрела
   *[other] выстрелов
 }.
# CartridgeAmmoProvider
gun-chamber-bolt-ammo = A veneziana não está fechada
gun-chamber-bolt = Portão [color={ $color }]{ $bolt }[/color].
gun-chamber-bolt-closed = Obturador fechado
gun-chamber-bolt-opened = O obturador está aberto
gun-chamber-bolt-close = Feche o obturador
gun-chamber-bolt-open = Abra o obturador
gun-chamber-bolt-closed-state = abrir
gun-chamber-bolt-open-state = fechado
gun-chamber-rack = Puxe o obturador
# MagazineAmmoProvider
gun-magazine-examine = 
    Тут [color={ $color }]{ $count }[/color] { $contagem -> 
    [one] выстрел
    [few] выстрела
   *[other] выстрелов
 }.
# 🌟Starlight - Start🌟
gun-magazine-ammo-type = Contém [color={ $color }]{ $type }[/color].
gun-magazine-empty = A loja está vazia.
# 🌟Starlight - End🌟

# RevolverAmmoProvider
gun-revolver-empty = Descarregue o revólver
gun-revolver-full = O revólver está totalmente carregado
gun-revolver-insert = Cobrado
gun-revolver-spin = Gire o tambor
gun-revolver-spun = O tambor gira
gun-speedloader-empty = O Speedloader está vazio
