ent-ScpAirlockExit = { ent-ScpAirlockRCDResistant }
    .suffix = { access-name-exit }
    .desc = { ent-ScpAirlockRCDResistant.desc }
ent-ScpAirlockExitGlass = { ent-ScpAirlockExit }
    .suffix = { access-name-exit }, vidro
    .desc = { ent-ScpAirlockExit.desc }

# Chaves adicionadas para impedir qualquer fallback de localiza??o.

ent-ScpAirlockExternal = { ent-ScpAirlockRCDResistant }
    .desc = { ent-ScpAirlockRCDResistant.desc }

ent-ScpAirlockExternalGlass = { ent-ScpAirlockExternal }
    .suffix = Vidro SCP, Externo
    .desc = { ent-ScpAirlockExternal.desc }
