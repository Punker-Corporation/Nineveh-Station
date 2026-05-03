### Localization for role ban command

cmd-roleban-desc = Impede que o usuário desempenhe a função
cmd-roleban-help = Uso: roleban <name or user ID> <job> <razão> [duration in minutes, leave out or 0 for permanent ban]

## Sugestão do resultado de conclusão
cmd-roleban-hint-1 = <nome ou ID do utilizador>
cmd-roleban-hint-2 = <job>
cmd-roleban-hint-3 = <razão>
cmd-roleban-hint-4 = [duration in minutes, leave out or 0 for permanent ban]
cmd-roleban-hint-5 = [severity]

cmd-roleban-hint-duration-1 = Para sempre
cmd-roleban-hint-duration-2 = 1 dia
cmd-roleban-hint-duration-3 = 3 dias
cmd-roleban-hint-duration-4 = 1 semana
cmd-roleban-hint-duration-5 = 2 semanas
cmd-roleban-hint-duration-6 = 1 mês

### Localization for role unban command

cmd-roleunban-desc = Retorna a capacidade de roleplay para o usuário
cmd-roleunban-help = Uso: roleunban <role ban id>

## Completion result hints

cmd-roleunban-hint-1 = <role ban id>


### Localização do comando roleban list

cmd-rolebanlist-desc = Lista de proibições de papéis de jogadores
cmd-rolebanlist-help = Uso: <name or user ID> [include unbanned]

## Completion result hints

cmd-rolebanlist-hint-1 = <nome ou ID do utilizador>
cmd-rolebanlist-hint-2 = [include unbanned]


cmd-roleban-minutes-parse = { $time } - número de minutos inválido.\n{ $help }
cmd-roleban-severity-parse = ${ severity } não é um nível de gravidade válido\n{ $help }.
cmd-roleban-arg-count = Número inválido de argumentos.
cmd-roleban-job-parse = A tarefa { $job } não existe.
cmd-roleban-name-parse = Um jogador com este nome não pode ser encontrado.
cmd-roleban-existing = { $target } já tem banimento da função de { $role }.
cmd-roleban-success = { $target } está proibido de desempenhar o papel de { $role } devido a { $reason } { $length }.
cmd-roleban-inf = para sempre
cmd-roleban-until = até { $expires }
# Department bans
cmd-departmentban-desc = Impede que o usuário desempenhe funções que fazem parte do departamento
cmd-departmentban-help = Uso: departmentban <name or user ID> <department> <razão> [duration in minutes, leave out or 0 for permanent ban]

# Chaves adicionadas para impedir qualquer fallback de localiza??o.

cmd-roleunban-unable-to-parse-id = Não foi possível processar o {$id} como um número inteiro do ID de ban.
                                   {$help}

## Sugestão do resultado de conclusão
