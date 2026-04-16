### Voting system related console commands


## 'createvote' command

cmd-createvote-desc = Cria uma votação
cmd-createvote-help = Uso: createvote <'restart'|'preset'|'map'>
cmd-createvote-cannot-call-vote-now = Você não pode começar a votar agora!
cmd-createvote-invalid-vote-type = Tipo de votação inválido
cmd-createvote-arg-vote-type = <vote type>

## 'customvote' command

cmd-customvote-desc = Cria uma enquete personalizada
cmd-customvote-help = Uso: voto personalizado <title> <option1> <option2> [option3...]
cmd-customvote-on-finished-tie = Empate entre { $ties }!
cmd-customvote-on-finished-win = { $winner } vence!
cmd-customvote-arg-title = <title>
cmd-customvote-arg-option-n = <option{ $n }>

## 'vote' command

cmd-vote-desc = Votos em votação ativa
cmd-vote-help = Uso: vote <voteId> <option>
cmd-vote-cannot-call-vote-now = Você não pode começar a votar agora!
cmd-vote-on-execute-error-must-be-player = Deve ser um jogador
cmd-vote-on-execute-error-invalid-vote-id = ID de votação inválido
cmd-vote-on-execute-error-invalid-vote-options = Parâmetros de votação inválidos
cmd-vote-on-execute-error-invalid-vote = Voto errado
cmd-vote-on-execute-error-invalid-option = Parâmetro inválido

## 'listvotes' command

cmd-listvotes-desc = Lista votos ativos
cmd-listvotes-help = Uso: lista de votos

## 'cancelvote' command

cmd-cancelvote-desc = Cancela a votação atual
cmd-cancelvote-help = 
    Использование: cancelvote <id>
    Вы можете найти ID с помощью команды listvotes.
cmd-cancelvote-error-invalid-vote-id = ID de votação inválido
cmd-cancelvote-error-missing-vote-id = ID ausente
cmd-cancelvote-arg-id = <id>
