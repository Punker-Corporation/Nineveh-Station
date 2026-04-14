<div class="header" align="center">

# NINEVEH STATION

![GitHub commit activity](https://img.shields.io/github/commit-activity/y/Punker-Corporation/Nineveh-Station)
![GitHub Issues](https://img.shields.io/github/issues/Punker-Corporation/Nineveh-Station)
![GitHub Pull Requests](https://img.shields.io/github/issues-pr-closed/Punker-Corporation/Nineveh-Station)

</div>

---

## FUNDAMENTACAO CONCEITUAL

Nineveh Station constitui uma investigacao aplicada sobre os mecanismos de inducao de medo e desamparo em ambientes virtuais interativos. Desenvolvido sob a egide da Punker Corps, o projeto opera como uma plataforma de experimentacao de fenomenos aversivos validados empiricamente, transpostos para a arquitetura do Space Station 14. O objetivo central nao e o entretenimento casual, mas a producao controlada de estados de hipervigilancia, ansiedade antecipatoria e colapso da sensacao de agencia — todos eles documentados como componentes nucleares da experiencia traumatica.

A metodologia empregada fundamenta-se na sintese de tres vetores de desestabilizacao psicologica:

1.  **Imprevisibilidade Estocastica de Ameacas.** A variabilidade nao deterministica no comportamento de entidades hostis impede a formacao de modelos mentais seguros por parte do jogador. Esta estrategia encontra respaldo nos trabalhos de Mineka & Kihlstrom (1978) sobre a etiologia do medo e da ansiedade, especificamente no que concerne a imprevisibilidade e incontrolabilidade de estimulos aversivos. Conforme demonstrado por Grillon et al. (2004) em estudos com paradigmas de sobressalto, a ausencia de pistas preditivas confiaveis mantem o sistema nervoso autonomo em estado sustentado de ativacao simpatica, elevando os niveis basais de cortisol e a reatividade da amigdala. Em Nineveh Station, algoritmos de decisao baseados em arvores de comportamento com injecao de ruido e pesos probabilisticos substituem rotinas deterministicas, assegurando que nenhum encontro seja identico ao anterior.

2.  **Escuridao Funcional como Amplificador de Vulnerabilidade.** A privacao sensorial visual nao e meramente estetica; e um modulador ativo da resposta de sobressalto acustico. Pesquisas classicas em psicofisiologia, como as conduzidas por Grillon & Davis (1997) sobre o efeito da iluminacao no reflexo de sobressalto, indicam que ambientes escuros potencializam significativamente a magnitude da resposta eletromiografica a estimulos auditivos inesperados. O motor de iluminacao de Nineveh Station implementa um modelo de visibilidade baseado em *raycasting* de alta precisao que restringe o campo visual efetivo a um cone foveal reduzido, eliminando a visao periferica e criando "pontos cegos" funcionais. Esta tecnica, inspirada em analises tecnicas de titulos como *Amnesia: The Dark Descent* (Grip & Nilsson, 2010), explora a sensibilidade inata do sistema visual humano a movimentos na periferia extrapessoal, forçando o jogador a uma constante rotacao de camera que gera ansiedade procedimental.

3.  **Erosao Progressiva do Controle Percebido.** A perda de agencia — a sensacao de que as acoes do jogador nao mais produzem resultados previsiveis sobre o ambiente ou a narrativa — e um dos mais robustos preditores de estresse psicologico. O modelo teorico de desamparo aprendido, formalizado por Seligman (1972) e expandido por Abramson, Seligman & Teasdale (1978), postula que a exposicao a eventos incontrolaveis gera deficits motivacionais, cognitivos e emocionais. No contexto ludico, Nineveh Station subverte convencoes de design ao introduzir falhas de interface (inputs atrasados ou ignorados), alteracoes arquitetonicas nao sinalizadas (remocao de landmarks espaciais) e sistemas de progressao que recompensam a passividade em detrimento da exploracao ativa. Esta abordagem e consistentemente apoiada por estudos em Human-Computer Interaction, como o trabalho de Birk & Mandryk (2013) sobre os efeitos psicologicos da frustracao de controle em jogos eletronicos.

### A Confluencia do Arquivo Cientifico e da Memoria Ludica

A construcao de Nineveh Station nao se limita a literatura academica contemporanea. O projeto reconhece e incorpora, de forma analitica, o legado empirico de titulos que, em sua epoca, atuaram como estressores validados para audiencias globais, gerando relatos consistentes de perturbacao psicologica que transcendiam o mero susto.

- **O Fenomeno "Polybius" e a Lenda da Modulacao de Estado.** Embora amplamente considerada uma lenda urbana, a narrativa em torno do arcade *Polybius* (supostamente 1981) documenta um caso de estudo na psicologia coletiva sobre os efeitos adversos de estímulos visuais subliminares e padrões de flicker. Independentemente da veracidade do gabinete original, os relatos de amnesia, insonia e terror noturno associados a ele informam o design audiovisual de Nineveh Station. Utilizamos tecnicas de modulacao de luminancia em baixa frequencia (flicker fusion threshold) e ruido visual procedural (baseado em perlin noise) para induzir fadiga ocular e uma sensacao subliminar de desconforto, replicando as alegadas consequencias neurologicas do mitico experimento da Sinnesloschen.
- **O Legado de *Silent Hill 2* e a Dissolucao do Self.** A obra da Team Silent (2001) permanece um marco na aplicacao de simbolismo psicanalitico e design de som perturbador. O uso de ruido industrial diegetico (baseado em gravacoes de fabricas abandonadas e interferencia eletromagnetica), conforme detalhado por Akira Yamaoka, serve como um ansiolitico negativo: em vez de acalmar, o som constante e dissonante (ruido marrom e infra-sons) eleva o nivel de arousal basal do jogador. Emulamos esta tecnica atraves de um sistema de audio procedural que sobrepoe camadas de gravacoes de campo (field recordings) de fontes nao identificaveis, processadas com reverberacao de convolucao para simular a acustica opressiva de espacos claustrofobicos.
- ***Manhunt* e a Economia da Brutalidade.** O titulo da Rockstar North (2003) foi objeto de intenso escrutinio mediatico e academico devido a sua representacao grafica da violencia como unica ferramenta de sobrevivencia. A controversia gerada por *Manhunt* reside na inversao da dinamica de poder: o jogador nao e o heroi, mas a presa que deve adotar metodos igualmente brutais para persistir. Em Nineveh Station, essa filosofia se traduz em um sistema de combate assimetrico onde a confrontacao direta e estatisticamente inviavel. A unica moeda de troca e a informacao sensorial limitada, forçando o jogador a um estado de "hipervigilancia paranoide" similar ao descrito por jogadores de *Cry of Fear* (Team Psykskallar, 2012) — outro estudo de caso sobre como limitacoes tecnicas (visao turva, gerenciamento de inventario punitivo) podem ser alavancas para o horror psicologico.

---

## ARQUITETURA TECNICA E FILOSOFIA DE DESENVOLVIMENTO

O codigo de Nineveh Station e tratado com o mesmo rigor metodologico aplicado a pesquisa conceitual. A base de codigo e mantida como um *fork* do Space Station 14 (Space Wizards Federation), com alteracoes profundas no *game loop*, *render pipeline* e *audio engine* para suportar os mecanismos de terror descritos.

**Principais Diretrizes de Engenharia:**

1.  **Modularidade e Isolamento de Estado.** As alteracoes sao encapsuladas para garantir a compatibilidade com *upstream* quando necessario, embora a divergencia funcional seja intencionalmente extrema. Utilizamos extensivamente o sistema de *Dependency Injection* e *Entity Component System (ECS)* do Robust Toolbox para injetar comportamentos aversivos sem acoplamento ao codigo vanilla.
2.  **Processamento de Audio Digital (DSP) em Tempo Real.** Implementamos uma camada de efeitos sobre o OpenAL que permite modulacao dinamica de filtros passa-baixa, eco e *chorus* baseada na proximidade de entidades SCP. Isso permite que a trilha sonora e os ambientes "reajam" fisiologicamente ao estado mental simulado do personagem.
3.  **Iluminacao Baseada em Visibilidade (Shadow Mapping Avancado).** O sistema de escuridao funcional nao e uma simples reducao de brilho (*gamma correction*). E uma mascara de visibilidade que calcula a oclusao de geometria em tempo real, assegurando que objetos e entidades sejam renderizados apenas quando efetivamente visiveis pelo jogador, mesmo em modos de visao noturna limitada. Esta tecnica, descrita em papers sobre *occlusion culling* para VR, e fundamental para a sensacao de presenca de uma ameaca as suas costas.

---

## LICENCIAMENTO E PROPRIEDADE INTELECTUAL

> [!CAUTION]
> O codigo contido neste repositorio esta sujeito a um regime de dupla licenca. O codigo originario da Space Wizards Federation permanece sob a licenca **MIT**. Todas as modificacoes, adicoes e ativos desenvolvidos pela Punker Corps para Nineveh Station, bem como o codigo derivado do ecossistema Sunrise (Fire Station), estao protegidos pelos termos do **Contrato de Licenca de Colaborador (CLA)** da Sunrise.
> Nao ha intencao de mesclar indiscriminadamente o codigo proprietario com o *upstream*. Para evitar ambiguidades legais ou incidentes de licenciamento, recomenda-se que desenvolvedores terceiros obtenham o codigo base diretamente do repositorio oficial da Space Wizards Federation.

### Detalhamento das Licencas Aplicaveis

<details>
<summary><a href="#"><img src="https://img.shields.io/badge/licen%C3%A7a-MIT-green?style=for-the-badge" alt="MIT license"></a></summary>

**Licenca MIT**
Aplica-se estritamente aos arquivos de codigo fonte originais da Space Wizards Federation contidos neste repositorio.
Texto integral disponivel em: [https://opensource.org/license/MIT](https://opensource.org/license/MIT)

</details>

<details>
<summary><a href="#"><img src="https://img.shields.io/badge/licen%C3%A7a-CC_3.0_BY--SA-lightblue?style=for-the-badge" alt="Creative Commons 3.0 BY-SA"></a></summary>

**Creative Commons 3.0 BY-SA**
Aplica-se a ativos nao-codigo (audio, texturas, modelos) que nao constituem propriedade intelectual da Punker Corps ou Sunrise, salvo disposicao explicita em contrário no diretorio do recurso.
Texto integral disponivel em: [https://creativecommons.org/licenses/by-sa/3.0/](https://creativecommons.org/licenses/by-sa/3.0/)

</details>

<details>
<summary><a href="#"><img src="https://img.shields.io/badge/licen%C3%A7a-CLA-orange?style=for-the-badge" alt="CLA"></a></summary>

**Contrato de Licenca de Colaborador (CLA)**
Todo o codigo proprietario e modificacoes especificas relacionadas ao universo SCP (Fire Station) e a identidade Nineveh Station sao regidos por este instrumento legal. A contribuicao para este repositorio implica a aceitacao tacita dos termos do CLA da Sunrise.
Texto integral disponivel em: [CLA.txt](https://github.com/space-sunrise/space-station-14/blob/master/CLA.txt)

</details>

---

<div class="header" align="center">

[![Discord](https://img.shields.io/discord/1051873590301184031?label=Discord&logo=discord&logoColor=white)](https://discord.gg/aczWxXgF)
[![GitHub](https://img.shields.io/github/stars/Punker-Corporation/Nineveh-Station?style=social)](https://github.com/Punker-Corporation/Nineveh-Station)
![CodeRabbit Pull Request Reviews](https://img.shields.io/coderabbit/prs/github/Punker-Corporation/Nineveh-Station?label=CodeRabbit%20Reviews)

</div>
