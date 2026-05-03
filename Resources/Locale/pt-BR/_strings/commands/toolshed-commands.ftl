command-description-visualize = Exibe uma lista de entidades na interface do usuário para fácil visualização.
command-description-runverbas = Executa um verbo nas entidades especificadas com o usuário especificado.
command-description-acmd-perms = Retorna direitos administrativos para o comando especificado, se houver.
command-description-acmd-caninvoke = Verifica se o player especificado pode chamar o comando especificado.
command-description-jobs-jobs = Retorna todas as posições na estação.
command-description-jobs-job = Retorna a posição especificada na estação.
command-description-jobs-isinfinite = Retorna verdadeiro se a posição especificada for infinita; caso contrário, retorna falso.
command-description-jobs-adjust = Ajusta o número de slots para a posição especificada.
command-description-jobs-set = Define o número de slots para a posição especificada.
command-description-jobs-amount = Retorna o número de slots para a posição especificada.
command-description-laws-list = Retorna uma lista de todas as entidades associadas às leis.
command-description-laws-get = Retorna todas as leis da entidade especificada.
command-description-stations-list = Retorna uma lista de todas as estações.
command-description-stations-get = Obtém a estação ativa se houver apenas uma.
command-description-stations-getowningstation = Obtém a estação à qual a entidade especificada pertence (dentro dos limites).
command-description-stations-grids = Retorna todas as malhas associadas à estação especificada.
command-description-stations-config = Retorna a configuração associada à estação especificada, se houver.
command-description-stations-addgrid = Adiciona uma grade à estação especificada.
command-description-stations-rmgrid = Remove a grade da estação especificada.
command-description-stations-rename = Renomeia a estação especificada.
command-description-stations-largestgrid = Retorna a maior malha disponível na estação especificada, se houver.
command-description-stations-rerollBounties = Limpa todos os prêmios atuais da estação e recebe uma nova seleção.
command-description-stationevent-lsprob = Lista a probabilidade de vários eventos de estação em todo o conjunto.
command-description-stationevent-lsprobtime = Lista a probabilidade de vários eventos de estação dependendo da duração da rodada especificada.
command-description-stationevent-prob = Retorna a probabilidade de um evento de estação de todo o conjunto.
command-description-admins-active = Retorna uma lista de administradores ativos.
command-description-admins-all = Retorna uma lista de TODOS os administradores, incluindo os desadministrados.
command-description-marked = Retorna $marcado como uma lista de entidades.
command-description-rejuvenate = Revive entidades alvo, restaurando sua saúde, eliminando efeitos de status, etc.
command-description-tag-list = Lista tags nas entidades especificadas.
command-description-tag-with = Retorna apenas entidades com a tag fornecida da lista de entidades passada.
command-description-tag-add = Adiciona uma tag às entidades especificadas.
command-description-tag-rm = Remove uma tag das entidades especificadas.
command-description-tag-addmany = Adiciona uma lista de tags às entidades especificadas.
command-description-tag-rmmany = Remove a lista de tags das entidades especificadas.
command-description-polymorph = Polimorfa a entidade especificada com o protótipo fornecido.
command-description-unpolymorph = Retorna uma entidade polimorfa ao seu estado original.
command-description-solution-get = Retorna a solução armazenada no contêiner de soluções da entidade.
command-description-solution-adjreagent = Corrige o reagente especificado na solução especificada.
command-description-mind-get = Extrai a mente da entidade, se houver.
command-description-mind-control = Assume o controle de uma entidade com um determinado jogador.
command-description-addaccesslog = Adiciona um registro de acesso a esta entidade. Observe que isso ignora o limite padrão e pausa a verificação do log.
command-description-stationevent-simulate = Simula o número N de rodadas em que os eventos ocorrerão e gera a frequência de cada evento depois.

# Chaves adicionadas para impedir qualquer fallback de localiza??o.

command-description-bank-accounts =
    Devolve todas as contas numa estação.

command-description-bank-account =
    Devolve uma dada conta bancária de uma estação.

command-description-bank-adjust =
    Ajusta o dinheiro para a conta bancária dada.

command-description-bank-set =
    Define o dinheiro da conta bancária dada.

command-description-bank-amount =
    Devolve o dinheiro para a conta bancária dada.

command-description-clone-humanoidappearance =
    Clona o aspecto humanóide da entidade fornecida a todas as entidades de entrada.

command-description-clone-comps =
    Clona todos os componentes da entidade fornecida para todas as entidades de entrada. Só funciona para componentes suportados.

command-description-clone-equipment =
    Clona o equipamento da entidade fornecida a todas as entidades de entrada. Utiliza protótipos de base, o que significa que as mudanças no equipamento não persistirão nas versões clonadas.

command-description-clone-implants =
    Clona os implantes da entidade fornecida para todas as entidades de entrada. Utiliza protótipos de base, o que significa que as alterações nos implantes não persistirão nas versões clonadas.

