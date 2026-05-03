discord-watchlist-connection-header =
    { $players ->
        [one] O jogador {$players} numa lista de vigilância tem
        *[other] Os jogadores {$players} numa lista de vigilância têm
    } ligado ao {$serverName}

discord-watchlist-connection-entry = - {$playerName} com a mensagem " {$message} "{ $expiry ->
        [0] {""}
        *[other] {" "} (expira < t: {$expiry}: R>)
    }{ $otherWatchlists ->
        [0] {""}
        [one] {" "} e {$otherWatchlists} outra lista de vigilância
        *[other] {" "} e {$otherWatchlists} outras listas de vigilância
    }
