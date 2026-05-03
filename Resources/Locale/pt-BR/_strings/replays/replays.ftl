# Loading Screen

replay-loading = Carregando ({ $cur }/{ $total })
replay-loading-reading = Lendo arquivos
replay-loading-processing = Processamento de arquivos
replay-loading-spawning = Instanciando entidades
replay-loading-initializing = Inicializando entidades
replay-loading-starting = Entidades em execução
replay-loading-failed = Falha ao carregar a repetição. Erro:
                        {$reason}
replay-loading-retry = Tente carregar com uma tolerância de exceção maior - PODE CAUSAR BUGS!
replay-loading-cancel = Rejeitar
# Main Menu
replay-menu-subtext = Repetições
replay-menu-load = Carregar o replay selecionado
replay-menu-select = Selecione repetir
replay-menu-open = Abra a pasta de repetição
replay-menu-none = Nenhuma duplicata encontrada.
# Main Menu Info Box
replay-info-title = Informações de repetição
replay-info-none-selected = Repetir não selecionado
replay-info-invalid = [color=red]Repetição inválida selecionada[/color]
replay-info-info =
    { "[" }color=cinza]Selecionado:[/color] { $name } ({ $file })
    { "[" }color=cinza]Hora:[/color] { $time }
    { "[" }color=cinza]ID da rodada:[/color] { $roundId }
    { "[" }color=cinza]Duração:[/color] { $duration }
    { "[" }color=cinza]ForkId:[/color] { $forkId }
    { "[" }color=cinza]Versão:[/color] { $version }
    { "[" }color=cinza]Motor:[/color] { $engVersion }
    { "[" }color=cinza] Digite Hash:[/color] { $hash }
    { "[" }color=cinza]Comp Hash:[/color] { $compHash }
# Replay selection window
replay-menu-select-title = Selecione repetir
# Replay related verbs
replay-verb-spectate = Observar
# command
cmd-replay-spectate-help = replay spectate [optional entity]
cmd-replay-spectate-desc = Anexa ou cancela a atribuição de um player local ao uid da entidade especificada.
cmd-replay-spectate-hint = EntityUid opcional
cmd-replay-toggleui-desc = Mude a interface do usuário de controle de reprodução.
