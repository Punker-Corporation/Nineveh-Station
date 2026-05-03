## Survivor

roles-antag-survivor-name = Sobrevivente
# It's a Halo reference
roles-antag-survivor-objective = Objetivo Atual: Sobreviver
survivor-role-greeting =
    És um sobrevivente. Acima de tudo, tens de voltar vivo ao Comando Central.
    Recolha o poder de fogo necessário para garantir a sua sobrevivência.
    Não confies em ninguém.

survivor-round-end-dead-count =
    { $deadCount ->
    [one] [color=vermelho]{ $deadCount }[/color] sobrevivente morreu.
    [few] [color=vermelho]{ $deadCount }[/color] sobreviventes morreram.
   *[other] [color=vermelho]{ $deadCount }[/color] sobreviventes morreram.
 }
survivor-round-end-alive-count =
    { $aliveCount ->
    [one] [color=amarelo]{ $aliveCount }[/color] o sobrevivente permaneceu na delegacia.
    [few] [color=amarelo]{ $aliveCount }[/color] sobreviventes permaneceram na estação.
   *[other] [color=amarelo]{ $aliveCount }[/color] sobreviventes permaneceram na estação.
 }
survivor-round-end-alive-on-shuttle-count =
    { $aliveCount ->
    [one] [color=verde]{ $aliveCount }[/color] sobrevivente escapou.
    [few] [color=verde]{ $aliveCount }[/color] sobreviventes escaparam.
   *[other] [color=verde]{ $aliveCount }[/color] sobreviventes foram salvos.
 }

## Wizard

objective-issuer-swf = [color=turquoise]Federação de Magos Espaciais[/color]
wizard-title = Mago
wizard-description = Na estação Mag! Não se sabe o que ele pode fazer.
roles-antag-wizard-name = Mago
roles-antag-wizard-objective = Ensine-lhes uma lição que nunca esquecerão.
wizard-role-greeting =
    Está na hora, bola de fogo!
    Houve tensões entre a Federação dos Feiticeiros do Espaço e NanoTrasen. Você foi selecionado pela Federação de Feiticeiros Espaciais para fazer uma visita à estação e "lembrar-lhes" porque spellcasters não devem ser enganados.
    Causa caos e destruição! O que fazes é contigo, mas lembra-te que os Feiticeiros do Espaço querem que saias vivo.

wizard-round-end-name = mágico

## TODO: Wizard Apprentice (Coming sometime post-wizard release)
