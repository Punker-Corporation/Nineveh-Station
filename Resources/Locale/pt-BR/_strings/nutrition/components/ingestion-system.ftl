### Mensagens de Interação

# Sistema

## Ao tentar ingerir sem o utensílio necessário... mas tens de aguentar.
ingestion-you-need-to-hold-utensil = Você precisa estar segurando {INDEFINITE($utensil)} {$utensil} para comer isso!

ingestion-try-use-is-empty = O {CAPITALIZE(THE($entity))} está vazio!
ingestion-try-use-wrong-utensil = Você não pode {$verb} {THE($food)} com {INDEFINITE($utensil)} {$utensil}.

ingestion-remove-mask = Você precisa tirar o {$entity} primeiro.

## Ingestão Falhou

ingestion-you-cannot-ingest-any-more = Não podes mais {$verb}!
ingestion-other-cannot-ingest-any-more = {CAPITALIZE(SUBJECT($target))} não pode {$verb} mais!

ingestion-cant-digest = Não consegues digerir {THE($entity)}!
ingestion-cant-digest-other = {CAPITALIZE(SUBJECT($target))} não consegue digerir {THE($entity)}!

## Verbos de ação, não confundir com Verbos

ingestion-verb-food = Comer
ingestion-verb-drink = Bebe.

# Componente comestível

edible-nom = Nom. {$flavors}
edible-nom-other = Nom.
edible-slurp = Slurp. {$flavors}
edible-slurp-other = Slurp.
edible-swallow = Engula { THE($food) }
edible-gulp = Gulp. {$flavors}
edible-gulp-other = Gulp.

edible-has-used-storage = Você não pode {$verb} { THE($food) } com um item armazenado dentro.

## Substantivos

edible-noun-edible = Comestível
edible-noun-food = alimentos
edible-noun-drink = bebida
edible-noun-pill = comprimido

## Verbos

edible-verb-edible = ingerir
edible-verb-food = comer
edible-verb-drink = bebida
edible-verb-pill = engolir

## Forçar a alimentação

edible-force-feed = {CAPITALIZE(THE($user))} está tentando fazer você {$verb} algo!
edible-force-feed-success = {CAPITALIZE(THE($user))} forçou-te a {$verb} alguma coisa! {$flavors}
edible-force-feed-success-user = Você alimenta com sucesso o {THE($target)}
