ent-RandomHumanoidSpawnerNTFLeader = { ghost-role-information-mtf-leader-NTF-name }
    .suffix = FTM | Épsilon-11
    .desc = { ghost-role-information-mtf-leader-description }
ent-RandomHumanoidSpawnerNTFSpecialist = { ghost-role-information-mtf-specialist-NTF-name }
    .suffix = FTM | Épsilon-11
    .desc = { ghost-role-information-mtf-specialist-description }
ent-RandomHumanoidSpawnerNTFCadet = { ghost-role-information-mtf-cadet-NTF-name }
    .suffix = FTM | Épsilon-11
    .desc = { ghost-role-information-mtf-cadet-description }
ent-SpawnerMTFSquadNTF = { spawner-squad-name } FTM Épsilon-11
    .desc = { ent-SpawnerMTFSquadHD.desc }
    .suffix = Épsilon-11
ent-SpawnerMTFSquadNTFSpawnOnTrigger = { ent-SpawnerMTFSquadNTF }
    .desc = { ent-SpawnerMTFSquadNTF.desc }
    .suffix = { ent-SpawnerMTFSquadNTF.suffix }, { spawner-on-trigger-suffix }

# Chaves adicionadas para impedir qualquer fallback de localiza??o.

ent-IDCardNTFLeader = { ent-IDCardMTFStandard }
    .desc = { ent-IDCardMTFStandard.desc }

ent-IDCardNTFSpecialist = { ent-IDCardMTFStandard }
    .desc = { ent-IDCardMTFStandard.desc }

ent-IDCardNTFCadet = { ent-IDCardMTFStandard }
    .desc = { ent-IDCardMTFStandard.desc }

ent-NTFLeaderPDA = { ent-BaseMTFPDA }
    .desc = { ent-BaseMTFPDA.desc }

ent-NTFSpecialistPDA = { ent-BaseMTFPDA }
    .desc = { ent-BaseMTFPDA.desc }

ent-NTFCadetPDA = { ent-BaseMTFPDA }
    .desc = { ent-BaseMTFPDA.desc }