command-description-clone-storage =
    Clona o armazenamento da entidade fornecida para todas as entidades de entrada. Utiliza protótipos de base, o que significa que as alterações no conteúdo não persistirão nas versões clonadas.

command-description-stationevent-lsprobtheoretical =
    Dado um protótipo BasicStationEventScheduler, contagem de jogadores, e tempo de rodada, lista a probabilidade de diferentes eventos de estação que ocorrem com base no número especificado de jogadores e tempo de rodada.

command-description-xenoartifact-list =
    Listar todos os EntityUids de artefatos gerados.

command-description-xenoartifact-printMatrix =
    Imprime matriz que exibe todas as bordas entre nós.

command-description-xenoartifact-totalResearch =
    Obtém todos os pontos de pesquisa que podem ser extraídos do artefato atualmente.

command-description-xenoartifact-averageResearch =
    Calcula a quantidade de pontos de pesquisa que o artefato xeno gerado médio irá produzir quando estiver totalmente ativado.

command-description-xenoartifact-unlockAllNodes =
    Desbloqueia todos os nós do artefacto.

command-description-jobboard-completeJob =
    Completa um trabalho de salvamento para a estação.

command-description-scale-set =
    Define o tamanho da imagem de uma entidade para uma determinada escala (sem alterar a sua configuração).

command-description-scale-get =
    Obtenha a escala de imagens de uma entidade conforme definida pelo ScaleVisualsComponent. Não inclui quaisquer alterações feitas diretamente no SpriteComponent.

command-description-scale-multiply =
    Multiplique o tamanho da imagem de uma entidade com um determinado fator (sem alterar seu dispositivo).

command-description-scale-multiplyvector =
    Multiplique o tamanho da imagem de uma entidade com um determinado vetor 2d (sem alterar sua configuração).

command-description-scale-multiplywithfixture =
    Multiplique o tamanho da imagem de uma entidade com um determinado fator (incluindo seu dispositivo).

command-description-storage-fasttake =
    Toma o item mais recentemente colocado da entidade de armazenamento piped.

command-description-storage-insert =
    Insere a entidade canalizada na entidade de armazenamento indicada.

command-description-inventory-getflags =
    Obtém todas as entidades em slots na entidade de inventário piped correspondente a uma determinada bandeira de slot.

command-description-inventory-getnamed =
    Obtém todas as entidades em slots na entidade de inventário piped correspondente a um determinado nome de slot.

command-description-inventory-forceput =
    Coloca uma determinada entidade na primeira entidade que tem um slot correspondente à bandeira indicada, excluindo qualquer item anteriormente nesse slot.

command-description-inventory-forcespawn =
    Espalha um dado protótipo na primeira entidade que tem um slot correspondente à bandeira indicada, excluindo qualquer item anteriormente nesse slot.

command-description-inventory-put =
    Coloca uma determinada entidade na primeira entidade que tem um slot correspondente à bandeira indicada, sem igualar qualquer item anteriormente nesse slot.

command-description-inventory-spawn =
    Espalha um dado protótipo na primeira entidade que tem uma fenda correspondente à bandeira indicada, sem igualar qualquer item anteriormente nesse slot.

command-description-inventory-tryput =
    Tenta colocar uma determinada entidade na primeira entidade que tem um slot correspondente à bandeira indicada, falhando se algum item estiver presente nesse slot.

command-description-inventory-tryspawn =
    Tenta gerar um determinado protótipo na primeira entidade que tem um slot correspondente à bandeira indicada, falhando se algum item estiver presente nesse slot.

command-description-inventory-ensure =
    Coloca uma determinada entidade na primeira entidade que tem um slot correspondente à bandeira indicada se nenhuma existir, passando pelo UID do que quer que esteja no slot até o final.

command-description-inventory-ensurespawn =
    Espalha um determinado protótipo na primeira entidade que tem um slot correspondente à bandeira indicada se não existir nenhum, passando pelo UID do que quer que esteja no slot até o final.

command-description-dynamicrule-list =
    Lista todas as regras dinâmicas atualmente ativas, geralmente esta é apenas uma.

command-description-dynamicrule-get =
    Obtém a regra dinâmica activa de momento.

command-description-dynamicrule-budget =
    Obtém o orçamento atual da(s) regra(s) dinâmica(s).

command-description-dynamicrule-adjust =
    Ajusta o orçamento da(s) regra(s) dinâmica(s) canalizada(s) pela quantidade especificada.

command-description-dynamicrule-set =
    Define o orçamento da(s) regra(s) dinâmica(s) canalizada(s) para o montante especificado.

command-description-dynamicrule-dryrun =
    Retorna uma lista de regras que poderiam ser ativadas se a regra fosse executada neste momento com todo o contexto atual. Esta não é uma lista completa de todas as regras que poderiam ser executadas, apenas uma amostra das atuais válidas.

command-description-dynamicrule-executenow =
    Executa a regra dinâmica piped como se tivesse atingido seu tempo de atualização regular.

command-description-dynamicrule-rules =
    Obtém uma lista de todas as regras geradas pela regra dinâmica piped.
