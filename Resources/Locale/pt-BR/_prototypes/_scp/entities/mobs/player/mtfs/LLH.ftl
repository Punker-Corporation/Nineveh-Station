ent-RandomHumanoidSpawnerLLHLeader = { ghost-role-information-mtf-leader-LLH-name }
    .suffix = FTM | Ômega-1
    .desc = { ghost-role-information-mtf-leader-description }
ent-RandomHumanoidSpawnerLLHSpecialist = { ghost-role-information-mtf-specialist-LLH-name }
    .suffix = FTM | Ômega-1
    .desc = { ghost-role-information-mtf-specialist-description }
ent-SpawnerMTFSquadLLH = { spawner-squad-name } FTM Ômega-1
    .desc = { ent-SpawnerMTFSquadHD.desc }
    .suffix = Ômega-1
ent-SpawnerMTFSquadLLHSpawnOnTrigger = { ent-SpawnerMTFSquadLLH }
    .desc = { ent-SpawnerMTFSquadLLH.desc }
    .suffix = { ent-SpawnerMTFSquadLLH.suffix }, { spawner-on-trigger-suffix }
ent-RandomHumanoidSpawnerLLHCadet = { ghost-role-information-mtf-cadet-LLH-name }
    .suffix = FTM | Ômega-1
    .desc = { ghost-role-information-mtf-cadet-description }

# Chaves adicionadas para impedir qualquer fallback de localiza??o.

ent-IDCardLLHLeader = { ent-IDCardMTFStandard }
    .desc = { ent-IDCardMTFStandard.desc }

ent-IDCardLLHSpecialist = { ent-IDCardMTFStandard }
    .desc = { ent-IDCardMTFStandard.desc }

ent-LLHLeaderPDA = { ent-BaseMTFPDA }
    .desc = { ent-BaseMTFPDA.desc }

ent-LLHSpecialistPDA = { ent-BaseMTFPDA }
    .desc = { ent-BaseMTFPDA.desc }
