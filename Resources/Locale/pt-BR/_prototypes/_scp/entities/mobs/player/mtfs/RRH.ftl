ent-RandomHumanoidSpawnerRRHLeader = { ghost-role-information-mtf-leader-RRH-name }
    .suffix = FTM | Alfa-1
    .desc = { ghost-role-information-mtf-leader-description }
ent-RandomHumanoidSpawnerRRHSpecialist = { ghost-role-information-mtf-specialist-RRH-name }
    .suffix = FTM | Alfa-1
    .desc = { ghost-role-information-mtf-specialist-description }
ent-SpawnerMTFSquadRRH = { spawner-squad-name } FTM Alfa-1
    .desc = { ent-SpawnerMTFSquadHD.desc }
    .suffix = Alfa-1
ent-SpawnerMTFSquadRRHSpawnOnTrigger = { ent-SpawnerMTFSquadRRH }
    .desc = { ent-SpawnerMTFSquadRRH.desc }
    .suffix = { ent-SpawnerMTFSquadRRH.suffix }, { spawner-on-trigger-suffix }
ent-RandomHumanoidSpawnerRRHCadet = { ghost-role-information-mtf-cadet-RRH-name }
    .suffix = FTM | Alfa-1
    .desc = { ghost-role-information-mtf-cadet-description }

# Chaves adicionadas para impedir qualquer fallback de localiza??o.

ent-IDCardRRHLeader = { ent-IDCardMTFStandard }
    .desc = { ent-IDCardMTFStandard.desc }

ent-IDCardRRHSpecialist = { ent-IDCardMTFStandard }
    .desc = { ent-IDCardMTFStandard.desc }

ent-IDCardRRHCadet = { ent-IDCardMTFStandard }
    .desc = { ent-IDCardMTFStandard.desc }

ent-RRHLeaderPDA = { ent-BaseMTFPDA }
    .desc = { ent-BaseMTFPDA.desc }

ent-RRHSpecialistPDA = { ent-BaseMTFPDA }
    .desc = { ent-BaseMTFPDA.desc }

ent-RRHCadetPDA = { ent-BaseMTFPDA }
    .desc = { ent-BaseMTFPDA.desc }
