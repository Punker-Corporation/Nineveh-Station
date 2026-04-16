defusable-examine-defused = { CAPITALIZE($name) } [color=lime]neutralizado[/color].
defusable-examine-live = 
    { CAPITALIZE($name) } тикает [color=vermelho][/color] e [color=red]{ $time } { $tempo -> 
    [one] секунда
    [few] секунды
   *[other] секунд
 }.
defusable-examine-live-display-off = { CAPITALIZE($name) } [color=red] está marcando [/color] e o cronômetro parece estar desligado.
defusable-examine-inactive = { CAPITALIZE($name) } [color=lime] está inativo [/color], mas ainda pode explodir.
defusable-examine-bolts = 
    Болты { $down -> 
    [true] [color=vermelho]omitido[/color]
   *[false] [color=verde] elevado[/color]
 }.
