ent-RandomHumanoidSpawnerMHLeader = { ghost-role-information-mtf-leader-MH-name }
    .suffix = FTM | Beta-7
    .desc = { ghost-role-information-mtf-leader-description }
ent-RandomHumanoidSpawnerMHSpecialist = { ghost-role-information-mtf-specialist-MH-name }
    .suffix = FTM | Beta-7
    .desc = { ghost-role-information-mtf-specialist-description }
ent-RandomHumanoidSpawnerMHCadet = { ghost-role-information-mtf-cadet-MH-name }
    .suffix = FTM | Beta-7
    .desc = { ghost-role-information-mtf-cadet-description }
ent-SpawnerMTFSquadMH = { spawner-squad-name } FTM Beta-7
    .desc = { ent-SpawnerMTFSquadHD.desc }
    .suffix = Beta-7
ent-SpawnerMTFSquadMHSpawnOnTrigger = { ent-SpawnerMTFSquadMH }
    .desc = { ent-SpawnerMTFSquadMH.desc }
    .suffix = { ent-SpawnerMTFSquadMH.suffix }, { spawner-on-trigger-suffix }

# Chaves adicionadas para impedir qualquer fallback de localiza??o.

ent-IDCardMHLeader = { ent-IDCardMTFStandard }
    .desc = { ent-IDCardMTFStandard.desc }

ent-IDCardMHSpecialist = { ent-IDCardMTFStandard }
    .desc = { ent-IDCardMTFStandard.desc }

ent-IDCardMHCadet = { ent-IDCardMTFStandard }
    .desc = { ent-IDCardMTFStandard.desc }

ent-MHLeaderPDA = { ent-BaseMTFPDA }
    .desc = { ent-BaseMTFPDA.desc }

ent-MHSpecialistPDA = { ent-BaseMTFPDA }
    .desc = { ent-BaseMTFPDA.desc }

ent-MHCadetPDA = { ent-BaseMTFPDA }
    .desc = { ent-BaseMTFPDA.desc }
