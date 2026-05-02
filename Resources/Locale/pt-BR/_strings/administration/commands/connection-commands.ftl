## Linhas para o comando "passeio de ligação Grant".

cmd-grant_connect_bypass-desc = Permitir temporariamente que o usuário ignore as verificações normais de conexão.
cmd-grant_connect_bypass-help = Uso: Grant connect bypass <user> [duration minutes]
    Concede temporariamente a um usuário a capacidade de contornar restrições de conexões regulares.
    O bypass só se aplica a este servidor de jogo e expirará após (por padrão) 1 hora.
    Eles serão capazes de se juntar independentemente da lista branca, abrigo de pânico, ou boné do jogador.

cmd-grant_connect_bypass-arg-user = <user>
cmd-grant_connect_bypass-arg-duration = [duration minutes]

cmd-grant_connect_bypass-invalid-args = Esperado 1 ou 2 argumentos
cmd-grant_connect_bypass-unknown-user = Não foi possível encontrar o usuário '{ $user }'
cmd-grant_connect_bypass-invalid-duration = Duração inválida '{ $duration }'
cmd-grant_connect_bypass-success = Permissão de desvio adicionada com sucesso para o usuário '{ $user }'
